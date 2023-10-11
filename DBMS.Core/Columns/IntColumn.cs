namespace DBMS.Core.Columns
{
    public class IntColumn : Column
    {
        public override string Type { get; set; } = "INT";

        public IntColumn(string name) : base(name) { }

        public override bool Validate(string value) => int.TryParse(value, out _) || value == "";
    }


}