using System;
using System.Collections.Generic;
using System.Linq;
using Poker.Shared;

namespace Poker.Server.PokerEvaluators
{
    public static class HandEvaluation
    {
        public static HandStrength Evaluate(List<Card> cards)
        {
            cards = cards.OrderBy(c => c.CardNumber).ToList();

            if (RoyalFlush(cards))
                return HandStrength.RoyalFlush;
            if (StraightFlush(cards))
                return HandStrength.StraightFlush;
            if (FourOfAKind(cards))
                return HandStrength.FourOfAKind;
            if (FullHouse(cards))
                return HandStrength.FullHouse;
            if (Flush(cards))
                return HandStrength.Flush;
            if (Straight(cards))
                return HandStrength.Straight;
            if (ThreeOfAKind(cards))
                return HandStrength.ThreeOfAKind;
            if (TwoPair(cards))
                return HandStrength.TwoPair;
            if (OnePair(cards))
                return HandStrength.OnePair;
            return HandStrength.HighCard;
        }

        private static bool OnePair(List<Card> cards)
        {
            return cards.GroupBy(c => c.CardNumber)
                       .Count(c => c.Count() == 2) == 1;
        }

        private static bool TwoPair(List<Card> cards)
        {
            return cards.GroupBy(c => c.CardNumber)
                       .Count(c => c.Count() == 2) == 2;
        }

        private static bool ThreeOfAKind(List<Card> cards)
        {
            return cards.GroupBy(c => c.CardNumber)
                       .Count(c => c.Count() == 3) == 1;
        }

        private static bool Straight(List<Card> cards)
        {
            for (var i = 1; i < cards.Count; i++)
            {
                if (i == 1 && cards.First().CardNumber == CardRank.Ace && cards.Last().CardNumber == CardRank.King) continue;
                if (cards[i].CardNumber - 1 != cards[i - 1].CardNumber)
                    return false;
            }

            return true;
        }

        private static bool Flush(List<Card> cards)
        {
            return cards.GroupBy(c => c.CardSuit)
                       .Count(c => c.Count() == 5) == 1;
        }

        private static bool FullHouse(List<Card> cards)
        {
            return cards.GroupBy(c => c.CardNumber)
                .All(c => c.Count() == 3 || c.Count() == 2);
            
        }

        private static bool FourOfAKind(List<Card> cards)
        {
            return cards.GroupBy(c => c.CardNumber)
                       .Count(c => c.Count() == 4) == 1;
        }

        private static bool StraightFlush(List<Card> cards)
        {
            return Straight(cards) && Flush(cards);
        }

        private static bool RoyalFlush(List<Card> cards)
        {
            return cards[0].CardNumber == CardRank.Ace && cards[1].CardNumber == CardRank.Ten && cards[2].CardNumber == CardRank.Jack
                   && cards[3].CardNumber == CardRank.Queen && cards[4].CardNumber == CardRank.King && Flush(cards);
        }
    }
}