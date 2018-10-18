using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Atlassian.Jira;
using RestSharp;

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

    public static void ConnectJiraServer()
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      client = Jira.CreateRestClient(config.ServerUrl, config.UserName, config.Password);
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

      updateStandardFields(issue, testCaseName);

      addCustomFields(issue, testCaseName);

      addCustomCascadingFields(issue);

      issue.SaveChanges();

      string newDesc = updateDescription(issue, config);
      if (newDesc.Length > 0)
      {
        issue.Description = newDesc;
      }

      if (attachReport)
      {
        addRanorexReport(issue);
      }

      issue.SaveChanges();

      var jiraIssue = new JiraIssue(issue.Key.ToString(), issue.JiraIdentifier);

      return (jiraIssue);
    }

    private static void updateStandardFields(Issue issue, string testCaseName)
    {
      JiraConfiguration config = JiraConfiguration.Instance;

      if (config.JiraIssuePriority != null && config.JiraIssuePriority.Length > 0)
      {
        updatePriority(issue, config.JiraIssuePriority);
      }

      if (config.JiraEnvironment != null && config.JiraEnvironment.Length > 0)
      {
        updateEnvironment(issue, config.JiraEnvironment);
      }

      issue.Description = ".";

      issue.Summary = testCaseName + ": " + config.JiraSummary;

      foreach (string label in config.getAllLabels())
      {
        issue.Labels.Add(label);
      }

      if (config.AffectsVersions != null && config.AffectsVersions.Length > 0)
      {
        string[] versions = config.AffectsVersions.Split(';');
        issue.AffectsVersions.Clear();

        foreach (string version in versions)
        {
          issue.AffectsVersions.Add(version);
        }
      }

      if (config.Assignee != null && config.Assignee.Length > 0)
      {
        issue.Assignee = config.Assignee;
      }

      if (config.DueDate != null && config.DueDate.Length > 0)
      {
        issue.DueDate = DateTime.Parse(config.DueDate);
      }

      if (config.FixVersions != null && config.FixVersions.Length > 0)
      {
        string[] versions = config.FixVersions.Split(';');
        issue.FixVersions.Clear();

        foreach (string version in versions)
        {
          issue.FixVersions.Add(version);
        }
      }
    }

    private static void addCustomFields(Issue issue, string testCaseName)
    {
      JiraConfiguration config = JiraConfiguration.Instance;

      if (config.RxAutomationFieldName != null && !config.RxAutomationFieldName.Equals("") && !config.customFields.ContainsKey(config.RxAutomationFieldName))
      {
        config.customFields.Add(config.RxAutomationFieldName, testCaseName);
      }

      foreach (string key in config.customFields.Keys)
      {
        string value = null;
        config.customFields.TryGetValue(key, out value);

        if (key.Equals(""))
        {
          Ranorex.Report.Log(Ranorex.ReportLevel.Warn, "An invalid custom field is configured for jira. field name: '" + key + "' field value: '" + value + "'");
        }
        else
        {
          issue.CustomFields.Add(key, value);
        }
      }
    }

    private static void addCustomCascadingFields(Issue issue)
    {
      JiraConfiguration config = JiraConfiguration.Instance;

      foreach (string key in config.customCascadingFields.Keys)
      {
        string[] value = null;
        config.customCascadingFields.TryGetValue(key, out value);

        if (key.Equals(""))
        {
          Ranorex.Report.Log(Ranorex.ReportLevel.Warn, "An invalid custom field is configured for jira. field name: '" + key + "' field value: '" + value + "'");
        }
        else
        {
          CascadingSelectCustomField field = new CascadingSelectCustomField(key, value[0], value[1]);
          issue.CustomFields.AddCascadingSelectField(field);
        }
      }
    }

    private static void updateEnvironment(Issue issue, string envString)
    {
      issue.Environment = envString;
    }

    public static string getCurrentSprintItem(string itemKey)
    {
      return getCurrentSprintItem(1, itemKey);
    }

    public static string getCurrentSprintItem(int sprintId, string itemKey)
    {
      RestRequest req = new RestRequest("/rest/agile/latest/sprint/" + sprintId);
      RestResponse resp = client.RestClient.RestSharpClient.ExecuteAsGet(req, "GET") as RestResponse;
      RestSharp.Deserializers.JsonDeserializer deSerial = new RestSharp.Deserializers.JsonDeserializer();
      Dictionary<string, string> item = deSerial.Deserialize<Dictionary<string, string>>(resp);

      string state = "";
      string valueStr = "";
      item.TryGetValue("state", out state);
      if (state != null && state != "" && state.Equals("closed"))
      {
        return getCurrentSprintItem(sprintId + 1, itemKey);
      }
      item.TryGetValue(itemKey, out valueStr);
      return valueStr;
    }

    private static void updatePriority(Issue issue, string prio)
    {
      if (prio != null)
      {
        IssuePriority p = new IssuePriority(null, prio);
        issue.Priority = p;
      }
    }

    public static string updateDescription(Issue issue, JiraConfiguration config)
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
      }
      else
      {
        issue.Summary = null;
      }

      if (!string.IsNullOrEmpty(description))
      {
        issue.Description = description;
      }
      else
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

    public static void addRanorexReport(Issue issue, string fileName)
    {
      Ranorex.Core.Reporting.TestReport.SaveReport();
      Ranorex.Report.Zip(Ranorex.Core.Reporting.TestReport.ReportEnvironment, null, fileName);
      fileName = fileName.Replace(".rxlog", ".rxzlog");

      issue.AddAttachment(fileName);
    }

    private static void addRanorexReport(Issue issue)
    {
      addRanorexReport(issue, Ranorex.Core.Reporting.TestReport.ReportEnvironment.ReportViewFilePath);
    }

    public static void CheckIfClientConnected()
    {
      if (client == null)
        throw (new Exception("Jira client not initialized -- not connecting to Jira (maybe the 'InitializeJiraReporter' was forgotten in the global 'Setup' region)!"));
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
        if (type.Name.Equals(issueType))
        {
          return type;
        }
      }

      Ranorex.Report.Error("Issue type '" + issueType + "' not found! Available.");
      return null;
    }

    static private Jira client = null;
  }
}
