using Ranorex;
using Ranorex.Core.Testing;
using System;

namespace JiraReporter
{
  public abstract class AbstractJiraIntegrationClient
  {
    protected ITestContainer checkTestCase()
    {
      var tc = TestSuite.CurrentTestContainer;

      if (tc == null)
      {
        Report.Error("TestCase is 'null'; this usually happens when the module is used outside of testcases (e.g., global teardown).");
      }

      return tc;
    }

    protected void createIssue(ITestContainer tc)
    {
      try
      {
        var createdIssue = JiraReporter.CreateIssue(tc.Name, true);

        Report.Info("Jira issue created -- IssueKey: " + createdIssue.Key + "; IssueID: " + createdIssue.Id);
        Report.LogHtml(ReportLevel.Info, "<a href=\"" + JiraReporter.ServerURL + "browse/" + createdIssue.Id + "\">" + createdIssue.Id + "</a>");
      }
      catch (Exception e)
      {
        var inner = e.InnerException;
        string str = "";
        if (inner != null)
        {
          var prop = inner.GetType().GetProperty("ErrorResponse");
          if (prop != null)
            str = (string)prop.GetValue(e.InnerException, null);
        }

        Report.Error(e.Message + " (InnerException: " + e.InnerException + " -- " + str + ")");
      }
    }

    protected void reopenIssue(string JiraIssueKey, string TransitionName)
    {
      try
      {
        var curIssue = JiraReporter.ChangeState(JiraIssueKey, TransitionName, true);

        if (curIssue != null)
        {
          Report.Info("Jira issue reopened -- IssueKey: " + curIssue.Key + "; IssueID: " + curIssue.Id);
          Report.LogHtml(ReportLevel.Info, "<a href=\"" + JiraReporter.ServerURL + "browse/" + curIssue.Id + "\">" + curIssue.Id + "</a>");
        }
        else
          Report.Warn("Could not re-open Jira issue -- IssueKey: " + curIssue.Id);
      }
      catch (Exception e)
      {
        var inner = e.InnerException;
        string str = "";
        if (inner != null)
        {
          var prop = inner.GetType().GetProperty("ErrorResponse");
          if (prop != null)
            str = (string)prop.GetValue(e.InnerException, null);
        }

        Report.Error(e.Message + " (InnerException: " + e.InnerException + " -- " + str + ")");

      }
    }

    protected void resolveIssue(string JiraIssueKey, string TransitionName)
    {
      try
      {
        var curIssue = JiraReporter.ChangeState(JiraIssueKey, TransitionName, true);

        if (curIssue != null)
        {
          Report.Info("Jira issue resolved -- IssueKey: " + curIssue.Key + "; IssueID: " + curIssue.Id);
          Report.LogHtml(ReportLevel.Info, "<a href=\"" + JiraReporter.ServerURL + "browse/" + curIssue.Id + "\">" + curIssue.Id + "</a>");
        }
        else
          Report.Warn("Could not resolved Jira issue -- IssueKey: " + curIssue.Id);
      }
      catch (Exception e)
      {
        var inner = e.InnerException;
        string str = "";
        if (inner != null)
        {
          var prop = inner.GetType().GetProperty("ErrorResponse");
          if (prop != null)
            str = (string)prop.GetValue(e.InnerException, null);
        }

        Report.Error(e.Message + " (InnerException: " + e.InnerException + " -- " + str + ")");
      }
    }
  }
}
