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
    /// Interaction logic for Equipment.xaml
    /// </summary>
    public partial class Equipment : UserControl
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        public Equipment()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);
            FetchEquipment();
        }

        private void NewClientButton_Click(object sender, RoutedEventArgs e)
        {
            AddEquipments addEquipments = new AddEquipments();
            addEquipments.equipmentCreated += (s, e) =>
            {

                FetchEquipment();

            };
            addEquipments.ShowDialog();
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void editBtn_Click_1(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var selectedEquipment = btn.DataContext as EquipmentModel;

            if (selectedEquipment != null)
            {
                UpdateEquipment updateEquipment = new UpdateEquipment
                {
                    DataContext = selectedEquipment
                };

                updateEquipment.equipmentUpdated += (s, e) => {

                    FetchEquipment();
                };

                updateEquipment.ShowDialog();
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {

            var b = sender as Button;
            if(b == null) return;

            var selectedEquipment = b.DataContext as EquipmentModel;

            if (selectedEquipment != null)
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
                        SqlCommand cmd = new SqlCommand("DELETE FROM Equipments WHERE EquipmentId = @EquipmentId", sqlConnection);
                        cmd.Parameters.AddWithValue("@EquipmentId", selectedEquipment.EquipmentId);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Client deleted successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        sqlConnection.Close();

                        FetchEquipment();
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message);
                    }

                }

            }
        }

        private void FetchEquipment()
        {

            var clientModels = new List<EquipmentModel>();

            string query = @"SELECT EquipmentId,Name,Stat,Quantity,ImageUrl,Created FROM Equipments";
            try
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand(query, sqlConnection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    clientModels.Add(new EquipmentModel
                    {
                        EquipmentId = reader["EquipmentId"].ToString(),
                        Name = reader["Name"].ToString(),
                        IsAvailable = Convert.ToBoolean(reader["Stat"]),
                        Quantity = reader["Quantity"].ToString(),
                        ImageUrl = reader["ImageUrl"].ToString(),
                       Created = (DateTime) reader["Created"],

                    });

                }

                EquipmentDatagrid.ItemsSource = clientModels;
                reader.Close();
                sqlConnection.Close();

            }
            catch
            {
                MessageBox.Show($"Error while retrieving", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
}
    }
}
