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
        public List<Element> GetAllElements(bool onlyValid)
        {
            List<Element> element = new List<Element>();

            bool exec = ExecuteReader("SELECT * FROM elements " + (onlyValid ? "WHERE author_id != 0" : "") + " ORDER BY  `element_id` DESC", System.Data.CommandType.Text, (reader) =>
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

            return element.OrderByDescending(e => e.Date).ToList();
        }

        public Element GetElementById(int id)
        {
            Element e = null;

            bool exec = ExecuteReader("SELECT * FROM elements WHERE element_id = @id", System.Data.CommandType.Text, (reader) =>
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        e = parseElement(reader);
                        break;
                    }
                }
            }, new MySqlParameter("@id", id));

            return e;
        }

        /// <summary>
        /// Suppression d'un élément
        /// </summary>
        /// <param name="p"></param>
        public bool Delete(int id)
        {
            new DefinitionDao(ConnectionString).DeleteByElementId(id);
            new ContrepeteriePartialDao(ConnectionString).DeleteByElementId(id);

            int exec = ExecuteNonQuery("DELETE FROM elements WHERE element_id = @id", System.Data.CommandType.Text, new MySqlParameter("@id", id));

            return exec > 0;
        }

        public bool Create(Element e)
        {
            int type = 0;
            if (e is Word) type = 1;
            else if (e is Contrepeterie) type = 2;
            string sql = "INSERT INTO elements(type_id, element_date, element_title, element_favoriteCount, author_id) VALUES (@type,@date,@title,@votes,@author);"
                            + "select last_insert_id();";

            object execUpdate = ExecuteScalar(sql, System.Data.CommandType.Text
                , new MySqlParameter("@type", type)
                , new MySqlParameter("@date", e.Date)
                , new MySqlParameter("@title", e.Title)
                , new MySqlParameter("@votes", e.FavoriteCount)
                , new MySqlParameter("@author", e.Author.Id)
                );

            // Récupérer l'id
            if (execUpdate != null)
            {
                e.Id = Convert.ToInt32(execUpdate);

                if (e is Word)
                {
                    Word w = ((Word)e);

                    foreach (Definition d in w.Definitions)
                    {
                        d.WordId = w.Id;
                    }

                    DefinitionDao dao = new DefinitionDao(ConnectionString);

                    if (dao.Create(w) == false)
                    {
                        throw dao.LastException;
                    }
                }
                else
                {
                    ContrepeteriePartialDao dao = new ContrepeteriePartialDao(ConnectionString);

                    if (dao.Create((Contrepeterie)e) == false)
                    {
                        throw dao.LastException;
                    }
                }
            }

            return true;
        }

        public void Update(Element e)
        {
            int type = 0;
            if (e is Word) type = 1;
            else if (e is Contrepeterie) type = 2;

            DefinitionDao defdao = new DefinitionDao(ConnectionString);
            ContrepeteriePartialDao ctpdao = new ContrepeteriePartialDao(ConnectionString);

            if (e != null && e.Id != 0)
            {
                // Suppression de l'ancien contenu
                if (e is Word)
                {
                    defdao.DeleteByElementId(e.Id);
                }
                else if (e is Contrepeterie)
                {
                   ctpdao.DeleteByElementId(e.Id);
                }

                string sql = "UPDATE elements SET type_id=@type,element_date=@date,element_title=@title,element_favoriteCount=@votes,author_id=@author WHERE element_id = @id";
                int execUpdate = ExecuteNonQuery(sql, System.Data.CommandType.Text
                    , new MySqlParameter("@id", e.Id)
                    , new MySqlParameter("@type", type)
                    , new MySqlParameter("@date", e.Date)
                    , new MySqlParameter("@title", e.Title)
                    , new MySqlParameter("@votes", e.FavoriteCount)
                    , new MySqlParameter("@author", e.Author.Id)
                    );

                // Insertion contenu
                if (e is Word)
                {
                    defdao.Create(e as Word);
                }
                else if (e is Contrepeterie)
                {
                    ctpdao.Create(e as Contrepeterie);
                }
            }
        }

        private Element parseElement(DbDataReader reader)
        {
            Element e = null;

            //	element_id	type_id	element_date	element_title	element_favoriteCount	author_id
            int id = Convert.ToInt32(reader["element_id"]);
            int type = Convert.ToInt32(reader["type_id"]);
            string dateString = reader["element_date"].ToString();
            DateTime date = Convert.ToDateTime(dateString);
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

            var aut = m_authors.Where(a => a.Id == authorId).FirstOrDefault();
            if (aut == null)
            {
                throw new ArgumentException("No author found : " + authorId);
            }
            e.Author = aut;
            e.Title = title;
            e.Id = id;
            e.FavoriteCount = voteCount;

            return e;
        }
    }
}
