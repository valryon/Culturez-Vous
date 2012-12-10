using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CulturezVous.Service.Data.Elements;
using System.Net;
using System.Xml.Linq;
using CulturezVous.Service.Data.Elements.Dao;
using System.Configuration;

namespace CulturezVous.BackOffice.Controllers
{
    [Authorize]
    public class ImportController : Controller
    {
        private static string cvDb = ConfigurationManager.ConnectionStrings["CV_DB"].ToString();

        //
        // GET: /Import/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Import()
        {
            object results = "Pouet";
            ElementDao dao = new ElementDao(cvDb);
            AuthorDao authDao = new AuthorDao(cvDb);

            List<Author> authors = authDao.GetAuthors();
            List<Element> newWsElementsId = dao.GetAllElements(true).ToList();

            // Appel à l'ancien WS
            OldWSRetriever retriever = new OldWSRetriever();
            bool mergeComplete = false;
            int page = 1;
            int count = 0;

            while (mergeComplete == false)
            {
                List<Element> elements = retriever.GetElements(page);

                if (elements == null || elements.Count == 0)
                {
                    mergeComplete = true;
                }
                else
                {
                    foreach (Element oldE in elements)
                    {
                        if (newWsElementsId.Where(newE => newE.Title.ToLowerInvariant() == oldE.Title.ToLowerInvariant()).Count() == 0)
                        {
                            // Auteur
                            Author aut = authors.Where(a => a.Name.ToLower() == oldE.Author.Name.ToLowerInvariant()).FirstOrDefault();

                            if (aut != null)
                            {
                                oldE.Author = aut;

                                count++;

                                // Ajout
                                if (dao.Create(oldE) == false)
                                {
                                    throw dao.LastException;
                                }
                            }
                        }
                        else
                        {
                            mergeComplete = true;
                            break;
                        }
                    }

                    if (mergeComplete == false)
                    {
                        page++;
                    }
                }
            }

            results = count + " élements ajoutés sur " + page + " pages.";

            return View(results);
        }
    }

    /// <summary>
    /// Retrieve data from the CV webservice
    /// </summary>
    internal class OldWSRetriever
    {
        public OldWSRetriever()
            : base()
        {
        }

        /// <summary>
        /// Uri to get the last lement
        /// </summary>
        public Uri GetPagedElementsUri(int page)
        {
            return new Uri("http://thegreatpaperadventure.com/CulturezVous/index.php/element/page/" + page);
        }

        /// <summary>
        /// Uri to get a specific element
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private Uri GetDateElementUri(DateTime date)
        {
            return new Uri("http://thegreatpaperadventure.com/CulturezVous/index.php/element/date/" + date.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// Uri to get the best of
        /// </summary>
        public Uri BestOfElementsUri
        {
            get
            {
                return new Uri("http://thegreatpaperadventure.com/CulturezVous/index.php/element/bestof");
            }
        }

        /// <summary>
        /// Uri to get the last lement
        /// </summary>
        public Uri LastElementsUri
        {
            get
            {
                return new Uri("http://thegreatpaperadventure.com/CulturezVous/index.php/element");
            }
        }

        public List<Element> GetElements(int page)
        {
            return makeRequest(GetPagedElementsUri(page));
        }

        /// <summary>
        /// Make an HTTP request and retrieve the string content
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="downloadComplete"></param>
        private List<Element> makeRequest(Uri uri)
        {
            var webclient = new WebClient();
            webclient.Encoding = System.Text.Encoding.UTF8;

            string result = webclient.DownloadString(uri);
            List<Element> elements = parseResponse(result);

            return elements;
        }

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
                int id = Convert.ToInt32(element.Element("id").Value);
                string type = element.Element("type").Value;
                DateTime date = Convert.ToDateTime(element.Element("date").Value);
                string title = element.Element("title").Value.Replace(@"\", "");
                int voteCount = Convert.ToInt32(element.Element("voteCount").Value);

                if (element.Element("author") == null && element.Element("authorInfo") == null)
                {
                    continue;
                }

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
                            FavoriteCount = voteCount,
                            Definitions = definitions
                        };
                        w.Author.Info = authorInfo;
                        w.Author.Name = author;

                        newElement = w;

                        break;

                    case "contrepétrie":

                        Contrepeterie c = new Contrepeterie()
                        {
                            Id = id,
                            Date = date,
                            Title = title,
                            FavoriteCount = voteCount,
                            Content = element.Element("content").Value.Replace(@"\", ""),
                            Solution = element.Element("solution").Value.Replace(@"\", ""),
                        };
                        c.Author.Info = authorInfo;
                        c.Author.Name = author;

                        newElement = c;

                        break;

                    default:
                        continue;
                }

                elements.Add(newElement);
            }

            return elements;
        }

    }
}
