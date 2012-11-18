using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using CulturezVous.Data;
using CulturezVous.WP8.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CulturezVous.WP8.Views
{
    /// <summary>
    /// Main page of the application, showing the main information
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        private MainPageViewModel m_mainPageViewModel;

        public MainPage()
        {
            // Loading
            ProgressIndicator progressIndicator = new ProgressIndicator();
            progressIndicator.IsIndeterminate = true;
            progressIndicator.IsVisible = true;
            progressIndicator.Text = "chargement des données...";

            SystemTray.SetProgressIndicator(this, progressIndicator);

            m_mainPageViewModel = new MainPageViewModel(true);
            m_mainPageViewModel.Orientation = Orientation;

            m_mainPageViewModel.LoadingFailed += new Action(() =>
            {
                Dispatcher.BeginInvoke(() =>
               {
                   progressIndicator.IsVisible = false;
               });
            });

            m_mainPageViewModel.LoadingComplete += new Action(() =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (Settings.IsFirstLaunch)
                    {
                        Settings.IsFirstLaunch = false;

                        if (App.IsLowMemoryDevice == false)
                        {
                            var result = MessageBox.Show("Culturez-Vous vous propose d'ajouter une tuile sur votre page d'accueil pour recevoir chaque jour les nouveautés.\n\nVoulez-vous ajouter cette tuile maintenant ? \nVous pourrez toujours le faire plus tard dans les paramètres.", "Bienvenue !", MessageBoxButton.OKCancel);
                            if (result == MessageBoxResult.OK)
                            {
                                var p = new ParametersPageViewModel();
                                p.UpdateLiveTile();
                            }
                        }
                    }

                    progressIndicator.IsVisible = false;
                });
            });

            DataContext = m_mainPageViewModel;

            InitializeComponent();
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            m_mainPageViewModel.Orientation = Orientation;
            base.OnOrientationChanged(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            wordsListBox.SelectedIndex = -1;
            CTPListBox.SelectedIndex = -1;

            // Play animation for more
            if (panorama.SelectedItem == wordsPanoramaItem)
            {
                resetMoreAnimationWords.Begin();
            }
            else if (panorama.SelectedItem == ctpsPanoramaItems)
            {
                resetMoreAnimationCTPs.Begin();
            }
            else
            {
                resetSettingsAnimation.Begin();
            }

            m_mainPageViewModel.CheckForUpdate();

            base.OnNavigatedTo(e);
        }

        #region Events management


        private void otherListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedItem = ((ListBox)sender).SelectedItem;

            if (selectedItem == null) return;

            if (selectedItem is ElementViewModel)
            {
                var elementVM = selectedItem as ElementViewModel;
                string uri = null;

                if (elementVM is WordViewModel)
                {
                    uri = m_mainPageViewModel.SelectWord(elementVM as WordViewModel);
                }
                else if (elementVM is ContrepeterieViewModel)
                {
                    uri = m_mainPageViewModel.SelectContrepeterie(elementVM as ContrepeterieViewModel);
                }

                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
        }

        private void moreWordsPanel_Click(object sender, RoutedEventArgs e)
        {
            showMoreAnimationWords.Begin();

            Dispatcher.BeginInvoke(() =>
            {
                Thread.Sleep(250);

                string uri = string.Format("/Views/ListPage.xaml?type=" + ListType.Words);
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            });
        }

        private void moreCTPPanel_Click(object sender, RoutedEventArgs e)
        {
            showMoreAnimationCTPs.Begin();

            Dispatcher.BeginInvoke(() =>
            {
                Thread.Sleep(250);
                string uri = string.Format("/Views/ListPage.xaml?type=" + ListType.Contrepeteries);
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            });
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            showSettingsAnimation.Begin();

            Dispatcher.BeginInvoke(() =>
            {
                Thread.Sleep(250);
                string uri = string.Format("/Views/ParametersPage.xaml");
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            });
        }

        #endregion

    }
}