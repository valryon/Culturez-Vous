using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CulturezVous.Service.Data.Elements
{
    [DataContract]
    public class Author
    {
        [IgnoreDataMember]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "info")]
        public string Info { get; set; }
    }
}

