
using System.Collections.Generic;

namespace JiraReporter
{

  public class JiraConfiguration
  {
    private static JiraConfiguration instance;

    public bool enabled { get; set; }

    public string ServerUrl { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    public string JiraProjectKey { get; set; }
    public string JiraIssueType { get; set; }
    public string JiraSummary { get; set; }
    public IList<JiraDescriptionItem> JiraDescription { get; set; }
    public string JiraLabels { get; set; }
    public string StateClosed { get; set; }
    public string StateReopen { get; set; }
    public string RxAutomationFieldName { get; set; }
    public string jqlQueryToConnectIssues { get; set; }
    public string JiraIssuePriority { get; set; }
    public string JiraEnvironment { get; set; }
    public Dictionary<string, string> customFields = new Dictionary<string, string>();
    public Dictionary<string, string[]> customCascadingFields = new Dictionary<string, string[]>();

    
    public string Assignee { get; set; }
    public string DueDate { get; set; }

    public string AffectsVersions { get; set; }
    public string FixVersions { get; set; }

    // this content of this configuration gets reset after the "AutoHandleJiraIntegration" is run
    // it's intended to be used for 
    public class TransientJiraConfig
    {
      public string JiraIssueKey { get; set; }

      public TransientJiraConfig()
      {
        Clear();
      }
      
      public void Clear()
      {
        JiraIssueKey = string.Empty;
      }
    }

    public TransientJiraConfig transientConfig = new TransientJiraConfig();

    private JiraConfiguration()
    {
      JiraDescription = new List<JiraDescriptionItem>();
    }

    public static JiraConfiguration Instance
    {
      get
      {
        if (instance == null)
        {
          instance = new JiraConfiguration();
        }
        return instance;
      }
    }

    public List<string> getAllLabels()
    {
      char delimiterChar = ';';
      return new List<string>(JiraLabels.Split(delimiterChar));
    }
  }
}
