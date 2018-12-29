using System.Globalization;
using System.IO;
using System.Linq;

namespace DatabaseScriptsGenerator
{
    class CommonFunctions
    {
        public static string ConvertDbColNameToDotNetPropName(string columnName)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return string.Join(string.Empty, columnName.Split(" _".ToCharArray()).Select(s => textInfo.ToTitleCase(s)));
        }

        public static string ConvertDbColNameToDotNetVariableName(string columnName)
        {
            var propName = ConvertDbColNameToDotNetPropName(columnName);
            return propName[0].ToString().ToLower() + propName.Substring(1);
        }

        public static string ConvertSqlTypeToDotNetType(string sqlType)
        {
            string dotNetType = string.Empty;
            switch (sqlType)
            {
                case "tinyint":
                    {
                        dotNetType = "byte";
                        break;
                    }
                case "smallint":
                    {
                        dotNetType = "short";
                        break;
                    }
                case "int":
                    {
                        dotNetType = "int";
                        break;
                    }
                case "bigint":
                    {
                        dotNetType = "long";
                        break;
                    }
                case "numeric":
                case "money":
                    {
                        dotNetType = "decimal";
                        break;
                    }
                case "date":
                case "datetime":
                    {
                        dotNetType = "DateTime";
                        break;
                    }
                case "time":
                    {
                        dotNetType = "TimeSpan";
                        break;
                    }
                case "image":
                case "varbinary":
                    {
                        dotNetType = "byte[]";
                        break;
                    }
                case "uniqueidentifier":
                    {
                        dotNetType = "Guid";
                        break;
                    }
                case "real":
                    {
                        dotNetType = "Single";
                        break;
                    }
                case "float":
                    {
                        dotNetType = "double";
                        break;
                    }
                case "sql_variant":
                    {
                        dotNetType = "object";
                        break;
                    }
                case "bit":
                    {
                        dotNetType = "bool";
                        break;
                    }
                default:
                    {
                        dotNetType = "string";
                        break;
                    }
            }

            return dotNetType;
        }


        public static string ConvertSqlTypeToDotNetType(string sqlType, bool colNullable)
        {
            string dotNetType = ConvertSqlTypeToDotNetType(sqlType);
            string[] defaultNullableTypesInDotNet = { "string", "byte[]", "object" };
            return (colNullable && defaultNullableTypesInDotNet.All(t => !t.Equals(dotNetType))) ? dotNetType + "?" : dotNetType;
        }

        public static string ConvertSqlValueToDotNetValue(ColumnInfo column)
        {
            string value = string.Empty;

            var sqlType = column.Type;
            switch (sqlType)
            {

                case "bit":
                    {
                        //in c#, default value for bool type is false. hence, no need to set to false
                        value = (column.DefaultValue == "1" ? "true" : string.Empty);
                        break;
                    }
                default:
                    {
                        value = column.DefaultValue;
                        break;
                    }
            }

            return value == string.Empty ? string.Empty : $" = {value};";
        }

        public static void WriteFileToProject(string itemType, string filePath, string projectPath, string content)
        {
            //if (!File.Exists(filePath))
            //{
            //    File.WriteAllText(filePath, content);
            //    var p = new Microsoft.Build.Evaluation.Project(projectPath);
            //    p.AddItem(itemType, filePath);//Compile for .cs file and Content for other files .sql, .txt etc..
            //    p.Save();
            //}
            //else
            //{
                File.WriteAllText(filePath, content);
            //}
        }
    }
}
