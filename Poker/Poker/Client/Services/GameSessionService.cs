using System.Net.Http;

namespace Poker.Client.Services
{
    public class GameSessionService : IGameSessionService
    {
        private readonly HttpClient _httpClient;

        public GameSessionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //public async Task<IEnumerable<string>> GetPlayers(int tableId)
        //{
        //    var result = await _httpClient.PostJsonAsync<IEnumerable<string>>("api/GameSession", tableId);
        //    return result;
        //}
    }
}
