using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CulturezVous.Data;
using CulturezVous.WP7.Utils;
using Microsoft.Xna.Framework.GamerServices;

namespace CulturezVous.WP7.ViewModels
{
    /// <summary>
    /// Generic view model for elements
    /// </summary>
    public class ElementViewModel : ViewModelBase
    {
        private Element m_model;

        public ElementViewModel(Element model)
        {
            m_model = model;
        }

        public int Id
        {
            get
            {
                return m_model.Id;
            }
        }
        public string Title
        {
            get
            {
                return m_model.Title;
            }

        }
        public string DateLabel
        {
            get
            {
                if ((m_model.Date.DayOfYear == DateTime.Now.DayOfYear) && (m_model.Date.Year == DateTime.Now.Year))
                {
                    return "Aujourd'hui";
                }
                else if ((m_model.Date.DayOfYear == DateTime.Now.DayOfYear - 1) && (m_model.Date.Year == DateTime.Now.Year))
                {
                    return "Hier";
                }
                else
                {
                    return m_model.Date.ToString("dd MMM yyyy").ToLower();
                }
            }
        }
        public DateTime Date
        {
            get
            {
                return m_model.Date;
            }
        }
        public string VoteCountLabel
        {
            get
            {
                return string.Format("({0})", m_model.VoteCount);
            }
        }
        public int VoteCount
        {
            get
            {
                return m_model.VoteCount;
            }
        }
        public bool IsFavorite
        {
            get
            {
                return m_model.IsFavorite;
            }
            set
            {
                if (m_model.IsFavorite != value)
                {
                    m_model.IsFavorite = value;
                    RaisePropertyChanged("IsFavorite");
                }
            }
        }

        public bool IsRead
        {
            get
            {
                return m_model.IsRead;
            }
            set
            {
                if (m_model.IsRead != value)
                {
                    m_model.IsRead = value;

                    // Save it
                    CacheManager.Instance.SaveToDb();

                    RaisePropertyChanged("IsRead");
                    RaisePropertyChanged("ReadColor");
                }
            }
        }

        public Brush ReadColor
        {
            get
            {
                if (IsRead)
                {
                    return new SolidColorBrush(Color.FromArgb(255, 160, 160, 160));
                }
                else
                {
                    return ((Brush)App.Current.Resources["PhoneAccentBrush"]);
                }
            }
        }

        public Visibility MoreInfoVisibility
        {
            get
            {
                return string.IsNullOrEmpty(m_model.AuthorInfo) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public string AuteurCaption
        {
            get
            {
                string e = (IsContrepeterie ? "e" : "");
                return "Proposé" + e + " par : " + m_model.Author;
            }
        }

        /// <summary>
        /// Pop-up displayed to open author information
        /// </summary>
        public ICommand MoreInfoCommand
        {
            get
            {
                return new Command(() =>
                {
                    string type = "";
                    if (IsWord) type = "cette expression";
                    else if (IsContrepeterie) type = "cette contrepèterie";

                    string caption = type + " a été proposée par " + m_model.Author;

                    Guide.BeginShowMessageBox("Informations sur l'auteur",
                                                caption + ".\nVoulez-vous en savoir plus sur cet auteur ?\n" + m_model.AuthorInfo,
                                                new List<string> { "Oui", "Retour" },
                                                0,
                                                MessageBoxIcon.None,
                                                new AsyncCallback(OnMessageBoxClosed)
                                                , null);
                });
            }
        }

        private void OnMessageBoxClosed(IAsyncResult ar)
        {
            int? buttonIndex = Guide.EndShowMessageBox(ar);
            switch (buttonIndex)
            {
                case 0:
                    try
                    {
                        var task = new Microsoft.Phone.Tasks.WebBrowserTask()
                        {
                            Uri = new Uri(m_model.AuthorInfo)
                        };

                        task.Show();
                    }
                    catch (Exception) { }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Status to share on Facebook, Twitter, etc
        /// </summary>
        public string ShareStatus
        {
            get
            {
                string message = null;

                if (IsWord)
                {
                    message = "Connaissez-vous \"" + m_model.Title + "\" ? Via http://www.valryon.fr/culturez-vous/";
                }
                else if (IsContrepeterie)
                {
                    message = "Petite contrepèterie : \"" + ((Contrepeterie)m_model).Content + "\". Via http://www.valryon.fr/culturez-vous/";
                }

                return message;
            }
        }

        public bool IsWord
        {
            get
            {
                return m_model is Word;
            }
        }

        public bool IsContrepeterie
        {
            get
            {
                return m_model is Contrepeterie;
            }
        }
    }
}
