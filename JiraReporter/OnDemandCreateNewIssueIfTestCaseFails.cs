/*
 * Created by Ranorex
 * User: cbreit
 * Date: 23.10.2014
 * Time: 15:34
 * 
 * Acknowledgement:
 * This product includes software developed by TechTalk.
 * 
 */
using System;

using Ranorex;
using Ranorex.Core.Testing;

using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace JiraReporter
{
    /// <summary>
    /// Description of OnDemandCreateNewIssueIfTestCaseFails.
    /// </summary>
    [TestModule("35D114D7-201E-4F8B-A52D-6C530C680A40", ModuleType.UserCode, 1)]
    public class OnDemandCreateNewIssueIfTestCaseFails : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public OnDemandCreateNewIssueIfTestCaseFails()
        {
            // Do not delete - a parameterless constructor is required!
        }

        string _JiraCLIFileLocation = "";
        [TestVariable("0057FD5D-5F39-44FA-99AA-7D38476179DF")]
        public string JiraCLIFileLocation
        {
          get { return _JiraCLIFileLocation; }
          set { _JiraCLIFileLocation = value; }
        }    
        
        string _JiraBatchFileFolderLocation = "";
        [TestVariable("059A3ED0-68B3-4F55-B17D-6C0266B28016")]
        public string JiraBatchFileFolderLocation
        {
          get { return _JiraBatchFileFolderLocation; }
          set { _JiraBatchFileFolderLocation = value; }
        }

        //-----------------------------------------------------------------------------------------
        private string attachments = "";
        
        void writeBatchFile(string batFileName, string reportFileName, string testcaseName)
        {
            ITestContainer tc = TestSuite.CurrentTestContainer;
            JiraConfiguration config = JiraConfiguration.Instance;
        	
            var file = File.CreateText(batFileName);
            
            string chageDirectory = String.Format("CD \"{0}\"", JiraCLIFileLocation.Substring(0, JiraCLIFileLocation.LastIndexOf('\\') +1));
            // Jira WebService calls for CLI environment
            string jiraCall = String.Format("CALL \"{0}\" {1} {2} {3} {4} ", 
                                            JiraCLIFileLocation.Substring(JiraCLIFileLocation.LastIndexOf('\\') +1), config.ServerUrl, config.UserName, config.Password, tc.Name);

            string actionCreateIssue = String.Format("{0} \"{1}\" \"{2}\" \"{3}\" ",
                                                     config.JiraProjectKey, config.JiraIssueType, config.JiraSummary, getDescriptionString(config));
            
            
            if(!string.IsNullOrEmpty(config.JiraLabels))
            {
              actionCreateIssue = actionCreateIssue + String.Format(" \"{0}\" ", config.JiraLabels);
            }

            actionCreateIssue += String.Format("\"{0}\" ", 
                                                       System.IO.Path.Combine(Ranorex.Core.Reporting.TestReport.ReportEnvironment.ReportFileDirectory, reportFileName));

            actionCreateIssue += String.Format("\"{0}\" ", attachments);

            file.WriteLine("@ECHO off");
            file.WriteLine(chageDirectory);
            file.WriteLine( String.Format("{0} {1} %* > return_string_{2}.txt", jiraCall, actionCreateIssue, testcaseName));
            file.WriteLine(String.Format("type return_string_{0}.txt", testcaseName));
            
            //parse return string from Jira WebService call to get issue ID
            file.WriteLine(String.Format("FOR /F \"tokens=2,6,8 delims== \" %%I IN (return_string_{0}.txt) DO ( set KEY=%%I & set ID=%%J & set URL=%%K)", testcaseName));
            file.WriteLine("set ID=%ID:~0,-1%");
            
            //prompt if user wants to review the created issue in the default browser
            file.WriteLine("if %errorlevel% neq 0 echo ERROR: Something went wrong uploading the issue! & pause");
            file.WriteLine("if %errorlevel% neq 0 exit /b %errorlevel%");
            file.WriteLine("echo Issue created and report uploaded");
            
            file.WriteLine("set /p answer=Do you want to open the issue in your default browser (Y/[N])?");
            file.WriteLine("if /i \"%answer:~,1%\" EQU \"Y\" start %URL%");
            file.WriteLine("else exit /b");
            file.WriteLine("echo Please type Y for Yes or N for No");
            
            file.Close();
            file.Dispose();
        }

        string getDescriptionString(JiraConfiguration config)
        {
            string descriptionString = "";

            foreach (JiraDescriptionItem item in config.JiraDescription)
            {
                descriptionString += "{LB} " + item.text;

                if (item.isImageEntry())
                {
                    if (attachments.Length > 0)
                    {
                        attachments += ";";
                    }
                    attachments += System.IO.Path.Combine(Ranorex.Core.Reporting.TestReport.ReportEnvironment.ReportFileDirectory, item.getValue());
                    descriptionString += "{LB} !" + Path.GetFileName(item.getValue()) + "!";
                }
            }

            return descriptionString;
        }
        
        //-----------------------------------------------------------------------------------------
        
        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
          var tc = TestSuite.CurrentTestContainer;

          if (tc == null)
          {
            Report.Error("TestCase is 'null'; this usually happens when the module is used outside of testcases (e.g., global teardown).");
          }

          if(tc.Status == Ranorex.Core.Reporting.ActivityStatus.Failed)
          {
            try
            {
              // create intermediate zipped report
              string reportFileName =  Ranorex.Core.Reporting.TestReport.ReportEnvironment.ReportViewFileName;
              Ranorex.Core.Reporting.TestReport.SaveReport();
              Ranorex.Report.Zip(Ranorex.Core.Reporting.TestReport.ReportEnvironment, 
                                 Ranorex.Core.Reporting.TestReport.ReportEnvironment.ReportFileDirectory, reportFileName);
              reportFileName = reportFileName.Replace(".rxlog", ".rxzlog");
              
              string batFileName = String.Format("createJiraIssue_{0}_{1}.bat", tc.Name, 
                                                 Ranorex.Core.Reporting.TestReport.ReportEnvironment.ReportViewFileName);
              
              System.IO.FileInfo fi = null;
              try
              {
                fi = new System.IO.FileInfo(JiraCLIFileLocation);
              }
              catch(Exception e)
              {
                Report.Warn("Jira CLI file not accessible; please check path to file! (Exception: " + e.Message +")");
              }
              
              // create batch file, which is triggered from the report to create an issue
              writeBatchFile(batFileName, reportFileName, tc.Name);
              
              // copy batch file to the final reporting folder if needed
              var batfileLocation = "";
              if(String.IsNullOrEmpty(JiraBatchFileFolderLocation))
                batfileLocation = System.IO.Path.Combine(Ranorex.Core.Reporting.TestReport.ReportEnvironment.ReportFileDirectory, batFileName);
              else
              {
                fi = new System.IO.FileInfo(JiraBatchFileFolderLocation);
                batfileLocation = System.IO.Path.Combine(JiraBatchFileFolderLocation, "");
              }
              
              if(batfileLocation != System.IO.Directory.GetCurrentDirectory())
                File.Copy(batFileName, batfileLocation, true);
              
              // create link to trigger the batch file
              // NOTE: link is only working if the original batch file location is still valid!
              JiraConfiguration config = JiraConfiguration.Instance;
              Report.LogHtml(ReportLevel.Info, String.Format("Jira issue ready for creation : <a href=\"file:///{0}\\{1}\" > Create Issue </a>", batfileLocation, batFileName));
              Report.Info(String.Format("Used BatchFile: {0};  Original location: {1}", batFileName, batfileLocation));
              Report.LogHtml(ReportLevel.Info, String.Format("Link to Jira project: <a href=\"{0}/browse/{1} \"> {1} </a>", 
                                                             JiraReporter.ServerURL, config.JiraProjectKey));
            }
            catch(Exception e)
            {
              Report.Error(e.Message + " (InnerException: " + e.InnerException + ")");
            }
          }
        }
    }
}
