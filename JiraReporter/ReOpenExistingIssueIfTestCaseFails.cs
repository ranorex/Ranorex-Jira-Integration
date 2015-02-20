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
    /// Description of ReOpenExistingIssue.
    /// </summary>
    [TestModule("C3E1D3D1-C336-48FE-B857-C3D43B59D91A", ModuleType.UserCode, 1)]
    public class ReOpenExistingIssueIfTestCaseFails : ITestModule
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
        
        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
          var tc = TestCase.Current;
          if(tc.Status == Ranorex.Core.Reporting.ActivityStatus.Failed)
          {
            try
            {
              var curIssue = JiraReporter.ReOpenIssue(JiraIssueKey);

              if (curIssue != null)
              {
                Report.Info("Jira issue reopened -- IssueKey: " + curIssue.Key + "; IssueID: " + curIssue.Id);
                Report.LogHtml(ReportLevel.Info, "<a href=\"" + JiraReporter.ServerURL + "/browse/" + curIssue.Key + "\">" + curIssue.Key + "</a>");
              }
              else
                Report.Warn("Could not re-open Jira issue -- IssueKey: " + curIssue.Key);
            }
            catch(Exception e)
            {
              var inner = e.InnerException;
              string str = "";
              if(inner != null)
              {
                var prop = inner.GetType().GetProperty("ErrorResponse");
                if(prop != null)
                  str = (string)prop.GetValue(e.InnerException, null);
              }

              Report.Error(e.Message + " (InnerException: " + e.InnerException + " -- " + str + ")");
            
            }
          }
        }
    }
}
