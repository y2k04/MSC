using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSC
{
    public partial class Setup : Form
    {
        private readonly string AppDir = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf(char.Parse(@"\")));
        private readonly string Config = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf(char.Parse(@"\"))) + @"\config.json";
        private int currentStage = 0;
        private string stage2Task = "java";
        private string mcBrand = "";
        private string mcVersion = "";
        private string JDKArchiveFolder = "";
        private List<VersionData> brandMetadata = new List<VersionData>();
        public Setup()
        {
            InitializeComponent();
        }

        private void ClearArea() => panel1.Controls.Clear();

        private void Stage1()
        {
            GroupBox chooseBrandBox = new GroupBox() { Text = "Choose Server Brand", Location = new Point(16, 86), Size = new Size(250, 48) };
            GroupBox chooseVersionBox = new GroupBox() { Text = "Choose Minecraft Version", Location = new Point(chooseBrandBox.Size.Width + 32, 86), Size = new Size(250, 48) };
            ComboBox chooseBrand = new ComboBox() { Location = new Point(8, 16), Size = new Size(chooseBrandBox.Size.Width - 16, 32), DropDownStyle = ComboBoxStyle.DropDownList };
            ComboBox chooseVersion = new ComboBox() { Location = new Point(8, 16), Size = new Size(chooseVersionBox.Size.Width - 16, 32), DropDownStyle = ComboBoxStyle.DropDownList, Enabled = false };
            chooseBrand.Items.AddRange(new string[] { "Vanilla", "Fabric", "Forge" });
            chooseBrand.SelectedIndexChanged += (object sender, EventArgs e) => { chooseVersion.SelectedIndex = -1; nextStage.Enabled = false; chooseVersion.Items.Clear(); chooseVersion.Items.AddRange(getBrandMetadata((string)chooseBrand.SelectedItem)); chooseVersion.Enabled = true; chooseVersion.SelectedIndex = 0; };
            chooseVersion.SelectedIndexChanged += (object sender, EventArgs e) => { nextStage.Enabled = true; mcVersion = (string)chooseVersion.SelectedItem; };
            chooseBrandBox.Controls.Add(chooseBrand);
            chooseVersionBox.Controls.Add(chooseVersion);
            panel1.Controls.Add(chooseBrandBox);
            panel1.Controls.Add(chooseVersionBox);
        }

        private async void Stage2()
        {
            ProgressBar progress = new ProgressBar() { Width = panel1.Width, Location = new Point(0, panel1.Height / 2), Style = ProgressBarStyle.Continuous };
            Label progressLabel = new Label() { AutoSize = true, TextAlign = ContentAlignment.MiddleCenter, Text = $"Downloading Java...{Environment.NewLine + Environment.NewLine + Environment.NewLine}0% complete", Location = new Point(progress.Width / 4 + progress.Width / 6, progress.Location.Y - 16) };
            WebClient web = new WebClient();
            web.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) => { progress.Value = e.ProgressPercentage; if (stage2Task == "java") { progressLabel.Text = $"Downloading Java...{Environment.NewLine + Environment.NewLine + Environment.NewLine}{e.ProgressPercentage}% complete"; } else if (stage2Task == "mc") { progressLabel.Text = $"Downloading {mcBrand} {mcVersion}...{Environment.NewLine + Environment.NewLine + Environment.NewLine + e.ProgressPercentage}% complete"; } };
            panel1.Controls.Add(progress);
            panel1.Controls.Add(progressLabel);
            var jdk = getJDK();
            if (!Directory.Exists($@"{AppDir}\runtime")) Directory.CreateDirectory($@"{AppDir}\runtime");
            if (!Directory.Exists($@"{AppDir}\temp")) Directory.CreateDirectory($@"{AppDir}\temp");
            if (!Directory.Exists($@"{AppDir}\runtime\bin") && !Directory.Exists($@"{AppDir}\runtime\lib"))
            {
                await web.DownloadFileTaskAsync(jdk[0], $@"{AppDir}\temp\{jdk[1]}");
                progressLabel.Text = $"Extracting Java package...";
                await Task.Run(() =>
                {
                    ZipFile.ExtractToDirectory($@"{AppDir}\temp\{jdk[1]}", $@"{AppDir}\temp\");
                    Directory.Move($@"{AppDir}\temp\{JDKArchiveFolder}\bin", $@"{AppDir}\runtime\bin\");
                    Directory.Move($@"{AppDir}\temp\{JDKArchiveFolder}\conf", $@"{AppDir}\runtime\conf\");
                    Directory.Move($@"{AppDir}\temp\{JDKArchiveFolder}\include", $@"{AppDir}\runtime\include\");
                    Directory.Move($@"{AppDir}\temp\{JDKArchiveFolder}\jmods", $@"{AppDir}\runtime\jmods\");
                    Directory.Move($@"{AppDir}\temp\{JDKArchiveFolder}\lib", $@"{AppDir}\runtime\lib\");
                    return;
                });
                if (!File.Exists($@"{AppDir}\runtime\bin\java.exe")) MessageBox.Show($"'java.exe' could not be found. This means this stage failed.{Environment.NewLine + Environment.NewLine}The temporary files have not been deleted yet, so you can still manually open '{jdk[1]}' (located in the 'temp' directory) and copy the 'bin' and 'lib' folders to the 'JDK' folder where this application is stored.{Environment.NewLine + Environment.NewLine}Press 'Ok' to continue with the setup.", "Minecraft Server Client", MessageBoxButtons.OK);
            }
            stage2Task = "mc";
            var mcJar = getMCJar();
            if (!File.Exists($@"{AppDir}\temp\{mcJar[1]}") || !File.Exists($@"{AppDir}\server.jar"))
            {
                if (mcBrand == "Vanilla" || mcBrand == "Fabric") await web.DownloadFileTaskAsync(mcJar[0], $@"{AppDir}\server.jar");
                else if (mcBrand == "Forge") await web.DownloadFileTaskAsync(mcJar[0], $@"{AppDir}\temp\{mcJar[1]}");
            }

            if (mcBrand == "Forge")
            {
                Process install = new Process();
                install.StartInfo = new ProcessStartInfo($@"{AppDir}\runtime\bin\java.exe", $"-jar \"{AppDir}\\server.jar\" --installServer") { CreateNoWindow = false, UseShellExecute = false, WorkingDirectory = AppDir };
                progress.Value = 50;
                progress.Style = ProgressBarStyle.Marquee;
                progressLabel.Text = $"Initialising Forge {mcVersion}...";
                install.Start();
                install.WaitForExit();
            }

            switch (mcBrand)
            {
                case "Vanilla":
                    File.AppendAllText(Config, $@"brand=Vanilla{Environment.NewLine}jargs=-XX:+UseG1GC -XX:+ParallelRefProcEnabled -XX:MaxGCPauseMillis=200 -XX:+UnlockExperimentalVMOptions -XX:+DisableExplicitGC -XX:+AlwaysPreTouch -XX:G1NewSizePercent=30 -XX:G1MaxNewSizePercent=40 -XX:G1HeapRegionSize=8M -XX:G1ReservePercent=20 -XX:G1HeapWastePercent=5 -XX:G1MixedGCCountTarget=4 -XX:InitiatingHeapOccupancyPercent=15 -XX:G1MixedGCLiveThresholdPercent=90 -XX:G1RSetUpdatingPauseTimePercent=5 -XX:SurvivorRatio=32 -XX:+PerfDisableSharedMem -XX:MaxTenuringThreshold=1 -Dusing.aikars.flags=https://mcflags.emc.gs -Daikars.new.flags=true -Dlog4j2.formatMsgNoLookups=true{Environment.NewLine}");
                    break;
                case "Fabric":
                    File.AppendAllText(Config, $@"brand=Fabric{Environment.NewLine}jargs=-XX:+UseG1GC -XX:+ParallelRefProcEnabled -XX:MaxGCPauseMillis=200 -XX:+UnlockExperimentalVMOptions -XX:+DisableExplicitGC -XX:+AlwaysPreTouch -XX:G1NewSizePercent=30 -XX:G1MaxNewSizePercent=40 -XX:G1HeapRegionSize=8M -XX:G1ReservePercent=20 -XX:G1HeapWastePercent=5 -XX:G1MixedGCCountTarget=4 -XX:InitiatingHeapOccupancyPercent=15 -XX:G1MixedGCLiveThresholdPercent=90 -XX:G1RSetUpdatingPauseTimePercent=5 -XX:SurvivorRatio=32 -XX:+PerfDisableSharedMem -XX:MaxTenuringThreshold=1 -Dusing.aikars.flags=https://mcflags.emc.gs -Daikars.new.flags=true -Dlog4j2.formatMsgNoLookups=true{Environment.NewLine}");
                    break;
                case "Forge":
                    File.AppendAllText(Config, $@"brand=Forge{Environment.NewLine}jargs={File.ReadAllText($@"{AppDir}\libraries\net\minecraftforge\forge\{mcJar[1].Replace("forge-", "").Replace("-installer.jar", "")}\win_args.txt").Replace("\n", " ").Replace("\r", "") + Environment.NewLine}");
                    break;
            }

            currentStage++;
            goStage();
        }

        private void FinalStage()
        {
            Label text = new Label() { TextAlign = ContentAlignment.MiddleCenter, Text = $"The First Time Setup has completed.{Environment.NewLine + Environment.NewLine}Press 'Finish' to start the Minecraft Server Client.", AutoSize = true, Location = new Point(panel1.Width / 2 / 2, panel1.Height / 2) };
            nextStage.Enabled = true;
            splitContainer2.Panel2.Controls.Remove(goAStageBack);
            splitContainer2.Panel2.Controls.Remove(quitSetup);
            nextStage.Text = "Finish";
            panel1.Controls.Add(text);
        }

        private void goStage()
        {
            switch (currentStage)
            {
                case 0:
                    ClearArea();
                    nextStage.Enabled = true;
                    panel1.Visible = goAStageBack.Enabled = false;
                    stageTitle.Text = "First Time Setup";
                    break;
                case 1:
                    ClearArea();
                    panel1.Visible = goAStageBack.Enabled = true;
                    nextStage.Enabled = false;
                    stageTitle.Text = "First Time Setup - Server Configuration";
                    Stage1();
                    break;
                case 2:
                    ClearArea();
                    goAStageBack.Enabled = nextStage.Enabled = quitSetup.Enabled = false;
                    stageTitle.Text = "First Time Setup - Download Server Data";
                    Stage2();
                    break;
                case 3:
                    ClearArea();
                    stageTitle.Text = "First Time Setup";
                    FinalStage();
                    break;
                case 4:
                    Directory.Delete($@"{AppDir}\temp\", true);
                    Process.Start(Application.ExecutablePath);
                    Environment.Exit(0);
                    break;
            }
        }

        private string[] getBrandMetadata(string brand)
        {
            WebClient web = new WebClient();
            List<string> data = new List<string>();
            if (brand == "Vanilla")
            {
                if (mcBrand == brand)
                {
                    data.Clear();
                    for (var i = 0; i < brandMetadata.Count; i++)
                    {
                        data.Add(brandMetadata[i].id);
                    }
                }
                else
                {
                    brandMetadata.Clear();
                    dynamic response = JsonConvert.DeserializeObject(web.DownloadString("https://launchermeta.mojang.com/mc/game/version_manifest.json"));
                    JArray versions = response.versions;
                    for (var i = 0; i < versions.Count; i++)
                    {
                        if (versions[i]["type"].ToObject<VersionType>() == VersionType.release)
                        {
                            brandMetadata.Add(new VersionData(versions[i]["id"].ToString(), versions[i]["url"].ToString()));
                            data.Add(versions[i]["id"].ToString());
                        }
                    }
                    mcBrand = brand;
                }
            }
            else if (brand == "Fabric")
            {
                if (mcBrand == brand)
                {
                    data.Clear();
                    for (var i = 0; i < brandMetadata.Count; i++)
                    {
                        data.Add(brandMetadata[i].id);
                    }
                }
                else
                {
                    brandMetadata.Clear();
                    dynamic response = JsonConvert.DeserializeObject(web.DownloadString("https://launchermeta.mojang.com/mc/game/version_manifest.json"));
                    JArray versions = response.versions;
                    for (var i = 0; i < versions.Count; i++)
                    {
                        if (versions[i]["type"].ToObject<VersionType>() == VersionType.release)
                        {
                            if ((string)versions[i]["id"] != "1.13.2")
                            {
                                brandMetadata.Add(new VersionData((string)versions[i]["id"], (string)versions[i]["url"]));
                                data.Add(versions[i]["id"].ToString());
                            }
                            else break;
                        }
                    }
                    mcBrand = brand;
                }
            }
            else if (brand == "Forge")
            {
                if (mcBrand == brand)
                {
                    data.Clear();
                    for (var i = 0; i < brandMetadata.Count; i++)
                    {
                        data.Add(brandMetadata[i].id);
                    }
                }
                else
                {
                    brandMetadata.Clear();
                    dynamic response = JsonConvert.DeserializeObject(web.DownloadString("https://launchermeta.mojang.com/mc/game/version_manifest.json"));
                    JArray versions = response.versions;
                    for (var i = 0; i < versions.Count; i++)
                    {
                        if (versions[i]["type"].ToObject<VersionType>() == VersionType.release)
                        {
                            if ((string)versions[i]["id"] != "1.0")
                            {
                                brandMetadata.Add(new VersionData((string)versions[i]["id"], (string)versions[i]["url"]));
                                data.Add(versions[i]["id"].ToString());
                            }
                            else break;
                        }
                    }
                    mcBrand = brand;
                }
            }
            return data.ToArray();
        }

        private string[] getJDK()
        {
            if (mcVersion.StartsWith("1.18")) { JDKArchiveFolder = "jdk-17.0.1"; return new string[] { "https://download.java.net/java/GA/jdk17.0.1/2a2082e5a09d4267845be086888add4f/12/GPL/openjdk-17.0.1_windows-x64_bin.zip", "openjdk-17.0.1_windows-x64_bin.zip" }; }
            else if (mcVersion.StartsWith("1.17")) { JDKArchiveFolder = "jdk-16.0.1"; return new string[] { "https://download.java.net/java/GA/jdk16.0.1/7147401fd7354114ac51ef3e1328291f/9/GPL/openjdk-16.0.1_windows-x64_bin.zip", "openjdk-16.0.1_windows-x64_bin.zip" }; }
            else if (mcVersion.StartsWith("1.16") || mcVersion.StartsWith("1.15") || mcVersion.StartsWith("1.14") || mcVersion.StartsWith("1.13")) { JDKArchiveFolder = "jdk-11.0.2"; return new string[] { "https://download.java.net/java/GA/jdk11/9/GPL/openjdk-11.0.2_windows-x64_bin.zip", "jdk-11.0.13_windows-x64_bin.zip" }; }
            else { JDKArchiveFolder = "jdk8u312-b07-jre"; return new string[] { "https://github.com/adoptium/temurin8-binaries/releases/download/jdk8u312-b07/OpenJDK8U-jre_x64_windows_hotspot_8u312b07.zip", "OpenJDK8U-jre_x64_windows_hotspot_8u312b07.zip" }; }
        }

        private string[] getMCJar()
        {
            WebClient web = new WebClient();
            var url = "";
            var filename = "";
            if (mcBrand == "Vanilla")
            {
                string verDataUrl = "";
                for (var i = 0; i < brandMetadata.Count; i++)
                {
                    if (brandMetadata[i].id == mcVersion)
                    {
                        verDataUrl = brandMetadata[i].url;
                        break;
                    }
                }
                dynamic response = JsonConvert.DeserializeObject(web.DownloadString(verDataUrl));
                url = response.downloads.server.url;
                filename = url.Substring(url.LastIndexOf("/") + 1);
            }
            else if (mcBrand == "Fabric")
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(web.DownloadString("https://maven.fabricmc.net/net/fabricmc/fabric-installer/maven-metadata.xml"));
                var installer = doc.DocumentNode.SelectSingleNode("//metadata//versioning//release").InnerText;
                dynamic loader = JsonConvert.DeserializeObject(web.DownloadString("https://meta.fabricmc.net/v2/versions/loader"));
                var lver = "";
                for (var i = 0; i < loader.Count; i++) { if (loader[i].stable == true) { lver = loader[i].version; break; } }
                url = $@"https://meta.fabricmc.net/v2/versions/loader/{mcVersion}/{lver}/{installer}/server/jar";
                filename = "server.jar";
            }
            else if (mcBrand == "Forge")
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(web.DownloadString($"https://files.minecraftforge.net/net/minecraftforge/forge/index_{mcVersion}.html"));
                url = doc.DocumentNode.SelectSingleNode("//main//div//div//div//div//div//div//div//a").Attributes["href"].Value.Replace("https://adfoc.us/serve/sitelinks/?id=271228&url=", "");
                filename = url.Substring(url.LastIndexOf("/") + 1);
            }
            return new string[] { url, filename };
        }

        private void quitSetup_Click(object sender, EventArgs e)
        {
            var m = MessageBox.Show("Are you sure you want to quit the setup?", "Minecraft Server Client", MessageBoxButtons.YesNo);
            if (m == DialogResult.Yes) Application.Exit();
        }
        private void nextStage_Click(object sender, EventArgs e)
        {
            currentStage++;
            goStage();
        }
        private void goAStageBack_Click(object sender, EventArgs e)
        {
            currentStage--;
            goStage();
        }
    }

    public struct VersionData
    {
        public VersionData(string Id, string Url)
        {
            id = Id;
            url = Url;
        }
        public string id { get; internal set; }
        public string url { get; internal set; }
    }
    public enum VersionType
    {
        snapshot,
        release,
        old_beta,
        old_alpha
    }
}
