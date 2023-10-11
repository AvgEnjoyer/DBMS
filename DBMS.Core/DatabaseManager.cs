using DBMS.Core;
using DBMS.Core.Columns;
using System.Text.Json;

public class DatabaseManager
{
    private static DatabaseManager _instance;
    // Specify the path to the JSON file
    public string filePath = "mem6.json";

    public static DatabaseManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DatabaseManager();
            }

            return _instance;
        }
    }
    
    private DatabaseManager() {
        
        
    }

    public void CreateDatabase(string name)
    {
        

        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the existing JSON data from the file
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON data into a list of Database objects
            List<Database> databases = JsonSerializer.Deserialize<List<Database>>(json);

            // Check if a database with the given name already exists
            if (databases.Any(db => db.Name == name))
            {
                Console.WriteLine($"Database with the name '{name}' already exists. No need to create a new one.");
                return;
            }
        }
        else
        {
            // If the file doesn't exist, create an empty list of databases
            File.WriteAllText(filePath, "[]");
        }

        // Create an instance of your Database class
        var database = new Database(name);

        // Serialize the new Database instance to JSON
        string newJson = JsonSerializer.Serialize(database);

        // Read the existing JSON data again
        string existingJson = File.ReadAllText(filePath);

        // Deserialize the existing JSON data into a list of Database objects
        List<Database> existingDatabases = JsonSerializer.Deserialize<List<Database>>(existingJson);

        // Add the new Database to the list
        existingDatabases.Add(database);

        // Serialize the updated list of Database objects back to JSON
        string updatedJson = JsonSerializer.Serialize(existingDatabases);

        // Write the updated JSON data to the file
        File.WriteAllText(filePath, updatedJson);

        Console.WriteLine($"Database '{name}' serialized and saved to mem.json");
    }
    public Database LoadDatabaseByName(string name)
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the existing JSON data from the file
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON data into a list of Database objects
            List<Database> databases = JsonSerializer.Deserialize<List<Database>>(json);

            // Find the database with the specified name
            Database database = databases.FirstOrDefault(db => db.Name == name);

            return database;
        }

        // If the file doesn't exist or the database is not found, return null
        return null;
    }
    public bool DeleteDatabaseByName(string name)
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the existing JSON data from the file
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON data into a list of Database objects
            List<Database> databases = JsonSerializer.Deserialize<List<Database>>(json);

            // Find the index of the database with the specified name
            int index = databases.FindIndex(db => db.Name == name);

            if (index >= 0)
            {
                // Remove the database from the list
                databases.RemoveAt(index);

                // Serialize the updated list of Database objects back to JSON
                string updatedJson = JsonSerializer.Serialize(databases);

                // Write the updated JSON data to the file
                File.WriteAllText(filePath, updatedJson);

                return true; // Database deleted successfully
            }
        }

        return false; // Database not found or file doesn't exist
    }

    private Column ResolveColumn(Column col)
    {
        switch (col.Type)
        {
            case "CHAR":
                return new CharColumn(col.Name);
            case "DATE":
                return new DateColumn(col.Name);
            case "DATE_INTERVAL":
                return new DateIntervalColumn(col.Name);
            case "INT":
                return new IntColumn(col.Name);
            case "REAL":
                return new RealColumn(col.Name);
            case "STRING":
                return new StringColumn(col.Name);
            default:
                // Handle an unknown or unsupported type here, or return null as needed
                return col ;
        }
    }

    public Row ToRow(List<string> rowData, List<Column> columns)
    {
        if (rowData == null || columns == null || rowData.Count != columns.Count)
        {
            throw new ArgumentException("Invalid input data or column count mismatch.");
        }

        var row = new Row();

        for (int i = 0; i < rowData.Count; i++)
        {
            Column resolvedColumn = ResolveColumn(columns[i]);

            if (resolvedColumn.Validate(rowData[i]))
                row.Values.Add(rowData[i]);
            else throw new ArgumentException($"Types mismatch");
            
        }        
        return row;
    }

    
    public List<Database> GetAllDatabases()
    {
        if (File.Exists(filePath))
        {
            // Read the existing JSON data from the file
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON data into a list of Database objects
            List<Database> databases = JsonSerializer.Deserialize<List<Database>>(json);

            return databases;
        }

        return new List<Database>(); // Return an empty list if the file doesn't exist
    }
    public bool DeleteTableInDatabaseByName(string databaseName, string tableName)
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the existing JSON data from the file
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON data into a list of Database objects
            List<Database> databases = JsonSerializer.Deserialize<List<Database>>(json);

            // Find the database with the specified name
            Database database = databases.FirstOrDefault(db => db.Name == databaseName);

            if (database != null)
            {
                // Find the index of the table within the database
                int index = database.Tables.FindIndex(table => table.Name == tableName);

                if (index >= 0)
                {
                    // Remove the table from the database
                    database.Tables.RemoveAt(index);

                    // Serialize the updated list of Database objects back to JSON
                    string updatedJson = JsonSerializer.Serialize(databases);

                    // Write the updated JSON data to the file
                    File.WriteAllText(filePath, updatedJson);

                    return true; // Table deleted successfully
                }
            }
        }

        return false; // Database or table not found, or file doesn't exist
    }
    public bool AddTableToDatabaseByName(string databaseName, string tableName)
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the existing JSON data from the file
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON data into a list of Database objects
            List<Database> databases = JsonSerializer.Deserialize<List<Database>>(json);

            // Find the database with the specified name
            Database database = databases.FirstOrDefault(db => db.Name == databaseName);

            if (database != null)
            {
                // Check if a table with the same name already exists
                if (database.Tables.Any(table => table.Name == tableName))
                {
                    // Table with the same name already exists in the database
                    return false;
                }

                // Add the new table to the database
                database.Tables.Add(new Table(tableName));

                // Serialize the updated list of Database objects back to JSON
                string updatedJson = JsonSerializer.Serialize(databases);

                // Write the updated JSON data to the file
                File.WriteAllText(filePath, updatedJson);

                return true; // Table added successfully
            }
        }

        return false; // Database not found or file doesn't exist
    }

    public bool AddColumnToTable(string databaseName, string tableName, string columnName, string columnType)
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the existing JSON data from the file
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON data into a list of Database objects
            List<Database> databases = JsonSerializer.Deserialize<List<Database>>(json);

            // Find the database with the specified name
            Database database = databases.FirstOrDefault(db => db.Name == databaseName);

            if (database != null)
            {
                // Find the table within the database
                Table table = database.Tables.FirstOrDefault(t => t.Name == tableName);

                if (table != null)
                {
                    // Check if the column name already exists in the table
                    if (table.Columns.Any(col => col.Name == columnName))
                    {
                        return false; // Column with the same name already exists
                    }

                    // Add the new column to the table
                    Column col = new Column(columnName);
                    col.Type = columnType;//костиль, але його ніхто не побачить, бо цей код не прочитають, мені 

                    table.Columns.Add(col);
                    
                    // Serialize the updated list of Database objects back to JSON
                    string updatedJson = JsonSerializer.Serialize(databases);

                    // Write the updated JSON data to the file
                    File.WriteAllText(filePath, updatedJson);

                    return true; // Column added successfully
                }
            }
        }

        return false; // Database or table not found, or file doesn't exist
    }
    public bool AddRowToTable(string databaseName, string tableName, List<string> rowData)
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the existing JSON data from the file
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON data into a list of Database objects
            List<Database> databases = JsonSerializer.Deserialize<List<Database>>(json);

            // Find the index of the specified database within the list
            int databaseIndex = databases.FindIndex(db => db.Name == databaseName);

            if (databaseIndex >= 0)
            {
                // Find the table with the specified name within the database
                Table table = databases[databaseIndex].Tables.FirstOrDefault(t => t.Name == tableName);

                if (table != null)
                {
                    // Add the new row to the table
                    table.Rows.Add(ToRow(rowData, table.Columns));

                    // Serialize the updated list of Database objects back to JSON
                    string updatedJson = JsonSerializer.Serialize(databases);

                    // Write the updated JSON data to the file
                    File.WriteAllText(filePath, updatedJson);

                    return true; // Row added successfully
                }
            }
        }

        return false; // Database, table, or file doesn't exist
    }
    public bool RemoveColumnFromTable(string databaseName, string tableName, string columnName)
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the existing JSON data from the file
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON data into a list of Database objects
            List<Database> databases = JsonSerializer.Deserialize<List<Database>>(json);

            // Find the index of the specified database within the list
            int databaseIndex = databases.FindIndex(db => db.Name == databaseName);

            if (databaseIndex >= 0)
            {
                // Find the table with the specified name within the database
                Table table = databases[databaseIndex].Tables.FirstOrDefault(t => t.Name == tableName);

                if (table != null)
                {
                    // Find the index of the column to remove
                    int columnIndex = table.Columns.FindIndex(col => col.Name == columnName);

                    if (columnIndex >= 0)
                    {
                        // Remove the column from the table
                        table.Columns.RemoveAt(columnIndex);

                        // Serialize the updated list of Database objects back to JSON
                        string updatedJson = JsonSerializer.Serialize(databases);

                        // Write the updated JSON data to the file
                        File.WriteAllText(filePath, updatedJson);

                        return true; // Column removed successfully
                    }
                }
            }
        }

        return false; // Database, table, column, or file doesn't exist
    }
    public bool RemoveRowFromTable(string databaseName, string tableName, int rowIndex)
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the existing JSON data from the file
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON data into a list of Database objects
            List<Database> databases = JsonSerializer.Deserialize<List<Database>>(json);

            // Find the index of the specified database within the list
            int databaseIndex = databases.FindIndex(db => db.Name == databaseName);

            if (databaseIndex >= 0)
            {
                // Find the table with the specified name within the database
                Table table = databases[databaseIndex].Tables.FirstOrDefault(t => t.Name == tableName);

                if (table != null && rowIndex >= 0 && rowIndex < table.Rows.Count)
                {
                    // Remove the row from the table
                    table.Rows.RemoveAt(rowIndex);

                    // Serialize the updated list of Database objects back to JSON
                    string updatedJson = JsonSerializer.Serialize(databases);

                    // Write the updated JSON data to the file
                    File.WriteAllText(filePath, updatedJson);

                    return true; // Row removed successfully
                }
            }
        }

        return false; // Database, table, row, or file doesn't exist, or rowIndex is out of bounds
    }

    public bool AddTableAsCartesianProduct(string databaseName, string table1,string table2)
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the existing JSON data from the file
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON data into a list of Database objects
            List<Database> databases = JsonSerializer.Deserialize<List<Database>>(json);

            // Find the database with the specified name
            Database database = databases.FirstOrDefault(db => db.Name == databaseName);

            if (database != null)
            {
                // Find the first table within the database
                Table table1_entity = database.Tables.FirstOrDefault(t => t.Name == table1);

                if (table1_entity == null) return false;
                
                Table table2_entity= database.Tables.FirstOrDefault(t => t.Name == table2);

                if (table2_entity == null) return false;

                // Create a new table to store the Cartesian product
                Table cartesianProduct = new Table(table1_entity.Name + " x " + table2_entity.Name);

                // Add the columns from the first and second table
                cartesianProduct.Columns.AddRange(table1_entity.Columns);
                cartesianProduct.Columns.AddRange(table2_entity.Columns);

                // Add the rows from the first and second table
                foreach (Row row1 in table1_entity.Rows)
                {
                    foreach (Row row2 in table2_entity.Rows)
                    {
                        cartesianProduct.Rows.Add(ToRow(row1.Values.Concat(row2.Values).ToList(), cartesianProduct.Columns));
                    }
                }
                database.Tables.Add(cartesianProduct);


                // Serialize the updated list of Database objects back to JSON
                string updatedJson = JsonSerializer.Serialize(databases);

                // Write the updated JSON data to the file
                File.WriteAllText(filePath, updatedJson);

                return true; // Table added successfully
            }
        }

        return false; // Database not found or file doesn't exist
    }


}
