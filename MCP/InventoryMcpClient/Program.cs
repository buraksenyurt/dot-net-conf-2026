using System.Text.Json;
using InventoryMcpClient.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddSingleton<McpChatService>();
builder.Services.AddSingleton<IMcpChatService>(sp => sp.GetRequiredService<McpChatService>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<McpChatService>());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.MapGet("/api/chat/stream", async (
    string?          q,
    IMcpChatService  chatService,
    HttpContext      ctx,
    CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(q))
    {
        ctx.Response.StatusCode = 400;
        return;
    }

    ctx.Response.ContentType       = "text/event-stream";
    ctx.Response.Headers.CacheControl = "no-cache";
    ctx.Response.Headers["X-Accel-Buffering"] = "no";

    await foreach (var evt in chatService.StreamChatAsync(q, ct))
    {
        var json = JsonSerializer.Serialize(evt);
        await ctx.Response.WriteAsync($"data: {json}\n\n", ct);
        await ctx.Response.Body.FlushAsync(ct);
    }
});

app.Run();
