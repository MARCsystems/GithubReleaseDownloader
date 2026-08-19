using System;
using System.Collections.Generic;
using System.Text;

namespace GithubReleaseDownloader
{
    public class VersionEntry
    {
        private string versionName;
        private string versionSequence;
        private string versionDescription;
        private List<VersionAsset> versionAssets = new List<VersionAsset>();
        private bool isPreRelease;
        private DateTime publishTime;

        private VersionEntry(string versionName, string versionSequence, string versionDescription, DateTime publishTime, bool isPreRelease)
        {
            this.versionName = versionName;
            this.versionSequence = versionSequence;
            this.versionDescription = versionDescription;
            this.publishTime = publishTime;
            this.isPreRelease = isPreRelease;
        }

        public class VersionAsset
        {
            private string assetName;
            private long assetSize;
            private string downloadUrl;
            private string[] assetHash;

            private VersionAsset(string assetName, long assetSize, string downloadUrl, string assetHash)
            {
                this.assetName = assetName;
                this.assetSize = assetSize;
                this.downloadUrl = downloadUrl;
                this.assetHash = assetHash.Split(':');
            }

            public static VersionAsset RegisterAssets(string assetName, long assetSize, string downloadUrl, string assetHash)
            {
                return new VersionAsset(assetName, assetSize, downloadUrl, assetHash);
            }

            public string AssetName
            {
                get { return assetName; }
            }

            public long AssetSize
            {
                get { return assetSize; }
            }

            public string AssetDownloadUrl
            {
                get
                {
                    return downloadUrl;
                }
            }

            public string AssetSizeString
            {
                get
                {
                    return UnitCollapser.CollapseBytes(assetSize);
                }
            }

            public string AssetHashType
            {
                get
                {
                    return $"{assetHash[0]}: {assetHash[1]}";
                }
            }
        }

        public static VersionEntry StoreEntry(string versionName, string versionSequence, string versionDescription, DateTime publishTime, bool isPreRelease)
        {
            return new VersionEntry(versionName, versionSequence, versionDescription, publishTime, isPreRelease);
        }

        public string VersionName
        {
            get { return versionName; }
        }

        public string VersionSequence
        {
            get
            {
                return versionSequence;
            }
        }

        public Version VersionInfo
        {
            get
            {
                return Version.Parse(versionSequence);
            }
        }

        public string VersionDescription
        {
            get
            {
                return versionDescription;
            }
        }

        public DateTime PublishDate
        {
            get
            {
                return TimeZoneInfo.ConvertTimeFromUtc(publishTime, TimeZoneInfo.Local);
            }
        }

        public bool IsPreRelease
        {
            get
            {
                return isPreRelease;
            }
        }

        public void RegisterAsset(VersionAsset ve)
        {
            versionAssets.Add(ve);
        }

        public VersionAsset[] AssetsInfo
        {
            get
            {
                return versionAssets.ToArray();
            }
        }
    }
}
