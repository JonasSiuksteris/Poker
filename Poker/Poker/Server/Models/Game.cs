using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Poker.Shared;

namespace Poker.Server.Models
{
    public class Game
    {
        public Deck Deck { get; set; }
        public List<Card> TableCards { get; set; }

        public List<Player> Players { get; set; }

        public int TableId { get; set; }

        public int Index { get; set; }

        public int RoundEndIndex { get; set; }

        public int SmallBlindIndex { get; set; }

        public int BigBlindIndex { get; set; }

        public CommunityCardsActions CommunityCardsActions { get; set; }

        public Game(int tableId, int smallBlindIndex)
        {
            TableId = tableId;
            Players = new List<Player>();
            TableCards = new List<Card>();
            Deck = new Deck();
            SmallBlindIndex = smallBlindIndex;
            BigBlindIndex = smallBlindIndex + 1;
            RoundEndIndex = smallBlindIndex + 2;
            Index = smallBlindIndex + 2;
            CommunityCardsActions = CommunityCardsActions.PreFlop;
        }
        public int NormalizeIndex(int index)
        {
            return index % Players.Count;
        }

        public void NormalizeAllIndexes()
        {
            SmallBlindIndex = NormalizeIndex(SmallBlindIndex);
            BigBlindIndex = NormalizeIndex(BigBlindIndex);
            RoundEndIndex = NormalizeIndex(RoundEndIndex);
            Index = NormalizeIndex(Index);
        }

        public void SetIndex(int index)
        {
            Index = NormalizeIndex(index);
        }

        public void SetRoundEndIndex(int index)
        {
            RoundEndIndex = NormalizeIndex(index);
        }

        public string GetPlayerNameByIndex(int index)
        {
            return Players.ElementAt(index)?.Name;
        }
    }
}
