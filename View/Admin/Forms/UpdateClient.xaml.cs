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
    /// Interaction logic for UpdateClient.xaml
    /// </summary>
    public partial class UpdateClient : Window
    {
        private readonly ClientService _service;
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;

        public event EventHandler clientUpdated;
        public UpdateClient()
        {
            InitializeComponent();
            _service = new ClientService();
            sqlConnection = new SqlConnection(connection.ConnectionString);
            MembershipPlan();
            GetAllTrainee();
        }
        private void MembershipPlan()
        {


            var plans = _service.GetPlans();
            Membership.ItemsSource = plans;
            Membership.DisplayMemberPath = "PlanName";
            Membership.SelectedValuePath = "PlanId";


        }
        private void GetAllTrainee()
        {


            var plans = _service.GetTrainor();
            Trainee.ItemsSource = plans;
            Trainee.DisplayMemberPath = "Name";
            Trainee.SelectedValuePath = "TraineerId";


        }

        private void Clear()
        {

            Name.Text = "";
            Contact.Text = "";
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateInfoClient()
        {
            string query = @"UPDATE Client 
                     SET FullName = @FullName, 
                         Contact = @Contact, 
                         TrainerId = @TrainerId, 
                         PlanId = @PlanId 
                     WHERE ClientId = @ClientId";

            try
            {
                sqlConnection.Open();

                string? selectedPlanId = Membership?.SelectedValue?.ToString();
                string? selectedTrainee = Trainee?.SelectedValue?.ToString();

                if (string.IsNullOrWhiteSpace(Name?.Text) || string.IsNullOrWhiteSpace(Contact?.Text))
                {
                    MessageBox.Show("Full Name and Contact are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@ClientId", clientId);
                    cmd.Parameters.AddWithValue("@FullName", Name.Text.Trim());
                    cmd.Parameters.AddWithValue("@Contact", Contact.Text.Trim());
                    cmd.Parameters.AddWithValue("@TrainerId", string.IsNullOrEmpty(selectedTrainee) ? (object)DBNull.Value : selectedTrainee);
                    cmd.Parameters.AddWithValue("@PlanId", string.IsNullOrEmpty(selectedPlanId) ? (object)DBNull.Value : selectedPlanId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Client updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        Clear();
                        clientUpdated?.Invoke(this, EventArgs.Empty); 
                    }
                    else
                    {
                        MessageBox.Show("No client record found to update.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"SQL error: {sqlEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        string? clientId = "";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var clients = DataContext as ClientModel;

            if (clients != null)
            {
                clientId = clients.ClientId;
                Name.Text = clients.FullName;
                Contact.Text = clients.Contact;
                Trainee.SelectedValue = clients.TraineeId;
                Membership.SelectedValue = clients.PlanId;


            }

        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateInfoClient();
        }
    }
}
