namespace GwentApi.Classes.GwentActionResults
{
    public class BaseGwentActionResult
    {
        public GwentActionType ActionType { get; set; }
        public bool IsSuccess { get; set; } = false;
    }
}
