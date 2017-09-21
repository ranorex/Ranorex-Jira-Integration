/*
 * Created by Ranorex
 * User: sknopper
 * Date: 7/14/2017
 * Time: 9:23 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using WinForms = System.Windows.Forms;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Reporting;
using Ranorex.Core.Testing;

namespace JiraReporter
{
    /// <summary>
    /// Ranorex User Code collection. A collection is used to publish User Code methods to the User Code library.
    /// </summary>
    [UserCodeCollection]
    public class ValueExtractorHelper
    {
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static string getCurrentBrowser()
    	{
    		return "";
    	}
    	
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void updateSummary()
    	{ 		
    		ITestContainer tc = TestSuite.CurrentTestContainer;
    		string testCaseName = tc.Name;
    		
    		Ranorex.Core.Reporting.ActivityStack.Instance.VisitAll(activity => {
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
    		} else
    		{
    			ReportItem rItem = item as ReportItem;
    			return rItem.Message;
    		}
    	}
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void updateDescriptionWithAllStepsMade()
    	{
    		IList<JiraDescriptionItem> items = JiraConfiguration.Instance.JiraDescription;
    		items.Clear();
    		
    		ITestContainer tc = TestSuite.CurrentTestContainer;
    		string testCaseName = tc.Name;
    		
    		Ranorex.Core.Reporting.ActivityStack.Instance.VisitAll(activity => {
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
    			} else if (item.GetType().Name.Equals("ReportItem")) {
    				IList<JiraDescriptionItem> list = JiraConfiguration.Instance.JiraDescription;
    				JiraDescriptionItem newItem;
    				
    				string fileName = (item as ReportItem).ScreenshotFileName;
    				if (fileName != null && !fileName.Equals(""))
    				{
    					newItem = new JiraDescriptionItem((item as ReportItem).Message, fileName);
    				} else 
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
    		if (item.text.Contains("href=\"")) {
    			int startIndex = item.text.LastIndexOf("href=");
    			item.filePath = (item.text.Substring(startIndex)).Split('\"')[1];
    			item.text = "";
    		}
    	}
    }
}
