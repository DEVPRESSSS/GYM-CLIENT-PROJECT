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
    /// Interaction logic for Attendance.xaml
    /// </summary>
    public partial class Attendance : UserControl
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;

        public event EventHandler clientCreated;
        public Attendance()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);

        }

        private void CameraBtn_Click(object sender, RoutedEventArgs e)
        {
            CameraWindow cameraWindow = new CameraWindow();
            cameraWindow.ShowDialog();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FetchAttendance();
        }

        private void FetchAttendance()
        {

            var clientModels = new List<AttendanceModel>();

            string query = @"SELECT a.ClientId,c.FullName,a.CheckInTime FROM Attendance a
                            INNER JOIN [Client] c ON c.ClientId = a.ClientId ";
            try
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand(query, sqlConnection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    clientModels.Add(new AttendanceModel
                    {
                        ClientId = reader["ClientId"].ToString(),
                        FullName = reader["FullName"].ToString(),
                        CheckInTime = reader["CheckInTime"] == DBNull.Value? (DateTime?)null:(DateTime)reader["CheckInTime"]

                });

                }

                ClientAttendance.ItemsSource = clientModels;
                reader.Close();
                sqlConnection.Close();

            }
            catch(Exception ex) 
            {
                MessageBox.Show($"Error:{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }



        }
    }
}
