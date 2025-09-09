namespace GwentWebAssembly.Data.Dtos
{
    public class StartStatusDto
    {
        public List<GwentBoardCard> PlayerCards { get; set; }
        public PlayerIdentity Turn;
        public int EnemyCardsCount { get; set; }
        public GwentCard PlayerLeaderCard { get; set; }
        public GwentCard EnemyLeaderCard { get; set; }

    }
}
