using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
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
using oop11.MVVM.Model;
using System.Security.Policy;
using System.Text.RegularExpressions;
using oop4_5.Core;
using System.Collections;

namespace oop11.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для RecordsView.xaml
    /// </summary>
    public partial class RecordsView : UserControl
    {
        private static RecordsView recordsView;
        string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
        int recordsCount = 0;

        public RecordsView()
        {
            InitializeComponent();
            recordsView = this;
            ShowRecordsData("SELECT * FROM RECORDS");

        }

        private void ShowRecordsData(string query)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                RecordsDataGrid.ItemsSource = dt.DefaultView;
            }
        }


        private void skipTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox skip = (TextBox)sender;
            for (int i = 0; i < skip.Text.Length; i++)
            {
                if (!Regex.IsMatch(skip.Text, @"^(2| )$"))
                {
                    skip.Text = skip.Text.Remove(i, 1);
                    skip.SelectionStart = skip.Text.Length;
                    System.Windows.MessageBox.Show("2 or ' '", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void scoreTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox score = (TextBox)sender;
            for (int i = 0; i < score.Text.Length; i++)
            {
                if (!Regex.IsMatch(score.Text, @"^(10|[0-9]| )$"))
                {
                    score.Text = score.Text.Remove(i, 1);
                    score.SelectionStart = score.Text.Length;
                    System.Windows.MessageBox.Show("0-10", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }



        private void addRecord_Click(object sender, RoutedEventArgs e)
        {
            string studentName = "";

            string studentId = textBox_stId.Text;

            if (!string.IsNullOrEmpty(studentId))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                        string query = "SELECT FullName FROM STUDENT WHERE Id = @Id";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", studentId);
                            var result = command.ExecuteScalar();
                            studentName = result?.ToString();
                        }
                }
            }
            else
            {
                MessageBox.Show("StudentID is incorrect. Repeat input");
                return;
            }

            string datestring = System.DateTime.Now.ToString("dd-MM-yyyy"); ;

            string scorestring = scoreTextBox.Text;
            string skipstring = skipTextBox.Text;

            if (skipstring == "2")
            {
                scorestring = string.Empty;
                MessageBox.Show("Score = ' ' ");
            }

            if (!string.IsNullOrEmpty(studentName))
            {
                bool isRecordAdded = AddRecord(new Record(recordsCount++, studentName, datestring, scorestring, skipstring));
                if (isRecordAdded)
                {
                    MessageBox.Show($"Your record added! \nName of the student: {studentName}\nDate: {datestring}\nScore: {scorestring}\nSkip: {skipstring}");
                }
                ShowRecordsData("SELECT * FROM RECORDS");
            }
            else
            {
                MessageBox.Show("StudentID is incorrect. Repeat input");
                return;
            }
        }

        private bool AddRecord(Record record)
        {
            List<Record> recordsList = GetRecordList();
            bool containsSameRecord = recordsList.Any(t =>
            t.StudentName == record.StudentName &&
            t.Date == record.Date);

            if (containsSameRecord)
            {
                MessageBox.Show("This student has already been marked.");
                return false;
            }

            while (recordsList.Any(r => r.Id == record.Id))
            {
                record.Id++;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("AddRecord", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@RecordID", record.Id));
                command.Parameters.Add(new SqlParameter("@StudentName", record.StudentName));
                command.Parameters.Add(new SqlParameter("@Date", record.Date));
                command.Parameters.Add(new SqlParameter("@Score", record.Score));
                command.Parameters.Add(new SqlParameter("@Skip", record.Skip));

                connection.Open();
                command.ExecuteNonQuery();
            }
            ShowRecordsData("SELECT * FROM RECORDS");

            return true;
        }

        private List<Record> GetRecordList()
        {
            string query = "SELECT * FROM RECORDS";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                List<Record> records = new List<Record>();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string studentname = reader.GetString(1);
                    string date = reader.GetString(2);
                    string score = reader.GetString(3);
                    string skip = reader.GetString(4);

                    records.Add(new Record(id, studentname, date, score, skip));
                }

                return records;
            }
        }

        private void deleteRecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecordsDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int recordId = (int)selectedRow["Id"];

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand("DELETERECORD", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@RecordID", recordId));

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                ShowRecordsData("SELECT * FROM RECORDS");
            }
            else
            {
                MessageBox.Show("Please select a record to delete.");
            }
        }

        private void textBox_stId_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]");
            return !regex.IsMatch(text);
        }
    }
}
