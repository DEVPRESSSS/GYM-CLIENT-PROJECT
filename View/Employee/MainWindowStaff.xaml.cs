using GYM_CLIENT.Auth;
using GYM_CLIENT.View.Admin;
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

namespace GYM_CLIENT.View.Employee
{
    /// <summary>
    /// Interaction logic for MainWindowStaff.xaml
    /// </summary>
    public partial class MainWindowStaff : Window
    {
        private string _user;
        public MainWindowStaff(string user)
        {
            InitializeComponent();
            _user = user;

            MainContentArea.Content = new Attendance();

        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {

                WindowState = WindowState.Minimized;
            }
            else
            {

                WindowState = WindowState.Normal;

            }
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;

            }
            else
            {

                WindowState = WindowState.Normal;

            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Are you sure you want to logout?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (dialogResult == MessageBoxResult.Yes)
            {
                var logout = new Login();
                logout.Show();
                this.Close();

            }
        }

        private void Clients_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new Client();

        }

        private void Payments_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new Payments();

        }

        private void Equipments_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new Equipment();

        }

        private void Attendance_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new Attendance();

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
