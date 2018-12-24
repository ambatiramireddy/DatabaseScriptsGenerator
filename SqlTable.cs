using DatabaseScriptsGenerator.Templates;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseScriptsGenerator
{
    public class SqlTable
    {
        string nonKeyColumnList;
        string allColumnList;
        string keyColumnList;
        string whereClause;
        string onClause;
        string insertColumnList;
        string updateColumnList;
        string keyColumnAndTypeList;
        bool hasCreatedDateColumn;
        string[] referenceTableDeletes;
        string referenceTableWhereClause;
        string allColumnAndTypeList;
        List<ColumnInfo> allColumns;

        public SqlTable()
        {

        }

        public string Owner { get; set; }
        public string Name { get; set; }
        public DataTable ColumnDetails { get; set; }
        public string KeyColumns { get; set; }
        public DataTable ForeignKeyRelations { get; set; }

        public void GenerateProcs()
        {
            List<ColumnInfo> keyColumns = new List<ColumnInfo>();
            List<ColumnInfo> nonKeyColumns = new List<ColumnInfo>();
            foreach (DataRow row in ColumnDetails.Rows)
            {
                var colName = row[0].ToString();
                var colType = row[1].ToString();
                var colLength = Convert.ToInt32(row[3].ToString());
                colLength = colType.ToLower().Equals("nvarchar") ? colLength / 2 : colLength;

                var colNullable = row[6].ToString() == "yes" ? true : false;

                //specify length only for char, varchar, nvarchar types
                string colLengthString = colType.ToLower().Contains("char") ? $"({colLength})" : string.Empty;
                if (colName.Equals(KeyColumns))
                {
                    keyColumns.Add(new ColumnInfo { Name = colName, Type = $"{colType}{colLengthString}", Nullable = colNullable });
                }
                else
                {
                    nonKeyColumns.Add(new ColumnInfo { Name = colName, Type = $"{colType}{colLengthString}", Nullable = colNullable });
                }
            }

            allColumns = keyColumns.Concat(nonKeyColumns).ToList();

            var indentation = string.Format("{0}{1}", Environment.NewLine, new string('\t', 2));
            var oneTabSeparator = string.Format(",{0}{1}", Environment.NewLine, new string('\t', 1));
            var twoTabSeparator = string.Format(",{0}{1}", Environment.NewLine, new string('\t', 2));

            this.allColumnAndTypeList = string.Join(oneTabSeparator, allColumns.Select(c => string.Format("[{0}] {1} {2}", c.Name, c.Type, c.Nullable ? "NULL" : "NOT NULL")));
            this.keyColumnAndTypeList = string.Join("," + Environment.NewLine, keyColumns.Select(c => $"@{c.Name} {c.Type}"));
            this.keyColumnList = string.Join(twoTabSeparator, keyColumns.Select(c => $"[{c.Name}]"));
            this.nonKeyColumnList = string.Join(twoTabSeparator, nonKeyColumns.Select(c => $"[{c.Name}]"));
            this.allColumnList = indentation + string.Join(twoTabSeparator, allColumns.Select(c => $"[{c.Name}]"));
            this.whereClause = string.Join("AND", keyColumns.Select(c => $"{c.Name} = @{c.Name}"));
            this.onClause = string.Join("AND", keyColumns.Select(c => $"t.{c.Name} = s.{c.Name}"));

            this.hasCreatedDateColumn = nonKeyColumns.Any(c => string.Join("", c.Name.Split(" _".ToCharArray())).ToLower().Equals("createddate"));

            this.insertColumnList = indentation + string.Join(twoTabSeparator, allColumns.Select(c =>
            {
                var col = string.Join("", c.Name.Split(" _".ToCharArray())).ToLower();
                if (col.Equals("createddate") || col.Equals("modifieddate") || col.Equals("updateddate"))
                    return "@today";
                else
                    return $"[{c.Name}]";
            }));

            this.updateColumnList = indentation + string.Join(twoTabSeparator, nonKeyColumns.Select(c =>
            {
                var col = string.Join("", c.Name.Split(" _".ToCharArray())).ToLower();
                if (col.Equals("createddate"))
                {
                    //createDate column should not be updated
                    return string.Empty;
                }
                else if (col.Equals("modifieddate") || col.Equals("updateddate"))
                {
                    return $"[{c.Name}] = GETUTCDATE()";
                }
                else
                {
                    return $"[{c.Name}] = s.[{c.Name}]";
                }
            }).Where(s => s != string.Empty));

            referenceTableDeletes = ForeignKeyRelations.AsEnumerable().Select(r =>
            {
                var fkTableOwner = r[5].ToString();
                var fkTableName = r[6].ToString();
                var fkColumnName = r[7].ToString();
                var pkColumnName = r[3].ToString();
                return $"DELETE FROM [{fkTableOwner}].[{fkTableName}] WHERE [{fkColumnName}] = @{pkColumnName}";
            }).ToArray();

            this.GenerateScripts();
        }

        private void GenerateScripts()
        {
            string tableTypeName = $"[{this.Owner}].[tt_{this.Name}]";
            string selectProcName = $"[{this.Owner}].[usp_Select_{this.Name}]";
            string selectAllProcName = $"[{this.Owner}].[usp_SelectAll_{this.Name}s]";
            string insertProcName = $"[{this.Owner}].[usp_Insert_{this.Name}]";
            string updateProcName = $"[{this.Owner}].[usp_Update_{this.Name}]";
            string deleteProcName = $"[{this.Owner}].[usp_Delete_{this.Name}]";

            var dropScript = new DropScript()
            {
                TableName = this.Name,
                TableTypeName = tableTypeName,
                SelectProcName = selectProcName,
                SelectAllProcName = selectAllProcName,
                InsertProcName = insertProcName,
                UpdateProcName = updateProcName,
                DeleteProcName = deleteProcName,
            }.TransformText().TrimStart('\r', '\n');

            var tableType = new TableType()
            {
                TableName = this.Name,
                Owner = this.Owner,
                TableTypeName = tableTypeName,
                AllColumnAndTypeList = this.allColumnAndTypeList
            }.TransformText().TrimStart('\r', '\n');

            var selectProc = new SelectProc()
            {
                TableName = this.Name,
                Owner = this.Owner,
                SelectProcName = selectProcName,
                AllColumnList = this.allColumnList,
                keyColumnAndTypeList = this.keyColumnAndTypeList,
                WhereClause = this.whereClause
            }.TransformText().TrimStart('\r', '\n');

            var selectAllProc = new SelectAllProc()
            {
                TableName = this.Name,
                Owner = this.Owner,
                SelectAllProcName = selectAllProcName,
                AllColumnList = this.allColumnList,
            }.TransformText().TrimStart('\r', '\n');

            var insertProc = new InsertProc()
            {
                TableName = this.Name,
                Owner = this.Owner,
                InsertProcName = insertProcName,
                AllColumnList = this.allColumnList,
                InsertColumnList = this.insertColumnList,
                HasCreatedDateColumn = this.hasCreatedDateColumn
            }.TransformText().TrimStart('\r', '\n');

            var updateProc = new UpdateProc()
            {
                TableName = this.Name,
                Owner = this.Owner,
                UpdateProcName = updateProcName,
                UpdateColumnList = this.updateColumnList,
                OnClause = this.onClause
            }.TransformText().TrimStart('\r', '\n');

            var deleteProc = new DeleteProc()
            {
                TableName = this.Name,
                Owner = this.Owner,
                DeleteProcName = deleteProcName,
                keyColumnAndTypeList = this.keyColumnAndTypeList,
                WhereClause = this.whereClause,
                ReferenceTableDeletes = this.referenceTableDeletes,
            }.TransformText().TrimStart('\r', '\n');

            StringBuilder sb = new StringBuilder();
            sb.Append(dropScript);
            sb.Append(tableType);
            sb.Append(selectProc);
            sb.Append(selectAllProc);
            sb.Append(insertProc);
            sb.Append(updateProc);
            sb.Append(deleteProc);

            var script = sb.ToString();

            var solutionRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../");
            

            var dbProjectPath = Path.Combine(solutionRootFolder, "DatabaseScriptsGenerator/DatabaseScriptsGenerator.csproj");
            var dbScriptPath = Path.Combine(solutionRootFolder, "DatabaseScriptsGenerator/Files/Script.sql");
            CommonFunctions.WriteFileToProject("Content", dbScriptPath, dbProjectPath, script);

            var entity = new Entity()
            {
                TableName = this.Name,
                Columns = allColumns
            }.TransformText().TrimStart('\r', '\n');

            var entityPath = Path.Combine(solutionRootFolder, $"AddAppAPI/Models/{this.Name}.cs");
            var apiProjectPath = Path.Combine(solutionRootFolder, "AddAppAPI/AddAppAPI.csproj");
            CommonFunctions.WriteFileToProject("Compile", entityPath, apiProjectPath, entity);
        }
    }
}
