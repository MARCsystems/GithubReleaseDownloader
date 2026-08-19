using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace GithubReleaseDownloader
{
    internal class Updater
    {
        #region Events
        public event Action<string> CheckUpdateReport;
        public event Action ReportReady;
        public event Action<string, string, double> ReportDownloadPercentage;
        public event Action<bool, string> DownloadReport;
        #endregion

        #region Private Variables
        private string repositoryOwner = "";
        private string repositoryName = "";
        private Version currentAppVersion = new Version();
        private string updateFilePath = "";
        private string tokenID = "";

        private List<VersionEntry> versions = new List<VersionEntry>();
        #endregion

        #region Private Functions
        private string GetReleaseUrl()
        {
            return $"https://api.github.com/repos/{repositoryOwner}/{repositoryName}/release";
        }
        #endregion

        #region Class Properties
        public string RepositoryOwner
        {
            set { repositoryOwner = value; }
            get { return repositoryOwner; }
        }

        public string RepositoryName
        {
            set { repositoryName = value; }
            get { return repositoryName; }
        }

        public string UpdateFileSavePath
        {
            set { updateFilePath = value; }
            get { return updateFilePath; }
        }

        public string TokenID
        {
            set { tokenID = value; }
            get { return tokenID; }
        }

        public Version CurrentAppVersion
        {
            set { currentAppVersion = value; }
            get { return currentAppVersion; }
        }

        public VersionEntry[] FetchedVersions
        {
            get
            {
                return versions.ToArray();
            }
        }

        public VersionEntry LatestStable
        {
            get
            {
                return versions.Where(v => !v.IsPreRelease).Where(v => v.VersionInfo.CompareTo(CurrentAppVersion) == 1).FirstOrDefault();
            }
        }

        public VersionEntry LatestPreRelease
        {
            get
            {
                return versions.Where(v => v.IsPreRelease).Where(v => v.VersionInfo.CompareTo(CurrentAppVersion) == 1).Where(v => v.VersionInfo.CompareTo(LatestStable?.VersionInfo ?? currentAppVersion) == 1).FirstOrDefault();
            }
        }
        #endregion

        #region Download and Install
        public async void BeginDownload()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    if (!string.IsNullOrEmpty(tokenID))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenID);
                    }

                    using (HttpResponseMessage response = await client.GetAsync(GetReleaseUrl()))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            long totalBytes = response.Content.Headers.ContentLength ?? -1;
                            using (Stream contentStream = await response.Content.ReadAsStreamAsync(), fileStream = new FileStream(updateFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                byte[] buffer = new byte[65536];
                                long totalRead = 0;
                                int bytesRead;

                                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                                    totalRead += bytesRead;

                                    if (totalBytes > 0)
                                    {
                                        double percent = Math.Round((double)totalRead / totalBytes * 100, 2);
                                        ReportDownloadPercentage?.Invoke(UnitCollapser.CollapseBytes(totalRead), UnitCollapser.CollapseBytes(totalBytes), percent);
                                        Thread.Sleep(new Random().Next(10, 25));
                                    }
                                    else
                                    {
                                        ReportDownloadPercentage?.Invoke(UnitCollapser.CollapseBytes(totalRead), "-1", -1);
                                    }
                                }
                            }

                            DownloadReport?.Invoke(false, $"Download Failed! [Status Code {response.StatusCode}]");
                        }
                    }
                }
                catch (Exception err)
                {
                    DownloadReport?.Invoke(false, $"Download failed due to the following error(s)\r\n\r\n{err.Message}\r\n\r\n{err.StackTrace}");
                }
            }
        }
        #endregion
    }
}