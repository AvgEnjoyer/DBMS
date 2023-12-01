using DBMS.Core;

namespace GraphQL_API.Data
{
    public class DataService : IDataService
    {
        private DatabaseManager _dbManager = DatabaseManager.Instance;

        public DataService()
        {
            
        }
    }
}
