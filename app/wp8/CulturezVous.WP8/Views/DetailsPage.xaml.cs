using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using CulturezVous.Data;
using CulturezVous.WP8.Services;
using CulturezVous.WP8.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Diagnostics;
using CulturezVous.WP8.Utils;
using System.Windows.Media;
using Microsoft.Xna.Framework;
using System.ComponentModel;

namespace CulturezVous.WP8.Views
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        private DetailsPageViewModel m_viewModel;
        private List<int> m_dataList;
        private bool m_isPreviousEnable, m_isNextEnable;
        private DetailsPageViewModel m_previousViewModel, m_nextViewModel;
        private bool m_isFromLiveTile;
        private ListType m_type;

        public DetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            m_isFromLiveTile = false;
            m_type = ListType.Unknow;

            // Retrieve aimed id
            //------------------------------------------------------------------------------------
            int elementId = -128;

            // Retrieve the element ID to get its details
            if (NavigationContext.QueryString.Keys.Contains("id"))
            {
                var number = NavigationContext.QueryString["id"];

                try
                {
                    elementId = Convert.ToInt32(number);
                }
                catch (Exception) { }
            }

            if (elementId == -128)
            {
                errorGoBack();
            }
            else if (elementId == -1)
            {
                // Special id: element stored for live tile
                if (m_isFromLiveTile == false)
                {
                    m_isFromLiveTile = true;
                }
                elementId = Settings.LiveTileElementId;
            }

            // Cache not loaded (we're coming from tile) ? Load it
            if (CacheManager.Instance.IsInitialized == false)
            {
                CacheManager.Instance.Initialize(InitializeLevel.Update);
            }

            // Get the element from WS
            Element element = getElementFromId(elementId);

            // Create view models
            ElementViewModel elementVM = null;

            if (element is Word)
            {
                elementVM = new WordViewModel(element as Word);
            }
            else if (element is Contrepeterie)
            {
                var ctpVM = new ContrepeterieViewModel(element as Contrepeterie);
                ctpVM.IsSolutionRevealed = false;

                elementVM = ctpVM;

            }
            else
            {
                errorGoBack();
                return;
            }

            if (elementVM.IsRead == false) elementVM.IsRead = true;

            m_viewModel = new DetailsPageViewModel(elementVM);
            m_viewModel.Orientation = Orientation;

            // Load data fetcher
            //------------------------------------------------------------------------------------
            string fetcherType = "";
            string param = "";

            if (NavigationContext.QueryString.Keys.Contains("type"))
            {
                fetcherType = NavigationContext.QueryString["type"];

                if (NavigationContext.QueryString.Keys.Contains("param"))
                {
                    param = NavigationContext.QueryString["param"];
                }
            }
            else if (m_isFromLiveTile)
            {
                if (element is Word) fetcherType = ListType.Words.ToString();
                else fetcherType = ListType.Contrepeteries.ToString();
            }
            else
            {
                throw new ArgumentException("fetcher type");
            }

            m_dataList = getAllData((ListType)Enum.Parse(typeof(ListType), fetcherType, true), param);

            bufferNextAndPreviousPage(element.Id);

            // ------------------
            m_viewModel.IsRead = true;
            setDataContext(m_viewModel);

            // Mark as read if from tile
            if (m_isFromLiveTile)
            {
                BackgroundWorker bworker = new BackgroundWorker();
                bworker.DoWork += (o, args) =>
                {
                    // Delay
                    Thread.Sleep(1000);
                    Dispatcher.BeginInvoke(() =>
                    {
                        TileData.FindAndUpdateTile();
                    });
                };
                bworker.RunWorkerAsync();
            }

            // Dynamically load app bar icons
            updateAppBar();

            base.OnNavigatedTo(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (m_isFromLiveTile)
            {
                m_isFromLiveTile = false;
                // Go to list
                string uri = string.Format("/Views/ListPage.xaml?fromLiveTile=true&type=" + m_type);
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
            else
            {
                // From list ?
                if (NavigationService.BackStack.Where(b => b.Source.ToString().Contains("ListPage")).FirstOrDefault() != null)
                {
                    // Remove previous back entry
                    if (NavigationService.BackStack.Where(b => b.Source.ToString().Contains("List")).Count() > 0 && NavigationService.CanGoBack)
                    {
                        NavigationService.RemoveBackEntry();
                    }

                    string uri = string.Format("/Views/ListPage.xaml?fromLiveTile=true&type=" + m_type);
                    NavigationService.Navigate(new Uri(uri, UriKind.Relative));
                }
                else
                {
                    base.OnBackKeyPress(e);
                }
            }
        }

        private void setDataContext(DetailsPageViewModel viewModel)
        {
            DataContext = viewModel;

            // Mega hack !
            if (m_viewModel.ElementViewModel is ContrepeterieViewModel)
            {
                var ctpvm = m_viewModel.ElementViewModel as ContrepeterieViewModel;

                ThreadPool.QueueUserWorkItem(delegate
                {
                    Dispatcher.BeginInvoke(delegate
                    {
                        var grid = VisualChildFinder.FindVisualChild<Grid>(currentElementPResenter);
                        if (grid != null)
                        {
                            ctpvm.ShowSolutionAnimation = (Storyboard)grid.Resources["showSolutionAnimation"];
                        }
                    });
                });
            }
        }

        #region Data loading

        /// <summary>
        /// Bufferize previous and buffer detail page
        /// </summary>
        /// <param name="currentId"></param>
        private void bufferNextAndPreviousPage(int currentId)
        {
            int index = m_dataList.IndexOf(currentId);

            m_isPreviousEnable = (index > 0);
            m_isNextEnable = (index + 1 < m_dataList.Count);

            if (m_isPreviousEnable)
            {
                var previousElement = CacheManager.Instance.GetCacheElementFromId<Element>(m_dataList[index - 1]);
                ElementViewModel previousElementViewModel = null;

                if (previousElement is Word) previousElementViewModel = new WordViewModel(previousElement as Word);
                else if (previousElement is Contrepeterie) previousElementViewModel = new ContrepeterieViewModel(previousElement as Contrepeterie);

                m_previousViewModel = new DetailsPageViewModel(previousElementViewModel);
            }

            if (m_isNextEnable)
            {
                var nextElement = CacheManager.Instance.GetCacheElementFromId<Element>(m_dataList[index + 1]);
                ElementViewModel nextElementViewModel = null;

                if (nextElement is Word) nextElementViewModel = new WordViewModel(nextElement as Word);
                else if (nextElement is Contrepeterie) nextElementViewModel = new ContrepeterieViewModel(nextElement as Contrepeterie);

                m_nextViewModel = new DetailsPageViewModel(nextElementViewModel);
            }
        }

        private Element getElementFromId(int m_elementId)
        {
            Element element = null;

            element = CacheManager.Instance.GetCacheElementFromId<Element>(m_elementId);
            if (element == null)
            {
                MessageBox.Show("L'élément auquel vous tentez d'accéder n'a pas pu être chargé. Réessayer plus tard.", "Désolé, action impossible", MessageBoxButton.OK);
            }

            return element;
        }

        private List<int> getAllData(ListType type, string param)
        {
            this.m_type = type;

            switch (type)
            {
                case ListType.Words:
                    return CacheManager.Instance.GetCacheAll<Word>().Select(e => e.Id).ToList();

                case ListType.Contrepeteries:
                    return CacheManager.Instance.GetCacheAll<Contrepeterie>().Select(e => e.Id).ToList();

                case ListType.Favorites:
                    return CacheManager.Instance.GetCacheAll<Element>(true).Select(e => e.Id).ToList();

                case ListType.Florilege:
                    return CacheManager.Instance.GetCacheAll<Element>(null, true).Select(e => e.Id).ToList();

                default:
                    throw new ArgumentException("type");
            }
        }

        private void errorGoBack()
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        private void updateAppBar()
        {
            ApplicationBarIconButton btnFavorite = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            ApplicationBarIconButton btnShare = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            ApplicationBarIconButton btnBack = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            ApplicationBarIconButton btnNext = (ApplicationBarIconButton)ApplicationBar.Buttons[3];

            if (m_viewModel.IsFavorite)
            {
                btnFavorite.IconUri = ImagesManager.Instance.UnFavoriteIcon;
            }
            else
            {
                btnFavorite.IconUri = ImagesManager.Instance.FavoriteIcon;
            }

            btnShare.IconUri = ImagesManager.Instance.ShareIcon;
            btnBack.IconUri = ImagesManager.Instance.AppBarBackIcon;
            btnBack.IsEnabled = m_isPreviousEnable;
            btnNext.IconUri = ImagesManager.Instance.AppBarNextIcon;
            btnNext.IsEnabled = m_isNextEnable;
        }

        private void loadPreviousElement()
        {
            m_viewModel = m_previousViewModel;
            bufferNextAndPreviousPage(m_previousViewModel.ElementViewModel.Id);

            m_viewModel.IsRead = true;
            setDataContext(m_viewModel);

            updateAppBar();
        }

        private void loadNextElement()
        {
            m_viewModel = m_nextViewModel;
            bufferNextAndPreviousPage(m_nextViewModel.ElementViewModel.Id);

            m_viewModel.IsRead = true;
            setDataContext(m_viewModel);

            updateAppBar();
        }

        #endregion

        #region AppBar events

        private void favoriteButton_Click(object sender, EventArgs e)
        {
            m_viewModel.ElementViewModel.IsFavorite = !m_viewModel.ElementViewModel.IsFavorite;

            // Thanks to the powerful data context, the favorite modification is alredy reported in tables
            CacheManager.Instance.SaveToDb();

            updateAppBar();
        }

        private void shareButton_Click(object sender, EventArgs e)
        {
            ShareStatusTask shareStatusTask = new ShareStatusTask();
            shareStatusTask.Status = m_viewModel.ElementViewModel.ShareStatus;
            shareStatusTask.Show();
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            playChangeElementAnimation(-1, 0.2f);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            playChangeElementAnimation(1, 0.2f);
        }

        #endregion

        #region Pan gesture Left/right

        // Left / right pan gesture
        private System.Windows.Point m_panTranslation;
        private System.Windows.Point m_panOrigin;

        private void element_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            m_panTranslation = new System.Windows.Point();
            m_panOrigin = e.ManipulationContainer.TransformToVisual(LayoutRoot).Transform(new System.Windows.Point(0, 0));
        }

        private void element_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            m_panTranslation.X += e.DeltaManipulation.Translation.X;
            m_panTranslation.Y += e.DeltaManipulation.Translation.Y;

            System.Windows.Point current = m_panOrigin;
            current.X += m_panTranslation.X;

            if ((m_panTranslation.X > 0 && m_isPreviousEnable) || (m_panTranslation.X < 0 && m_isNextEnable))
                LayoutRoot.SetValue(Canvas.LeftProperty, current.X);
        }

        private void element_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            bool cancelGesture = true;

            // Considere gesture as canceled if too short
            if ((Math.Abs(m_panTranslation.X) >= LayoutRoot.ActualWidth * 0.35f) || (Math.Abs(e.FinalVelocities.LinearVelocity.X) > 1500))
            {
                int direction = 0;
                if (m_panTranslation.X < 0 && m_isNextEnable)
                {
                    cancelGesture = false;
                    direction = 1;

                }
                else if (m_panTranslation.X > 0 && m_isPreviousEnable)
                {
                    cancelGesture = false;
                    direction = -1;
                }

                if (direction != 0)
                {
                    playChangeElementAnimation(direction, 0.2f);
                }
            }

            if (cancelGesture)
            {
                playCancelChangeAnimation(0.3f, new BackEase()
                {
                    EasingMode = EasingMode.EaseOut
                });
            }
        }

        private void playChangeElementAnimation(int direction, double durationSec)
        {
            // Create animation
            Duration duration = new Duration(TimeSpan.FromSeconds(durationSec));

            Storyboard storyboard = new Storyboard();
            storyboard.Duration = duration;

            DoubleAnimation animation = new DoubleAnimation();
            animation.EasingFunction = new PowerEase()
            {
                EasingMode = EasingMode.EaseOut
            };

            animation.Duration = duration;

            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, LayoutRoot);

            // Set the attached properties of Canvas.Left and Canvas.Top
            // to be the target properties of the two respective DoubleAnimations.
            Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));

            if (direction > 0)
            {
                animation.To = -LayoutRoot.ActualWidth;
                storyboard.Completed += new EventHandler((o, s) =>
                {
                    // Left gesture = next
                    loadNextElement();

                    // Make as if the canvas is infinite
                    LayoutRoot.SetValue(Canvas.LeftProperty, LayoutRoot.ActualWidth);

                    // Animation to show the next element
                    playCancelChangeAnimation(0.2f, new PowerEase()
                    {
                        EasingMode = EasingMode.EaseOut
                    });
                });
            }
            else if (direction < 0)
            {
                animation.To = LayoutRoot.ActualWidth;
                storyboard.Completed += new EventHandler((o, s) =>
                {
                    // Right gesture = previous
                    loadPreviousElement();

                    // Make as if the canvas is infinite
                    LayoutRoot.SetValue(Canvas.LeftProperty, -LayoutRoot.ActualWidth);

                    // Animation to show the previous element
                    playCancelChangeAnimation(0.2f, new PowerEase()
                    {
                        EasingMode = EasingMode.EaseOut
                    });
                });
            }

            storyboard.Begin();
        }

        private void playCancelChangeAnimation(double durationSec, IEasingFunction easeFunction)
        {
            // Create animation
            Duration duration = new Duration(TimeSpan.FromSeconds(durationSec));

            Storyboard storyboard = new Storyboard();
            storyboard.Duration = duration;

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = duration;
            animation.EasingFunction = easeFunction;

            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, LayoutRoot);

            // Set the attached properties of Canvas.Left and Canvas.Top
            // to be the target properties of the two respective DoubleAnimations.
            Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));

            animation.To = 0;

            storyboard.Begin();
        }

        #endregion
    }
}