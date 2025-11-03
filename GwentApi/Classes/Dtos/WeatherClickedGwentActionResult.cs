using GwentApi.Classes.GwentActionResults;

namespace GwentApi.Classes.Dtos
{
    public class WeatherClickedGwentActionResult : BaseGwentActionResult
    {
        public GwentBoardCard PlayedCard { get; set; }
        public List<GwentBoardCard> RemovedCards { get; set; }
    }
}
