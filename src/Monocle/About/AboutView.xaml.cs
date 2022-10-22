using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dynamo.Controls;
using Dynamo.Views;
using MonocleViewExtension.Utilities;

namespace MonocleViewExtension.About
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : Window
    {
        public AboutView()
        {
            InitializeComponent();

            string MonocleOnlineVerInfo = "";
            string MonocleHighestWebVer = "";

            Version MonocleGitRepoVersion;
            Version MonoclePackWebVersion;

            string MonocleOnlineVersionsCompared = "";
            string MonocleGitRepoCompared = "";
            string MonoclePackWebCompared = "";

            string MonocleGithubVerStartText = "Github Version: ";
            string MonoclePackageVerStartText = "Package Manager Version: ";


            Version AssemblyVerInfo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            AssemblyVerInfo = Version.Parse(AssemblyVerInfo.Major + "." + AssemblyVerInfo.Minor + "." + AssemblyVerInfo.Build);

            this.MonocleInstalledVerText.Text = System.Environment.NewLine + "Installed Version: " + AssemblyVerInfo.ToString();

            string MonocleGitRepoInfo = VersionCheckerUtils.MonocleGitHubRepoVerion();
            string MonoclePackWebInfo = VersionCheckerUtils.MonoclePackageWebsiteVerion();


            if (MonocleGitRepoInfo != "Cannot Access Webpage" && MonoclePackWebInfo != "Cannot Access Webpage")
            {
                MonoclePackWebCompared = VersionCheckerUtils.AssembleyVersionComparer(AssemblyVerInfo, MonoclePackWebInfo);
                MonocleGitRepoCompared = VersionCheckerUtils.AssembleyVersionComparer(AssemblyVerInfo,MonocleGitRepoInfo);
                MonocleOnlineVersionsCompared = VersionCheckerUtils.OnlineVersionComparer(MonocleGitRepoInfo, MonoclePackWebInfo);
            }
            else if (MonocleGitRepoInfo == "Cannot Access Webpage" && MonoclePackWebInfo != "Cannot Access Webpage")
            {
                MonoclePackWebCompared = VersionCheckerUtils.AssembleyVersionComparer(AssemblyVerInfo, MonoclePackWebInfo);
            }
            else if(MonoclePackWebInfo == "Cannot Access Webpage" && MonocleGitRepoInfo != "Cannot Access Webpage")
            {
                MonocleGitRepoCompared = VersionCheckerUtils.AssembleyVersionComparer(AssemblyVerInfo, MonocleGitRepoInfo);
            }


            /// TODO Will need further Checking/comparison

            if (MonocleGitRepoCompared == "" && MonoclePackWebCompared == "")
            {
                this.MonocleGithubVerText.Text = MonocleGithubVerStartText + "N/A";
                this.MonoclePackageVerText.Text = MonoclePackageVerStartText + "N/A";
                this.MonocleVerText.Text = "Currently cannot reach either website to check" + System.Environment.NewLine + "if there is a revised version of the extension.";


            }
            else if (MonocleOnlineVersionsCompared != "Both Github and Package Website have the same version" && MonocleGitRepoCompared == "Your Installed version of Monocle is up to date." && MonoclePackWebCompared == "Your Installed version of Monocle is up to date.")
            {
                this.MonocleGithubVerText.Text = MonocleGithubVerStartText + MonocleGitRepoInfo;
                this.MonoclePackageVerText.Text = MonoclePackageVerStartText + MonoclePackWebInfo;

                this.MonocleVerText.Text = "";

            }
            else if (MonocleOnlineVersionsCompared == "Package Website has the latest version" || MonocleOnlineVersionsCompared == "Github Website has the latest version" && MonoclePackWebCompared != "" && MonocleGitRepoCompared != "")
            {
                this.MonocleGithubVerText.Text = MonocleGithubVerStartText + MonocleGitRepoInfo;
                this.MonoclePackageVerText.Text = MonoclePackageVerStartText + MonoclePackWebInfo;
                this.MonocleVerText.Text = MonocleOnlineVersionsCompared;

            }
            else if (MonocleOnlineVersionsCompared != "" && MonocleGitRepoCompared == "Your version of Monocle is out of date and needs updating" && MonoclePackWebCompared == "Your version of Monocle is out of date and needs updating")
            {
                this.MonocleGithubVerText.Text = MonocleGithubVerStartText + MonocleGitRepoInfo;
                this.MonoclePackageVerText.Text = MonoclePackageVerStartText + MonoclePackWebInfo;
                this.MonocleVerText.Text = MonocleOnlineVersionsCompared;

            }
            else if (MonocleGitRepoCompared == "" && MonoclePackWebCompared == "Your Installed version of Monocle is up to date.")
            {

                this.MonocleGithubVerText.Text = MonocleGithubVerStartText + "N/A" ;
                this.MonoclePackageVerText.Text = MonoclePackageVerStartText + MonoclePackWebInfo;

                this.MonocleVerText.Text = "Github Repository Website Could not be Reach";
            }
            else if (MonocleGitRepoCompared == "Your Installed version of Monocle is up to date." && MonoclePackWebCompared == "")
            {
                this.MonocleGithubVerText.Text = MonocleGithubVerStartText + MonocleGitRepoInfo;
                this.MonoclePackageVerText.Text = MonoclePackageVerStartText + "N/A";

                this.MonocleVerText.Text = "Package Manager Website Could not be Reach";

            }
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

    }
}
