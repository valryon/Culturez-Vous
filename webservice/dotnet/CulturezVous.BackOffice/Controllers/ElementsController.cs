using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using CulturezVous.Service.Areas.Admin.Models;
using CulturezVous.Service.Data.Elements;
using CulturezVous.Service.Data.Elements.Dao;
using CulturezVous.Service.Models;

namespace CulturezVous.BackOffice.Controllers
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
                    viewModel.LoadFromElement(e);
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

                Element e = model.ToElement();

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
                viewModel.LoadFromElement(e);
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
       
    }
}
