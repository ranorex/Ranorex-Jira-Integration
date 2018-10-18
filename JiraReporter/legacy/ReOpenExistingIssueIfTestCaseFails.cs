/*
 * Created by Ranorex
 * User: cbreit
 * Date: 23.10.2014
 * Time: 11:30
 * 
 * Acknowledgement:
 * This product includes software developed by TechTalk.
 * 
 */

using Ranorex.Core.Testing;

namespace JiraReporter
{
  /// <summary>
  /// Description of ReOpenExistingIssue.
  /// </summary>
  [TestModule("C3E1D3D1-C336-48FE-B857-C3D43B59D91A", ModuleType.UserCode, 1)]
  public class ReOpenExistingIssueIfTestCaseFails : AbstractJiraIntegrationClient, ITestModule
  {
    /// <summary>
    /// Constructs a new instance.
    /// </summary>
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

    /// <summary>
    /// Performs the playback of actions in this module.
    /// </summary>
    /// <remarks>You should not call this method directly, instead pass the module
    /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
    /// that will in turn invoke this method.</remarks>
    public void Run()
    {
      var tc = checkTestCase();

      if (tc.Status == Ranorex.Core.Reporting.ActivityStatus.Failed)
      {
        reopenIssue(JiraIssueKey, TransitionName);
      }
    }
  }
}
