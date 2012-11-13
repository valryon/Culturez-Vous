using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CulturezVous.Service.Data.Db;
using System.Data.Common;

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
