using System.Collections;
using System.Linq;
using Atlassian.Jira;
using Ranorex;
using Ranorex.Core.Testing;

namespace JiraReporter {
    // <summary>
    /// Description of CreateNewIssueIfTestCaseFails.
    /// </summary>
    [TestModule("8B73DAD1-CE2B-4FF6-8618-571F7B31FC6D", ModuleType.UserCode, 1)]
    public class AutoHandleJiraIntegration : AbstractJiraIntegrationClient, ITestModule {

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
            JiraConfiguration config = JiraConfiguration.Instance;

            IEnumerable issues = null;
            if ((config.jqlQueryToConnectIssues == null || config.jqlQueryToConnectIssues.Length == 0)
                && (config.RxAutomationFieldName == null || config.RxAutomationFieldName.Length == 0)) {
            	issues = Enumerable.Empty<Issue>();
            } else if (config.jqlQueryToConnectIssues.Length > 0) {
            	issues = JiraReporter.getJiraIssues(config.jqlQueryToConnectIssues);
            } else {
            		issues = JiraReporter.getJiraIssues("'" + config.RxAutomationFieldName + "' ~ '" + tc.Name + "'");
            }

            if (tc.Status == Ranorex.Core.Reporting.ActivityStatus.Failed) {
                bool isEmpty = true;
                foreach (Issue issue in issues) {
                	if (!issue.Status.Name.Contains(config.StateReopen)) {
                		reopenIssue(issue.Key.ToString(), config.StateReopen);
                	} else {
                		Report.Info("Jira issue is already open -- IssueKey: " + issue.Key.ToString() + "; IssueID: " + issue.Key.ToString());
                    	Report.LogHtml(ReportLevel.Info, "<a href=\"" + JiraReporter.ServerURL + "browse/" + issue.Key.ToString() + "\">" + issue.Key.ToString() + "</a>");
                	}
                    isEmpty = false;
                }
                if (isEmpty)
                {
                    createIssue(tc);
                }
            } else if (tc.Status == Ranorex.Core.Reporting.ActivityStatus.Success)
            {
            	foreach (Issue issue in issues) {
            		if (!issue.Status.Name.Contains(config.StateClosed)) {
                    resolveIssue(issue.Key.ToString(), config.StateClosed);
            		}
                }
            }
        }
    }
}
