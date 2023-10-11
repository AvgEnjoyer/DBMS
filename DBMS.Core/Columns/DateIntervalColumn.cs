namespace DBMS.Core.Columns
{
    public class DateIntervalColumn : Column
    {
        public override string Type { get; set; } = "DATE INTERVAL";

        public DateIntervalColumn(string name) : base(name) { }

        public override bool Validate(string value)
        {
            string[] buf = value.Replace(" ", "").Split(',');

            return (buf.Length == 2 && DateTime.TryParse(buf[0], out DateTime a) &&
              DateTime.TryParse(buf[1], out DateTime b) && a<b) || value == "";
        }
    }


}