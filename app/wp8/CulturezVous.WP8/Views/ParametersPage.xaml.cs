using CulturezVous.WP8.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;

namespace CulturezVous.WP8.Views
{
    public partial class ParametersPage : PhoneApplicationPage
    {
        private ParametersPageViewModel m_viewModel;

        public ParametersPage()
        {
            InitializeComponent();

            m_viewModel = new ParametersPageViewModel();
            DataContext = m_viewModel;
        }

        private void authorizeCb_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            m_viewModel.IsBackgroundAgentAuthorized = true;
        }

        private void authorizeCb_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            m_viewModel.IsBackgroundAgentAuthorized = false;
        }

        private void addBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            m_viewModel.UpdateLiveTile();
        }
    }
}