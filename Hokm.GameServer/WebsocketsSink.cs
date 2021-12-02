using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Hokm.GameServer
{
    public class WebsocketsSink : IMachReportSink
    {
        private readonly IHubContext<MatchReporterHub> _reportContext;

        public WebsocketsSink(IHubContext<MatchReporterHub> reportContext)
        {
            _reportContext = reportContext;
        }

        public Task ReportAsync(MatchInfo info)
        {
            return _reportContext.Clients.All.SendAsync("report", info);
        }
    }
}