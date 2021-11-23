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
    }
}