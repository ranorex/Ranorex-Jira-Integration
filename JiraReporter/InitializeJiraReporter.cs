/*
 * Created by Ranorex
 * User: cbreit
 * Date: 22.10.2014
 * Time: 19:09
 * 
 * Acknowledgement:
 * This product includes software developed by TechTalk.
 */
using System;

using Ranorex;
using Ranorex.Core.Testing;

namespace JiraReporter
{
    /// <summary>
    /// Description of InitializeJiraReporter.
    /// </summary>
    [TestModule("0C593212-390B-4EC4-AFA2-C63525001F3C", ModuleType.UserCode, 1)]
    public class InitializeJiraReporter : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public InitializeJiraReporter()
        {
            // Do not delete - a parameterless constructor is required!
        }

        string _JiraUserName = "";
        [TestVariable("87147480-A368-4C8D-935A-AC9928641B17")]
        public string JiraUserName
        {
          get { return _JiraUserName; }
          set { _JiraUserName = value; }
        }
        
        string _JiraServerURL = "";
        [TestVariable("2D58EEE0-06DE-4D7E-9C04-DCA5CD7A1A91")]
        public string JiraServerURL
        {
          get { return _JiraServerURL; }
          set { _JiraServerURL = value; }
        }
        
        string _JiraPassword = "";
        [TestVariable("D4637164-3891-47A1-A96E-77396F53F387")]
        public string JiraPassword
        {
          get { return _JiraPassword; }
          set { _JiraPassword = value; }
        }
        
        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
          try
          {
          	JiraConfiguration config = JiraConfiguration.Instance;
	        config.ServerUrl = _JiraServerURL;
	        config.Password = _JiraPassword;
	        config.UserName = _JiraUserName;
          	
            JiraReporter.ConnectJiraServer(_JiraUserName, _JiraPassword, _JiraServerURL);
            //Report.Info(JiraReporter.GetServerTitle() + " -- " + JiraReporter.GetServerVersion());
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
