using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CulturezVous.Service.Data.Elements;
using System.ComponentModel.DataAnnotations;
using CulturezVous.Service.Data.Elements.Dao;
using System.Configuration;

namespace CulturezVous.Service.Areas.Admin.Models
{
    public class ElementViewModel
    {
        public ElementViewModel()
        {
            AuthorDao autdao = new AuthorDao(ConfigurationManager.ConnectionStrings["CV_DB"].ToString());
            Authors = autdao.GetAuthors();
        }

        public string Message { get; set; }

        public bool IsCreation { get; set; }
        public string Type { get; set; }

        public int ElementId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int VoteCount { get; set; }

        public int AuthorId { get; set; }

        public List<Author> Authors { get; set; }

        public int IdDef1 { get; set; }
        public string Details1 { get; set; }
        public string Content1 { get; set; }
        public int IdDef2 { get; set; }
        public string Details2 { get; set; }
        public string Content2 { get; set; }
        public int IdDef3 { get; set; }
        public string Details3 { get; set; }
        public string Content3 { get; set; }

        public string Content { get; set; }
        public string Solution { get; set; }
    }
}