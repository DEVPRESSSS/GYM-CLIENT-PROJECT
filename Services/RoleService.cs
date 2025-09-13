using GYM_CLIENT.DatabaseConnection;
using GYM_CLIENT.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYM_CLIENT.Services
{
    public class RoleService
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        public RoleService()
        {
            sqlConnection = new SqlConnection(connection.ConnectionString);

        }
        public List<RoleModel> GetRoles()
        {
            List<RoleModel> roles = new List<RoleModel>();

            string query = "SELECT RoleId,RoleName FROM Role";
            sqlConnection.Open();
            SqlCommand cmd = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = cmd.ExecuteReader();


            while (reader.Read())
            {

                roles.Add(new RoleModel
                {
                    RoleId = reader["RoleId"].ToString(),
                    RoleName = reader["RoleName"].ToString(),


                });



            }
            sqlConnection.Close();

            return roles;

        }
    }
}
