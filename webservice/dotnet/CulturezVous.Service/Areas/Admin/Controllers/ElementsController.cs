using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CulturezVous.Service.Areas.Admin.Models;
using CulturezVous.Service.Data.Elements.Dao;
using System.Configuration;
using CulturezVous.Service.Data.Elements;

namespace CulturezVous.Service.Areas.Admin.Controllers
{
    public class ElementsController : Controller
    {
        private static string cvDb = ConfigurationManager.ConnectionStrings["CV_DB"].ToString();

        private static int elementsPerPage = 14;

        //
        // GET: /elements/

        public ActionResult Index(int page = 1)
        {
            if (page < 1) page = 1;

            AdminElementsViewModel viewModel = new AdminElementsViewModel();

            ElementDao dao = new ElementDao(cvDb);
            var elements = dao.GetAllElements(true);

            if (dao.LastException != null) throw dao.LastException;

            viewModel.Pagination = new PaginationModel();
            viewModel.Pagination.CurrentPage = page;
            viewModel.Pagination.TotalPage = (elements.Count / elementsPerPage);
            viewModel.Pagination.Action = "Index";
            viewModel.Pagination.Controller = "Elements";

            viewModel.Elements = elements.Skip((page - 1) * elementsPerPage).Take(elementsPerPage).ToList();

            return View(viewModel);
        }

        //
        // GET: /elements/add -> edit sans id

        public ActionResult Create(string type = null)
        {
            return Edit(null, type);
        }

        //
        // GET: /elements/edit

        public ActionResult Edit(int? id = null, string type = null)
        {
            ElementViewModel viewModel = new ElementViewModel();

            viewModel.IsCreation = (id.HasValue == false);
            viewModel.Type = type;

            // Trouver une date libre
            ElementDao dao = new ElementDao(cvDb);
            var elements = dao.GetAllElements(true);

            DateTime d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (type == "word")
            {
                while (d.DayOfWeek == DayOfWeek.Sunday || elements.Where(e => e.Type.ToLower() == type && e.Date == d).Count() > 0)
                {
                    d = d.AddDays(1);
                }
            }
            else
            {
                while (d.DayOfWeek != DayOfWeek.Sunday || elements.Where(e => e.Type.ToLower() == type && e.Date == d).Count() > 0)
                {
                    d = d.AddDays(1);
                }
            }
            viewModel.Date = d;


            if (id.HasValue)
            {
                Element e = dao.GetElementById(id.Value);

                if (e != null)
                {
                    viewModel = loadViewModel(e);
                }
                else
                {
                    throw new ArgumentException("Element inconnu : " + id.Value);
                }
            }

            return View("Edit", viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ElementViewModel model)
        {
            // Contrôles
            bool valid = true;

            ElementDao dao = new ElementDao(cvDb);
            var elements = dao.GetAllElements(true);

            if (model.AuthorId <= 0)
            {
                valid = false;
                model.Message = "Auteur manquant";
            }
            else if (string.IsNullOrEmpty(model.Title))
            {
                valid = false;
                model.Message = "Titre manquant";
            }
            else if (elements.Where(e => e.Date == new DateTime(model.Date.Year, model.Date.Month, model.Date.Day)).Count() > 0)
            {
                valid = false;
                model.Message = "Date déjà occupée !";
            }

            if (valid)
            {
                AuthorDao autdao = new AuthorDao(cvDb);
                var authors = autdao.GetAuthors();

                Element e = createElement(model);

                // Création
                if (model.ElementId == 0)
                {
                    dao.Create(e);
                }
                else
                {
                    dao.Update(e);
                }

                if (dao.LastException != null)
                {
                    model.Message = dao.LastException.ToString();
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            return View(model);

        }

        //
        // GET: /elements/delete

        public ActionResult Delete(int id, string type = null)
        {
            ElementDao dao = new ElementDao(cvDb);
            Element e = dao.GetElementById(id);
            ElementViewModel viewModel = new ElementViewModel();

            if (e != null)
            {
                viewModel = loadViewModel(e);
            }
            else
            {
                throw new ArgumentException("Element inconnu : " + id);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Delete(ElementViewModel model)
        {
            ElementDao dao = new ElementDao(cvDb);
            dao.Delete(model.ElementId);

            return RedirectToAction("Index");
        }

        private static ElementViewModel loadViewModel(Element e)
        {
            ElementViewModel viewModel = new ElementViewModel();

            viewModel.Title = e.Title;
            viewModel.Type = e.Type;
            viewModel.ElementId = e.Id;
            viewModel.Date = e.Date;
            viewModel.VoteCount = e.FavoriteCount;
            viewModel.AuthorId = e.Author.Id;

            viewModel.Type = e.Type;

            if (e is Word)
            {
                Word word = e as Word;

                if (word.Definitions.Count > 0)
                {
                    viewModel.Details1 = word.Definitions[0].Details;
                    viewModel.Content1 = word.Definitions[0].Content;
                }
                if (word.Definitions.Count > 1)
                {
                    viewModel.Details2 = word.Definitions[1].Details;
                    viewModel.Content2 = word.Definitions[1].Content;
                }
                if (word.Definitions.Count > 2)
                {
                    viewModel.Details3 = word.Definitions[2].Details;
                    viewModel.Content3 = word.Definitions[2].Content;
                }
            }
            else if (e is Contrepeterie)
            {
                Contrepeterie ctp = e as Contrepeterie;

                viewModel.Solution = ctp.Solution;
                viewModel.Content = ctp.Content;
            }

            return viewModel;
        }

        private static Element createElement(ElementViewModel viewModel)
        {
            AuthorDao autdao = new AuthorDao(cvDb);
            var authors = autdao.GetAuthors();

            Element e = null;

            if (viewModel.Type == "word")
            {
                Word w = new Word();

                if (string.IsNullOrEmpty(viewModel.Details1) == false && string.IsNullOrEmpty(viewModel.Content1) == false)
                {
                    w.Definitions.Add(new Definition()
                    {
                        Id = viewModel.IdDef1,
                        Content = viewModel.Content1,
                        Details = viewModel.Details1,
                        WordId = w.Id
                    });
                }
                if (string.IsNullOrEmpty(viewModel.Details2) == false && string.IsNullOrEmpty(viewModel.Content2) == false)
                {
                    w.Definitions.Add(new Definition()
                    {
                        Id = viewModel.IdDef2,
                        Content = viewModel.Content2,
                        Details = viewModel.Details2,
                        WordId = w.Id
                    });
                }
                if (string.IsNullOrEmpty(viewModel.Details3) == false && string.IsNullOrEmpty(viewModel.Content3) == false)
                {
                    w.Definitions.Add(new Definition()
                    {
                        Id = viewModel.IdDef3,
                        Content = viewModel.Content3,
                        Details = viewModel.Details3,
                        WordId = w.Id
                    });
                }

                e = w;
            }
            else
            {
                Contrepeterie ctp = new Contrepeterie();

                ctp.Content = viewModel.Content;
                ctp.Solution = viewModel.Solution;

                e = ctp;
            }

            e.Author = authors.Where(a => a.Id == viewModel.AuthorId).FirstOrDefault();
            e.Id = viewModel.ElementId;
            e.Title = viewModel.Title;
            e.Date = viewModel.Date;
            e.FavoriteCount = viewModel.VoteCount;

            return e;
        }
    }
}
