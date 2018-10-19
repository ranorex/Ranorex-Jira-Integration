using Ranorex;
using Ranorex.Core.Testing;

namespace JiraReporter
{

  [TestModule("C3E1D3D1-C336-48FE-B857-C3D43B59D91A", ModuleType.UserCode, 1)]
  public class ReOpenExistingIssueIfTestCaseFails : AbstractJiraIntegrationClient, ITestModule
  {

    public ReOpenExistingIssueIfTestCaseFails()
    {
      // Do not delete - a parameterless constructor is required!
    }

    string _JiraIssueKey = "";
    [TestVariable("30890A5D-0962-4C63-B96C-6A2A4094636C")]
    public string JiraIssueKey
    {
      get { return _JiraIssueKey; }
      set { _JiraIssueKey = value; }
    }

    string _TransitionName = "";
    [TestVariable("C4F14A65-DB66-4381-BABD-921E14077BE6")]
    public string TransitionName
    {
      get { return _TransitionName; }
      set { _TransitionName = value; }
    }

    public void Run()
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      if (!config.enabled)
      {
        Report.Debug("Jira integration disabled in config!");
        return;
      }

      var tc = checkTestCase();

      if (tc.Status == Ranorex.Core.Reporting.ActivityStatus.Failed)
      {
        reopenIssue(JiraIssueKey, TransitionName);
      }
    }
  }
}
