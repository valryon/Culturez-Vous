using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CulturezVous.Data
{
    /// <summary>
    /// A definition for a word
    /// </summary>
    [Table]
    public class Definition
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        protected int DefId;

        [Column]
        public int LinkedWordId { get; set; }

        private EntityRef<Word> word;
        [Association(OtherKey = "Id", ThisKey = "LinkedWordId", Storage = "word")]
        public Word Word
        {
            get
            {
                return word.Entity;
            }
            set
            {
                word.Entity = value;
                LinkedWordId = value.Id;
            }
        }

        [Column]
        public string Details { get; set; }
        [Column]
        public string Content { get; set; }


    }

    /// <summary>
    /// Words
    /// </summary>
    public class Word : Element
    {
        private List<Definition> m_definitions;

        public List<Definition> Definitions
        {
            get
            {
                return m_definitions;
            }
            set
            {
                m_definitions = value;
                foreach (var def in m_definitions)
                {
                    def.Word = this;
                }
            }
        }

        public Word()
            : base()
        {
            m_definitions = new List<Definition>();
        }
    }
}
