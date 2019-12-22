using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using iPatch.Authentication;
using iPatch.iPatch_Functions;
using System.Collections.Generic;
using System.Diagnostics;

namespace iPatch
{
    public static class iPatch_Properties
    {
        public static bool isSilent = false;
        public static bool withAuthentication = true;
        public static List<string> Tasks = new List<string>();
    }
    class Program
    {
        static IConfigurationRoot LoadAppSettings()
        {
            var appConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            // Check for required settings
            if (string.IsNullOrEmpty(appConfig["appId"]) ||
                // Make sure there's at least one value in the scopes array
                string.IsNullOrEmpty(appConfig["scopes:0"]))
            {
                return null;
            }

            return appConfig;
        }
        private void CheckForPython()
        {

        }
        private void RunPyScript(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "my/full/path/to/python.exe";
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }
        static void CommandLineParse(string[] args)
        { 
            /**
            <summary>
            Parsing command line arguments.
            </summary> 
            <example>
            ipatch -s --NoAuthentication detectHW 
            or
            ipatch detectHW disable-dGPU
            </example>
             */
            foreach(string option in args)
            {
                if (option.ToLower() == "-s" || option.ToLower() == "--silent")/**Disable user friendly console output. 
                Used for inApp calling (eg. My Hackintosh, Hackintstaller)*/
                {
                    iPatch_Properties.isSilent = true;
                }
                if(option.ToLower() == "--noauthentication")///disable authentication with Microsoft Grpah. Useful if the user is paranoic
                {
                    iPatch_Properties.withAuthentication = false;
                }
                else ///for everything else, treat it as a task for iPatch
                {
                    iPatch_Properties.Tasks.Add(option.ToLower());
                }

            }
        }
        static void Main(string[] args)
        {
            CommandLineParse(args);//parse arguments
            #region Authentication_OAts2
            if (iPatch_Properties.withAuthentication)///if the user didn't disable the authetication, begin
            {
                if (!iPatch_Properties.isSilent)
                {
                    Console.WriteLine("iPatch needs to authenticate using a Microsoft Account. \n" +
                          "iPatch promises not to use your account for any other purposes other than downloading the required kexts\n" +
                          "from a public Onedrive folder. If you agree, type y(Yes) or n(No)");///tell the user the point of the File.Read.All permision
                    if (Console.Read() != 'y')///if he/she didn't acccept, quit
                    {
                        return;
                    } 
                }
                else
                {
                    Console.WriteLine("ipatch-auth-request\n" +
                        "console\n" +
                        "waiting-for-approval");///print requests for apps(for custom screens if they have some)
                    if (Console.Read() != 'y')///if he/she didn't acccept, quit
                    {
                        return;
                    }
                }
                var appConfig = LoadAppSettings();//self explanatory

                if (appConfig == null)
                {
                    Console.WriteLine("Missing or invalid appsettings.json...exiting");
                    return;
                }

                var appId = appConfig["appId"];
                var scopes = appConfig.GetSection("scopes").Get<string[]>();///get AppID and scopes for MSGraph

                // Initialize the auth provider with values from appsettings.json
                var authProvider = new DeviceCodeAuthenticationProvider(appId, scopes);

                // Request a token to sign in the user
                var accessToken = authProvider.GetAccessToken().Result;
                if (accessToken != null)
                {
                    Console.WriteLine($"Connected to MSGraph using {accessToken}");
                }
            }
            else //In case of the authentication being disabled
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WARNING! --NoAuthentication was passed. iPatch WON'T be able to automatically download the required kexts\n" +
                    "You MUST download the kexts manually");///warn the user tat he will need to download the files manually
                Console.ResetColor();
            }
            #endregion
            foreach(string task in iPatch_Properties.Tasks)
            {
                if(task=="detectHW")
                {

                }
            }
        }
    }
}
