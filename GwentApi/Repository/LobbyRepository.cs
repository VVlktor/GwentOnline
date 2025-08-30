using GwentApi.Classes;
using GwentApi.Repository.Interfaces;
using System.Collections.Generic;
using System.Security;

namespace GwentApi.Repository
{
    public class LobbyRepository : ILobbyRepository
    {
        private List<Lobby> lobbyList = new();//do zmiany na baze danych

        public async Task<Lobby> AddLobby(Lobby lobby)
        {
            lobbyList.Add(lobby);
            return lobby;
        }

        public async Task<bool> ExistsByCode(string lobbyCode)
        {
            return lobbyList.Any(x => x.LobbyCode == lobbyCode);
        }

        public async Task<Lobby> GetLobbyByCode(string lobbyCode)
        {
            return lobbyList.FirstOrDefault(x => x.LobbyCode == lobbyCode);
        }

        public async Task<PlayerInfo> SetDeckForPlayer(PlayerIdentity identity, string lobbyCode, PlayerInfo playerInfo)
        {
            Random rng = new Random();
            
            int n = playerInfo.Cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (playerInfo.Cards[n], playerInfo.Cards[k]) = (playerInfo.Cards[k], playerInfo.Cards[n]);
            }

            Lobby lobby = lobbyList.FirstOrDefault(x=>x.LobbyCode==lobbyCode);
            if(identity == PlayerIdentity.PlayerOne)
                lobby.PlayerOneInfo = playerInfo;
            else
                lobby.PlayerTwoInfo = playerInfo;
            return playerInfo;
        }

        public async Task<bool> PlayersReady(string lobbyCode)
        {
            Lobby lobby = lobbyList.FirstOrDefault(x => x.LobbyCode == lobbyCode);
            return lobby.ArePlayersReady();
        }

        public async Task<PlayerInfo> GetPlayerInfo(string lobbyCode, PlayerIdentity identity)
        {
            Lobby lobby = lobbyList.First(x => x.LobbyCode == lobbyCode);
            PlayerInfo playerInfo = identity == PlayerIdentity.PlayerOne ? lobby.PlayerOneInfo : lobby.PlayerTwoInfo;
            return playerInfo;
        }

        public async Task SwapDeck(string lobbyCode, PlayerIdentity identity, List<GwentCard> Cards)
        {
            Lobby lobby = lobbyList.First(x => x.LobbyCode == lobbyCode);
            PlayerInfo playerInfo = identity == PlayerIdentity.PlayerOne ? lobby.PlayerOneInfo: lobby.PlayerTwoInfo;
            playerInfo.Cards = Cards;
        }
    }
}
