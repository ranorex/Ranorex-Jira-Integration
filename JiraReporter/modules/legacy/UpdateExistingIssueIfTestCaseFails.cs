
using System;
using System.Collections.Generic;

using Ranorex;
using Ranorex.Core.Testing;

namespace JiraReporter
{

  [TestModule("7F4E25E4-174E-4E61-B291-EE7882426969", ModuleType.UserCode, 1)]
  public class UpdateExistingIssueIfTestCaseFails : ITestModule
  {

    public UpdateExistingIssueIfTestCaseFails()
    {
      // Do not delete - a parameterless constructor is required!
    }

    string _JiraSummary = "";
    [TestVariable("A17CCC90-3800-4B66-89F6-33025E95CD83")]
    public string JiraSummary
    {
      get { return _JiraSummary; }
      set { _JiraSummary = value; }
    }

    string _JiraDescription = "";
    [TestVariable("2CBA8E0D-CE84-447D-80D2-4044B4CC4735")]
    public string JiraDescription
    {
      get { return _JiraDescription; }
      set { _JiraDescription = value; }
    }

    string _JiraIssueKey = "";
    [TestVariable("861CFAE9-60E6-409F-99D0-1BED4082BB01")]
    public string JiraIssueKey
    {
      get { return _JiraIssueKey; }
      set { _JiraIssueKey = value; }
    }

    string _JiraLabels = "";
    [TestVariable("1BFCEF2A-F7F4-484C-ADE6-18F6ABC8DFE2")]
    public string JiraLabels
    {
      get { return _JiraLabels; }
      set { _JiraLabels = value; }
    }


    void ITestModule.Run()
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      if (!config.enabled)
      {
        Report.Debug("Jira integration disabled in config!");
        return;
      }


      var tc = TestSuite.CurrentTestContainer;
      if (tc == null)
      {
        Report.Error("TestCase is 'null'; this usually happens when the module is used outside of testcases (e.g., global teardown).");
      }

      if (tc.Status == Ranorex.Core.Reporting.ActivityStatus.Failed)
      {
        try
        {
          char delimiterChar = ';';
          List<string> labels = null;

          if (!string.IsNullOrEmpty(JiraLabels))
            labels = new List<string>(JiraLabels.Split(delimiterChar));

          var curIssue = JiraReporter.UpdateIssue(JiraIssueKey, tc.Name, JiraSummary, JiraDescription, labels, true);

          Report.Info("Jira issue updated -- IssueKey: " + curIssue.Key + "; IssueID: " + curIssue.Id);
          Report.LogHtml(ReportLevel.Info, "<a href=\"" + JiraReporter.ServerURL + "/browse/" + curIssue.Id + "\">" + curIssue.Id + "</a>");
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
}
