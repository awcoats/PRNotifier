using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Octokit;

namespace PRNotify.Tests
{
    [TestFixture]
    public class OpenPullRequestMonitorTests
    {
        [Test]
        public async Task TestPoll()
        {
            var monitor = new OpenPullRequestMonitor(TimeSpan.FromSeconds(1));
            var requests = await monitor.PollGitHub(ItemState.All);
        }
    }
}
