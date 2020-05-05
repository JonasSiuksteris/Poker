using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Poker.Client.Services;
using Poker.Shared;

namespace Poker.Client.Pages
{
    public class GameSessionBase : ComponentBase
    {
        [Inject] public ILocalStorageService LocalStorageService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] public ITableService TableService { get; set; }

        [Inject] public IGameSessionService GameSessionService { get; set; }

        public GameInformation GameInformation { get; set; } = new GameInformation{Players = new List<GamePlayer>()};

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Console.WriteLine(1);
            var result = await GameSessionService.GetPlayers(await LocalStorageService.GetItemAsync<int>("currentTable"));
            Console.WriteLine(2);
            var playerList = result.Select(name => new GamePlayer {Username = name}).ToList();
            GameInformation.Players = playerList;

        }

        protected async Task LeaveTable()
        {
            await TableService.RemovePlayer(await LocalStorageService.GetItemAsync<int>("currentTable"));
            await LocalStorageService.RemoveItemAsync("currentTable");

            NavigationManager.NavigateTo("/");

        }


    }
}
