using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using CulturezVous.Data;
using Newtonsoft.Json.Linq;

namespace CulturezVous.WP7.Data.Retrievers
{
    /// <summary>
    /// Load elements from a local JSON test file
    /// </summary>
    public class TestElementsRetriever : ElementsRetriever
    {
        private List<Element> m_elements;
        private DateTime m_launchTime;
        private static int m_fakeCount = 999;

        public TestElementsRetriever()
        {
            // Load JSON files
            var bouchonData = System.Windows.Application.GetResourceStream(new Uri("/CulturezVous.WP7;component/Resources/Bouchons/bouchon_data.json", UriKind.Relative));
            var streamReaderData = new StreamReader(bouchonData.Stream);
            string json = streamReaderData.ReadToEnd();
            JObject data = JObject.Parse(json);

            m_elements = ExtractElementsFromData(data);

            IsAvailable = true;

            launchFakeUpdater();
        }

        private void launchFakeUpdater()
        {
            m_launchTime = DateTime.Now;

            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler((o, a) =>
            {
                while (true)
                {
                    Thread.Sleep(15000);

                    IsAvailable = false;

                    m_fakeCount++;

                    var rand = new Random(DateTime.Now.Millisecond);
                    int n = rand.Next(2);

                    Element randelement = null;

                    // Create a random word
                    if (n % 2 == 0)
                    {
                        randelement = new Word()
                        {
                            Id = m_fakeCount,
                            Definitions = new List<Definition>()
                            {
                                new Definition(){
                                    Content = "Definition 1 blabablla",
                                    Details = "bla blabla blabla blabla blabla blabla blabla blabla blabla bla bla bla bla bla"
                                },
                                new Definition(){
                                    Content = "Definition 1 blabablla",
                                    Details = "bla blabla blabla blabla blabla blabla blabla blabla blabla bla bla bla bla bla"
                                },
                            },
                            Title = "Generated Word no " + m_fakeCount,
                            Date = DateTime.Now,
                            VoteCount = 42,
                            Author = "Dam",
                            AuthorInfo = "Tocard"
                        };

                        ((Word)randelement).Definitions.ForEach(d =>
                        {
                            d.Word = randelement as Word;
                        });
                    }
                    // Or create a random contrepeterie
                    else
                    {
                        randelement = new Contrepeterie()
                        {
                            Id = m_fakeCount,
                            Title = "Generated CTP no " + m_fakeCount,
                            Content = "Bla bla bllllaaa dsfsdfsdfsd",
                            Solution = "!!!!!!!!!!!!!!!!!!!!!!!!!",
                            Date = DateTime.Now,
                            VoteCount = 42,
                            Author = "Dam",
                            AuthorInfo = "Tocard"
                        };
                    }

                    m_elements.Insert(0, randelement);

                    IsAvailable = true;
                    Debug.WriteLine("Generated: " + randelement.Title);
                }

            });
            worker.RunWorkerAsync();
        }

        private List<Element> ExtractElementsFromData(JObject data)
        {
            List<Element> elements = new List<Element>();

            JArray jaElements = (JArray)data["elements"];

            foreach (var e in jaElements.Children())
            {
                Element newElement = null;

                switch (e["type"].ToString())
                {
                    case "word":

                        List<Definition> definitions = new List<Definition>();
                        foreach (var def in (JArray)e["definitions"])
                        {
                            definitions.Add(new Definition()
                            {
                                Details = def["details"].ToString(),
                                Content = def["content"].ToString()
                            });
                        }

                        newElement = new Word()
                        {
                            Id = Convert.ToInt32(e["id"].ToString()),
                            Date = Convert.ToDateTime(e["date"].ToString()),
                            Title = e["title"].ToString(),
                            IsFavorite = false,
                            VoteCount = Convert.ToInt32(e["voteCount"].ToString()),
                            Author = e["author"].ToString(),
                            AuthorInfo = e["authorInfo"].ToString(),
                            Definitions = definitions
                        };

                        break;

                    case "contrepeterie":
                        newElement = new Contrepeterie()
                        {
                            Id = Convert.ToInt32(e["id"].ToString()),
                            Date = Convert.ToDateTime(e["date"].ToString()),
                            Title = e["title"].ToString(),
                            IsFavorite = false,
                            VoteCount = Convert.ToInt32(e["voteCount"].ToString()),
                            Author = e["author"].ToString(),
                            AuthorInfo = e["authorInfo"].ToString(),
                            Content = e["content"].ToString(),
                            Solution = e["solution"].ToString()
                        };
                        break;

                    default:
                        throw new ArgumentException("Unknow type: " + e["type"]);
                }

                elements.Add(newElement);
            }

            // Sort by date
            elements.Sort(delegate(Element e1, Element e2) { return e2.Date.CompareTo(e1.Date); });

            return elements;
        }

        public override void TestConnectionAsync(Action<bool> testCompleted)
        {
            testCompleted(true);
        }

        public override void GetServerLastElementAsync(Action<Element> downloadComplete)
        {
            var element = m_elements.First();

            if (downloadComplete != null) downloadComplete(element);
        }

        public override void GetNewElementsAsync(int id, Action<List<Element>> downloadComplete)
        {
            var element = m_elements.Where(e => e.Id > id).ToList();

            if (downloadComplete != null) downloadComplete(element);
        }

        public override void GetElementsPaginatedAsync(Action<List<Element>> downloadPageComplete, Action downloadComplete, int firstPage, int? pageLimit)
        {
            // No page simulation
            downloadPageComplete(m_elements);

            if (downloadComplete != null) downloadComplete();
        }

        public override void GetElementFromDateAsync(DateTime date, Action<Element> downloadComplete)
        {
            var element = m_elements.Where(e => e.Date.Date == date.Date).SingleOrDefault();

            if (downloadComplete != null) downloadComplete(element);
        }

        public override void GetBestOf(Action<List<Element>> downloadComplete)
        {
            downloadComplete(m_elements.Take(20).ToList());
        }

        public override bool IsAvailable
        {
            get;
            protected set;
        }


    }
}
