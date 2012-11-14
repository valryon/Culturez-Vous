using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CulturezVous.Service.Data.Elements;
using System.ComponentModel.DataAnnotations;

namespace CulturezVous.Service.Areas.Admin.Models
{
    public class ElementViewModel
    {
        public bool IsCreation { get; set; }
        public string Type { get; set; }

        public int ElementId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int VoteCount { get; set; }

        public string Author { get; set; }
    }
}