namespace DBMS.Core
{
    [Serializable]
    public class Database
    {
        public string Name { get; set; }
        public List<Table> Tables { get; set; } = new List<Table>();

        public Database(string name)
        {
            Name = name;
        }
    }

}