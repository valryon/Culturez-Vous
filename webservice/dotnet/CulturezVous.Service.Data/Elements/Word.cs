using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CulturezVous.Service.Data.Elements
{
    [DataContract]
    public class Word : Element
    {
        public Word()
            : base()
        {
            Definitions = new List<Definition>();
        }

        [DataMember(Name = "definitions")]
        public List<Definition> Definitions { get; set; }
    }
}
