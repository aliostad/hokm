using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace Hokm.GameServer
{
    public class MatchReportHubService : IHostLifetime, IMatchReportHub
    {
        private readonly IHubContext<MatchReporterHub> _reportContext;

        private bool _working = false;

        private List<IMachReportSink> _sinks = new List<IMachReportSink>();

        public MatchReportHubService(IHubContext<MatchReporterHub> reportContext)
        {
            _reportContext = reportContext;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _working = false;
            return Task.CompletedTask;
        }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            _working = true;
            return Task.CompletedTask;
        }

        public void ReportMatch(Match match)
        {
            throw new System.NotImplementedException();
        }

        public void StopReportingMatch(Match match)
        {
            throw new System.NotImplementedException();
        }

        public void AddSubscriber(IMachReportSink sink)
        {
            _sinks.Add(sink);
        }

        public void RemoveSubscriber(IMachReportSink sink)
        {
            _sinks.Remove(sink);
        }
    }
}