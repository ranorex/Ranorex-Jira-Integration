using Ranorex;
using Ranorex.Core.Testing;

namespace JiraReporter
{
  /// <summary>
  /// This module creates a new issue within jira if the test case fails.
  /// </summary>
  [TestModule("DE17F33E-345F-416F-9F58-505B5E34AAE3", ModuleType.UserCode, 1)]
  public class AutoCreateNewIssueIfTestCaseFails : AbstractJiraIntegrationClient, ITestModule
  {

    public AutoCreateNewIssueIfTestCaseFails()
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
      if (tc.Status == Ranorex.Core.Reporting.ActivityStatus.Failed)
      {
        string rxField = config.RxAutomationFieldName;
        string query = config.jqlQueryToConnectIssues;
        config.RxAutomationFieldName = "";
        config.jqlQueryToConnectIssues = "";

        AutoHandleJiraIntegration ah = new AutoHandleJiraIntegration();
        ah.Run();

        config.RxAutomationFieldName = rxField;
        config.jqlQueryToConnectIssues = query;
      }
    }
  }
}
