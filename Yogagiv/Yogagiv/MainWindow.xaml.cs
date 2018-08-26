using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Data;
using System.Data.SqlClient;

namespace Yogagiv
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AppLogic Logic { get; set; }

        public MainWindow()
        {
            Logic = new AppLogic();
            InitializeComponent();
            login();
        }

        private void login()
        {
            Logic.Connection = new SqlConnection();
            Logic.Connection.ConnectionString = "Data Source=TAMIR; Initial Catalog = Yogagiv; Integrated Security = True";
            string query = "select * from Month";
            updateDataGrid(query);
        }

        private void updateDataGrid(string query)
        {
                
            SqlCommand command = new SqlCommand(query, Logic.Connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);
            dataGrid.ItemsSource = table.DefaultView;
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;
            string selectedMonth = combobox.SelectedItem as string;
            string query = string.Format("select * from Month where Month='{0}'",selectedMonth);
            updateDataGrid(query);
        }

        private void monthComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            fillComboBox(sender);
        }

        private void fillComboBox(object sender)
        {
            List<string> months = new List<string>() { "ינואר", "פברואר", "מרץ", "אפריל", "מאי", "יוני",
            "יולי", "אוגוסט", "ספטמבר", "אוקטובר", "נובמבר", "דצמבר"};
            ComboBox combobox = sender as ComboBox;
            combobox.ItemsSource = months;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (nameTextBox.Text == string.Empty)
            {
                MessageBox.Show("נא להזין שם");
                return;
            }

            string query= string.Format("select* From Month where Name='{0}' and Month='{1}'",
                nameTextBox.Text, monthComboBox.SelectedItem as string);
            updateDataGrid(query);
            if (dataGrid.ItemsSource == null)
                MessageBox.Show("לא קיים מידע העונה על הפרטים הנל");
        }


        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(updateNameTextBox.Text) &&
                !string.IsNullOrEmpty(dateTextBox.Text) &&
                    !string.IsNullOrEmpty(addMonthComboBox.Text))
            {
                Logic.Connection.Open();
                string selectedMonth =addMonthComboBox.SelectedItem as string;
                if (!customerExists(updateNameTextBox.Text)) // add customer to database
                {
                    string query1 = string.Format("insert into Customers (Name, NumOfClass) values('{0}', 1)",
                        updateNameTextBox.Text);
                    new SqlCommand(query1, Logic.Connection).ExecuteNonQuery();
                }

                string query2 = string.Format("select NumOfClass from Customers where Name='{0}'",
                    updateNameTextBox.Text); // find num Of class
                SqlCommand numOfClassCommand = new SqlCommand(query2, Logic.Connection);
                var reader = numOfClassCommand.ExecuteReader();
                reader.Read();
                int numOfClass = reader.GetInt32(0);
                reader.Close();
                if(!customerMonthExists(updateNameTextBox.Text, selectedMonth)) // update month
                {
                    string query3 = string.Format("insert into Month (Name, Month) values('{0}','{1}')",
                      updateNameTextBox.Text, selectedMonth);
                    SqlCommand command = new SqlCommand(query3, Logic.Connection);
                    command.ExecuteNonQuery();
                }

                string query4; // update class
                if (!string.IsNullOrEmpty(paidTextBox.Text))
                {
                    query4 = string.Format("update Month set [{0}]='{1}', Paid={2} where Name= '{3}' and Month='{4}",
                            numOfClass, dateTextBox.Text, paidTextBox.Text, updateNameTextBox.Text, selectedMonth);
                }
                else
                {
                    query4 = string.Format("update Month set [{0}]='{1}' where Name= '{2}' and Month='{3}'",
                            numOfClass, dateTextBox.Text, updateNameTextBox.Text, selectedMonth);
                }

                new SqlCommand(query4, Logic.Connection).ExecuteNonQuery();
               string query5 = string.Format("update Customers set NumOfClass+=1 where Name='{0}'",
                   updateNameTextBox.Text); // increase num of classes
               new SqlCommand(query5, Logic.Connection).ExecuteNonQuery();
                string query6 = string.Format("select* from Month where Name='{0}' and Month='{1}'",
                    updateNameTextBox.Text, addMonthComboBox.Text);
                updateDataGrid(query6); // update the grid
                Logic.Connection.Close();
                dateTextBox.Clear();
                paidTextBox.Clear();
               
            }
        }

        private bool customerMonthExists(string i_Name, string i_Month)
        {
            string query = string.Format("select * from Month where Name='{0}' and Month='{1}'",
                    i_Name, i_Month);
            SqlCommand verification = new SqlCommand(query, Logic.Connection);
            if (verification.ExecuteScalar() != null)
                return true;
            return false;
        }

        private bool customerExists(string i_Name)
        {
            string query2 = string.Format("select * from Customers where Name= '{0}'", i_Name);
            if (new SqlCommand(query2, Logic.Connection).ExecuteScalar() != null) // uset exists
                return true;
            return false;
        }

        private void addMonthComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            fillComboBox(sender);
        }
    }
}
