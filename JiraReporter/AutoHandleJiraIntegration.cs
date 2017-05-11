using System.Collections;
using Ranorex;
using Ranorex.Core.Testing;
using Atlassian.Jira;

namespace JiraReporter {
    // <summary>
    /// Description of CreateNewIssueIfTestCaseFails.
    /// </summary>
    [TestModule("8B73DAD1-CE2B-4FF6-8618-571F7B31FC6D", ModuleType.UserCode, 1)]
    public class AutoHandleJiraIntegration : AbstractJiraIntegrationClient, ITestModule {
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

        string _StateClosed = "";
        [TestVariable("C4F14A65-DB66-4381-BABD-921E14077BE6")]
        public string StateClosed
        {
            get { return _StateClosed; }
            set { _StateClosed = value; }
        }

        string _StateReopen = "";
        [TestVariable("F6201ADA-3A5B-4879-B9E1-2D9C839B2736")]
        public string StateReopen
        {
            get { return _StateReopen; }
            set { _StateReopen = value; }
        }

        string _RxAutomationFieldName = "";
        [TestVariable("24AAC369-38AB-45EC-8A87-C79EC20CFD07")]
        public string RxAutomationFieldName
        {
            get { return _RxAutomationFieldName; }
            set { _RxAutomationFieldName = value; }
        }
        
        string _jqlQueryToConnectIssues = "";
        [TestVariable("0FA988AE-AAE9-40C4-9386-F51B13E8C96B7")]
        public string jqlQueryToConnectIssues
        {
            get { return _jqlQueryToConnectIssues; }
            set { _jqlQueryToConnectIssues = value; }
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public AutoHandleJiraIntegration()
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

            IEnumerable issues;
            if (jqlQueryToConnectIssues.Length > 0) {
            	issues = JiraReporter.getJiraIssues(jqlQueryToConnectIssues);
            } else {
            		issues = JiraReporter.getJiraIssues(RxAutomationFieldName + " ~ " + tc.Name);
            }

            if (tc.Status == Ranorex.Core.Reporting.ActivityStatus.Failed) {
                bool isEmpty = true;
                foreach (Issue issue in issues) {
                	if (!issue.Status.Name.Contains(StateReopen)) {
                		reopenIssue(issue.Key.ToString(), StateReopen);
                	} else {
                		Report.Info("Jira issue is already open -- IssueKey: " + issue.Key.ToString() + "; IssueID: " + issue.Key.ToString());
                    	Report.LogHtml(ReportLevel.Info, "<a href=\"" + JiraReporter.ServerURL + "/browse/" + issue.Key.ToString() + "\">" + issue.Key.ToString() + "</a>");
                	}
                    isEmpty = false;
                }
                if (isEmpty)
                {
                    createIssue(tc, JiraLabels, JiraSummary, JiraDescription, JiraIssueType, JiraProjectKey, RxAutomationFieldName);
                }
            } else if (tc.Status == Ranorex.Core.Reporting.ActivityStatus.Success)
            {
            	foreach (Issue issue in issues) {
            		if (!issue.Status.Name.Contains(StateClosed)) {
                    resolveIssue(issue.Key.ToString(), StateClosed);
            		}
                }
            }
        }
    }
}
