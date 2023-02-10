using EasySave.src.Models.Exceptions;
using Octokit;
using System;
using System.Reflection;

namespace EasySave.src.Utils
{
    public static class VersionUtils
    {

        public static string GetVersionFromGit()
        {
            try
            {
                var client = new GitHubClient(new ProductHeaderValue("EasySave"));
                return client.Repository.Release.GetAll("arnoux23u-CESI", "EasySave").GetAwaiter().GetResult()[0].TagName;
            }
            catch
            {
                throw new CantCheckUpdateException();
            }
        }

        public static int[] VersionFromStr(string version)
        {
            if (version.StartsWith("V"))
                version = version[1..];
            var versionParts = version.Split('.');
            return new int[] { int.Parse(versionParts[0]), int.Parse(versionParts[1]), (versionParts.Length > 2 ? int.Parse(versionParts[2]) : 0) };
        }

        public static bool CompareVersions()
        {
            Version versionData = Assembly.GetExecutingAssembly().GetName().Version;
            int[] current = VersionFromStr($"V{versionData.Major}.{versionData.Minor}.{versionData.Build}");
            int[] latest = VersionFromStr(GetVersionFromGit());
            return (latest[0] > current[0]) || (latest[0] == current[0] && latest[1] > current[1]) || (latest[0] == current[0] && latest[1] == current[1] && latest[2] > current[2]);
        }

    }
}
