using System.Linq;
using System.Collections.Generic;

using Ranorex;
using Ranorex.Core.Reporting;
using Ranorex.Core.Testing;

namespace JiraReporter
{
  [UserCodeCollection]
  public class ValueExtractorHelper
  {

    /// <summary>
    /// This methods updates the summary with the failed action in the current test case.
    /// </summary>
    [UserCodeMethod]
    public static void updateSummary()
    {
      ITestContainer tc = TestSuite.CurrentTestContainer;
      string testCaseName = tc.Name;

      Ranorex.Core.Reporting.ActivityStack.Instance.VisitAll(activity =>
      {
        if ((activity.GetType().Name.Equals("TestContainerActivity")
            && activity.Status == ActivityStatus.Failed
            && (activity as ITestContainerActivity).ContainerName.Equals(testCaseName))
                 )
        {
          JiraConfiguration config = JiraConfiguration.Instance;
          config.JiraSummary = getFailedAction(activity);
        }
        return true;
      });
    }

    private static string getFailedAction(IActivity activity)
    {
      object item = null;
      IList<IReportItem> list = activity.Children;
      item = list.Select(x => x).Where(x => !x.GetType().Name.Equals("TestSetupTeardownContainerActivity")).Last();

      if (item.GetType().Name.Equals("TestModuleActivity"))
      {
        return getFailedAction(item as IActivity);
      }
      else
      {
        ReportItem rItem = item as ReportItem;
        return rItem.Message;
      }
    }

    /// <summary>
    /// This methods adds all executed steps in the current test case as text to the description and also upload screenshots.
    /// </summary>
    [UserCodeMethod]
    public static void updateDescriptionWithAllStepsMade()
    {
      IList<JiraDescriptionItem> items = JiraConfiguration.Instance.JiraDescription;
      items.Clear();

      ITestContainer tc = TestSuite.CurrentTestContainer;
      string testCaseName = tc.Name;

      Ranorex.Core.Reporting.ActivityStack.Instance.VisitAll(activity =>
      {
        if ((activity.GetType().Name.Equals("TestContainerActivity")
            && activity.Status == ActivityStatus.Failed
            && (activity as ITestContainerActivity).ContainerName.Equals(testCaseName))
                 )
        {
          fillDescription(activity);
        }
        return true;
      });
    }

    private static void fillDescription(IActivity activity)
    {
      foreach (object item in activity.Children)
      {
        Report.Log(ReportLevel.Info, item.GetType() + " ; " + activity.Children.Count);
        if (item.GetType().Name.Equals("TestModuleActivity"))
        {
          fillDescription(item as IActivity);
        }
        else if (item.GetType().Name.Equals("ReportItem"))
        {
          IList<JiraDescriptionItem> list = JiraConfiguration.Instance.JiraDescription;
          JiraDescriptionItem newItem;

          string fileName = (item as ReportItem).ScreenshotFileName;
          if (fileName != null && !fileName.Equals(""))
          {
            newItem = new JiraDescriptionItem((item as ReportItem).Message, fileName);
          }
          else
          {
            newItem = new JiraDescriptionItem((item as ReportItem).Message, null);
          }

          extractFilePathforInlineImage(newItem);

          JiraConfiguration.Instance.JiraDescription.Add(newItem);
        }
      }
    }

    private static void extractFilePathforInlineImage(JiraDescriptionItem item)
    {
      if (item.text.Contains("href=\""))
      {
        int startIndex = item.text.LastIndexOf("href=");
        item.filePath = (item.text.Substring(startIndex)).Split('\"')[1];
        item.text = "";
      }
    }
  }
}
