using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using MoreLinq;
using Poker.Server.Models;
using Poker.Server.PokerEvaluators;
using Poker.Shared;
using Poker.Shared.Models;

namespace Poker.Server.Hubs
{
    public class GameHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public static List<Game> Games { get; set; } = new List<Game>();
        public static List<User> Users { get; set; } = new List<User>();

        public GameHub(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await DisconnectPlayer(Context.User.Identity.Name);
            await base.OnDisconnectedAsync(exception);
        }

        private async Task DisconnectPlayer(string user)
        {
            try
            {
                var tableId = Users.FirstOrDefault(e => e.Name == user)?.TableId ?? -1;
                var smallBlindIndexTemp = 0;

                if (Users.First(e => e.Name == user).InGame)
                {
                    foreach (var player in Games.SelectMany(game => game.Players.Where(player => player.Name == user)))
                    {
                        player.ActionState = PlayerActionState.Left;
                    }

                    if (Games.First(e => e.TableId == tableId).Players.Count(e => e.ActionState != PlayerActionState.Left) < 2)
                    {
                        _ =Clients.Group(tableId.ToString())
                            .SendAsync("ReceiveWinner", GetWinner(tableId));
                        smallBlindIndexTemp = Games.First(e => e.TableId == tableId).SmallBlindIndex;
                        Games.Remove(Games.FirstOrDefault(e => e.TableId == tableId));
                        Thread.Sleep(10000);
                    }
                }

                await Withdraw(user);
                Users.Remove(Users.FirstOrDefault(e => e.Name == user));
                foreach (var e in Users.Where(e => e.TableId == tableId))
                {
                    e.InGame = false;
                }
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
                SeatNumber = newSeatNumber,
                InGame = false,
                Balance = 0
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


        public async void RemoveFromUsers(int tableId)
        {
            await DisconnectPlayer(Context.User.Identity.Name);
        }
        
        public async Task MarkReady(int tableId, int depositAmount)
        {
            var user = Context.User.Identity.Name;
            await Deposit(depositAmount, user);

            Users.First(e => e.Name == user).IsReady = true;
            PlayerStateRefresh(tableId);

            if (Users.Where(e => e.TableId == tableId).Count(e => e.IsReady) >= 2 && Games.All(e => e.TableId != tableId))
            {
                StartGame(tableId, 0);
            }
        }

        private async Task Deposit(int depositAmount, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user != null && user.Currency >= depositAmount)
            {
                user.Currency -= depositAmount;
                await _userManager.UpdateAsync(user);
                Users.First(e => e.Name == user.UserName).Balance = depositAmount;
            }

        }

        public async Task UnmarkReady(int tableId)
        {
            var user = Context.User.Identity.Name;
            await Withdraw(user);
            Users.First(e => e.Name == user).IsReady = false;

            PlayerStateRefresh(tableId);
        }

        private async Task Withdraw(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            user.Currency += Users.First(e => e.Name == user.UserName).Balance;
            Users.First(e => e.Name == user.UserName).Balance = 0;
            await _userManager.UpdateAsync(user);
        }

        private async void  StartGame(int tableId, int smallBlindPosition)
        {
            //Initialize Game

            var newGame = new Game(tableId, smallBlindPosition);


            foreach (var user in Users.Where(user => user.IsReady && user.TableId == tableId))
            {
                newGame.Players.Add(new Player{Name = user.Name, RoundBet = 0});
                user.InGame = true;
            }

            PlayerStateRefresh(tableId);

            //Deal cards

            foreach (var player in newGame.Players)
            {
                if(Users.Where(e => e.TableId == tableId && e.InGame).Select(e => e.Name).ToList().Contains(player.Name))
                {
                    player.HandCards.AddRange(newGame.Deck.DrawCards(2));
                    var connectionId = Users.First(e => e.Name == player.Name).ConnectionId;
                    if (Users.First(e => e.Name == player.Name).InGame)
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

        public async Task ActionFold(int tableId)
        {
            var currentGame = Games.First(e => e.TableId == tableId);

            if (currentGame.Players.Any() &&
                currentGame.GetPlayerNameByIndex(currentGame.Index) == Context.User.Identity.Name)
            {
                //PlayerFolded
                Games.First(e => e.TableId == tableId).Players
                    .First(e => e.Name == Context.User.Identity.Name).ActionState = PlayerActionState.Folded;
                //CheckIfOnlyOneLeft
                if (Games.First(e => e.TableId == tableId).Players
                        .Count(e => e.ActionState == PlayerActionState.Playing) == 1)
                {
                    var winner = GetWinner(tableId);
                    _ = Clients.Group(tableId.ToString())
                        .SendAsync("ReceiveWinner", winner);

                    Thread.Sleep(10000);

                    Games.Remove(Games.FirstOrDefault(e => e.TableId == tableId));

                    if (Users.Where(e => e.TableId == tableId).Count(e => e.IsReady) >= 2 && Games.All(e => e.TableId != tableId))
                    {
                        StartGame(tableId, currentGame.SmallBlindIndex + 1);
                    }
                    else
                    {
                        foreach (var e in Users.Where(e => e.TableId == tableId))
                        {
                            e.InGame = false;
                        }
                        PlayerStateRefresh(tableId);
                    }
                }
                else
                {
                    await MoveIndex(tableId, currentGame);
                }
            }
        }

        public async Task ActionCheck(int tableId)
        {
            var currentGame = Games.First(e => e.TableId == tableId);

            if (currentGame.Players.Any() &&
                currentGame.GetPlayerNameByIndex(currentGame.Index) == Context.User.Identity.Name)
            {

                await MoveIndex(tableId, currentGame);
            }
        }

        public async Task ActionRaise(int tableId, int raiseAmount)
        {
            var currentGame = Games.First(e => e.TableId == tableId);
            var currentPlayer = currentGame.Players.First(e => e.Name == Context.User.Identity.Name);

            if (currentGame.Players.Any() &&
                currentGame.GetPlayerNameByIndex(currentGame.Index) == Context.User.Identity.Name &&
                Users.First(e => e.Name == Context.User.Identity.Name).Balance > raiseAmount + currentGame.RaiseAmount - currentPlayer.RoundBet)
            {
                Users.First(e => e.Name == Context.User.Identity.Name).Balance -= raiseAmount + currentGame.RaiseAmount - currentPlayer.RoundBet;

                Games.First(e => e.TableId == tableId).Players.First(e => e.Name == Context.User.Identity.Name).RoundBet = raiseAmount + currentGame.RaiseAmount;

                Games.First(e => e.TableId == tableId).RaiseAmount += raiseAmount;

                Games.First(e => e.TableId == tableId).RoundEndIndex =
                    Games.First(e => e.TableId == tableId).Index;

                await MoveIndex(tableId, currentGame);
            }
        }

        public async Task ActionCall(int tableId)
        {
            var currentGame = Games.First(e => e.TableId == tableId);
            var currentPlayer = currentGame.Players.First(e => e.Name == Context.User.Identity.Name);

            if (currentGame.Players.Any() &&
                currentGame.GetPlayerNameByIndex(currentGame.Index) == Context.User.Identity.Name &&
                currentGame.RaiseAmount > 0)
            {
                if (currentGame.RaiseAmount - currentPlayer.RoundBet <
                    Users.First(e => e.TableId == tableId && e.Name == Context.User.Identity.Name).Balance)
                {
                    Users.First(e => e.Name == Context.User.Identity.Name).Balance -= currentGame.RaiseAmount - currentPlayer.RoundBet;
                    Games.First(e => e.TableId == tableId).Players.First(e => e.Name == Context.User.Identity.Name).RoundBet = currentGame.RaiseAmount;
                }
                else if (currentGame.RaiseAmount - currentPlayer.RoundBet >=
                         Users.First(e => e.TableId == tableId && e.Name == Context.User.Identity.Name).Balance)
                {
                    var allInSum = Users.First(e => e.Name == Context.User.Identity.Name).Balance;
                    Users.First(e => e.Name == Context.User.Identity.Name).Balance = 0;
                    Games.First(e => e.TableId == tableId).Players.First(e => e.Name == Context.User.Identity.Name).RoundBet = allInSum;
                }
                await MoveIndex(tableId, currentGame);

            }
        }

        public async Task ActionAllIn(int tableId)
        {
            var currentGame = Games.First(e => e.TableId == tableId);
            var currentPlayer = currentGame.Players.First(e => e.Name == Context.User.Identity.Name);

            if (currentGame.Players.Any() &&
                currentGame.GetPlayerNameByIndex(currentGame.Index) == Context.User.Identity.Name &&
                Users.First(e => e.Name == Context.User.Identity.Name).Balance > currentGame.RaiseAmount - currentPlayer.RoundBet)
            {
                var allInSum = Users.First(e => e.Name == Context.User.Identity.Name).Balance;
                Users.First(e => e.Name == Context.User.Identity.Name).Balance = 0;
                Games.First(e => e.TableId == tableId).Players.First(e => e.Name == Context.User.Identity.Name).RoundBet = allInSum;
                Games.First(e => e.TableId == tableId).RaiseAmount += allInSum;

                Games.First(e => e.TableId == tableId).RoundEndIndex =
                    Games.First(e => e.TableId == tableId).Index;

                await MoveIndex(tableId, currentGame);
            }
        }


        private async Task MoveIndex(int tableId, Game currentGame)
        {
            do
            {
                currentGame.SetIndex(currentGame.Index + 1);
                Games.FirstOrDefault(e => e.TableId == tableId)?.SetIndex(currentGame.Index);

                if (Games.First(e => e.TableId == tableId).Index == currentGame.RoundEndIndex)
                {
                    CommunityCardsController(tableId);
                    Games.First(e => e.TableId == tableId).RaiseAmount = 0;
                    Games.First(e => e.TableId == tableId).RoundEndIndex = Games.First(e => e.TableId == tableId).BigBlindIndex + 1;
                    Games.First(e => e.TableId == tableId).Index = Games.First(e => e.TableId == tableId).BigBlindIndex + 1;
                    Games.First(e => e.TableId == tableId).NormalizeAllIndexes();
                    foreach (var player in Games.First(e => e.TableId == tableId).Players)
                    {
                        player.RoundBet = 0;
                    }
                }
            } while (currentGame.GetPlayerByIndex(currentGame.Index).ActionState != PlayerActionState.Playing);

            if (Games.First(e => e.TableId == tableId).CommunityCardsActions ==
                CommunityCardsActions.AfterRiver)
            {
                Thread.Sleep(10000);

                Games.Remove(Games.FirstOrDefault(e => e.TableId == tableId));

                if (Users.Where(e => e.TableId == tableId).Count(e => e.IsReady) >= 2 && Games.All(e => e.TableId != tableId))
                {
                    StartGame(tableId, currentGame.SmallBlindIndex + 1);
                }
                else
                {
                    foreach (var e in Users.Where(e => e.TableId == tableId))
                    {
                        e.InGame = false;
                    }
                    PlayerStateRefresh(tableId);
                }
            }
            else
            {
                PlayerStateRefresh(tableId);
                await Clients.Group(tableId.ToString())
                    .SendAsync("ReceiveTurnPlayer",
                        currentGame.GetPlayerNameByIndex(Games.First(e => e.TableId == tableId).Index));
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
                    Games.First(e => e.TableId == tableId).CommunityCardsActions++;
                    _ = Clients.Group(tableId.ToString())
                        .SendAsync("ReceiveFlop", flop);

                    break;

                case CommunityCardsActions.Flop:
                    var turn = Games.FirstOrDefault(e => e.TableId == tableId)?.Deck.DrawCards(1);
                    Games.FirstOrDefault(e => e.TableId == tableId)?.TableCards.AddRange(turn);
                    Games.First(e => e.TableId == tableId).CommunityCardsActions++;
                    _ = Clients.Group(tableId.ToString())
                        .SendAsync("ReceiveTurnOrRiver", turn);
                    break;

                case CommunityCardsActions.Turn:
                    var river = Games.FirstOrDefault(e => e.TableId == tableId)?.Deck.DrawCards(1);
                    Games.FirstOrDefault(e => e.TableId == tableId)?.TableCards.AddRange(river);
                    Games.First(e => e.TableId == tableId).CommunityCardsActions++;
                    _ = Clients.Group(tableId.ToString())
                        .SendAsync("ReceiveTurnOrRiver", river);
                    break;

                case CommunityCardsActions.River:
                    var winner = GetWinner(tableId);
                    _ = Clients.Group(tableId.ToString())
                        .SendAsync("ReceiveWinner", winner);
                    Games.First(e => e.TableId == tableId).CommunityCardsActions++;
                    break;
            }
        }

        public string GetWinner(int tableId)
        {
            var communityCards = Games.First(e => e.TableId == tableId).TableCards;
            var evaluatedPlayers = new List<Player>();
            foreach (var player in Games.First(e => e.TableId == tableId).Players.Where(e => e.ActionState == PlayerActionState.Playing))
            {
                player.HandStrength = HandEvaluation.Evaluate(communityCards.Concat(player.HandCards).ToList());
                evaluatedPlayers.Add(player);
            }

            return evaluatedPlayers.Where(e => e.ActionState == PlayerActionState.Playing).MinBy(e => e.HandStrength).First().Name;
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
                    SeatNumber = user.SeatNumber,
                    GameMoney = user.Balance
                });

                if (Games.FirstOrDefault(e => e.TableId == tableId) != null && 
                    Games.First(e => e.TableId == tableId).Players.Select(e => e.Name).Contains(user.Name))
                {
                    playerState.Players.Last().ActionState = Games.First(e => e.TableId == tableId).Players
                        .First(e => e.Name == user.Name).ActionState;
                }
            }

            playerState.CommunityCards = Games.FirstOrDefault(e => e.TableId == tableId)?.TableCards;

            playerState.GameInProgress = playerState.CommunityCards != null;

            if (Games.FirstOrDefault(e => e.TableId == tableId)?.RaiseAmount > 0)
                playerState.RaiseAmount = Games.First(e => e.TableId == tableId).RaiseAmount;

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
            }
        }
    }
}
