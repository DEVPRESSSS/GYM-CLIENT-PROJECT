using GYM_CLIENT.Auth;
using GYM_CLIENT.View.Admin;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GYM_CLIENT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainContentArea.Content = new Dashboard();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if(WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;

            }
            else
            {

                WindowState = WindowState.Normal;

            }
        }

        private void Minimize_Click_1(object sender, RoutedEventArgs e)
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

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new Dashboard();

        }

        private void Employee_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new Staff();

        }

        private void Clients_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new Client();

        }

        private void Attendance_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new Attendance();

        }

        private void Payments_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Equipments_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new Equipment();

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Are you sure you want to logout?", "Confirmation", MessageBoxButton.YesNo,MessageBoxImage.Question);

            if(dialogResult == MessageBoxResult.Yes)
            {
                var logout = new Login();
                logout.Show();
                this.Close();

            }


           
        }
    }
}