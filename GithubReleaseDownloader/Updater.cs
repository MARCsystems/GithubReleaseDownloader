using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace GithubReleaseDownloader
{
    public class Updater
    {
        #region Events
        public event Action<string> CheckUpdateReport;
        public event Action CheckUpdateReportReady;
        public event Action<string, string, double> ReportDownloadPercentage;
        public event Action<bool, string> DownloadReport;
        #endregion

        #region Private Variables
        private string repositoryOwner = "";
        private string repositoryName = "";
        private Version currentAppVersion = new Version(0,0,0,0);
        private string updateFilePath = "";
        private string tokenID = "";

        private List<VersionEntry> versions = new List<VersionEntry>();
        #endregion

        #region Private Functions
        private string GetReleaseUrl()
        {
            return $"https://api.github.com/repos/{repositoryOwner}/{repositoryName}/releases";
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

        #region Query, Download and Install
        public void CheckForUpdates(string mimetype, bool interruptIfFail = false)
        {
            versions.Clear();
            Thread updateThread = new Thread(() =>
            {
                #region Update Fetching Cycle
                do
                {
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            if (!string.IsNullOrEmpty(tokenID))
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenID);
                            }
                            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
                            HttpResponseMessage response = client.GetAsync(GetReleaseUrl()).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                if (response.RequestMessage.RequestUri.ToString().StartsWith("https://api.github.com/"))
                                {
                                    string jsonData = response.Content.ReadAsStringAsync().Result;
                                    JArray releases = JArray.Parse(jsonData);

                                    foreach (var release in releases)
                                    {
                                        DateTime publishTime;
                                        try // Parse with NLS standards
                                        {
                                            publishTime = DateTime.Parse(release["published_at"].ToString(), CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal);
                                        }
                                        catch (Exception dtEx) // Parse with ICU standards
                                        {
                                            Console.WriteLine($"Time String Parse: {release["published_at"].ToString()}\r\n\r\n{dtEx.Message}\r\n\r\n{dtEx.StackTrace}", "Win11 DateTime Parsing Error");
                                            publishTime = DateTime.MinValue;
                                        }

                                        VersionEntry versionEntry = VersionEntry.StoreEntry(release["name"].ToString(), release["tag_name"].ToString(), release["body"].ToString(), publishTime, (bool)release["prerelease"]);
                                        foreach (var asset in release["assets"])
                                        {
                                            if (asset["content_type"].ToString() == mimetype || mimetype == "*")
                                            {
                                                versionEntry.RegisterAsset(VersionEntry.VersionAsset.RegisterAssets(
                                                    asset["name"].ToString(),
                                                    Convert.ToInt64(asset["size"].ToString()),
                                                    asset["browser_download_url"].ToString(),
                                                    asset["digest"]?.ToString() ?? "N/A"
                                                    ));
                                            }
                                        }
                                        versions.Add(versionEntry);
                                        CheckUpdateReport?.Invoke($"Fetched version {versionEntry.VersionSequence}");
                                    }
                                    VersionEntry stableVersion = versions.Where(v => !v.IsPreRelease).Where(v => v.VersionInfo.CompareTo(currentAppVersion) == 1).OrderByDescending(v => v.VersionInfo).FirstOrDefault();

                                    VersionEntry prereleaseVersion = versions.Where(v => v.IsPreRelease).Where(v => v.VersionInfo.CompareTo(currentAppVersion) == 1).Where(v => v.VersionInfo.CompareTo(stableVersion?.VersionInfo ?? currentAppVersion) == 1).OrderByDescending(v => v.VersionInfo).FirstOrDefault();

                                    if (stableVersion != null || prereleaseVersion != null)
                                    {
                                        CheckUpdateReport?.Invoke($"{(stableVersion != null ? "Stable" : "")}{(stableVersion != null && prereleaseVersion != null ? " and " : "")}{(prereleaseVersion != null ? "Pre-Release" : "")} version{(stableVersion != null && prereleaseVersion != null ? "s" : "")} are available for download.");
                                    }
                                    else
                                    {
                                        CheckUpdateReport?.Invoke("No updates available.");
                                    }
                                    CheckUpdateReportReady?.Invoke();
                                }
                                else
                                {
                                    CheckUpdateReport?.Invoke($"Cannot get to Github Release Server! Redirect Detected at [{response.RequestMessage.RequestUri.ToString()}] [Status Code {response.StatusCode}]");
                                    if (interruptIfFail)
                                    {
                                        CheckUpdateReportReady?.Invoke();
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                CheckUpdateReport?.Invoke($"Failed to fetch updates! {response.ToString()} [Status Code {response.StatusCode}]");
                                if (interruptIfFail)
                                {
                                    CheckUpdateReportReady?.Invoke();
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        CheckUpdateReport?.Invoke($"Failed to fetch version releases. The following error(s) has occured! [{err.Message}]");
                        if (interruptIfFail)
                        {
                            CheckUpdateReportReady?.Invoke();
                            break;
                        }
                    }
                    break;
                    #endregion
                } while (true);
            });
            updateThread.Name = $"Update Checker";
            updateThread.IsBackground = true;
            updateThread.Start();
        }

        public async void BeginDownload(string downloadUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    if (!string.IsNullOrEmpty(tokenID))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenID);
                    }

                    using (HttpResponseMessage response = await client.GetAsync(downloadUrl))
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