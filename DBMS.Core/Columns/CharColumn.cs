namespace DBMS.Core.Columns
{
    public class CharColumn : Column
    {
        public override string Type { get; set; } = "CHAR";

        public CharColumn(string name) : base(name) { }

        public override bool Validate(string value) => char.TryParse(value, out _) || value=="";
    }

}