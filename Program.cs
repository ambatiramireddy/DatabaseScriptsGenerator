using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DatabaseScriptsGenerator
{
    class Program
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["TestConnection"].ConnectionString;

        static void Main(string[] args)
        {
            SqlTable table = new SqlTable();
            table.Owner = "dbo";
            table.Name = "SavedScreen";
            table.NameColumns = new string[] { };

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
                        var primaryKeyDetailsRow = ds.Tables[5].AsEnumerable().FirstOrDefault(row => row[1].ToString().Contains("primary key"));
                        if (primaryKeyDetailsRow != null)
                        {
                            table.KeyColumnNames = primaryKeyDetailsRow[2].ToString().Split(',').Select(s => s.Trim(' ')).ToArray();
                            table.KeyColumnType = "primary";
                        }
                        else
                        {
                            var uniqueKeyDetailsRow = ds.Tables[5].AsEnumerable().FirstOrDefault(row => row[1].ToString().Contains("unique key"));
                            if (uniqueKeyDetailsRow != null)
                            {
                                table.KeyColumnNames = uniqueKeyDetailsRow[2].ToString().Split(',').Select(s => s.Trim(' ')).ToArray();
                                table.KeyColumnType = "unique";
                            }
                        }
                    }
                }
            }

            DataTable dt = null;
            FindForeignKeyRelationHeirarchy(table.Name, ref dt);
            table.ForeignKeyRelations = dt;

            table.GenerateProcs();
        }

        static void FindForeignKeyRelationHeirarchy(string tableName, ref DataTable dt)
        {
            using (var con = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand($"exec sp_fkeys '{tableName}'", con))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet();
                        da.Fill(ds);

                        if (dt == null)
                        {
                            dt = ds.Tables[0].Copy();
                            foreach (string fkTableName in ds.Tables[0].AsEnumerable().Select(row => row[6].ToString()).Distinct())
                            {
                                FindForeignKeyRelationHeirarchy(fkTableName, ref dt);
                            }
                        }
                        else
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                var newRow = dt.NewRow();
                                newRow.ItemArray = row.ItemArray;
                                dt.Rows.Add(newRow);
                            }

                            foreach (string fkTableName in ds.Tables[0].AsEnumerable().Select(row => row[6].ToString()).Distinct())
                            {
                                FindForeignKeyRelationHeirarchy(fkTableName, ref dt);
                            }
                        }
                    }
                }
            }
        }
    }
}
