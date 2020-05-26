using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Poker.Shared.Models;

namespace Poker.Client.Services
{
    public class PlayerNoteService : IPlayerNoteService
    {
        private readonly HttpClient _httpClient;

        public PlayerNoteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CreateNoteResult> Create(CreateNoteModel model)
        {
            var result = await _httpClient.PostJsonAsync<CreateNoteResult>("api/PlayerNote", model);
            return result;
        }

        public async Task<GetNotesResult> GetList()
        {
            var result = await _httpClient.GetJsonAsync<GetNotesResult>("api/PlayerNote");
            return result;
        }

        public async Task<DeleteTableResult> Delete(string notedPlayerName)
        {
            var result = await _httpClient.PostJsonAsync<DeleteTableResult>("api/PlayerNote/delete", notedPlayerName);
            return result;
        }
    }
}
