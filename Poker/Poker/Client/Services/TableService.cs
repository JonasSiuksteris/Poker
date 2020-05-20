using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Poker.Shared;
using Poker.Shared.Models;

namespace Poker.Client.Services
{
    public class TableService : ITableService
    {
        private readonly HttpClient _httpClient;

        public TableService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<CreateTableResult> Create(CreateTableModel model)
        {
            var result = await _httpClient.PostJsonAsync<CreateTableResult>("api/table", model);
            return result;
        }

        public async Task<GetTablesResult> GetList()
        {
            var result = await _httpClient.GetJsonAsync<GetTablesResult>("api/table");
            return result;
        }

        public async Task<PokerTable> GetById(int id)
        {
            var result = await _httpClient.GetJsonAsync<PokerTable>($"api/table/{id}");
            return result;
        }

        public async Task<DeleteTableResult> Delete(int id)
        {
            var result = await _httpClient.PostJsonAsync<DeleteTableResult>("api/table/delete", id);
            return result;
        }
    }
}
