using GithubReleaseDownloader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GithubDownloaderTest
{
    internal partial class TestForm : Form
    {
        private Updater updater;

        internal TestForm()
        {
            InitializeComponent();
            updater = new Updater();
            updater.CheckUpdateReport += (val) =>
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate ()
                    {
                        txt_Progress.Text = val;
                    });
                }
                else
                {
                    txt_Progress.Text = val;
                }
            };
            updater.DownloadReport += (downloadSuccess, val) =>
            {
                Console.WriteLine(downloadSuccess ? $"Success - Downloaded {val}!" : $"Failed downloading {val}!");
            };
            updater.ReportDownloadPercentage += (sizeCurrent, sizeTotal, percVal) =>
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate ()
                    {
                        txt_Progress.Text = $"[{sizeCurrent}/{sizeTotal}] {percVal.ToString("0.00")}%";
                    });
                }
                else
                {
                    txt_Progress.Text = $"[{sizeCurrent}/{sizeTotal}] {percVal.ToString("0.00")}%";
                }
            };
            updater.ReportReady += () =>
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate ()
                    {
                        toggleInteractables(true);
                    });
                }
                else
                {
                    toggleInteractables(true);
                }
            };
        }

        private void toggleInteractables(bool toggle)
        {
            txt_RepoOwner.Enabled = toggle;
            txt_RepoName.Enabled = toggle;
            txt_TempInstallerPath.Enabled = toggle;
            btn_Browse.Enabled = toggle;
            txt_PrivateRepoKey.Enabled = toggle;
            cmb_Extension.Enabled = toggle;
            btn_StartQuery.Enabled = toggle;
        }

        private void btn_StartQuery_Click(object sender, EventArgs e)
        {

        }
    }
}
