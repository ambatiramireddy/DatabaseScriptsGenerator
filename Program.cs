using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace DatabaseScriptsGenerator
{
    
    class Program
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["TestConnection"].ConnectionString;

        static void Main(string[] args)
        {
            List<SqlTable> tables = new List<SqlTable>();
            tables.Add(new SqlTable { Name = "Screen" });

            if (tables.Count == 0)
            {
                tables = GetTablesList();
            }

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

            var solutionRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../");
            var dbProjectPath = Path.Combine(solutionRootFolder, "DatabaseScriptsGenerator/DatabaseScriptsGenerator.csproj");
            var dbScriptPath = Path.Combine(solutionRootFolder, $"DatabaseScriptsGenerator/Files/Settings.json");
            CommonFunctions.WriteFileToProject("Content", dbScriptPath, dbProjectPath, JsonConvert.SerializeObject(tables, Formatting.Indented));
        }

        static List<SqlTable> GetTablesList()
        {
            List<SqlTable> tables = new List<SqlTable>();
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand($"select * from sys.tables where type ='U'", con))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet();
                        da.Fill(ds);

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            tables.Add(new SqlTable { Name = row[0].ToString() });
                        }
                    }
                }

            }

            return tables;
        }


        static string GenerateTableObjectScripts(SqlTable table)
        {
            int tableNameTableIndex = 0;
            int tableSchemaTableIndex = 1;
            int tableIdentityColDetailsTableIndex = 2;
            int tableRowGuidColDetailsTableIndex = 3;

            int tableConstraintsTableIndex = -1;
            using (var con = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand($"exec sp_help '{table.Owner}.{table.Name}'", con))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet();
                        da.Fill(ds);
                        table.Owner = ds.Tables[tableNameTableIndex].Rows[0][1].ToString();
                        table.ColumnDetails = ds.Tables[tableSchemaTableIndex];
                        var identityColumn = ds.Tables[tableIdentityColDetailsTableIndex].Rows[0][0].ToString();
                        table.IdentityColumn = identityColumn.StartsWith("No identity") ? string.Empty : identityColumn;

                        if (ds.Tables[ds.Tables.Count - 1].Columns[0].ColumnName.Equals("constraint_type"))
                        {
                            tableConstraintsTableIndex = ds.Tables.Count - 1;
                        }
                        else if (ds.Tables[ds.Tables.Count - 2].Columns[0].ColumnName.Equals("constraint_type"))
                        {
                            tableConstraintsTableIndex = ds.Tables.Count - 2;
                        }

                        if (tableConstraintsTableIndex != -1)
                        {
                            var constraintsTableRows = ds.Tables[tableConstraintsTableIndex].AsEnumerable().ToArray();
                            Dictionary<string, string> uniqueForeignKeyDetails = new Dictionary<string, string>();
                            Dictionary<string, string> defaultColumnsAndValues = new Dictionary<string, string>();
                            Dictionary<string, string> foreignKeyDetails = new Dictionary<string, string>();
                            List<string> primaryColumnNames = new List<string>();
                            List<string> uniqueKeyColumnNames = new List<string>();
                            int i = 0;
                            while (i < constraintsTableRows.Length)
                            {
                                var row = constraintsTableRows[i];
                                var firstColumnValue = row[0].ToString();
                                if (firstColumnValue.StartsWith("FOREIGN KEY"))
                                {
                                    foreignKeyDetails.Add(row[6].ToString(), constraintsTableRows[i + 1][6].ToString().Replace("REFERENCES",""));
                                    i = i + 2;
                                    continue;
                                }
                                else if (firstColumnValue.StartsWith("PRIMARY KEY"))
                                {
                                    primaryColumnNames.AddRange(row[6].ToString().Split(',').Select(s => s.Trim(' ')));
                                }
                                else if (firstColumnValue.StartsWith("UNIQUE"))
                                {
                                    uniqueKeyColumnNames.AddRange(row[6].ToString().Split(',').Select(s => s.Trim(' ')));
                                }
                                else if (firstColumnValue.StartsWith("DEFAULT"))
                                {
                                    defaultColumnsAndValues.Add(row[0].ToString().Split(' ').Last(), row[6].ToString().TrimStart('(').TrimEnd(')').Replace("N'", "\"").Replace("'", "\""));
                                }

                                i = i + 1;
                            }

                            if (primaryColumnNames.Count > 0)
                            {
                                table.PrimaryOrUniqueKeyColumnNames = primaryColumnNames;
                                table.KeyColumnCategory = "primary";
                            }
                            else if (uniqueKeyColumnNames.Count > 0)
                            {
                                table.PrimaryOrUniqueKeyColumnNames = uniqueKeyColumnNames;
                                table.KeyColumnCategory = "unique";
                            }

                            foreach (var g in foreignKeyDetails.GroupBy(kv => kv.Value))
                            {
                                uniqueForeignKeyDetails.Add(g.First().Key, g.Key);
                            }

                            table.ForeignKeyReferencingColumns = uniqueForeignKeyDetails;
                            table.DefaultColumnsAndValues = defaultColumnsAndValues;
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
