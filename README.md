# Ranorex Jira Integration

## What's in this repository?

This repository contains the source code for the Ranorex.Jira nuget package, which integrates Ranorex Studio into Jira. It relies on the Atlassian.SDK NuGet package and utilizes Jira's REST API.

# Workflows & Features

**The integration supports the following workflows:**
  * Create a new issue if a test case failed
  * Re-open existing issue(s) if a test case failed
  * Close existing issue(s) if a test case succeeded
  * Automatically create, re-open, or close an issue if there is a custom field to store the Ranorex Studio test case name.

**Provided features:**
  * Support for custom fields and cascading custom fields
  * JQL supported for advanced queries
  * On-premise and cloud-based Jira instance
  * Custom modules
  * Enable only during CU/command line execution
  * Custom close or re-open states

## Modules
#### InitializeDefaultConfiguration
This module initializes the default configuration of the Ranorex Studio/Jira integration.
The following variables must be set:
  * JiraUserName (in case of cloud, use the user's email address)
  * JiraServerURL
  * JiraPassword
  * EnableJiraIntegration
      
  * JiraProjectKey
  * JiraIssueType
  * StateClosed (desired issue state in case of a successful test case)
  * StateReopen (desired issue state in case of an already existing issue and a failed test case)

__Important:__
The default value of `EnableJiraIntegration` is `false`. Please specify a Boolean value (True,False) to enable it. Otherwise, a warning will appear in the report. This parameter is intended to enable the plugin during CI usage and keep it disabled when maintaining or extending the test suite.

*Other variables:*
  * JiraSummary
    Default value for the `summary` string
  * JiraDescription
    Default value for the `description` string
  * JiraLabels
    Default values for the issue's `labels` (multiple labels must be separated with a `;`)
  * jiraIssuePriority
    Default value for the issue `priority`
  * jiraEnvironment
    Default value for the `environment` field
  * jqlQueryToConnectIssues
    Default JQL query to find issues

**Special field:**
  * RxAutomationFieldName
    This fields enables automatic issue treatment by Ranorex Studio. In case of a failed test, Ranorex Studio searches if an issue exists where the test case name matches the content of this field of a Jira issue. If an issue is found, it is reopened. Otherwise, a new issue is created and the test case name is stored in the issue within this field, so that Ranorex Studio can associate the issue with the test case later on.

    __Important:__ A `custom field` with its name matching this variables value must be created in Jira manually!

This configuration is available throughout the whole project. Any re-initialization overwrites the values for any subsequent usage in the Ranorex Studio project.


#### AutoHandleJiraIntegration
This module interacts with Jira during test execution and creates/re-opens/closes issues based on the test case state. In case of a failed test, an issue is created if no issue key is provided, no issue with a matching `RxAutomation` field is found, or the JQL query delivered no issues. If an issue was found, the issue is re-opened.
In case of a successful test case, any existing associated issues are closed. If no issues are associated, no command is executed on the Jira end.


## User code library
The integration comes with a user code library to modify the values specified in the default configuration at runtime.
The following user code methods exist in the `JiraFieldHelper` class:
  * addCustomFieldValue
  * removeCustomFieldValue
  * addCustomCascadingField
  * removeCustomCascadingFieldValue
  * getCurrentSprintId
  * getCurrentSprintName
  * addNewLineToDescription
  * clearDescription
  * setEnvironment
  * setAssignee
  * setDueDate
  * setFixVersions (separated by `;`)
  * addFixVersions
  * removeFixVersions
  * setAffectsVersions (separated by `;`)
  * addAffectsVersions
  * removeAffectsVersions
  * setLabels (separated by `;`)
  * addNewLabel
  * removeLabel
  * setJiraIssueKey
    *Please note:* This value is cleared after a module interacting with Jira is run (i.e. 'AutoHandleJiraIntegration')
  * setJiraJQLQuery

The `ValueExtractorHelper` class provides following utility functions:
  * updateSummary
  * updateDescriptionWithAllStepsMade



## FAQ
  * Does the integration support Jira on-premise and in the cloud?
    &rarr; The integration supports both instance types. For a cloud-based instance, please use the account's email address to connect Ranorex with Jira.
  * Which .NET framework is required?
    &rarr; Please use  .NET 4.5.2 or higher.
  * Can I customize or add workflows?
    Yes; the NuGet package contains the source code of the integration, so it's possible to extend or modify it as desired.
    *__Please note__: Such  modifications are not supported and may be overwritten if the package is updated!* 


## Legacy modules
The modules in the `legacy` folder are still in the package for backward compatibility. They will be removed soon, so please modify your Ranorex Studio project to use the new modules mentioned above.
