using System.Globalization;
using System.IO;
using System.Linq;

namespace DatabaseScriptsGenerator
{
    class CommonFunctions
    {
        public static string ConvertDbColumnNameToCSharpPropName(string columnName)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return string.Join(string.Empty, columnName.Split(" _".ToCharArray()).Select(s => textInfo.ToTitleCase(s)));
        }

        public static string ConvertDbColumnNameToCSharpVariableName(string columnName)
        {
            var propName = ConvertDbColumnNameToCSharpPropName(columnName);
            return propName[0].ToString().ToLower() + propName.Substring(1);
        }

        public static string ConvertSqlTypeToDotNetType(ColumnInfo column)
        {
            string dotNetType = string.Empty;

            var sqlType = column.Type;
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

            string[] defaultNullableTypesInDotNet = { "string", "byte[]" };
            return (column.Nullable && defaultNullableTypesInDotNet.All(t => !t.Equals(dotNetType))) ? dotNetType + "?" : dotNetType;
        }

        public static string ConvertSqlValueToDotNetValue(ColumnInfo column)
        {
            string value = string.Empty;

            var sqlType = column.Type;
            switch (sqlType)
            {
               
                case "bit":
                    {
                        value = (column.DefaultValue == "0" ? "false" : "true");
                        break;
                    }
                default:
                    {
                        value = column.DefaultValue;
                        break;
                    }
            }

            return value;
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
