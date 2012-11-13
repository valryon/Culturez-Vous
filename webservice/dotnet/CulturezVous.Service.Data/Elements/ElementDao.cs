using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CulturezVous.Service.Data.Db;

namespace CulturezVous.Service.Data.Elements
{
    public class ElementDao : MySqlDao
    {
        public ElementDao(string connectionString)
            : base(connectionString)
        {

        }

        public bool TestDb()
        {
            return ExecuteReader("SELECT * FROM elements", System.Data.CommandType.Text, (reader) =>
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                    }
                }
            });
        }
    }
}
