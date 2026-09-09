using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GithubReleaseDownloader
{
    public class Updater
    {
        #region Events
        public event Action<string> CheckUpdateReport;
        public event Action<bool> CheckUpdateReportReady;
        public event Action<string, string, double> ReportDownloadPercentage;
        public event Action<string> GlobalMessage;
        public event Action<bool, string> DownloadReport;
        public event Action DownloadEventStopped;
        #endregion

        #region Private Variables
        private string repositoryOwner = "";
        private string repositoryName = "";
        private Version currentAppVersion = new Version(0,0,0,0);
        private string updateFilePath = "";
        private string pat_Token = "";
        private string pem_Data = "";
        private string pem_AppID = "";
        private string pem_InstallationID = "";
        private string mimeType = "*";
        private string applicationName = "";
        private ReleaseMode releaseMode = ReleaseMode.PUBLIC;
        private double downloadSizeLimit = 32;

        private List<VersionEntry> versions = new List<VersionEntry>();
        #endregion

        #region Private Functions
        private string GetReleaseUrl()
        {
            return $"https://api.github.com/repos/{repositoryOwner}/{repositoryName}/releases";
        }

        private async Task DownloadAsync(string link, string fileName)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    SetAuthorization(client);
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("MSYS-GRD/1.0");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));


                    using (HttpClientHandler handler = new HttpClientHandler())
                    {
                        using (HttpClient clientHandle = new HttpClient(handler))
                        {
                            GlobalMessage?.Invoke("Requesting Download...");
                            client.Timeout = TimeSpan.FromSeconds(10);

                            using (HttpResponseMessage response = await client.GetAsync(link))
                            {
                                if (response.IsSuccessStatusCode)
                                { 
                                    int bufferSize = Convert.ToInt32(downloadSizeLimit * 1024.0);

                                    long totalBytes = response.Content.Headers.ContentLength ?? -1;
                                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(), fileStream = new FileStream(Path.Combine(updateFilePath, fileName), FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, useAsync: true))
                                    {
                                        byte[] buffer = new byte[bufferSize];
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
                                                Thread.Sleep(1);
                                            }
                                            else
                                            {
                                                ReportDownloadPercentage?.Invoke(UnitCollapser.CollapseBytes(totalRead), "-1", -1);
                                            }
                                        }
                                    }
                                    DownloadReport?.Invoke(true, $"Download Completed. [{UnitCollapser.CollapseBytes(totalBytes)}]");
                                    DownloadEventStopped?.Invoke();
                                }
                                else
                                {
                                    DownloadReport?.Invoke(false, $"Download Failed! [Status Code {(int)response.StatusCode}, {response.ReasonPhrase}]");
                                    DownloadEventStopped?.Invoke();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    DownloadReport?.Invoke(false, $"Download failed due to the following error(s): \r\n\r\n{ex.Message}\r\n\r\n{ex.StackTrace}");
                    DownloadEventStopped?.Invoke();
                }
            }
        }

        private void SetAuthorization(HttpClient client)
        {
            if (releaseMode == ReleaseMode.PRIVATE_PAT)
            {
                GlobalMessage?.Invoke("Authenticating Personal Access Token (PAT)...");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", pat_Token);
            }
            else if (releaseMode == ReleaseMode.PRIVATE_PEM)
            {
                GlobalMessage?.Invoke("Authenticating Public Key Encrypted Module (PEM)...");
                RSA rsaToken = PemProcessor.PrepareRsaToken(pem_Data);
                GlobalMessage?.Invoke("Authenticating Github App - Application ID...");
                string jwtValidation = PemProcessor.CreateJwt(rsaToken, pem_AppID);
                GlobalMessage?.Invoke("Authenticating Github App - Installation ID...");
                string installToken = PemProcessor.GetInstallationToken(ApplicationName, jwtValidation, pem_InstallationID).GetAwaiter().GetResult();
                GlobalMessage?.Invoke("Finalizing Authentication Cycle...");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", installToken);
            }
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// The owner name of the repository.
        /// </summary>
        public string RepositoryOwner
        {
            set { repositoryOwner = value; }
            get { return repositoryOwner; }
        }

        /// <summary>
        /// The name of the repository.
        /// </summary>
        public string RepositoryName
        {
            set { repositoryName = value; }
            get { return repositoryName; }
        }

        /// <summary>
        /// The version of the application. Must come from Assembly.GetExecutingAssembly().GetName().Version
        /// </summary>
        public Version CurrentAppVersion
        {
            set { currentAppVersion = value; }
            get { return currentAppVersion; }
        }

        /// <summary>
        /// The folder path where installation gets saved.
        /// </summary>
        public string UpdateFileSavePath
        {
            set { updateFilePath = value; }
            get { return updateFilePath; }
        }

        /// <summary>
        /// Personal Access Token issued by Github. Legacy and Fine-Grained both accepted.
        /// </summary>
        public string PAT_Token
        {
            set { pat_Token = value; }
        }

        /// <summary>
        /// Register the PEM Credentials using read data. Recommended for deployed custom shareable installation key that contains the PEM Data, Application ID and Installation ID in one file.
        /// </summary>
        /// <param name="pem_Data"></param>
        /// <param name="pem_AppID"></param>
        /// <param name="pem_InstallationID"></param>
        public void RegisterPEM_FromData(string pem_Data, string pem_AppID, string pem_InstallationID)
        {
            this.pem_Data = pem_Data;
            this.pem_AppID = pem_AppID;
            this.pem_InstallationID = pem_InstallationID;
        }

        /// <summary>
        /// Register the PEM Credentials using the PEM file, the Appliication ID, and Installation ID.
        /// </summary>
        /// <param name="pem_FilePath"></param>
        /// <param name="pem_AppID"></param>
        /// <param name="pem_InstallationID"></param>
        public void RegisterPEM_FromFile(string pem_FilePath, string pem_AppID, string pem_InstallationID)
        {
            string pem_Data = "";
            if (File.Exists(pem_FilePath))
            {
                pem_Data=File.ReadAllText(pem_FilePath);
            }
            RegisterPEM_FromData(pem_Data, pem_AppID, pem_InstallationID);
        }

        /// <summary>
        /// The Github File Mimetype. Assign "*" to get all filetypes.
        /// </summary>
        public string MimeType
        {
            set { mimeType = value; }
            get { return mimeType; }
        }

        /// <summary>
        /// THe release mode. PUBLIC does not require PAT or PEM properties filled up.
        /// </summary>
        public ReleaseMode RepoReleaseMode
        {
            set { releaseMode = value; }
            get { return releaseMode; }
        }

        /// <summary>
        /// The application name. Will reflect with the API Request. Assignment not required.
        /// </summary>
        public string ApplicationName
        {
            set { applicationName = value; }
            get { return applicationName.Trim().Length == 0 ? "My Github App Downloader" : applicationName; }
        }

        /// <summary>
        /// Get all fetched versions via CheckUpdate
        /// </summary>
        public VersionEntry[] FetchedVersions
        {
            get
            {
                return versions.ToArray();
            }
        }

        /// <summary>
        /// Get only the latest stable release
        /// </summary>
        public VersionEntry LatestStable
        {
            get
            {
                return versions.Where(v => !v.IsPreRelease).Where(v => v.VersionInfo.CompareTo(CurrentAppVersion) == 1).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get only the latest pre-release above the stable release
        /// </summary>
        public VersionEntry LatestPreRelease
        {
            get
            {
                return versions.Where(v => v.IsPreRelease).Where(v => v.VersionInfo.CompareTo(CurrentAppVersion) == 1).Where(v => v.VersionInfo.CompareTo(LatestStable?.VersionInfo ?? currentAppVersion) == 1).FirstOrDefault();
            }
        }

        /// <summary>
        /// Download Limit in Kibibyte per millisecond (1024 bytes = 1 Kibibyte)
        /// Default Value: 32KiB
        /// </summary>
        public double DownloadSizeLimit
        {
            set { downloadSizeLimit = value; }
        }
        #endregion

        #region Query, Download and Install
        /// <summary>
        /// Get all updates above the given version.
        /// IMPORTANT: Please assign data to the following properties first:
        /// * RepositoryOwner = The name of the repository owner
        /// * RepositoryName = The name of the repository
        /// * CurrentAppVersion = The version of the application. Recommended reference is from Assembly.GetExecutingAssembly().GetName().Version
        /// 
        /// For Private Repositories, please initialize the following:
        /// If using Personal Access Token
        /// * PAT_Token = The Personal Access Token issued by Github.
        /// 
        /// If using Github Apps
        /// * PEM_FilePath = The file path of the downloaded from Github Apps.
        /// * PEM_Data = The data from PEM_FilePath. An alternative to PEM_FilePath. DO NOT HARDCODE YOUR PEM TO THE APP.
        /// * PEM_
        /// </summary>
        /// <param name="interruptIfFail">Breaks the update checking if checking failed.</param>
        public void CheckForUpdates(bool interruptIfFail = false)
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
                            SetAuthorization(client);
                            client.DefaultRequestHeaders.UserAgent.ParseAdd("MSYS_GRD/1.0");

                            GlobalMessage?.Invoke("Initializing Update Check Request...");

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
                                            if (asset["content_type"].ToString().Equals(mimeType, StringComparison.OrdinalIgnoreCase) || mimeType.Equals("*", StringComparison.OrdinalIgnoreCase))
                                            {
                                                versionEntry.RegisterAsset(VersionEntry.VersionAsset.RegisterAssets(
                                                    asset["name"]?.ToString()??"N/A",
                                                    Convert.ToInt64(asset["size"].ToString()),
                                                    asset["url"]?.ToString()??"N/A",
                                                    asset["content_type"]?.ToString() ?? "N/A",
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
                                    CheckUpdateReportReady?.Invoke(versions.Count > 0);
                                }
                                else
                                {
                                    CheckUpdateReport?.Invoke($"Cannot get to Github Release Server! Redirect Detected at [{response.RequestMessage.RequestUri.ToString()}] [Status Code {response.StatusCode}]");
                                    if (interruptIfFail)
                                    {
                                        CheckUpdateReportReady?.Invoke(false);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                CheckUpdateReport?.Invoke($"Failed to fetch updates! [Status Code {(int)response.StatusCode}, {response.ReasonPhrase}]");
                                if (interruptIfFail)
                                {
                                    CheckUpdateReportReady?.Invoke(false);
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
                            CheckUpdateReportReady?.Invoke(false);
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

        /// <summary>
        /// Starts downloading the file
        /// </summary>
        /// <param name="link">The download link from the fetched assets.</param>
        /// <param name="fileName">The file name to be assigned. Preferrably should have the same extension as the original asset.</param>
        public void BeginDownload(string link, string fileName)
        {
            Thread downloadThread = new Thread(() =>
            {
                try
                {
                    DownloadAsync(link, fileName).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    DownloadReport?.Invoke(false, $"Download failed due to the following error(s): \r\n\r\n{ex.Message}\r\n\r\n{ex.StackTrace}");
                    DownloadEventStopped?.Invoke();
                }
            });

            downloadThread.Name = $"Download Activity";
            downloadThread.IsBackground = true;
            downloadThread.Start();
        }
        #endregion
    }
}