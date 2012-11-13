using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CulturezVous.Service.Data.Elements
{
    public abstract class Element
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public int FavoriteCount { get; set; }

        public Author Author { get; set; }
    }
}
