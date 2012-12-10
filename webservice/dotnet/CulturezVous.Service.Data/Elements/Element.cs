using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CulturezVous.Service.Data.Elements
{
    [DataContract]
    public abstract class Element
    {
        [DataMember(Name="id")]
        public int Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "date")]
        public DateTime Date { get; set; }

        [DataMember(Name = "votes")]
        public int FavoriteCount { get; set; }

        [DataMember(Name = "author")]
        public Author Author { get; set; }

        [DataMember(Name = "type")]
        public string Type
        {
            get
            {
                return (GetType().Name);
            }
        }

        public Element()
        {
            Author = new Author();
        }
    }
}
