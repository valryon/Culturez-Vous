using System;
using System.ComponentModel;
using System.Windows;
using CulturezVous.WP8.Services;

namespace CulturezVous.WP8.Utils
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Execute some code on the UI Thread
        /// </summary>
        public void ExecuteUI(Action action)
        {
            Deployment.Current.Dispatcher.BeginInvoke(action);
        }

        #region Orientation

        private Microsoft.Phone.Controls.PageOrientation m_orientation;
        public Microsoft.Phone.Controls.PageOrientation Orientation
        {
            get
            {
                return m_orientation;
            }
            set
            {
                m_orientation = value;
                RaisePropertyChanged("Orientation");
                RaisePropertyChanged("ScrollingState");
            }
        }
        public System.Windows.Controls.ScrollBarVisibility ScrollingState
        {
            get
            {
                bool landscape = Orientation == Microsoft.Phone.Controls.PageOrientation.Landscape
                    || Orientation == Microsoft.Phone.Controls.PageOrientation.LandscapeLeft
                    || Orientation == Microsoft.Phone.Controls.PageOrientation.LandscapeRight;

                if (landscape) return System.Windows.Controls.ScrollBarVisibility.Auto;
                else return System.Windows.Controls.ScrollBarVisibility.Disabled;
            }
        }

        #endregion

        #region Common texts

        public string ApplicationTitle
        {
            get
            {
                return "CULTUREZ-VOUS !";
            }
        }

        #endregion

        #region Images

        public Uri SettingsIconUri
        {
            get
            {
                return ImagesManager.Instance.SettingsIcon;
            }
        }

        public Uri MoreUri
        {
            get
            {
                return ImagesManager.Instance.MoreIcon;
            }
        }

        public Uri FavoriteIconUri
        {
            get
            {
                return ImagesManager.Instance.FavoriteIcon;
            }
        }

        public Uri NewIconUri
        {
            get
            {
                return ImagesManager.Instance.NewIcon;
            }
        }

        public Uri SearchIconUri
        {
            get
            {
                return ImagesManager.Instance.SearchIcon;
            }
        }

        public Uri ShareIconUri
        {
            get
            {
                return ImagesManager.Instance.ShareIcon;
            }
        }

        public Uri BackIconUri
        {
            get
            {
                return ImagesManager.Instance.BackIcon;
            }
        }

        public Uri DefinitionIconUri
        {
            get
            {
                return ImagesManager.Instance.DefinitionIcon;
            }
        }

        public Uri AppBarBackIconUri
        {
            get
            {
                return ImagesManager.Instance.AppBarBackIcon;
            }
        }

        public Uri AppBarNextIconUri
        {
            get
            {
                return ImagesManager.Instance.AppBarNextIcon;
            }
        }

        public Uri LockIconUri
        {
            get
            {
                return ImagesManager.Instance.LockIcon;
            }
        }

        public Uri UnlockIconUri
        {
            get
            {
                return ImagesManager.Instance.UnlockIcon;
            }
        }

        #endregion
    }
}