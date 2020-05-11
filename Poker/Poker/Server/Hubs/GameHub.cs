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
using Poker.Shared.Models;

namespace Poker.Server.Hubs
{
    public class GameHub : Hub
    {
        public static List<Game> Games { get; set; } = new List<Game>();
        public static List<User> Users { get; set; } = new List<User>();

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            DisconnectPlayer(Context.User.Identity.Name);
            await base.OnDisconnectedAsync(exception);
        }

        private void DisconnectPlayer(string user)
        {
            try
            {
                var tableId = Users.FirstOrDefault(e => e.Name == user)?.TableId ?? -1;
                var smallBlindIndexTemp = 0;

                if (Users.FirstOrDefault(e => e.Name == user).InGame)
                {
                    foreach (var player in Games.SelectMany(game => game.Players.Where(player => player.Name == user)))
                    {
                        player.Name += "_Left";
                        player.Left = true;
                    }

                    if (Games.FirstOrDefault(e => e.TableId == tableId).Players.Count(e => !e.Left) < 2)
                    {
                        Clients.Group(tableId.ToString())
                            .SendAsync("ReceiveWinner", GetWinner(tableId));
                        smallBlindIndexTemp = Games.FirstOrDefault(e => e.TableId == tableId).SmallBlindIndex;
                        Games.Remove(Games.FirstOrDefault(e => e.TableId == tableId));
                        Thread.Sleep(10000);
                    }
                }
                Users.Remove(Users.FirstOrDefault(e => e.Name == user));
                if (Users.Count(e => e.IsReady) >= 2 && Games.All(e => e.TableId != tableId))
                {
                    StartGame(tableId, smallBlindIndexTemp + 1);
                }
                else
                {
                    PlayerStateRefresh(tableId);
                }
            }
            catch (Exception)
            {
                //Log
            }
        }

        public async void AddToUsers(int tableId)
        {
            if (Users.Any(e => e.Name == Context.User.Identity.Name))
            {
                _ =Clients.Client(Context.ConnectionId).SendAsync("ReceiveKick");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, tableId.ToString());

            var newSeatNumber = AssignTableToUser(tableId);
            var name = Context.User.Identity.Name;

            Users.Add(new User
            {
                Name = name,
                TableId = tableId,
                ConnectionId = Context.ConnectionId,
                SeatNumber = newSeatNumber

            });

            PlayerStateRefresh(tableId);
        }

        private static int AssignTableToUser(int tableId)
        {
            var occupiedSeats = Users.Where(e => e.TableId == tableId).Select(e => e.SeatNumber).OrderBy(e => e).ToList();
            for (var i = 0; i < occupiedSeats.Count; i++)
            {
                if (occupiedSeats[i] != i)
                    return i;
            }
            return occupiedSeats.Count;

        }


        public void RemoveFromUsers(int tableId)
        {
            DisconnectPlayer(Context.User.Identity.Name);
        }

        public void MarkReady(int tableId)
        {

            Users.FirstOrDefault(e => e.Name == Context.User.Identity.Name).IsReady = true;

            PlayerStateRefresh(tableId);

            if (Users.Where(e => e.TableId == tableId).Count(e => e.IsReady) >= 2 && Games.All(e => e.TableId != tableId))
            {
                StartGame(tableId, 0);
            }
        }

        public void UnmarkReady(int tableId)
        {
            Users.FirstOrDefault(e => e.Name == Context.User.Identity.Name).IsReady = false;

            PlayerStateRefresh(tableId);
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

            PlayerStateRefresh(tableId);

            //Deal cards

            foreach (var player in newGame.Players)
            {
                if(Users.Where(e => e.TableId == tableId && e.InGame).Select(e => e.Name).ToList().Contains(player.Name))
                {
                    player.HandCards.AddRange(newGame.Deck.DrawCards(2));
                    var connectionId = Users.FirstOrDefault(e => e.Name == player.Name).ConnectionId;
                    if (Users.FirstOrDefault(e => e.Name == player.Name).InGame)
                        _ = Clients.Client(connectionId).SendAsync("ReceiveStartingHand", player.HandCards);
                }
            }

            newGame.NormalizeAllIndexes();

            Games.Add(newGame);

            //Notify about the end of preparations

            PlayerStateRefresh(tableId);

            await Clients.Group(tableId.ToString())
                .SendAsync("ReceiveTurnPlayer", newGame.GetPlayerNameByIndex(newGame.Index));
        }

        public void ActionFold(int tableId)
        {

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

                    //await Clients.Group(tableId.ToString())
                    //    .SendAsync("ReceiveNewGame");

                    Games.Remove(Games.FirstOrDefault(e => e.TableId == tableId));

                    if (Users.Where(e => e.TableId == tableId).Count(e => e.IsReady) >= 2 && Games.All(e => e.TableId != tableId))
                    {
                        StartGame(tableId, currentGame.SmallBlindIndex + 1);
                    }
                    else
                    {
                        PlayerStateRefresh(tableId);
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

        public void CommunityCardsController(int tableId)
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
                    _ = Clients.Group(tableId.ToString())
                        .SendAsync("ReceiveWinner", GetWinner(tableId));
                    Games.FirstOrDefault(e => e.TableId == tableId).CommunityCardsActions++;
                    break;
            }
        }

        public string GetWinner(int tableId)
        {
            var communityCards = Games.FirstOrDefault(e => e.TableId == tableId).TableCards;
            foreach (var player in Games.FirstOrDefault(e => e.TableId == tableId).Players.Where(e => !e.Left))
            {
                player.HandStrength = HandEvaluation.Evaluate(communityCards.Concat(player.HandCards).ToList());
            }

            return Games.FirstOrDefault(e => e.TableId == tableId)
                .Players.Where(e => !e.Left).MinBy(e => e.HandStrength).First().Name;
        }

        public void PlayerStateRefresh(int tableId)
        {
            var playerState = new PlayerStateModel();

            foreach (var user in Users.Where(e => e.TableId == tableId))
            {
                playerState.Players.Add(new GamePlayer
                {
                    Username = user.Name,
                    IsPlaying = user.InGame,
                    IsReady = user.IsReady,
                    SeatNumber = user.SeatNumber
                });
            }

            playerState.CommunityCards = Games.FirstOrDefault(e => e.TableId == tableId)?.TableCards;

            playerState.GameInProgress = playerState.CommunityCards != null;
            var gamePlayers = Games.FirstOrDefault(e => e.TableId == tableId)?.Players;

            if (gamePlayers == null)
            {
                Clients.Group(tableId.ToString()).SendAsync("ReceiveStateRefresh", playerState);
            }
            else
            {
                foreach (var user in Users.Where(e => e.TableId == tableId))
                {
                    playerState.HandCards = gamePlayers.FirstOrDefault(e => e.Name == user.Name)?.HandCards;
                    Clients.Client(user.ConnectionId).SendAsync("ReceiveStateRefresh", playerState);
                }
                //foreach (var player in gamePlayers)
                //{
                //    playerState.HandCards = player.HandCards;
                //    var connectionId = Users.FirstOrDefault(e => e.Name == player.Name)?.ConnectionId;
                //    if(connectionId != null)
                //        Clients.Client(connectionId).SendAsync("ReceiveStateRefresh", playerState);
                //}
            }




        }
    }
}
