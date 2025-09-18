using GYM_CLIENT.DatabaseConnection;
using GYM_CLIENT.Model;
using GYM_CLIENT.Services;
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
    /// Interaction logic for Client.xaml
    /// </summary>
    public partial class Client : UserControl
    {
        private ClientService service = new ClientService();
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;

        public Client()
        {
            InitializeComponent();

            service = new ClientService();
            sqlConnection = new SqlConnection(connection.ConnectionString);
            FetchAllClient();
        }

        private void NewClientButton_Click(object sender, RoutedEventArgs e)
        {

            AddClient client = new AddClient();

            client.clientCreated += (s,e) =>{
                FetchAllClient();
            };
            client.ShowDialog();
         
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var selectedClient = btn.DataContext as ClientModel;

            if (selectedClient != null)
            {
                UpdateClient updateClient = new UpdateClient
                {
                    DataContext = selectedClient
                };

                updateClient.clientUpdated += (s, e) => {

                    FetchAllClient();
                };

                updateClient.ShowDialog();
            }

        }

        private void FetchAllClient()
        {
                    var clientModels = new List<ClientModel>();

                    string query = @"
                                SELECT 
                                    c.ClientId,
                                    c.FullName,
                                    c.Contact,
                                    c.TrainerId,
                                    c.hasAccessCard,
                                    t.Name AS TrainerName,
                                    c.PlanId,
                                    p.PlanName
                                FROM Client c
                                LEFT JOIN Trainee t ON c.TrainerId = t.TraineerId
                                LEFT JOIN [Plan] p ON c.PlanId = p.Id";
            try
            {
                sqlConnection.Open();

                        SqlCommand cmd = new SqlCommand(query, sqlConnection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            clientModels.Add(new ClientModel
                            {
                                ClientId = reader["ClientId"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Contact = reader["Contact"].ToString(),
                                TraineeId = reader["TrainerId"].ToString(),
                                hasAccessCard = reader["hasAccessCard"] != DBNull.Value && Convert.ToBoolean(reader["hasAccessCard"]),
                                TraineeName = reader["TrainerName"].ToString(),
                                PlanId = reader["PlanId"].ToString(),
                                PlanName = reader["PlanName"].ToString()

                            });

                        }

                        Clients.ItemsSource = clientModels;
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
            var btn = sender as Button;
            if (btn == null) return;

            var selectedClient = btn.DataContext as ClientModel;

            if (selectedClient != null)
            {

                MessageBoxResult result = MessageBox.Show(
                  "Are you sure you want to delete this product?",
                  "Confirmation",
                  MessageBoxButton.YesNo,
                  MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        sqlConnection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Client WHERE ClientId = @ClientId", sqlConnection);
                        cmd.Parameters.AddWithValue("@ClientId", selectedClient.ClientId);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Client deleted successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        sqlConnection.Close();

                        FetchAllClient();
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message);
                    }

                }

            }
        }

        private void GenerateQRbtn_Click(object sender, RoutedEventArgs e)
        {
          
            var btn = sender as Button;
            if (btn == null) return;

            var selectedClient = btn.DataContext as ClientModel;

            if (selectedClient != null)
            {
                AccessCardView updateClient = new AccessCardView
                {
                    DataContext = selectedClient
                };

                //updateClient.clientUpdated += (s, e) => {

                //    FetchAllClient();
                //};

                updateClient.ShowDialog();
            }
        }
    }
}
