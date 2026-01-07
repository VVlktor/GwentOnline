using GwentShared.Classes;

namespace GwentApi.Classes.GwentActionResults
{
    public class EnemyLaneClickedGwentActionResult : BaseGwentActionResult
    {
        public GwentBoardCard PlayedCard { get; set; }
        public List<GwentCard> DrawnCards { get; set; }
    }
}
