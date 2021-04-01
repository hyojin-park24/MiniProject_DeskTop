using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls.Dialogs;

namespace WpfSMSApp.View.Store
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddStore : Page
    {
        public AddStore()
        {
            InitializeComponent();
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LblUserIdentityNumber.Visibility = LblUserSurName.Visibility =
                    Visibility.Hidden;

                TxtUserID.Text = TxtUserIdentityNumber.Text = "";
            }
            catch (Exception ex)
            {

                Commons.LOGGER.Error($"예외발생 EditAccount Loaded : {ex}");
                throw ex;
            }
        }

        private void BtnEidtMyAccount_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

