using GYM_CLIENT.DatabaseConnection;
using GYM_CLIENT.Services;
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
    /// Interaction logic for AddStaff.xaml
    /// </summary>
    public partial class AddStaff : Window
    {

        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        public event EventHandler staffCreated;
        private readonly RoleService _service;

        public AddStaff()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);
            _service = new RoleService();
            FetchRole();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateStaff();  
        }

        private void CreateStaff()
        {

            string query = "INSERT INTO Staff (StaffId,Name,Contact,Email,Role,Username,Password) VALUES(@StaffId,@Name,@Contact,@Email,@Role,@Username,@Password)";


            try
            {
                sqlConnection.Open();
                string? selectedPlanId = Role?.SelectedValue?.ToString();

                if (string.IsNullOrWhiteSpace(Name.Text) || (string.IsNullOrWhiteSpace(Contact.Text)) || (string.IsNullOrWhiteSpace(Username.Text))
                    || (string.IsNullOrWhiteSpace(Password.Text)) || (string.IsNullOrWhiteSpace(Email.Text)))
                {
                    MessageBox.Show($"All fields are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {

                    cmd.Parameters.AddWithValue("@StaffId", $"STAFF-{Guid.NewGuid().ToString().ToUpper().Substring(0, 10)}");
                    cmd.Parameters.AddWithValue("@Name", Name.Text);
                    cmd.Parameters.AddWithValue("@Contact", Contact.Text);
                    cmd.Parameters.AddWithValue("@Email", Email.Text);
                    cmd.Parameters.AddWithValue("@Username", Username.Text);
                    cmd.Parameters.AddWithValue("@Password", Password.Text);
                    cmd.Parameters.AddWithValue("@Role", selectedPlanId);


                    int row = cmd.ExecuteNonQuery();

                    if (row > 0)
                    {
                        MessageBox.Show("Client inserted successfully");
                        staffCreated?.Invoke(this, new EventArgs());
                    }



                }

              }
            catch
            {
                MessageBox.Show($"Staff Name is already exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                sqlConnection.Close();

            }


        }

        private void FetchRole()
        {

            var roles = _service.GetRoles();
            Role.ItemsSource = roles;
            Role.DisplayMemberPath = "RoleName";
            Role.SelectedValuePath = "RoleId";


        }
    }
}
