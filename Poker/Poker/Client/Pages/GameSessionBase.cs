using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Poker.Client.Modals;
using Poker.Client.Services;
using Poker.Shared;
using Poker.Shared.Models;

namespace Poker.Client.Pages
{
    public class GameSessionBase : ComponentBase
    {
        [Inject] public IModalService ModalService { get; set; }
        [Inject] public ILocalStorageService LocalStorageService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] public ITableService TableService { get; set; }

        [Inject] public IGameSessionService GameSessionService { get; set; }

        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public AuthenticationState AuthState { get; set; }

        private HubConnection _hubConnection;

        public GameInformation GameInformation { get; set; } = new GameInformation{Players = new List<GamePlayer>()};

        protected override async Task OnInitializedAsync()
        {
            AuthState = await AuthenticationStateProvider.GetAuthenticationStateAsync();


            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/gameHub"))
                .Build();

            //_hubConnection.On("ReceiveAddedUser", ((string playerName, int seatNumber) =>
            //{
            //    GameInformation.Players.Add(new GamePlayer
            //    {
            //        Username = playerName,
            //        SeatNumber = seatNumber
            //    });
            //    StateHasChanged();
            //}));

            //_hubConnection.On("ReceiveRemovedUser", ((string playerName) =>
            //    {
            //        GameInformation.Players.Remove(
            //            GameInformation.Players.FirstOrDefault(e => e.Username == playerName));
            //        StateHasChanged();
            //    }));

            //_hubConnection.On("ReceiveReadyState", (string player, bool isReady) =>
            //{
            //    GameInformation.Players.FirstOrDefault(e => e.Username == player).IsReady = isReady;
            //    StateHasChanged();
            //});

            _hubConnection.On("ReceiveStartingHand", (object hand) =>
            {
                var newHand = JsonConvert.DeserializeObject<List<Card>>(hand.ToString());
                GameInformation.Hand.AddRange(newHand);
                StateHasChanged();
            });

            //_hubConnection.On("ReceivePlayingPlayers", (object playersjson) =>
            //{
            //    var players = JsonConvert.DeserializeObject<List<string>>(playersjson.ToString());
            //    foreach (var player in GameInformation.Players)
            //    {
            //        if (players.Contains(player.Username))
            //            player.IsPlaying = true;
            //    }

            //    GameInformation.GameInProgress = true;
            //    StateHasChanged();
            //});

            _hubConnection.On("ReceiveTurnPlayer", (string currentPlayer) =>
            {
                GameInformation.CurrentPlayer = currentPlayer;
                StateHasChanged();
            });

            _hubConnection.On("ReceiveFlop", (object flopjson) =>
            {
                var flop = JsonConvert.DeserializeObject<List<Card>>(flopjson.ToString());
                GameInformation.TableCards.AddRange(flop);
                StateHasChanged();
            });

            _hubConnection.On("ReceiveTurnOrRiver", (object card) =>
            {
                var turnOrRiverCard = JsonConvert.DeserializeObject<List<Card>>(card.ToString());
                GameInformation.TableCards.AddRange(turnOrRiverCard);
                StateHasChanged();
            });


            _hubConnection.On("ReceiveWinner", (string winner) =>
            {
                GameInformation.Winner = winner;
                StateHasChanged();
            });

            //_hubConnection.On("ReceiveNewGame",  () =>
            //{
            //    GameInformation = new GameInformation();
            //});

            _hubConnection.On("ReceiveStateRefresh", (object playerState) =>
            {
                var playerStateModel = JsonConvert.DeserializeObject<PlayerStateModel>(playerState.ToString());

                GameInformation.Players = playerStateModel.Players;
                GameInformation.TableCards = playerStateModel.CommunityCards ?? new List<Card>();
                GameInformation.Hand = playerStateModel.HandCards ?? new List<Card>();
                GameInformation.GameInProgress = playerStateModel.GameInProgress;
                GameInformation.RaiseAmount = playerStateModel.RaiseAmount;
                if (GameInformation.GameInProgress == false)
                {
                    foreach (var gameInformationPlayer in GameInformation.Players)
                    {
                        gameInformationPlayer.IsPlaying = false;
                    }
                }

                GameInformation.Winner = null;

                StateHasChanged();
            });

            _hubConnection.On("ReceiveKick", async () =>
            {
                await LocalStorageService.RemoveItemAsync("currentTable");
                NavigationManager.NavigateTo("/");
            });

            await _hubConnection.StartAsync();

            await _hubConnection.SendAsync("AddToUsers", await LocalStorageService.GetItemAsync<int>("currentTable"));

            await base.OnInitializedAsync();

        }


        //private async Task GetPlayersList()
        //{
        //    var result = await GameSessionService.GetPlayers(await LocalStorageService.GetItemAsync<int>("currentTable"));
        //    var playerList = result.Select(name => new GamePlayer { Username = name }).ToList();
        //    GameInformation.Players = playerList;
        //    StateHasChanged();
        //}

        protected async Task LeaveTable()
        {
            await LocalStorageService.RemoveItemAsync("currentTable");
            await _hubConnection.StopAsync();
            NavigationManager.NavigateTo("/");

        }

        protected async void MarkReady()
        {
            var formModal = ModalService.Show<JoinTable>("Join table");
            var result = await formModal.Result;
            if (result.Cancelled) return;
            await _hubConnection.SendAsync("MarkReady", await LocalStorageService.GetItemAsync<int>("currentTable"), result.Data);

        }

        protected async void UnmarkReady()
        {
            await _hubConnection.SendAsync("UnmarkReady", await LocalStorageService.GetItemAsync<int>("currentTable"));

        }

        protected async Task Check()
        {
            await _hubConnection.SendAsync("ActionCheck", await LocalStorageService.GetItemAsync<int>("currentTable"));
        }

        protected async Task Fold()
        {
            await _hubConnection.SendAsync("ActionFold", await LocalStorageService.GetItemAsync<int>("currentTable"));
        }

        protected async Task Raise()
        {
            await _hubConnection.SendAsync("ActionRaise", await LocalStorageService.GetItemAsync<int>("currentTable"),1);
        }

        protected async Task Call()
        {
            await _hubConnection.SendAsync("ActionCall", await LocalStorageService.GetItemAsync<int>("currentTable"));
        }
    }
}
