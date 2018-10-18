/*
 * Created by Ranorex
 * User: sknopper
 * Date: 7/17/2017
 * Time: 2:04 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace JiraReporter
{
  /// <summary>
  /// Description of JiraDescriptionItem.
  /// </summary>
  public class JiraDescriptionItem
  {
    public string text { get; set; }
    public string filePath { get; set; }

    public JiraDescriptionItem(string text, string filePath)
    {
      this.text = text;
      this.filePath = filePath;
    }

    public string getValue()
    {
      if (isImageEntry())
      {
        return filePath;
      }

      return text;
    }

    public bool isImageEntry()
    {
      return filePath != null;
    }
  }
}
