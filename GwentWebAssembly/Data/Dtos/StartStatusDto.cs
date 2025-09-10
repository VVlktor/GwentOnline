namespace GwentWebAssembly.Data.Dtos
{
    public class StartStatusDto
    {
        public List<GwentCard> PlayerCards { get; set; }
        public PlayerIdentity Turn { get; set; }
        public int EnemyCartsCount { get; set; }
        public int EnemyDeckCount { get; set; }
        public GwentCard PlayerLeaderCard { get; set; }
        public GwentCard EnemyLeaderCard { get; set; }
        public int PlayerDeckCount { get; set; }

    }
}
