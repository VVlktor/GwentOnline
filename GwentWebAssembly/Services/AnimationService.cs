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

        public AnimationService(IJSRuntime js, IPlayerService playerService)
        {
            _jsRuntime = js;
            _playerService = playerService;
        }

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
            //gameStatusDto.Action.CardsPlayed[0].Abilities
            switch (gameStatusDto.Action.ActionType)// dodac morale boost, muster, tight bond i tak dalej
            {
                case GwentActionType.SpyCardPlayed:
                    await PlayPostSpyAnimation(gameStatusDto);
                    break;
                case GwentActionType.MedicCardPlayed:
                    await PlayPostBasicAnimation(gameStatusDto);
                    break;
                case GwentActionType.MusterCardPlayed:
                    await PlayPostMusterAnimation(gameStatusDto);
                    break;

            }
        }

        private async Task PlayPostSpyAnimation(GameStatusDto gameStatusDto)
        {
            if (gameStatusDto.Action.CardsDrawn.Count == 0) return;

            CardJsInfo jsInfo = GetData(gameStatusDto.Action.CardsPlayed[0]);
            List<CardJsInfo> drawnCardsIds = gameStatusDto.Action.CardsDrawn.Select(GetData).ToList();

            await _jsRuntime.InvokeVoidAsync("playPostAnimation", jsInfo.PrimaryId, jsInfo.AbilityName);

            if (gameStatusDto.Action.Issuer == _playerService.EnemyIdentity()) return;

            await _jsRuntime.InvokeVoidAsync("playPostSpyAnimation", drawnCardsIds);
        }


        //kiedy wystawiam karte, to moglaby znikac z dolu (z reki gracza)
        public async Task ProcessReceivedAnimation(GameStatusDto gameStatusDto)
        {
            if (gameStatusDto.Action.LeaderUsed)
            {
                string message = gameStatusDto.Action.Issuer == _playerService.GetIdentity() ? "You used leader ability" : "Enemy used leader ability";
                await OverlayAnimation(message);
            }

            switch (gameStatusDto.Action.ActionType)
            {
                case GwentActionType.NormalCardPlayed:
                case GwentActionType.MedicCardPlayed:
                    await PlayNormalCardAnimation(gameStatusDto);
                    break;
                case GwentActionType.CommandersHornCardPlayed:
                    await PlayCommandersHornAnimation(gameStatusDto);
                    break;
                case GwentActionType.DecoyCardPlayed:
                    await PlayDecoyAnimation(gameStatusDto);
                    break;
                case GwentActionType.WeatherCardPlayed:
                    await PlayWeatherCardAnimation(gameStatusDto);
                    break;
                case GwentActionType.SpyCardPlayed:
                    await PlaySpyCardAnimation(gameStatusDto);
                    break;
                case GwentActionType.ScorchCardPlayed:
                    await PlayScorchCardAnimation(gameStatusDto);
                    break;
                case GwentActionType.MusterCardPlayed:
                    await PlayMusterAnimation(gameStatusDto);
                    break;
                case GwentActionType.ScorchBoardCardPlayed:
                    await PlayScorchBoardCardAnimation(gameStatusDto);
                    break;
                case GwentActionType.Pass:
                    await PlayPassAnimation(gameStatusDto);
                    break;
                case GwentActionType.EndRound:
                    await OverlayAnimation("End of round!");//moze cos wiecej, np. zmiatanie kart z planszy
                    break;
            }
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
                CardJsInfo cardData = GetData(card);
                //string endName = $"{endNamePart}{card.Placement.ToString()}";
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

            CardJsInfo data = GetData(boardCard);
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

            CardJsInfo data = GetData(boardCard);
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

            CardJsInfo data = GetData(boardCard);
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

            CardJsInfo data = GetData(swappedCard);
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

            CardJsInfo data = GetData(boardCard);

            await _jsRuntime.InvokeVoidAsync("moveCardByElementIdsWithInfo", startName, endName, data, true);
        }

        private async Task PlayPostBasicAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            CardJsInfo jsInfo = GetData(boardCard);
            await _jsRuntime.InvokeVoidAsync("playPostAnimation", boardCard.PrimaryId, jsInfo.AbilityName);
        }

        private CardJsInfo GetData(GwentBoardCard boardCard)
        {
            CardJsInfo jsInfo = GetData((GwentCard)boardCard);
            jsInfo.Strength = boardCard.CurrentStrength;
            return jsInfo;
        }

        private CardJsInfo GetData(GwentCard card)
        {
            CardJsInfo jsInfo = new();
            jsInfo.ImagePath = $"img/cards/{card.FileName}";
            jsInfo.IsHero = card.Abilities.HasFlag(Abilities.Hero);
            jsInfo.PlacementName = card.Placement.ToString().ToLower();
            jsInfo.AbilityName = card.Abilities.GetAbilityName();
            jsInfo.PrimaryId = card.PrimaryId;
            jsInfo.Strength = card.Strength;
            return jsInfo;
        }
    }
}