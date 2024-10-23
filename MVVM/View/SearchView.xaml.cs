using System;
using System.Collections.Generic;
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
using System.Configuration;
using System.Collections;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace oop11.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
        public SearchView()
        {
            InitializeComponent();
            ShowStudentsData("SELECT * FROM STUDENT");
        }

        private void ShowStudentsData(string query)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                AllStudentsItemsContol.ItemsSource = dt.DefaultView;
            }
        }

        public List<Student> GetStudentList()
        {
            string query = "SELECT * FROM STUDENT";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                List<Student> students = new List<Student>();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string fullname = reader.GetString(1);
                    string speciality = reader.GetString(2);
                    int group = reader.GetInt32(3);
                    int subgroup = reader.GetInt32(4);
                    int course = reader.GetInt32(5);

                    students.Add(new Student(id, fullname, speciality, group, subgroup, course));
                }

                return students;
            }
        }

        private List<Student> SearchStudentsByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                ShowStudentsData("SELECT * FROM STUDENT");
            }

            List<Student> allStudents = GetStudentList();
            IEnumerable<Student> filteredStudents =
                allStudents.Where(d => d.FullName.ToLower().Contains(name.ToLower()));

            return filteredStudents.ToList();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            AllStudentsItemsContol.ItemsSource = SearchStudentsByName(searchTextBox.Text);
        }

    }
}
