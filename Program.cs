using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommandLineToolkit
{
    class Program
    {
        static void Main(string[] args)
        {
            ShowMainMenu();
        }

        static void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                DisplayAsciiArt();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n🛠️ 主菜单");
                Console.ResetColor();

                var menu = new Dictionary<int, string>()
                {
                    {1, "智能批处理脚本生成器"},
                    {2, "本地文件搜索引擎"},
                    {3, "正则表达式助手"},
                    {4, "网络探测扫描器"},
                    {0, "退出程序"}
                };

                foreach (var item in menu)
                {
                    Console.WriteLine($"\t[{item.Key}] {item.Value}");
                }

                Console.Write("\n请输入选项数字：");
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1: BatchScriptGenerator.Start(); break;
                        case 2: FileSearchEngine.Start(); break;
                        case 3: RegexHelper.Start(); break;
                        case 4: NetworkScanner.Start(); break;
                        case 0: Environment.Exit(0); break;
                        default: ShowError("无效选项"); break;
                    }
                }
                else
                {
                    ShowError("输入格式错误");
                }
            }
        }

        static void DisplayAsciiArt()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"
  _______          _      _____ _          _ _       
 |__   __|        | |    / ____| |        | | |      
    | | ___   ___ | |   | |    | | ___   _| | |_ __  
    | |/ _ \ / _ \| |   | |    | |/ / | | | | | '_ \ 
    | | (_) | (_) | |   | |____|   <| |_| | | | |_) |
    |_|\___/ \___/|_|    \_____|_|\_\\__,_|_|_| .__/ 
                                              | |    
                                              |_|   
            ");
            Console.ResetColor();
        }

        static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"⚠️ {message}");
            Console.ResetColor();
            Console.ReadKey();
        }
    }

    class BatchScriptGenerator
    {
        public static void Start()
        {
            Console.Clear();
            Console.WriteLine("=== 批处理脚本生成器 ===");

            Console.Write("请输入脚本类型（bat/powershell）：");
            string type = Console.ReadLine()?.ToLower();

            Console.Write("需要执行哪些操作？（文件操作/网络请求/其他）：");
            string operation = Console.ReadLine();

            // 修改后的switch语句（兼容C# 7.3）
            string script;
            switch (type)
            {
                case "bat":
                    script = "@echo off\n" + $"echo 执行 {operation} 操作";
                    break;
                case "powershell":
                    script = $"Write-Host '执行 {operation} 操作'";
                    break;
                default:
                    script = "echo 未知脚本类型";
                    break;
            }

            Console.WriteLine("\n生成的脚本：");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(script);
            Console.ResetColor();

            Extensions.WaitToReturn();
        }
    }

    class FileSearchEngine
    {
        public static void Start()
        {
            Console.Clear();
            Console.WriteLine("=== 文件搜索引擎 ===");

            Console.Write("请输入搜索目录：");
            string path = Console.ReadLine();
            Console.Write("请输入搜索关键词：");
            string keyword = Console.ReadLine();

            try
            {
                var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                                .Where(f => LevenshteinDistance(Path.GetFileName(f), keyword) <= 2);

                Console.WriteLine("\n搜索结果：");
                foreach (var file in files)
                {
                    Console.WriteLine($"- {file}");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"搜索出错：{ex.Message}");
                Console.ResetColor();
            }

            Extensions.WaitToReturn();
        }

        static int LevenshteinDistance(string s, string t)
        {
            int[,] dp = new int[s.Length + 1, t.Length + 1];
            for (int i = 0; i <= s.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= t.Length; j++) dp[0, j] = j;

            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + cost);
                }
            }
            return dp[s.Length, t.Length];
        }
    }

    class RegexHelper
    {
        public static void Start()
        {
            Console.Clear();
            Console.WriteLine("=== 正则表达式助手 ===");

            var patterns = new Dictionary<string, string>()
            {
                {"邮箱地址", @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"},
                {"中国大陆手机号", @"^1[3-9]\d{9}$"},
                {"IP地址", @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$"}
            };

            Console.WriteLine("可用模式：" + string.Join(", ", patterns.Keys));
            Console.Write("\n请输入要匹配的模式：");
            string input = Console.ReadLine();

            if (patterns.TryGetValue(input, out string pattern))
            {
                Console.WriteLine($"\n生成的正则表达式：{pattern}");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(Regex.Unescape(pattern));
                Console.ResetColor();
                TestRegex(pattern);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("未知模式，请输入列表中的名称");
                Console.ResetColor();
            }

            Extensions.WaitToReturn();
        }

        static void TestRegex(string pattern)
        {
            try
            {
                Console.Write("\n输入测试字符串：");
                string input = Console.ReadLine();
                bool isMatch = Regex.IsMatch(input, pattern);
                Console.ForegroundColor = isMatch ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(isMatch ? "✅ 匹配成功" : "❌ 匹配失败");
                Console.ResetColor();
            }
            catch (ArgumentException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"正则表达式错误：{ex.Message}");
                Console.ResetColor();
            }
        }
    }

    class NetworkScanner
    {
        public static void Start()
        {
            Console.Clear();
            Console.WriteLine("=== 网络探测扫描器 ===");

            var activeDevices = new List<string>();
            Console.WriteLine("正在扫描局域网...");

            Parallel.For(1, 255, (i) =>
            {
                string ip = $"192.168.1.{i}";
                using (Ping ping = new Ping())
                {
                    try
                    {
                        PingReply reply = ping.Send(ip, 100);
                        if (reply.Status == IPStatus.Success)
                        {
                            lock (activeDevices)
                            {
                                activeDevices.Add(ip);
                            }
                        }
                    }
                    catch { /* 忽略Ping错误 */ }
                }
            });

            Console.WriteLine("\n活跃设备列表：");
            foreach (var ip in activeDevices.OrderBy(x => x))
            {
                Console.WriteLine($"\nIP: {ip}");
                Console.WriteLine("端口扫描结果：");
                ScanPorts(ip);
            }

            Extensions.WaitToReturn();
        }

        static void ScanPorts(string ip)
        {
            int[] commonPorts = { 21, 22, 80, 443, 3389, 8080 };
            Parallel.ForEach(commonPorts, (port) =>
            {
                using (TcpClient client = new TcpClient())
                {
                    try
                    {
                        client.Connect(ip, port);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"  PORT {port} 开放");
                        Console.ResetColor();
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($"  PORT {port} 关闭");
                        Console.ResetColor();
                    }
                }
            });
        }
    }

    static class Extensions
    {
        public static void WaitToReturn()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n按任意键返回主菜单...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}