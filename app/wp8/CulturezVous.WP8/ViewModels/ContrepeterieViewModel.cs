using System.Windows;
using CulturezVous.Data;
using CulturezVous.WP8.Utils;
using System.Windows.Media.Animation;
using CulturezVous.WP8.Views;
using System;
using System.Windows.Input;
using CulturezVous.WP8.Services;
using System.Collections.Generic;

namespace CulturezVous.WP8.ViewModels
{
    public class ContrepeterieViewModel : ElementViewModel
    {
        private Contrepeterie m_model;
        private bool m_isSolutionRevealed;

        public ContrepeterieViewModel(Contrepeterie model)
            : base(model)
        {
            m_model = model;
            m_isSolutionRevealed = false;
        }

        public Storyboard ShowSolutionAnimation { get; set; }

        public ICommand ShowSolutionCommand
        {
            get
            {
                return new Command(
                    () =>
                    {
                        ExecuteUI(delegate
                        {
                            IsSolutionRevealed = true;

                            if (ShowSolutionAnimation != null)
                            {
                                ShowSolutionAnimation.Begin();
                            }
                        });
                    }
                );
            }
        }

        public bool IsSolutionRevealed
        {
            get
            {
                return m_isSolutionRevealed;
            }
            set
            {
                m_isSolutionRevealed = value;
                RaisePropertyChanged("IsSolutionRevealed");
                RaisePropertyChanged("Content");
                RaisePropertyChanged("SolutionVisibility");
                RaisePropertyChanged("ButtonVisibility");
            }
        }

        public Uri IconUri
        {
            get
            {
                return UnlockIconUri;
            }
        }

        public string ButtonCaption
        {
            get
            {
                return "Voir les indices";
            }
        }

        public Visibility ButtonVisibility
        {
            get
            {
                return (IsSolutionRevealed ? Visibility.Collapsed : Visibility.Visible);
            }
        }

        public Visibility SolutionVisibility
        {
            get
            {
                return (!IsSolutionRevealed ? Visibility.Collapsed : Visibility.Visible);
            }
        }

        public string Content
        {
            get
            {
                string content = @"<Run Text=""" + m_model.Content + @""" />";

                if (IsSolutionRevealed)
                {
                    var result = TextDiff.Diff(m_model.Content, m_model.Solution);
                    content = TextDiff.Format(result);
                }

                return generateRichTextBox(content); ;
            }
        }


        private string generateRichTextBox(string content)
        {
            string xaml = @"<RichTextBox xmlns=""http://schemas.microsoft.com/client/2007"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" "
                          +
                          @" IsReadOnly=""True""  TextWrapping=""Wrap"" FontSize=""{StaticResource PhoneFontSizeLarge}"" Background=""{x:Null}""><Paragraph>" + content + "</Paragraph></RichTextBox>";
            return xaml;
        }

        public Contrepeterie Contrepetrie
        {
            get
            {
                return m_model;
            }
        }
    }
}
