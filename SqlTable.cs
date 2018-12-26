﻿using DatabaseScriptsGenerator.Templates;
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
        public List<string> NameColumns { get; set; }
        public DataTable ColumnDetails { get; set; }
        public string IdentityColumn { get; set; }
        public string[] KeyColumns { get; set; }
        public DataTable ForeignKeyRelations { get; set; }

        public void GenerateProcs()
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
                else if (KeyColumns.Any(c => c.Equals(columnName)))
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

            this.allColumnAndTypeList = string.Join(oneTabSeparator, allColumns.Select(c => string.Format("[{0}] {1} {2}", c.Name, c.Type, c.Nullable ? "NULL" : "NOT NULL")));
            this.keyColumnAndTypeList = string.Join("," + Environment.NewLine, keyColumns.Select(c => $"@{c.Name} {c.Type}"));
            this.keyColumnList = string.Join(twoTabSeparator, keyColumns.Select(c => $"[{c.Name}]"));
            this.nonKeyColumnList = string.Join(twoTabSeparator, nonKeyColumns.Select(c => $"[{c.Name}]"));
            this.allColumnList = indentation + string.Join(twoTabSeparator, allColumns.Select(c => $"[{c.Name}]"));

            this.whereClause = string.Join(" AND ", keyColumns.Select(c => $"{c.Name} = @{c.Name}"));
            this.onClause = string.Join(" AND ", keyColumns.Select(c => $"t.{c.Name} = s.{c.Name}"));

            this.hasCreatedDateColumn = nonKeyColumns.Any(c => string.Join("", c.Name.Split(" _".ToCharArray())).ToLower().Equals("createddate"));

            this.keyColumn = keyColumns[0];
            var insertColumns = allColumns.Where(c => !c.Name.Equals(this.IdentityColumn)).ToArray();
            this.insertIntoColumnList = indentation + string.Join(twoTabSeparator, insertColumns.Select(c => $"[{c.Name}]"));
            this.insertSelectColumnList = indentation + string.Join(twoTabSeparator, insertColumns.Select(c =>
            {
                var col = string.Join("", c.Name.Split(" _".ToCharArray())).ToLower();
                if (col.Equals("createddate") || col.Equals("modifieddate") || col.Equals("updateddate"))
                {
                    return "@today";
                }
                else if (c.Type.Equals("uniqueidentifier"))
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

            if (this.NameColumns.Count > 0)
            {
                this.idNameColumnList = string.Format("[id], ({0}) as [name]", string.Join(" + ' ' + ", this.NameColumns.Select(s => $"[{s}]")));
            }

            this.GenerateScripts();
        }

        private void GenerateScripts()
        {
            string tableTypeName = $"[{this.Owner}].[tt_{this.Name}]";
            string selectProcName = $"[{this.Owner}].[usp_Select_{this.Name}]";
            string selectAllProcName = $"[{this.Owner}].[usp_SelectAll_" + (this.Name.EndsWith("y") ? $"{this.Name.TrimEnd('y')}ies]" : $"{this.Name}s]");
            string insertProcName = $"[{this.Owner}].[usp_Insert_{this.Name}]";
            string updateProcName = $"[{this.Owner}].[usp_Update_{this.Name}]";
            string deleteProcName = $"[{this.Owner}].[usp_Delete_{this.Name}]";

            var tableType = new TableType()
            {
                TableTypeName = tableTypeName,
                AllColumnAndTypeList = this.allColumnAndTypeList
            }.TransformText().TrimStart('\r', '\n');

            var selectProc = new SelectProc()
            {
                FullTableName = this.fullTableName,
                SelectProcName = selectProcName,
                AllColumnList = this.allColumnList,
                keyColumnAndTypeList = this.keyColumnAndTypeList,
                WhereClause = this.whereClause
            }.TransformText().TrimStart('\r', '\n');

            var selectAllProc = new SelectAllProc()
            {
                FullTableName = this.fullTableName,
                SelectAllProcName = selectAllProcName,
                AllColumnList = this.allColumnList,
            }.TransformText().TrimStart('\r', '\n');

            var insertProc = new InsertProc()
            {
                TableName = this.Name,
                FullTableName = this.fullTableName,
                InsertProcName = insertProcName,
                InsertIntoColumnList = this.insertIntoColumnList,
                InsertSelectColumnList = this.insertSelectColumnList,
                IdentityColumn = this.IdentityColumn,
                HasCreatedDateColumn = this.hasCreatedDateColumn,
                IsKeyColumnGuidColumn = this.keyColumn.Type.Equals("uniqueidentifier")
            }.TransformText().TrimStart('\r', '\n');

            var updateProc = new UpdateProc()
            {
                TableName = this.Name,
                FullTableName = this.fullTableName,
                UpdateProcName = updateProcName,
                UpdateColumnList = this.updateColumnList,
                OnClause = this.onClause
            }.TransformText().TrimStart('\r', '\n');

            var deleteProc = new DeleteProc()
            {
                DeleteProcName = deleteProcName,
                keyColumnAndTypeList = this.keyColumnAndTypeList,
                DeleteStatement = this.hardDeleteStatement
            }.TransformText().TrimStart('\r', '\n');

            string selectIdNamePairsProcName = string.Empty;
            string selectIdNamePairsProc = string.Empty;
            if (!string.IsNullOrEmpty(this.idNameColumnList))
            {
                selectIdNamePairsProcName = $"[{this.Owner}].[usp_Select_{this.Name}_IdNamePairs]";
                selectIdNamePairsProc = new SelectIdNamePairsProc()
                {
                    FullTableName = this.fullTableName,
                    SelectIdNamePairsProcName = selectIdNamePairsProcName,
                    IdNameColumnList = this.idNameColumnList,
                }.TransformText().TrimStart('\r', '\n');
            }

            var dropScript = new DropScript()
            {
                TableName = this.Name,
                TableTypeName = tableTypeName,
                SelectProcName = selectProcName,
                SelectAllProcName = selectAllProcName,
                SelectIdNamePairsProcName = selectIdNamePairsProcName,
                InsertProcName = insertProcName,
                UpdateProcName = updateProcName,
                DeleteProcName = deleteProcName,
            }.TransformText().TrimStart('\r', '\n');

            StringBuilder sb = new StringBuilder();
            sb.Append(dropScript);
            sb.Append(tableType);

            sb.Append(selectProc);
            sb.Append(selectAllProc);
            if (!string.IsNullOrEmpty(selectIdNamePairsProc))
            {
                sb.Append(selectIdNamePairsProc);
            }

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
                HasNameColumn = this.NameColumns.Count > 0
            }.TransformText().TrimStart('\r', '\n');

            var entityControllerPath = Path.Combine(solutionRootFolder, $"AddAppAPI/Controllers/{this.Name}Controller.cs");
            CommonFunctions.WriteFileToProject("Compile", entityControllerPath, apiProjectPath, entityController);
        }

        private string GenerateHardDeleteStatement(DataTable ForeignKeyRelations, List<ColumnInfo> keyColumns)
        {
            StringBuilder db = new StringBuilder();
            db.Append($"DELETE {this.fullTableName}{Environment.NewLine}");
            db.Append($"\tFROM {this.fullTableName}{Environment.NewLine}");
            //row[5] is fkTableOwner, row[6] is fkTableName
            foreach (var g in ForeignKeyRelations.AsEnumerable().GroupBy(row=> $"[{row[5].ToString()}].[{row[6].ToString()}]"))
            {
                db.Append($"\tINNER JOIN {g.Key} ON ");

                List<string> onClauseParts = new List<string>();
                foreach (DataRow row in g)
                {
                    var pkTableOwner = row[1].ToString();
                    var pkTableName = row[2].ToString();
                    var pkColumnName = row[3].ToString();
                    var fkTableOwner = row[5].ToString();
                    var fkTableName = row[6].ToString();
                    var fkColumnName = row[7].ToString();

                    onClauseParts.Add($"[{pkTableOwner}].[{pkTableName}].[{pkColumnName}] = [{fkTableOwner}].[{fkTableName}].[{fkColumnName}]");
                }

                db.Append($"{string.Join(" AND ", onClauseParts)}{Environment.NewLine}");
            }
            this.whereClauseForDelete = string.Join(" AND ", keyColumns.Select(c => $"[{this.Owner}].[{this.Name}].[{c.Name}] = @{c.Name}"));
            db.Append($"\tWHERE {this.whereClauseForDelete}");
            var deleteStatement = db.ToString();

            return deleteStatement;
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
