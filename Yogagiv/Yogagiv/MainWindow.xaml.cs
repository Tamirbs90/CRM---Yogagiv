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
        public ApplicationLogic AppLogic { get; set; }

        public MainWindow()
        {
            AppLogic = new ApplicationLogic();
            InitializeComponent();
            login();
        }

        private void login()
        {
            AppLogic.Connection = new SqlConnection();
            AppLogic.Connection.ConnectionString = "Data Source=TAMIR; Initial Catalog = Yogagiv; Integrated Security = True";
            string query = "select * from Month";
            updateDataGrid(query);
        }

        private void updateDataGrid(string query)
        {
            SqlCommand command = new SqlCommand(query, AppLogic.Connection);
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
                AppLogic.Connection.Open();
                string selectedMonth =addMonthComboBox.SelectedItem as string;
                AppLogic.UpdateData(updateNameTextBox.Text, addMonthComboBox.Text,
                    dateTextBox.Text, paidTextBox.Text);
                string query = string.Format("select* from Month where Name='{0}' and Month='{1}'",
                    updateNameTextBox.Text, addMonthComboBox.Text);
                updateDataGrid(query); // update the grid
                AppLogic.Connection.Close();
                dateTextBox.Clear();
                paidTextBox.Clear();
            }
        }

        private void addMonthComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            fillComboBox(sender);
        }
    }
}
