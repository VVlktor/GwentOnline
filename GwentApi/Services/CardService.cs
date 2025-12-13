using GwentApi.Classes;
using GwentApi.Classes.GwentActionResults;
using GwentApi.Extensions;
using GwentApi.Repository.Interfaces;
using GwentApi.Services.Interfaces;
using GwentShared.Classes;
using GwentShared.Classes.Dtos;

namespace GwentApi.Services
{
    public class CardService : ICardService
    {
        private IGameRepository _gameRepository;
        private IGameDataService _gameDataService;
        private CardsProvider _cardsProvider;
        private ICardServiceValidator _cardServiceValidator;

        public CardService(IGameRepository gameRepository, IGameDataService gameDataService, CardsProvider cardsProvider, ICardServiceValidator cardServiceValidator)
        {
            _cardsProvider = cardsProvider;
            _gameRepository = gameRepository;
            _gameDataService = gameDataService;
            _cardServiceValidator = cardServiceValidator;
        }

        public async Task<HornClickedGwentActionResult> HornClicked(HornClickedDto hornClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(hornClickedDto.Code);
            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, hornClickedDto.Identity);

            if (!_cardServiceValidator.ValidateHorn(game, hornClickedDto)) return new();

            GwentCard card = playerSide.CardsInHand.Where(x => x.PrimaryId == hornClickedDto.Card.PrimaryId).First();

            card.Placement = hornClickedDto.Placement;

            GwentBoardCard gwentBoardCard = _cardsProvider.CreateGwentBoardCard(card, hornClickedDto.Identity);

            playerSide.CardsInHand.Remove(card);
            game.CardsOnBoard.Add(gwentBoardCard);

            await _gameRepository.UpdateGame(game);

            return new()
            {
                ActionType = GwentActionType.CommandersHornCardPlayed,
                GwentBoardCard = gwentBoardCard,
                IsSuccess= true
            };
        }

        public async Task<LaneClickedGwentActionResult> LaneClicked(LaneClickedDto laneClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(laneClickedDto.Code);
            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, laneClickedDto.Identity);

            if (!_cardServiceValidator.ValidateLane(game, laneClickedDto)) return new();

            PlayerSide enemySide = _gameDataService.GetEnemySide(game, laneClickedDto.Identity);
            GwentCard card = playerSide.CardsInHand.First(x => x.PrimaryId == laneClickedDto.Card.PrimaryId);

            if (card.Placement == TroopPlacement.Agile)
                card.Placement = laneClickedDto.Placement;

            GwentBoardCard boardCard = _cardsProvider.CreateGwentBoardCard(card, laneClickedDto.Identity);

            GwentActionType gwentActionType = GwentActionType.NormalCardPlayed;

            if (card.Abilities.HasFlag(Abilities.Medic) && playerSide.UsedCards.Any(x => !x.Abilities.HasFlag(Abilities.Hero) && x.Placement != TroopPlacement.Weather && x.Placement != TroopPlacement.Special && x.CardId != 2 && x.CardId != 6))
                gwentActionType = GwentActionType.MedicCardPlayed;
            else if (card.Abilities.HasFlag(Abilities.Muster))
                gwentActionType = GwentActionType.MusterCardPlayed;
            else if (card.Abilities.HasFlag(Abilities.Scorch) ||
                card.Abilities.HasFlag(Abilities.ScorchMelee) ||
                card.Abilities.HasFlag(Abilities.ScorchSiege) ||
                card.Abilities.HasFlag(Abilities.ScorchRange))
            {
                gwentActionType = GwentActionType.ScorchBoardCardPlayed;
            }

            LaneClickedGwentActionResult actionResult = new()
            {
                ActionType = gwentActionType,
                KilledCards = new(),
                PlayedCards = new() { boardCard },
                IsSuccess = true
            };

            Random rnd = new();

            if (gwentActionType == GwentActionType.ScorchBoardCardPlayed)
            {
                var result = ScorchBoardCardPlayed(game, boardCard, laneClickedDto.Identity);
                actionResult.KilledCards = result.Item1;
                actionResult.ActionType = result.Item2;
            }
            else if (gwentActionType == GwentActionType.MusterCardPlayed)
            {
                var musterCards = MusterCardPlayed(game, boardCard, laneClickedDto.Identity);
                if (musterCards.Count() == 0)
                    actionResult.ActionType = GwentActionType.NormalCardPlayed;
                else
                    actionResult.PlayedCards.AddRange(musterCards);
            }
            else if (gwentActionType == GwentActionType.MedicCardPlayed)//obecnie nie zaimplementowane do konca
            {
                if ((playerSide.LeaderCard.CardId == 61 && playerSide.LeaderCard.LeaderActive) || (enemySide.LeaderCard.CardId == 61 && enemySide.LeaderCard.LeaderActive))
                {
                    GwentCard cardToRev = playerSide.UsedCards.Where(x => !x.Abilities.HasFlag(Abilities.Hero) && x.Placement != TroopPlacement.Weather).ToList()[rnd.Next(playerSide.UsedCards.Count)];
                    GwentBoardCard boardCardToRev = _cardsProvider.CreateGwentBoardCard(cardToRev, laneClickedDto.Identity);
                    actionResult.ActionType = GwentActionType.MusterCardPlayed;
                    actionResult.PlayedCards.Add(boardCardToRev);
                    game.CardsOnBoard.Add(boardCardToRev);
                }
            }

            playerSide.CardsInHand.Remove(card);
            game.CardsOnBoard.Add(boardCard);

            await _gameRepository.UpdateGame(game);

            return actionResult;
        }

        public async Task<CardClickedGwentActionResult> CardClicked(CardClickedDto cardClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(cardClickedDto.Code);
            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, cardClickedDto.Identity);

            if (!_cardServiceValidator.ValidateCard(game, cardClickedDto)) return new();

            GwentCard card = playerSide.CardsInHand.First(x => x.PrimaryId == cardClickedDto.SelectedCard.PrimaryId);
            GwentBoardCard clickedCard = game.CardsOnBoard.First(x => x.PrimaryId == cardClickedDto.ClickedCard.PrimaryId);

            card.Placement = clickedCard.Placement;
            GwentBoardCard decoyCard = _cardsProvider.CreateGwentBoardCard(card, cardClickedDto.Identity);

            playerSide.CardsInHand.Remove(card);
            game.CardsOnBoard.Remove(clickedCard);

            GwentCard cardToHand = new()
            {
                Name = clickedCard.Name,
                PrimaryId = clickedCard.PrimaryId,
                CardId = clickedCard.CardId,
                Faction = clickedCard.Faction,
                Placement = clickedCard.Placement,
                Strength = clickedCard.Strength,
                Abilities = clickedCard.Abilities,
                FileName = clickedCard.FileName
            };

            playerSide.CardsInHand.Add(cardToHand);
            game.CardsOnBoard.Add(decoyCard);

            await _gameRepository.UpdateGame(game);

            return new()
            {
                ActionType = GwentActionType.DecoyCardPlayed,
                SwappedCard = clickedCard,
                PlayedCard = decoyCard,
                IsSuccess = true
            };
        }

        public async Task<WeatherClickedGwentActionResult> WeatherClicked(WeatherClickedDto weatherClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(weatherClickedDto.Code);
            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, weatherClickedDto.Identity);

            if (!_cardServiceValidator.ValidateWeather(game, weatherClickedDto)) return new();

            GwentCard card = playerSide.CardsInHand.First(x => x.PrimaryId == weatherClickedDto.Card.PrimaryId);
            GwentBoardCard boardCard = _cardsProvider.CreateGwentBoardCard(card, weatherClickedDto.Identity);

            if (card.CardId == 11)//id karty Scorch
            {
                var nonHeroes = game.CardsOnBoard.Where(x => !x.Abilities.HasFlag(Abilities.Hero));

                WeatherClickedGwentActionResult scorchActionResult = new()
                {
                    ActionType = GwentActionType.ScorchCardPlayed,
                    RemovedCards = new(),
                    PlayedCard = boardCard,
                    IsSuccess = true
                };

                if (nonHeroes.Any())
                {
                    int maxCurrentSstrength = nonHeroes.Max(x => x.CurrentStrength);
                    var strongestCards = nonHeroes.Where(x => x.CurrentStrength == maxCurrentSstrength).ToList();

                    scorchActionResult.RemovedCards = strongestCards;

                    foreach (var strongCard in strongestCards)
                    {
                        game.CardsOnBoard.Remove(strongCard);
                        _gameDataService.GetPlayerSide(game, strongCard.Owner).UsedCards.Add(strongCard);
                    }

                }

                playerSide.CardsInHand.Remove(card);

                await _gameRepository.UpdateGame(game);

                return scorchActionResult;
            }

            if (card.CardId == 5)//id karty Clear Weather
            {
                List<GwentBoardCard> weatherCards = game.CardsOnBoard.Where(x => x.Placement == TroopPlacement.Weather).ToList();
                foreach (var weatherCard in weatherCards)
                {
                    game.CardsOnBoard.Remove(weatherCard);
                    playerSide.UsedCards.Add(weatherCard);
                }

                playerSide.CardsInHand.Remove(card);

                await _gameRepository.UpdateGame(game);

                WeatherClickedGwentActionResult clearWeatherActionResult = new()
                {
                    ActionType = GwentActionType.WeatherCardPlayed,
                    RemovedCards = weatherCards,
                    PlayedCard = boardCard,
                    IsSuccess = true
                };

                return clearWeatherActionResult;
            }

            playerSide.CardsInHand.Remove(card);
            game.CardsOnBoard.Add(boardCard);

            await _gameRepository.UpdateGame(game);

            WeatherClickedGwentActionResult weatherActionResult = new()
            {
                ActionType = GwentActionType.WeatherCardPlayed,
                RemovedCards = new(),
                PlayedCard = boardCard,
                IsSuccess = true
            };

            return weatherActionResult;
        }

        public async Task<EnemyLaneClickedGwentActionResult> EnemyLaneClicked(EnemyLaneClickedDto enemyLaneClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(enemyLaneClickedDto.Code);

            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, enemyLaneClickedDto.Identity);

            if (!_cardServiceValidator.ValidateEnemyLane(game, enemyLaneClickedDto)) return new();

            GwentCard card = playerSide.CardsInHand.First(x => x.PrimaryId == enemyLaneClickedDto.Card.PrimaryId);

            GwentBoardCard boardCard = _cardsProvider.CreateGwentBoardCard(card, enemyLaneClickedDto.Identity.GetEnemy());

            playerSide.CardsInHand.Remove(card);
            game.CardsOnBoard.Add(boardCard);

            List<GwentCard> drawnCards = new();
            switch (playerSide.Deck.Count)
            {
                case >= 2:
                    drawnCards = [playerSide.Deck[0], playerSide.Deck[1]];
                    playerSide.Deck.RemoveRange(0, 2);
                    break;

                case 1:
                    drawnCards = [playerSide.Deck[0]];
                    playerSide.Deck.RemoveAt(0);
                    break;
            }

            playerSide.CardsInHand.AddRange(drawnCards);

            await _gameRepository.UpdateGame(game);

            EnemyLaneClickedGwentActionResult actionResult = new()
            {
                ActionType = GwentActionType.SpyCardPlayed,
                PlayedCard = boardCard,
                IsSuccess = true
            };

            return actionResult;
        }

        public async Task<PassClickedGwentActionResult> PassClicked(PassClickedDto passClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(passClickedDto.Code);

            if (_cardServiceValidator.ValidatePass(game, passClickedDto)) return new();

            if (passClickedDto.Identity == PlayerIdentity.PlayerOne)
                game.HasPassed = (true, game.HasPassed.PlayerTwo);
            else
                game.HasPassed = (game.HasPassed.PlayerOne, true);

            PassClickedGwentActionResult actionResult = new()
            {
                ActionType = GwentActionType.Pass,
                Passed = true,
                IsSuccess = true
            };

            await _gameRepository.UpdateGame(game);

            return actionResult;
        }

        public async Task<LeaderClickedGwentActionResult> LeaderClicked(LeaderClickedDto leaderClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(leaderClickedDto.Code);

            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, leaderClickedDto.Identity);
            PlayerSide enemySide = _gameDataService.GetEnemySide(game, leaderClickedDto.Identity);

            if (!_cardServiceValidator.ValidateLeader(game, leaderClickedDto)) return new();

            int identityOffset = leaderClickedDto.Identity == PlayerIdentity.PlayerOne ? 1 : 2;

            if (playerSide.LeaderCard.CardId == 24)
            {
                GwentCard clearCard = _cardsProvider.GetCardByCardId(5);
                clearCard.PrimaryId = 200 + 10 + identityOffset;

                GwentBoardCard clearBoardCard = _cardsProvider.CreateGwentBoardCard(clearCard, leaderClickedDto.Identity);

                List<GwentBoardCard> weatherCards = game.CardsOnBoard.Where(x => x.Placement == TroopPlacement.Weather).ToList();

                foreach (var weatherCard in weatherCards)
                    game.CardsOnBoard.Remove(weatherCard);

                playerSide.LeaderCard.LeaderAvailable = false;

                await _gameRepository.UpdateGame(game);

                return new()
                {
                    RemovedCards = weatherCards,
                    ActionType = GwentActionType.WeatherCardPlayed,
                    PlayedCard = clearBoardCard,
                    IsSuccess = true
                };
            }

            if (playerSide.LeaderCard.CardId == 23 || playerSide.LeaderCard.CardId == 57 || playerSide.LeaderCard.CardId == 143)
            {
                GwentCard weatherCard = _cardsProvider.GetCardByCardId(playerSide.LeaderCard.CardId == 23 ? 10 : playerSide.LeaderCard.CardId == 57 ? 12 : 3);
                weatherCard.PrimaryId = 200 + 20 + identityOffset;

                GwentBoardCard weatherBoardCard = _cardsProvider.CreateGwentBoardCard(weatherCard, leaderClickedDto.Identity);

                game.CardsOnBoard.Add(weatherBoardCard);

                playerSide.LeaderCard.LeaderAvailable = false;

                await _gameRepository.UpdateGame(game);

                return new()
                {
                    ActionType = GwentActionType.WeatherCardPlayed,
                    PlayedCard = weatherBoardCard,
                    RemovedCards = [],
                    IsSuccess = true
                };
            }

            if (playerSide.LeaderCard.CardId == 25 || playerSide.LeaderCard.CardId == 141 || playerSide.LeaderCard.CardId == 94)
            {
                TroopPlacement placement = playerSide.LeaderCard.CardId == 25 ? TroopPlacement.Siege : playerSide.LeaderCard.CardId == 141 ? TroopPlacement.Range : TroopPlacement.Melee;
                if (game.CardsOnBoard.Any(x => x.CardId == 6 && x.Placement == placement)) return new();//trzeba sprawdzic czy wtedy pozwala a abilitka sie nie marnuje, moze to jest overprotective z mojej strony

                GwentCard hornCard = _cardsProvider.GetCardByCardId(6);
                hornCard.PrimaryId = 200 + 30 + identityOffset;
                hornCard.Placement = placement;

                GwentBoardCard hornBoardCard = _cardsProvider.CreateGwentBoardCard(hornCard, leaderClickedDto.Identity);

                game.CardsOnBoard.Add(hornBoardCard);
                playerSide.LeaderCard.LeaderAvailable = false;

                await _gameRepository.UpdateGame(game);

                return new()
                {
                    ActionType = GwentActionType.CommandersHornCardPlayed,
                    PlayedCard = hornBoardCard,
                    RemovedCards = [],
                    IsSuccess = true
                };
            }

            if (playerSide.LeaderCard.CardId == 27 || playerSide.LeaderCard.CardId == 26 || playerSide.LeaderCard.CardId == 140)
            {
                TroopPlacement placement = playerSide.LeaderCard.CardId == 26 ? TroopPlacement.Siege : playerSide.LeaderCard.CardId == 27 ? TroopPlacement.Range : TroopPlacement.Melee;
                var rowCards = game.CardsOnBoard.Where(x => x.Placement == placement && x.Owner == leaderClickedDto.Identity.GetEnemy());
                List<GwentBoardCard> KilledCards = new();
                if (rowCards.Sum(x => x.CurrentStrength) >= 10 && rowCards.Any(x => !x.Abilities.HasFlag(Abilities.Hero)))
                {
                    int maxCurrentSstrength = rowCards.Where(x => !x.Abilities.HasFlag(Abilities.Hero)).Max(x => x.CurrentStrength);
                    var strongestCards = rowCards.Where(x => x.CurrentStrength == maxCurrentSstrength).ToList();
                    KilledCards = strongestCards;
                    foreach (var strongCard in strongestCards)
                    {
                        game.CardsOnBoard.Remove(strongCard);
                        _gameDataService.GetPlayerSide(game, strongCard.Owner).UsedCards.Add(strongCard);
                    }
                }

                playerSide.LeaderCard.LeaderAvailable = false;

                await _gameRepository.UpdateGame(game);

                return new()
                {
                    ActionType = GwentActionType.ScorchCardPlayed,
                    RemovedCards = KilledCards,
                    IsSuccess = true,
                };
            }

            int[] justChangeStatus = [98];//61

            if (justChangeStatus.Contains(playerSide.LeaderCard.CardId))
            {
                playerSide.LeaderCard.LeaderActive = true;
                playerSide.LeaderCard.LeaderAvailable = false;

                return new()
                {
                    ActionType = GwentActionType.LeaderCardPlayed,
                    RemovedCards = [],
                    IsSuccess = true
                };
            }


            throw new NotImplementedException($"{playerSide.LeaderCard.CardId}");
        }

        public async Task<CarouselCardClickedGwentActionResult> CarouselCardClicked(CarouselCardClickedDto carouselCardClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(carouselCardClickedDto.Code);

            if (game.Turn != carouselCardClickedDto.Identity) return new();

            GwentAction lastGwentAction = game.Actions.Last();

            if (!lastGwentAction.AbilitiyUsed.HasFlag(Abilities.Medic) || lastGwentAction.Issuer != carouselCardClickedDto.Identity) return new();

            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, carouselCardClickedDto.Identity);

            if (carouselCardClickedDto.Card.CardId == 2) return new();

            GwentCard? card = playerSide.UsedCards.FirstOrDefault(x => x.PrimaryId == carouselCardClickedDto.Card.PrimaryId);

            if (card is null) return new();

            GwentBoardCard boardCard = _cardsProvider.CreateGwentBoardCard(card, carouselCardClickedDto.Identity);

            if (card.Placement == TroopPlacement.Agile)
                card.Placement = TroopPlacement.Melee;

            GwentActionType gwentActionType = GwentActionType.NormalCardPlayed;

            if (card.Abilities.HasFlag(Abilities.Spy))
            {
                boardCard.Owner = carouselCardClickedDto.Identity.GetEnemy();//TUTAJ WROCIC
                gwentActionType = GwentActionType.SpyCardPlayed;

                playerSide.UsedCards.Remove(card);
                game.CardsOnBoard.Add(boardCard);

                List<GwentCard> drawnCards = new();
                switch (playerSide.Deck.Count)
                {
                    case >= 2:
                        drawnCards = [playerSide.Deck[0], playerSide.Deck[1]];
                        playerSide.Deck.RemoveRange(0, 2);
                        break;

                    case 1:
                        drawnCards = [playerSide.Deck[0]];
                        playerSide.Deck.RemoveAt(0);
                        break;
                }

                playerSide.CardsInHand.AddRange(drawnCards);

                await _gameRepository.UpdateGame(game);

                CarouselCardClickedGwentActionResult spyActionResult = new()
                {
                    ActionType = GwentActionType.SpyCardPlayed,
                    PlayedCards = new() { boardCard },
                    KilledCards = new(),
                    IsSuccess = true
                };

                return spyActionResult;
            }
            else if (card.Abilities.HasFlag(Abilities.Medic))
                gwentActionType = GwentActionType.MedicCardPlayed;
            else if (card.Abilities.HasFlag(Abilities.Muster))
                gwentActionType = GwentActionType.MusterCardPlayed;
            else if (card.Abilities.HasFlag(Abilities.Scorch) ||
                card.Abilities.HasFlag(Abilities.ScorchMelee) ||
                card.Abilities.HasFlag(Abilities.ScorchSiege) ||
                card.Abilities.HasFlag(Abilities.ScorchRange))
            {
                gwentActionType = GwentActionType.ScorchBoardCardPlayed;
            }

            CarouselCardClickedGwentActionResult actionResult = new()
            {
                ActionType = gwentActionType,
                KilledCards = new(),
                PlayedCards = new() { boardCard },
                IsSuccess = true
            };

            if (gwentActionType == GwentActionType.ScorchBoardCardPlayed)
            {
                var result = ScorchBoardCardPlayed(game, boardCard, carouselCardClickedDto.Identity);
                actionResult.KilledCards = result.Item1;
                actionResult.ActionType = result.Item2;
            }

            playerSide.UsedCards.Remove(card);
            game.CardsOnBoard.Add(boardCard);

            await _gameRepository.UpdateGame(game);

            return actionResult;
        }

        private (List<GwentBoardCard>, GwentActionType) ScorchBoardCardPlayed(Game game, GwentBoardCard boardCard, PlayerIdentity identity)
        {
            //139 - toad - range
            //214 schirru - siege
            //15 - viller - melee
            List<GwentBoardCard> KilledCards = new();
            GwentActionType actionType = GwentActionType.ScorchBoardCardPlayed;
            if (boardCard.CardId == 139 || boardCard.CardId == 214 || boardCard.CardId == 15)
            {
                var rowCards = game.CardsOnBoard.Where(x => x.Placement == boardCard.Placement && x.Owner == identity.GetEnemy());
                if (rowCards.Sum(x => x.CurrentStrength) >= 10 && rowCards.Any(x => !x.Abilities.HasFlag(Abilities.Hero)))
                {
                    int maxCurrentSstrength = rowCards.Where(x => !x.Abilities.HasFlag(Abilities.Hero)).Max(x => x.CurrentStrength);
                    var strongestCards = rowCards.Where(x => x.CurrentStrength == maxCurrentSstrength).ToList();
                    KilledCards = strongestCards;
                    foreach (var strongCard in strongestCards)
                    {
                        game.CardsOnBoard.Remove(strongCard);
                        _gameDataService.GetPlayerSide(game, strongCard.Owner).UsedCards.Add(strongCard);
                    }
                }
                else
                {
                    actionType = GwentActionType.NormalCardPlayed;
                }
            }
            else
            {
                var nonHeroes = game.CardsOnBoard.Where(x => !x.Abilities.HasFlag(Abilities.Hero));
                if (nonHeroes.Any())
                {
                    int maxCurrentSstrength = nonHeroes.Max(x => x.CurrentStrength);
                    var strongestCards = nonHeroes.Where(x => x.CurrentStrength == maxCurrentSstrength).ToList();
                    KilledCards = strongestCards;
                    foreach (var strongCard in strongestCards)
                    {
                        game.CardsOnBoard.Remove(strongCard);
                        _gameDataService.GetPlayerSide(game, strongCard.Owner).UsedCards.Add(strongCard);
                    }
                }
            }

            return (KilledCards, actionType);
        }

        private List<GwentBoardCard> MusterCardPlayed(Game game, GwentBoardCard boardCard, PlayerIdentity identity)
        {
            List<GwentBoardCard> playedCards = new();
            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, identity);

            string musterName = boardCard.Name.Split(' ')[0];
            int[] badCards = [19, 102];

            var deckAndInHandsCards = playerSide.CardsInHand.Concat(playerSide.Deck);

            if ((boardCard.CardId == 4 || boardCard.CardId == 9) && deckAndInHandsCards.Any(x => x.CardId == 215))
            {
                GwentCard roachCard = deckAndInHandsCards.First(x => x.CardId == 215);
                GwentBoardCard roachBoardCard = _cardsProvider.CreateGwentBoardCard(roachCard, identity);
                game.CardsOnBoard.Add(roachBoardCard);
                playedCards.Add(roachBoardCard);
                if (playerSide.CardsInHand.Contains(roachCard))
                    playerSide.CardsInHand.Remove(roachCard);
                else
                    playerSide.Deck.Remove(roachCard);
                return playedCards;
            }

            foreach (var musterCard in playerSide.CardsInHand.Where(x => x.Name.Split(' ')[0] == musterName && x.PrimaryId != boardCard.PrimaryId && !badCards.Contains(x.CardId)).ToList())
            {
                playerSide.CardsInHand.Remove(musterCard);
                GwentBoardCard boardMusterCard = _cardsProvider.CreateGwentBoardCard(musterCard, identity);
                game.CardsOnBoard.Add(boardMusterCard);
                playedCards.Add(boardMusterCard);
            }

            foreach (var musterCard in playerSide.Deck.Where(x => x.Name.Split(' ')[0] == musterName && x.PrimaryId != boardCard.PrimaryId && !badCards.Contains(x.CardId)).ToList())
            {
                playerSide.Deck.Remove(musterCard);
                GwentBoardCard boardMusterCard = _cardsProvider.CreateGwentBoardCard(musterCard, identity);
                game.CardsOnBoard.Add(boardMusterCard);
                playedCards.Add(boardMusterCard);
            }

            return playedCards;
        }
    }
}
