using System;

using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace DuckDNS
{
    class Program
    {
        public static int configVersion = 1;
        public static Settings set = new Settings();
        static void Main(string[] args)
        {
            
            if (File.Exists("config.json") == false)
            {
                CreateConfig();
            } else
            {
                set = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("config.json"));
                if(set.configfileVersion < configVersion)
                {
                    Console.WriteLine("Updating Config File, Please Edit to see what changed.");
                    File.WriteAllText("config.json", JsonConvert.SerializeObject(set, Formatting.Indented));
                }
                else if(set.configfileVersion > configVersion)
                {
                    Console.WriteLine("Using a config file from a future version of this application is not supported. If issues happen, this is likely your issue.");
                }
                Console.WriteLine("Config File Loaded. Sites being Managed:");
                foreach(var p in set.sites)
                {
                    Console.WriteLine(p.Key);
                }
            }
            Console.WriteLine("Scheduling Automatic Updates every " + set.DoUpdateEveryXMinutes + " Minutes.");

            var tmr = new Timer(TimedUpdate(), null, 0, set.DoUpdateEveryXMinutes * 1000);
            
            Console.WriteLine("Console Ready. type \"help\" for help");
            do
            {
                Console.Write(">");
                var command = Console.ReadLine();
                command = command.ToLower();
                if(command == "help") { PrintHelp(); }
                if(command == "ip") { PrintIPAsync(); }
                if(command == "update") { ForceUpdate(); }
                if(command == "exit") { Environment.Exit(0); }
            } while (true);
        }

        private static TimerCallback TimedUpdate()
        {
            Console.WriteLine("Executing Automatic IP Update...");
            ForceUpdate();
            return delegate { };
        }

        static void PrintHelp()
        {
            Console.WriteLine("Help: Displays Help (This)");
            Console.WriteLine("IP: Displays your currently detected IP");
        }

        static async void PrintIPAsync()
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DuckDNS_Updater", "1.0"));
                var result = await httpClient.GetStringAsync("http://whatismyip.akamai.com/");
                Console.WriteLine(result);
            } catch
            {
                Console.WriteLine("Failed to get IP");
            } 
        }

        static async void ForceUpdate()
        {
            foreach (var p in set.sites)
            {
                try
                {
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DuckDNS_Updater_github_com_pfckrutonium_DuckDNSUpdater", "1.0"));
                    var result = await httpClient.GetStringAsync("https://duckdns.org/update?domains=" + p.Key + "&token=" + p.Value + "&ip=");
                    if(result == "KO")
                    {
                        Console.WriteLine("Failed to update IP of " + p.Key);
                    } else
                    {
                        Console.WriteLine("Updated " + p.Key);
                    }
                }
                catch
                {
                    Console.WriteLine("Failed to update IP of " + p.Key);
                }
            }
            Console.WriteLine("Update Complete.");
        }

        static void CreateConfig()
        {
            Settings set = new Settings();
            set.sites = new List<KeyValuePair<string, string>>();
            set.sites.Add(new KeyValuePair<string, string>("domain", "token"));
            set.sites.Add(new KeyValuePair<string, string>("domain2", "token2"));
            File.WriteAllText("config.json", JsonConvert.SerializeObject(set, Formatting.Indented));
            Console.WriteLine("Created Default Configuration file, \"config.json\", you should edit it to have your domains and tokens.");
            Console.ReadKey();
            Environment.Exit(1);
        }

        public class Settings
        {
            public List<KeyValuePair<string, string>> sites { get; set; } //Domain, Token
            public int DoUpdateEveryXMinutes = 5;
            public int configfileVersion = configVersion;
        }

    }
}