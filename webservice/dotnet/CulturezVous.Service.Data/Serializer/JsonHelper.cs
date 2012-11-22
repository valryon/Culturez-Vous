using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace CulturezVous.Service.Data.Serializer
{
    /// <summary>
    /// Utiliaires pour la sérialization en JSON
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Sérialize un objet en Json en utilisant le data contract
        /// </summary>
        /// <param name="data"></param>
        /// <param name="indented"></param>
        /// <returns></returns>
        public static string Serialize(object data, bool indented = false)
        {
            string json = JsonConvert.SerializeObject(data, (indented ? Formatting.Indented : Formatting.None)
                , new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
            );

            return json;
        }

        /// <summary>
        /// Tente de désérializer un json en objet d'un type donné
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            object obj = JsonConvert.DeserializeObject(json, typeof(T)
                , new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
            );

            if (obj is T)
            {
                return (T)obj;
            }

            return default(T);
        }
    }
}
