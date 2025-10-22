using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;
using GwentWebAssembly.Services.Interfaces;

namespace GwentWebAssembly.Services
{
    public class DataService : IDataService
    {
        public List<GwentBoardCard> CardsOnBoard { get; set; } = new();
        public List<GwentCard> CardsInHand { get; set; } = new();

        public GwentCard PlayerLeaderCard { get; set; } = new();
        public GwentCard EnemyLeaderCard { get; set; } = new();

        public int EnemyCardsCount { get; set; } = 10;//do zagrania
        public int EnemyDeckCount { get; set; } = 0;//pozostale/nieuzywane w talii
        public int EnemyUsedCardsCount { get; set; } = 0;//zuzyte

        public int PlayerDeckCount { get; set; } = 0;
        public List<GwentCard> PlayerUsedCards { get; set; } = new();

        public PlayerIdentity Turn { get; set; } = PlayerIdentity.PlayerOne;
        public int PlayerHp { get; set; } = 2;
        public int EnemyHp { get; set; } = 2;

        public int EnemyPoints { get; set; } = 0;
        public int PlayerPoints { get; set; } = 0;

        public void SetStartData(StartStatusDto startStatusDto)
        {
            Turn = startStatusDto.Turn;
            PlayerDeckCount = startStatusDto.PlayerDeckCount;
            PlayerLeaderCard = startStatusDto.PlayerLeaderCard;
            EnemyLeaderCard = startStatusDto.EnemyLeaderCard;
            CardsInHand = startStatusDto.PlayerCards;
            EnemyDeckCount = startStatusDto.EnemyDeckCount;
        }

        public void UpdateData(GameStatusDto gameStatusDto)
        {
            CardsOnBoard = gameStatusDto.CardsOnBoard;
            CardsInHand = gameStatusDto.CardsInHand;
            Turn = gameStatusDto.Turn;
            EnemyCardsCount = gameStatusDto.EnemyCardsCount;
            PlayerUsedCards = gameStatusDto.UsedCards;
            EnemyUsedCardsCount = gameStatusDto.EnemyUsedCardsCount;
            PlayerDeckCount = gameStatusDto.PlayerDeckCount;
            EnemyDeckCount = gameStatusDto.EnemyDeckCount;
            PlayerHp = gameStatusDto.PlayerHp;
            EnemyHp = gameStatusDto.EnemyHp;
        }
    }
}
