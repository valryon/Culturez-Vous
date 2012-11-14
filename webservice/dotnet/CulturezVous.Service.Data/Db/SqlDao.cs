using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace CulturezVous.Service.Data.Db
{
    public abstract class SqlDao : GenericDao
    {
        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        /// <param name="connectionString">Chaine de connexion à utiliser</param>
        protected SqlDao(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Crée un paramètre pour les requêtes
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override DbParameter CreateParameter(string paramName, object value)
        {
            return new SqlParameter(paramName, value);
        }

        /// <summary>
        /// Exécuter une requête SQL et parser le résultat
        /// </summary>
        /// <param name="strSql">Requête Sql à exéctuer</param>
        /// <param name="parameterList">Liste de paramètre à associer à la requête</param>
        /// <param name="callback">Résultat à parcourir</param>
        /// <param param name="result">Liste des objets trouvés et mappés</returns>
        /// <returns>Vrai si tout s'est bien passé, faux sinon</rereturns>
        protected override bool ExecuteReader(string strSql, CommandType type, Action<DbDataReader> callback, params DbParameter[] parameterList)
        {
            SqlConnection currentConnection = null;

            try
            {
                // Initialisation de la connexion
                using (currentConnection = new SqlConnection(this.ConnectionString))
                {
                    currentConnection.Open();

                    // Initialisation de la commande
                    using (SqlCommand cmd = new SqlCommand(strSql, currentConnection))
                    {
                        cmd.CommandType = type;
                        cmd.CommandTimeout = 120;

                        if (parameterList != null)
                        {
                            foreach (var item in parameterList)
                            {
                                cmd.Parameters.Add(item);
                            }
                        }

                        // Exécution de la commande
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            // récupération des données sous forme d'un datatable
                            if (callback != null)
                            {
                                callback(dr);
                            }

                            dr.Close();
                        }

                        return true;
                    }
                }
            }
            catch (System.Exception e)
            {
                LastException = e;
            }
            finally
            {
                if (currentConnection != null) currentConnection.Close();
            }

            return false;
        }

        /// <summary>
        /// Exécuter une requête SQL et récupérer le nombre de lignes affectées
        /// </summary>
        /// <param name="strSql">Requête Sql à exéctuer</param>
        /// <param name="parameterList">Liste de paramètre à associer à la requête</param>
        /// <returns>Nombre de lignes affectées</rereturns>
        protected override int ExecuteNonQuery(string strSql, CommandType type, params DbParameter[] parameterList)
        {
            SqlConnection currentConnection = null;
            try
            {
                // Initialisation de la connexion
                using (currentConnection = new SqlConnection(this.ConnectionString))
                {
                    currentConnection.Open();

                    // Initialisation de la commande
                    using (SqlCommand cmd = new SqlCommand(strSql, currentConnection))
                    {
                        cmd.CommandType = type;
                        if (parameterList != null)
                        {
                            foreach (var item in parameterList)
                            {
                                cmd.Parameters.Add(item);
                            }
                        }

                        // Exécution de la commande
                        int rows = cmd.ExecuteNonQuery();

                        return rows;
                    }
                }

            }
            catch (System.Exception e)
            {
                LastException = e;
            }
            finally
            {
                if (currentConnection != null) currentConnection.Close();
            }

            return -1;
        }

        protected override object ExecuteScalar(string strSql, CommandType type, params DbParameter[] parameterList)
        {
            SqlConnection currentConnection = null;
            try
            {
                // Initialisation de la connexion
                using (currentConnection = new SqlConnection(this.ConnectionString))
                {
                    currentConnection.Open();

                    // Initialisation de la commande
                    using (SqlCommand cmd = new SqlCommand(strSql, currentConnection))
                    {
                        cmd.CommandType = type;
                        if (parameterList != null)
                        {
                            foreach (var item in parameterList)
                            {
                                cmd.Parameters.Add(item);
                            }
                        }

                        // Exécution de la commande
                        object o = cmd.ExecuteScalar();

                        return o;
                    }
                }

            }
            catch (System.Exception e)
            {
                LastException = e;
            }
            finally
            {
                if (currentConnection != null) currentConnection.Close();
            }

            return null;
        }
    }
}
