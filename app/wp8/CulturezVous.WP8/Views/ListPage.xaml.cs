using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CulturezVous.WP8.Services;
using CulturezVous.WP8.Utils;
using CulturezVous.WP8.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using CulturezVous.Data;

namespace CulturezVous.WP8.Views
{
    public partial class ListPage : PhoneApplicationPage
    {
        private ListPageViewModel m_viewModel;
        private ListType m_type;

        public ListPage()
        {
            InitializeComponent();

            m_viewModel = new ListPageViewModel(true);

            wordsList.ListScrolled += new Action<ListBox, double>(ListScrolled);
            wordsList.ListEndScroll += (() =>
            {
                if (m_viewModel.IsLoading == false)
                {
                    m_viewModel.LoadMore(ListType.Words);
                }
            });

            ctpList.ListScrolled += new Action<ListBox, double>(ListScrolled);
            ctpList.ListEndScroll += (() =>
            {
                if (m_viewModel.IsLoading == false)
                {
                    m_viewModel.LoadMore(ListType.Contrepeteries);
                }
            });
            
            DataContext = m_viewModel;

            // Dynamically load app bar icons
            ApplicationBarIconButton btnSearch = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            btnSearch.IconUri = ImagesManager.Instance.SearchIcon;

            showSearch.Completed += new EventHandler(showSearch_Completed);
            hideSearch.Completed += new EventHandler(hideSearch_Completed);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            m_type = ListType.Unknow;

            if (NavigationContext.QueryString.Keys.Contains("fromLiveTile"))
            {
                m_viewModel.UpdateList(ListType.Words);
                m_viewModel.UpdateList(ListType.Contrepeteries);
                m_viewModel.UpdateList(ListType.Favorites);
                m_viewModel.UpdateList(ListType.Florilege);

                var fromLiveTile = Convert.ToBoolean(NavigationContext.QueryString["fromLiveTile"]);
                if (fromLiveTile && NavigationService.CanGoBack)
                {
                    NavigationService.RemoveBackEntry();
                }
            }

            if (NavigationContext.QueryString.Keys.Contains("type"))
            {
                var type = NavigationContext.QueryString["type"];
                m_type = (ListType)Enum.Parse(typeof(ListType), type, true);
            }

            Dispatcher.BeginInvoke(() =>
            {
                // Reinitialize lists
                wordsList.SelectedIndex = -1;
                ctpList.SelectedIndex = -1;
                favoritesList.SelectedIndex = -1;
                bestofList.SelectedIndex = -1;
            });

            base.OnNavigatedTo(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (m_viewModel.SearchPanelVisiblity == System.Windows.Visibility.Visible)
            {
                hideSearch.Begin();
            }
            else
            {
                base.OnBackKeyPress(e);
            }
        }

        private void updatePivotTitleWithFirstDisplayedElementFromCurrentList()
        {
            // It's not working with search item
            var item = ((PivotItem)pivotControl.SelectedItem).Content;
            if (item is ListBox)
            {
                var list = (ListBox)item;

                if (updatePivotTitleWithFirstDisplayedElementFromList(list) == false)
                {
                    PlayMogerAnimationOnTitle(((PivotItem)pivotControl.SelectedItem).Header.ToString());
                }
            }
        }

        private bool updatePivotTitleWithFirstDisplayedElementFromList(ListBox sender)
        {
            if (sender.Items.Count > 0)
            {
                // Get the position of the first item of a listbox, relative to the pivot
                ListBoxItem selectedItem = sender.ItemContainerGenerator.ContainerFromItem(sender.Items[0]) as ListBoxItem;

                if (selectedItem != null)
                {
                    double heightItem = selectedItem.ActualHeight;
                    double widthItem = selectedItem.ActualWidth;

                    // Get the listbox position
                    var point = sender.TransformToVisual(pivotControl).Transform(new Point(0, 0));

                    // Compute the first item position from the pivot control point of view
                    Point lookPoint = new Point(point.X + (widthItem / 2), point.Y + heightItem / 2);

                    // And just get the first Listboxitem from here if available
                    var firstElements = VisualTreeHelper.FindElementsInHostCoordinates(lookPoint, pivotControl);

                    foreach (var element in firstElements)
                    {
                        if (element is ListBoxItem)
                        {
                            var item = (ListBoxItem)element;
                            var datacontext = item.DataContext as ElementViewModel;

                            if (datacontext != null)
                            {
                                PlayMogerAnimationOnTitle(datacontext.Title);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void PlayMogerAnimationOnTitle(string newTitle)
        {

            if (m_viewModel.PivotTitle != newTitle)
            {
                // Play animation
                var tb = VisualChildFinder.FindVisualChild<TextBlock>(pivotControl);
                var hideAnimation = (Storyboard)tb.Resources["hideTitle"];
                var showAnimation = (Storyboard)tb.Resources["showTitle"];

                hideAnimation.Completed += new EventHandler((a, b) =>
                {
                    // Change text
                    m_viewModel.PivotTitle = newTitle;

                    // Replay an animation
                    showAnimation.Begin();
                });
                hideAnimation.Begin();

            }
        }

        #region Events

        // Pivot

        private void pivotControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Retrieve type if present
            if (m_type != ListType.Unknow)
            {
                // Focus on a category
                if (m_type == ListType.Words)
                {
                    pivotControl.SelectedIndex = 0;
                }
                else if (m_type == ListType.Contrepeteries)
                {
                    pivotControl.SelectedIndex = 1;
                }
                else if (m_type == ListType.Florilege)
                {
                    pivotControl.SelectedIndex = 3;
                }
                else if (m_type == ListType.Favorites)
                {
                    pivotControl.SelectedIndex = 2;
                }
            }
        }

        private void pivotControl_UnloadingPivotItem(object sender, PivotItemEventArgs e)
        {
            // Reset search if page change
            m_viewModel.ResetSearch();
            searchInputTb.Text = "";
        }

        private void pivotControl_LoadedPivotItem(object sender, PivotItemEventArgs e)
        {
            // If you want animation, you have to delay the display of the app bar
            ApplicationBar.IsVisible = true;

            // Type is currently loaded pivot
            if (pivotControl.SelectedItem == wordsPivotItem)
            {
                m_type = ListType.Words;
            }
            else if (pivotControl.SelectedItem == ctpsPivotItem)
            {
                m_type = ListType.Contrepeteries;
            }
            else if (pivotControl.SelectedItem == favoritesPivotItem)
            {
                m_type = ListType.Favorites;
            }
            else if (pivotControl.SelectedItem == bestofPivotItem)
            {
                m_type = ListType.Florilege;
            }

            m_viewModel.UpdateList(m_type);
            m_viewModel.LoadMore(m_type); // Prefetch some data

            ThreadPool.QueueUserWorkItem(delegate
            {
                // Have to delay otherwise the item is not found
                Thread.Sleep(500);
                Dispatcher.BeginInvoke(delegate
                {
                    updatePivotTitleWithFirstDisplayedElementFromCurrentList();
                });
            });
        }

        // Lists

        /// <summary>
        /// Selecting an item in the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void list_selection(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var list = (ListBox)sender;
            var selection = list.SelectedItem;

            if (selection == null) return;

            ListType dataType = ListType.Unknow;
            string param = "";
            if (list == wordsList) dataType = ListType.Words;
            else if (list == ctpList) dataType = ListType.Contrepeteries;
            else if (list == favoritesList) dataType = ListType.Favorites;
            else if (list == bestofList) dataType = ListType.Florilege;

            ElementViewModel vm = null;
            if (selection is WordViewModel)
            {
                vm = selection as WordViewModel;
            }
            else if (selection is ContrepeterieViewModel)
            {
                vm = selection as ContrepeterieViewModel;
            }

            string uri = m_viewModel.SelectElement(vm, dataType, param);
            NavigationService.Navigate(new Uri(uri, UriKind.Relative));
        }

        /// <summary>
        /// Display first element title in pivot title
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="scrollValue"></param>
        void ListScrolled(ListBox sender, double scrollValue)
        {
            updatePivotTitleWithFirstDisplayedElementFromList(sender);
        }

        // Search

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (m_viewModel.SearchPanelVisiblity == System.Windows.Visibility.Collapsed)
            {
                // Display overlay
                m_viewModel.SearchPanelVisiblity = System.Windows.Visibility.Visible;

                // Animation
                showSearch.Begin();
            }
            else
            {
                hideSearch.Begin();
            }
        }

        private void searchInputTb_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                m_viewModel.Search(m_type, searchInputTb.Text);
                hideSearch.Begin();
            }
        }

        void showSearch_Completed(object sender, EventArgs e)
        {
            searchInputTb.Text = "";
            m_viewModel.SearchInput = "";

            // Display keyboard
            searchInputTb.Focus();
        }

        void hideSearch_Completed(object sender, EventArgs e)
        {
            pivotControl.Focus();
            m_viewModel.SearchPanelVisiblity = System.Windows.Visibility.Collapsed;
        }

        private void searchInputTb_LostFocus(object sender, RoutedEventArgs e)
        {
            hideSearch.Begin();
        }

        // Manual update

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            // Loading
            ProgressIndicator progressIndicator = new ProgressIndicator();
            progressIndicator.IsIndeterminate = true;
            progressIndicator.IsVisible = true;

            SystemTray.SetProgressIndicator(this, progressIndicator);

            bool available = CacheManager.Instance.UpdateCache(true, () =>
            {
                m_viewModel.UpdateList(ListType.Words);
                m_viewModel.UpdateList(ListType.Contrepeteries);
                m_viewModel.UpdateList(ListType.Favorites);
                m_viewModel.UpdateList(ListType.Florilege);

                progressIndicator.IsVisible = false;
            });

            if (available == false)
            {
                progressIndicator.IsVisible = false;
            }
        }

        #endregion
    }
}