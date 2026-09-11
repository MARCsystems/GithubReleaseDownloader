using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GithubReleaseDownloader
{
    public class SemanticVersion
    {
        private int v_1 = 0;
        private int v_2 = 0;
        private int v_3 = 0;
        private int v_4_legacy = 0;
        private string semantic_revision = "";
        private string semantic_build = "";

        private bool isLegacy = false;

        private SemanticVersion(int major, int minor=0, int build=0, int revision=0)
        {
            this.v_1 = major;
            this.v_2 = minor;
            this.v_3 = build;
            this.v_4_legacy = revision;
            
            isLegacy = true;
        }

        private SemanticVersion(int major, int minor, int patch, string semantic_prerelease = "", string semantic_build = "")
        {
            ValidateVersionNumber(nameof(major), major);
            ValidateVersionNumber(nameof(minor), minor);
            ValidateVersionNumber(nameof(patch), patch);
            ValidatePatchString(nameof(semantic_prerelease), semantic_prerelease);
            ValidatePatchString(nameof(semantic_build), semantic_build);

            this.v_1 = major;
            this.v_2 = minor;
            this.v_3 = patch;
            this.semantic_revision = semantic_prerelease;
            this.semantic_build = semantic_build;

            isLegacy = false;
        }

        public static SemanticVersion Parse(string versionString, string prefix = "")
        {
            versionString = versionString.Substring(prefix.Length);

            int prereleaseIndex = versionString.IndexOf("-");
            int buildIndex = versionString.IndexOf("+");

            if (prereleaseIndex == -1) { prereleaseIndex = versionString.Length; }
            if (buildIndex == -1) { buildIndex = versionString.Length; }

            bool hasPrerelease = buildIndex > prereleaseIndex && prereleaseIndex != versionString.Length;
            bool hasbuild = buildIndex != versionString.Length;

            if (prereleaseIndex == -1 && buildIndex == -1)
            {
                int[] versionLegacy = versionString.Split('.').Select(v => Convert.ToInt32(v)).ToArray();
                return new SemanticVersion(
                    versionLegacy[0],
                    versionLegacy[1],
                    versionLegacy[2],
                    versionLegacy.Length == 4 ? versionLegacy[3] : 0);
            }
            else
            {
                int[] versionLegacy = versionString.Substring(0, Math.Min(prereleaseIndex, buildIndex)).Split('.').Select(v => Convert.ToInt32(v)).ToArray();
                string prerelease = hasPrerelease ? versionString.Substring(prereleaseIndex + 1, buildIndex - prereleaseIndex - 1) : "";
                string build = hasbuild ? versionString.Substring(buildIndex + 1, versionString.Length - buildIndex - 1) : "";

                return new SemanticVersion(
                    versionLegacy[0],
                    versionLegacy[1],
                    versionLegacy[2],
                    prerelease,
                    build);
            }
        }

        private void ValidateVersionNumber(string parameterName, int value)
        {
            if (value < 0)
            {
                throw new ArgumentException($"{parameterName} cannot be set to negative.");
            }
        }

        private void ValidatePatchString(string parameterName, string value, bool numericalWithLeadingZeros = false)
        {
            string[] identifiers = value.Split('.');

            // Bypass first element in case of no identifier for patch string
            if (identifiers.Length == 1)
            {
                if (identifiers[0].Length == 0)
                {
                    return;
                }
            }

            foreach (string identifier in identifiers)
            {
                if (identifier.Trim().Length == 0 && identifiers.Length > 1)
                {
                    throw new ArgumentException($"{parameterName} can not contain empty identifiers.");
                }

                if (numericalWithLeadingZeros && int.TryParse(identifier, out int numerical))
                {
                    if (identifier.StartsWith("0"))
                    {
                        throw new ArgumentException($"{parameterName} can not start with leading zeros on a purely numerical identifier. Value: {identifier}].");
                    }
                }
            }

            if (!Regex.IsMatch(value, @"^[0-9A-Za-z-]+$"))
            {
                throw new ArgumentException($"{value} does not match the expected version format for {parameterName}");
            }
        }

        public int CompareTo(SemanticVersion other)
        {
            // Compare Major
            if (this.v_1 > other.v_1) { return 1; }
            else if (this.v_1 < other.v_1) { return -1; }

            // Compare Minor
            if (this.v_2 > other.v_2) { return 1; }
            else if (this.v_2 < other.v_2) { return -1; }

            // Compare Legacy Build / Semantic Patch
            if(this.v_3>other.v_3) { return 1; }
            else if (this.v_3<other.v_3) { return -1; }

                // If nothing else to compare
                return 0;
        }
    }
}
