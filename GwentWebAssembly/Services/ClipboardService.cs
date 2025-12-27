using GwentWebAssembly.Services.Interfaces;
using Microsoft.JSInterop;

namespace GwentWebAssembly.Services;

public class ClipboardService : IClipboardService
{
    private IJSRuntime _jsRuntime;

    public ClipboardService(IJSRuntime js)
    {
        _jsRuntime = js;
    }

    public async Task CopyToClipboard(string text) => await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
}
