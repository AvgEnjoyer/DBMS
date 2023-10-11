namespace DBMS.Core.Columns
{
    public class DateColumn : Column
    {
        public override string Type { get; set; } = "DATE";

        public DateColumn(string name) : base(name) { }

        public bool Validate(string value) => DateTime.TryParse(value, out _) || value == "";
    }


}