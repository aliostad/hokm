namespace CardGame
{
    public class Card
    {
        public Rank Rank { get; }
        
        public Suit Suit { get; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString()
        {
            return $"{RankInfo.Name(Rank)}{SuitInfo.Icon(Suit)}";
        }
    }
}