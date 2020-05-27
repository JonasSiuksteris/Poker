namespace Poker.Shared
{
    public class Card
    {
        public CardRank CardNumber { get; set; }
        public CardSuit CardSuit { get; set; }

        public Card(CardRank cardNumber, CardSuit cardSuit)
        {
            CardNumber = cardNumber;
            CardSuit = cardSuit;
        }
    }
}
