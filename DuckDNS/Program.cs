using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace DuckDNS
{
    class Program
    {
        public static int configVersion = 1;
        public static Settings set = new Settings(); //Used all over the place, so it made sense to only have 1.
        static void Main(string[] args)
        {
            if (File.Exists("duckdns_config.json") == false)
            {
                CreateConfig();
            }
            else
            {
                set = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("duckdns_config.json"));
                if (set.configfileVersion < configVersion)
                {
                    Console.WriteLine("Updating Config File, Please Edit to see what changed.");
                    File.WriteAllText("duckdns_config.json", JsonConvert.SerializeObject(set, Formatting.Indented));
                }
                else if (set.configfileVersion > configVersion)
                {
                    Console.WriteLine("Using a config file from a future version of this application is not supported. If issues happen, this is likely your issue.");
                }
                Console.WriteLine("Config File Loaded. Sites being Managed:");
                foreach (var p in set.sites)
                {
                    Console.WriteLine(p.Domain);
                }
            }
            Console.WriteLine("Scheduling Automatic Updates every " + set.DoUpdateEveryXMinutes + " Minutes.");

            TimedUpdate(set.DoUpdateEveryXMinutes * 1000 * 60); //Update the DNS names every 5 minutes. Minutes*1000=Minutes in Milliseconds. Runs Immediatly.

            if (set.IsUseConsoleInput)
            {
                Console.WriteLine("Console Ready. type \"help\" for help");
                do
                {
                    var command = Console.ReadLine();
                    command = command.ToLower();
                    if (command == "help") { PrintHelp(); }
                    if (command == "ip") { PrintIPAsync(); }
                    if (command == "update") { ForceUpdate(); }
                    if (command == "exit") { Environment.Exit(0); }
                } while (true);
            }
            else
            {
                Console.WriteLine("Console is disabled. (Just update by Schedule)");
                while (true)
                {
                    Thread.Sleep(Timeout.Infinite);
                }
            }
        }

        private static async void TimedUpdate(int millisecondsDelay)
        {
            while (true)
            {
                Console.WriteLine("Executing Automatic IP Update...");
                ForceUpdate();
                await Task.Delay(millisecondsDelay);
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("Help: Displays Help (This)");
            Console.WriteLine("IP: Displays your currently detected IP");
            Console.WriteLine("Update: Forces an immediate update to the DDNS Entries");
            Console.WriteLine("Exit: Closes the updater.");
        }

        static async void PrintIPAsync()
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DuckDNS_Updater", "1.0"));
                var result = await httpClient.GetStringAsync("http://whatismyip.akamai.com/");
                Console.WriteLine(result);
            }
            catch
            {
                Console.WriteLine("Failed to get IP");
            }
        }

        static async void ForceUpdate()
        {
            Console.WriteLine("Reloading Config...");
            set = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("duckdns_config.json"));
            Console.WriteLine("Reload Complete!");
            foreach (var p in set.sites)
            {
                try
                {
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DuckDNS_Updater_github_com_pfckrutonium_DuckDNS_Updater", "1.0")); //*Grin*
                    string query = "https://duckdns.org/update?domains=" + p.Domain + "&token=" + p.Token;
                    if (p.force_ip_number == "" == false) //Allows overriding the ip's.
                    {
                        query += "&ip=" + p.force_ip_number;
                    }
                    if (p.force_ipv6_number == "" == false)
                    {
                        query += "&ipv6=" + p.force_ipv6_number;
                    }

                    var result = await httpClient.GetStringAsync(query);
                    if (result == "KO")
                    {
                        Console.WriteLine("Failed to update IP of " + p.Domain);
                    }
                    else if (result == "OK")
                    {
                        Console.WriteLine("Updated " + p.Domain); //It didn't error, and it didn't return KO, so we are going to assume it worked. Probably not the smartest way.
                    }
                    else
                    {
                        Console.WriteLine("Somthing Happened when updating " + p.Domain + ". Please contact PFCKrutonium on GitHub.");
                    }
                }
                catch
                {
                    Console.WriteLine("Failed to update IP of " + p.Domain);
                }
            }
            Console.WriteLine("Update Complete.");
        }

        static void CreateConfig()
        {
            Settings set = new Settings();
            set.sites = new List<ValueSet>();
            //set.sites.Add(new KeyValuePair<string, string>("domain", "token"));
            //set.sites.Add(new KeyValuePair<string, string>("domain2", "token2")); //Examples. There can be an unlimited number of domains and tokens here.
            var temp = new ValueSet();
            temp.Domain = "domain,domain2";
            temp.Token = "token";
            temp.force_ip_number = "";
            temp.force_ipv6_number = "";
            set.sites.Add(temp);
            temp.Domain = "domain3";
            temp.Token = "token2";
            set.sites.Add(temp);
            File.WriteAllText("duckdns_config.json", JsonConvert.SerializeObject(set, Formatting.Indented));
            Console.WriteLine("Created Default Configuration file, \"duckdns_config.json\", you should edit it to have your domains and tokens.");
            Console.ReadKey();
            Environment.Exit(1);
        }

        public class Settings
        {
            public string Message = "If you leave the force ips' blank, it will use autodetected values. This is the recommended mode of operation. Do not override the values unless you need to. This message is not evaluated.";
            public string Message2 = "This config file is reloaded every time the program updates the addresses, so live editing is possible. This can be used to maintain the IP addresses of remote computers.";
            public string Message3 = "At the time of writing, DuckDNS is unable to automatically determine IPv6 addresses, and so unless you override the value, your domain will be ipv4 only.";
            public int DoUpdateEveryXMinutes = 5; //Defauts to every 5 minutes.
            public bool IsUseConsoleInput = true; // Use runtime console input.
            public int configfileVersion = configVersion; //Useful for future updates.
            public List<ValueSet> sites; //Domain, Token
        }
        public struct ValueSet
        {
            public string Domain;
            public string Token;
            public string force_ip_number;
            public string force_ipv6_number;
        }

    }
}
