using System;
using System.Data;
using System.Data.SqlClient;
namespace globelinkapi.Helpers
{
    public class DbAccess
    {
        SqlConnection conn;
        public  DbAccess()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "139.5.147.55";
            builder.UserID = "sa";
            builder.Password = "reallyStrongPWD123";
            builder.InitialCatalog = "globlink";
            conn = new SqlConnection(builder.ConnectionString);
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public DataTable GetData(String sql)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                        //while (reader.Read())
                        //{
                        //    Console.WriteLine(reader.ToString());
                        //}
                        //dt.Load(reader);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return dt;
        }
    }
}
