using Ranorex;
using Ranorex.Core.Testing;

namespace JiraReporter
{

  [TestModule("B01C5C25-EC0B-48D9-AE60-EDF4904F289D", ModuleType.UserCode, 1)]
  public class ResolveIssueIfTestCaseSuccessful : AbstractJiraIntegrationClient, ITestModule
  {
    string _JiraIssueKey = "";
    [TestVariable("763880E1-C9E1-4CCE-B626-898390004888")]
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

    public ResolveIssueIfTestCaseSuccessful()
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

      var tc = checkTestCase();
      if (tc.Status == Ranorex.Core.Reporting.ActivityStatus.Success)
      {

        resolveIssue(JiraIssueKey, TransitionName);
      }
    }
  }
}
