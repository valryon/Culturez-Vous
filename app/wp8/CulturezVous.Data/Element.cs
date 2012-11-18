using System;
using System.Data.Linq.Mapping;
using System.Data.Linq;

namespace CulturezVous.Data
{
    /// <summary>
    /// Basic element of content
    /// </summary>
    [Table]
    [InheritanceMapping(Code = "W", Type = typeof(Word))]
    [InheritanceMapping(Code = "CTP", Type = typeof(Contrepeterie))]
    [InheritanceMapping(Code = "E", Type = typeof(Element), IsDefault = true)]
    public class Element
    {
        /// <summary>
        /// Item version for SQL Mapping
        /// </summary>
        [Column(IsVersion = true)]
        protected Binary version;

        /// <summary>
        /// Item type for SQL Mapping
        /// </summary>
        [Column(IsDiscriminator = true)]
        protected string discKey;

        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column]
        public virtual string Title { get; set; }

        [Column]
        public virtual DateTime Date { get; set; }

        [Column]
        public virtual bool IsFavorite { get; set; }

        [Column]
        public virtual int VoteCount { get; set; }

        [Column]
        public virtual string Author { get; set; }

        [Column]
        public virtual string AuthorInfo { get; set; }

        [Column]
        public bool IsRead { get; set; }

        public override string ToString()
        {
            return Title + " - " + Date;
        }
    }
}
