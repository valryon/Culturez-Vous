using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CulturezVous.Data;
using CulturezVous.WP7.Services;
using CulturezVous.WP7.Utils;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.GamerServices;
using System.Windows;

namespace CulturezVous.WP7.ViewModels
{
    /// <summary>
    /// View model for the parameters page
    /// </summary>
    public class ParametersPageViewModel : ViewModelBase
    {
        public ParametersPageViewModel()
            : base()
        {

        }

        public void UpdateLiveTile()
        {
            ShellTile tileToFind = findTile();

            if (findTile() != null)
            {
                tileToFind.Delete();
            }

            // Get today element
            Word word = CacheManager.Instance.GetCacheLast<Word>();
            Contrepeterie ctp = CacheManager.Instance.GetCacheLast<Contrepeterie>();

            Element element = null;

            if (word.Date > ctp.Date) element = word;
            else element = ctp;

            int unreadElementCount = CacheManager.Instance.Elements.Where(e => e.IsRead == false).Count();

            // Create the tile is required
            TileCreator.CreateTile(element, unreadElementCount);

            RaisePropertyChanged("LiveTileIsSet");
            RaisePropertyChanged("SwitchedIsChecked");
            RaisePropertyChanged("LiveTileSetCaption");
        }

        private static ShellTile findTile()
        {
            ShellTile tileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("/Views/DetailsPage"));
            return tileToFind;
        }

        public Visibility LowMemoryDeviceParametersVisibility
        {
            get
            {
                return App.IsLowMemoryDevice ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public bool IsBackgroundAgentAuthorized
        {
            get
            {
                return Settings.IsBackgroundAgentAuthorized;
            }
            set
            {
                if (App.IsLowMemoryDevice == false)
                {
                    Settings.IsBackgroundAgentAuthorized = value;

                    if (IsBackgroundAgentAuthorized == false)
                    {
                        var periodicTask = ScheduledActionService.Find(Settings.AppName) as PeriodicTask;
                        if (periodicTask != null)
                        {
                            ScheduledActionService.Remove(Settings.AppName);
                        }
                    }
                    else
                    {
                        var periodicTask = ScheduledActionService.Find(Settings.AppName) as PeriodicTask;
                        if (periodicTask == null)
                        {
                            periodicTask = new PeriodicTask(Settings.AppName) { Description = Settings.SchedulerDesc };
                            ScheduledActionService.Add(periodicTask);
                        }
                    }
                }
            }
        }

        public ICommand ShowWebsiteCommand
        {
            get
            {
                return new Command(() =>
                {
                    try
                    {
                        var task = new Microsoft.Phone.Tasks.WebBrowserTask()
                        {
                            Uri = new Uri(Settings.WebsiteUrl)
                        };

                        task.Show();
                    }
                    catch (Exception) { }
                });
            }
        }
    }
}
