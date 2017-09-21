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
    	public static void addCustomCascadingField(string fieldName, string parentValue, string childValue)
    	{
    		JiraConfiguration config = JiraConfiguration.Instance;
			removeCustomFieldValue(fieldName);
			string[] values = {parentValue, childValue};
			config.customCascadingFields.Add(fieldName, values);
    	}
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void removeCustomCascadingFieldValue(string fieldName)
    	{
    		JiraConfiguration config = JiraConfiguration.Instance;
    		if (config.customCascadingFields.ContainsKey(fieldName)) {
				config.customCascadingFields.Remove(fieldName);
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
    	
		[UserCodeMethod]
    	public static void setFixVersions(string versions)
    	{
    		JiraConfiguration config = JiraConfiguration.Instance;
    		config.FixVersions = versions;
    	}
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void addFixVersions(string version)
    	{
    		JiraConfiguration config = JiraConfiguration.Instance;
    		config.FixVersions += ";" + version;
    	}
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void removeFixVersions(string version)
    	{
    		JiraConfiguration config = JiraConfiguration.Instance;
    		string oldVersions = config.FixVersions;
    		string[] versions = oldVersions.Split(';');
    		string newVersions = "";
    		foreach (string ver in versions) {
    			if (! newVersions.Equals("")) {
    				newVersions += ";";
    			}
    			if (!ver.Equals(version)) {
    				newVersions += ver;
    			}
    		}
    		
    		JiraFieldHelper.setFixVersions(newVersions);
    	}
		
    	[UserCodeMethod]
    	public static void setAffectsVersions(string versions)
    	{
    		JiraConfiguration config = JiraConfiguration.Instance;
    		config.AffectsVersions = versions;
    	}
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void addAffectsVersions(string version)
    	{
    		JiraConfiguration config = JiraConfiguration.Instance;
    		config.AffectsVersions += ";" + version;
    	}
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void removeAffectsVersions(string version)
    	{
    		JiraConfiguration config = JiraConfiguration.Instance;
    		string oldVersions = config.AffectsVersions;
    		string[] versions = oldVersions.Split(';');
    		string newVersions = "";
    		foreach (string ver in versions) {
    			if (! newVersions.Equals("")) {
    				newVersions += ";";
    			}
    			if (!ver.Equals(version)) {
    				newVersions += ver;
    			}
    		}
    		
    		JiraFieldHelper.setAffectsVersions(newVersions);
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
