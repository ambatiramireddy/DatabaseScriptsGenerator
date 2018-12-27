using System;
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
            List<SqlTable> tables = new List<SqlTable>();
            tables.Add(new SqlTable { Owner = "dbo", Name = "Country", NameColumns = new string[] { "name" } });
           

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                foreach (var table in tables)
                {
                    var sqlScript = GenerateTableObjectScripts(table);

                    foreach (var objectScript in sqlScript.Split(new[] { "GO" }, StringSplitOptions.None))
                    {
                        using (var cmd = new SqlCommand($"exec sp_executesql  N'{objectScript.Replace("'", "''")}'", con))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        

        static string GenerateTableObjectScripts(SqlTable table)
        {
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
                        var indexDetailsTable = ds.Tables[5].AsEnumerable();
                        var primaryKeyDetailsRow = indexDetailsTable.FirstOrDefault(row => row[1].ToString().Contains("primary key"));
                        if (primaryKeyDetailsRow != null)
                        {
                            table.KeyColumnNames = primaryKeyDetailsRow[2].ToString().Split(',').Select(s => s.Trim(' ')).ToArray();
                            table.KeyColumnCategory = "primary";
                        }
                        else
                        {
                            var uniqueKeyDetailsRow = indexDetailsTable.FirstOrDefault(row => row[1].ToString().Contains("unique key"));
                            if (uniqueKeyDetailsRow != null)
                            {
                                table.KeyColumnNames = uniqueKeyDetailsRow[2].ToString().Split(',').Select(s => s.Trim(' ')).ToArray();
                                table.KeyColumnCategory = "unique";
                            }
                        }
                    }
                }
            }

            DataTable dt = null;
            FindForeignKeyRelationHeirarchy(table.Name, ref dt, 0);
            table.ForeignKeyRelations = dt;

            return table.GenerateProcs();
        }

        static void FindForeignKeyRelationHeirarchy(string tableName, ref DataTable dt, int level)
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
                            dt.Columns.Add(new DataColumn("level", typeof(int)));
                            foreach (DataRow row in dt.Rows)
                            {
                                row["level"] = level;
                            }

                            // taking distinct because primary key can be applied on multiple columns. 
                            // which causes mutliple records in sp_fkeys result set for a single table
                            foreach (string fkTableName in ds.Tables[0].AsEnumerable().Select(row => row[6].ToString()).Distinct())
                            {
                                FindForeignKeyRelationHeirarchy(fkTableName, ref dt, level+1);
                            }
                        }
                        else
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                var newRow = dt.NewRow();
                                newRow.ItemArray = row.ItemArray;
                                newRow["level"] = level;
                                dt.Rows.Add(newRow);
                            }

                            foreach (string fkTableName in ds.Tables[0].AsEnumerable().Select(row => row[6].ToString()).Distinct())
                            {
                                FindForeignKeyRelationHeirarchy(fkTableName, ref dt, level+1);
                            }
                        }
                    }
                }
            }
        }
    }
}
