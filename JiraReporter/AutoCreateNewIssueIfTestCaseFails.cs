/*
 * Created by Ranorex
 * User: sknopper
 * Date: 14.04.2017
 * 
 * Acknowledgement:
 * This product includes software developed by TechTalk.
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using Ranorex.Core.Testing;

namespace JiraReporter
{
    /// <summary>
    /// Description of CreateNewIssueIfTestCaseFails.
    /// </summary>
    [TestModule("DE17F33E-345F-416F-9F58-505B5E34AAE3", ModuleType.UserCode, 1)]
    public class AutoCreateNewIssueIfTestCaseFails : AbstractJiraIntegrationClient, ITestModule
    {
        string _varJiraProjectKey = "";
        [TestVariable("2DC0BBB3-C44A-42B6-A695-9FB1ABFF8D84")]
        public string JiraProjectKey
        {
            get { return _varJiraProjectKey; }
            set { _varJiraProjectKey = value; }
        }

        string _varJiraIssueType = "Bug";
        [TestVariable("D9248964-3F28-4ABE-A5A8-324C190CF2F4")]
        public string JiraIssueType
        {
            get { return _varJiraIssueType; }
            set { _varJiraIssueType = value; }
        }

        string _varJiraSummary = "";
        [TestVariable("E16AC939-1D71-4936-8009-DE50E23325CA")]
        public string JiraSummary
        {
            get { return _varJiraSummary; }
            set { _varJiraSummary = value; }
        }

        string _varJiraDescription = "";
        [TestVariable("835F55D5-961E-4F62-AAE1-0AA992C725EF")]
        public string JiraDescription
        {
            get { return _varJiraDescription; }
            set { _varJiraDescription = value; }
        }

        string _varJiraLabels = "";
        [TestVariable("AB382EAB-9609-430A-B18E-46B19A493A5F")]
        public string JiraLabels
        {
            get { return _varJiraLabels; }
            set { _varJiraLabels = value; }
        }
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public AutoCreateNewIssueIfTestCaseFails()
        {
            // Do not delete - a parameterless constructor is required!
        }

        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        public void Run()
        {
            ITestContainer tc = checkTestCase();

            if (tc.Status == Ranorex.Core.Reporting.ActivityStatus.Failed)
          {
            createIssue(tc, JiraLabels, JiraSummary, JiraDescription, JiraIssueType, JiraProjectKey, null);
          } 
        }
    }
}
