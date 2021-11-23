namespace CardGame
{
    public enum Suit
    {
        Club, // ♣
        Diamond, // ♦
        Heart, // ♥
        Spade // ♠
    }
    
    public static class SuitInfo
    {
        private static readonly string[] _names = new[] { "Club", "Diamond", "Heart", "Spade" };
        private static readonly string[] _icons = new[] { "♣", "♦", "♥", "♠" };
        
        public static string Name(Suit s)
        {
            return _names[(int) s];
        }

        public static string Icon(Suit s)
        {
            return _icons[(int)s];
        }
    }
}