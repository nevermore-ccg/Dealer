using System;
using System.Data.SqlClient;
using System.IO;
using System.Data;

namespace Dealer.Классы
{
    class DataBase
    {
        SqlConnection sqlconnection = new SqlConnection(@"Data Source=.\SQLEXPRESS01;Initial Catalog=DealerDB;Integrated Security=True");

        public void OpenConnection()
        {
            if (sqlconnection.State == ConnectionState.Closed)
            {
                sqlconnection.Open();
            }
        }

        public void CloseConnection()
        {
            if (sqlconnection.State == ConnectionState.Open)
            {
                sqlconnection.Close();
            }
        }

        public SqlConnection GetConnection()
        {
            return sqlconnection;
        }
    }
}
