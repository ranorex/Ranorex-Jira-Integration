/*
 * Created by Ranorex
 * User: sknopper
 * Date: 7/14/2017
 * Time: 9:15 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace JiraReporter
{
    /// <summary>
    /// Ranorex User Code collection. A collection is used to publish User Code methods to the User Code library.
    /// </summary>
    [UserCodeCollection]
    public class JiraFieldHelper
    {
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void addCustomFieldValue(string fieldName, string fieldValue)
    	{
			JiraConfiguration config = JiraConfiguration.Instance;
			removeCustomFieldValue(fieldName);
			config.customFields.Add(fieldName, fieldValue);
    	}
    	
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void removeCustomFieldValue(string fieldName)
    	{
    		JiraConfiguration config = JiraConfiguration.Instance;
    		if (config.customFields.ContainsKey(fieldName)) {
				config.customFields.Remove(fieldName);
			} 
    	}
    	
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static string getCurrentSprintId()
    	{
    		return JiraReporter.getCurrentSprintItem("id");
    	}
    	
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static string getCurrentSprintName()
    	{
    		return JiraReporter.getCurrentSprintItem("name");
    	}
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void addNewLineToDescription(string line)
    	{
    		JiraConfiguration config = JiraConfiguration.Instance;
    		config.JiraDescription.Add(new JiraDescriptionItem("\r\n " + line, null));
    	}
    	
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void setEnvironment(string environment)
    	{
    		JiraConfiguration config = JiraConfiguration.Instance;
    		config.JiraEnvironment = environment;
    	}
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void setLabels(string labels)
    	{
    		JiraConfiguration config = JiraConfiguration.Instance;
    		config.JiraLabels = labels;
    	}
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void addNewLabel(string label)
    	{
    		JiraConfiguration config = JiraConfiguration.Instance;
    		config.JiraLabels += ";" + label;
    	}
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void removeLabel(string label)
    	{
    		JiraConfiguration config = JiraConfiguration.Instance;
    		string oldLabels = config.JiraLabels;
    		string[] labels = oldLabels.Split(';');
    		string newLabels = "";
    		foreach (string lab in labels) {
    			if (! newLabels.Equals("")) {
    				newLabels += ";";
    			}
    			if (!lab.Equals(label)) {
    				newLabels += lab;
    			}
    		}
    		
    		JiraFieldHelper.setLabels(newLabels);
    	}
    }
}
