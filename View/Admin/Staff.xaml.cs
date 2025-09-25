using GYM_CLIENT.DatabaseConnection;
using GYM_CLIENT.Model;
using GYM_CLIENT.View.Admin.Forms;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GYM_CLIENT.View.Admin
{
    /// <summary>
    /// Interaction logic for Staff.xaml
    /// </summary>
    public partial class Staff : UserControl
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        private CollectionViewSource collectionViewSource;
        public Staff()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);
            collectionViewSource = new CollectionViewSource();

            FetchAllStaff();
        }

        private void StaffBtn_Click(object sender, RoutedEventArgs e)
        {
            AddStaff addStaff = new AddStaff();
            addStaff.staffCreated += (s, e) =>
            {

                FetchAllStaff();

            };


            addStaff.ShowDialog();
            
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var selectedStaff = btn.DataContext as StaffModel;

            if (selectedStaff != null)
            {
                UpdateStaff updateStaff = new UpdateStaff
                {
                    DataContext = selectedStaff
                };

                updateStaff.staffUpdated += (s, e) =>
                {

                    FetchAllStaff();
                };

                updateStaff.ShowDialog();
            }
        }

        private void FetchAllStaff()
        {

            var clientModels = new List<StaffModel>();

            string query = @"SELECT c.StaffId,
                               c.Name,
                               c.Contact,
                               c.Email,
                               c.Role,
                               t.RoleName,
                               c.Created,
                               c.Username,
                               c.Password
                        FROM Staff c
                        LEFT JOIN Role t ON c.Role = t.RoleId
                        WHERE Role = 'ROLE-102'";
            try
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand(query, sqlConnection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    clientModels.Add(new StaffModel
                    {
                        StaffId = reader["StaffId"].ToString(),
                        Name = reader["Name"].ToString(),
                        Contact = reader["Contact"].ToString(),
                        Gmail = reader["Email"].ToString(),
                        RoleId = reader["Role"].ToString(),
                        RoleName = reader["RoleName"].ToString(),
                        Username = reader["Username"].ToString(),
                        Password = reader["Password"].ToString(),
                        Created = reader["Created"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["Created"]),

                    });

                }
                collectionViewSource.Source = clientModels;

                StaffGrid.ItemsSource = collectionViewSource.View;
                reader.Close();
                sqlConnection.Close();

            }
            catch
            {
                MessageBox.Show($"Error while retrieving", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var button = sender as Button;

            if(button==null)
            {
                return;
            }
            var selectedStaff = button.DataContext as StaffModel;

            if (selectedStaff != null)
            {
                MessageBoxResult result = MessageBox.Show(
                 "Are you sure you want to delete this staff?",
                 "Confirmation",
                 MessageBoxButton.YesNo,
                 MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        sqlConnection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Staff WHERE StaffId = @StaffId", sqlConnection);
                        cmd.Parameters.AddWithValue("@StaffId", selectedStaff.StaffId);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Staff deleted successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        sqlConnection.Close();

                        FetchAllStaff();
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message);
                    }

                }
            }
        }
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (collectionViewSource?.View != null)
            {
                collectionViewSource.View.Filter = item =>
                {
                    var staff = item as StaffModel;
                    if (staff == null) return false;

                    string searchText = Search.Text.Trim().ToLower();
                    if (string.IsNullOrEmpty(searchText)) return true; 

                    return
                        (!string.IsNullOrEmpty(staff.StaffId) && staff.StaffId.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(staff.Name) && staff.Name.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(staff.Contact) && staff.Contact.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(staff.Gmail) && staff.Gmail.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(staff.RoleId) && staff.RoleId.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(staff.RoleName) && staff.RoleName.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(staff.Username) && staff.Username.ToLower().Contains(searchText)) ||
                        (staff.Created.HasValue && staff.Created.Value.ToString("yyyy-MM-dd").Contains(searchText));
                };
            }
        }


        private void NewClientButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
