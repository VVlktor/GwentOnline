namespace GwentShared.Classes;

public class ResponseData
{
    public ResponseData()
    {
        
    }

    public ResponseData(bool isValid, string message)
    {
        IsValid = isValid;
        Message = message;
    }

    public bool IsValid { get; set; }
    public string Message { get; set; }
}
