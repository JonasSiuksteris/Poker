using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

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
