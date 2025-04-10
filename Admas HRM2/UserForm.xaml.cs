using MaterialDesignThemes.Wpf;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Admas_HRM2
{
    public partial class UserForm : UserControl
    {
        public UserForm()
        {
            InitializeComponent();
        }

        // Insert User into the database
        private void UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Retrieve input values directly from textboxes
                string firstName = txtFirstName.Text.Trim();
                string lastName = txtLastName.Text.Trim();
                string userName = txtUserName.Text.Trim();
                string password = txtPassword.Text.Trim(); // Use PasswordBox
                string departmentID = txtDepartmentID.Text.Trim();
                bool isActive = chkIsActive.IsChecked == true;

                string role = cmbRole.SelectedItem is ComboBoxItem selectedRole ? selectedRole.Content.ToString() : "";

                // Validate input
                if (string.IsNullOrWhiteSpace(firstName) ||
                    string.IsNullOrWhiteSpace(lastName) ||
                    string.IsNullOrWhiteSpace(userName) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(departmentID) ||
                    string.IsNullOrWhiteSpace(role))
                {
                    MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Call method to insert into the database
                bool insertSuccess = UserDAL2.InsertUser(firstName, lastName, userName, password, role, departmentID, isActive);

                if (insertSuccess)
                {
                    MessageBox.Show("User added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to add user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Close the form
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Users editEmployee = new Users();
            HR_Admin mainWindow = Application.Current.MainWindow as HR_Admin;

            if (mainWindow != null)
            {
                mainWindow.AnimateContentChange(editEmployee);
            }
        }
    }

    public static class UserDAL2
    {

        public static bool InsertUser(string firstName, string lastName, string userName, string password, string role, string departmentID, bool isActive)
        {
            string query = @"
                INSERT INTO Users (UserFirst, UserLast, UserName, PasswordHash, Role, DepartmentID, IsActive)
                VALUES (@UserFirst, @UserLast, @UserName, @PasswordHash, @Role, @DepartmentID, @IsActive);";

            using (SqlConnection conn = new SqlConnection(Connection.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add values directly from parameters
                    cmd.Parameters.AddWithValue("@UserFirst", firstName);
                    cmd.Parameters.AddWithValue("@UserLast", lastName);
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@PasswordHash", password); // You should hash the password before storing it
                    cmd.Parameters.AddWithValue("@Role", role);
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
