using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Windows;

namespace DBMS.UI
{
    public class Parameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    
    public partial class ProvideParameters : Window
    {

        public List<Parameter> Parameters { get; private set; }
        public string SuccessMessage{ get; private set; }

        public Action<List<string>> CallbackFunction { get; set; }

        public ProvideParameters(Action<List<string>> callbackFunction,
            List<string> parameterNames,
            string successMessage)
        {
            InitializeComponent();

            // Initialize the Parameters list based on the provided parameter names
            Parameters = new List<Parameter>();
            foreach (string paramName in parameterNames)
            {
                Parameters.Add(new Parameter { Name = paramName, Value = "" });
            }

            // Set the ListView's data context to the Parameters list
            ParametersListView.ItemsSource = Parameters;

            // Store the callback function to be called on submit
            CallbackFunction = callbackFunction;
            SuccessMessage= successMessage;
        }


        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CallbackFunction.Invoke(Parameters.ConvertAll(p => p.Value));
                // Close the window
                DialogResult = true;
                MessageBox.Show(SuccessMessage);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
