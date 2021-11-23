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

        public override bool Equals(object obj)
        {
            if (obj is not Card card)
                return false;

            return card.ToString() == ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static Card? FromString(string s)
        {
            if (s == null)
                return null;
            
            var len = s.Length;
            if (len != 2 && len != 3)
                return null;

            var suitString = s.Substring(len - 1);
            var rankString = s.Substring(0, len - 1);

            var rank = RankInfo.FromString(rankString);
            var suit = SuitInfo.FromString(suitString);

            if (!rank.HasValue || !suit.HasValue)
                return null;

            return new Card(suit.Value, rank.Value);
        }
    }
}