namespace DBMS.Core.Columns
{
    public class RealColumn : Column
    {
        public override string Type { get; set; } = "REAL";

        public RealColumn(string name) : base(name) { }

        public override bool Validate(string value) => double.TryParse(value, out _) || value == "";
    }


}