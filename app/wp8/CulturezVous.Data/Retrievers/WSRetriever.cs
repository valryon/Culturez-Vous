using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using CulturezVous.Data;
using System.Net.NetworkInformation;
using CulturezVous.Data.Cache;

namespace CulturezVous.WP8.Data.Retrievers
{
    /// <summary>
    /// Retrieve data from the CV webservice
    /// </summary>
    public class WSRetriever : ElementsRetriever
    {
        public WSRetriever(bool connectivityCheck)
            : base()
        {
            IsAvailable = Microsoft.Phone.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            if (connectivityCheck)
                NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
        }

        /// <summary>
        /// Event on network interface to check availibility
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            TestConnectionAsync(null);
        }

        /// <summary>
        /// Try to reach the webservice
        /// </summary>
        /// <param name="testComplete"></param>
        public override void TestConnectionAsync(Action<bool> testComplete)
        {
            bool available = Microsoft.Phone.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            if (available)
            {
                // Try to reach our website
                var webclient = new WebClient();
                webclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler((o, t) =>
                {
                    available = (t.Error == null);

                    IsAvailable = available;
                    if (testComplete != null) testComplete(available);
                });
                webclient.DownloadStringAsync(new Uri(Settings.WebserviceUrl));
            }
            else
            {
                IsAvailable = available;
                if (testComplete != null) testComplete(available);
            }
        }

        #region Downloader

        /// <summary>
        /// Uri to get the last lement
        /// </summary>
        public Uri GetPagedElementsUri(int page)
        {
            return new Uri(Settings.WebserviceUrl + "element/page/" + page);
        }

        /// <summary>
        /// Uri to get a specific element
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private Uri GetDateElementUri(DateTime date)
        {
            return new Uri(Settings.WebserviceUrl + "element/date/" + date.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// Uri to get the best of
        /// </summary>
        public Uri BestOfElementsUri
        {
            get
            {
                return new Uri(Settings.WebserviceUrl + "element/bestof");
            }
        }

        /// <summary>
        /// Uri to get the last lement
        /// </summary>
        public Uri LastElementsUri
        {
            get
            {
                return new Uri(Settings.WebserviceUrl + "element");
            }
        }

        /// <summary>
        /// Make an HTTP request and retrieve the string content
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="downloadComplete"></param>
        private void makeRequest(Uri uri, Action<List<Element>> downloadComplete)
        {
            var webclient = new WebClient();
            webclient.Encoding = System.Text.Encoding.UTF8;

            webclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler((o, t) =>
            {
                if (t.Error == null)
                {
                    Debug.WriteLine("<-- WS response, parsing...");

                    // Parse response
                    List<Element> elements = parseResponse(t.Result);

                    if (downloadComplete != null)
                    {
                        downloadComplete(elements);
                    }
                }
                else
                {
                    Debug.WriteLine("ERROR: WS download: " + t.Error.Message);
                    //if (downloadFailed != null) downloadFailed();
                }
            });

            webclient.DownloadStringAsync(uri);
#if DEBUG
            Debug.WriteLine("--> WS request to " + uri.ToString());
#endif
        }

        #endregion

        #region Parser

        /// <summary>
        /// Parse the XML response of the webservice
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private List<Element> parseResponse(string xml)
        {
            List<Element> elements = new List<Element>();

            XDocument xdoc = XDocument.Parse(xml);

            foreach (var element in xdoc.Descendants("element"))
            {
                try
                {
                    int id = Convert.ToInt32(element.Element("id").Value);
                    string type = element.Element("type").Value;
                    DateTime date = Convert.ToDateTime(element.Element("date").Value);
                    string title = element.Element("title").Value.Replace(@"\", "");
                    int voteCount = Convert.ToInt32(element.Element("voteCount").Value);
                    string author = element.Element("author").Value;
                    string authorInfo = element.Element("authorInfo").Value;

                    Element newElement = null;

                    switch (type)
                    {
                        case "mot":

                            List<Definition> definitions = new List<Definition>();
                            foreach (XElement defXml in element.Element("definitions").Elements("definition"))
                            {
                                Definition newDef = new Definition()
                                {
                                    Details = defXml.Element("details").Value.Replace(@"\", ""),
                                    Content = defXml.Element("content").Value.Replace(@"\", ""),
                                };

                                definitions.Add(newDef);
                            }

                            Word w = new Word()
                            {
                                Id = id,
                                Date = date,
                                Title = title,
                                IsFavorite = false,
                                IsRead = false,
                                VoteCount = voteCount,
                                Author = author,
                                AuthorInfo = authorInfo,
                                Definitions = definitions
                            };

                            newElement = w;

                            break;

                        case "contrepétrie":

                            Contrepeterie c = new Contrepeterie()
                            {
                                Id = id,
                                Date = date,
                                Title = title,
                                IsFavorite = false,
                                IsRead = false,
                                VoteCount = voteCount,
                                Author = author,
                                AuthorInfo = authorInfo,
                                Content = element.Element("content").Value.Replace(@"\", ""),
                                Solution = element.Element("solution").Value.Replace(@"\", ""),
                            };

                            newElement = c;

                            break;

                        default:
#if DEBUG
                            Debug.WriteLine("WARNING: Ignored element " + id);
#endif
                            continue;
                    }

                    elements.Add(newElement);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERROR: (parsing) " + ex);
                    continue;
                }
            }

            // Link definitions
            elements.Where(e => e is Word).ToList().ForEach(w => ((Word)w).Definitions.ForEach(d =>
           {
               d.Word = w as Word;
           }));

            Debug.WriteLine("--- WS: Parse complete (" + elements.Count + ")");

            return elements;
        }

        #endregion

        #region Data access

        /// <summary>
        /// Download the best of
        /// </summary>
        /// <param name="downloadComplete"></param>
        public override void GetBestOf(Action<List<Element>> downloadComplete)
        {
            makeRequest(BestOfElementsUri,
               list =>
               {
                   if (downloadComplete != null) downloadComplete(list);
               });
        }

        /// <summary>
        /// Get all elements with pagination
        /// </summary>
        /// <param name="downloadPageComplete"></param>
        /// <param name="downloadComplete"></param>
        public override void GetElementsPaginatedAsync(Action<List<Element>> downloadPageComplete, Action downloadComplete, int firstPage, int? pageLimit)
        {
            // Get ALL THE PAGES!!!
            try
            {
                loadPage(downloadPageComplete, downloadComplete, firstPage, 1, pageLimit);
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("ERROR retrieving a page: " + e.Message);
#endif
            }

        }

        internal void loadPage(Action<List<Element>> downloadPageComplete, Action downloadComplete, int page, int pageCount, int? pageLimit)
        {
            makeRequest(GetPagedElementsUri(page),
                list =>
                {
                    if (downloadPageComplete != null) downloadPageComplete(list);

                    bool exit = false;

                    exit = (pageLimit.HasValue && pageCount >= pageLimit.Value);
                    if (!exit) exit = list.Count == 0;

                    // Try to load a new page
                    if (exit == false)
                    {
                        loadPage(downloadPageComplete, downloadComplete, page + 1, pageCount + 1, pageLimit);
                    }
                    // Nothing more to load: exit
                    else
                    {
                        if (downloadComplete != null) downloadComplete();
                    }
                }
            );
        }

        /// <summary>
        /// Get the last server element
        /// </summary>
        /// <param name="downloadComplete"></param>
        public override void GetServerLastElementAsync(Action<Element> downloadComplete)
        {
            makeRequest(LastElementsUri,
                list =>
                {
                    downloadComplete(list.FirstOrDefault());
                }
            );
        }

        /// <summary>
        /// Get elements until we find the last local id
        /// </summary>
        /// <param name="downloadComplete"></param>
        public override void GetNewElementsAsync(int lastId, Action<List<Element>> downloadComplete)
        {
            List<Element> elements = new List<Element>();

            // Download elements until we find the last local id
            getAllElements(downloadComplete, elements, lastId, 1);
        }

        /// <summary>
        /// Get the element from a date (can return null)
        /// </summary>
        /// <param name="date"></param>
        /// <param name="downloadComplete"></param>
        public override void GetElementFromDateAsync(DateTime date, Action<Element> downloadComplete)
        {
            makeRequest(GetDateElementUri(date),
                list =>
                {
                    downloadComplete(list.FirstOrDefault());
                }
            );
        }


        private int getAllElements(Action<List<Element>> downloadComplete, List<Element> elements, int lastId, int page)
        {
            makeRequest(GetPagedElementsUri(page),
                list =>
                {
                    foreach (var element in list)
                    {
                        if (element.Id == lastId)
                        {
                            downloadComplete(elements);
                            return;
                        }
                        else
                        {
                            Debug.WriteLine(string.Format("NEW: ({0}{1} {2})", element.Id, element.Title, element.Date));

                            elements.Add(element);
                        }
                    }

                    getAllElements(downloadComplete, elements, lastId, page + 1);
                }
            );
            return page;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Internet activated ?
        /// </summary>
        public override bool IsAvailable
        {
            get;
            protected set;
        }

        #endregion
    }
}
