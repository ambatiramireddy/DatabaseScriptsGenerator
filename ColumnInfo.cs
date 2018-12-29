namespace DatabaseScriptsGenerator
{
    public class ColumnInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsKeyColumn { get; set; }
        public bool Nullable { get; set; }
        public string DefaultValue { get; set; }

        public string ReferencingColumnName { get; set; }
        public string DotNetType { get; set; }
        public string DotNetPropName { get; set; }
        public string DotNetVariableName { get; set; }
    }
}
