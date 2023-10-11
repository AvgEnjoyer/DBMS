using System.Runtime.Serialization;

namespace DBMS.Core.Columns
{
    [Serializable]
    public class Column
    {
        public string Name { get; set; }
        public virtual string Type { get; set; }

        public Column(string name)
        {
            Name = name;
        }
        public virtual bool Validate(string value) => false;
    }


}
