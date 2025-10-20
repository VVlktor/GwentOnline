using GwentApi.Classes;
using GwentApi.Classes.Dtos;
using GwentApi.Extensions;
using GwentApi.Repository.Interfaces;
using GwentApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
                Placement = hornClickedDto.Placement,
                Strength = card.Strength,
                Abilities = card.Abilities,
                CurrentStrength = 0,
                Owner = hornClickedDto.Identity,
                FileName= card.FileName
            };

            playerSide.CardsInHand.Remove(card);
            game.CardsOnBoard.Add(gwentBoardCard);

            await _gameRepository.UpdateGame(game);

            return gwentBoardCard;
        }

        public async Task<LaneClickedGwentActionResult> LaneClicked(LaneClickedDto laneClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(laneClickedDto.Code);

            if (game.Turn != laneClickedDto.Identity) return null;

            PlayerSide playerSide = game.GetPlayerSide(laneClickedDto.Identity);

            if (!playerSide.CardsInHand.Any(x => x.PrimaryId == laneClickedDto.Card.PrimaryId)) return null;

            GwentCard card = playerSide.CardsInHand.First(x => x.PrimaryId == laneClickedDto.Card.PrimaryId);

            if(card.Abilities.HasFlag(Abilities.Spy) ||
                card.Placement == TroopPlacement.Weather ||
                card.Placement == TroopPlacement.Special) return null;

            bool canBePlaced = card.Placement == laneClickedDto.Placement
                                                || (card.Placement == TroopPlacement.Agile && (laneClickedDto.Placement == TroopPlacement.Melee
                                                || laneClickedDto.Placement == TroopPlacement.Range));

            if(!canBePlaced) return null;

            //koniec sprawdzania

            if (card.Placement == TroopPlacement.Agile)
                card.Placement = laneClickedDto.Placement;
            
            GwentBoardCard boardCard = new()
            {
                Name = card.Name,
                PrimaryId = card.PrimaryId,
                CardId = card.CardId,
                Faction = card.Faction,
                Placement = card.Placement,
                Strength = card.Strength,
                Abilities = card.Abilities,
                CurrentStrength = card.Strength,
                Owner = laneClickedDto.Identity,
                FileName = card.FileName
            };

            GwentActionType gwentActionType = GwentActionType.NormalCardPlayed;
            if (card.Abilities.HasFlag(Abilities.Medic))
            {
                gwentActionType = GwentActionType.MedicCardPlayed;
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
                gwentActionType = GwentActionType.ScorchBoardCardPlayed;
            }

            LaneClickedGwentActionResult actionResult = new()
            {
                ActionType = gwentActionType,
                KilledCards = new(),
                PlayedCards = new() { boardCard }
            };

            if(gwentActionType == GwentActionType.ScorchBoardCardPlayed)
            {
                var result = ScorchBoardCardPlayed(game, boardCard, laneClickedDto.Identity);
                actionResult.KilledCards = result.Item1;
                actionResult.ActionType = result.Item2;
            }
            else if (gwentActionType == GwentActionType.MusterCardPlayed)
            {
                var musterCards = MusterCardPlayed(game, boardCard, laneClickedDto.Identity);
                if(musterCards.Count() == 0)
                    actionResult.ActionType = GwentActionType.NormalCardPlayed;
                else
                    actionResult.PlayedCards.AddRange(musterCards);
            }

            playerSide.CardsInHand.Remove(card);
            game.CardsOnBoard.Add(boardCard);

            await _gameRepository.UpdateGame(game);

            return actionResult;
        }

        public async Task<CardClickedGwentActionResult> CardClicked(CardClickedDto cardClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(cardClickedDto.Code);

            if (game.Turn != cardClickedDto.Identity) return null;

            PlayerSide playerSide = game.GetPlayerSide(cardClickedDto.Identity);

            if (!playerSide.CardsInHand.Any(x => x.PrimaryId == cardClickedDto.SelectedCard.PrimaryId)) return null;

            GwentCard card = playerSide.CardsInHand.First(x => x.PrimaryId == cardClickedDto.SelectedCard.PrimaryId);

            if (card.CardId != 2) return null;

            if (!game.CardsOnBoard.Any(x => x.PrimaryId == cardClickedDto.ClickedCard.PrimaryId && cardClickedDto.ClickedCard.Owner == cardClickedDto.Identity)) return null;

            GwentBoardCard clickedCard = game.CardsOnBoard.First(x => x.PrimaryId == cardClickedDto.ClickedCard.PrimaryId);

            if (clickedCard.Abilities.HasFlag(Abilities.Hero)) return null;
            if(clickedCard.CardId==2) return null;

            GwentBoardCard decoyCard = new()
            {
                Name = card.Name,
                PrimaryId = card.PrimaryId,
                CardId = card.CardId,
                Faction = card.Faction,
                Placement = clickedCard.Placement,
                Strength = card.Strength,
                Abilities = card.Abilities,
                CurrentStrength = 0,
                Owner = cardClickedDto.Identity,
                FileName = card.FileName
            };

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
                PlayedCard =  decoyCard 
            };
        }

        public async Task<WeatherClickedGwentActionResult> WeatherClicked(WeatherClickedDto weatherClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(weatherClickedDto.Code);

            if (game.Turn != weatherClickedDto.Identity) return null;

            PlayerSide playerSide = game.GetPlayerSide(weatherClickedDto.Identity);

            if (!playerSide.CardsInHand.Any(x => x.PrimaryId == weatherClickedDto.Card.PrimaryId)) return null;

            GwentCard card = playerSide.CardsInHand.First(x => x.PrimaryId == weatherClickedDto.Card.PrimaryId);

            if (card.Placement != TroopPlacement.Weather) return null;
            if (game.CardsOnBoard.Any(x => x.CardId == card.CardId)) return null;

            GwentBoardCard boardCard = new()
            {
                Name = card.Name,
                PrimaryId = card.PrimaryId,
                CardId = card.CardId,
                Faction = card.Faction,
                Placement = card.Placement,
                Strength = 0,
                Abilities = card.Abilities,
                CurrentStrength = 0,
                Owner = weatherClickedDto.Identity,
                FileName = card.FileName
            };

            if(card.CardId==11)//id karty Scorch
            {
                var nonHeroes = game.CardsOnBoard.Where(x => !x.Abilities.HasFlag(Abilities.Hero));

                WeatherClickedGwentActionResult scorchActionResult = new()
                {
                    ActionType = GwentActionType.ScorchCardPlayed,
                    RemovedCards = new(),
                    PlayedCard = boardCard
                };

                if (nonHeroes.Any())
                {
                    int maxCurrentSstrength = nonHeroes.Max(x => x.CurrentStrength);
                    var strongestCards = nonHeroes.Where(x => x.CurrentStrength == maxCurrentSstrength).ToList();
                    
                    scorchActionResult.RemovedCards = strongestCards;

                    foreach (var strongCard in strongestCards)
                    {
                        game.CardsOnBoard.Remove(strongCard);
                        game.GetPlayerSide(strongCard.Owner).UsedCards.Add(strongCard);
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
                    //na razie nie daje playerSide.UsedCards.Add(weatherCard); (i dla przeciwnika jeli jego), bo nie mozna wskrzeszac pogodowych. Zalezy czy bedzie gdzies counter zuzytych kart
                }

                playerSide.CardsInHand.Remove(card);

                await _gameRepository.UpdateGame(game);

                WeatherClickedGwentActionResult clearWeatherActionResult = new()
                {
                    ActionType = GwentActionType.WeatherCardPlayed,
                    RemovedCards = weatherCards,
                    PlayedCard = boardCard
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
                PlayedCard = boardCard
            };

            return weatherActionResult;
        }

        public async Task<EnemyLaneClickedGwentActionResult> EnemyLaneClicked(EnemyLaneClickedDto enemyLaneClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(enemyLaneClickedDto.Code);

            if (game.Turn != enemyLaneClickedDto.Identity) return null;

            PlayerSide playerSide = game.GetPlayerSide(enemyLaneClickedDto.Identity);

            if(!playerSide.CardsInHand.Any(x => x.PrimaryId == enemyLaneClickedDto.Card.PrimaryId)) return null;

            GwentCard card = playerSide.CardsInHand.First(x => x.PrimaryId == enemyLaneClickedDto.Card.PrimaryId);

            if (!card.Abilities.HasFlag(Abilities.Spy)) return null;

            if (card.Placement != enemyLaneClickedDto.Placement) return null;

            GwentBoardCard boardCard = new()
            {
                Name = card.Name,
                PrimaryId = card.PrimaryId,
                CardId = card.CardId,
                Faction = card.Faction,
                Placement = card.Placement,
                Strength = card.Strength,
                Abilities = card.Abilities,
                CurrentStrength = card.Strength,
                Owner = enemyLaneClickedDto.Identity.GetEnemy(),
                FileName = card.FileName
            };

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
                PlayedCard = boardCard
            };

            return actionResult;
        }

        public async Task<PassClickedGwentActionResult> PassClicked(PassClickedDto passClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(passClickedDto.Code);

            if (game.Turn != passClickedDto.Identity) return null;

            if (passClickedDto.Identity == PlayerIdentity.PlayerOne)
                game.HasPassed = (true, game.HasPassed.PlayerTwo);
            else
                game.HasPassed = (game.HasPassed.PlayerOne, true);

            PassClickedGwentActionResult actionResult = new()
            {
                ActionType = GwentActionType.Pass,
                Passed = true
            };

            await _gameRepository.UpdateGame(game);

            return actionResult;
        }

        public async Task<LeaderClickedGwentActionResult> LeaderClicked(LeaderClickedDto leaderClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(leaderClickedDto.Code);

            PlayerSide playerSide = game.GetPlayerSide(leaderClickedDto.Identity);

            //playerSide.LeaderCard podmienic LeaderCard na cos dziedziczącego po GwentCard
            throw new NotImplementedException();
        }

        public async Task<CarouselCardClickedGwentActionResult> CarouselCardClicked(CarouselCardClickedDto carouselCardClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(carouselCardClickedDto.Code);
            
            if (game.Turn != carouselCardClickedDto.Identity) return null;

            GwentAction lastGwentAction = game.Actions.Last();

            if (lastGwentAction.AbilitiyUsed.HasFlag(Abilities.Medic) || lastGwentAction.Issuer != carouselCardClickedDto.Identity) return null;

            PlayerSide playerSide = game.GetPlayerSide(carouselCardClickedDto.Identity);

            if (!playerSide.UsedCards.Any(x => x.PrimaryId == carouselCardClickedDto.Card.PrimaryId)) return null;//wsm mozna zrobic firstordefalut i jesli default to zwrocic null, do poprawki

            if (carouselCardClickedDto.Card.CardId == 2) return null;

            GwentCard card = playerSide.UsedCards.First(x => x.PrimaryId == carouselCardClickedDto.Card.PrimaryId);

            GwentBoardCard boardCard = new()
            {
                Name = card.Name,
                PrimaryId = card.PrimaryId,
                CardId = card.CardId,
                Faction = card.Faction,
                Placement = card.Placement,
                Strength = card.Strength,
                Abilities = card.Abilities,
                CurrentStrength = card.Strength,
                Owner = carouselCardClickedDto.Identity,
                FileName = card.FileName
            };

            if (card.Placement == TroopPlacement.Agile)
                card.Placement = TroopPlacement.Melee;//na wszelki wypadek

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
                    PlayedCards = new(){ boardCard },
                    KilledCards = new()
                };

                return spyActionResult;
            }
            else if (card.Abilities.HasFlag(Abilities.Medic))
            {
                gwentActionType = GwentActionType.MedicCardPlayed;
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
                gwentActionType = GwentActionType.ScorchBoardCardPlayed;
            }

            CarouselCardClickedGwentActionResult actionResult = new()
            {
                ActionType = gwentActionType,
                KilledCards = new(),
                PlayedCards = new() { boardCard }
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
                        game.GetPlayerSide(strongCard.Owner).UsedCards.Add(strongCard);
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
                        game.GetPlayerSide(strongCard.Owner).UsedCards.Add(strongCard);
                    }
                }
            }

            return (KilledCards, actionType);
        }

        private List<GwentBoardCard> MusterCardPlayed(Game game, GwentBoardCard boardCard, PlayerIdentity identity)
        {
            List<GwentBoardCard> playedCards = new();
            PlayerSide playerSide = game.GetPlayerSide(identity);

            string musterName = boardCard.Name.Split(' ')[0];
            int[] badCards = [19, 102];

            //if(boardCard.CardId == 4 || boardCard.CardId == 9)
            //{
            //    if(playerSide.CardsInHand.Any(x=>x.CardId == 1000))
            //}dodac plotke do jsona

            foreach (var musterCard in playerSide.CardsInHand.Where(x => x.Name.Split(' ')[0] == musterName && x.PrimaryId != boardCard.PrimaryId && !badCards.Contains(x.CardId)).ToList())
            {
                playerSide.CardsInHand.Remove(musterCard);
                GwentBoardCard boardMusterCard = new()
                {
                    Name = musterCard.Name,
                    PrimaryId = musterCard.PrimaryId,
                    CardId = musterCard.CardId,
                    Faction = musterCard.Faction,
                    Placement = musterCard.Placement,
                    Strength = musterCard.Strength,
                    Abilities = musterCard.Abilities,
                    CurrentStrength = musterCard.Strength,
                    Owner = identity,
                    FileName = musterCard.FileName
                };
                game.CardsOnBoard.Add(boardMusterCard);
                playedCards.Add(boardMusterCard);
            }

            foreach (var musterCard in playerSide.Deck.Where(x => x.Name.Split(' ')[0] == musterName && x.PrimaryId != boardCard.PrimaryId && !badCards.Contains(x.CardId)).ToList())
            {
                playerSide.Deck.Remove(musterCard);
                GwentBoardCard boardMusterCard = new()
                {
                    Name = musterCard.Name,
                    PrimaryId = musterCard.PrimaryId,
                    CardId = musterCard.CardId,
                    Faction = musterCard.Faction,
                    Placement = musterCard.Placement,
                    Strength = musterCard.Strength,
                    Abilities = musterCard.Abilities,
                    CurrentStrength = musterCard.Strength,
                    Owner = identity,
                    FileName = musterCard.FileName
                };
                game.CardsOnBoard.Add(boardMusterCard);
                playedCards.Add(boardMusterCard);
            }

            return playedCards;
        }
    }
}
