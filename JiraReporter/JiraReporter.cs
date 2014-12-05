/*
 * Created by Ranorex
 * User: cbreit
 * Date: 22.10.2014
 * Time: 18:58
 * 
 * Acknowledgement:
 * This product includes software developed by TechTalk.
 */

using System;
using System.Collections.Generic;

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
      _client = new TechTalk.JiraRestClient.JiraClient(serverURL, user,password);
      _serverURL = serverURL;
      _info = _client.GetServerInfo();
    }

    public static JiraIssue CreateIssue(string testCaseName, string summary, string description, List<string> labels, string issueType, string projectKey, bool attachReport)
    {
      CheckIfClientConnected();

      TechTalk.JiraRestClient.IssueFields fields = new TechTalk.JiraRestClient.IssueFields();

      fields.summary = testCaseName + ": " + summary;
      fields.description = description;
      fields.labels = labels;

      // not supported by the JiraClient yet
      //fields.issuePriority = "1";

      if (GetIssueType(issueType) == null)
        throw (new Exception(String.Format("Issue Type '{0}' not found!", issueType)));

      var createdIssue = _client.CreateIssue(projectKey, issueType, fields);
      if(attachReport)
        UploadRannorexReport(createdIssue);

      return(new JiraIssue(createdIssue.key, createdIssue.id));
    }
    
    public static JiraIssue ReOpenIssue(string issueKey)
    {
      CheckIfClientConnected();

      var curIssue = _client.LoadIssue(issueKey);

      JiraIssue issue = null;

      IEnumerable<TechTalk.JiraRestClient.Transition> transitions = _client.GetTransitions(curIssue);
      foreach (TechTalk.JiraRestClient.Transition trans in transitions)
      {
        if (trans.to.name.Contains("Reopened"))
        {
          trans.fields = null;
          _client.TransitionIssue(curIssue, trans);
          issue = new JiraIssue(curIssue.key, curIssue.id);
          break;
        }
      }

      return issue;
    }

    public static JiraIssue ResolveIssue(string issueKey, bool attachReport)
    {
      CheckIfClientConnected();

      var curIssue = _client.LoadIssue(issueKey);

      JiraIssue issue = null;

      IEnumerable<TechTalk.JiraRestClient.Transition> transitions = _client.GetTransitions(curIssue);
      foreach (TechTalk.JiraRestClient.Transition trans in transitions)
      {
        if (trans.to.name.Contains("Resolved"))
        {
          trans.fields = null;
          _client.TransitionIssue(curIssue, trans);
          issue = new JiraIssue(curIssue.key, curIssue.id);
          break;
        }
      }

      if (attachReport)
        UploadRannorexReport(curIssue);

      return issue;
    }

    public static JiraIssue UpdateIssue(string issueKey, string testCaseName, string summary, 
      string description, List<string> labels, bool attachReport)
    {
      CheckIfClientConnected();

      var curIssue = _client.LoadIssue(issueKey);

      curIssue.fields.summary = testCaseName + ": " + summary;
      curIssue.fields.description = description;

      curIssue.fields.labels = labels;

      _client.UpdateIssue(curIssue);
      JiraReporter.UploadRannorexReport(curIssue);

      if (attachReport)
        UploadRannorexReport(curIssue);

      return (new JiraIssue(curIssue.key, curIssue.id));
    }

    public static string GetServerTitle()
    {
      CheckIfClientConnected();

      return (_info.serverTitle);
    }

    public static string GetServerVersion()
    {
      CheckIfClientConnected();

       return (_info.version);
    }

    public static string GetIssueTypeID(string issueType)
    {
      CheckIfClientConnected();

      var type = GetIssueType(issueType);
      
      if (type == null)
        return null;


      return type.id.ToString();
    }

    private static void UploadRannorexReport(TechTalk.JiraRestClient.IssueRef issueRef)
    {
      string fileName = Ranorex.Core.Reporting.TestReport.ReportEnvironment.ReportViewFileName;
      Ranorex.Core.Reporting.TestReport.SaveReport();
      Ranorex.Report.Zip(Ranorex.Core.Reporting.TestReport.ReportEnvironment, null, fileName);
      fileName = fileName.Replace(".rxlog", ".rxzlog");
      System.IO.FileStream stream = System.IO.File.Open(System.IO.Path.GetFullPath(fileName), System.IO.FileMode.Open);

      _client.CreateAttachment(issueRef, stream, fileName);
    }

    private static TechTalk.JiraRestClient.IssueType GetIssueType(string issueType)
    {
      var issueTypes = _client.GetIssueTypes();
      string available = "";
      foreach (TechTalk.JiraRestClient.IssueType type in issueTypes)
      {
        if (type.name == issueType)
          return type;
        available = available + type.name + " ";
      }

      Ranorex.Report.Error("Issue type '" + issueType + "' not found! Available types: " + available.TrimEnd());
      return null;
    }

    private static void CheckIfClientConnected()
    {
      if(_client == null)
        throw(new Exception("Jira client not initialized -- not connecting to Jira (maybe the 'InitializeJiraReporter' was forgotten in the global 'Setup' region)!"));
    }

    public static string ServerURL
    {
      get
      {
        CheckIfClientConnected();
        return _serverURL;
      }
    }
    
    static private TechTalk.JiraRestClient.IJiraClient _client = null;
    static private TechTalk.JiraRestClient.ServerInfo  _info = null;
    static private string _serverURL;
  }
}
