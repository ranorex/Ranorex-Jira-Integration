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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

using System.IO;

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
        
        string _JiraServerURL = "";
        [TestVariable("0EB874F5-E0B4-4901-B372-95CB3D392046")]
        public string JiraServerURL
        {
          get { return _JiraServerURL; }
          set { _JiraServerURL = value; }
        }
        
        string _JiraPassword = "";
        [TestVariable("C2AC03AE-CC82-4C2F-8AA3-8AF635E2CBFA")]
        public string JiraPassword
        {
          get { return _JiraPassword; }
          set { _JiraPassword = value; }
        }
        
        string _JiraUserName = "";
        [TestVariable("C363DFE4-03BF-4831-8B4B-16544C6CEFF6")]
        public string JiraUserName
        {
          get { return _JiraUserName; }
          set { _JiraUserName = value; }
        }
        
        string _JiraProjectKey = "";
        [TestVariable("B9F47338-E1BF-4131-82E1-F2C69B8CFD3D")]
        public string JiraProjectKey
        {
          get { return _JiraProjectKey; }
          set { _JiraProjectKey = value; }
        }
        
        string _JiraIssueType = "Bug";
        [TestVariable("6EFBD4FE-807E-46B4-8CAF-8B75821FA7C8")]
        public string JiraIssueType
        {
          get { return _JiraIssueType; }
          set { _JiraIssueType = value; }
        }
        
        string _JiraDescription = "";
        [TestVariable("319E5B87-949B-4F5E-9F5A-66153BFA2B83")]
        public string JiraDescription
        {
          get { return _JiraDescription; }
          set { _JiraDescription = value; }
        }
        
        string _JiraSummary = "";
        [TestVariable("A0209D92-DC22-4031-A8F5-190088FE4826")]
        public string JiraSummary
        {
          get { return _JiraSummary; }
          set { _JiraSummary = value; }
        }
        
        string _JiraBatchFileFolderLocation = "";
        [TestVariable("059A3ED0-68B3-4F55-B17D-6C0266B28016")]
        public string JiraBatchFileFolderLocation
        {
          get { return _JiraBatchFileFolderLocation; }
          set { _JiraBatchFileFolderLocation = value; }
        }
        
        string _JiraLabels = "";
        [TestVariable("DB921E51-216C-40AD-AD1A-DA693C53551C")]
        public string JiraLabels
        {
          get { return _JiraLabels; }
          set { _JiraLabels = value; }
        }
        
        
        //-----------------------------------------------------------------------------------------
        
        void writeBatchFile(string batFileName, string issueTypeId, string reportFileName, string testcaseName)
        {
            var file = File.CreateText(batFileName);
            // Jira WebService calls for CLI environment
            string jiraCall = String.Format("CALL java -jar \"{0}\" --server {1} --user {2} --password {3} ", 
                                            JiraCLIFileLocation, JiraServerURL, JiraUserName, JiraPassword);
                       
            string actionCreateIssue = String.Format("--action createIssue --project {0} --type \"1\" --summary \"{2}\" ",
                                                     JiraProjectKey, issueTypeId, JiraSummary);
            
            
            if(!string.IsNullOrEmpty(JiraLabels))
            {
              char delimiterChar = ';';
              var labels = JiraLabels.Replace(delimiterChar, ' ');
              actionCreateIssue = actionCreateIssue + String.Format("--labels \"{0}\" ", labels);
            }
            
            
            string actionAddAttachment = String.Format("--action addAttachment --issue %KEY% --file \"{0}\" ", 
                                                       System.IO.Path.Combine(Ranorex.Core.Reporting.TestReport.ReportEnvironment.ReportFileDirectory, reportFileName));
            
            file.WriteLine("@ECHO off");
            file.WriteLine("echo Important: 'Accept remote API calls' must be enabled on the Jira server!");
            
            file.WriteLine( String.Format("{0} {1} %* > return_string_{2}.txt", jiraCall, actionCreateIssue, testcaseName));
            file.WriteLine(String.Format("type return_string_{0}.txt", testcaseName));
            
            //parse return string from Jira WebService call to get issue ID
            file.WriteLine(String.Format("FOR /F \"tokens=2,6,8 delims== \" %%I IN (return_string_{0}.txt) DO ( set KEY=%%I & set ID=%%J & set URL=%%K)", testcaseName));
            file.WriteLine("set ID=%ID:~0,-1%");
            file.WriteLine( jiraCall + actionAddAttachment+" %*");
            
            //prompt if user wants to review the created issue in the default browser
            file.WriteLine("if %errorlevel% neq 0 echo ERROR: Something went wrong uploading the issue! & pause");
            file.WriteLine("if %errorlevel% neq 0 exit /b %errorlevel%");
            file.WriteLine("echo Issue created and report uploaded");
            
            file.WriteLine("set /p answer=Do you want to open the issue in your default browser (Y/[N])?");
            file.WriteLine("if /i \"%answer:~,1%\" EQU \"Y\" start %URL%");
            file.WriteLine("else exit /b");
            file.WriteLine("echo Please type Y for Yes or N for No");
            
            file.Close();
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
          var tc = TestCase.Current;
          if(tc.Status == Ranorex.Core.Reporting.ActivityStatus.Failed)
          {
            try
            {
              var id = JiraReporter.GetIssueTypeID(JiraIssueType);
              if (id == null)
                throw (new Exception(String.Format("Issue Type '{0}' not found!", JiraIssueType)));

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
              writeBatchFile(batFileName, id, reportFileName, tc.Name);
              
              // copy batch file to the final reporting folder if needed
              var batfileLocation = "";
              if(String.IsNullOrEmpty(JiraBatchFileFolderLocation))
                batfileLocation = System.IO.Path.Combine(Ranorex.Core.Reporting.TestReport.ReportEnvironment.ReportFileDirectory, batFileName);
              else
              {
                fi = new System.IO.FileInfo(JiraBatchFileFolderLocation);
                batfileLocation = System.IO.Path.Combine(JiraBatchFileFolderLocation, batFileName);
              }
              
              if(batfileLocation != System.IO.Directory.GetCurrentDirectory())
                File.Copy(batFileName, batfileLocation, true);
              
              // create link to trigger the batch file
              // NOTE: link is only working if the original batch file location is still valid!
              Report.LogHtml(ReportLevel.Info, String.Format("Jira issue ready for creation : <a href=\"file:///{0}\" > Create Issue </a>", batfileLocation));
              Report.Info(String.Format("Used BatchFile: {0};  Original location: {1}", batFileName, batfileLocation));
              Report.LogHtml(ReportLevel.Info, String.Format("Link to Jira project: <a href=\"{0}/browse/{1} \"> {1} </a>", 
                                                             JiraReporter.ServerURL, JiraProjectKey));
            }
            catch(Exception e)
            {
              Report.Error(e.Message + " (InnerException: " + e.InnerException + ")");
            }
          }
        }
    }
}
