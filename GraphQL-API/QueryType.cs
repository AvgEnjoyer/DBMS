using DBMS.Core;

namespace GraphQL_API
{
    public class MyQueryType : ObjectType
    {
        public string GetHello() => "HelloWorld";


        public IList<Database> GetDatabases(string dbName = null)
        {
            var allDatabases = DatabaseManager.Instance.GetAllDatabases();

            if (dbName != null)
            {
                return allDatabases.Where(db => db.Name == dbName).ToList();
            }

            return allDatabases.ToList();
        }
        private IQueryable<Table> GetTables()
        {
            return new List<Table>().AsQueryable();
        }

        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor.Field("hello")
                .Resolve(ctx => GetHello());

            descriptor.Field("databases")
                .Argument("name", a => a.Type<StringType>())
                .Resolve(ctx =>
                {
                    string dbName = ctx.ArgumentValue<string>("name");
                    return GetDatabases(dbName);
                });
            descriptor.Field("tables")
                .Resolve(ctx => GetTables());
        }

        
    }

}
