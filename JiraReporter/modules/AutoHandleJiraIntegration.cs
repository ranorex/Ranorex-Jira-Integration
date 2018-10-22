using System.Collections;
using System.Linq;
using Atlassian.Jira;
using Ranorex;
using Ranorex.Core.Testing;

namespace JiraReporter
{
  [TestModule("8B73DAD1-CE2B-4FF6-8618-571F7B31FC6D", ModuleType.UserCode, 1)]
  public class AutoHandleJiraIntegration : AbstractJiraIntegrationClient, ITestModule
  {

    public AutoHandleJiraIntegration()
    {
      // Do not delete - a parameterless constructor is required!
    }

    public void Run()
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      if (!config.enabled)
      {
        Report.Debug("Jira integration disabled in config!");
        return;
      }

      ITestContainer tc = checkTestCase();

      //Try to get issues associated with the test case
      IEnumerable issues = null;
      if ((config.jqlQueryToConnectIssues == null || config.jqlQueryToConnectIssues.Length == 0)
          && (config.RxAutomationFieldName == null || config.RxAutomationFieldName.Length == 0)
          && (config.transientConfig.JiraIssueKey == null || config.transientConfig.JiraIssueKey.Length == 0))
      {
        issues = Enumerable.Empty<Issue>();
      }
      else if (config.jqlQueryToConnectIssues.Length > 0)
        issues = JiraReporter.getJiraIssues(config.jqlQueryToConnectIssues);
      else if(config.transientConfig.JiraIssueKey.Length > 0)
        issues = JiraReporter.getJiraIssues("issue="  + config.transientConfig.JiraIssueKey);
      else 
        issues = JiraReporter.getJiraIssues("'" + config.RxAutomationFieldName + "' ~ '" + tc.Name + "'");

      // if no issues were found, create one; otherwise reopen existing ones
      if (tc.Status == Ranorex.Core.Reporting.ActivityStatus.Failed)
      {
        bool isEmpty = true;
        foreach (Issue issue in issues)
        {
          if (!issue.Status.Name.Contains(config.StateReopen))
            reopenIssue(issue.Key.ToString(), config.StateReopen);
          else
          {
            Report.Info("Jira issue is already open -- IssueKey: " + issue.Key.ToString() + "; IssueID: " + issue.Key.ToString());
            Report.LogHtml(ReportLevel.Info, "<a href=\"" + JiraReporter.ServerURL + "browse/" + issue.Key.ToString() + "\">" + issue.Key.ToString() + "</a>");
          }
          isEmpty = false;
        }
        if (isEmpty)
          createIssue(tc);
      }
      // otherwise, if the test case was successful, close the issues
      else if (tc.Status == Ranorex.Core.Reporting.ActivityStatus.Success)
      {
        foreach (Issue issue in issues)
          if (!issue.Status.Name.Contains(config.StateClosed))
            resolveIssue(issue.Key.ToString(), config.StateClosed);
      }

      config.transientConfig.Clear();
    }
  }
}
