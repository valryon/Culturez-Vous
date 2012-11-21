using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CulturezVous.Service.Data.Elements
{
    [DataContract]
    public class Contrepeterie : Element
    {
        [DataMember(Name="content")]
        public string Content { get; set; }

        [DataMember(Name = "solution")]
        public string Solution { get; set; }
    }
}
