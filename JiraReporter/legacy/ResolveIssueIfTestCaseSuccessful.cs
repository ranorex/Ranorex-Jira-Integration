/*
 * Created by Ranorex
 * User: cbreit
 * Date: 23.10.2014
 * Time: 12:24
 * 
 * Acknowledgement:
 * This product includes software developed by TechTalk.
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using Ranorex;
using Ranorex.Core.Testing;

namespace JiraReporter
{
  /// <summary>
  /// Description of CloseIssueIfTestCaseSuccessful.
  /// </summary>
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
    /// <summary>
    /// Constructs a new instance.
    /// </summary>
    public ResolveIssueIfTestCaseSuccessful()
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
      var tc = checkTestCase();

      if (tc.Status == Ranorex.Core.Reporting.ActivityStatus.Success)
      {

        resolveIssue(JiraIssueKey, TransitionName);
      }
    }
  }
}
