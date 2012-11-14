using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CulturezVous.Service.Areas.Admin.Models
{
    public class PaginationModel
    {
        public string Action { get; set; }

        public string Controller { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPage { get; set; }
    }
}