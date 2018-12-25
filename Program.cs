using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DatabaseScriptsGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlTable table = new SqlTable();
            table.Owner = "dbo";
            table.Name = "Request";
            table.NameColumns = new List<string> {  };

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
                        var identityColumn = ds.Tables[2].Rows[0][0].ToString();
                        table.IdentityColumn = identityColumn.StartsWith("No identity") ? string.Empty : identityColumn;
                        table.KeyColumns = ds.Tables[5].Rows[0][2].ToString().Split(',').Select(s => s.Trim(' ')).ToArray();
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
