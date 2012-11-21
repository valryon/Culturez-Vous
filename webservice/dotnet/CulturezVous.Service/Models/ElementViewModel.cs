using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CulturezVous.Service.Data.Elements;
using System.ComponentModel.DataAnnotations;
using CulturezVous.Service.Data.Elements.Dao;
using System.Configuration;

namespace CulturezVous.Service.Models
{
    public class ElementViewModel
    {
        private static string cvDb = ConfigurationManager.ConnectionStrings["CV_DB"].ToString();

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

        public ElementViewModel()
        {
            AuthorDao autdao = new AuthorDao(cvDb);
            Authors = autdao.GetAuthors();
        }

        public ElementViewModel(Element e)
        {
            LoadFromElement(e);
        }

        public void LoadFromElement(Element e)
        {
            Title = e.Title;
            Type = e.Type;
            ElementId = e.Id;
            Date = e.Date;
            VoteCount = e.FavoriteCount;
            AuthorId = e.Author.Id;

            Type = e.Type;

            if (e is Word)
            {
                Word word = e as Word;

                if (word.Definitions.Count > 0)
                {
                    Details1 = word.Definitions[0].Details;
                    Content1 = word.Definitions[0].Content;
                }
                if (word.Definitions.Count > 1)
                {
                    Details2 = word.Definitions[1].Details;
                    Content2 = word.Definitions[1].Content;
                }
                if (word.Definitions.Count > 2)
                {
                    Details3 = word.Definitions[2].Details;
                    Content3 = word.Definitions[2].Content;
                }
            }
            else if (e is Contrepeterie)
            {
                Contrepeterie ctp = e as Contrepeterie;

                Solution = ctp.Solution;
                Content = ctp.Content;
            }
        }

        public Element ToElement()
        {
            AuthorDao autdao = new AuthorDao(cvDb);
            var authors = autdao.GetAuthors();

            Element e = null;

            if (Type == "word")
            {
                Word w = new Word();

                if (string.IsNullOrEmpty(Details1) == false && string.IsNullOrEmpty(Content1) == false)
                {
                    w.Definitions.Add(new Definition()
                    {
                        Id = IdDef1,
                        Content = Content1,
                        Details = Details1,
                        WordId = w.Id
                    });
                }
                if (string.IsNullOrEmpty(Details2) == false && string.IsNullOrEmpty(Content2) == false)
                {
                    w.Definitions.Add(new Definition()
                    {
                        Id = IdDef2,
                        Content = Content2,
                        Details = Details2,
                        WordId = w.Id
                    });
                }
                if (string.IsNullOrEmpty(Details3) == false && string.IsNullOrEmpty(Content3) == false)
                {
                    w.Definitions.Add(new Definition()
                    {
                        Id = IdDef3,
                        Content = Content3,
                        Details = Details3,
                        WordId = w.Id
                    });
                }

                e = w;
            }
            else
            {
                Contrepeterie ctp = new Contrepeterie();

                ctp.Content = Content;
                ctp.Solution = Solution;

                e = ctp;
            }

            e.Author = authors.Where(a => a.Id == AuthorId).FirstOrDefault();
            e.Id = ElementId;
            e.Title = Title;
            e.Date = Date;
            e.FavoriteCount = VoteCount;

            return e;
        }
    }
}