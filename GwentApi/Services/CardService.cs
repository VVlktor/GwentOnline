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

            if (game.CardsOnBoard.Any(x => x.CardId == 6 && x.Placement == hornClickedDto.Placement)) return null;
            if (!playerSide.CardsInHand.Any(x => x.PrimaryId == hornClickedDto.Card.PrimaryId)) return null;

            GwentCard card = playerSide.CardsInHand.Where(x => x.PrimaryId == hornClickedDto.Card.PrimaryId).First();

            GwentBoardCard gwentBoardCard = new()
            {
                Name = card.Name,
                PrimaryId = card.PrimaryId,
                CardId = card.CardId,
                Faction = card.Faction,
                Description = card.Description,
                Placement = hornClickedDto.Placement,
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

        public async Task<LaneClickedGwentActionResult> LaneClicked(LaneClickedDto laneClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(laneClickedDto.Code);

            //if (game.Turn != laneClickedDto.Identity) return null;

            PlayerSide playerSide = game.GetPlayerSide(laneClickedDto.Identity);

            if (!playerSide.CardsInHand.Any(x => x.PrimaryId == laneClickedDto.Card.PrimaryId)) return null;

            GwentCard card = playerSide.CardsInHand.First(x => x.PrimaryId == laneClickedDto.Card.PrimaryId);

            if(card.Abilities.HasFlag(Abilities.Spy) ||
                card.Placement == TroopPlacement.Weather ||
                card.Placement == TroopPlacement.Special) return null;

            //int[] badCards = [195, 7, 6, ];

            //if (badCards.Contains(laneClickedDto.Card.CardId)) return null;

            //bool isPlacementAcceptable = laneClickedDto.Card.Placement switch
            //{
            //    TroopPlacement.Melee => (GwentLane.Melee == laneClickedDto.Lane),
            //    TroopPlacement.Range => (GwentLane.Range == laneClickedDto.Lane),
            //    TroopPlacement.Siege => (GwentLane.Siege == laneClickedDto.Lane),
            //    TroopPlacement.Agile => (GwentLane.Melee == laneClickedDto.Lane || GwentLane.Range == laneClickedDto.Lane),
            //    _ => false
            //};

            //if (!isPlacementAcceptable) return null;

            if (card.Placement != laneClickedDto.Placement) return null;

            //koniec sprawdzania

            if (card.Placement == TroopPlacement.Agile)
                card.Placement = laneClickedDto.Placement;
            
            
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
                CurrentStrength = card.Strength,
                Owner = laneClickedDto.Identity
            };

            GwentActionType gwentActionType = GwentActionType.NormalCardPlayed;
            if (card.Abilities.HasFlag(Abilities.Medic))
            {
                gwentActionType = GwentActionType.MedicCardPlayed;
                //mediko
            }
            else if (card.Abilities.HasFlag(Abilities.Muster))
            {
                gwentActionType = GwentActionType.MusterCardPlayed;

            }
            else if (card.Abilities.HasFlag(Abilities.Scorch) ||
                card.Abilities.HasFlag(Abilities.ScorchMelee) ||
                card.Abilities.HasFlag(Abilities.ScorchSiege) ||
                card.Abilities.HasFlag(Abilities.ScorchRange))
            {
                gwentActionType = GwentActionType.ScorchCardPlayed;//moze bedzie trzeba rozdzielic scorch jako karte od kart z abilitką scorch, sie zobaczyc

            }
            else{
                //normalne - przemyslec jak rozwiazac sprawe np. jaskra - niby animacja ale karta zagrywana jak kazda inna
                //edit: w gwenthub napisalem co i jak
            }

            playerSide.CardsInHand.Remove(card);
            game.CardsOnBoard.Add(boardCard);

            await _gameRepository.UpdateGame(game);

            return new()
            {
                ActionType = gwentActionType,
                KilledCards=new(),
                PlayedCards = new() { boardCard }
            };
        }
    }
}
