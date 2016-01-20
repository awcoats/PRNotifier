using System;
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

        public OpenPullRequestMonitor()
        {
            var pollingPeriod = TimeSpan.FromMinutes(1).TotalMilliseconds;
            _timer = new Timer(pollingPeriod) {AutoReset = true};
            _timer.Elapsed += (sender, eventArgs) =>
            {
                var prs = 0;
                Task.Run(async () =>
                {
                    prs = await PollGitHub();
                    Console.WriteLine("Polled PR count:" + prs);
                }).Wait();

                if (prs > OpenPrs)
                {
                    var soundPlayer = new SoundPlayer("fanfare3.wav");
                    soundPlayer.Play();
                    if (_pr != null)
                    {
                        var speech = new SpeechSynthesizer();
                        speech.Speak("New pull request from " +_pr.User.Name);
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

        public async Task<int> PollGitHub()
        {
            var gitHubClient = new GitHubClient(new ProductHeaderValue("repo"));
            gitHubClient.Credentials = new Credentials("****", "****");
            var openPullRequests = new PullRequestRequest {State = ItemState.Open};
            var prs = await gitHubClient.PullRequest.GetAllForRepository("owner", "reponame", openPullRequests);
            if (prs.Count > 0)
            {
                _pr = prs[0];
            }
            return prs.Count;
        }
    }
}