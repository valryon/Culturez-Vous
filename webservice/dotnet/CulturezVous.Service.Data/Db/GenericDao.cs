using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace CulturezVous.Service.Data.Db
{
    public abstract class GenericDao
    {
        protected string ConnectionString;

        protected GenericDao(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Crée un paramètre pour les requêtes
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract DbParameter CreateParameter(string paramName, object value);

        /// <summary>
        /// Exécuter une requête SQL et parser le résultat
        /// </summary>
        /// <param name="strSql">Requête Sql à exéctuer</param>
        /// <param name="parameterList">Liste de paramètre à associer à la requête</param>
        /// <param name="callback">Résultat à parcourir</param>
        /// <param param name="result">Liste des objets trouvés et mappés</returns>
        /// <returns>Vrai si tout s'est bien passé, faux sinon</rereturns>
        protected abstract bool ExecuteReader(string strSql, CommandType type, Action<DbDataReader> callback, params DbParameter[] parameterList);

        /// <summary>
        /// Exécuter une requête SQL et récupérer le nombre de lignes affectées
        /// </summary>
        /// <param name="strSql">Requête Sql à exéctuer</param>
        /// <param name="parameterList">Liste de paramètre à associer à la requête</param>
        /// <returns>Nombre de lignes affectées</rereturns>
        protected abstract int ExecuteNonQuery(string strSql, CommandType type, params DbParameter[] parameterList);

        /// <summary>
        /// Si une requête a retourné faux, l'exception métier sera stocké dans cette propriété
        /// </summary>
        public Exception LastException { get; protected set; }
    }
}
