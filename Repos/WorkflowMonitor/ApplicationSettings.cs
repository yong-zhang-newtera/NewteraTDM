/*
* @(#)ApplicationSettings.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Xml.Serialization;

namespace Newtera.WorkflowMonitor
{
    /// <summary>
    /// This class is used to store user settings for monitoring
    /// </summary>
    /// <version> 1.0.0 03 Jan 2006</version>
    public class ApplicationSettings
    {
        //Set to true if the settings have changed since last saved
        private bool applicationSettingsChanged;
        private int pollingInterval = 5000;
        private bool autoSelectLatest = false;

        internal ApplicationSettings()
        {
            applicationSettingsChanged = false;
        }

        //Save app info to the config file
        internal void SaveSettings(string path)
        { 
            if (applicationSettingsChanged)
            {
                StreamWriter writer = null;
                XmlSerializer serializer = null;
                try
                {
                    // Create an XmlSerializer for the 
                    // ApplicationSettings type.
                    serializer = new XmlSerializer(typeof(ApplicationSettings));
                    writer = new StreamWriter(path, false);
                    // Serialize this instance of the ApplicationSettings 
                    // class to the config file.
                    serializer.Serialize(writer, this);
                }
                catch
                {
                }
                finally
                {
                    // If the FileStream is open, close it.
                    if (writer != null)
                    {
                        writer.Close();
                    }
                }
            }
        }

        //Load app info from the config file
        internal bool LoadAppSettings(string path)
        {
            XmlSerializer serializer = null;
            FileStream fileStream = null;
            bool fileExists = false;

            try
            {
                // Create an XmlSerializer for the ApplicationSettings type.
                serializer = new XmlSerializer(typeof(ApplicationSettings));
                FileInfo info = new FileInfo(path);
                // If the config file exists, open it.
                if (info.Exists)
                {
                    fileStream = info.OpenRead();
                    // Create a new instance of the ApplicationSettings by
                    // deserializing the config file.
                    ApplicationSettings applicationSettings = (ApplicationSettings)serializer.Deserialize(fileStream);
                    // Assign the property values to this instance of 
                    // the ApplicationSettings class.
                    this.pollingInterval = applicationSettings.pollingInterval;
                    this.autoSelectLatest = applicationSettings.autoSelectLatest;

                    fileExists = true;
                }
            }
            catch
            {
            }
            finally
            {
                // If the FileStream is open, close it.
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
            return fileExists;
        }

        public int PollingInterval
        {
            get { return pollingInterval; }
            set
            {
                if (value != pollingInterval)
                {
                    pollingInterval = value;
                    applicationSettingsChanged = true;
                }
            }
        }

        public bool ApplicationSettingsChanged
        {
            get { return applicationSettingsChanged; }
        }

        public bool AutoSelectLatest
        {
            get { return autoSelectLatest; }
            set
            {
                if (value != autoSelectLatest)
                {
                    autoSelectLatest = value;
                    applicationSettingsChanged = true;
                }
            }
        }
    }
}
