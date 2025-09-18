using GwentApi.Classes;
using GwentApi.Classes.Dtos;
using GwentApi.Repository.Interfaces;
using GwentApi.Services.Interfaces;

namespace GwentApi.Services
{
    public class CardService : ICardService
    {
        private IGameRepository _gameRepository;

        public CardService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<GwentBoardCard> HornClicked(HornClickedDto hornClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(hornClickedDto.Code);

            if (game.Turn != hornClickedDto.Identity) return null;

            PlayerSide playerSide = game.GetPlayerSide(hornClickedDto.Identity);

            if (game.CardsOnBoard.Any(x => x.PrimaryId == 6 && x.Placement == hornClickedDto.Placement)) return null;
            if (!playerSide.CardsInHand.Any(x => x.PrimaryId == hornClickedDto.Card.PrimaryId)) return null;

            GwentCard card = playerSide.CardsInHand.Where(x => x.PrimaryId == hornClickedDto.Card.PrimaryId).First();

            GwentBoardCard gwentBoardCard = new()
            {
                Name = card.Name,
                PrimaryId = card.PrimaryId,
                CardId = card.CardId,
                Faction = card.Faction,
                Description = card.Description,
                Placement = card.Placement,
                Strength = card.Strength,
                Abilities = card.Abilities,
                CurrentStrength = 0,
                Owner = hornClickedDto.Identity
            };

            playerSide.CardsInHand.Remove(card);
            game.CardsOnBoard.Add(gwentBoardCard);

            await _gameRepository.UpdateGame(game);

            return gwentBoardCard;
        }

        public async Task<GwentBoardCard> LaneClicked(LaneClickedDto laneClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(laneClickedDto.Code);

            if (game.Turn != laneClickedDto.Identity) return null;

            PlayerSide playerSide = game.GetPlayerSide(laneClickedDto.Identity);

            if (!playerSide.CardsInHand.Any(x => x.PrimaryId == laneClickedDto.Card.PrimaryId)) return null;

            if (laneClickedDto.Card.Abilities.HasFlag(Abilities.Medic) ||
               laneClickedDto.Card.Abilities.HasFlag(Abilities.Spy) ||
               laneClickedDto.Card.Placement == TroopPlacement.Weather ||
               laneClickedDto.Card.Placement == TroopPlacement.Special) return null;

            //int[] badCards = [195, 7, 6, ];

            //if (badCards.Contains(laneClickedDto.Card.CardId)) return null;

            bool isPlacementAcceptable = laneClickedDto.Card.Placement switch
            {
                TroopPlacement.Melee => (GwentLane.Melee == laneClickedDto.Lane),
                TroopPlacement.Range => (GwentLane.Range == laneClickedDto.Lane),
                TroopPlacement.Siege => (GwentLane.Siege == laneClickedDto.Lane),
                TroopPlacement.Agile => (GwentLane.Melee == laneClickedDto.Lane || GwentLane.Range == laneClickedDto.Lane),
                _ => false
            };

            if (!isPlacementAcceptable) return null;

            //koniec sprawdzania
            GwentCard card = playerSide.CardsInHand.First(x => x.PrimaryId == laneClickedDto.Card.PrimaryId);

            if (card.Placement == TroopPlacement.Agile)
                card.Placement = laneClickedDto.Lane == GwentLane.Melee ? TroopPlacement.Melee : TroopPlacement.Range;

            GwentBoardCard boardCard = new()
            {
                Name = card.Name,
                PrimaryId = card.PrimaryId,
                CardId = card.CardId,
                Faction = card.Faction,
                Description = card.Description,
                Placement = card.Placement,
                Strength = card.Strength,
                Abilities = card.Abilities,
                CurrentStrength = card.Strength,//solved?: zalezy od podejscia, ale bedzie trzeba to zmienic tak czy inaczej - nie wiem jeszcze kiedy bede liczyl sily jednostek (pewnie tuz przed zwroceniem w hubie)
                Owner = laneClickedDto.Identity
            };

            playerSide.CardsInHand.Remove(card);
            game.CardsOnBoard.Add(boardCard);//robione przy mega spanku, sprawdzic czy jest git



            await _gameRepository.UpdateGame(game);

            return boardCard;
        }
    }
}
