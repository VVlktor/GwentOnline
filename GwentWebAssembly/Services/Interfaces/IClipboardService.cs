namespace GwentWebAssembly.Services.Interfaces;

public interface IClipboardService
{
    Task CopyToClipboard(string text);
}
