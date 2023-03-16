using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    public class GestioneDB
    {
        public string ConnectionString { get; set; } = "data source=DESKTOP-IDSL57F\\SQLEXPRESS2019;initial catalog=WinGrinderRemoteDB;persist security info=False;user id=tenova;Password=Nirvana64!;packet size=4096;Timeout=30;TrustServerCertificate=True";
        public SqlConnection Connection { get; set; }
        public SqlCommand Command { get; set; }
        public SqlDataReader Reader { get; set; }
        public GestioneDB() 
        {
            Connection= new SqlConnection(ConnectionString);
            Command = Connection.CreateCommand();
            Command.CommandType = CommandType.Text;
        } 

        public void Execute(string name)
        {
            Command.CommandText = name;
            using (Connection)
            {
                Connection.Open();
                Command.ExecuteNonQuery();
            }
        }

        public string DataReader(string spec)
        {
            string response = "";
            string query = "select * from CF_Users where Settings='" + spec + "'";
            Command.CommandText = query;
            using (Connection)
            {
                Connection.Open();
                Reader = Command.ExecuteReader();
                while (Reader.Read())
                {
                    response = Reader["Settings"].ToString();
                }
            }
            return response;
        }
    }
}
