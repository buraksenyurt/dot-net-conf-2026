using System.ClientModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OpenAI;

namespace InventoryMcpClient.Services;

public sealed class McpChatService : IMcpChatService, IHostedService, IAsyncDisposable
{
    private const int MaxIterations      = 3;
    private const int MaxToolResultChars = 2000;
    private const int MaxRelevantTools   = 4;

    private readonly IConfiguration _config;
    private readonly ILogger<McpChatService> _logger;

    private McpClient? _mcpClient;
    private IChatClient? _chatClient;
    private IList<McpClientTool> _tools = [];

    private string SystemPrompt =>
        _config["McpClient:SystemPrompt"] ??
        "You are a DMS (Dealer Management System) assistant. Use the available tools to help the user.";

    public McpChatService(IConfiguration config, ILogger<McpChatService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        var mcpServerUrl = _config["McpClient:McpServerUrl"] ?? "http://localhost:5290/mcp";
        var lmStudioUrl  = _config["McpClient:LmStudioUrl"]  ?? "http://localhost:1234/v1";
        var chatModel    = _config["McpClient:ChatModel"]    ?? "meta-llama-3.1-8b-instruct";

        var transport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint      = new Uri(mcpServerUrl),
            TransportMode = HttpTransportMode.StreamableHttp
        });

        _mcpClient = await McpClient.CreateAsync(transport, cancellationToken: ct);
        _tools     = await _mcpClient.ListToolsAsync(cancellationToken: ct);

        _logger.LogInformation(
            "MCP: connected to {Url}. {Count} tools: {Names}",
            mcpServerUrl, _tools.Count, string.Join(", ", _tools.Select(t => t.Name)));

        var openAiClient = new OpenAIClient(
            new ApiKeyCredential("lm-studio"),
            new OpenAIClientOptions { Endpoint = new Uri(lmStudioUrl) });

        _chatClient = openAiClient.GetChatClient(chatModel).AsIChatClient();

        _logger.LogInformation("LLM chat client ready. Model: {Model}", chatModel);
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;

    public async IAsyncEnumerable<ChatStreamEvent> StreamChatAsync(
        string prompt,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        if (_chatClient is null)
        {
            yield return new ChatStreamEvent("error", "Service not yet initialized. Please retry in a moment.");
            yield return new ChatStreamEvent("done", string.Empty);
            yield break;
        }

        var relevantTools = SelectRelevantTools(prompt);
        _logger.LogInformation("Selected tools: {Tools}", string.Join(", ", relevantTools.Select(t => t.Name)));

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, SystemPrompt),
            new(ChatRole.User,   prompt)
        };

        var optionsWithTools = new ChatOptions
        {
            Tools    = [.. relevantTools],
            ToolMode = ChatToolMode.Auto
        };

        var optionsNoTools = new ChatOptions
        {
            Tools    = [],
            ToolMode = ChatToolMode.None
        };

        bool firstIteration = true;

        for (int iteration = 0; iteration < MaxIterations; iteration++)
        {
            var currentOptions = firstIteration ? optionsWithTools : optionsNoTools;
            var buffer = new List<ChatResponseUpdate>();

            await foreach (var update in _chatClient.GetStreamingResponseAsync(messages, currentOptions, ct))
            {
                if (update.Text is { Length: > 0 } text)
                    yield return new ChatStreamEvent("token", text);

                buffer.Add(update);
            }

            var response = buffer.ToChatResponse();

            if (!firstIteration)
                break;

            var toolCalls = response.Messages
                                    .SelectMany(m => m.Contents.OfType<FunctionCallContent>())
                                    .Where(tc => !tc.InformationalOnly)
                                    .ToList();

            if (toolCalls.Count == 0)
                break;

            foreach (var msg in response.Messages)
                messages.Add(msg);

            var toolResults = new List<AIContent>();
            foreach (var call in toolCalls)
            {
                var toolName = call.Name ?? "unknown";
                _logger.LogInformation("Tool call: {ToolName}", toolName);
                yield return new ChatStreamEvent("tool", toolName);

                var resultText = await InvokeToolAsync(call, ct);
                toolResults.Add(new FunctionResultContent(call.CallId ?? toolName, resultText));
            }

            messages.Add(new ChatMessage(ChatRole.Tool, toolResults));
            firstIteration = false;
        }

        yield return new ChatStreamEvent("done", string.Empty);
    }

    private async Task<string> InvokeToolAsync(FunctionCallContent call, CancellationToken ct)
    {
        try
        {
            var tool = _tools.FirstOrDefault(t => t.Name == call.Name);
            if (tool is null)
                return $"Tool '{call.Name}' not found on MCP server.";

            var funcArgs = new AIFunctionArguments();
            if (call.Arguments is not null)
                foreach (var kvp in call.Arguments)
                    funcArgs[kvp.Key] = kvp.Value;

            var raw        = await tool.InvokeAsync(funcArgs, ct);
            var resultText = raw?.ToString() ?? string.Empty;

            if (resultText.Length > MaxToolResultChars)
            {
                _logger.LogWarning(
                    "Tool result for '{Tool}' truncated from {Len} to {Max} chars.",
                    call.Name, resultText.Length, MaxToolResultChars);
                resultText = resultText[..MaxToolResultChars]
                             + $"\n... [ilk {MaxToolResultChars} karakter gösteriliyor, sonuçlar kesildi]";
            }

            return resultText;
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tool call failed: {ToolName}", call.Name);
            return $"Araç hatası ({call.Name}): {ex.Message}";
        }
    }

    private IList<McpClientTool> SelectRelevantTools(string prompt)
    {
        var lower = prompt.ToLowerInvariant();

        (string Keyword, string[] ToolNames)[] mapping =
        [
            ("araç",      ["list_vehicles", "add_vehicle", "validate_vin"]),
            ("araba",     ["list_vehicles", "add_vehicle"]),
            ("stok",      ["list_vehicles"]),
            ("envanter",  ["list_vehicles", "add_vehicle"]),
            ("vin",       ["validate_vin",  "add_vehicle"]),
            ("müşteri",   ["list_customers", "register_customer"]),
            ("kayıt",     ["register_customer"]),
            ("opsiyon",   ["create_option", "cancel_option", "get_advisor_dashboard"]),
            ("rezerv",    ["create_option"]),
            ("iptal",     ["cancel_option"]),
            ("dashboard", ["get_advisor_dashboard"]),
            ("danışman",  ["get_advisor_dashboard"]),
            ("hikaye",    ["get_user_story"]),
            ("entity",    ["get_domain_entity"]),
            ("domain",    ["get_domain_entity"]),
            ("adr",       ["list_adrs", "get_adr"]),
            ("mimari",    ["list_adrs", "get_adr"]),
        ];

        var scores = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var (keyword, toolNames) in mapping)
        {
            if (!lower.Contains(keyword)) continue;
            foreach (var name in toolNames)
                scores[name] = scores.GetValueOrDefault(name) + 1;
        }

        var selected = _tools
            .Where(t => scores.ContainsKey(t.Name))
            .OrderByDescending(t => scores[t.Name])
            .Take(MaxRelevantTools)
            .ToList();

        if (selected.Count > 0)
            return selected;

        _logger.LogWarning("No keyword match for tool selection; using default tool set.");
        string[] defaults = ["list_vehicles", "list_customers", "get_advisor_dashboard", "validate_vin"];
        return _tools.Where(t => defaults.Contains(t.Name)).ToList();
    }

    public async ValueTask DisposeAsync()
    {
        _chatClient?.Dispose();
        if (_mcpClient is IAsyncDisposable d)
            await d.DisposeAsync();
    }
}
