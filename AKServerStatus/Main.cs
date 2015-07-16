using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AKServerStatus.ServerChecker;
using AKServerStatus.ServerData;
using Timer = System.Windows.Forms.Timer;

namespace AKServerStatus
{
    public partial class Main : Form
    {
        private BackgroundWorker serverCheckWorker, refresherWorker;
        private const int RefreshTime = 60;
        private long lastRefresh;

        private Timer timer;

        private List<ServerInfo> currentServerList = new List<ServerInfo>();
        
        

        public Main()
        {
            InitializeComponent();
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (timer == null)
            {
                RunRefresherWorker();
                timer_tick(null, null);
            }
            else
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
                SetRefreshingState(false);
            }
        }

        private void RunRefresherWorker()
        {
            SetRefreshingState(true);
            timer = new Timer();
            timer.Tick += timer_tick;
            timer.Interval = RefreshTime*1000;
            timer.Start();
        }

        private void timer_tick(object sender, EventArgs o)
        {
            serverCheckWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            var newServerList = new List<ServerInfo>();
            serverCheckWorker.DoWork += (sender1, eventArgs) =>
            {
                newServerList = ServerStatusChecker.GetServerStatus();
            };
            serverCheckWorker.RunWorkerCompleted += (sender1, eventArgs) =>
            {
                SetNewServerList(newServerList);
            };
            serverCheckWorker.RunWorkerAsync();
        }

        private void SetNewServerList(List<ServerInfo> newServerList)
        {
            var updatedServers =
                newServerList.Where(
                    ns =>
                        currentServerList.Any(
                            cs => cs.Name == ns.Name && cs.IsOffline != ns.IsOffline && cs.AlertOnChange)).ToList();
            if (updatedServers.Any())
            {
                StringBuilder message = new StringBuilder("Server status updated:\n");
                foreach (var server in updatedServers)
                {
                    message.Append(string.Format("{0} is now {1}\n", server.Name,
                        server.IsOffline ? "offline" : "online (" + server.Ping + " ping)"));
                }
                SystemSounds.Exclamation.Play();
                MessageBox.Show(message.ToString(), "Servers updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            foreach (var server in currentServerList.Where(csl => csl.AlertOnChange))
            {
                var newServer = newServerList.FirstOrDefault(ns => ns.Name == server.Name);
                if (newServer != null)
                {
                    newServer.AlertOnChange = true;
                }
            }
            currentServerList = newServerList;
            serverBindingSource.DataSource = newServerList;

        }

        private void SetRefreshingState(bool refreshing)
        {
            btnStartStop.Text = refreshing ? "Stop refreshing" : "Start refreshing";
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            var _checkboxColumnIndex = 3;
            DataGridView dgv = (DataGridView) sender;
            if (_checkboxColumnIndex == dgv.CurrentCell.ColumnIndex &&
                dgv.Columns[_checkboxColumnIndex].GetType() == typeof (DataGridViewCheckBoxColumn) &&
                dgv.IsCurrentCellDirty)
            {
                //Remember that here dgv.CurrentCell.Value is previous/old value yet
                currentServerList[dgv.CurrentCell.RowIndex].AlertOnChange = !(bool) dgv.CurrentCell.Value;
            }

            dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
