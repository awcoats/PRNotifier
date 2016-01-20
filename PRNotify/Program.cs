using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using Topshelf;

namespace PRNotify
{  
    public class Program
    {
     
        public static  void Main(string[] args)
        {

            //var monitor = new OpenPullRequestMonitor();
            //monitor.Start();

            //Console.WriteLine("Press enter to finish");
            //Console.ReadLine();
            HostFactory.Run(x => //1
            {
                x.Service<OpenPullRequestMonitor>(s => //2
                {
                    x.StartAutomatically(); // Start the service automatically
                    s.ConstructUsing(name => new OpenPullRequestMonitor()); //3
                    s.WhenStarted(tc => tc.Start()); //4
                    s.WhenStopped(tc => tc.Stop()); //5
                });
                x.RunAsLocalSystem(); //6

                x.SetDescription("CalPEATS GitHub PR Monitor"); //7
                x.SetDisplayName("CalPEATS PR Monitor"); //8
                x.SetServiceName("CalPEATSPRMon");
            });
        }
    }
}
