namespace DBMS.Core.Columns
{
    public class StringColumn : Column
    {
        public override string Type { get; set; } = "STRING";

        public StringColumn(string name) : base(name) { }

        public override bool Validate(string value) => true;
    }


}