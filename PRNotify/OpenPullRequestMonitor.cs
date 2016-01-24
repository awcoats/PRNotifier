using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Timers;
using Octokit;

namespace PRNotify
{
    public class OpenPullRequestMonitor
    {
        private readonly Timer _timer;
        private PullRequest _pr;

        public OpenPullRequestMonitor(TimeSpan timespan, ItemState state=ItemState.Open)
        {
            var pollingPeriod = timespan.TotalMilliseconds;
            _timer = new Timer(pollingPeriod) {AutoReset = true};
            _timer.Elapsed += (sender, eventArgs) =>
            {
                var prs = 0;
                Task.Run(async () =>
                {
                    var requests = await PollGitHub(state);
                    prs = requests.Count;
                    _pr = requests.FirstOrDefault();
                    Console.WriteLine("Polled PR count:" + prs);
                }).Wait();

                if (prs > OpenPrs)
                {
                    var soundPlayer = new SoundPlayer("fanfare3.wav");
                    soundPlayer.PlaySync();
                    if (_pr != null)
                    {
                        var speech = new SpeechSynthesizer();
                        speech.Speak("New pull request.");
                        speech.Speak(_pr.Title);
                    }

                    OpenPrs = prs;
                }
                else if (prs < OpenPrs)
                {
                    OpenPrs = prs;
                }
            };
        }

        public int OpenPrs { get; private set; }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public async Task<IReadOnlyList<PullRequest>> PollGitHub(ItemState state = ItemState.Open)
        {
            var gitHubClient = new GitHubClient(new ProductHeaderValue("CalPEATS"));
            gitHubClient.Credentials = new Credentials("***email**", "******");
            var openPullRequests = new PullRequestRequest {State = state };
            var prs = await gitHubClient.PullRequest.GetAllForRepository("calicosol", "CalPEATS", openPullRequests);
            return prs;
        }
    }
}