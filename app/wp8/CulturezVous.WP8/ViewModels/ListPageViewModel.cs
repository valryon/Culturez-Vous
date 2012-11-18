using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CulturezVous.Data;
using CulturezVous.WP8.Utils;
using System.Threading;

namespace CulturezVous.WP8.ViewModels
{
    public enum ListType
    {
        Unknow = 0,
        Words,
        Contrepeteries,
        Favorites,
        Florilege
    }

    /// <summary>
    /// View model for the page list
    /// </summary>
    public class ListPageViewModel : ViewModelBase
    {

        private static int elementsPerPage = 7;

        #region Designer

        public ListPageViewModel()
        {
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

            Contrepeteries = new ObservableCollection<ContrepeterieViewModel>() {
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

            Favorites = new ObservableCollection<ElementViewModel>()
            {
                Words[0],
                Contrepeteries[1]
            };

            BestOf = new ObservableCollection<ElementViewModel>()
            {
                Words[1],
                Contrepeteries[2],
                Contrepeteries[0]
            };

            SearchPanelVisiblity = Visibility.Collapsed;
        }

        #endregion

        public ListPageViewModel(bool notDesigner)
        {
            PivotTitle = "Culturez-Vous !";

            Words = new ObservableCollection<WordViewModel>();
            Contrepeteries = new ObservableCollection<ContrepeterieViewModel>();
            Favorites = new ObservableCollection<ElementViewModel>();
            BestOf = new ObservableCollection<ElementViewModel>();

            SearchPanelVisiblity = Visibility.Collapsed;
        }

        /// <summary>
        /// Request some data loading
        /// </summary>
        /// <param name="type"></param>
        public void LoadMore(ListType type)
        {
            // Display loading bar
            IsLoading = true;

            if (type == ListType.Words)
            {
                // Compute last page number
                int pageNumber = (Words.Count / ListPageViewModel.elementsPerPage) + 2;

                // Try to load from cache
                var list = CacheManager.Instance.GetCacheElements<Word>(Words.Count, elementsPerPage, SearchInput).Select(w => new WordViewModel(w)).ToList();
                list.ForEach(wm => Words.Add(wm));

                if (list.Count < elementsPerPage)
                {
                    CacheManager.Instance.DownloadOldData(pageNumber, (data) =>
                    {
                        // Add to words list
                        data.Where(d => d is Word).Select(d => d as Word).ToList().ForEach(w => Words.Add(new WordViewModel(w)));
                        endLoading("Words");
                    });
                }
                else
                {
                    endLoading("Words");
                }
            }
            else if (type == ListType.Contrepeteries)
            {
                // Try to load from cache
                var list = CacheManager.Instance.GetCacheElements<Contrepeterie>(Contrepeteries.Count, elementsPerPage, SearchInput).Select(c => new ContrepeterieViewModel(c)).ToList();
                list.ForEach(cm => Contrepeteries.Add(cm));

                if (list.Count < elementsPerPage)
                {
                    CacheManager.Instance.DownloadOldData(Contrepeteries.Last().Date.AddDays(-7), (data) =>
                    {
                        if (data != null && data is Contrepeterie)
                        {
                            Contrepeteries.Add(new ContrepeterieViewModel(data as Contrepeterie));

                            CacheManager.Instance.DownloadOldData(data.Date.AddDays(-7), (data2) =>
                            {
                                if (data2 != null && data2 is Contrepeterie)
                                {
                                    Contrepeteries.Add(new ContrepeterieViewModel(data2 as Contrepeterie));

                                    CacheManager.Instance.DownloadOldData(data2.Date.AddDays(-7), (data3) =>
                                    {
                                        if (data3 != null && data3 is Contrepeterie)
                                        {
                                            Contrepeteries.Add(new ContrepeterieViewModel(data3 as Contrepeterie));

                                            CacheManager.Instance.DownloadOldData(data3.Date.AddDays(-7), (data4) =>
                                            {
                                                if (data4 != null && data4 is Contrepeterie)
                                                {
                                                    Contrepeteries.Add(new ContrepeterieViewModel(data4 as Contrepeterie));
                                                    endLoading("Contrepeteries");
                                                }
                                                else endLoading("Contrepeteries");
                                            });
                                        }
                                        else endLoading("Contrepeteries");
                                    });
                                }
                                else endLoading("Contrepeteries");
                            });
                        }
                        else endLoading("Contrepeteries");
                    });
                }
                else
                {
                    endLoading("Contrepeteries");
                }
            }
            else
            {
                endLoading("Favorites");
                endLoading("BestOf");
            }
        }

        private void endLoading(string propertyToUpdate)
        {
            // End loading
            ExecuteUI(delegate()
            {
                RaisePropertyChanged(propertyToUpdate);
                IsLoading = false;
            });
        }

        public bool IsListLoaded(ListType type)
        {
            switch (type)
            {
                case ListType.Words:
                    return Words.Count != 0;
                case ListType.Contrepeteries:
                    return Contrepeteries.Count != 0;
                case ListType.Florilege:
                    return BestOf.Count != 0;
                case ListType.Favorites:
                    return Favorites.Count != 0;
            }

            return false;
        }

        /// <summary>
        /// Update a specific list (updating all on once is too long)
        /// </summary>
        /// <param name="type"></param>
        public void UpdateList(ListType type)
        {
            switch (type)
            {
                case ListType.Words:
                    updateWords();
                    break;
                case ListType.Contrepeteries:
                    updateContrepeteries();
                    break;
                case ListType.Florilege:
                    updateFlorilege();
                    break;
                case ListType.Favorites:
                    updateFavorites();
                    break;
            }
        }

        private void updateWords()
        {
            // No words in the list = first loading
            if (Words.Count == 0)
            {
                Words = new ObservableCollection<WordViewModel>(CacheManager.Instance.GetCacheElements<Word>(0, elementsPerPage, SearchInput).Select(w => new WordViewModel(w)).ToList());
            }
            else
            {
                var lastWord = CacheManager.Instance.GetCacheLast<Word>();

                // New Words ?
                var firstListWord = Words.First();
                if (lastWord.Id != firstListWord.Id)
                {
                    var results = CacheManager.Instance.GetCacheNewElementsFrom<Word>(lastWord);
                    foreach (Word rw in results)
                    {
                        Words.Add(new WordViewModel(rw));
                    }
                }
            }
            RaisePropertyChanged("Words");
        }

        private void updateContrepeteries()
        {
            if (Contrepeteries.Count == 0)
            {
                Contrepeteries = new ObservableCollection<ContrepeterieViewModel>(CacheManager.Instance.GetCacheElements<Contrepeterie>(0, elementsPerPage, SearchInput).Select(w => new ContrepeterieViewModel(w)).ToList());
            }
            else
            {
                var lastContrepeterie = CacheManager.Instance.GetCacheLast<Contrepeterie>();

                // New Contrepeteries ?
                var lastListCtp = Contrepeteries.First();

                if (lastListCtp.Id != lastContrepeterie.Id)
                {
                    var results = CacheManager.Instance.GetCacheNewElementsFrom<Contrepeterie>(lastContrepeterie);
                    foreach (Contrepeterie cw in results)
                    {
                        Contrepeteries.Add(new ContrepeterieViewModel(cw));
                    }
                }
            }
            RaisePropertyChanged("Contrepeteries");
        }

        private void updateFlorilege()
        {
            if (BestOf == null)
            {
                BestOf = new ObservableCollection<ElementViewModel>();
            }
            else
            {
                BestOf.Clear();
            }

            var cache = CacheManager.Instance.GetCacheAll<Element>(null, true);

            // Apply search
            cache = CacheManager.ApplyFilter(cache, SearchInput); 

            foreach (var element in cache)
            {
                if (element is Word) BestOf.Add(new WordViewModel(element as Word));
                else if (element is Contrepeterie) BestOf.Add(new ContrepeterieViewModel(element as Contrepeterie));
            }

            RaisePropertyChanged("BestOf");
            RaisePropertyChanged("EmptyBestOfListVisibility");

        }

        private void updateFavorites()
        {
            var favList = CacheManager.Instance.GetCacheAll<Element>(true);
            Favorites = new ObservableCollection<ElementViewModel>();

            foreach (var element in favList)
            {
                if (element is Word) Favorites.Add(new WordViewModel(element as Word));
                else if (element is Contrepeterie) Favorites.Add(new ContrepeterieViewModel(element as Contrepeterie));
            }

            RaisePropertyChanged("Favorites");
            RaisePropertyChanged("EmptyFavListVisibility");
        }

        #region Search

        /// <summary>
        /// Perform a search on a text input and fill a list for it
        /// </summary>
        /// <param name="type"></param>
        /// <param name="query"></param>
        public void Search(ListType type, string query)
        {
            SearchInput = query;
            CurrentSearchListType = type;

            // Récupérer la liste courant
            switch (type)
            {
                case ListType.Words:
                    var listWords = CacheManager.Instance.GetCacheElements<Word>(0, elementsPerPage, SearchInput).Select(w => new WordViewModel(w)).ToList();
                    Words = new ObservableCollection<WordViewModel>(listWords);
                    RaisePropertyChanged("Words");
                    break;

                case ListType.Contrepeteries:
                    var listCtp = CacheManager.Instance.GetCacheElements<Contrepeterie>(0, elementsPerPage, SearchInput).Select(w => new ContrepeterieViewModel(w)).ToList();
                    Contrepeteries = new ObservableCollection<ContrepeterieViewModel>(listCtp);
                    RaisePropertyChanged("Contrepeteries");
                    break;
                case ListType.Florilege:
                    updateFlorilege();
                    break;
                case ListType.Favorites:
                    updateFavorites();
                    break;
            }

            RaisePropertyChanged("SearchInput");
            RaisePropertyChanged("CurrentSearch");
        }

        public void ResetSearch()
        {
            SearchInput = "";

            switch (CurrentSearchListType)
            {
                case ListType.Words:
                    Words.Clear();
                    RaisePropertyChanged("Words");
                    break;
                case ListType.Contrepeteries:
                    Contrepeteries.Clear();
                    RaisePropertyChanged("Contrepeteries");
                    break;
                case ListType.Florilege:
                    BestOf.Clear();
                    RaisePropertyChanged("BestOf");
                    break;
                case ListType.Favorites:
                    Favorites.Clear();
                    RaisePropertyChanged("Favorites");
                    break;
            }

            RaisePropertyChanged("SearchInput");
            RaisePropertyChanged("CurrentSearch");
        }

        public string SearchInput
        {
            get;
            set;
        }

        public Visibility EmptyFavListVisibility
        {
            get
            {
                return (Favorites.Count == 0 ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public Visibility EmptyBestOfListVisibility
        {
            get
            {
                return (BestOf.Count == 0 ? Visibility.Visible : Visibility.Collapsed);
            }
        }



        public ListType CurrentSearchListType
        {
            get;
            set;
        }

        #endregion

        #region Selection

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Navigation Uri</returns>
        public string SelectElement(ElementViewModel elementVm, ListType dataType, string param)
        {
            elementVm.IsRead = true;

            switch (dataType)
            {
                case ListType.Words:
                    RaisePropertyChanged("Words");
                    break;
                case ListType.Contrepeteries:
                    RaisePropertyChanged("Contrepeteries");
                    break;
                case ListType.Florilege:
                    RaisePropertyChanged("BestOf");
                    break;
                case ListType.Favorites:
                    RaisePropertyChanged("Favorites");
                    break;
            }
            return string.Format("/Views/DetailsPage.xaml?id={0}&type={1}&param={2}", elementVm.Id, dataType, param);
        }

        #endregion

        #region Properties

        private string m_pivotTitle;
        /// <summary>
        /// Title to display instead of the application name
        /// </summary>
        public string PivotTitle
        {
            get { return m_pivotTitle; }
            set
            {
                m_pivotTitle = value.ToUpper(); ;
                RaisePropertyChanged("PivotTitle");
            }
        }

        private Visibility m_searchPanelVisibility;
        public Visibility SearchPanelVisiblity
        {
            get
            {
                return m_searchPanelVisibility;
            }
            set
            {
                m_searchPanelVisibility = value;
                RaisePropertyChanged("SearchPanelVisiblity");
            }
        }

        /// <summary>
        /// List of words
        /// </summary>
        public ObservableCollection<WordViewModel> Words
        {
            get;
            private set;
        }

        /// <summary>
        /// List of Contrepeteries
        /// </summary>
        public ObservableCollection<ContrepeterieViewModel> Contrepeteries
        {
            get;
            private set;
        }


        private ObservableCollection<ElementViewModel> m_favorites;

        /// <summary>
        /// List of Favorites
        /// </summary>
        public ObservableCollection<ElementViewModel> Favorites
        {
            get { return m_favorites; }
            private set
            {
                m_favorites = value;
                RaisePropertyChanged("Favorites");
                RaisePropertyChanged("EmptyFavListVisibility");
            }
        }

        private ObservableCollection<ElementViewModel> m_bestOf;
        /// <summary>
        /// List of Bestof
        /// </summary>
        public ObservableCollection<ElementViewModel> BestOf
        {
            get { return m_bestOf; }
            private set
            {
                m_bestOf = value;
                RaisePropertyChanged("BestOf");
                RaisePropertyChanged("EmptyBestOfListVisibility");
            }
        }

        /// <summary>
        /// Display or not progress bar for loading
        /// </summary>
        public Visibility ProgressBarVisibility
        {
            get
            {
                return IsLoading ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private bool m_isLoading;
        /// <summary>
        /// Boolean telling if we are currently retrieving new data
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return m_isLoading;
            }
            private set
            {
                m_isLoading = value;
                RaisePropertyChanged("IsLoading");
                RaisePropertyChanged("ProgressBarVisibility");
            }
        }

        #endregion
    }
}
