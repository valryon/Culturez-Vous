using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CulturezVous.Service.Data.Db;
using System.Data.Common;

namespace CulturezVous.Service.Data.Elements.Dao
{
    /// <summary>
    /// Gestion des définitions en base
    /// </summary>
    public class DefinitionDao : MySqlDao
    {
        public DefinitionDao(string connectionString)
            : base(connectionString)
        {

        }

              /// <summary>
        /// Récupération de tous les auteurs
        /// </summary>
        /// <returns></returns>
        public List<Definition> GetDefinitions()
        {
            List<Definition> defs = new List<Definition>();

            bool exec = ExecuteReader("SELECT * FROM definitions", System.Data.CommandType.Text, (reader) =>
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Definition d = parseAuthor(reader);

                        defs.Add(d);
                    }
                }
            });

            return defs;
        }

        private Definition parseAuthor(DbDataReader reader)
        {
            Definition d = new Definition();

            d.Id = Convert.ToInt32(reader["definition_id"].ToString());
            d.WordId = Convert.ToInt32(reader["element_id"].ToString());
            d.Details = reader["definition_detail"].ToString();
            d.Content = reader["definition_content"].ToString();

            return d;
        }
    }
}

