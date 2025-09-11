using System;
using System.Collections.Generic;
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
    /// Interaction logic for ForgotPassword.xaml
    /// </summary>
    public partial class ForgotPassword : Window
    {
        public ForgotPassword()
        {
            InitializeComponent();
        }

        private void PackIconMaterial_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void SendOTP_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
          
        }

        private void Window_MouseDown_1(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
