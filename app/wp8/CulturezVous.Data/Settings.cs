using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CulturezVous.Data
{
    /// <summary>
    /// Application settings, stored in local storage or static access
    /// </summary>
    public static class Settings
    {
        private static SettingsData m_instance;
        private static string containerName = "settings";

        static Settings()
        {
            if (System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.TryGetValue<SettingsData>(containerName, out m_instance) == false)
            {
                m_instance = new SettingsData();
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Add(containerName, m_instance);
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public static void Update()
        {
            System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings[containerName] = m_instance;
            System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public static bool IsFirstLaunch
        {
            get { return m_instance.IsFirstLaunch; }
            set
            {
                m_instance.IsFirstLaunch = value;
                Update();
            }
        }

        public static bool IsBackgroundAgentAuthorized
        {
            get { return m_instance.IsBackgroundAgentAuthorized; }
            set
            {
                m_instance.IsBackgroundAgentAuthorized = value;
                Update();
            }
        }

        /// <summary>
        /// Id of the element presented in the live tile
        /// </summary>
        public static int LiveTileElementId
        {
            get { return m_instance.LiveTileElementId; }
            set
            {
                m_instance.LiveTileElementId = value;
                Update();
            }
        }

        public static string AppName
        {
            get
            {
                return "Culturez-Vous !";
            }
        }
        public static string SchedulerDesc
        {
            get
            {
                return "Culturez-Vous vous tient informés des dernières trouvailles disponibles : mots désuets amusants ou contrepèteries tous les dimanches.";
            }
        }
        public static string WebserviceUrl
        {
            get
            {
                return "http://thegreatpaperadventure.com/CulturezVous/index.php/";
            }
        }
        public static string WebsiteUrl
        {
            get
            {
                return "http://www.culturez-vous.fr";
            }
        }
    }

    /// <summary>
    /// Not static data to save
    /// </summary>
    public class SettingsData
    {
        public bool IsBackgroundAgentAuthorized { get; set; }
        public int LiveTileElementId { get; set; }
        public bool IsFirstLaunch { get; set; }
        public Guid UserId { get; set; }

        public SettingsData()
        {
            IsBackgroundAgentAuthorized = true;
            LiveTileElementId = -1;
            UserId = new Guid();
            IsFirstLaunch = true;
        }
    }
}
