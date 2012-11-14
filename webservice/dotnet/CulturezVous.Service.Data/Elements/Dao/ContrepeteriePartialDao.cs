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
    /// Gestion de la contrepeterie
    /// </summary>
    public class ContrepeteriePartialDao : MySqlDao
    {
        public ContrepeteriePartialDao(string connectionString)
            : base(connectionString)
        {

        }

        /// <summary>
        /// Récupère les contrepétries en mode partiel
        /// </summary>
        /// <returns></returns>
        public List<ContreperiePartial> GetContrepeteriesPartial()
        {
            List<ContreperiePartial> ctps = new List<ContreperiePartial>();

            bool exec = ExecuteReader("SELECT * FROM contrepetries", System.Data.CommandType.Text, (reader) =>
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ContreperiePartial c = parseCtp(reader);

                        ctps.Add(c);
                    }
                }
            });

            return ctps;
        }

        public bool DeleteByElementId(int id)
        {
            int execCtp = ExecuteNonQuery("DELETE FROM contrepetries WHERE element_id = @id", System.Data.CommandType.Text, new MySqlParameter("@id", id));

            return execCtp > 0;
        }

        public bool Create(Contrepeterie contrepeterie)
        {
            string sql = "INSERT INTO contrepetries(element_id, contrepetrie_content, contrepetrie_solution) VALUES (@id,@content,@solution);"
              + "select last_insert_id();";
            object ret = ExecuteScalar(sql, System.Data.CommandType.Text
                , new MySqlParameter("@id", contrepeterie.Id)
                , new MySqlParameter("@solution", contrepeterie.Solution)
                , new MySqlParameter("@content", contrepeterie.Content)
                );

            return ret != null;
        }

        private ContreperiePartial parseCtp(DbDataReader reader)
        {
            ContreperiePartial c = new ContreperiePartial();

            c.Id = Convert.ToInt32(reader["contrepetrie_id"].ToString());
            c.CtpId = Convert.ToInt32(reader["element_id"].ToString());
            c.Content = reader["contrepetrie_content"].ToString();
            c.Solution = reader["contrepetrie_solution"].ToString();

            return c;
        }


    }
}
