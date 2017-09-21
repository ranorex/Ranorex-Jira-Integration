/*
 * Created by Ranorex
 * User: sknopper
 * Date: 14.04.2017
 * 
 * Acknowledgement:
 * This product includes software developed by TechTalk.
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using Ranorex.Core.Testing;

namespace JiraReporter
{
    /// <summary>
    /// This module creates a new issue within jira if the test case fails.
    /// </summary>
    [TestModule("DE17F33E-345F-416F-9F58-505B5E34AAE3", ModuleType.UserCode, 1)]
    public class AutoCreateNewIssueIfTestCaseFails : AbstractJiraIntegrationClient, ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public AutoCreateNewIssueIfTestCaseFails()
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
        	ITestContainer tc = checkTestCase();
        	
        	if (tc.Status == Ranorex.Core.Reporting.ActivityStatus.Failed) {
	        	JiraConfiguration config = JiraConfiguration.Instance;
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
