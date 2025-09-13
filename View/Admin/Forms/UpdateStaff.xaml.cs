using GYM_CLIENT.DatabaseConnection;
using GYM_CLIENT.Model;
using Microsoft.Data.SqlClient;
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

namespace GYM_CLIENT.View.Admin.Forms
{
    /// <summary>
    /// Interaction logic for UpdateStaff.xaml
    /// </summary>
    public partial class UpdateStaff : Window
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        public UpdateStaff()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);

        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private string? StaffId = "";

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            var staff = DataContext as StaffModel;

            if (staff != null)
            {
                StaffId = staff.StaffId;
                Name.Text = staff.Name;
                Contact.Text = staff.Contact;
                Email.Text = staff.Gmail;
                Email.Text = staff.Gmail;
                Username.Text = staff.Username;
                Password.Text = staff.Password;
                Role.SelectedValue = staff.Role;


            }
        }
    }
}
