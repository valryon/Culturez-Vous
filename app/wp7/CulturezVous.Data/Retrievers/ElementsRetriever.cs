using System;
using System.Collections.ObjectModel;
using CulturezVous.Data;
using CulturezVous.Data.Cache;
using System.Collections.Generic;

namespace CulturezVous.WP7.Data.Retrievers
{
    /// <summary>
    /// Data source
    /// </summary>
    public abstract class ElementsRetriever
    {
        public ElementsRetriever()
        {
        }

        /// <summary>
        /// Connection test
        /// </summary>
        /// <param name="testCompleted"></param>
        public abstract void TestConnectionAsync(Action<bool> testCompleted);

        /// <summary>
        /// Get Data according to the last known update
        /// </summary>
        public virtual void GetLastDataAsync(Action<List<Element>> downloadComplete)
        {
            // Get the last news & last element
            GetServerLastElementAsync(lastElement =>
            {
                var newestCacheElement = CacheManager.Instance.GetCacheLast<Element>();

                if (lastElement != null && lastElement.Id != newestCacheElement.Id)
                {
                    // There is some new things on the server!
                    GetNewElementsAsync(newestCacheElement.Id, (elements) =>
                    {
                        if (downloadComplete != null)
                        {
                            downloadComplete(elements);
                        }
                    });
                }
                else
                {
                    if (downloadComplete != null)
                    {
                        downloadComplete(new List<Element>());
                    }
                }
            });
        }

        /// <summary>
        /// Get the server last element
        /// </summary>
        /// <returns></returns>
        public abstract void GetServerLastElementAsync(Action<Element> downloadComplete);

        /// <summary>
        /// Get elements page by page
        /// </summary>
        /// <param name="downloadPageComplete"></param>
        /// <param name="downloadComplete"></param>
        /// <param name="firstPage"></param>
        /// <param name="pageLimit"></param>
        public abstract void GetElementsPaginatedAsync(Action<List<Element>> downloadPageComplete, Action downloadComplete, int firstPage, int? pageLimit);

        /// <summary>
        /// Get elements 
        /// </summary>
        /// <returns></returns>
        public abstract void GetNewElementsAsync(int lastId, Action<List<Element>> downloadComplete);

        /// <summary>
        /// Get an element from the date
        /// </summary>
        /// <param name="date"></param>
        /// <param name="downloadComplete"></param>
        public abstract void GetElementFromDateAsync(DateTime date, Action<Element> downloadComplete);

        /// <summary>
        /// Get 20 best elements
        /// </summary>
        /// <param name="downloadComplete"></param>
        public abstract void GetBestOf(Action<List<Element>> downloadComplete);

        /// <summary>
        /// Data source is available (3G / local)
        /// </summary>
        public abstract bool IsAvailable
        {
            get;
            protected set;
        }
    }
}
