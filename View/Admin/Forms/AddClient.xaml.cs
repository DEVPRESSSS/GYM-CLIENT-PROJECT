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
            string query = "INSERT INTO Client (ClientId,FullName,Contact,TrainerId,PlanId)VALUES(@ClientId,@FullName,@Contact,@TrainerId,@PlanId)";

            try
            {
                sqlConnection.Open();
                string? selectedPlanId = Membership?.SelectedValue?.ToString();
                string? selectedTrainee = Trainee?.SelectedValue?.ToString();




                if (string.IsNullOrWhiteSpace(Name.Text) || (string.IsNullOrWhiteSpace(Contact.Text)))
                {
                    MessageBox.Show($"All fields are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {

                    cmd.Parameters.AddWithValue("@ClientId", $"CLIENT-{Guid.NewGuid().ToString().ToUpper().Substring(0, 10)}");
                    cmd.Parameters.AddWithValue("@FullName", Name.Text);
                    cmd.Parameters.AddWithValue("@Contact", Contact.Text);
                    cmd.Parameters.AddWithValue("@TrainerId", string.IsNullOrEmpty(selectedTrainee) ? (object)DBNull.Value : selectedTrainee);
                    cmd.Parameters.AddWithValue("@PlanId", string.IsNullOrEmpty(selectedPlanId) ? (object)DBNull.Value : selectedPlanId);


                    int row = cmd.ExecuteNonQuery();

                    if (row > 0)
                    {
                        MessageBox.Show("Client inserted successfully");
                        Clear(); 
                        clientCreated?.Invoke(this, new EventArgs());
                    }



                }

            }
            catch
            {
                MessageBox.Show($"Name is already exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

 
    }
}
