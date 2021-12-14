using MCQuery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Timer = System.Windows.Forms.Timer;

namespace MSC
{
    public partial class MainUI : Form
    {
        private readonly string AppDir = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf(char.Parse(@"\")));
        private readonly string Config = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf(char.Parse(@"\"))) + @"\config.json";
        private readonly Timer usageUpdate = new Timer() { Interval = 1000 };
        private readonly Timer statusUpdate = new Timer() { Interval = 1000 };

        private DateTime curTime;
        private DateTime lastTime;
        private TimeSpan curTotalCPUTime;
        private TimeSpan lastTotalCPUTime;

        private Process server;
        private MCServer serverStatus = new MCServer("127.0.0.1", 25565);
        private bool started = false;

        private string Java;
        private int RAM = 2048;
        private string Args;
        private string Brand;

        private List<string> configScratch = new List<string>();

        public MainUI()
        {
            InitializeComponent();
            Java = $@"{AppDir}\runtime\bin\java.exe";
            var a = File.ReadAllLines(Config);
            for (var i = 0; i < a.Length; i++)
            {
                if (a[i].StartsWith("brand")) Brand = a[i].Substring(a[i].IndexOf("=") + 1);
                else if (a[i].StartsWith("jargs")) Args = a[i].Substring(a[i].IndexOf("=") + 1);
            }
            usageUpdate.Tick += (object sender, EventArgs e) => { if (started) UpdateUsage(server); };
            statusUpdate.Tick += (object sender, EventArgs e) => { if (started) { try { plvr.Text = $"{serverStatus.Status().Players.Online}/{serverStatus.Status().Players.Max+Environment.NewLine}Minecraft {serverStatus.Status().Version.Name}"; } catch { } } };
            usageUpdate.Enabled = statusUpdate.Enabled = true;
            UpdateConfig();
            GetMods();
            GetWhitelist();
            GetOPList();
            GetJDKVersion();
        }
        private void MainUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (started)
            {
                server.Close();
                server.WaitForExit();
            }
            usageUpdate.Enabled = statusUpdate.Enabled = false;
        }
        private void UpdateUsage(Process p)
        {
            p.Refresh();
            if (lastTime == null || lastTime == new DateTime())
            {
                lastTime = DateTime.Now;
                lastTotalCPUTime = p.TotalProcessorTime;
            }
            else
            {
                curTime = DateTime.Now;
                curTotalCPUTime = p.TotalProcessorTime;
                var c = (curTotalCPUTime.TotalMilliseconds - lastTotalCPUTime.TotalMilliseconds) / curTime.Subtract(lastTime).TotalMilliseconds / Convert.ToDouble(Environment.ProcessorCount) * 100;
                var r = p.WorkingSet64 / 1024 / 1024;
                cpu.Series[0].Points.RemoveAt(0);
                ram.Series[0].Points.RemoveAt(0);
                cpu.Series[0].Points.Add(new DataPoint(7, c) { AxisLabel = " " });
                ram.Series[0].Points.Add(new DataPoint(7, r) { AxisLabel = " " });
                if ((c + 5) > cpu.ChartAreas[0].AxisY.Maximum) cpu.ChartAreas[0].AxisY.Maximum = c + 5;
                ram.ChartAreas[0].AxisY.Maximum = p.PeakWorkingSet64 / 1024 / 1024;
                cpuBox.Text = $"CPU Usage: {Math.Round(c, 2)}%";
                ramBox.Text = $"RAM Usage: {Math.Round((decimal)r, 0)} MB";
            }
        }
        private void StartServer()
        {
            if (Brand == "Forge")
            {
                server = new Process() { StartInfo = new ProcessStartInfo(Java, $"-Xmx{RAM}M -Xms{RAM}M {Args} nogui") { RedirectStandardInput = true, RedirectStandardError = true, RedirectStandardOutput = true, CreateNoWindow = true, UseShellExecute = false }, EnableRaisingEvents = true };
            }
            else
            {
                server = new Process() { StartInfo = new ProcessStartInfo(Java, $"-Xmx{RAM}M -Xms{RAM}M {Args} -jar \"{AppDir}\\server.jar\" nogui") { RedirectStandardInput = true, RedirectStandardError = true, RedirectStandardOutput = true, CreateNoWindow = true, UseShellExecute = false }, EnableRaisingEvents = true };
            }
            server.ErrorDataReceived += DataReceived;
            server.OutputDataReceived += DataReceived;
            server.Start();
            server.BeginErrorReadLine();
            server.BeginOutputReadLine();
            started = true;
            server.WaitForExit();
            started = false;
            plvr.ResetText();
            start.Enabled = true;
            status.Text = "Status: Offline";
            cpu.Series[0].Points.Clear();
            ram.Series[0].Points.Clear();
            cpuBox.Text = "CPU Usage: 0.00%";
            ramBox.Text = "RAM Usage: 0 MB";
            return;
        }
        private void UpdateConfig()
        {
            try
            {
                var config = File.ReadAllLines($@"{AppDir}\server.properties");
                for (var i = 0; i < config.Length; i++)
                {
                    configScratch.Add(config[i]);
                    if (config[i].StartsWith("level-seed")) worldSeed.Text = config[i].Substring(config[i].IndexOf("=") + 1);
                    else if (config[i].StartsWith("gamemode")) defaultGamemode.SelectedItem = defaultGamemode.Items.IndexOf(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("enable-command-block")) enableCB.Checked = bool.Parse(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("level-name")) worldName.Text = config[i].Substring(config[i].IndexOf("=") + 1);
                    else if (config[i].StartsWith("motd")) serverMOTD.Text = config[i].Substring(config[i].IndexOf("=") + 1);
                    else if (config[i].StartsWith("pvp")) enablePVP.Checked = bool.Parse(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("generate-structures")) generateStructures.Checked = bool.Parse(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("difficulty")) difficulty.SelectedIndex = difficulty.Items.IndexOf(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("require-resource-pack")) requireResrcPack.Checked = bool.Parse(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("max-players")) maxPlayers.Value = int.Parse(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("online-mode")) onlineMode.Checked = bool.Parse(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("allow-flight")) allowFlight.Checked = bool.Parse(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("allow-nether")) enableNether.Checked = bool.Parse(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("force-gamemode")) forceGameMode.Checked = bool.Parse(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("hardcore") && bool.Parse(config[i].Substring(config[i].IndexOf("=") + 1))) defaultGamemode.SelectedIndex = defaultGamemode.Items.IndexOf("Hardcore");
                    else if (config[i].StartsWith("whitelist")) enableWhitelist.Checked = bool.Parse(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("spawn-npcs")) spawnNPCs.Checked = bool.Parse(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("spawn-animals")) spawnAnimals.Checked = bool.Parse(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("spawn-monsters")) spawnMonsters.Checked = bool.Parse(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("enforce-whitelist")) enforceWhitelist.Checked = bool.Parse(config[i].Substring(config[i].IndexOf("=") + 1));
                    else if (config[i].StartsWith("spawn-protection")) spawnProtection.Value = int.Parse(config[i].Substring(config[i].IndexOf("=") + 1));
                }
            }
            catch { }
        }
        private void GetMods()
        {
            try
            {
                var mods = Directory.GetFiles($@"{AppDir}\mods");
                for (var i = 0; i < mods.Length; i++)
                {
                    if (mods[i].EndsWith(".jar"))
                    {
                        dynamic metadata = JsonConvert.DeserializeObject(new string(new StreamReader(ZipFile.OpenRead($@"{AppDir}\mods\{mods[i]}").Entries.Where(x => x.Name.Equals("fabric.mod.json", StringComparison.InvariantCulture)).FirstOrDefault().Open(), Encoding.UTF8).ReadToEnd().ToArray()));
                        modsList.Nodes.Add(new TreeNode($@"{metadata.name} {metadata.version}") { ToolTipText = metadata.description });
                    }
                }
            }
            catch { }
        }
        private void GetWhitelist()
        {
            try
            {
                dynamic wl = JsonConvert.DeserializeObject(File.ReadAllText($@"{AppDir}\whitelist.json"));
                for (var i = 0; i < wl.Count; i++)
                {
                    whitelist.Nodes.Add(wl[i].name);
                }
            }
            catch { }
        }
        private void GetOPList()
        {
            try
            {
                dynamic op = JsonConvert.DeserializeObject(File.ReadAllText($@"{AppDir}\ops.json"));
                for (var i = 0; i < op.Count; i++)
                {
                    opList.Nodes.Add(op[i].name);
                }
            }
            catch { }
        }
        private void GetJDKVersion()
        {
            var jdk = new Process() { StartInfo = new ProcessStartInfo(Java, "-version") { RedirectStandardError = true, CreateNoWindow = true, UseShellExecute = false }, EnableRaisingEvents = true };
            jdk.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => { JDKVersion.Text = e.Data.Substring(e.Data.IndexOf("\"")+1,e.Data.LastIndexOf("\"")-e.Data.IndexOf("\"")-1); jdk.CancelErrorRead(); };
            jdk.Start();
            jdk.BeginErrorReadLine();
            jdk.WaitForExit();
        }
        private void DataReceived(object sender, DataReceivedEventArgs e)
        {
            consoleView.AppendText(e.Data + Environment.NewLine);
            if (e.Data.Contains($"[main/INFO]: Loading ") && !e.Data.StartsWith("<")) status.Text = "Status: Starting...";
            else if (e.Data.Contains("[Server thread/INFO]: Done") && !e.Data.StartsWith("<")) { status.Text = "Status: Online"; restart.Enabled = shutdown.Enabled = quit.Enabled = consoleInput.Enabled = true; }
            else if (e.Data.Contains("[Server thread/INFO]: Stopping server") && !e.Data.StartsWith("<")) { status.Text = "Status: Stopping..."; restart.Enabled = shutdown.Enabled = quit.Enabled = consoleInput.Enabled = false; }
        }
        private void consoleInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                server.StandardInput.WriteLine(consoleInput.Text);
                consoleInput.Clear();
                Focus();
                inputReminder.Show();
            };
        }
        private void start_Click(object sender, EventArgs e)
        {
            start.Enabled = false;
            Task.Run(StartServer);
        }
        private void restart_Click(object sender, EventArgs e)
        {
            server.StandardInput.WriteLine("stop");
            server.WaitForExit();
            Task.Run(StartServer);
        }
        private void shutdown_Click(object sender, EventArgs e)
        {
            server.StandardInput.WriteLine("stop");
            server.WaitForExit();
        }
        private void quit_Click(object sender, EventArgs e)
        {
            server.StandardInput.WriteLine("stop");
            server.WaitForExit();
            Environment.Exit(0);
        }
        private void FocusConsoleInput(object sender, EventArgs e)
        {
            inputReminder.Hide();
            consoleInput.Focus();
        }
        private void consoleInput_Leave(object sender, EventArgs e) { if (consoleInput.Text == "") { inputReminder.Show(); } Focus(); }
        private void randomSeed_Click(object sender, EventArgs e)
        {
            worldSeed.ResetText();
            worldSeed.Text += new Random().Next(int.MinValue, int.MaxValue);
        }
        private void discardChanges_Click(object sender, EventArgs e) => UpdateConfig();
        private void saveChanges_Click(object sender, EventArgs e)
        {
            File.Delete($@"{AppDir}\server.properties");
            try
            {
                for (var i = 0; i < configScratch.Count; i++)
                {
                    if (configScratch[i].StartsWith("level-seed")) File.AppendAllText($@"{AppDir}\server.properties", $"level-seed={worldSeed.Text + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("gamemode") && ((string)defaultGamemode.SelectedItem) != "Hardcore") File.AppendAllText($@"{AppDir}\server.properties", $"gamemode={defaultGamemode.SelectedValue.ToString().ToLower() + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("gamemode") && ((string)defaultGamemode.SelectedItem) == "Hardcore") File.AppendAllText($@"{AppDir}\server.properties", $"gamemode=survival{Environment.NewLine}hardcore=true{Environment.NewLine}");
                    else if (configScratch[i].StartsWith("enable-command-block")) File.AppendAllText($@"{AppDir}\server.properties", $"enable-command-block={enableCB.Checked.ToString().ToLower() + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("level-name")) File.AppendAllText($@"{AppDir}\server.properties", $"level-name={worldName.Text + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("motd")) File.AppendAllText($@"{AppDir}\server.properties", $"motd={serverMOTD.Text + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("pvp")) File.AppendAllText($@"{AppDir}\server.properties", $"pvp={enablePVP.Checked.ToString().ToLower() + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("generate-structures")) File.AppendAllText($@"{AppDir}\server.properties", $"generate-structures={generateStructures.Checked.ToString().ToLower() + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("difficulty")) File.AppendAllText($@"{AppDir}\server.properties", $"difficulty={difficulty.SelectedItem.ToString().ToLower() + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("require-resource-pack")) File.AppendAllText($@"{AppDir}\server.properties", $"require-resource-pack={requireResrcPack.Checked.ToString().ToLower() + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("max-players")) File.AppendAllText($@"{AppDir}\server.properties", $"max-players={maxPlayers.Value + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("online-mode")) File.AppendAllText($@"{AppDir}\server.properties", $"online-mode={onlineMode.Checked.ToString().ToLower() + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("allow-flight")) File.AppendAllText($@"{AppDir}\server.properties", $"allow-flight={allowFlight.Checked.ToString().ToLower() + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("allow-nether")) File.AppendAllText($@"{AppDir}\server.properties", $"allow-nether={enableNether.Checked.ToString().ToLower() + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("force-gamemode")) File.AppendAllText($@"{AppDir}\server.properties", $"force-gamemode={forceGameMode.Checked.ToString().ToLower() + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("whitelist")) File.AppendAllText($@"{AppDir}\server.properties", $"whitelist={enableWhitelist.Checked.ToString().ToLower() + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("spawn-npcs")) File.AppendAllText($@"{AppDir}\server.properties", $"spawn-npcs={spawnNPCs.Checked.ToString().ToLower() + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("spawn-animals")) File.AppendAllText($@"{AppDir}\server.properties", $"spawn-animals={spawnAnimals.Checked.ToString().ToLower() + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("spawn-monsters")) File.AppendAllText($@"{AppDir}\server.properties", $"spawn-monsters={spawnMonsters.Checked.ToString().ToLower() + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("enforce-whitelist")) File.AppendAllText($@"{AppDir}\server.properties", $"enforce-whitelist={enforceWhitelist.Checked.ToString().ToLower() + Environment.NewLine}");
                    else if (configScratch[i].StartsWith("spawn-protection")) File.AppendAllText($@"{AppDir}\server.properties", $"spawn-protection={spawnProtection.Value + Environment.NewLine}");
                    else File.AppendAllText($@"{AppDir}\server.properties", configScratch[i] + Environment.NewLine);
                }
                MessageBox.Show("Saved changes.", "Minecraft Server Client", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save changes.{Environment.NewLine + Environment.NewLine + ex.Message}", "Minecraft Server Client", MessageBoxButtons.OK);
            }
        }
        private void refreshMods_Click(object sender, EventArgs e) => GetMods();
        private void addWhitelist_Click(object sender, EventArgs e)
        {
            if (!started) { MessageBox.Show("You need to start the server to add to the whitelist!", "Minecraft Server Client", MessageBoxButtons.OK); return; }
            try
            {
                server.StandardInput.WriteLine($"whitelist add {playerWOInput.Text}");
                playerWOInput.Clear();
                whitelist.Nodes.Clear();
                GetWhitelist();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Minecraft Server Client", MessageBoxButtons.OK); }
        }
        private void addOP_Click(object sender, EventArgs e)
        {
            if (!started) { MessageBox.Show("You need to start the server to add to the OP list!", "Minecraft Server Client", MessageBoxButtons.OK); return; }
            try
            {
                server.StandardInput.WriteLine($"op {playerWOInput.Text}");
                playerWOInput.Clear();
                opList.Nodes.Clear();
                GetOPList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Minecraft Server Client", MessageBoxButtons.OK); }
        }
        private void backupWorld_Click(object sender, EventArgs e)
        {
            try
            {
                var backupName = $@"(Backup - {DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year + " " + DateTime.Now.Hour + ";" + DateTime.Now.Minute}\)";
                backupName = $@"{AppDir}\backups\{worldName.Text} - {backupName}";
                Directory.CreateDirectory(backupName);
                var d = Directory.GetDirectories($@"{AppDir}\{worldName.Text}", "*", SearchOption.AllDirectories);
                for (var i = 0; i < d.Length; i++)
                {
                    Directory.CreateDirectory(backupName + @"\" + d[i].Substring(d[i].IndexOf(worldName.Text) + worldName.Text.Length + 1));
                }
                var f = Directory.GetFiles($@"{AppDir}\{worldName}", "*", SearchOption.AllDirectories);
                for (var i = 0; i < f.Length; i++)
                {
                    File.Copy(f[i], backupName + @"\" + f[i].Substring(f[i].IndexOf(worldName.Text) + worldName.Text.Length + 1), true);
                }
                MessageBox.Show("Backup Completed!");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Minecraft Server Client", MessageBoxButtons.OK); }
        }
        private void replaceWorld_Click(object sender, EventArgs e)
        {
            if (started) { MessageBox.Show("The server has to be stopped to replace the world!", "Minecraft Server Client", MessageBoxButtons.OK); return; }
            FolderBrowserDialog folder = new FolderBrowserDialog() { ShowNewFolderButton = false, Description = "Select World Folder", RootFolder = Environment.SpecialFolder.ApplicationData };
            if (folder.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    backupWorld_Click(sender, e);
                    Directory.Delete($@"{AppDir}\{worldName.Text}", true);
                    Directory.CreateDirectory($@"{AppDir}\{worldName.Text}");
                    var d = Directory.GetDirectories(folder.SelectedPath, "*", SearchOption.AllDirectories);
                    for (var i = 0; i < d.Length; i++)
                    {
                        Directory.CreateDirectory($@"{AppDir}\{worldName.Text + @"\" + d[i].Substring(d[i].IndexOf(folder.SelectedPath) + folder.SelectedPath.Length + 1)}");
                    }
                    var f = Directory.GetFiles(folder.SelectedPath, "*", SearchOption.AllDirectories);
                    for (var i = 0; i < f.Length; i++)
                    {
                        File.Copy(f[i], $@"{AppDir}\{worldName.Text + @"\" + f[i].Substring(f[i].IndexOf(folder.SelectedPath) + folder.SelectedPath.Length + 1)}");
                    }
                    MessageBox.Show("World has been replaced. The previous world has been placed in the backups folder.", "Minecraft Server Client", MessageBoxButtons.OK);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Minecraft Server Client", MessageBoxButtons.OK); }
            }
        }
        private void deleteWorld_Click(object sender, EventArgs e)
        {
            if (started) { MessageBox.Show("The server has to be stopped to delete the world!", "Minecraft Server Client", MessageBoxButtons.OK); return; }
            if (MessageBox.Show($"Are you sure you want to delete the world?{Environment.NewLine}This action is irreversible and will probably make you cry if you press Yes accidentally.{Environment.NewLine + Environment.NewLine}Just saying...", "Minecraft Server Client", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (MessageBox.Show("Are you really REALLY sure???", "Minecraft Server Client", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        Directory.Delete($@"{AppDir}\{worldName.Text}\", true);
                        MessageBox.Show("World deleted.", "Minecraft Server Client", MessageBoxButtons.OK);
                    }
                    catch (Exception ex) { MessageBox.Show($"Failed to delete the world.{Environment.NewLine + ex.Message}", "Minecraft Server Client", MessageBoxButtons.OK); }
                }
            }
        }
        private void DAPD_Click(object sender, EventArgs e)
        {
            if (started) { MessageBox.Show("The server has to be stopped to delete player data!", "Minecraft Server Client", MessageBoxButtons.OK); return; }
            if (MessageBox.Show($"Are you sure you want to delete all player data?{Environment.NewLine}", "Minecraft Server Client", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    Directory.Delete($@"{AppDir}\{worldName.Text}\playerdata", true);
                    Directory.Delete($@"{AppDir}\{worldName.Text}\stats", true);
                    Directory.Delete($@"{AppDir}\{worldName.Text}\advancements", true);
                    MessageBox.Show("World deleted.", "Minecraft Server Client", MessageBoxButtons.OK);
                }
                catch (Exception ex) { MessageBox.Show($"Failed to delete the world.{Environment.NewLine + ex.Message}", "Minecraft Server Client", MessageBoxButtons.OK); }
            }
        }
        private void installNewServer_Click(object sender, EventArgs e)
        {
            if (started) { MessageBox.Show("The server has to be stopped to change the server version / brand!", "Minecraft Server Client", MessageBoxButtons.OK); return; }
            try
            {
                Directory.Delete($@"{AppDir}\logs", true);
                Directory.Delete($@"{AppDir}\libraries", true);
                Directory.Delete($@"{AppDir}\versions", true);
                Directory.Delete($@"{AppDir}\runtime", true);
                try { Directory.Move($@"{AppDir}\mods", $@"{AppDir}\mods.old"); } catch { }
                if (Brand == "Vanilla") File.Delete($@"{AppDir}\server.jar");
                else if (Brand == "Fabric"){File.Delete($@"{AppDir}\server.jar");File.Delete($@"{AppDir}\fabric-server-launch.jar");File.Delete($@"{AppDir}\fabric-server-launch.properties");Directory.Delete($@"{AppDir}\.fabric", true);}
                Process.Start(Application.ExecutablePath);
                Environment.Exit(0);
            }
            catch (Exception ex){ MessageBox.Show($"Error: {ex.Message}", "Minecraft Server Client", MessageBoxButtons.OK); };
        }
    }
}
