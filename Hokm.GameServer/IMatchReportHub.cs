using System.Text.RegularExpressions;

namespace Hokm.GameServer
{
    public interface IMatchReportHub
    {
        void ReportMatch(Match match);

        void StopReportingMatch(Match match);

        void AddSubscriber(IMachReportSink sink);
        
        void RemoveSubscriber(IMachReportSink sink);
    }
}