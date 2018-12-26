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
        string fullTableName;
        List<ColumnInfo> allColumns;
        string nonKeyColumnList;
        string allColumnList;
        string keyColumnList;
        string whereClause;
        string onClause;

        string insertIntoColumnList;
        string insertSelectColumnList;
        bool hasCreatedDateColumn;
        ColumnInfo keyColumn;

        string updateColumnList;
        string keyColumnAndTypeList;
        string keyColumnAndDotNetTypeList;
        string keyColumnDotNetVariableNameList;

        string[] referenceTableDeletes;
        string allColumnAndTypeList;
        string idNameColumnList;
        string hardDeleteStatement;
        string whereClauseForDelete;


        public SqlTable()
        {

        }

        public string Owner { get; set; }
        public string Name { get; set; }
        public string[] NameColumns { get; set; }
        public DataTable ColumnDetails { get; set; }
        public string IdentityColumn { get; set; }
        public string[] KeyColumnNames { get; set; }
        public string KeyColumnCategory { get; set; }
        public DataTable ForeignKeyRelations { get; set; }

        public string GenerateProcs()
        {
            this.fullTableName = $"[{this.Owner}].[{this.Name}]";
            List<ColumnInfo> keyColumns = new List<ColumnInfo>();
            List<ColumnInfo> nonKeyColumns = new List<ColumnInfo>();
            foreach (DataRow row in ColumnDetails.Rows)
            {
                var columnName = row[0].ToString();
                var columnType = row[1].ToString();
                var columnLength = Convert.ToInt32(row[3].ToString());
                columnLength = columnType.ToLower().Equals("nvarchar") ? columnLength / 2 : columnLength;

                var colNullable = row[6].ToString() == "yes" ? true : false;

                //specify length only for char, varchar, nvarchar types
                string colLengthString = columnType.ToLower().Contains("char") ? $"({columnLength})" : string.Empty;

                var modifiedColumnName = string.Join("", columnName.Split(" _".ToCharArray())).ToLower();
                if (modifiedColumnName.Equals("isdeleted"))
                {
                    //ignore
                }
                else if (this.KeyColumnNames != null && this.KeyColumnNames.Any(c => c.Equals(columnName)))
                {
                    keyColumns.Add(new ColumnInfo { Name = columnName, Type = $"{columnType}{colLengthString}", Nullable = colNullable });
                }
                else
                {
                    nonKeyColumns.Add(new ColumnInfo { Name = columnName, Type = $"{columnType}{colLengthString}", Nullable = colNullable });
                }
            }

            allColumns = keyColumns.Concat(nonKeyColumns).ToList();

            var indentation = string.Format("{0}{1}", Environment.NewLine, new string('\t', 2));
            var oneTabSeparator = string.Format(",{0}{1}", Environment.NewLine, new string('\t', 1));
            var twoTabSeparator = string.Format(",{0}{1}", Environment.NewLine, new string('\t', 2));

            if (keyColumns.Count > 0)
            {
                this.keyColumn = keyColumns[0];
                this.keyColumnAndTypeList = string.Join("," + Environment.NewLine, keyColumns.Select(c => $"@{c.Name} {c.Type}"));
                this.keyColumnAndDotNetTypeList = string.Join(", ", keyColumns.Select(c => $"{CommonFunctions.ConvertSqlTypeToDotNetType(c)} {CommonFunctions.ConvertDbColumnNameToCSharpVariableName(c.Name)}"));
                this.keyColumnDotNetVariableNameList = string.Join(" and ", keyColumns.Select(c => $"{CommonFunctions.ConvertDbColumnNameToCSharpVariableName(c.Name)}"));
                this.keyColumnList = string.Join(twoTabSeparator, keyColumns.Select(c => $"[{c.Name}]"));

                this.whereClause = string.Join(" AND ", keyColumns.Select(c => $"{c.Name} = @{c.Name}"));
                this.onClause = string.Join(" AND ", keyColumns.Select(c => $"t.{c.Name} = s.{c.Name}"));
            }

            this.allColumnAndTypeList = string.Join(oneTabSeparator, allColumns.Select(c => string.Format("[{0}] {1} {2}", c.Name, c.Type, c.Nullable ? "NULL" : "NOT NULL")));
            this.nonKeyColumnList = string.Join(twoTabSeparator, nonKeyColumns.Select(c => $"[{c.Name}]"));
            this.allColumnList = indentation + string.Join(twoTabSeparator, allColumns.Select(c => $"[{c.Name}]"));
            this.hasCreatedDateColumn = nonKeyColumns.Any(c => string.Join("", c.Name.Split(" _".ToCharArray())).ToLower().Equals("createddate"));

            var insertColumns = allColumns.Where(c => !c.Name.Equals(this.IdentityColumn)).ToArray();
            this.insertIntoColumnList = indentation + string.Join(twoTabSeparator, insertColumns.Select(c => $"[{c.Name}]"));
            this.insertSelectColumnList = indentation + string.Join(twoTabSeparator, insertColumns.Select(c =>
            {
                var col = string.Join("", c.Name.Split(" _".ToCharArray())).ToLower();
                if (col.Equals("createddate") || col.Equals("modifieddate") || col.Equals("updateddate"))
                {
                    return "@today";
                }
                else if (c.Type.Equals("uniqueidentifier") && c.Name.Equals(this.keyColumn.Name) && this.KeyColumnCategory.Equals("primary"))
                {
                    return "@guid";
                }
                else
                {
                    return $"[{c.Name}]";
                }
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


            this.hardDeleteStatement = GenerateHardDeleteStatement(ForeignKeyRelations, keyColumns);
            var softDeleteStatement = GenerateSoftDeleteStatement(ForeignKeyRelations, keyColumns);

            if (this.NameColumns.Length > 0)
            {
                this.idNameColumnList = string.Format("[id], ({0}) as [name]", string.Join(" + ' ' + ", this.NameColumns.Select(s => $"[{s}]")));
            }

            return this.GenerateScripts();
        }

        private string GenerateScripts()
        {
            string tableTypeName = $"[{this.Owner}].[tt_{this.Name}]";
            string selectProcName = $"[{this.Owner}].[usp_Select_{this.Name}]";
            string selectAllProcName = $"[{this.Owner}].[usp_SelectAll_" + (this.Name.EndsWith("y") ? $"{this.Name.TrimEnd('y')}ies]" : $"{this.Name}s]");
            string insertProcName = $"[{this.Owner}].[usp_Insert_{this.Name}]";
            string updateProcName = $"[{this.Owner}].[usp_Update_{this.Name}]";
            string deleteProcName = $"[{this.Owner}].[usp_Delete_{this.Name}]";

            StringBuilder scriptBuilder = new StringBuilder();
            StringBuilder dropScriptBuilder = new StringBuilder();

            var tableType = new TableType()
            {
                TableTypeName = tableTypeName,
                AllColumnAndTypeList = this.allColumnAndTypeList
            }.TransformText().TrimStart('\r', '\n');

            scriptBuilder.Append(tableType);

            if (!string.IsNullOrEmpty(this.KeyColumnCategory) && this.KeyColumnCategory.Equals("primary"))
            {
                var selectProc = new SelectProc()
                {
                    FullTableName = this.fullTableName,
                    SelectProcName = selectProcName,
                    AllColumnList = this.allColumnList,
                    keyColumnAndTypeList = this.keyColumnAndTypeList,
                    WhereClause = this.whereClause
                }.TransformText().TrimStart('\r', '\n');

                scriptBuilder.Append(selectProc);
                dropScriptBuilder.Append(new DropProc() { ProcName = selectProcName }.TransformText().TrimStart('\r', '\n'));

                var selectAllProc = new SelectAllProc()
                {
                    FullTableName = this.fullTableName,
                    SelectAllProcName = selectAllProcName,
                    AllColumnList = this.allColumnList,
                }.TransformText().TrimStart('\r', '\n');

                scriptBuilder.Append(selectAllProc);
                dropScriptBuilder.Append(new DropProc() { ProcName = selectAllProcName }.TransformText().TrimStart('\r', '\n'));

                var updateProc = new UpdateProc()
                {
                    TableName = this.Name,
                    FullTableName = this.fullTableName,
                    UpdateProcName = updateProcName,
                    UpdateColumnList = this.updateColumnList,
                    OnClause = this.onClause
                }.TransformText().TrimStart('\r', '\n');

                scriptBuilder.Append(updateProc);
                dropScriptBuilder.Append(new DropProc() { ProcName = updateProcName }.TransformText().TrimStart('\r', '\n'));

                if (this.NameColumns.Length > 0)
                {
                    var selectIdNamePairsProcName = $"[{this.Owner}].[usp_Select_{this.Name}_IdNamePairs]";
                    var selectIdNamePairsProc = new SelectIdNamePairsProc()
                    {
                        FullTableName = this.fullTableName,
                        SelectIdNamePairsProcName = selectIdNamePairsProcName,
                        IdNameColumnList = this.idNameColumnList,
                    }.TransformText().TrimStart('\r', '\n');

                    scriptBuilder.Append(selectIdNamePairsProc);
                    dropScriptBuilder.Append(new DropProc() { ProcName = selectIdNamePairsProcName }.TransformText().TrimStart('\r', '\n'));
                }
            }

            var insertProc = new InsertProc()
            {
                TableName = this.Name,
                FullTableName = this.fullTableName,
                InsertProcName = insertProcName,
                InsertIntoColumnList = this.insertIntoColumnList,
                InsertSelectColumnList = this.insertSelectColumnList,
                IdentityColumn = this.IdentityColumn,
                HasCreatedDateColumn = this.hasCreatedDateColumn,
                IsKeyColumnGuidColumn = (this.keyColumn != null && this.keyColumn.Type.Equals("uniqueidentifier"))
            }.TransformText().TrimStart('\r', '\n');

            scriptBuilder.Append(insertProc);
            dropScriptBuilder.Append(new DropProc() { ProcName = insertProcName }.TransformText().TrimStart('\r', '\n'));

            var deleteProc = new DeleteProc()
            {
                DeleteProcName = deleteProcName,
                keyColumnAndTypeList = this.keyColumnAndTypeList,
                DeleteStatement = this.hardDeleteStatement
            }.TransformText().TrimStart('\r', '\n');

            scriptBuilder.Append(deleteProc);
            dropScriptBuilder.Append(new DropProc() { ProcName = deleteProcName }.TransformText().TrimStart('\r', '\n'));

            dropScriptBuilder.Append(new DropType() { TableName = this.Name, TypeName = tableTypeName }.TransformText().TrimStart('\r', '\n'));

            scriptBuilder.Insert(0, dropScriptBuilder.ToString());
            var script = scriptBuilder.ToString();

            var solutionRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../");

            var dbProjectPath = Path.Combine(solutionRootFolder, "DatabaseScriptsGenerator/DatabaseScriptsGenerator.csproj");
            var dbScriptPath = Path.Combine(solutionRootFolder, "DatabaseScriptsGenerator/Files/Script.sql");
            CommonFunctions.WriteFileToProject("Content", dbScriptPath, dbProjectPath, script);

            var entity = new Entity()
            {
                TableName = this.Name,
                Columns = allColumns
            }.TransformText().TrimStart('\r', '\n');

            var apiProjectPath = Path.Combine(solutionRootFolder, "AddAppAPI/AddAppAPI.csproj");

            var entityPath = Path.Combine(solutionRootFolder, $"AddAppAPI/Models/{this.Name}.cs");
            CommonFunctions.WriteFileToProject("Compile", entityPath, apiProjectPath, entity);

            var lowerCaseTableName = this.Name[0].ToString().ToLower() + this.Name.Substring(1);
            var entityController = new EntityController()
            {
                TableName = this.Name,
                LowerCaseTableName = lowerCaseTableName,
                PluralCaseTableName = lowerCaseTableName.EndsWith("y") ? (lowerCaseTableName.TrimEnd('y') + "ies") : (lowerCaseTableName + "s"),
                KeyColumnType = CommonFunctions.ConvertSqlTypeToDotNetType(this.keyColumn),
                KeyColumnCategory = this.KeyColumnCategory,
                HasNameColumn = this.NameColumns.Length > 0,
                KeyColumnNames = this.KeyColumnNames,
                KeyColumnAndDotNetTypeList = this.keyColumnAndDotNetTypeList,
                KeyColumnDotNetVariableNameList = this.keyColumnDotNetVariableNameList
            }.TransformText().TrimStart('\r', '\n');

            var entityControllerPath = Path.Combine(solutionRootFolder, $"AddAppAPI/Controllers/{this.Name}Controller.cs");
            CommonFunctions.WriteFileToProject("Compile", entityControllerPath, apiProjectPath, entityController);

            return script;
        }

        private string GenerateHardDeleteStatement(DataTable ForeignKeyRelations, List<ColumnInfo> keyColumns)
        {
            StringBuilder db = new StringBuilder();
            this.whereClauseForDelete = string.Join(" AND ", keyColumns.Select(c => $"[{this.Owner}].[{this.Name}].[{c.Name}] = @{c.Name}"));
            int levelColumnInex = ForeignKeyRelations.Columns.Count - 1;

            //to build delete statements in reverse order from most referenced to least referenced
            var rows = ForeignKeyRelations.AsEnumerable().OrderByDescending(row => row[levelColumnInex]).ToArray();

            //to construct delete statements for child(fk) tables
            //grouping to get one group for each reference table (will have mutliple rows if primary key is a mutli-column key)
            foreach (var g in rows.GroupBy(r => r[6].ToString()).ToArray())
            {
                StringBuilder currentDeleteStatementBuilder = new StringBuilder();
                var firstRowInGroup = g.First();
                var fkTableOwner = firstRowInGroup[5].ToString();
                var fkTableName = firstRowInGroup[6].ToString();

                currentDeleteStatementBuilder.Append($"\tDELETE [{fkTableOwner}].[{fkTableName}]{Environment.NewLine}");
                currentDeleteStatementBuilder.Append($"\tFROM [{fkTableOwner}].[{fkTableName}]{Environment.NewLine}");
                var pkTableName = CreateInnerJoinLine(g.ToArray(), currentDeleteStatementBuilder);

                for (int i = (int)firstRowInGroup["level"] - 1; i >= 0; i--)
                {
                    //to get parent table rows for current parent (pk) table
                    //will return mutliple rows if primary key is a mutli-column key
                    var nextLevelRows = rows.Where(r => (int)r[levelColumnInex] == i && r[6].ToString() == pkTableName).ToArray();
                    pkTableName = CreateInnerJoinLine(nextLevelRows, currentDeleteStatementBuilder);
                }

                currentDeleteStatementBuilder.Append($"\tWHERE {this.whereClauseForDelete}{Environment.NewLine}");
                db.Append($"{currentDeleteStatementBuilder.ToString()}{Environment.NewLine}");
            }

            //parent table delete statement
            db.Append($"\tDELETE FROM {this.fullTableName} WHERE {this.whereClauseForDelete}{Environment.NewLine}");

            var deleteStatement = db.ToString();
            return deleteStatement;
        }

        private string CreateInnerJoinLine(DataRow[] currentLevelRows, StringBuilder currentDeleteStatementBuilder)
        {
            StringBuilder sb = new StringBuilder();
            var firstRow = currentLevelRows[0];

            //below values are same for all rows in a level
            var pkTableOwner = firstRow[1].ToString();
            var pkTableName = firstRow[2].ToString();
            var fkTableOwner = firstRow[5].ToString();
            var fkTableName = firstRow[6].ToString();

            currentDeleteStatementBuilder.Append($"\tINNER JOIN [{currentLevelRows[0][1].ToString()}].[{currentLevelRows[0][2].ToString()}] ON ");
            List<string> onClauseParts = new List<string>();
            foreach (var currentLevelRow in currentLevelRows)
            {
                var pkColumnName = currentLevelRow[3].ToString();
                var fkColumnName = currentLevelRow[7].ToString();
                onClauseParts.Add($"[{pkTableOwner}].[{pkTableName}].[{pkColumnName}] = [{fkTableOwner}].[{fkTableName}].[{fkColumnName}]");
            }

            currentDeleteStatementBuilder.Append($"{string.Join(" AND ", onClauseParts)}{Environment.NewLine}");
            return pkTableName;
        }

        private string GenerateSoftDeleteStatement(DataTable ForeignKeyRelations, List<ColumnInfo> keyColumns)
        {
            StringBuilder db = new StringBuilder();
            db.Append($"UPDATE {this.Name}{Environment.NewLine}");
            db.Append($"SET{Environment.NewLine}");
            foreach (DataRow r in ForeignKeyRelations.Rows)
            {
                var fkTableOwner = r[5].ToString();
                var fkTableName = r[6].ToString();

                db.Append($"\t[{fkTableOwner}].[{fkTableName}].[is_deleted] = 1{Environment.NewLine}");
            }


            db.Append($"FROM {this.Name}{Environment.NewLine}");
            foreach (DataRow r in ForeignKeyRelations.Rows)
            {
                var pkTableOwner = r[1].ToString();
                var pkTableName = r[2].ToString();
                var pkColumnName = r[3].ToString();
                var fkTableOwner = r[5].ToString();
                var fkTableName = r[6].ToString();
                var fkColumnName = r[7].ToString();

                db.Append($"INNER JOIN [{fkTableOwner}].[{fkTableName}] ON [{pkTableOwner}].[{pkTableName}].[{pkColumnName}] = [{fkTableOwner}].[{fkTableName}].[{fkColumnName}]{Environment.NewLine}");
            }
            this.whereClauseForDelete = string.Join(" AND ", keyColumns.Select(c => $"[{this.Owner}].[{this.Name}].[{c.Name}] = @{c.Name}"));
            db.Append($"WHERE {this.whereClauseForDelete}");
            var deleteStatement = db.ToString();

            return deleteStatement;
        }
    }
}
