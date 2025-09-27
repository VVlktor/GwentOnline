namespace GwentApi.Classes.Dtos
{
    public class WeatherClickedGwentActionResult
    {
        public GwentActionType ActionType { get; set; }
        public GwentBoardCard PlayedCard { get; set; }
        public List<GwentBoardCard> RemovedCards { get; set; }
    }
}
