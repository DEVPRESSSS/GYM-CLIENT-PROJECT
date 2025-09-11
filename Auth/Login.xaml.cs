using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GYM_CLIENT.Auth
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Passwordtxt_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

        private void PasswordUnmask_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Usernametxt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Eye_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void forgotpassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ForgotPassword pass = new ForgotPassword();
            pass.Show();
            this.Close();
        }

        private void Eye2_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
