using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MonocleViewExtension.Utilities
{
    public static class MiscUtils
    {
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    public static class StringUtils
    {
        public static string SetCustomNodeNotePrefix(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                return "Custom Node: ";
            }

            return !Char.IsWhiteSpace(prefix[prefix.Length - 1]) ? $"{prefix} " : prefix;
        }

        public static string SimplifyString(this string str)
        {
            return str.ToLower().Replace(" ", "").Replace(".", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            // Step 7
            return d[n, m];
        }
        public static string CleanupString(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().ToLower();
        }
    }
    public static class Compatibility
    {
        /// <summary>
        /// Check if DevExpress is loaded (typically for KiwiCodes)
        /// </summary>
        public static void CheckForDevExpress()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            try
            {
                Globals.DevExpress = assemblies.First(a => a.FullName.Contains("DevExpress.Xpf.Core"));
            }
            catch (Exception)
            {
                Globals.DevExpress = null;
            }

            Globals.IsDevExpressLoaded = Globals.DevExpress != null;
        }

        /// <summary>
        /// This allows us to fix our UI when a user has KiwiCodes Family Browser loaded
        /// </summary>
        /// <param name="window"></param>
        public static void FixThemesForDevExpress(Window window)
        {
            if (Globals.DevExpress is null) return;

            try
            {
                var objType = Enumerable.First<Type>(GetTypesSafely(Globals.DevExpress),
                    t => t.Name.Equals("ThemeManager"));

                object baseObject = System.Runtime.Serialization.FormatterServices
                    .GetUninitializedObject(objType);

                objType.InvokeMember("SetThemeName",
                    BindingFlags.Default | BindingFlags.InvokeMethod, null, baseObject, new object[] { window, "None" });
            }
            catch (Exception)
            {
                //do nothing
            }
        }

        private static IEnumerable<Type> GetTypesSafely(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x != null);
            }
        }
    }

    internal static class VersionCheckerUtils
    {
        ///<summary>
        //Checks github repository for latest release version and returns the version number
        /// </summary>
        public static string MonocleGitHubRepoVerion()
        {
            const string MonocleRepoAddress = "https://api.github.com/repos/johnpierson/MonocleForDynamo/releases/latest";
            string MonocleStreamOutput;

            try
            {
                HttpWebRequest siteRequest = (HttpWebRequest)System.Net.WebRequest.Create(MonocleRepoAddress);
                siteRequest.Method = WebRequestMethods.Http.Get;
                siteRequest.Accept = "application/json";
                siteRequest.ContentType = "application/json; charset=utf-8";
                siteRequest.UserAgent = "(Anything other than an empty string)";

                var siteResponse = siteRequest.GetResponse();

                using (var sr = new StreamReader(siteResponse.GetResponseStream()))
                {
                    MonocleStreamOutput = sr.ReadToEnd();
                }

                dynamic JsonOutput = JsonConvert.DeserializeObject(MonocleStreamOutput);

                return JsonOutput.tag_name.Value;
            }
            catch
            {
                return "Cannot Access Webpage";
            }
        }

        ///<summary>
        //Checks Dynamo Package Website for latest release version and returns the version number
        /// </summary>
        public static string MonoclePackageWebsiteVerion()
        {
            const string MonoclePackageAddress = "https://dynamopackages.com/package/5bb7c639452aacde53000059/";
            string MonocleStreamOutput;

            try
            {
                HttpWebRequest siteRequest = (HttpWebRequest)System.Net.WebRequest.Create(MonoclePackageAddress);
                siteRequest.Method = WebRequestMethods.Http.Get;
                siteRequest.Accept = "application/json";
                siteRequest.ContentType = "application/json; charset=utf-8";

                var siteResponse = siteRequest.GetResponse();

                using (var sr = new StreamReader(siteResponse.GetResponseStream()))
                {
                    MonocleStreamOutput = sr.ReadToEnd();
                }

                dynamic JsonOutput = JsonConvert.DeserializeObject(MonocleStreamOutput);

                string packwebsiteVerionOutput = "";
                string jsonContentLatestVersion = JsonOutput.content.latest_version_update.Value.ToString();

                foreach (var jsonVersions in JsonOutput.content.versions)
                {
                    if (jsonVersions.created.Value.ToString() == jsonContentLatestVersion)
                    {
                        packwebsiteVerionOutput = jsonVersions.version.Value;
                    }
                }
                return packwebsiteVerionOutput;
            }
            catch
            {
                return "Cannot Access Webpage";
            }
        }



        public static string AssembleyVersionComparer(Version assemblyVer, string StringVerToCompare)
        {
            Version compareVersion = Version.Parse(StringVerToCompare);

            int comparedOutput = assemblyVer.CompareTo(compareVersion);

            if (comparedOutput < 0)
            {
                /// Assembly is less than released version
                /// 

                return "Your version of Monocle is out of date and needs updating";

            }
            else if (comparedOutput > 0)
            {
                /// Assembly is Greater than released version
                return "Ohhh...Your a lucky one getting Monocle early.";
            }
            else
            {
                /// Assembly is equal to released version

                return "Your Installed version of Monocle is up to date.";
            }

        }

        public static string OnlineVersionComparer(string GitHubVersion, string PackageWebsiteVersion)
        {
            Version version1 = Version.Parse(GitHubVersion);

            Version version2 = Version.Parse(PackageWebsiteVersion);

            int comparedOutput = version1.CompareTo(version2);

            if (comparedOutput < 0)
            {
                /// GitHubVersion is less than PackageWebsiteVersion
                /// 

                return "Package Website has the latest version";

            }
            else if (comparedOutput > 0)
            {
                /// GitHubVersion is Greater than PackageWebsiteVersion
                return "Github Website has the latest version";
            }
            else
            {
                /// GitHubVersion is equal to PackageWebsiteVersion

                return "Both Github and Package Website have the same version";
            }

        }

    }
}
