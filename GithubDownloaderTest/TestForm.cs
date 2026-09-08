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
        private List<VersionEntry.VersionAsset> versions = new List<VersionEntry.VersionAsset>();
        private bool isLocked = false;

        internal TestForm()
        {
            InitializeComponent();

            Load += (a, b) =>
            {
                cmb_ReleaseMode.Items.Clear();
                foreach(ReleaseMode mode in Enum.GetValues(typeof(ReleaseMode)))
                {
                    cmb_ReleaseMode.Items.Add(mode);
                }
                cmb_ReleaseMode.SelectedIndex = 0;
            };

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
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate ()
                    {
                        toggleInteractables(versions.Count == 0);
                        txt_Progress.Text = downloadSuccess ? $"Success - Downloaded {val}!" : $"Failed downloading {val}!";
                    });
                }
                else
                {
                    Invoke((MethodInvoker)delegate ()
                    {
                        toggleInteractables(versions.Count == 0);
                        txt_Progress.Text = downloadSuccess ? $"Success - Downloaded {val}!" : $"Failed downloading {val}!";
                    });
                }
            };
            updater.DownloadEventStopped += () =>
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate ()
                    {
                        toggleInteractables(versions.Count == 0);

                        btn_StartQuery.Text = "Unlock";
                        btn_StartQuery.Enabled = true;

                        dgv_Releases.Enabled = true;
                    });
                }
                else
                {
                    Invoke((MethodInvoker)delegate ()
                    {
                        toggleInteractables(versions.Count == 0);

                        btn_StartQuery.Text = "Unlock";
                        btn_StartQuery.Enabled = true;

                        dgv_Releases.Enabled = true;
                    });
                }
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
            updater.CheckUpdateReportReady += (isFetched) =>
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate ()
                    {
                        toggleInteractables(!isFetched);
                        populateUpdateTable();

                        btn_StartQuery.Text = isFetched ? "Unlock" : "Start Query";
                        btn_StartQuery.Enabled = true;

                        dgv_Releases.Enabled = true;

                        isLocked = isFetched;
                    });
                }
                else
                {
                    toggleInteractables(!isFetched);
                    populateUpdateTable();

                    btn_StartQuery.Text = isFetched ? "Unlock" : "Start Query";
                    btn_StartQuery.Enabled = true;

                    dgv_Releases.Enabled = true;

                    isLocked = isFetched;
                }
            };
        }

        private void populateUpdateTable()
        {
            dgv_Releases.Rows.Clear();
            versions.Clear();
            foreach (VersionEntry entry in updater.FetchedVersions)
            {
                foreach(VersionEntry.VersionAsset asset in entry.AssetsInfo)
                {
                    versions.Add(asset);
                    dgv_Releases.Rows.Add("⬇", entry.VersionSequence, entry.VersionName, asset.AssetName, asset.AssetHashType);
                }
            }
        }

        private void toggleInteractables(bool toggle)
        {
            txt_RepoOwner.Enabled = toggle;
            txt_RepoName.Enabled = toggle;
            txt_TempInstallerPath.Enabled = toggle;
            cmb_ReleaseMode.Enabled = toggle;
            btn_Browse.Enabled = toggle;
            txt_PATkey.Enabled = toggle;
            txt_PEMpath.Enabled = toggle;
            txt_AppID.Enabled = toggle;
            txt_InstallationID.Enabled = toggle;
            btn_StartQuery.Enabled = toggle;
            dgv_Releases.Enabled = toggle;
        }

        private void btn_StartQuery_Click(object sender, EventArgs e)
        {
            if (isLocked)
            {
                isLocked = false;
                versions.Clear();
                dgv_Releases.Rows.Clear();
                btn_StartQuery.Text = "Start Query";
                toggleInteractables(true);
                return;
            }

            toggleInteractables(false);
            updater.CurrentAppVersion = new Version(0, 0, 0, 0);
            updater.RepositoryOwner = txt_RepoOwner.Text.Trim();
            updater.RepositoryName = txt_RepoName.Text.Trim();
            updater.UpdateFileSavePath = txt_TempInstallerPath.Text.Trim();
            updater.PAT_Token = txt_PATkey.Text.Trim();
            updater.PEM_FilePath = txt_PEMpath.Text.Trim();
            updater.PEM_AppId = txt_AppID.Text.Trim();
            updater.PEM_InstallationId = txt_InstallationID.Text.Trim();
            updater.MimeType = "*";
            updater.RepoReleaseMode = (ReleaseMode)cmb_ReleaseMode.SelectedItem;
            updater.CheckForUpdates(true);
        }

        private void dgv_Releases_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = dgv_Releases.CurrentCell.RowIndex, col = dgv_Releases.CurrentCell.ColumnIndex;

            if (col == 0)
            {
                toggleInteractables(false);
                txt_Progress.Text = "Downloading...";
                updater.BeginDownload(versions[row].AssetDownloadUrl, versions[row].AssetName);
            }
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

        private void btn_PEMbrowser_Click(object sender, EventArgs e)
        {
            if (ofd_PEMpath.ShowDialog() == DialogResult.OK)
            {
                txt_PEMpath.Text = ofd_PEMpath.FileName;
            }
        }
    }
}
