using GwentShared.Classes;
using GwentShared.Classes.Dtos;
using GwentWebAssembly.Data;
using GwentWebAssembly.Extensions;
using GwentWebAssembly.Services.Interfaces;
using Microsoft.JSInterop;

namespace GwentWebAssembly.Services
{
    public class AnimationService : IAnimationService
    {
        private readonly IJSRuntime _jsRuntime;
        private IPlayerService _playerService;

        private Dictionary<GwentActionType, Func<GameStatusDto, Task>> receivedAnimationHandlers;
        private Dictionary<GwentActionType, Func<GameStatusDto, Task>> postAnimationHandlers;

        public AnimationService(IJSRuntime js, IPlayerService playerService)
        {
            _jsRuntime = js;
            _playerService = playerService;

            receivedAnimationHandlers = new()
            {
                { GwentActionType.NormalCardPlayed, PlayNormalCardAnimation },
                { GwentActionType.MedicCardPlayed, PlayNormalCardAnimation },
                { GwentActionType.CommandersHornCardPlayed, PlayCommandersHornAnimation },
                { GwentActionType.DecoyCardPlayed, PlayDecoyAnimation },
                { GwentActionType.WeatherCardPlayed, PlayWeatherCardAnimation },
                { GwentActionType.SpyCardPlayed, PlaySpyCardAnimation },
                { GwentActionType.ScorchCardPlayed, PlayScorchCardAnimation },
                { GwentActionType.MusterCardPlayed, PlayMusterAnimation },
                { GwentActionType.ScorchBoardCardPlayed, PlayScorchBoardCardAnimation },
                { GwentActionType.Pass, PlayPassAnimation },
                { GwentActionType.EndRound, (x) => OverlayAnimation("End of round!") }
            };

            postAnimationHandlers = new()
            {
                { GwentActionType.SpyCardPlayed, PlayPostSpyAnimation },
                { GwentActionType.MedicCardPlayed, PlayPostBasicAnimation },
                { GwentActionType.MusterCardPlayed, PlayPostMusterAnimation },
            };
        }//jak medyk revive robi to karty powinny isc ze stosu zurzytych

        public async Task OverlayAnimation(string text) => await _jsRuntime.InvokeVoidAsync("showOverlay", text);

        public async Task OverlayAnimation(PlayerIdentity turn)
        {
            string stringTurn = "Opponent's turn!";
            if (_playerService.GetIdentity() == turn)
                stringTurn = "Your turn!";
            await _jsRuntime.InvokeVoidAsync("showOverlay", stringTurn);
        }

        public async Task ResizeCardContainters(int cardInHandCount, List<GwentBoardCard> cardsOnBoard) {
            await _jsRuntime.InvokeVoidAsync("resizeCardContainer", "hand-row", 11, 0.075, .00225, cardInHandCount);

            TroopPlacement[] placements = [TroopPlacement.Melee, TroopPlacement.Range, TroopPlacement.Siege];
            string[] sides = ["player", "enemy"];

            foreach (var placement in placements)
            {
                foreach(var side in sides)
                {
                    PlayerIdentity identity = side == "player" ? _playerService.GetIdentity() : _playerService.EnemyIdentity();
                    int cardCount = cardsOnBoard.Count(x => x.Placement == placement && x.Owner == identity);
                    await _jsRuntime.InvokeVoidAsync("resizeCardContainer", $"{side}Lane{placement.ToString()}", 10, 0.075, .00325, cardCount);
                }
            }
        }

        public async Task EndGameOverlayAnimation(string message) => await _jsRuntime.InvokeVoidAsync("showEndGameOverlay", message);

        public async Task ProcessPostAnimation(GameStatusDto gameStatusDto)
        {
            if(postAnimationHandlers.TryGetValue(gameStatusDto.Action.ActionType, out var postAnimation))
                await postAnimation(gameStatusDto);
            else
                await PlayPostUnknownAnimation(gameStatusDto);
        }

        private async Task PlayPostUnknownAnimation(GameStatusDto gameStatusDto)
        {
            if (gameStatusDto.Action.CardsPlayed.Count == 0) return;

            Abilities abilities = gameStatusDto.Action.CardsPlayed[0].Abilities;

            if (abilities.HasFlag(Abilities.Morale))
                await PlayPostBasicAnimation(gameStatusDto);
            else if (abilities.HasFlag(Abilities.Bond))
                await PlayPostBondAnimation(gameStatusDto);
        }

        private async Task PlayPostBondAnimation(GameStatusDto gameStatusDto)
        {
            List<string> cardsIds = gameStatusDto.CardsOnBoard.Where(x => x.Owner == gameStatusDto.Action.Issuer && x.CardId == gameStatusDto.Action.CardsPlayed[0].CardId).Select(x=>$"card-on-board-{x.PrimaryId}").ToList();
            if (cardsIds.Count < 2) return;
            await _jsRuntime.InvokeVoidAsync("multipleAbilityAnimation", cardsIds, Abilities.Bond.GetAbilityName());
        }

        private async Task PlayPostSpyAnimation(GameStatusDto gameStatusDto)
        {
            if (gameStatusDto.Action.CardsDrawn.Count == 0) return;

            CardJsInfo jsInfo = gameStatusDto.Action.CardsPlayed[0].GetData();
            List<CardJsInfo> drawnCardsIds = gameStatusDto.Action.CardsDrawn.Select(x=>x.GetData()).ToList();
            
            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity()) await _jsRuntime.InvokeVoidAsync("hideElementsById", drawnCardsIds.Select(x=>$"card-in-hand-{x.PrimaryId}").ToList());

            await _jsRuntime.InvokeVoidAsync("playPostAnimation", jsInfo.PrimaryId, jsInfo.AbilityName);
            
            if (gameStatusDto.Action.Issuer == _playerService.EnemyIdentity()) return;

            await _jsRuntime.InvokeVoidAsync("playPostSpyAnimation", drawnCardsIds);
        }

        public async Task ProcessReceivedAnimation(GameStatusDto gameStatusDto)
        {
            if (gameStatusDto.Action.LeaderUsed)
            {
                string message = gameStatusDto.Action.Issuer == _playerService.GetIdentity() ? "You used leader ability" : "Enemy used leader ability";
                await OverlayAnimation(message);
            }

            if (receivedAnimationHandlers.TryGetValue(gameStatusDto.Action.ActionType, out var animationHandler))
                await animationHandler(gameStatusDto);
        }

        private async Task PlayPostMusterAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];

            bool isPlayer = gameStatusDto.Action.Issuer == _playerService.GetIdentity();

            string startName = isPlayer ? "deck-me" : "deck-op";
            string endNamePart = isPlayer ? "playerLane" : "enemyLane";

            var cardsId = gameStatusDto.Action.CardsPlayed[1..].Select(x => $"card-on-board-{x.PrimaryId}").ToList();

            await _jsRuntime.InvokeVoidAsync("hideElementsById", cardsId);
            await _jsRuntime.InvokeVoidAsync("removeCardsOverlays");

            foreach (var card in gameStatusDto.Action.CardsPlayed[1..])
            {
                CardJsInfo cardData = card.GetData();
                string endName = $"card-on-board-{card.PrimaryId}";
                await _jsRuntime.InvokeVoidAsync("moveCardByElementIdsWithInfo", startName, endName, cardData, false);
            }

            await _jsRuntime.InvokeVoidAsync("showElementsById", cardsId);
            await _jsRuntime.InvokeVoidAsync("removeCardsOverlays");
        }

        private async Task PlayMusterAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            string startName = "deck-name-op", endName = $"enemyLane{boardCard.Placement.ToString()}";

            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
            {
                startName = $"card-in-hand-{boardCard.PrimaryId}";
                endName = $"playerLane{boardCard.Placement.ToString()}";
                await _jsRuntime.InvokeVoidAsync("hideElementById", startName);
            }

            CardJsInfo data = boardCard.GetData();
            await _jsRuntime.InvokeVoidAsync("moveCardByElementIdsWithInfo", startName, endName, data, false);
            await _jsRuntime.InvokeVoidAsync("playAbilityAnimation", data);//niedokonczone
        }


        private async Task PlayScorchBoardCardAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            string startName = "deck-name-op", endName = $"enemyLane{boardCard.Placement.ToString()}";
            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
            {
                startName = $"card-in-hand-{boardCard.PrimaryId}";
                endName = $"playerLane{boardCard.Placement.ToString()}";
                await _jsRuntime.InvokeVoidAsync("hideElementById", startName);
            }

            CardJsInfo data = boardCard.GetData();
            await _jsRuntime.InvokeVoidAsync("moveCardByElementIdsWithInfo", startName, endName, data, false);

            List<string> killedCardsIds = gameStatusDto.Action.CardsKilled.Select(x => $"card-on-board-{x.PrimaryId}").ToList();
            await _jsRuntime.InvokeVoidAsync("showScorchAnimation", killedCardsIds);
            await _jsRuntime.InvokeVoidAsync("removeCardsOverlays");
        }

        private async Task PlayScorchCardAnimation(GameStatusDto gameStatusDto)
        {
            if (gameStatusDto.Action.LeaderUsed) return;
            
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            string startName = "deck-name-op", endName = "weather";

            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
            {
                startName = $"card-in-hand-{boardCard.PrimaryId}";
                await _jsRuntime.InvokeVoidAsync("hideElementById", startName);
            }

            await _jsRuntime.InvokeVoidAsync("moveCardByElementIdsNoInfo", startName, endName, $"img/cards/{boardCard.FileName}", false);
            
            List<string> killedCardsIds = gameStatusDto.Action.CardsKilled.Select(x => $"card-on-board-{x.PrimaryId}").ToList();
            await _jsRuntime.InvokeVoidAsync("showScorchAnimation", killedCardsIds);
            await _jsRuntime.InvokeVoidAsync("removeCardsOverlays");
        }

        private async Task PlayPassAnimation(GameStatusDto gameStatusDto)
        {
            string text = "Your opponent has passed!";
            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
                text = "Round passed!";

            await OverlayAnimation(text);
        }

        private async Task PlaySpyCardAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];

            string startName = "deck-name-op", endName = $"playerLane{boardCard.Placement.ToString()}";

            bool isPlayer = gameStatusDto.Action.Issuer == _playerService.GetIdentity();
            if (isPlayer){

                startName = $"card-in-hand-{boardCard.PrimaryId}";
                endName = $"enemyLane{boardCard.Placement.ToString()}";
                await _jsRuntime.InvokeVoidAsync("hideElementById", startName);
            }

            CardJsInfo data = boardCard.GetData();
            await _jsRuntime.InvokeVoidAsync("moveCardByElementIdsWithInfo", startName, endName, data, true);
        }

        private async Task PlayWeatherCardAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            string startName = "deck-name-op", endName = "weather";

            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
            {
                startName = $"card-in-hand-{boardCard.PrimaryId}";
                await _jsRuntime.InvokeVoidAsync("hideElementById", startName);
            }

            await _jsRuntime.InvokeVoidAsync("moveCardByElementIdsNoInfo", startName, endName, $"img/cards/{boardCard.FileName}", true);
        }

        private async Task PlayDecoyAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard decoyCard = gameStatusDto.Action.CardsPlayed[0];
            GwentBoardCard swappedCard = gameStatusDto.Action.CardsKilled[0];

            string startName = "", endName = $"card-on-board-{swappedCard.PrimaryId}";

            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
            {
                startName = $"card-in-hand-{decoyCard.PrimaryId}";
                await _jsRuntime.InvokeVoidAsync("hideElementById", startName);
            }
            else
                startName = "deck-name-op";

            await _jsRuntime.InvokeVoidAsync("moveCardByElementIdsNoInfo", startName, endName, $"img/cards/{decoyCard.FileName}", false);

            startName = endName;
            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
                endName = $"card-in-hand-{decoyCard.PrimaryId}";
            else
                endName = "deck-name-op";

            CardJsInfo data = swappedCard.GetData();
            await _jsRuntime.InvokeVoidAsync("moveCardByElementIdsWithInfo", startName, endName, data, true);
            await _jsRuntime.InvokeVoidAsync("removeCardsOverlays");
        }

        private async Task PlayCommandersHornAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            string startName = "", endName = "";
            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
            {
                startName = gameStatusDto.Action.LeaderUsed ? "leader-me" : $"card-in-hand-{boardCard.PrimaryId}";
                endName = $"playerHornLane{boardCard.Placement.ToString()}";
            }
            else
            {
                startName = "deck-name-op";
                endName = $"enemyHornLane{boardCard.Placement.ToString()}";
            }

            await _jsRuntime.InvokeVoidAsync("moveCardByElementIdsNoInfo", startName, endName, $"img/cards/{boardCard.FileName}", true);//potencjalnie w przyszlosci dodac efekt animacji-podswietlenia przy hornie
        }

        private async Task PlayNormalCardAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            string startName = "deck-name-op", endName = $"enemyLane{boardCard.Placement.ToString()}";
            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
            {
                startName = $"card-in-hand-{boardCard.PrimaryId}";
                endName = $"playerLane{boardCard.Placement.ToString()}";
                await _jsRuntime.InvokeVoidAsync("hideElementById", startName);
            }

            CardJsInfo data = boardCard.GetData();

            await _jsRuntime.InvokeVoidAsync("moveCardByElementIdsWithInfo", startName, endName, data, true);
        }

        private async Task PlayPostBasicAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            CardJsInfo jsInfo = boardCard.GetData();
            await _jsRuntime.InvokeVoidAsync("playPostAnimation", boardCard.PrimaryId, jsInfo.AbilityName);
        }
    }
}