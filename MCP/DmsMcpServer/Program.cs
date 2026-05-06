using DmsMcpServer.HttpClients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<DmsApiClient>(client =>
{
    var baseUrl = builder.Configuration["DmsApi:BaseUrl"] ?? "http://localhost:5000";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithExposedHeaders("Mcp-Session-Id"));
});

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

app.UseCors();

// MCP SSE endpoint'lerini yayınla: GET /sse ve POST /message
app.MapMcp("/mcp");

app.Run();
