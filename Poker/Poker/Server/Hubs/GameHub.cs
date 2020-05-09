using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using MoreLinq;
using Newtonsoft.Json;
using Poker.Server.Models;
using Poker.Server.PokerEvaluators;
using Poker.Server.Repositories;
using Poker.Shared;

namespace Poker.Server.Hubs
{
    public class GameHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITableRepository _tableRepository;
        public static List<Game> Games { get; set; } = new List<Game>();
        public static List<User> Users { get; set; } = new List<User>();

        public GameHub(UserManager<ApplicationUser> userManager,
            ITableRepository tableRepository)
        {
            _userManager = userManager;
            _tableRepository = tableRepository;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await DisconnectPlayer();
            await Clients.All.SendAsync("ReceiveRefresh");
            await base.OnDisconnectedAsync(exception);
        }

        private async Task DisconnectPlayer()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(Context.User);
                Users.Remove(Users.FirstOrDefault(e => e.Name == Context.User.Identity.Name));
                await _tableRepository.RemoveUserFromTable(currentUser.CurrentTableId, currentUser.Id);
                currentUser.CurrentTableId = 0;
                await _userManager.UpdateAsync(currentUser);
            }
            catch (Exception)
            {
                //Log
            }
        }

        private static List<string> GetGamePlayers(int tableId)
        {
            return Users.Where(e => e.TableId == tableId).Select(e => e.ConnectionId).ToList();
        }

        public async Task AddToUsers(int tableId)
        {
            Users.Add(new User
            {
                Name = Context.User.Identity.Name,
                TableId = tableId,
                ConnectionId = Context.ConnectionId
            });


            await Clients.Clients(GetGamePlayers(tableId)).SendAsync("ReceiveAddedUser", Context.User.Identity.Name);
        }

        public async Task RemoveFromUsers(int tableId)
        {
            Users.Remove(Users.FirstOrDefault(e => e.Name == Context.User.Identity.Name));

            await Clients.Clients(GetGamePlayers(tableId)).SendAsync("ReceiveRemovedUser", Context.User.Identity.Name);
        }

        public async Task MarkReady(int tableId)
        {

            Users.FirstOrDefault(e => e.Name == Context.User.Identity.Name).IsReady = true;

            await Clients.Clients(GetGamePlayers(tableId)).SendAsync("ReceiveReadyState", Context.User.Identity.Name, true);

            if (Users.Count(e => e.IsReady) >= 2 && Games.All(e => e.TableId != tableId))
            {
                StartGame(tableId, 0);
            }
        }

        private async void  StartGame(int tableId, int smallBlindPosition)
        {
            //Initialize Game

            var newGame = new Game(tableId, smallBlindPosition);


            foreach (var user in Users.Where(user => user.IsReady && user.TableId == tableId))
            {
                newGame.Players.Add(new Player{Name = user.Name});
                user.InGame = true;
            }


            //Deal cards

            foreach (var player in newGame.Players)
            {
                player.HandCards.AddRange(newGame.Deck.DrawCards(2));
                var connectionId = Users.FirstOrDefault(e => e.Name == player.Name).ConnectionId;
                await Groups.AddToGroupAsync(connectionId, tableId.ToString());
                _ = Clients.Client(connectionId).SendAsync("ReceiveStartingHand", player.HandCards);
            }

            newGame.NormalizeAllIndexes();

            Games.Add(newGame);

            //Notify about the end of preparations

            await Clients.Group(tableId.ToString())
                .SendAsync("ReceivePlayingPlayers", newGame.Players.Select(e => e.Name).ToList());
            
            await Clients.Group(tableId.ToString())
                .SendAsync("ReceiveTurnPlayer", newGame.GetPlayerNameByIndex(newGame.Index));
        }

        public async Task ActionCheck(int tableId)
        {
            var currentGame = Games.First(e => e.TableId == tableId);

            var test = currentGame.GetPlayerNameByIndex(currentGame.Index);

            if (currentGame.Players.Any() &&
                currentGame.GetPlayerNameByIndex(currentGame.Index) == Context.User.Identity.Name)
            {
                Games.FirstOrDefault(e => e.TableId == tableId)?.SetIndex(currentGame.Index+1);

                if (Games.FirstOrDefault(e => e.TableId == tableId).Index == currentGame.RoundEndIndex)
                {
                    CommunityCardsController(tableId);
                }
                if (Games.FirstOrDefault(e => e.TableId == tableId).CommunityCardsActions ==
                    CommunityCardsActions.AfterRiver)
                {
                    Thread.Sleep(10000);

                    await Clients.Group(tableId.ToString())
                        .SendAsync("ReceiveNewGame");

                    Games.Remove(Games.FirstOrDefault(e => e.TableId == tableId));

                    if (Users.Count(e => e.IsReady) >= 2 && Games.All(e => e.TableId != tableId))
                    {
                        StartGame(tableId, currentGame.SmallBlindIndex + 1);
                    }
                }
                else
                {
                    await Clients.Group(tableId.ToString())
                        .SendAsync("ReceiveTurnPlayer",
                            currentGame.GetPlayerNameByIndex(Games.FirstOrDefault(e => e.TableId == tableId).Index));
                }
            }
        }

        public async void CommunityCardsController(int tableId)
        {
            var currentGame = Games.First(e => e.TableId == tableId);

            switch (currentGame.CommunityCardsActions)
            {
                case CommunityCardsActions.PreFlop:
                    var flop = Games.FirstOrDefault(e => e.TableId == tableId)?.Deck.DrawCards(3);
                    Games.FirstOrDefault(e => e.TableId == tableId)?.TableCards.AddRange(flop);
                    Games.FirstOrDefault(e => e.TableId == tableId).CommunityCardsActions++;
                    _ = Clients.Group(tableId.ToString())
                        .SendAsync("ReceiveFlop", flop);

                    break;

                case CommunityCardsActions.Flop:
                    var turn = Games.FirstOrDefault(e => e.TableId == tableId)?.Deck.DrawCards(1);
                    Games.FirstOrDefault(e => e.TableId == tableId)?.TableCards.AddRange(turn);
                    Games.FirstOrDefault(e => e.TableId == tableId).CommunityCardsActions++;
                    _ = Clients.Group(tableId.ToString())
                        .SendAsync("ReceiveTurnOrRiver", turn);
                    break;

                case CommunityCardsActions.Turn:
                    var river = Games.FirstOrDefault(e => e.TableId == tableId)?.Deck.DrawCards(1);
                    Games.FirstOrDefault(e => e.TableId == tableId)?.TableCards.AddRange(river);
                    Games.FirstOrDefault(e => e.TableId == tableId).CommunityCardsActions++;
                    _ = Clients.Group(tableId.ToString())
                        .SendAsync("ReceiveTurnOrRiver", river);
                    break;

                case CommunityCardsActions.River:
                    var communityCards = Games.FirstOrDefault(e => e.TableId == tableId).TableCards;
                    foreach (var player in Games.FirstOrDefault(e => e.TableId == tableId).Players)
                    {
                        player.HandStrength = HandEvaluation.Evaluate(communityCards.Concat(player.HandCards).ToList());
                    }

                    var winner = Games.FirstOrDefault(e => e.TableId == tableId)
                        .Players.MinBy(e => e.HandStrength).First().Name;
                    Games.FirstOrDefault(e => e.TableId == tableId).CommunityCardsActions++;

                    _ = Clients.Group(tableId.ToString())
                        .SendAsync("ReceiveWinner", winner);
                    break;
            }
        }

    }
}
