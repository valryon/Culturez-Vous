using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CulturezVous.Data;

namespace CulturezVous.WP8.ViewModels
{
    /// <summary>
    /// View model for words elements
    /// </summary>
    public class WordViewModel : ElementViewModel
    {
        private Word m_model;

        public WordViewModel(Word word)
            : base(word)
        {
            m_model = word;

            Definitions = m_model.Definitions.Select(d => new DefinitionViewModel(d)).ToList();
        }

        public List<DefinitionViewModel> Definitions
        {
            get;
            private set;
        }

        public string DefinitionsCaption
        {
            get
            {
                return Definitions.Count + " défintion" + (Definitions.Count > 1 ? "s" : "");
            }
        }


        public Word Word
        {
            get
            {
                return m_model;
            }
        }
    }
}
