using System;
using System.Threading.Tasks;

namespace Hokm.GameServer
{
    public class ConsoleSink : IMachReportSink
    {
        public Task ReportAsync(MatchInfo info)
        {
            Console.WriteLine(info);
            return Task.CompletedTask;
        }
    }
}