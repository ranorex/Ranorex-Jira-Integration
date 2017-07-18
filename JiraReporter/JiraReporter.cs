/*
 * Created by Ranorex
 * User: cbreit, sknopper
 * Date: 22.10.2014
 * Time: 18:58
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira;
using RestSharp.Extensions;

namespace JiraReporter
{
  public class JiraIssue 
  {
    public JiraIssue(string id, string key)
    {
      _id = id;
      _key = key;
    }

    public string Key
    {
      get { return _key; }
      set { _key = value; }
    }

    public string Id
    {
      get { return _id; }
      set { _id = value; }
    }

    private string _key;
    private string _id;
  }


  /// <summary>
  /// Description of class JiraReporter.
  /// </summary>
  /// 
  public static class JiraReporter
  {
    static JiraReporter() { }
    
    public static void ConnectJiraServer(string user, string password, string serverURL)
    {
      client = Jira.CreateRestClient(serverURL, user, password);
    }

    public static JiraIssue CreateIssue(string testCaseName, bool attachReport)
    {
            CheckIfClientConnected();
            JiraConfiguration config = JiraConfiguration.Instance;

            if (GetIssueType(config.JiraProjectKey, config.JiraIssueType) == null)
            {
                throw (new Exception(String.Format("Issue Type '{0}' not found!", config.JiraIssueType)));
            }

            Issue issue = client.CreateIssue(config.JiraProjectKey);
            issue.Type = config.JiraIssueType;
            //issue.Priority = "Major";
            issue.Summary = testCaseName + ": " + config.JiraSummary;
            
            if (config.RxAutomationFieldName != null && !config.customFields.ContainsKey(config.RxAutomationFieldName)) 
            {
            	config.customFields.Add(config.RxAutomationFieldName, testCaseName);
            }
            
            foreach (string label in config.getAllLabels()) {
                issue.Labels.Add(label);
            }

            foreach (string key in config.customFields.Keys)
            {
                string value = null;
                config.customFields.TryGetValue(key, out value);
                issue.CustomFields.Add(key, value);
            }
            
            issue.SaveChanges();

            if (attachReport)
            {
                addRanorexReport(issue);
            }
            
            issue.Description = updateDescription(issue, config);
            issue.SaveChanges();

            var jiraIssue = new JiraIssue(issue.Key.ToString(), issue.JiraIdentifier);

            return (jiraIssue);
    }
    
    private static string updateDescription(Issue issue, JiraConfiguration config)
    {
    	string descriptionString = "";

    	foreach (JiraDescriptionItem item in config.JiraDescription)
    	{
    		descriptionString += "\r\n " + item.text;
    		
    		if (item.isImageEntry())
    		{
    			issue.AddAttachment(item.getValue());
    			descriptionString += "\r\n !" + Path.GetFileName(item.getValue()) + "!";
    		} 
    	}
    	
    	return descriptionString;
    }

        public static IEnumerable<Issue> getJiraIssues(string searchString)
        {
            CheckIfClientConnected();
            var issues = client.Issues.GetIssuesFromJqlAsync(searchString);
            return issues.Result;
        }
    
    public static JiraIssue ChangeState(string issueKey, string transitionName, bool attachReport)
        {
            CheckIfClientConnected();

            Issue issue = client.Issues.GetIssueAsync(issueKey, CancellationToken.None).Result;
            JiraIssue jiraIssue = new JiraIssue(issue.Key.ToString(), issue.JiraIdentifier);

            issue.WorkflowTransitionAsync(transitionName).GetAwaiter().GetResult();

            if (issue == null)
            {
                throw new Ranorex.RanorexException(String.Format("Transition '{0}' was not found, unable to change the state of the issue", transitionName));
            }

            if (attachReport)
            {
                addRanorexReport(issue);
            }

            issue.SaveChanges();
            return jiraIssue;
        }

    public static JiraIssue UpdateIssue(string issueKey, string testCaseName, string summary, 
      string description, List<string> labels, bool attachReport)
    {
      CheckIfClientConnected();

            Issue issue = client.Issues.GetIssueAsync(issueKey, CancellationToken.None).Result;
            if (issue == null)
            {
                throw (new Exception(String.Format("Could not load issue '{0}'!", issueKey)));
            }

      if (!string.IsNullOrEmpty(summary))
            {
                issue.Summary = testCaseName + ": " + summary;
            } else
            {
                issue.Summary = null;
            }

      if (!string.IsNullOrEmpty(description))
            {
                issue.Description = description;
            } else
            {
                issue.Description = null;
            }

            issue.Labels.Clear();
            if (labels != null)
            {
                foreach (string label in labels)
                {
                    issue.Labels.Add(label);
                }
            }

      if (attachReport)
            {
                addRanorexReport(issue);
            }

            issue.SaveChanges();

            return new JiraIssue(issue.Key.ToString(), issue.JiraIdentifier);
    }

    private static void addRanorexReport(Issue issue)
    {
      string fileName = Ranorex.Core.Reporting.TestReport.ReportEnvironment.ReportViewFilePath;
      Ranorex.Core.Reporting.TestReport.SaveReport();
      Ranorex.Report.Zip(Ranorex.Core.Reporting.TestReport.ReportEnvironment, null, fileName);
      fileName = fileName.Replace(".rxlog", ".rxzlog");

      issue.AddAttachment(fileName);
    }

    public static void CheckIfClientConnected()
    {
      if(client == null)
        throw(new Exception("Jira client not initialized -- not connecting to Jira (maybe the 'InitializeJiraReporter' was forgotten in the global 'Setup' region)!"));
    }

    public static string ServerURL
    {
      get
      {
        CheckIfClientConnected();
        return client.Url;
      }
    }

        private static IssueType GetIssueType(string projectKey, string issueType)
        {
            IEnumerable<IssueType> types = client.IssueTypes.GetIssueTypesForProjectAsync(projectKey, CancellationToken.None).Result;
            foreach (IssueType type in types)
            {
                if (type.Name.Equals(issueType)) {
                    return type;
                }
            }

            Ranorex.Report.Error("Issue type '" + issueType + "' not found! Available.");
            return null;
        }

        static private Jira client = null;
  }
}
