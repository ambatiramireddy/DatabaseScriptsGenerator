using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseScriptsGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlTable table = new SqlTable();
            table.Owner = "dbo";
            table.Name = "Screen";
            string connectionString = ConfigurationManager.ConnectionStrings["TestConnection"].ConnectionString;

            using (var con = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand($"exec sp_help '{table.Owner}.{table.Name}'", con))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet();
                        da.Fill(ds);
                        table.ColumnDetails = ds.Tables[1];
                        table.KeyColumns = ds.Tables[5].Rows[0][2].ToString();
                    }
                }
            }

            using (var con = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand($"exec sp_fkeys '{table.Name}'", con))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet();
                        da.Fill(ds);
                        table.ForeignKeyRelations = ds.Tables[0];
                    }
                }
            }

            table.GenerateProcs();
        }
    }
}
