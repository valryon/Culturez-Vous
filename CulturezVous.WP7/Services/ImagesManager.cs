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
using CulturezVous.Data;

namespace CulturezVous.WP7.Services
{
    /// <summary>
    /// Smart management of images, depending on the phone theme
    /// </summary>
    public class ImagesManager
    {
        private enum Theme
        {
            Light,
            Dark
        }

        private Theme m_currentTheme, m_iconTheme;
        private string m_resourcesDirectory;
        private string m_contentDirectory;

        private ImagesManager()
        {
            m_currentTheme = (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"] == Visibility.Visible ? Theme.Light : Theme.Dark;

            // Icon get the theme other color 
            m_iconTheme = m_currentTheme == Theme.Light ? Theme.Dark : Theme.Light;

            m_resourcesDirectory = "/CulturezVous.WP7;component/Resources/Images/" + m_iconTheme.ToString() + "/";
            m_contentDirectory = "/Resources/Images/" + m_iconTheme.ToString() + "/";
        }

        #region Singleton

        private static ImagesManager m_Instance;
        public static ImagesManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new ImagesManager();
                }
                return m_Instance;
            }
        }

        #endregion

        public Uri SettingsIcon
        {
            get
            {
                return new Uri(m_resourcesDirectory + "settingsIcon.png", UriKind.RelativeOrAbsolute);
            }
        }

        public Uri MoreIcon
        {
            get
            {
                return new Uri(m_resourcesDirectory + "moreIcon.png", UriKind.RelativeOrAbsolute);
            }
        }

        public Uri Background
        {
            get
            {
                return new Uri("/CulturezVous.WP7;component/Resources/Images/Backgrounds/" + m_currentTheme +".jpg", UriKind.RelativeOrAbsolute);
            }
        }

        public Uri LockIcon
        {
            get
            {
                return new Uri(m_resourcesDirectory + "lockIcon.png", UriKind.RelativeOrAbsolute);
            }
        }

        public Uri UnlockIcon
        {
            get
            {
                return new Uri(m_resourcesDirectory + "unlockIcon.png", UriKind.RelativeOrAbsolute);
            }
        }

        public Uri FavoriteIcon
        {
            get
            {
                return new Uri(m_contentDirectory + "favoriteIcon.png", UriKind.RelativeOrAbsolute);
            }
        }

        public Uri UnFavoriteIcon
        {
            get
            {
                return new Uri(m_contentDirectory + "unfavoriteIcon.png", UriKind.RelativeOrAbsolute);
            }
        }

        public Uri NewIcon
        {
            get
            {
                return new Uri(m_resourcesDirectory + "newIcon.png", UriKind.RelativeOrAbsolute);
            }
        }

        public Uri SearchIcon
        {
            get
            {
                return new Uri(m_contentDirectory + "searchIcon.png", UriKind.RelativeOrAbsolute);
            }
        }

        public Uri ShareIcon
        {
            get
            {
                return new Uri(m_contentDirectory + "shareIcon.png", UriKind.RelativeOrAbsolute);
            }
        }

        public Uri BackIcon
        {
            get
            {
                return new Uri(m_resourcesDirectory + "backIcon.png", UriKind.RelativeOrAbsolute);
            }
        }

        public Uri DefinitionIcon
        {
            get
            {
                return new Uri(m_resourcesDirectory + "definitionIcon.png", UriKind.RelativeOrAbsolute);
            }
        }

        public Uri AppBarBackIcon
        {
            get
            {
                return new Uri(m_contentDirectory + "appbar.back.rest.png", UriKind.RelativeOrAbsolute);
            }
        }

        public Uri AppBarNextIcon
        {
            get
            {
                return new Uri(m_contentDirectory + "appbar.next.rest.png", UriKind.RelativeOrAbsolute);
            }
        }
    }
}
