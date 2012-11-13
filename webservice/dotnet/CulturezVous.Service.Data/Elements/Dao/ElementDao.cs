using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CulturezVous.Service.Data.Db;
using System.Data.Common;

namespace CulturezVous.Service.Data.Elements.Dao
{
    /// <summary>
    /// Gestion des éléments en base
    /// </summary>
    public class ElementDao : MySqlDao
    {
        private List<Author> m_authors;
        private List<Definition> m_definitions;
        private List<ContreperiePartial> m_ctpPartials;

        public ElementDao(string connectionString)
            : base(connectionString)
        {
            // On récupère les auteurs et les définitions pour simplifier le remplissage et faire un peu de cache
            AuthorDao autdao = new AuthorDao(connectionString);
            m_authors = autdao.GetAuthors();

            DefinitionDao defdao = new DefinitionDao(connectionString);
            m_definitions = defdao.GetDefinitions();

            ContrepeteriePartialDao ctpPdao = new ContrepeteriePartialDao(connectionString);
            m_ctpPartials = ctpPdao.GetContrepeteriesPartial();
        }

        /// <summary>
        /// Récupère tous les éléments
        /// </summary>
        /// <returns></returns>
        public List<Element> GetAllElements()
        {
            List<Element> element = new List<Element>();

            bool exec = ExecuteReader("SELECT * FROM elements", System.Data.CommandType.Text, (reader) =>
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Element e = parseElement(reader);

                        element.Add(e);
                    }
                }
            });

            return element;
        }

        private Element parseElement(DbDataReader reader)
        {
            Element e = null;

            //	element_id	type_id	element_date	element_title	element_favoriteCount	author_id
            int id = Convert.ToInt32(reader["element_id"]);
            int type = Convert.ToInt32(reader["type_id"]);
            DateTime date = Convert.ToDateTime(reader["element_date"]);
            string title = reader["element_title"].ToString();
            int voteCount = Convert.ToInt32(reader["element_favoriteCount"]);
            int authorId = Convert.ToInt32(reader["author_id"]);

            if (type == 1)
            {
                Word word = new Word()
                {
                    Definitions = m_definitions.Where(d => d.WordId == id).ToList()
                };

                e = word;
            }
            else if (type == 2)
            {
                ContreperiePartial ctpPartial = m_ctpPartials.Where(c => c.CtpId == id).Single();

                Contrepeterie ctp = new Contrepeterie()
                {
                    Content = ctpPartial.Content,
                    Solution = ctpPartial.Solution,
                };

                e = ctp;
            }

            e.Date = date;
            e.Author = m_authors.Where(a => a.Id == authorId).Single();
            e.Title = title;
            e.Id = id;
            e.FavoriteCount = voteCount;

            return e;
        }
    }
}
