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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace JiraReporter
{
    /// <summary>
    /// Description of CloseIssueIfTestCaseSuccessful.
    /// </summary>
    [TestModule("B01C5C25-EC0B-48D9-AE60-EDF4904F289D", ModuleType.UserCode, 1)]
    public class ResolveIssueIfTestCaseSuccessful : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ResolveIssueIfTestCaseSuccessful()
        {
            // Do not delete - a parameterless constructor is required!
        }

        string _JiraIssueKey = "";
        [TestVariable("763880E1-C9E1-4CCE-B626-898390004888")]
        public string JiraIssueKey
        {
          get { return _JiraIssueKey; }
          set { _JiraIssueKey = value; }
        }
        
        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
          var tc = TestCase.Current;
          if(tc.Status == Ranorex.Core.Reporting.ActivityStatus.Success)
          {
            try
            {
              var curIssue = JiraReporter.ResolveIssue(JiraIssueKey, true);

              if (curIssue != null)
              {
                Report.Info("Jira issue resolved -- IssueKey: " + curIssue.Key + "; IssueID: " + curIssue.Id);
                Report.LogHtml(ReportLevel.Info, "<a href=\"" + JiraReporter.ServerURL + "/browse/" + curIssue.Key + "\">" + curIssue.Key + "</a>");
              }
              else
                Report.Warn("Could not resolved Jira issue -- IssueKey: " + curIssue.Key);
            }
            catch(Exception e)
            {
              Report.Error(e.Message + " (InnerException: " + e.InnerException + ")");
            }
            
          }
          
        }
    }
}
