using System;
using System.IO;
using System.Text;
using Nancy;
using Nancy.Hosting.Self;
using ThunderSdk;
using static Voltali.Program;

namespace Voltali
{
    static class Program
    {
        public static DownloadManager manager = new DownloadManager(64, Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"));
        static void Main()
        {
            Console.WriteLine("Voltali Download Manager");
            Console.WriteLine("-------------------");

            manager.TaskDownload += (s, e) =>
            {
                if (!(s is DownFileInfo info)) return;

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"{info.TaskInfo.Percent * 100:0.0}% TotalSize: {info.TaskInfo.TotalSize:0.0} " +
                              $"/ {info.TaskInfo.TotalDownload:0.0} | Speed: {info.TaskInfo.Speed:0.0}" +
                              $"(P2P: {info.TaskInfo.SpeedP2P:0.0} P2S: {info.TaskInfo.SpeedP2S:0.0}) | OnlySource: {info.IsOnlyOriginal}");

            };

            manager.TaskCompleted += (s, e) =>
            {
                if (!(s is DownFileInfo info)) return;
                Console.WriteLine();
                Console.WriteLine(info.FileName + " Download Completed | " + info.WasteTime.Seconds + "s");
            };

            manager.TaskError += (s, e) =>
            {
                if (!(s is DownFileInfo info)) return;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(info.FileName + " Download Error");
            };

            manager.TaskPause += (s, e) =>
            {
                if (!(s is DownFileInfo info)) return;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(info.FileName + " Download Pause | " + info.WasteTime.Seconds + "s");
            };

            using (NancyHost host = new NancyHost(new HostConfiguration
                    {RewriteLocalhost = true, UrlReservations = new UrlReservations {CreateAutomatically = true}},
                new Uri("http://localhost:2020/")))
            {
                host.Start();

                Console.WriteLine("Voltali Download Manager is running on 2020 port");
                Console.WriteLine("Press any [Enter] to close the host.");
                Console.ReadLine();
            }
        }
    }

    public sealed class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", x => "Voltali Download Manager");

            Get("/download/{url}", x =>
            {
                var mUrl = new Url(Encoding.Default.GetString(Convert.FromBase64String(x.url)));
                manager.CreateNewTask(mUrl.ToString(), mUrl.Path, isOnlyOriginal: false);
                return manager.StartAllTask().ToString();
            });

            Get("/stop", x => manager.PauseAllTask().ToString());
            Get("/stop/{item}", x => manager.AllDownLoad[x.item].StopTask().ToString());

            Get("/start", x => manager.StartAllTask().ToString());
            Get("/start/{item}", x => manager.AllDownLoad[x.item].StartTask().ToString());

            Get("/delete/{item}",
                x => (manager.AllDownLoad[x.item].StopTask() &&
                      manager.AllDownLoad[x.item].DeleteTask()).ToString());

            Get("/get", x =>
            {
                if (manager.AllDownLoad.Count == 0)
                    return "# nothing at all";

                string str = "";
                foreach (var item in manager.AllDownLoad)
                    str +=
                        $"# {(item.Id - 1).ToString()} | {item.FileName} | {item.TaskInfo.Percent * 100:0.0} | {item.TaskInfo.State}"
                        + Environment.NewLine;
                return str;
            });
        }
    }
}
