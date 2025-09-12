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

namespace GYM_CLIENT.Services
{
    public class ClientService
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        public event EventHandler refreshDatagridView;
        public ClientService()
        {
            sqlConnection = new SqlConnection(connection.ConnectionString);

        }

        public List<PlanModel> GetPlans()
        {
            List<PlanModel> plans = new List<PlanModel>();

            string query = "SELECT Id,PlanName FROM [Plan]";
            sqlConnection.Open();
            SqlCommand cmd = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = cmd.ExecuteReader();


            while (reader.Read())
            {

                plans.Add(new PlanModel
                {
                    PlanId = reader["Id"].ToString(),
                    PlanName = reader["PlanName"].ToString(),


                });



            }
            sqlConnection.Close();

            return plans;

        }

        public List<TraineeModel> GetTrainor()
        {
            List<TraineeModel> plans = new List<TraineeModel>();

            string query = "SELECT TraineerId,Name FROM Trainee";
            sqlConnection.Open();
            SqlCommand cmd = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = cmd.ExecuteReader();


            while (reader.Read())
            {

                plans.Add(new TraineeModel
                {
                    TraineerId = reader["TraineerId"].ToString(),
                    Name = reader["Name"].ToString(),


                });



            }
            sqlConnection.Close();

            return plans;

        }



    }
}
