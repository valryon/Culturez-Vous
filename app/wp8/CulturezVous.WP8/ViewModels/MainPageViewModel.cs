using System;
using System.Collections.ObjectModel;
using System.Linq;
using CulturezVous.Data;
using CulturezVous.WP8.Utils;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CulturezVous.WP8.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace CulturezVous.WP8.ViewModels
{
    /// <summary>
    /// ViewModel for the main page (for words, contrepèterie & about)
    /// </summary>
    public class MainPageViewModel : ViewModelBase
    {
        public event Action LoadingComplete, LoadingFailed;

        private bool m_wordsNeedsUpdate;
        private bool m_ctpsNeedsUdpdate;
        private bool updating = false;

        #region Designer

        public MainPageViewModel()
        {
            IsLoading = false;
            IsEnabled = true;

            Words = new ObservableCollection<WordViewModel>() {
                new WordViewModel(new Word() {
                    Date = DateTime.Now,
                    Title = "Mot designer 1",
                    IsRead = false,
                }),
                new WordViewModel(new Word() {
                    Date = DateTime.Now.AddDays(-1),
                    Title = "Mot designer 2",
                    IsRead = true,
                }),
                new WordViewModel(new Word() {
                    Date = DateTime.Now.AddDays(-2),
                    Title = "Mot designer 3",
                    IsRead = true,
                })
            };

            CTPs = new ObservableCollection<ContrepeterieViewModel>() {
                new ContrepeterieViewModel(new Contrepeterie() {
                    Date = DateTime.Now,
                    Title = "Contrepet designer 1",
                    IsRead = false,
                }),
                new ContrepeterieViewModel(new Contrepeterie() {
                    Date = DateTime.Now.AddDays(-1),
                    Title = "Contrepet designer 2",
                    IsRead = true,
                }),
                new ContrepeterieViewModel(new Contrepeterie() {
                    Date = DateTime.Now.AddDays(-2),
                    Title = "Contrepet designer 3",
                    IsRead = true,
                })
            };
        }

        #endregion

        public MainPageViewModel(bool notDesigner)
        {
            // New content available listeners
            CacheManager.Instance.NewWordsAvailable += new System.Action<List<Word>>(newWordsAvailable);
            CacheManager.Instance.NewContrepeteriesAvailable += new System.Action<List<Contrepeterie>>(newContrepeteriesAvailable);

            IsLoading = true;
            IsEnabled = true;

            // New page listener
            CacheManager.Instance.InitializationNewPage += new Action<List<Element>>((elements) =>
            {

                // We are getting the elements first
                // So we just need to display the 3 first on each panorama page
                // -- Words
                int limit = 4;
                if (Words == null || Words.Count < limit)
                {
                    if (Words == null)
                    {
                        Words = new ObservableCollection<WordViewModel>();
                    }
                    foreach (Element e in elements)
                    {
                        if ((e is Word) && (Words.Count < limit))
                        {
                            Words.Add(new WordViewModel(e as Word));
                        }
                    }
                }
                else if (Words.Count >= limit)
                {
                    // Update UI when the page is complete to avoid thread-cross troubles
                    ExecuteUI(() =>
                    {
                        RaisePropertyChanged("Words");
                    });
                }

                //-- CTPs
                if (CTPs == null || CTPs.Count < limit)
                {
                    if (CTPs == null)
                    {
                        CTPs = new ObservableCollection<ContrepeterieViewModel>();
                    }
                    foreach (Element e in elements)
                    {
                        if ((e is Contrepeterie) && (CTPs.Count < limit))
                        {
                            CTPs.Add(new ContrepeterieViewModel(e as Contrepeterie));
                        }
                    }
                }
                else if (CTPs.Count >= limit)
                {
                    // Update UI when the page is complete to avoid thread-cross troubles
                    ExecuteUI(() =>
                    {
                        RaisePropertyChanged("CTPs");
                    });
                }
            });

            // Loading complete!
            CacheManager.Instance.InitializationCompleted += new Action(() =>
            {
                if (LoadingComplete != null) LoadingComplete();
                loadingComplete(true);
            });

            // Loading failed!
            CacheManager.Instance.InitializationFailed += new Action(() =>
            {
                if (LoadingFailed != null) LoadingFailed();

                if (CacheManager.Instance.IsInitialized == false)
                {
                    CacheManager.Instance.Initialize(InitializeLevel.Offline);
                }
                bool readonlyMode = false;
                try
                {
                    readonlyMode = (CacheManager.Instance.Elements.Count > 0);
                }
                catch (Exception) { }

                if (readonlyMode == false)
                {
                    ExecuteUI(() =>
                    {
                        MessageBox.Show("L'application n'a pas pu démarrer. Vérifiez que vous êtes connectés à Internet et réessayez.", "Culturez-Vous un peu plus tard...", MessageBoxButton.OK);
                    });
                }

                loadingComplete(readonlyMode);
            });

            // Launch initialization
            CacheManager.Instance.Initialize(InitializeLevel.Complete);
        }


        private void loadingComplete(bool success)
        {
            IsLoading = false;
            IsPivotAvailable = success;

            ExecuteUI(delegate()
            {
                RaisePropertyChanged("CTPs");
                RaisePropertyChanged("Words");
                RaisePropertyChanged("IsLoading");
                RaisePropertyChanged("IsPivotAvailable");
            });

            m_wordsNeedsUpdate = true;
            m_ctpsNeedsUdpdate = true;

            CheckForUpdate();
        }

        void newWordsAvailable(List<Word> obj)
        {
            m_wordsNeedsUpdate = true;
        }

        void newContrepeteriesAvailable(List<Contrepeterie> obj)
        {
            m_ctpsNeedsUdpdate = true;
        }

        /// <summary>
        /// Check if some new elements have to be added to the lists
        /// </summary>
        public void CheckForUpdate()
        {
            if (updating) return;

            updating = true;

            if (m_wordsNeedsUpdate)
            {
                m_wordsNeedsUpdate = false;

                // get the few last words
                var lastWords = CacheManager.Instance.GetCacheElements<Word>(0, 4, "");

                Words = new ObservableCollection<WordViewModel>();

                foreach (var vm in lastWords.Select(w => new WordViewModel(w)).ToList())
                {
                    try
                    {
                        Words.Add(vm);
                    }
                    catch (Exception) { } // Sale hack
                }
            }

            if (m_ctpsNeedsUdpdate)
            {
                m_ctpsNeedsUdpdate = false;

                // And the few last contrepetries
                var lastCTPs = CacheManager.Instance.GetCacheElements<Contrepeterie>(0, 4, "");

                CTPs = new ObservableCollection<ContrepeterieViewModel>();

                foreach (var vm in lastCTPs.Select(c => new ContrepeterieViewModel(c)).ToList())
                {
                    try
                    {
                        CTPs.Add(vm);
                    }
                    catch (Exception) { } // Sale hack
                }
            }

            ExecuteUI(delegate()
            {
                RaisePropertyChanged("CTPs");
                RaisePropertyChanged("Words");
            });

            updating = false;
        }

        /// <summary>
        /// Select a word in the list
        /// </summary>
        /// <param name="wordViewModel"></param>
        /// <returns></returns>
        public string SelectWord(WordViewModel wordViewModel)
        {
            wordViewModel.IsRead = true;
            RaisePropertyChanged("Words");

            return string.Format("/Views/DetailsPage.xaml?id={0}&type=words", wordViewModel.Id);
        }

        /// <summary>
        /// Select a CTP in the list
        /// </summary>
        /// <param name="contrepeterieViewModel"></param>
        /// <returns></returns>
        public string SelectContrepeterie(ContrepeterieViewModel contrepeterieViewModel)
        {
            contrepeterieViewModel.IsRead = true;
            RaisePropertyChanged("CTPs");

            return string.Format("/Views/DetailsPage.xaml?id={0}&type=contrepeteries", contrepeterieViewModel.Id);
        }

        #region Properties

        /// <summary>
        /// Smart background (light / dark theme)
        /// </summary>
        public ImageBrush PanoramaBackgroundImage
        {
            get
            {
                var brush = new ImageBrush
                {
                    ImageSource = new BitmapImage(ImagesManager.Instance.Background)
                };
                return brush;
            }
        }

        /// <summary>
        /// Page available
        /// </summary>
        public bool IsEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Pivot avaiable (after looding)
        /// </summary>
        public bool IsPivotAvailable
        {
            get;
            private set;
        }

        /// <summary>
        /// Still loading
        /// </summary>
        public bool IsLoading
        {
            get;
            private set;
        }

        /// <summary>
        /// Loading bar visibility
        /// </summary>
        public Visibility LoadingVisibility
        {
            get
            {
                if (IsLoading) return Visibility.Visible;
                else return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Words list
        /// </summary>
        public ObservableCollection<WordViewModel> Words
        {
            get;
            private set;
        }

        /// <summary>
        /// Contrepetries list
        /// </summary>
        public ObservableCollection<ContrepeterieViewModel> CTPs
        {
            get;
            private set;
        }

        #endregion
    }
}
