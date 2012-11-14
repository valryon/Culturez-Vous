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
            var elements = dao.GetAllElements();

            viewModel.Pagination = new PaginationModel();
            viewModel.Pagination.CurrentPage = page;
            viewModel.Pagination.TotalPage = (elements.Count / elementsPerPage);
            viewModel.Pagination.Action = "Index";
            viewModel.Pagination.Controller = "Elements";

            viewModel.Elements = elements.Skip(page * elementsPerPage).Take(elementsPerPage).ToList();

            return View(viewModel);
        }

        //
        // GET: /elements/edit

        public ActionResult Edit(int? id = null, string type = null)
        {
            ElementViewModel viewModel = new ElementViewModel();

            viewModel.IsCreation = (id.HasValue == false);
            viewModel.Type = type;

            if (id.HasValue)
            {
                ElementDao dao = new ElementDao(cvDb);
                Element e = dao.GetElementById(id.Value);

                if (e != null)
                {
                    viewModel.Title = e.Title;
                    viewModel.Type = e.Type;
                    viewModel.ElementId = e.Id;
                    viewModel.Date = e.Date;
                    viewModel.VoteCount = e.FavoriteCount;
                    viewModel.Author = e.Author.Name;
                }
                else
                {
                    throw new ArgumentException("Element inconnu : " + id.Value);
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ElementViewModel model)
        {
            return RedirectToAction("Index");
        }

        //
        // GET: /elements/delete

        public ActionResult Delete(int id, string type = null)
        {
            return View();
        }
    }
}
