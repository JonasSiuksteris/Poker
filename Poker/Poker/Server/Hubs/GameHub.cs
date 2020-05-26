using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Poker.Server.Models;
using Poker.Server.PokerEvaluators;
using Poker.Server.Repositories;
using Poker.Shared;
using Poker.Shared.Models;

namespace Poker.Server.Hubs
{
    public class GameHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITableRepository _tableRepository;

        public static List<Game> Games { get; set; } = new List<Game>();
        public static List<User> Users { get; set; } = new List<User>();

        public GameHub(UserManager<ApplicationUser> userManager, ITableRepository tableRepository)
        {
            _userManager = userManager;
            _tableRepository = tableRepository;
        }

        public async Task SendMessage(int tableId, string message)
        {
            var newMessage = new GetMessageResult{Sender = Context.User.Identity.Name, Message = message};
            await Clients.Groups(tableId.ToString()).SendAsync("ReceiveMessage", newMessage);
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
                        UpdatePot(tableId);
                        GetAndAwardWinners(tableId);
                        PlayerStateRefresh(tableId);
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
                    await StartGame(tableId, smallBlindIndexTemp + 1);
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

        public async Task AddToUsers(int tableId)
        {
            var currentTable = await _tableRepository.GetTableById(tableId);

            if (Users.Count(e => e.TableId == tableId) >= currentTable.MaxPlayers)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveKick");
                return;
            }

            if (Users.Any(e => e.Name == Context.User.Identity.Name))
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveKick");
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


        public async Task RemoveFromUsers(int tableId)
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
                await StartGame(tableId, 0);
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

        private async Task StartGame(int tableId, int smallBlindPosition)
        {
            //Initialize Game
            var currentTableInfo = await _tableRepository.GetTableById(tableId);

            var newGame = new Game(tableId, smallBlindPosition, currentTableInfo.SmallBlind);

            //Adding players to table
            foreach (var user in Users.Where(user => user.IsReady && user.TableId == tableId))
            {
                newGame.Players.Add(new Player{Name = user.Name, RoundBet = 0});
                user.InGame = true;
            }

            newGame.NormalizeAllIndexes();

            //Small blind
            if (Users.First(e => e.Name == newGame.GetPlayerNameByIndex(newGame.SmallBlindIndex)).Balance >=
                newGame.SmallBlind)
            {
                Users.First(e => e.Name == newGame.GetPlayerNameByIndex(newGame.SmallBlindIndex)).Balance -=
                    newGame.SmallBlind;
                newGame.Players.First(e => e.Name == newGame.GetPlayerNameByIndex(newGame.SmallBlindIndex)).RoundBet +=
                    newGame.SmallBlind;
            }
            else
            {
                newGame.Players.First(e => e.Name == newGame.GetPlayerNameByIndex(newGame.SmallBlindIndex)).RoundBet +=
                    Users.First(e => e.Name == newGame.GetPlayerNameByIndex(newGame.SmallBlindIndex)).Balance;
                Users.First(e => e.Name == newGame.GetPlayerNameByIndex(newGame.SmallBlindIndex)).Balance = 0;
            }

            //Big blind
            if (Users.First(e => e.Name == newGame.GetPlayerNameByIndex(newGame.BigBlindIndex)).Balance >=
    newGame.SmallBlind * 2)
            {
                Users.First(e => e.Name == newGame.GetPlayerNameByIndex(newGame.BigBlindIndex)).Balance -=
                    newGame.SmallBlind * 2;
                newGame.Players.First(e => e.Name == newGame.GetPlayerNameByIndex(newGame.BigBlindIndex)).RoundBet +=
                    newGame.SmallBlind * 2;
            }
            else
            {
                newGame.Players.First(e => e.Name == newGame.GetPlayerNameByIndex(newGame.BigBlindIndex)).RoundBet +=
                    Users.First(e => e.Name == newGame.GetPlayerNameByIndex(newGame.BigBlindIndex)).Balance;
                Users.First(e => e.Name == newGame.GetPlayerNameByIndex(newGame.BigBlindIndex)).Balance = 0;
            }

            //Deal cards

            foreach (var player in newGame.Players)
            {
                if(Users.Where(e => e.TableId == tableId && e.InGame).Select(e => e.Name).ToList().Contains(player.Name))
                {
                    player.HandCards.AddRange(newGame.Deck.DrawCards(2));
                    var connectionId = Users.First(e => e.Name == player.Name).ConnectionId;
                    if (Users.First(e => e.Name == player.Name).InGame) 
                        await Clients.Client(connectionId).SendAsync("ReceiveStartingHand", player.HandCards);
                }
            }

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

                //Remove from pots
                foreach (var pot in Games.First(e => e.TableId == tableId).Winnings)
                {
                    pot.Players.Remove(Context.User.Identity.Name);
                }
                
                //CheckIfOnlyOneLeft
                if (Games.First(e => e.TableId == tableId).Players
                        .Count(e => e.ActionState == PlayerActionState.Playing) == 1)
                {
                    UpdatePot(tableId);
                    GetAndAwardWinners(tableId);
                    PlayerStateRefresh(tableId);

                    Thread.Sleep(10000);

                    Games.Remove(Games.FirstOrDefault(e => e.TableId == tableId));

                    if (Users.Where(e => e.TableId == tableId).Count(e => e.IsReady) >= 2 && Games.All(e => e.TableId != tableId))
                    {
                        await StartGame(tableId, currentGame.SmallBlindIndex + 1);
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
                    UpdatePot(tableId);
                    Games.First(e => e.TableId == tableId).RaiseAmount = 0;
                    Games.First(e => e.TableId == tableId).RoundEndIndex = Games.First(e => e.TableId == tableId).BigBlindIndex + 1;
                    Games.First(e => e.TableId == tableId).Index = Games.First(e => e.TableId == tableId).BigBlindIndex + 1;
                    Games.First(e => e.TableId == tableId).NormalizeAllIndexes();
                    foreach (var player in Games.First(e => e.TableId == tableId).Players)
                    {
                        player.RoundBet = 0;
                    }
                }
            } while ((currentGame.GetPlayerByIndex(currentGame.Index).ActionState != PlayerActionState.Playing || Users.First(e => e.Name == currentGame.GetPlayerByIndex(currentGame.Index).Name).Balance == 0
                     || currentGame.Players.Count(e => e.ActionState == PlayerActionState.Playing) < 2 ) && Games.First(e => e.TableId == tableId).CommunityCardsActions !=
                     CommunityCardsActions.AfterRiver);

            if (Games.First(e => e.TableId == tableId).CommunityCardsActions ==
                CommunityCardsActions.AfterRiver)
            {
                Thread.Sleep(10000);

                Games.Remove(Games.FirstOrDefault(e => e.TableId == tableId));

                if (Users.Where(e => e.TableId == tableId).Count(e => e.IsReady) >= 2 && Games.All(e => e.TableId != tableId))
                {
                    await StartGame(tableId, currentGame.SmallBlindIndex + 1);
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
                    Clients.Group(tableId.ToString())
                        .SendAsync("ReceiveFlop", flop);

                    break;

                case CommunityCardsActions.Flop:
                    var turn = Games.FirstOrDefault(e => e.TableId == tableId)?.Deck.DrawCards(1);
                    Games.FirstOrDefault(e => e.TableId == tableId)?.TableCards.AddRange(turn);
                    Games.First(e => e.TableId == tableId).CommunityCardsActions++;
                    Clients.Group(tableId.ToString())
                        .SendAsync("ReceiveTurnOrRiver", turn);
                    break;

                case CommunityCardsActions.Turn:
                    var river = Games.FirstOrDefault(e => e.TableId == tableId)?.Deck.DrawCards(1);
                    Games.FirstOrDefault(e => e.TableId == tableId)?.TableCards.AddRange(river);
                    Games.First(e => e.TableId == tableId).CommunityCardsActions++;
                    Clients.Group(tableId.ToString())
                        .SendAsync("ReceiveTurnOrRiver", river);
                    break;

                case CommunityCardsActions.River:
                    GetAndAwardWinners(tableId);
                    PlayerStateRefresh(tableId);
                    Games.First(e => e.TableId == tableId).CommunityCardsActions++;
                    break;
            }
        }

        private void UpdatePot(int tableId)
        {
            var players = Games.First(e => e.TableId == tableId)
                .Players.Where(player => player.RoundBet > 0 && player.ActionState == PlayerActionState.Playing)
                .ToList();

            while (players.Any())
            {
                var pot = new Pot { PotAmount = players.Min(e => e.RoundBet) };

                foreach (var player in players)
                {
                    player.RoundBet -= pot.PotAmount;
                    pot.Players.Add(player.Name);
                }

                pot.PotAmount *= players.Count;

                if (Games.First(e => e.TableId == tableId).Winnings
                        .Count(winningPot => winningPot.Players.SetEquals(pot.Players)) > 0)
                {
                    Games.First(e => e.TableId == tableId).Winnings.First(e => e.Players.SetEquals(pot.Players)).PotAmount +=
                        pot.PotAmount;
                }
                else
                {
                    Games.First(e => e.TableId == tableId).Winnings.Add(pot);
                }

                players = players.Where(e => e.RoundBet > 0).ToList();
            }
        }

        private void GetAndAwardWinners(int tableId)
        {
            var communityCards = Games.First(e => e.TableId == tableId).TableCards;
            var evaluatedPlayers = new Hashtable();

            foreach (var player in Games.First(e => e.TableId == tableId).Players.Where(e => e.ActionState == PlayerActionState.Playing))
            {
                player.HandStrength = HandEvaluation.Evaluate(communityCards.Concat(player.HandCards).ToList());
                evaluatedPlayers.Add(player.Name, player.HandStrength);
            }

            foreach (var pot in Games.First(e => e.TableId == tableId).Winnings)
            {
                var highestHand = HandStrength.Nothing;
                string winner = null;
                foreach (var potPlayer in pot.Players.Where(potPlayer => highestHand > (HandStrength) evaluatedPlayers[potPlayer]))
                {
                    highestHand = (HandStrength) evaluatedPlayers[potPlayer];
                    winner = potPlayer;
                }
                pot.Winner = winner;
                Users.First(e => e.Name == pot.Winner).Balance += pot.PotAmount;
            }
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

            playerState.Pots = Games.FirstOrDefault(e => e.TableId == tableId)?.Winnings;

            if(Games.FirstOrDefault(e => e.TableId == tableId) != null)
                playerState.SmallBlind = Games.First(e => e.TableId == tableId).SmallBlind;

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
