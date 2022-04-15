using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Net;
using System.Threading;

namespace Jobe_Optimizer
{

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        public static void ReadWriteFile(string path, string msg, char splitChar, string newValue, string type) //bool trailingZeros = true)// bool useLast = true)
        {
            var originalLines = File.ReadAllLines(path);

            var updatedLines = new List<string>();
            foreach (var line in originalLines)
            {
                string[] infos = line.Split(splitChar);
                if (infos[0] == msg)
                {
                    // update value
                    type = type.ToLower(); // me reading the code later: why did i do this? I could have just always made string?? Eh good practice for me, ill fix it one day.
                    if (type == "int") // System.FormatException if you put soemtyhing thats not a int.
                    {
                        infos[1] = int.Parse(newValue).ToString();
                    }
                    else if (type == "string" || type == "str")
                    {
                        infos[1] = newValue;
                    }
                    else if (type == "float")
                    {
                        infos[1] = float.Parse(newValue).ToString();
                        /*if (trailingZeros)
                        {
                            float f;
                            float.TryParse(newValue, NumberStyles.Any,
                            CultureInfo.InvariantCulture, out f);
                            Console.WriteLine(f.ToString(CultureInfo.InvariantCulture));
                        }
                        else infos[1] = float.Parse(newValue).ToString(); */
                    }
                    else if (type == "bool" || type == "boolean")
                    {
                        infos[1] = bool.Parse(newValue).ToString();
                    }

                }

                updatedLines.Add(string.Join("=", infos));
            }

            File.WriteAllLines(path, updatedLines);
        }
        public static string localAD = Environment.GetEnvironmentVariable("LocalAppData");
        public static string userSettings = localAD + @"FortniteGame\Saved\Config\WindowsClient\GameUserSettings.ini";
        public static void ChangeUserSettings()
        {
            ReadWriteFile(userSettings, "bMotionBlur", '=', "False", "bool");
            ReadWriteFile(userSettings, "bShowGrass", '=', "False", "bool");
           
            ReadWriteFile(userSettings, "sg.AntiAliasingQuality", '=', "0", "int");
            ReadWriteFile(userSettings, "sg.ShadowQuality", '=', "0", "int");
            ReadWriteFile(userSettings, "sg.PostProcessQuality", '=', "0", "int");
            
            ReadWriteFile(userSettings, "sg.EffectsQuality", '=', "0", "int");
            ReadWriteFile(userSettings, "sg.FoliageQuality", '=', "0", "int");
            ReadWriteFile(userSettings, "sg.ShadingQuality", '=', "0", "int");
            ReadWriteFile(userSettings, "sg.ResolutionQuality", '=', "100", "int");
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            string time11 = DateTime.Now.ToString("t");
            LogTextBox.AppendText(time11 + ":" + " " + "Ready To Go \n");

        }
        readonly static string CompatTelRunnerFile = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), @"Windows\System32\CompatTelRunner.exe");

        readonly static string CompatTelRunnerFileOff = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), @"Windows\System32\CompatTelRunner.exe.OFF");

        readonly static string CompatTelRunnerFileName = "CompatTelRunner.exe";
        readonly static string CompatTelRunnerFileNameOff = "CompatTelRunner.exe.OFF";
        internal static void DisableTelemetryRunner()
        {
            try
            {
                if (File.Exists(CompatTelRunnerFileOff)) File.Delete(CompatTelRunnerFileOff);

                if (File.Exists(CompatTelRunnerFile))
                {
                    Utilities.RunCommand(string.Format("takeown /F {0}", CompatTelRunnerFile));
                    Utilities.RunCommand(string.Format("icacls \"{0}\" /grant administrators:F", CompatTelRunnerFile));

                    FileSystem.RenameFile(CompatTelRunnerFile, CompatTelRunnerFileNameOff);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hmm I seemed to have skunked it ");
            }
        }
        internal static void DisableStickyKeys()
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Accessibility\StickyKeys", "Flags", "506", RegistryValueKind.String);
            Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Accessibility\Keyboard Response", "Flags", "122", RegistryValueKind.String);
            Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Accessibility\ToggleKeys", "Flags", "58", RegistryValueKind.String);
            Registry.SetValue(@"HKEY_USERS\.DEFAULT\Control Panel\Accessibility\StickyKeys", "Flags", "506", RegistryValueKind.String);
            Registry.SetValue(@"HKEY_USERS\.DEFAULT\Control Panel\Accessibility\Keyboard Response", "Flags", "122", RegistryValueKind.String);
            Registry.SetValue(@"HKEY_USERS\.DEFAULT\Control Panel\Accessibility\ToggleKeys", "Flags", "58", RegistryValueKind.String);
        }
        internal static void DisableSuperfetch()
        {
            Utilities.StopService("SysMain");
            //Utilities.StopService("Schedule");

            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SysMain", "Start", "4", RegistryValueKind.DWord);
            //Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\Schedule", "Start", "4", RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters", "EnableSuperfetch", "0", RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters", "EnablePrefetcher", "0", RegistryValueKind.DWord);
        }
        internal static void DisableNetworkThrottling()
        {
            Int32 tempInt = Convert.ToInt32("ffffffff", 16);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile", "NetworkThrottlingIndex", tempInt, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Psched", "NonBestEffortLimit", 0, RegistryValueKind.DWord);
        }

        internal static void Optimize()
        {
            // enable auto-complete in Run Dialog
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\AutoComplete", "Append Completion", "yes", RegistryValueKind.String);

            // show all tray icons
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer", "EnableAutoTray", "0", RegistryValueKind.DWord);

            // disable Remote Assistance
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Remote Assistance", "fAllowToGetHelp", "0", RegistryValueKind.DWord);

            // disable shaking to minimize
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "DisallowShaking", "1", RegistryValueKind.DWord);

            Registry.SetValue("HKEY_CLASSES_ROOT\\AllFilesystemObjects\\shellex\\ContextMenuHandlers\\Copy To", "", "{C2FBB630-2971-11D1-A18C-00C04FD75D13}");
            Registry.SetValue("HKEY_CLASSES_ROOT\\AllFilesystemObjects\\shellex\\ContextMenuHandlers\\Move To", "", "{C2FBB631-2971-11D1-A18C-00C04FD75D13}");

            Registry.SetValue("HKEY_CURRENT_USER\\Control Panel\\Desktop", "AutoEndTasks", "1");
            Registry.SetValue("HKEY_CURRENT_USER\\Control Panel\\Desktop", "HungAppTimeout", "1000");
            Registry.SetValue("HKEY_CURRENT_USER\\Control Panel\\Desktop", "MenuShowDelay", "8");
            Registry.SetValue("HKEY_CURRENT_USER\\Control Panel\\Desktop", "WaitToKillAppTimeout", "2000");
            Registry.SetValue("HKEY_CURRENT_USER\\Control Panel\\Desktop", "LowLevelHooksTimeout", "1000");
            Registry.SetValue("HKEY_CURRENT_USER\\Control Panel\\Mouse", "MouseHoverTime", "8");
            Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoLowDiskSpaceChecks", "00000001", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "LinkResolveIgnoreLinkInfo", "00000001", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoResolveSearch", "00000001", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoResolveTrack", "00000001", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoInternetOpenWith", "00000001", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control", "WaitToKillServiceTimeout", "2000");

            Utilities.StopService("DiagTrack");
            Utilities.StopService("diagnosticshub.standardcollector.service");
            Utilities.StopService("dmwappushservice");

            Utilities.RunCommand("sc config \"RemoteRegistry\" start= disabled");

            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\DiagTrack", "Start", "4", RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\diagnosticshub.standardcollector.service", "Start", "4", RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\dmwappushservice", "Start", "4", RegistryValueKind.DWord);

            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", "0", RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Hidden", "1", RegistryValueKind.DWord);
            //Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSuperHidden", "1", RegistryValueKind.DWord);

            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "SystemResponsiveness", 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "GPU Priority", 8, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "Priority", 6, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "Scheduling Category", "High", RegistryValueKind.String);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "SFIO Priority", "High", RegistryValueKind.String);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management", "FeatureSettingsOverride", "00000003", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management", "FeatureSettingsOverrideMask", "00000003", RegistryValueKind.DWord);
           
            // disable bluetooth services
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\BTAGService", "Start", "00000004", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\bthserv", "Start", "00000004", RegistryValueKind.DWord);
           
            // disable unnecessary services
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\WbioSrvc", "Start", "00000004", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\FontCache", "Start", "00000004", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\FontCache3.0.0.0", "Start", "00000004", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\hidserv", "Start", "00000004", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\GraphicsPerfSvc", "Start", "00000004", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\stisvc", "Start", "00000004", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\WerSvc", "Start", "00000004", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\PcaSvc", "Start", "00000004", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\Wecsvc", "Start", "00000004", RegistryValueKind.DWord);
           
            // disable telemetry shit
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\DiagTrack", "Start", "00000004", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\dmwappushservice", "Start", "00000004", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\diagsvc", "Start", "00000004", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\diagnosticshub.standardcollector.service", "Start", "00000004", RegistryValueKind.DWord);
         
            // who tf uses m$ maps, like everyone should disable this shit...
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\MapsBroker", "Start", "00000004", RegistryValueKind.DWord);
        
            // disable xbox services
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\XblGameSave", "Start", "00000004", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\XboxNetApiSvc", "Start", "00000004", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\XboxGipSvc", "Start", "00000004", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\XblAuthManager", "Start", "00000004", RegistryValueKind.DWord);
         
            // disable game dvr
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\PolicyManager\\default\\ApplicationManagement\\AllowGameDVR", "value", "00000000", RegistryValueKind.DWord);
            Registry.SetValue("HKEY_CURRENT_USER\\System\\GameConfigStore", "GameDVR_Enabled", "00000000", RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Low Latency", "GPU Priority", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Low Latency", "Priority", 8, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Low Latency", "Scheduling Category", "Medium", RegistryValueKind.String);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Low Latency", "SFIO Priority", "High", RegistryValueKind.String);
        }


        internal static void DisablePerformanceTweaks()
        {

                // disable auto-complete in Run Dialog
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\AutoComplete", true).DeleteValue("Append Completion", false);
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\AutoComplete", true).DeleteValue("AutoSuggest", false);

                // hide tray icons
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer", true).DeleteValue("EnableAutoTray", false);

                // enable Remote Assistance
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Remote Assistance", "fAllowToGetHelp", "1", RegistryValueKind.DWord);

                // enable shaking to minimize
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "DisallowShaking", "0", RegistryValueKind.DWord);

                Registry.ClassesRoot.DeleteSubKeyTree(@"AllFilesystemObjects\\shellex\\ContextMenuHandlers\\Copy To", false);
                Registry.ClassesRoot.DeleteSubKeyTree(@"AllFilesystemObjects\\shellex\\ContextMenuHandlers\\Move To", false);

                Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true).DeleteValue("AutoEndTasks", false);
                Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true).DeleteValue("HungAppTimeout", false);
                Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true).DeleteValue("WaitToKillAppTimeout", false);
                Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true).DeleteValue("LowLevelHooksTimeout", false);

                Registry.SetValue("HKEY_CURRENT_USER\\Control Panel\\Desktop", "MenuShowDelay", "400");
                Registry.SetValue("HKEY_CURRENT_USER\\Control Panel\\Mouse", "MouseHoverTime", "400");

                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", true).DeleteValue("NoLowDiskSpaceChecks", false);
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", true).DeleteValue("LinkResolveIgnoreLinkInfo", false);
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", true).DeleteValue("NoResolveSearch", false);
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", true).DeleteValue("NoResolveTrack", false);
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", true).DeleteValue("NoInternetOpenWith", false);

                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control", "WaitToKillServiceTimeout", "5000");

                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\DiagTrack", "Start", "2", RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\diagnosticshub.standardcollector.service", "Start", "2", RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\dmwappushservice", "Start", "2", RegistryValueKind.DWord);


                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", "1", RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Hidden", "0", RegistryValueKind.DWord);
                //Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSuperHidden", "0", RegistryValueKind.DWord);

                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "SystemResponsiveness", 14, RegistryValueKind.DWord);
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", true).DeleteValue("GPU Priority", false);
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", true).DeleteValue("Priority", false);
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", true).DeleteValue("Scheduling Category", false);
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", true).DeleteValue("SFIO Priority", false);

                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Low Latency", true).DeleteValue("GPU Priority", false);
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Low Latency", true).DeleteValue("Priority", false);
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Low Latency", true).DeleteValue("Scheduling Category", false);
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Low Latency", true).DeleteValue("SFIO Priority", false);
           

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (File.Exists("C:\\Program Files\\Jobe Optimize\\Optimized.optimized"))
            {
                string message1 = "It seems like ur pc has already been optimized \n are you sure you want to optimize again?";
                string caption1 = "Restart";
                MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                DialogResult result1;

                // Displays the MessageBox.
                
                result1 = MessageBox.Show(this, message1, caption1, buttons1,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                if (result1 == DialogResult.Yes)
                {
                    go();

                }
                if (result1 == DialogResult.No)
                {
                    Application.Exit();


                }
            }
            else
            {
                go();
            }

             void go()
            {
                Directory.CreateDirectory("C:\\Program Files\\Jobe Optimize");
                RestorePoint.Go();
                LogTextBox.Show();
                Optimize();
                WebClient yo = new WebClient();
                yo.DownloadFile("https://cdn.discordapp.com/attachments/872534627322036265/872535392631525417/Useful_Tweaks.bat", "C:\\Program Files\\Jobe Optimize\\1.bat");
                Process Process1 = new Process();
                Process1.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process1.StartInfo.FileName = ("C:\\Program Files\\Jobe Optimize\\1.bat");
                Process1.Start();
                Thread.Sleep(70);
                File.Delete("C:\\Program Files\\Jobe Optimize\\1.bat");
                string time = DateTime.Now.ToString("t");
                LogTextBox.AppendText(time + ":" + " " + "Optimize Completed \n");
                 DisableNetworkThrottling();
                string time1 = DateTime.Now.ToString("t");
                LogTextBox.AppendText(time1 + ":" + " " + "Disabled Network Throttling \n");
                 DisableSuperfetch();
                WebClient yo1 = new WebClient();
                yo1.DownloadFile("https://cdn.discordapp.com/attachments/872534627322036265/872535195394400336/Network.bat", "C:\\Program Files\\Jobe Optimize\\2.bat");
                Process Process11 = new Process();
                Process11.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process11.StartInfo.FileName = ("C:\\Program Files\\Jobe Optimize\\2.bat");
                Process11.Start();
                Thread.Sleep(30);
                File.Delete("C:\\Program Files\\Jobe Optimize\\2.bat");
                string time2 = DateTime.Now.ToString("t");
                LogTextBox.AppendText(time2 + ":" + " " + "Disabled Super Fetch \n");
                 DisableTelemetryRunner();
                string time3 = DateTime.Now.ToString("t");
                LogTextBox.AppendText(time3 + ":" + " " + "Disabled Telementery Runner \n");
                 DisableStickyKeys();
                string time4 = DateTime.Now.ToString("t");
                LogTextBox.AppendText(time4 + ":" + " " + "Disabled Sticky Keys \n");
                try
                {
                    ChangeUserSettings();
                }
                catch (System.IO.DirectoryNotFoundException)
                {

                }
                string time6 = DateTime.Now.ToString("t");
                LogTextBox.AppendText(DateTime.Now.ToString("t") + ":" + " " + "Fortnite Settings Optimized! \n");

                LogTextBox.AppendText(time6 + ":" + " " + "Full Optimization Completed \n");

                WebClient yo2 = new WebClient();
                yo2.DownloadFile("https://cdn.discordapp.com/attachments/872534627322036265/872535195394400336/Network.bat", "C:\\Program Files\\Jobe Optimize\\4.bat");
                Process Process2 = new Process();
                Process2.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process2.StartInfo.FileName = ("C:\\Program Files\\Jobe Optimize\\4.bat");
                Process2.Start();
                Thread.Sleep(30);
                File.Delete("C:\\Program Files\\Jobe Optimize\\4.bat");
                string time5 = DateTime.Now.ToString("t");
                LogTextBox.AppendText(time5 + ":" + " " + "Disabled RuntimeBroker \n");
               
                LogTextBox.AppendText(DateTime.Now.ToString("t") + ":" + " " + "PowerPlan enabled \n");

                File.Create("C:\\Program Files\\Jobe Optimize\\Optimized.optimized");
                string message = "Restart PC Now?";
                string caption = "Restart";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(this, message, caption, buttons,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Restarting Now");
                    Utilities.RunCommand("shutdown /r /t 0");

                }
                if (result == DialogResult.No)
                {
                    LogTextBox.Hide();


                }
            }

            



        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
        }



        private void button5_Click(object sender, EventArgs e)
        {
            Console.WriteLine("bcdedit /set useplatformclock no");
            Console.WriteLine("bcdedit /set disabledynamictick yes");
            Console.WriteLine("bcdedit /set useplatformtick yes");
            WebClient yo11 = new WebClient();
            yo11.DownloadFile("https://cdn.discordapp.com/attachments/872534627322036265/872535388185571419/Delete_FortniteGame_Folder.bat", "C:\\Program Files\\Jobe Optimize\\3.bat");
            Process Process111 = new Process();
            Process111.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process111.StartInfo.FileName = ("C:\\Program Files\\Jobe Optimize\\3.bat");
            Process111.Start();
            Console.WriteLine("bcdedit /set tscsyncpolicy enhanced");
            Console.WriteLine("bcdedit /timeout 0");
            Console.WriteLine("bcdedit /set nx alwaysoff");
            Thread.Sleep(30);
            File.Delete("C:\\Program Files\\Jobe Optimize\\3.bat");
            string message = "Restart PC Now?";
            string caption = "Restart";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(this, message, caption, buttons,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            if (result == DialogResult.Yes)
            {
                MessageBox.Show("Restarting Now");
                Utilities.RunCommand("shutdown /r /t 0");

            }
            if (result == DialogResult.No)
            {
                LogTextBox.Hide();


            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            WebClient yo22 = new WebClient();
            yo22.DownloadFile("https://cdn.discordapp.com/attachments/945408146845864006/945771845678956574/JobeTimer.exe", "C:\\Program Files\\Jobe Optimize\\JobeTimer.exe");

        }

        private void button6_Click(object sender, EventArgs e)
        {
            DisablePerformanceTweaks();
            string time = DateTime.Now.ToString("t");
            LogTextBox.AppendText(time + ":" + " " + "Restored Tweaks \n");
            string time1 = DateTime.Now.ToString("t");
            LogTextBox.AppendText(time1 + ":" + " " + "Enabled Network Throttling \n");
          
            string time2 = DateTime.Now.ToString("t");
            LogTextBox.AppendText(time2 + ":" + " " + "Enabled Super Fetch \n");

            string time3 = DateTime.Now.ToString("t");
            LogTextBox.AppendText(time3 + ":" + " " + "Enabled Telementery Runner \n");
            string time4 = DateTime.Now.ToString("t");
            LogTextBox.AppendText(time4 + ":" + " " + "Enabled Sticky Keys \n");
            string time5 = DateTime.Now.ToString("t");
            LogTextBox.AppendText(time5 + ":" + " " + "Enabled RuntimeBroker \n");

            LogTextBox.AppendText(time5 + ":" + " " + "Done Restoration \n");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            WebClient yo22 = new WebClient();
            yo22.DownloadFile("https://cdn.discordapp.com/attachments/738014059064066048/959534224468226058/nvidiaProfileInspector.exe", "C:\\Program Files\\Jobe Optimize\\nvidiaProfileInspector.exe");
            yo22.DownloadFile("https://cdn.discordapp.com/attachments/738014059064066048/959534224791191663/Optimized_Profile.nip", "C:\\Program Files\\Jobe Optimize\\Optimized_Profile.nip");
            Process.Start("C:\\Program Files\\Jobe Optimize");
            MessageBox.Show("Drag Optimized_Profile.nip onto nvidiaProfileInspector.exe");
           

        }
    }

}
