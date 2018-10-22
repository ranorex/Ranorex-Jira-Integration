# Ranorex Jira Integration

## What's in this repository?

This repository contains the source code for the Ranorex.Jira nuget package, which provides the integration of Ranorex into Jira. It relies on the Atlassian.SDK nuget package and utilizes Jira's REST API.

# Workflows & Features

**The integration provides support for the following workflows:**
  * Creating a new issue if a test case failed
  * Re-open existing issue(s) if a test case failed
  * Close a existing issue(s) if a test case succeeded
  * Automatically create, re-open, or close an issue if a custom field, storing the Ranorex test case name, exists.

**Provided features:**
  * Support for custom fields and cascading custom fields
  * JQL supported for advanced queries
  * On premise and cloud Jira instance
  * Custom modules
  * Enable only during CU/command line execution
  * Custom close or re-open states

## Modules
#### InitializeDefaultConfiguration
This module initializes the default configuration of the Ranorex/Jira integration.
The following variables are required to be set:
  * JiraUserName (in case of cloud, use the user's email address)
  * JiraServerURL
  * JiraPassword
  * EnableJiraIntegration
      
  * JiraProjectKey
  * JiraIssueType
  * StateClosed (desired issue state in case of a successful test case)
  * StateReopen (desired issue state in case of an already existing issue and a failed test case)

__Important:__
The default value of `EnableJiraIntegration` is `false`. Please specify a boolean value (True,False) to enable it. Otherwise a warning will appear in the report. This parameter is intended to enable the plugin during CI usage and keep it disabled when maintaining or extending the test suite.

*Other variables:*
  * JiraSummary
    Default value for the `summary` string
  * JiraDescription
    Default value for the `description` string
  * JiraLabels
    Default values for the issue's `labels` (multiple labels have to be seperated with a `;`)
  * jiraIssuePriority
    Default value for the issue `priority`
  * jiraEnvironment
    Default value for the `environment` field
  * jqlQueryToConnectIssues
    Default JQL query to find issues

**Special field:**
  * RxAutomationFieldName
    This fields enables an automatic issue treatment by Ranorex. In case of a failed test, Ranorex searches if an issue exists, where the test case name matches the content of this field of a Jira issue. If an issue if found, it gets reopened. Otherwise, a new issue is created and the test case name is stored in the issue within this field, so Ranorex can associate the issue with the test case later on.

    __Important:__ A `custom field` with its name matching this variables value must be created in Jira manually!

This configuration is available throughout the whole project. Any re-initialization overwrites the values for any subsequent usage in the Ranorex project.


#### AutoHandleJiraIntegration
This module interacts with Jira during test execution and creates/re-opens/closes issues based on the test case state. In case of a failed test, an issue is created, if no issue key is provided, no issue with a matching `RxAutomation` field is found, or the JQL query delivered no issues. If an issue was found, the issue is re-opened.
In case of a succeeded test case, existing associated issues are closed. If no issues are associated, no command is executed on the Jira end.


## User code library
The integration comes with a user code library to adopt the values specified in the default configuration at runtime.
Following user code methods exist in the `JiraFieldHelper` class:
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
    *Please note:* This value is cleared after a module interacting with Jira (like 'AutoHandleJiraIntegration') is run!
  * setJiraJQLQuery

The `ValueExtractorHelper` class provides following utility functions:
  * updateSummary
  * updateDescriptionWithAllStepsMade



## FAQ
  * Does the integration support Jira on premise and cloud?
    &rarr; The integration supports both instance types. In case of cloud, please use the account's email address to connect Ranorex with Jira.
  * Which .NET framework is required?
    &rarr; Please use  .NET 4.5.2 or higher.
  * Can I customize or add workflows?
    Yes; the nuget package contains the source code of the integration. It's thus possible to extend or modify it accordingly.
    *__Please note__: Such  modifications are not supported and can't be taken into account in case of an upgrade of the package!* 


## Legacy modules
The modules in the `legacy` folder are still in the package for compatibility reasons. They will get removed soon, so please adopt your project to the new modules mentioned above.