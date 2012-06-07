using System;
using System.Collections.Generic;
using System.Data.Linq;

namespace CulturezVous.Data.Cache
{
    /// <summary>
    /// Data stored in the cache and in the local database. It should be similar to what you can find on the server.
    /// </summary>
    public class CacheDataContext : DataContext
    {
        // We must declare all tables
        // --
        public Table<Definition> Definitions
        {
            get
            {
                return this.GetTable<Definition>();
            }
        }

        public Table<Element> Elements
        {
            get
            {
                return this.GetTable<Element>();
            }
        }

        public DateTime LastCacheUpdate { get; set; }

        public CacheDataContext(string connectionString)
            : base(connectionString)
        {
            LastCacheUpdate = DateTime.MinValue;
        }
    }
}
