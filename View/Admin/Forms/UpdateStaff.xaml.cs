using GYM_CLIENT.DatabaseConnection;
using GYM_CLIENT.Model;
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
    /// Interaction logic for UpdateStaff.xaml
    /// </summary>
    public partial class UpdateStaff : Window
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;

        public event EventHandler staffUpdated;
        private readonly RoleService _service;

        public UpdateStaff()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);
            _service = new RoleService();
            fetchRoles();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateStaffRecord()
        {
            string query = "UPDATE Staff SET Name = @Name, Contact = @Contact, Email = @Email, Role = @Role, Username = @Username, Password = @Password WHERE StaffId = @StaffId";

            try
            {
                sqlConnection.Open();
                string? selectedPlanId = Role?.SelectedValue?.ToString();

                if (string.IsNullOrWhiteSpace(Name.Text) || string.IsNullOrWhiteSpace(Contact.Text) ||
                    string.IsNullOrWhiteSpace(Username.Text) || string.IsNullOrWhiteSpace(Password.Text) ||
                    string.IsNullOrWhiteSpace(Email.Text) )
                {
                    MessageBox.Show("All fields are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@StaffId", StaffId);
                    cmd.Parameters.AddWithValue("@Name", Name.Text.Trim());
                    cmd.Parameters.AddWithValue("@Contact", Contact.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", Email.Text.Trim());
                    cmd.Parameters.AddWithValue("@Username", Username.Text.Trim());
                    cmd.Parameters.AddWithValue("@Password", Password.Text.Trim());
                    cmd.Parameters.AddWithValue("@Role", selectedPlanId ?? string.Empty);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("Staff updated successfully");
                        staffUpdated?.Invoke(this, new EventArgs()); 
                    }
                    else
                    {
                        MessageBox.Show("No staff record was updated. Please check the Staff ID.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating the staff: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlConnection.Close();
            }
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
                Role.SelectedValue = staff.RoleId;


            }
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateStaffRecord();
        }

        private void fetchRoles()
        {

            var roles = _service.GetRoles();
            Role.ItemsSource = roles;
            Role.DisplayMemberPath = "RoleName";
            Role.SelectedValuePath = "RoleId";
        }
    }
}
