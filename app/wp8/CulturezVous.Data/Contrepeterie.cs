using System;
using System.Data.Linq.Mapping;

namespace CulturezVous.Data
{
    /// <summary>
    /// A special element : contrepetrie
    /// </summary>
    public class Contrepeterie : Element
    {
        [Column]
        public string Content { get; set; }

        [Column]
        public string Solution { get; set; }
    }
}
