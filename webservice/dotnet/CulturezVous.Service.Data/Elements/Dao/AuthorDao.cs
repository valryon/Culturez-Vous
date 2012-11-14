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
    /// Récupération des auteurs
    /// </summary>
    public class AuthorDao : MySqlDao
    {
        public AuthorDao(string connectionString)
            : base(connectionString)
        {

        }

        /// <summary>
        /// Récupération de tous les auteurs
        /// </summary>
        /// <returns></returns>
        public List<Author> GetAuthors()
        {
            List<Author> authors = new List<Author>();

            bool exec = ExecuteReader("SELECT * FROM authors", System.Data.CommandType.Text, (reader) =>
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Author a = parseAuthor(reader);

                        authors.Add(a);
                    }
                }
            });

            return authors;
        }

        private Author parseAuthor(DbDataReader reader)
        {
            Author a = new Author();

            a.Id = Convert.ToInt32(reader["author_id"].ToString());
            a.Name = reader["author_name"].ToString();
            a.Info = reader["author_info"].ToString();

            return a;
        }


    }
}
