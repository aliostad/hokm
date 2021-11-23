namespace CardGame
{
    public enum Rank
    {
        Ace = 1,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }

    public static class RankInfo
    {
        private static readonly string[] _names = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        
        public static string Name(Rank r)
        {
            var index = (int)r - 1;
            return _names[index];
        }

        public static Rank? FromString(string s)
        {
            return s switch
            {
                "A" => Rank.Ace, 
                "2" => Rank.Two, 
                "3" => Rank.Three, 
                "4" => Rank.Four, 
                "5" => Rank.Five, 
                "6" => Rank.Six, 
                "7" => Rank.Seven, 
                "8" => Rank.Eight, 
                "9" => Rank.Nine, 
                "10" => Rank.Ten, 
                "J" => Rank.Jack, 
                "Q" => Rank.Queen, 
                "K" => Rank.King,
                _ => null
            };
        }
    }
}