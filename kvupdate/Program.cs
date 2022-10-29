using System;
using System.Net.NetworkInformation;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.IO.Compression;
using System.Diagnostics;

namespace kvupdate
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Checking for updates...");

            WebClient client = new WebClient();
            string downloadString = client.DownloadString("https://raw.githubusercontent.com/pogrammerX/Kuvamri/main/updater/currentversion.txt");

            bool requiresUpdate = false;

            if(!Directory.Exists("C:\\Program Files\\Kuvamri")) requiresUpdate = true;
            if (!File.Exists("C:\\Program Files\\Kuvamri\\.version")) requiresUpdate = true;
            if (!requiresUpdate) 
                if (File.ReadAllText("C:\\Program Files\\Kuvamri\\.version") != downloadString)
                    requiresUpdate = true;

            Console.WriteLine("Update found!");
            Console.WriteLine("Updating...");

            bool updateFinished = false;

            string updateFile = Path.GetTempFileName();

            client.DownloadProgressChanged += (object send, DownloadProgressChangedEventArgs e) =>
            {
                Console.Clear();
                Console.WriteLine("Downloading Update File...");
                string equalsSymbols = "";
                string spaces = "";

                for (int i = 0; i < e.ProgressPercentage; i++)
                {
                    equalsSymbols += "=";
                }

                for (int i = 0; i < 100 - e.ProgressPercentage; i++)
                {
                    spaces += " ";
                }

                Console.WriteLine("[" + equalsSymbols + "" + spaces + "] (" + e.ProgressPercentage + "%)");
            };

            client.DownloadFileCompleted += (object s, AsyncCompletedEventArgs e) =>
            {
                Console.Clear();
                Console.WriteLine("Extracting...");
                ZipFile.ExtractToDirectory(updateFile, "C:\\Program Files\\Kuvamri\\", true);
                Process.Start("cmd.exe", "/C set PATH=%PATH%;\"C:\\Program Files\\Kuvamri\\\"");
                File.Delete(updateFile);
                Console.WriteLine("Update Completed.");
                Console.WriteLine("Please restart your computer.");
                Console.ReadKey();
                updateFinished = true;
            };

            client.DownloadFileAsync(new Uri("https://raw.githubusercontent.com/pogrammerX/Kuvamri/main/updater/latestversion.zip"), updateFile);

            while(!updateFinished) { }
        }
    }
}
