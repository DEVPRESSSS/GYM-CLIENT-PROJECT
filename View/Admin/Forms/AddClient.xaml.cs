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

namespace GYM_CLIENT.Model
{
    /// <summary>
    /// Interaction logic for AddClient.xaml
    /// </summary>
    public partial class AddClient : Window
    {

      
        private readonly ClientService _service;
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;

        public event EventHandler clientCreated;

        public AddClient()
        {
            InitializeComponent();
            _service =  new ClientService();
            sqlConnection = new SqlConnection(connection.ConnectionString);
            MembershipPlan();
            GetAllTrainee();

        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateClient()
        {
            string query = @"INSERT INTO Client (ClientId,FullName,Contact,TrainerId,PlanId,PlanStarted,PlanEnded)
            VALUES(@ClientId,@FullName,@Contact,@TrainerId,@PlanId,@PlanStarted,@PlanEnded)";

            try
            {
                sqlConnection.Open();
                string? selectedPlanId = Membership?.SelectedValue?.ToString();
                string? selectedTrainee = Trainee?.SelectedValue?.ToString();

                if(selectedPlanId == null)
                {

                    MessageBox.Show("Membership is required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    sqlConnection.Close();

                    return;
                }

                var duration = _service.FetchDaysOfPlan(selectedPlanId);
                if (duration == 0)
                {
                    return;
                }

                

                if(Contact.Text.Length < 11)
                {

                    MessageBox.Show($"Contact must be 11 numbers", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    sqlConnection.Close();

                    return;
                }



                if (string.IsNullOrWhiteSpace(Name.Text) || (string.IsNullOrWhiteSpace(Contact.Text)))
                {
                    MessageBox.Show($"All fields are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var planStarted = DateTime.Today;
                var planEnded = DateTime.Today.AddDays(duration); 

                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {

                    cmd.Parameters.AddWithValue("@ClientId", $"CLIENT-{Guid.NewGuid().ToString().ToUpper().Substring(0, 10)}");
                    cmd.Parameters.AddWithValue("@FullName", Name.Text);
                    cmd.Parameters.AddWithValue("@Contact", Contact.Text);
                    cmd.Parameters.AddWithValue("@TrainerId", string.IsNullOrEmpty(selectedTrainee) ? (object)DBNull.Value : selectedTrainee);
                    cmd.Parameters.AddWithValue("@PlanId", string.IsNullOrEmpty(selectedPlanId) ? (object)DBNull.Value : selectedPlanId);
                    cmd.Parameters.AddWithValue("@PlanStarted", planStarted);
                    cmd.Parameters.AddWithValue("@PlanEnded",planEnded);
                   

                    int row = cmd.ExecuteNonQuery();

                    if (row > 0)
                    {
                        MessageBox.Show("Client inserted successfully");
                        Clear(); 

                        clientCreated?.Invoke(this, new EventArgs());
                        sqlConnection.Close();

                    }



                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error:{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                sqlConnection.Close();

            }
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
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateClient();
        }

        private void Contact_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HelperValidation.ValidationHelper.PersonNameTextComposition(sender, e);
        }

        private void Name_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HelperValidation.ValidationHelper.PersonNameTextKeyDown(sender, e);

        }

        private void Contact_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HelperValidation.ValidationHelper.AllowOnlyNumbers(sender, e);

        }

       
    }
}
