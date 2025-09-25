using GYM_CLIENT.DatabaseConnection;
using GYM_CLIENT.Model;
using GYM_CLIENT.Template;
using GYM_CLIENT.View.Admin.Forms;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for Payments.xaml
    /// </summary>
    public partial class Payments : UserControl
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        private CollectionViewSource collectionViewSource;

        public Payments()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);
            collectionViewSource = new CollectionViewSource();

            FetchAllPayments();

        }

        private void NewClientButton_Click(object sender, RoutedEventArgs e)
        {
            var paymentForm = new PaymentForm();

            paymentForm.paymentCreated += (s, e) =>
            {

                FetchAllPayments();
            };
            paymentForm.ShowDialog();
        }

        private void FetchAllPayments()
        {

            List<PaymentModel> payments = new List<PaymentModel>();

            string query = @"
                SELECT 
                    p.PaymentId,
                    p.ClientId,
                    ISNULL(c.FullName, p.NonMemberClient) AS ClientName,
                    p.PaymentType,
                    p.PlanName,
                    p.AmountPaid,
                    p.Changed,
                    p.PaymentDate
                FROM Payments p
                LEFT JOIN Client c ON p.ClientId = c.ClientId
                ORDER BY p.PaymentDate DESC;";

            using (SqlConnection sqlConnection = new SqlConnection(connection.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
            {
                sqlConnection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        payments.Add(new PaymentModel
                        {
                            PaymentId = reader["PaymentId"].ToString(),
                            ClientId = reader["ClientId"].ToString(),
                            ClientName = reader["ClientName"].ToString(),
                            PaymentType = reader["PaymentType"].ToString(),
                            PlanName = reader["PlanName"].ToString(),
                            AmountPaid = reader["AmountPaid"] == DBNull.Value ? null : (decimal?)reader["AmountPaid"],
                            Changed = reader["Changed"] == DBNull.Value ? null : (decimal?)reader["Changed"],
                            PaymentDate = Convert.ToDateTime(reader["PaymentDate"])
                        });
                    }
                    collectionViewSource.Source = payments;
                    ClientsPayment.ItemsSource = collectionViewSource.View;
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FetchAllPayments();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (collectionViewSource?.View != null)
            {
                collectionViewSource.View.Filter = item =>
                {
                    var payment = item as PaymentModel;
                    if (payment == null) return false;

                    string searchText = Search.Text.Trim().ToLower();
                    if (string.IsNullOrWhiteSpace(searchText)) return true;

                    return
                        (!string.IsNullOrEmpty(payment.PaymentId) && payment.PaymentId.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(payment.ClientId) && payment.ClientId.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(payment.ClientName) && payment.ClientName.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(payment.PaymentType) && payment.PaymentType.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(payment.PlanId) && payment.PlanId.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(payment.PlanName) && payment.PlanName.ToLower().Contains(searchText)) ||
                        (payment.AmountPaid.HasValue && payment.AmountPaid.Value.ToString().Contains(searchText)) ||
                        (payment.Changed.HasValue && payment.Changed.Value.ToString().Contains(searchText)) ||
                        payment.PaymentDate.ToString("yyyy-MM-dd").Contains(searchText); 
                };
            }

        }

        
        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {

            var listOfPayments = ClientsPayment.ItemsSource.Cast<PaymentModel>().ToList();

            PaymentReport paymentReport = new PaymentReport(listOfPayments);
            paymentReport.ShowDialog();
        }
    }
}
