using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CulturezVous.Service.Data.Elements
{
    [DataContract(Name="definition")]
    public class Definition
    {
        [IgnoreDataMember]
        public int Id { get; set; }

        [IgnoreDataMember]
        public int WordId { get; set; }

        [DataMember(Name = "details")]
        public string Details { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }
    }
}
