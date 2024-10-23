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
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

namespace oop11.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для StudentTwo.xaml
    /// </summary>
    public partial class StudentTwo : UserControl
    {
        string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
        int studentsCount = 0;

        public StudentTwo()
        {
            InitializeComponent();
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

        private bool AddStudent(Student student)
        {

            List<Student> studentsList = GetStudentList();

            bool containsSameStudent = studentsList.Any(t => t.FullName.Equals(student.FullName, StringComparison.OrdinalIgnoreCase));

            if (containsSameStudent)
            {
                MessageBox.Show("This student already exists.");
                return false;
            }

            while (studentsList.Any(r => r.Id == student.Id))
            {
                student.Id++;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("AddStudent", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@ID", student.Id));
                command.Parameters.Add(new SqlParameter("@FullName", student.FullName));
                command.Parameters.Add(new SqlParameter("@Speciality", student.Speciality));
                command.Parameters.Add(new SqlParameter("@sGroup", student.sGroup));
                command.Parameters.Add(new SqlParameter("@Subgroup", student.Subgroup));
                command.Parameters.Add(new SqlParameter("@Course", student.Course));


                connection.Open();
                command.ExecuteNonQuery();
            }

            return true;
        }

        private void addStudent_Click(object sender, RoutedEventArgs e)
        {
            if (fullnameTextBox.Text != "" && specialityComboBox.Text != "" && groupTextBox.Text != "" && subgroupTextBox.Text != "" && courseTextBox.Text != "")
            {

                string fullname = fullnameTextBox.Text;
            string speciality = specialityComboBox.Text;
            int group = int.Parse(groupTextBox.Text);
            int subgroup = int.Parse(subgroupTextBox.Text);
            int course = int.Parse(courseTextBox.Text);

                if (!IsValidFormat(fullname))
                    MessageBox.Show("The format is not correct. -- \"Фамилия И.И.\"");
                else
                {

                    bool isStudentAdded = AddStudent(new Student(studentsCount++, fullname, speciality, group, subgroup, course));
                    if (isStudentAdded)
                    {
                        MessageBox.Show($"Student added! \nName of the student: {fullname}\nspeciality: {speciality}\ngroup: {group}\nsubgroup: {subgroup}\ncourse: {course}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Не все поля заполнены");
            }
        }

        private void deleteStudent_Click(object sender, RoutedEventArgs e)
        {
            if (studentIDTextBox.Text != "")
            {
                int studentid = int.Parse(studentIDTextBox.Text);
                if (studentid > 0 && studentIDTextBox.Text != "")
                {

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "SELECT COUNT(1) FROM STUDENT WHERE Id = @Id";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", studentid);
                            int count = (int)command.ExecuteScalar();

                            if (count < 1)
                            {
                                MessageBox.Show("ID not found.");

                            }
                            else
                            {
                                using (SqlCommand command2 = new SqlCommand("DelStudent", connection))
                                {
                                    //SqlCommand command2 = new SqlCommand("DelStudent", connection);
                                    command2.CommandType = CommandType.StoredProcedure;
                                    command2.Parameters.Add(new SqlParameter("@ID", studentid));
                                    command2.ExecuteNonQuery();
                                }

                                MessageBox.Show("Student removed.");
                            }
                        }
                    }
                }

                else
                {
                    MessageBox.Show("Please select a student to delete.");
                }
            }
            else
            {
                MessageBox.Show("Please select a student to delete.");
            }
        }


        private void updateStudent_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(studentIDTextBox.Text))
            {
                int studentid = int.Parse(studentIDTextBox.Text);
                if (studentid >= 0 && studentIDTextBox.Text != "")
                {

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "SELECT COUNT(1) FROM STUDENT WHERE Id = @Id";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", studentid);
                            int count = (int)command.ExecuteScalar();

                            if (count < 1)
                            {
                                MessageBox.Show("ID not found.");

                            }
                            else
                            {
                                if (fullnameTextBox.Text != "" && specialityComboBox.Text != "" && groupTextBox.Text != "" && subgroupTextBox.Text != "" && courseTextBox.Text != "")
                                {
                                    string fullname = fullnameTextBox.Text;
                                string speciality = specialityComboBox.Text;
                                int group = int.Parse(groupTextBox.Text);
                                int subgroup = int.Parse(subgroupTextBox.Text);
                                int course = int.Parse(courseTextBox.Text);


                                    if (!IsValidFormat(fullname))
                                        MessageBox.Show("The format is not correct. -- \"Фамилия И.И.\"");
                                    else
                                    {

                                        using (SqlCommand command2 = new SqlCommand("UpdateStudent", connection))
                                        {

                                            command2.CommandType = CommandType.StoredProcedure;

                                            command2.Parameters.Add(new SqlParameter("@ID", studentid));
                                            command2.Parameters.Add(new SqlParameter("@FullName", fullname));
                                            command2.Parameters.Add(new SqlParameter("@Speciality", speciality));
                                            command2.Parameters.Add(new SqlParameter("@sGroup", group));
                                            command2.Parameters.Add(new SqlParameter("@Subgroup", subgroup));
                                            command2.Parameters.Add(new SqlParameter("@Course", course));
                                            command2.ExecuteNonQuery();
                                        }
                                        MessageBox.Show($"Student changed! \nName of the student: {fullname}\nspeciality: {speciality}\ngroup: {group}\nsubgroup: {subgroup}\ncourse: {course}");
                                    }
                                 
                                }
                                else
                                {
                                    MessageBox.Show("Не все поля заполнены.");
                                }
                            }
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Please select a student to delete.");
                }
            }
            else
            {
                MessageBox.Show("Please select a student to delete.");
            }
        }

        private void courseTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox course = (TextBox)sender;
            for (int i = 0; i < course.Text.Length; i++)
            {
                if (!Regex.IsMatch(course.Text, @"^(10|[1-4]| )$"))
                {
                    course.Text = course.Text.Remove(i, 1);
                    //course.SelectionStart = course.Text.Length;
                    System.Windows.MessageBox.Show("1-4", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void groupTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox group = (TextBox)sender;
            for (int i = 0; i < group.Text.Length; i++)
            {
                if (!Regex.IsMatch(group.Text, @"^(10|[1-9]| )$"))
                {
                    group.Text = group.Text.Remove(i, 1);
                    //group.SelectionStart = group.Text.Length;
                    System.Windows.MessageBox.Show("1-10", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void subgroupTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox subgroup = (TextBox)sender;
            for (int i = 0; i < subgroup.Text.Length; i++)
            {
                if (!Regex.IsMatch(subgroup.Text, @"^(2|[1-2]| )$"))
                {
                    subgroup.Text = subgroup.Text.Remove(i, 1);
                    //subgroup.SelectionStart = subgroup.Text.Length;
                    System.Windows.MessageBox.Show("1-2", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void studentIDTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]");
            return !regex.IsMatch(text);
        }



        private bool IsValidFormat(string input)
        {
            // Регулярное выражение для формата "Фамилия И.И."
            string pattern = @"^(?:[А-ЯЁ][а-яё]+(-[А-ЯЁ][а-яё]+)?)\s[А-ЯЁ]\.[А-ЯЁ]\.$";
            return Regex.IsMatch(input, pattern);
        }

        private void fullnameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
/*            string input = fullnameTextBox.Text;

            if (!IsValidFormat(input))
                MessageBox.Show("The format is not correct");*/
        }
    }
}
