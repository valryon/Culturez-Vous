using CulturezVous.Data;
using CulturezVous.WP8.Utils;
using System;

namespace CulturezVous.WP8.ViewModels
{
    /// <summary>
    /// Details page
    /// </summary>
    public class DetailsPageViewModel : ViewModelBase
    {
        private ElementViewModel m_viewModel;

        /// <summary>
        /// Designer
        /// </summary>
        public DetailsPageViewModel()
        {
            // Test CTP or Words
            //m_viewModel = new WordViewModel(new Word()
            //{
            //    Author = "Auteur ici",
            //    AuthorInfo = "www.culturez-vous.fr",
            //    Date = DateTime.Now,
            //    Definitions = new System.Collections.Generic.List<Definition>()
            //    {
            //        new Definition() {
            //            Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque gravida, eros ut consequat tempus",
            //            Details ="nom, verbe, truc"   
            //        },
            //        new Definition() {
            //            Content = "mi mi ornare nisl, eget convallis quam lorem in eros nullam.",
            //            Details ="proverbe, chinois"
            //        }
            //    },
            //    IsFavorite = false,
            //    IsRead = false,
            //    Title = "Element details designer"
            //});

            m_viewModel = new ContrepeterieViewModel(new Contrepeterie()
            {
                Author = "Auteur ici",
                AuthorInfo = "www.culturez-vous.fr",
                Date = DateTime.Now,
                Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque gravida, eros ut consequat tempus",
                Solution = "Lorem doum ipsolor sit amet, consectetur adipiscing elit. Quisque gravida, eros ut consequat tempus",
                IsFavorite = false,
                IsRead = false,
                Title = "Element details designer"
            });
        }

        public DetailsPageViewModel(ElementViewModel viewModel)
            : base()
        {
            m_viewModel = viewModel;
        }

        public ElementViewModel ElementViewModel
        {
            get { return m_viewModel; }
        }

        public bool IsFavorite
        {
            get { return m_viewModel.IsFavorite; }
        }

        public string DateLabel
        {
            get { return m_viewModel.DateLabel.ToUpper(); }
        }

        public string Title
        {
            get { return m_viewModel.Title; }
        }

        public bool IsRead
        {
            get
            {
                return m_viewModel.IsRead;
            }
            set
            {
                m_viewModel.IsRead = value;
                RaisePropertyChanged("IsRead");
            }
        }
    }
}
