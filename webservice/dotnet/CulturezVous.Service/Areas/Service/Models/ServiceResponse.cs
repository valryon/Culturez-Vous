using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace CulturezVous.Service.Areas.Service.Models
{
    [DataContract]
    public class ServiceResponse
    {
        [DataMember(Name = "code", IsRequired = true)]
        public int Code { get; set; }

        [DataMember(Name = "message", EmitDefaultValue = false)]
        public string Message { get; set; }

        [DataMember(Name = "r", EmitDefaultValue = false)]
        public object ResponseData { get; set; }
    }
}