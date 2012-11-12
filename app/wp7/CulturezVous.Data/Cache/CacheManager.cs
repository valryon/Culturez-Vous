using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CulturezVous.Data.Cache;
using CulturezVous.WP7.Data.Retrievers;
using System.Collections;

namespace CulturezVous.Data
{
    /// <summary>
    /// Sort criteria
    /// </summary>
    public enum Criteria
    {
        Date,
        Bestof,
        Favorites
    }

    /// <summary>
    /// Initialization gravity
    /// </summary>
    public enum InitializeLevel
    {
        Offline,
        Readonly,
        Update,
        UpdateAsync,
        Complete
    }

    /// <summary>
    /// Create and manage a local cache (database) to minimize data transfer.
    /// aka CacheDataViewModel
    /// </summary>
    public class CacheManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Database string connection (local storage with Linq 2 SQL)
        /// </summary>
        private static string databaseConnectionString = @"Data Source=isostore:/CulturezVous.sdf";

        /// <summary>
        /// Pages downloaded for the first successful app launch
        /// </summary>
        private const int initialPageDownload = 4;

        private CacheDataContext m_dataContext;

        private ElementsRetriever m_elementsRetriever;
        //private BackgroundWorker m_backgroundWorker;
        //private bool m_isActive;

        public event Action<List<Element>> InitializationNewPage;
        public event Action<List<Word>> NewWordsAvailable;
        public event Action<List<Contrepeterie>> NewContrepeteriesAvailable;
        public event Action InitializationCompleted, InitializationFailed;

        private CacheManager()
        {

        }

        #region Singleton

        private static CacheManager m_Instance;
        public static CacheManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new CacheManager();
                }
                return m_Instance;
            }
        }

        #endregion

        #region Intialize and load

        /// <summary>
        /// Smart initialization of the cache
        /// </summary>
        /// <param name="initLevel"></param>
        public void Initialize(InitializeLevel initLevel)
        {

            // Initialize a webservice (Internet) or bouchon (local) data source
#if BOUCHON
            m_elementsRetriever = new TestElementsRetriever();
#else
            m_elementsRetriever = new WSRetriever(initLevel == InitializeLevel.Readonly ? false : true);
#endif

            // Check for database existence
            m_dataContext = new CacheDataContext(databaseConnectionString);
            bool dbExists = m_dataContext.DatabaseExists();

            // Db is empty ?
            bool isDbEmpty = false;
            if (dbExists)
            {
                try
                {
                    isDbEmpty = m_dataContext.Elements.Count() == 0;
                }
                catch (Exception) { isDbEmpty = true; }
            }

            // Do not reload everything if we can just update
            if (initLevel == InitializeLevel.Complete)
            {
                if ((dbExists) && (isDbEmpty == false))
                {
                    initLevel = InitializeLevel.UpdateAsync;
                }
            }
            else
            {
                // No db but not a complete init? Weird
                if (dbExists == false)
                {
                    // Crash.
                    if (InitializationFailed != null) InitializationFailed();
                }
            }

            // Launch process
            switch (initLevel)
            {
                // Emergency mode
                case InitializeLevel.Offline:
                    initializeOffline();
                    break;
                // For readonly: no need to async process
                case InitializeLevel.Readonly:
                    initializeReadOnly();
                    break;

                case InitializeLevel.Update:
                    initializeUpdate();
                    break;

                // Complexe and long, above processes are async
                case InitializeLevel.UpdateAsync:

                    BackgroundWorker workerUpdate = new BackgroundWorker();
                    workerUpdate.DoWork += new DoWorkEventHandler((o, args) =>
                    {
                        initializeUpdate();
                    });
                    workerUpdate.RunWorkerAsync();

                    break;

                case InitializeLevel.Complete:

                    BackgroundWorker workerComplete = new BackgroundWorker();
                    workerComplete.DoWork += new DoWorkEventHandler((o, args) =>
                    {
                        initializeComplete();
                    });
                    workerComplete.RunWorkerAsync();

                    break;
            }
        }

        /// <summary>
        /// Load elements from db if possible, no events, no update
        /// </summary>
        private void initializeOffline()
        {
            if (m_dataContext.DatabaseExists())
            {
                loadElementsFromDb();
                IsInitialized = true;
            }
        }

        /// <summary>
        /// Fast and readonly init, and not async
        /// </summary>
        private void initializeReadOnly()
        {
            if (m_dataContext.DatabaseExists())
            {
                loadElementsFromDb();
            }

            bool updateComplete = false;
            float timeOutLeft = 10000f; // milli
            const int step = 250; // milli

            UpdateCache(false, () =>
            {
                updateComplete = true;
            });

            while (!updateComplete && timeOutLeft > 0)
            {
                timeOutLeft -= step;
                Thread.Sleep(step);
            }

            if (updateComplete)
            {
                IsInitialized = true;
                if (InitializationCompleted != null) InitializationCompleted();
            }
            else
            {
                if (InitializationFailed != null) InitializationFailed();
            }
        }

        /// <summary>
        /// Initialize the cache with an existing database
        /// </summary>
        private void initializeComplete()
        {
            //m_isActive = true;

            // Potential database suppression
            if (m_dataContext.DatabaseExists())
            {
                m_dataContext.DeleteDatabase();
            }

            // Creation of the database
            m_dataContext.CreateDatabase();

            // Fill it!
            m_elementsRetriever.TestConnectionAsync(connectionAvailable =>
            {
                if (connectionAvailable)
                {
                    Elements = new ObservableCollection<Element>();

                    // Get some pages to fill
                    m_elementsRetriever.GetElementsPaginatedAsync(
                        // A new page is ready
                        (newData) =>
                        {
                            foreach (Element e in newData)
                            {
                                AddElement(e);
                            }

                            // Notify for new content
                            if (newData.Count > 0)
                            {
                                if (InitializationNewPage != null) InitializationNewPage(newData);
                            }
                        },
                        // All has been loaded !
                        () =>
                        {
                            // First save
                            SaveToDb();

                            // Download the bestof
                            m_elementsRetriever.GetBestOf((bestof) =>
                            {
                                mergeData(bestof);

                                List<Element> listElements = new List<Element>((IList<Element>)Elements);

                                // Mark all as read
                                listElements.ForEach(e => e.IsRead = true);

                                // Mark first word and first ctp as not read
                                listElements.OrderByDescending(e => e.Date).Where(e => e is Word).First().IsRead = false;
                                listElements.OrderByDescending(e => e.Date).Where(e => e is Contrepeterie).First().IsRead = false;
                                listElements.Sort((Element e1, Element e2) => { return e2.Date.CompareTo(e1.Date); });

                                Elements = new ObservableCollection<Element>(listElements);

                                SaveToDb();

                                IsInitialized = true;
                                //loadElementsFromDb();

                                //setupBackgroundUpdater();

                                if (InitializationCompleted != null) InitializationCompleted();
                            });
                        }, 1, initialPageDownload // pages initiales
                    );
                }
                else
                {
                    // No database + no connection: abandon all!
                    if (InitializationFailed != null) InitializationFailed();
                }
            });

        }

        /// <summary>
        /// Create the database from scratch
        /// </summary>
        private void initializeUpdate()
        {
            //m_isActive = true;

            // The database must exists
            try
            {
                // Load data
                loadElementsFromDb();

                // Display old elements
                if (InitializationNewPage != null) InitializationNewPage((new List<Element>((IList<Element>)Elements)));

                IsInitialized = true;

                m_elementsRetriever.TestConnectionAsync(connectionAvailable =>
               {
                   if (connectionAvailable)
                   {
                       // Try to update from the last time
                       UpdateCache(true, () =>
                       {
                           if (InitializationCompleted != null) InitializationCompleted();
                       });
                   }
                   else
                   {
                       // No database + no connection: abandon all!
                       if (InitializationFailed != null) InitializationFailed();
                   }
               });
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
                // Delete database !
                m_dataContext.DeleteDatabase();

                if (InitializationFailed != null) InitializationFailed();
            }

            setupBackgroundUpdater();
        }

        /// <summary>
        /// Create a background Thread to update the cache
        /// </summary>
        private void setupBackgroundUpdater()
        {
            // Ne sert à rien
            //m_backgroundWorker = new BackgroundWorker();
            //m_backgroundWorker.DoWork += new DoWorkEventHandler((o, args) =>
            //{
            //    while (m_isActive)
            //    {
            //        Thread.Sleep(updateFrequency);
            //        UpdateCache();
            //    }
            //});
            //m_backgroundWorker.RunWorkerAsync();
        }

        private void loadElementsFromDb()
        {
            // Query the database and load items.
            // -- Elements
            var elementsInDB = (from Element ele in m_dataContext.Elements select ele).ToList();
            Elements = new ObservableCollection<Element>(elementsInDB);

            // -- Definitions for words
            foreach (Word word in Elements.Where(e => e is Word))
            {
                word.Definitions = m_dataContext.Definitions.Where(d => d.Word == word).ToList();
            }

            IsNewElementsAvailable = true;
        }

        #endregion

        #region Update

        /// <summary>
        /// Download some data not stored in the cache but available on our database
        /// </summary>
        /// <param name="updateComplete"></param>
        public void DownloadOldData(int requestedPage, Action<List<Element>> pageDownloaded)
        {
            m_elementsRetriever.GetElementsPaginatedAsync((elements) =>
            {
                List<Element> newElements = new List<Element>();

                if (elements.Count > 0)
                {
                    newElements = mergeData(elements);
                    elements.ForEach(e => e.IsRead = true); // Mark as read
                    SaveToDb();
                }

                pageDownloaded(newElements);

            }, null, requestedPage, 1);
        }

        /// <summary>
        /// Download some data not stored in the cache but available on our database
        /// </summary>
        /// <param name="updateComplete"></param>
        public void DownloadOldData(DateTime date, Action<Element> downloadCompleted)
        {
            m_elementsRetriever.GetElementFromDateAsync(date, (element) =>
            {
                if (element != null)
                {
                    List<Element> newElements = new List<Element>() { element };

                    newElements = mergeData(newElements);
                    newElements.ForEach(e => e.IsRead = true); // Mark as read
                    SaveToDb();
                }

                downloadCompleted(element);

            });
        }

        /// <summary>
        /// Update the cache
        /// </summary>
        public bool UpdateCache(bool updateBestOf, Action updateComplete = null)
        {
#if DEBUG
            Debug.WriteLine("Updating cache...");
#endif
            bool available = (IsInitialized && m_elementsRetriever.IsAvailable);

            if (available)
            {
                // Retrieve new data
                m_elementsRetriever.GetLastDataAsync(newData =>
               {
                   // Any differences ?
                   if (newData.Count != 0)
                   {
                       mergeData(newData);
                       SaveToDb();
                   }

                   // Update florilege in background
                   if (updateBestOf)
                   {
                       UpdateBestOf((bestofData) =>
                       {
                           if (bestofData.Count != 0)
                           {
                               mergeData(bestofData);
                               SaveToDb();
                           }

                           if (updateComplete != null) updateComplete();

                           // Update last update time
                           m_dataContext.LastCacheUpdate = DateTime.Now;

#if DEBUG
                           Debug.WriteLine("Updating cache completed.");
#endif
                       });
                   }
                   else
                   {

                       if (updateComplete != null) updateComplete();

                       // Update last update time
                       m_dataContext.LastCacheUpdate = DateTime.Now;

#if DEBUG
                       Debug.WriteLine("Updating cache completed.");
#endif
                   }
               });
            }

            return available;
        }

        /// <summary>
        /// Add the new data downloaded to the current cache
        /// </summary>
        /// <param name="newData"></param>
        /// <returns></returns>
        private List<Element> mergeData(List<Element> newData)
        {
            var newElements = new List<Element>();

            // Copy in a local variable to bypass ref
            var localList = (from e in newData
                             where (m_dataContext.Elements.Select(dbe => dbe.Id).Contains(e.Id) == false)
                             select e
                 ).ToList();

            localList.ForEach(e =>
            {
                newElements.Add(e);

                // Update model
                AddElement(e);
            });

            var tempList = Elements.ToList();
            tempList.Sort((Element e1, Element e2) => { return e2.Date.CompareTo(e1.Date); });
            Elements = new ObservableCollection<Element>(tempList);

            if (newElements.Where(e => e is Word).Count() > 0 && NewWordsAvailable != null) NewWordsAvailable(newElements.Where(e => e is Word).Select(e => e as Word).ToList());
            if (newElements.Where(e => e is Contrepeterie).Count() > 0 && NewContrepeteriesAvailable != null) NewContrepeteriesAvailable(newElements.Where(e => e is Word).Select(e => e as Contrepeterie).ToList());

            return newElements;
        }

        /// <summary>
        /// Save the cache on the phone
        /// </summary>
        public void SaveToDb()
        {
            try
            {
                m_dataContext.SubmitChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Saving DB failed !");
                Debug.WriteLine("-> Details: " + e.Message);
                throw new ArgumentException("Error while saving...", e);
            }
        }

        /// <summary>
        /// Simply delete the database
        /// </summary>
        public void DeleteDb()
        {
            if (m_dataContext.DatabaseExists())
            {
                m_dataContext.DeleteDatabase();
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Proper destructor
        /// </summary>
        public void Dispose()
        {
            //m_isActive = false;
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Internal data access

        /// <summary>
        /// Add a new element to the DB
        /// </summary>
        /// <param name="element"></param>
        public void AddElement(Element element)
        {
            m_dataContext.Elements.InsertOnSubmit(element);
            Elements.Add(element);

            // For words, add definitions
            if (element is Word)
            {
                var word = element as Word;
                m_dataContext.Definitions.InsertAllOnSubmit(word.Definitions);
            }
        }

        // Elements
        private ObservableCollection<Element> m_elements;
        public ObservableCollection<Element> Elements
        {
            get { return m_elements; }
            private set
            {
                m_elements = value;
                NotifyPropertyChanged("Elements");
            }
        }

        /// <summary>
        /// Tell components that they should update because new content is available
        /// </summary>
        public bool IsNewElementsAvailable { get; set; }

        /// <summary>
        /// Database still not initialized
        /// </summary>
        public bool IsInitialized { get; set; }

        #endregion

        #region Data access

        /// <summary>
        ///Download current best of
        /// </summary>
        /// <param name="bestOfElementsCallback"></param>
        public void UpdateBestOf(Action<List<Element>> bestOfElementsCallback = null)
        {
            m_elementsRetriever.GetBestOf(list =>
            {
                if (bestOfElementsCallback != null)
                    bestOfElementsCallback(list);
            });
        }

        /// <summary>
        /// Get the last element of a given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetCacheLast<T>() where T : Element
        {
            var element = Elements.Where(e => e is T).FirstOrDefault();

            return element == null ? null : (element as T);
        }

        /// <summary>
        /// Get all elements of a given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetCacheAll<T>(bool? isFavorite = null, bool? isFlorilege = null) where T : Element
        {
            var elements = Elements.Where(e => e is T).Select(e => e as T).ToList();

            if (isFavorite.HasValue && isFavorite.Value)
            {
                elements = elements.Where(e => e.IsFavorite).ToList();
            }
            if (isFlorilege.HasValue && isFlorilege.Value)
            {
                elements.Sort(delegate(T e1, T e2) { return e2.VoteCount.CompareTo(e1.VoteCount); });
                elements = elements.Take(20).ToList();
            }

            return elements;
        }

        /// <summary>
        /// Get all elements of the given type newer than the given element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetCacheNewElementsFrom<T>(T latestElement) where T : Element
        {
            List<T> result = new List<T>();

            foreach (var element in Elements)
            {
                if (element.Date > latestElement.Date)
                {
                    if (element is T)
                    {
                        result.Add(element as T);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get elements with pagination
        /// </summary>
        /// <param name="firstElementIndex"></param>
        /// <param name="elementsCount"></param>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public List<T> GetCacheElements<T>(int firstElementIndex, int elementsCount, string searchQuery) where T : Element
        {
            List<T> list = new List<T>();

            var elementsList = GetCacheAll<T>();
            elementsList = ApplyFilter<T>(elementsList, searchQuery);

            for (int i = firstElementIndex; i < firstElementIndex + elementsCount; i++)
            {
                if (i < elementsList.Count)
                {
                    list.Add(elementsList[i] as T);
                }
            }

            return list;
        }

        /// <summary>
        /// Get an element from its id
        /// </summary>
        /// <param name="m_elementId"></param>
        /// <returns></returns>
        public T GetCacheElementFromId<T>(int elementId) where T : Element
        {
            var list = Elements.Where(n => n.Id == elementId && n is T).Select(n => n as T);

            return list.FirstOrDefault();
        }

        /// <summary>
        /// Apply a textual filter
        /// </summary>
        /// <param name="globalList"></param>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public static List<T> ApplyFilter<T>(List<T> globalList, string searchQuery) where T : Element
        {
            List<T> buffer = new List<T>();

            foreach (var element in globalList)
            {
                bool add = true;

                if (string.IsNullOrEmpty(searchQuery) == false)
                {
                    add = false;

                    if (element.Title.ToLowerInvariant().Contains(searchQuery.ToLowerInvariant()))
                    {
                        add = true;
                    }
                }

                if (add)
                {
                    buffer.Add(element);
                }
            }

            return buffer;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }

}

