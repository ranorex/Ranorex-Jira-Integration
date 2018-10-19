
using Ranorex.Core.Testing;

namespace JiraReporter
{

  [UserCodeCollection]
  public class JiraFieldHelper
  {

    [UserCodeMethod]
    public static void addCustomFieldValue(string fieldName, string fieldValue)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      removeCustomFieldValue(fieldName);
      config.customFields.Add(fieldName, fieldValue);
    }

    [UserCodeMethod]
    public static void removeCustomFieldValue(string fieldName)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      if (config.customFields.ContainsKey(fieldName))
      {
        config.customFields.Remove(fieldName);
      }
    }


    /// <summary>
    /// This is advanced functionality to utilize cascading fields in Jira.
    /// </summary>
    [UserCodeMethod]
    public static void addCustomCascadingField(string fieldName, string parentValue, string childValue)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      removeCustomCascadingFieldValue(fieldName);
      string[] values = { parentValue, childValue };
      config.customCascadingFields.Add(fieldName, values);
    }

    [UserCodeMethod]
    public static void removeCustomCascadingFieldValue(string fieldName)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      if (config.customCascadingFields.ContainsKey(fieldName))
      {
        config.customCascadingFields.Remove(fieldName);
      }
    }

    [UserCodeMethod]
    public static string getCurrentSprintId()
    {
      return JiraReporter.getCurrentSprintItem("id");
    }

    [UserCodeMethod]
    public static string getCurrentSprintName()
    {
      return JiraReporter.getCurrentSprintItem("name");
    }

    [UserCodeMethod]
    public static void clearDescription()
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      config.JiraDescription.Clear();
    }

    [UserCodeMethod]
    public static void addNewLineToDescription(string line)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      config.JiraDescription.Add(new JiraDescriptionItem("\r\n " + line, null));
    }

    [UserCodeMethod]
    public static void setEnvironment(string environment)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      config.JiraEnvironment = environment;
    }

    [UserCodeMethod]
    public static void setAssignee(string assignee)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      config.Assignee = assignee;
    }

    [UserCodeMethod]
    public static void setDueDate(string dueDate)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      config.DueDate = dueDate;
    }

    /// <summary>
    /// Use ";" as a delimiter between versions
    /// </summary>
    [UserCodeMethod]
    public static void setFixVersions(string versions)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      config.FixVersions = versions;
    }

    [UserCodeMethod]
    public static void addFixVersions(string version)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      config.FixVersions += ";" + version;
    }

    /// <summary>
    /// Versions have to be separated by ";"
    /// </summary>
    [UserCodeMethod]
    public static void removeFixVersions(string version)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      string oldVersions = config.FixVersions;
      string[] versions = oldVersions.Split(';');
      string newVersions = "";
      foreach (string ver in versions)
      {
        if (!newVersions.Equals(""))
        {
          newVersions += ";";
        }
        if (!ver.Equals(version))
        {
          newVersions += ver;
        }
      }

      JiraFieldHelper.setFixVersions(newVersions);
    }

    /// <summary>
    /// Use ";" as a delimiter between versions
    /// </summary>
    [UserCodeMethod]
    public static void setAffectsVersions(string versions)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      config.AffectsVersions = versions;
    }

    [UserCodeMethod]
    public static void addAffectsVersions(string version)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      config.AffectsVersions += ";" + version;
    }

    /// <summary>
    /// Versions have to be separated by ";"
    /// </summary>
    [UserCodeMethod]
    public static void removeAffectsVersions(string version)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      string oldVersions = config.AffectsVersions;
      string[] versions = oldVersions.Split(';');
      string newVersions = "";
      foreach (string ver in versions)
      {
        if (!newVersions.Equals(""))
        {
          newVersions += ";";
        }
        if (!ver.Equals(version))
        {
          newVersions += ver;
        }
      }

      JiraFieldHelper.setAffectsVersions(newVersions);
    }

    /// <summary>
    /// Use ";" as a delimiter between labels
    /// </summary>
    [UserCodeMethod]
    public static void setLabels(string labels)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      config.JiraLabels = labels;
    }

    [UserCodeMethod]
    public static void addNewLabel(string label)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      config.JiraLabels += ";" + label;
    }

    /// <summary>
    /// Labels have to be separated by ";"
    /// </summary>
    [UserCodeMethod]
    public static void removeLabel(string label)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      string oldLabels = config.JiraLabels;
      string[] labels = oldLabels.Split(';');
      string newLabels = "";
      foreach (string lab in labels)
      {
        if (!newLabels.Equals(""))
        {
          newLabels += ";";
        }
        if (!lab.Equals(label))
        {
          newLabels += lab;
        }
      }

      JiraFieldHelper.setLabels(newLabels);
    }

    /// <summary>
    /// Set the Jira issue key for this test case. 
    /// <para /> <strong>IMPORTANT:</strong> This value is cleared after a module interacting with Jira (like 'AutoHandleJiraIntegration') is run!
    /// </summary>
    [UserCodeMethod]
    public static void setJiraIssueKey(string key)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      config.transientConfig.JiraIssueKey = key.Trim();
      
    }

    /// <summary>
    /// Set the global Jira JQL query. 
    /// </summary>
    [UserCodeMethod]
    public static void setJiraJQLQuery(string query)
    {
      JiraConfiguration config = JiraConfiguration.Instance;
      config.jqlQueryToConnectIssues = query;
    }

  }
}
