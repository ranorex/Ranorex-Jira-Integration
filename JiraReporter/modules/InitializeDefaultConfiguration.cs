
using System;
using Ranorex;
using Ranorex.Core.Testing;

namespace JiraReporter
{

  [TestModule("76FF8843-49B3-4968-9C30-CE917AB28D5E", ModuleType.UserCode, 1)]
  public class InitializeDefaultConfiguration : ITestModule
  {
    string _varEnableJiraIntegration = "default";
    [TestVariable("47CD5896-A6BA-4D0C-AA37-B1A6E974B214")]
    public string EnableJiraIntegration
    {
      get { return _varEnableJiraIntegration; }
      set { _varEnableJiraIntegration = value; }
    }

    string _varJiraProjectKey = "";
    [TestVariable("2DC0BBB3-C44A-42B6-A695-9FB1ABFF8D84")]
    public string JiraProjectKey
    {
      get { return _varJiraProjectKey; }
      set { _varJiraProjectKey = value; }
    }

    string _varJiraIssueType = "Bug";
    [TestVariable("D9248964-3F28-4ABE-A5A8-324C190CF2F4")]
    public string JiraIssueType
    {
      get { return _varJiraIssueType; }
      set { _varJiraIssueType = value; }
    }

    string _varJiraSummary = "";
    [TestVariable("E16AC939-1D71-4936-8009-DE50E23325CA")]
    public string JiraSummary
    {
      get { return _varJiraSummary; }
      set { _varJiraSummary = value; }
    }

    string _varJiraDescription = "";
    [TestVariable("835F55D5-961E-4F62-AAE1-0AA992C725EF")]
    public string JiraDescription
    {
      get { return _varJiraDescription; }
      set { _varJiraDescription = value; }
    }

    string _varJiraLabels = "";
    [TestVariable("AB382EAB-9609-430A-B18E-46B19A493A5F")]
    public string JiraLabels
    {
      get { return _varJiraLabels; }
      set { _varJiraLabels = value; }
    }

    string _StateClosed = "";
    [TestVariable("C4F14A65-DB66-4381-BABD-921E14077BE6")]
    public string StateClosed
    {
      get { return _StateClosed; }
      set { _StateClosed = value; }
    }

    string _StateReopen = "";
    [TestVariable("F6201ADA-3A5B-4879-B9E1-2D9C839B2736")]
    public string StateReopen
    {
      get { return _StateReopen; }
      set { _StateReopen = value; }
    }

    string _RxAutomationFieldName = "";
    [TestVariable("24AAC369-38AB-45EC-8A87-C79EC20CFD07")]
    public string RxAutomationFieldName
    {
      get { return _RxAutomationFieldName; }
      set { _RxAutomationFieldName = value; }
    }

    string _jqlQueryToConnectIssues = "";
    [TestVariable("0FA988AE-AAE9-40C4-9386-F51B13E8C96B7")]
    public string jqlQueryToConnectIssues
    {
      get { return _jqlQueryToConnectIssues; }
      set { _jqlQueryToConnectIssues = value; }
    }

    string _jiraIssuePriority = "";
    [TestVariable("153C1454-FAC1-49B6-BB0E-3126B0C1C0B1")]
    public string jiraIssuePriority
    {
      get { return _jiraIssuePriority; }
      set { _jiraIssuePriority = value; }
    }

    string _jiraEnvironment = "";
    [TestVariable("DF32F91E-A96C-40EA-B3E8-2E2154BF1012")]
    public string jiraEnvironment
    {
      get { return _jiraEnvironment; }
      set { _jiraEnvironment = value; }
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
    /// Constructs a new instance.
    /// </summary>
    public InitializeDefaultConfiguration()
    {
      // Do not delete - a parameterless constructor is required!
    }

    /// <summary>
    /// Performs the playback of actions in this module.
    /// </summary>
    /// <remarks>You should not call this method directly, instead pass the module
    /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
    /// that will in turn invoke this method.</remarks>
    void ITestModule.Run()
    {
      Mouse.DefaultMoveTime = 300;
      Keyboard.DefaultKeyPressTime = 100;
      Delay.SpeedFactor = 1.0;

      JiraConfiguration config = JiraConfiguration.Instance;

      if (this.EnableJiraIntegration == "default" || this.EnableJiraIntegration.IsEmpty())
      {
        config.enabled = false;
        Report.Warn("Jira integration disabled by default! Please specify the desired state as a paramater!");
      }
      else
      {
        config.enabled = ToBoolean(this.EnableJiraIntegration);
      }


      if (this.JiraProjectKey != null)
      {
        config.JiraProjectKey = this.JiraProjectKey;
      }

      if (this.JiraIssueType != null)
      {
        config.JiraIssueType = this.JiraIssueType;
      }

      if (this.JiraSummary != null)
      {
        config.JiraSummary = this.JiraSummary;
      }

      if (this.JiraDescription != null)
      {
        config.JiraDescription.Add(new JiraDescriptionItem(this.JiraDescription, null));
      }

      if (this.JiraLabels != null)
      {
        config.JiraLabels = this.JiraLabels;
      }

      if (this.StateClosed != null)
      {
        config.StateClosed = this.StateClosed;
      }

      if (this.StateReopen != null)
      {
        config.StateReopen = this.StateReopen;
      }

      if (this.RxAutomationFieldName != null && !this.RxAutomationFieldName.Equals(""))
      {
        config.RxAutomationFieldName = this.RxAutomationFieldName;
      }

      if (this.jqlQueryToConnectIssues != null)
      {
        config.jqlQueryToConnectIssues = this.jqlQueryToConnectIssues;
      }

      if (this.jiraIssuePriority != null)
      {
        config.JiraIssuePriority = this.jiraIssuePriority;
      }

      if (this.jiraEnvironment != null)
      {
        config.JiraEnvironment = this.jiraEnvironment;
      }
      
      initialConnect();
    }

    private void initialConnect()
    {
      try
      {
        JiraConfiguration config = JiraConfiguration.Instance;

        if(!config.enabled)
        {
          Report.Debug("Jira integration disabled in config!");
          return;
        }

        config.ServerUrl = _JiraServerURL;
        config.Password = _JiraPassword;
        config.UserName = _JiraUserName;

        JiraReporter.ConnectJiraServer();
        //Report.Info(JiraReporter.GetServerTitle() + " -- " + JiraReporter.GetServerVersion());
      }
      catch (Exception e)
      {
        var inner = e.InnerException;
        string str = "";
        if (inner != null)
        {
          var prop = inner.GetType().GetProperty("ErrorResponse");
          if (prop != null)
            str = (string)prop.GetValue(e.InnerException, null);
        }

        Report.Error(e.Message + " (InnerException: " + e.InnerException + " -- " + str + ")");
      }
    }

    private bool ToBoolean(string value)
    {
      switch (value.ToLower())
      {
        case "true":
          return true;
        case "t":
          return true;
        case "1":
          return true;
        case "0":
          return false;
        case "false":
          return false;
        case "f":
          return false;
        default:
          throw new InvalidCastException("Can't identify string as a boolean value!");
      }
    }

  }
}
