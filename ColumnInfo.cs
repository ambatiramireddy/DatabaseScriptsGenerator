namespace DatabaseScriptsGenerator
{
    public class ColumnInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsKeyColumn { get; set; }
        public bool Nullable { get; set; }
        public string DefaultValue { get; set; }
    }
}
