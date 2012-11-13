using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CulturezVous.Service.Data.Elements
{
    public class Word : Element
    {
        public Word()
            : base()
        {
            Definitions = new List<Definition>();
        }

        public List<Definition> Definitions { get; set; }
    }
}
