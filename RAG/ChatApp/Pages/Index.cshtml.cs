using ChatApp.Settings;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ChatApp.Pages;

public class IndexModel : PageModel
{
    public string ChatModel { get; private set; } = "";
    public string CollectionName { get; private set; } = "";

    public IndexModel(IOptions<ChatAppSettings> options)
    {
        ChatModel = options.Value.ChatModel;
        CollectionName = options.Value.CollectionName;
    }

    public void OnGet() { }
}
