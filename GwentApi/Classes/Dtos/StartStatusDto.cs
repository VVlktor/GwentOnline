namespace GwentApi.Classes.Dtos
{
    public class StartStatusDto
    {
        public List<GwentCard> PlayerCards { get; set; }
        public PlayerIdentity Turn { get; set; }
        public int EnemyDeckCount { get; set; }
        public GwentLeaderCard PlayerLeaderCard { get; set; }
        public GwentLeaderCard EnemyLeaderCard { get; set; }
        public int PlayerDeckCount { get; set; }
    }
}
