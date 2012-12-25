using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CulturezVous.Service.Data.Elements;

namespace CulturezVous.Service.Areas.Admin.Models
{
    public class AdminElementsViewModel
    {
        public List<Element> Elements { get; set; }

        public PaginationModel Pagination { get; set; }
    }
}