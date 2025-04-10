using System;
using System.Data.SqlClient;
using System.Windows;

namespace Admas_HRM2
{
    public partial class login : Window
    {

        public login()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string loginType = cbLoginType.Text;
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(loginType) || loginType == "Select Login Type")
            {
                MessageBox.Show("Please select a login type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string tableName = loginType switch
            {
                "HR Manager" => "HRManager",
                "HR Admin" => "HRAdmin",
                "Department Head" => "DepartmentHead",
                _ => null
            };

            if (tableName == null)
            {
                MessageBox.Show("Invalid login type selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(Connection.connectionString))
                {
                    con.Open();
                    string query = $"SELECT UserID, UserFirst, UserLast, Username, Role, DepartmentID FROM Users WHERE Username=@Username AND PasswordHash=@Password AND Role = @Role ";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Role", tableName);


                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Retrieve user info
                                string userID = reader["UserID"].ToString();
                                string userFirset = reader["UserFirst"].ToString();
                                string userLast = reader["UserLast"].ToString();
                                string role = reader["Role"].ToString();
                                string department = reader["DepartmentID"].ToString();

                                // Store in session
                                SessionManager.UserID = userID;
                                SessionManager.Username = username;
                                SessionManager.UserRole = role;
                                SessionManager.DepartmentID = department;
                                SessionManager.UserFirst = userFirset;
                                SessionManager.UserLast = userLast;

                                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                                // Open respective window
                                OpenUserDashboard(role, department);
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenUserDashboard(string role, string department)
        {
            if (role == "HRManager")
            {
                MainWindow main = new MainWindow();
                Application.Current.MainWindow = main;
                main.Show();
            }
            else if (role == "HRAdmin")
            {
                HR_Admin HR_Admin = new HR_Admin();
                Application.Current.MainWindow = HR_Admin;
                HR_Admin.Show();
            }
            else if (role == "DepartmentHead")
            {
               HR_Department Dep = new HR_Department();
               Application.Current.MainWindow = Dep;
                Dep.Show();
            }
        }
    }

    public static class SessionManager
    {
        public static string UserID { get; set; }
        public static string Username { get; set; }
        public static string UserRole { get; set; }
        public static string DepartmentID { get; set; }
        public static string UserFirst { get; set; }
        public static string UserLast { get; set; }
        public static void ClearSession()
        {
            UserID = null;
            Username = null;
            UserRole = null;
            DepartmentID = null;
            UserFirst = null;
            UserLast = null;
        }
    }
}
