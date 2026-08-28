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
            updater.CheckUpdateReportReady += () =>
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate ()
                    {
                        toggleInteractables(true);
                        populateUpdateTable();
                    });
                }
                else
                {
                    toggleInteractables(true);
                    populateUpdateTable();
                }
            };
        }

        private void populateUpdateTable()
        {
            dgv_Releases.Rows.Clear();
            foreach (VersionEntry entry in updater.FetchedVersions)
            {
                foreach(VersionEntry.VersionAsset asset in entry.AssetsInfo)
                {
                    dgv_Releases.Rows.Add("⬇", entry.VersionSequence, entry.VersionName, asset.AssetName, asset.AssetHashType);
                }
            }
        }

        private void toggleInteractables(bool toggle)
        {
            txt_RepoOwner.Enabled = toggle;
            txt_RepoName.Enabled = toggle;
            txt_TempInstallerPath.Enabled = toggle;
            btn_Browse.Enabled = toggle;
            txt_PrivateRepoKey.Enabled = toggle;
            txt_PEMpath.Enabled = toggle;
            txt_AppID.Enabled = toggle;
            txt_InstallationID.Enabled = toggle;
            btn_StartQuery.Enabled = toggle;
        }

        private void btn_StartQuery_Click(object sender, EventArgs e)
        {
            toggleInteractables(false);
            updater.CurrentAppVersion = new Version(0, 0, 0, 0);
            updater.RepositoryOwner = txt_RepoOwner.Text.Trim();
            updater.RepositoryName = txt_RepoName.Text.Trim();
            updater.PAT_Token = txt_PrivateRepoKey.Text.Trim();
            //updater.CheckForUpdates("application/x-msdownload", true);
            updater.CheckForUpdates("*", (ReleaseMode)cmb_ReleaseMode.SelectedItem, true);
        }

        private void dgv_Releases_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void btn_Browse_Click(object sender, EventArgs e)
        {
            if (fbd_downloadpath.ShowDialog() == DialogResult.OK)
            {
                txt_TempInstallerPath.Text = fbd_downloadpath.SelectedPath;
            }
        }

        private void cmb_ReleaseMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            grp_PATMode.Enabled = (ReleaseMode)cmb_ReleaseMode.SelectedItem == ReleaseMode.PRIVATE_PAT;
            grp_PEMMode.Enabled = (ReleaseMode)cmb_ReleaseMode.SelectedItem == ReleaseMode.PRIVATE_PEM;
        }
    }
}
