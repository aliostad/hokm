using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace Hokm.GameServer
{
    // MUST BE THREAD-SAFE
    public class MatchReportHubService : IHostedService, IMatchReportHub
    {
        // concurrent bag does not work hence using dictionary and object here is dummy
        private ConcurrentDictionary<IMachReportSink, object> _sinks = new ConcurrentDictionary<IMachReportSink, object>();
        private ConcurrentDictionary<Match, object> _activeMatches = new ConcurrentDictionary<Match, object>();

        public bool IsRunning { get; private set; } = false;
        public MatchReportHubService(IEnumerable<IMachReportSink> sinks)
        {
            foreach (var sink in sinks)
            {
                _sinks.TryAdd(sink, null);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            IsRunning = true;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            IsRunning = false;
            _activeMatches.ToList().ForEach(x => StopReportingMatch(x.Key));
            
            return Task.CompletedTask;
        }

        public void ReportMatch(Match match)
        {
            match.MatchEvent += MatchOnMatchEvent;
        }

        // NOTE: Async VOID !!! this is said to be dangerous but OK only on event handlers - which is this case
        private async void MatchOnMatchEvent(object sender, MatchEventArgs e)
        {
            if (!IsRunning)
                return;
            
            foreach (var sink in _sinks.Keys)
            {
                try
                {
                    // in sequence rather than in parallel - for now
                    await sink.ReportAsync(e.Info);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        public void StopReportingMatch(Match match)
        {
            match.MatchEvent -= MatchOnMatchEvent;
            object v = null;
            _activeMatches.TryRemove(match, out v);
        }

        public void AddSubscriber(IMachReportSink sink)
        {
            _sinks.TryAdd(sink, null);
        }

        public void RemoveSubscriber(IMachReportSink sink)
        {
            object v = null;
            _sinks.TryRemove(sink, out v);
        }
    }
}