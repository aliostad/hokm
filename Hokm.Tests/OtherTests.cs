using Xunit;

namespace Hokm.Tests
{
    public class MatchScoreTests
    {
        [Fact]
        public void DoesKoti()
        {
            var caller = PlayerPosition.Team1Player1;
            var gs = new GameScore();
            for (int i = 0; i < 7; i++)
                gs.RegisterWin(caller);
            var ms = new MatchScore();
            ms.RegisterGameWin(gs, caller);
            Assert.Equal(2, ms.Team1Points);
        }
        
        [Fact]
        public void DoesHaakemKoti()
        {
            var caller = PlayerPosition.Team1Player1;
            var gs = new GameScore();
            for (int i = 0; i < 7; i++)
                gs.RegisterWin(PlayerPosition.Team2Player1);
            var ms = new MatchScore();
            ms.RegisterGameWin(gs, caller);
            Assert.Equal(3, ms.Team2Points);
        }
        
    }
}