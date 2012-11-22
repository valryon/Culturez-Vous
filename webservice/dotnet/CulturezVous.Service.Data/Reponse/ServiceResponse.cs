using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using CulturezVous.Service.Data.Elements;

namespace CulturezVous.Service.Data.Response
{
    /// <summary>
    /// Réponse retournée par le service
    /// </summary>
    [DataContract]
    public class ServiceResponse
    {
        /// <summary>
        /// Code d'erreur
        /// </summary>
        [DataMember(Name = "code", IsRequired = true)]
        public int Code { get; set; }

        /// <summary>
        /// Message d'erreur
        /// </summary>
        [DataMember(Name = "message", EmitDefaultValue = false)]
        public string Message { get; set; }

        /// <summary>
        /// Réponse sérialisée
        /// </summary>
        [DataMember(Name = "r", EmitDefaultValue = false)]
        public object ResponseData { get; set; }
    }
}