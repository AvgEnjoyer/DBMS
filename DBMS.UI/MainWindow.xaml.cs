using DBMS.Core;
using DBMS.Core.Columns;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Table = DBMS.Core.Table;

namespace DBMS.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        DatabaseManager dbManager = DatabaseManager.Instance;
        
        public List<string> AvailableDBs
        {
            get
            {
                return Databases.Select(x => x.Name).ToList();
            }
        }
        public List<string> AvailableTables 
        { 
            get {
                if(CurrentDatabase==null)
                    return new List<string>();
                return CurrentDatabase.Tables.Select(x => x.Name).ToList();
            } 
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private List<Database> _databases;
        public List<Database> Databases
        {
            get { return _databases; }
            set
            {
                _databases = value;
                RaisePropertyChanged("Databases");
                RaisePropertyChanged("AvailableDBs"); 
            }
        }
        public Table CurrentTable { get; set; }

        private Database _currentDatabase;
        public Database CurrentDatabase
        {
            get { return _currentDatabase; }
            set
            {
                if (_currentDatabase != value)
                {
                    _currentDatabase = value;
                    RaisePropertyChanged("CurrentDatabase"); // Notify that CurrentDatabase has changed
                    RaisePropertyChanged("AvailableTables"); // Notify that AvailableTables should be re-evaluated
                }
            }
        }
        


        private void RenderTable()
        {
            ObservableCollection<object[]> _tableRows = new();
            tablesPanel.Children.Clear();

            var tableGrid = new DataGrid();
            tableGrid.AutoGenerateColumns = false;
            for (int i = 0; i < CurrentTable.Columns.Count(); i++)
            {
                DataGridTextColumn column = new()
                {
                    Header = CurrentTable.Columns[i].Name,
                    Width = new DataGridLength(150),
                    Binding = new Binding("[" + i + "]")
                };
                tableGrid.Columns.Add(column);
            }
            foreach (var row in CurrentTable.Rows)
            {
                _tableRows.Add(row.Values.Select(x => x).ToArray());
            }
            tableGrid.ItemsSource = _tableRows;

            tablesPanel.Children.Add(tableGrid);
        }


        public MainWindow()
        {
            InitializeComponent();

            PathInputWindow inputWindow = new PathInputWindow();
            inputWindow.ShowDialog();

            // Check if the user entered a path
            if (!string.IsNullOrEmpty(inputWindow.EnteredPath))
            {
                dbManager.filePath = inputWindow.EnteredPath;
            }
            Databases = dbManager.GetAllDatabases();
            DataContext = this;
        }
        private void ReloadDB()
        {
            tablesPanel.Children.Clear();
            Databases = dbManager.GetAllDatabases();

            if (CurrentDatabase != null)
            {
                CurrentDatabase = Databases.FirstOrDefault(x => x.Name == CurrentDatabase.Name);
                if (CurrentDatabase == null)
                {
                    CurrentTable = null;

                    selectedDB.SelectedIndex = -1;
                }
            }
            if (CurrentTable != null)
            {
                CurrentTable = CurrentDatabase.Tables.FirstOrDefault(x => x.Name == CurrentTable.Name);
                
            }
            if (CurrentTable == null)
            {
                selectedTable.SelectedIndex = -1;
            }
            else
            {
                RenderTable();
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (columnType.SelectedItem == null)
            {
                addColumnButton.IsEnabled = false;
            }
            else
            {
                addColumnButton.IsEnabled = true;
            }
        }
        private void AddTable_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProvideParameters((parameters) =>
            {
                dbManager.AddTableToDatabaseByName(CurrentDatabase.Name,parameters[0]);
                ReloadDB();
                
            }, new List<string> {"Table Name" }, "Table added successfully.");

            dialog.ShowDialog();
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            var columns = new List<string>();
            
            foreach (var column in CurrentTable.Columns)
            {
                columns.Add(column.Name);
            }
            var dialog = new ProvideParameters((parameters) =>
            {
                dbManager.AddRowToTable(CurrentDatabase.Name, CurrentTable.Name, parameters);
                ReloadDB();
                
            }, columns, "Row added successfully.");

            dialog.ShowDialog();
        }

        private void AddColumn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTable==null)
            {
                MessageBox.Show("Please select a table");
                return;
            }
          
            var dialog = new ProvideParameters((parameters) =>
            {
                dbManager.AddColumnToTable(CurrentDatabase.Name, CurrentTable.Name,parameters[0], columnType.Text);
                ReloadDB();
                
            }, new List<string> {"Column Name"}, "Column Added successfully.");

            dialog.ShowDialog();
        }

        


        private void DeleteDb_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProvideParameters((parameters) =>
            {
                dbManager.DeleteDatabaseByName(CurrentDatabase.Name);
                ReloadDB();

            }, new List<string> {}, "Database deleted successfully.");

            dialog.ShowDialog();
        }
    
       
        private void AddDb_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProvideParameters((parameters) =>
            {
                dbManager.CreateDatabase(parameters[0]);
                ReloadDB();

            }, new List<string> { "Database Name" }, "Database added successfully.");

            dialog.ShowDialog();
        }

        

        private void DeleteColumn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteTable_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProvideParameters((parameters) =>
            {
                dbManager.DeleteTableInDatabaseByName(CurrentDatabase.Name, CurrentTable.Name);
                ReloadDB();

            }, new List<string> { }, "Table deleted successfully.");

            dialog.ShowDialog();
        }

        private void DBComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tablesPanel.Children.Clear();
            if (selectedDB.SelectedIndex != -1)
            {
                CurrentDatabase = Databases.FirstOrDefault(x => x.Name == selectedDB.SelectedItem.ToString());
                if (CurrentDatabase != null)
                {
                    databaseInteractions.Visibility = Visibility.Visible;
                    selectedTable.SelectedIndex = -1;
                }
            }
            else{
                databaseInteractions.Visibility = Visibility.Hidden;
                
            }
        }

        private void TableComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedTable.SelectedIndex != -1)
            {
                if (CurrentDatabase != null)
                    CurrentTable = CurrentDatabase.Tables.FirstOrDefault(x => x.Name == selectedTable.SelectedItem.ToString());
                if (CurrentTable != null)
                {
                    tableInteractions.Visibility = Visibility.Visible;
                    RenderTable();
                }
            }
            else
            {
                tableInteractions.Visibility = Visibility.Hidden;
            }
        }






        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // Check if the property type is an array and exclude it
            if (e.PropertyType.IsArray)
            {
                e.Cancel = true;
            }
        }

        private void PRODUCT_TABLES_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProvideParameters((parameters) =>
            {
                dbManager.AddTableAsCartesianProduct(CurrentDatabase.Name, (string)prodTable1.SelectedItem, (string)prodTable2.SelectedItem);
                ReloadDB();
                prodTable1.SelectedIndex = -1;
                prodTable2.SelectedIndex = -1;

            }, new List<string> { }, "Tables were producted successfuly!");

            dialog.ShowDialog();
        }

        private void PRODUCT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (prodTable1.SelectedIndex != -1 && prodTable2.SelectedIndex != -1)
                PRODUCT_TABLES.IsEnabled = true;
            else
                PRODUCT_TABLES.IsEnabled = false;
            
        }
    }
}
