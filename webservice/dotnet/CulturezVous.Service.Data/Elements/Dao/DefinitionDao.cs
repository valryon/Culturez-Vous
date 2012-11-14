using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CulturezVous.Service.Data.Db;
using System.Data.Common;
using MySql.Data.MySqlClient;

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
                        Definition d = parseDefinition(reader);

                        defs.Add(d);
                    }
                }
            });

            return defs;
        }

        public bool DeleteByElementId(int id)
        {
            int execDef = ExecuteNonQuery("DELETE FROM definitions WHERE element_id = @id", System.Data.CommandType.Text, new MySqlParameter("@id", id));

            return execDef > 0;
        }


        public bool Create(Word word)
        {
            bool ret = true;

            foreach (Definition d in word.Definitions)
            {
                ret &= Create(word.Id, d);
            }

            return ret;
        }

        public bool Create(int elementId, Definition def)
        {
            string sql = "INSERT INTO definitions(element_id, definition_detail, definition_content) VALUES (@id,@details,@content);"
                          + "select last_insert_id();";
            object ret = ExecuteScalar(sql, System.Data.CommandType.Text
                , new MySqlParameter("@id", elementId)
                , new MySqlParameter("@details", def.Details)
                , new MySqlParameter("@content", def.Content)
                );

            if (ret != null)
            {
                def.Id = Convert.ToInt32(ret);
            }

            return ret != null;
        }

        private Definition parseDefinition(DbDataReader reader)
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

