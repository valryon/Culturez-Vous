using CulturezVous.Data;
using CulturezVous.WP7.Utils;

namespace CulturezVous.WP7.ViewModels
{
    public class DefinitionViewModel : ViewModelBase
    {
        private Definition m_def;

        public DefinitionViewModel(Definition definition) :
            base()
        {
            m_def = definition;
        }

        public string Details
        {
            get
            {
                //return "[" + m_def.Details + "]";
                return m_def.Details;
            }
        }

        public string Content
        {
            get
            {
                return m_def.Content;
            }
        }
    }
}
