/*
 * Created by Ranorex
 * User: sknopper
 * Date: 7/10/2017
 * Time: 10:14 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace JiraReporter
{
	/// <summary>
	/// Description of JiraConfiguration.
	/// </summary>
	public class JiraConfiguration
	{
		private static JiraConfiguration instance;
		
        public string JiraProjectKey { get; set; }
        public string JiraIssueType { get; set; }
        public string JiraSummary { get; set; }
        public IList<JiraDescriptionItem> JiraDescription { get; set; } = new List<JiraDescriptionItem>();
        public string JiraLabels { get; set; }
        public string StateClosed { get; set; }
        public string StateReopen { get; set; }
        public string RxAutomationFieldName { get; set; }
        public string jqlQueryToConnectIssues { get; set; }
        public string JiraIssuePriority { get; set; }
        public Dictionary<string, string> customFields = new Dictionary<string, string>();

		private JiraConfiguration()
		{
		}
		
		public static JiraConfiguration Instance {
			get
			{
				if (instance == null) {
					instance = new JiraConfiguration();
				}
				return instance;
			}
		}
		
		public List<string> getAllLabels() {
			char delimiterChar = ';';
            return new List<string>(JiraLabels.Split(delimiterChar));
		}
	}
}
