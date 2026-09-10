using System;
using System.Collections.Generic;

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
            private string contentType;
            private string[] assetHash;

            private VersionAsset(string assetName, long assetSize, string downloadUrl, string contentType, string assetHash)
            {
                this.assetName = assetName;
                this.assetSize = assetSize;
                this.downloadUrl = downloadUrl;
                this.contentType = contentType;
                this.assetHash = assetHash.Split(':');
            }

            internal static VersionAsset RegisterAssets(string assetName, long assetSize, string downloadUrl, string contentType, string assetHash)
            {
                return new VersionAsset(assetName, assetSize, downloadUrl, contentType, assetHash);
            }

            /// <summary>
            /// The name of the downloadable asset.
            /// </summary>
            public string AssetName
            {
                get { return assetName; }
            }

            /// <summary>
            /// The size of the asset determined by Github.
            /// </summary>
            public long AssetSize
            {
                get { return assetSize; }
            }

            /// <summary>
            /// Download API Url from Github.
            /// </summary>
            public string AssetDownloadUrl
            {
                get
                {
                    return downloadUrl;
                }
            }

            /// <summary>
            /// The content type determined by Github.
            /// </summary>
            public string ContentType
            {
                get
                {
                    return contentType;
                }
            }

            /// <summary>
            /// The size of the asset as a string, collapsed in smallest collapsible size.
            /// </summary>
            public string AssetSizeString
            {
                get
                {
                    return UnitCollapser.CollapseBytes(assetSize);
                }
            }

            /// <summary>
            /// The hash type.
            /// </summary>
            public string AssetHashType
            {
                get
                {
                    return assetHash[0];
                }
            }

            /// <summary>
            /// The hash value.
            /// </summary>
            public string AssetHashValue
            {
                get
                {
                    return assetHash[1];
                }
            }
        }

        internal static VersionEntry StoreEntry(string versionName, string versionSequence, string versionDescription, DateTime publishTime, bool isPreRelease)
        {
            return new VersionEntry(versionName, versionSequence, versionDescription, publishTime, isPreRelease);
        }

        /// <summary>
        /// The Release Version Name
        /// </summary>
        public string VersionName
        {
            get { return versionName; }
        }

        /// <summary>
        /// The Version Sequence from Github Release Tag.
        /// </summary>
        public string VersionSequence
        {
            get
            {
                return versionSequence;
            }
        }

        /// <summary>
        /// The version info
        /// </summary>
        public SemanticVersion SemanticVersionInfo
        {
            get
            {
                return SemanticVersion.Parse(versionSequence);
            }
        }

        /// <summary>
        /// The release's version description.
        /// </summary>
        public string VersionDescription
        {
            get
            {
                return versionDescription;
            }
        }

        /// <summary>
        /// The publish date of the release, reflected in Local Timezone.
        /// </summary>
        public DateTime PublishDate
        {
            get
            {
                return TimeZoneInfo.ConvertTimeFromUtc(publishTime, TimeZoneInfo.Local);
            }
        }
        
        /// <summary>
        /// Returns if the release is marked Stable or Pre-Release.
        /// </summary>
        public bool IsPreRelease
        {
            get
            {
                return isPreRelease;
            }
        }

        internal void RegisterAsset(VersionAsset ve)
        {
            versionAssets.Add(ve);
        }

        /// <summary>
        /// Assets within the release. Does not include the Source Code Assets.
        /// </summary>
        public VersionAsset[] AssetsInfo
        {
            get
            {
                return versionAssets.ToArray();
            }
        }
    }
}
