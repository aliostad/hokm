using System.Threading.Tasks;

namespace Hokm.GameServer
{
    public interface IMachReportSink
    {
        Task ReportAsync(MatchInfo info);
    }
}