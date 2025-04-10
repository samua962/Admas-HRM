using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Admas_HRM2
{
    public partial class Add_Department : UserControl
    {

        public Add_Department()
        {
            InitializeComponent();
            btnSubmit.Click += BtnSubmit_Click; // Attach click event handler
        }

        // Cancel and Navigate Back
        private void btnCancelEdit(object sender, RoutedEventArgs e)
        {
            Department departmentPage = new Department(SessionManager.UserRole);
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                mainWindow.AnimateContentChange(departmentPage);
            }
        }

        // Submit Button Click - Insert Department to Database
        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string departmentID = txtDepartmentID.Text.Trim();
            string departmentName = txtDepartmentName.Text.Trim();
            string departmentStatus = cbDepartmentStatus.Text.Trim();
            string firstName = txtDepartmentManagerFirst.Text.Trim();
            string lastName = txtDepartmentManagerLast.Text.Trim();
            string departmentManager = firstName + " " + lastName;
            string departmentDescription = txtDepartmentDescription.Text.Trim();


            // Input Validation
            if (string.IsNullOrWhiteSpace(departmentID) || string.IsNullOrWhiteSpace(departmentName))
            {
                MessageBox.Show("Department ID and Name are required!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO DepartmentTable (DepartmentID, DepartmentName, Manager, Description, Status, CreatedDate) 
                        VALUES (@DepartmentID, @DepartmentName, @DepartmentManager, @DepartmentDescription, @Status, GETDATE());";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                        cmd.Parameters.AddWithValue("@DepartmentName", departmentName);
                        cmd.Parameters.AddWithValue("@Status", departmentStatus);
                        cmd.Parameters.AddWithValue("@DepartmentManager", string.IsNullOrWhiteSpace(departmentManager) ? (object)DBNull.Value : departmentManager);
                        cmd.Parameters.AddWithValue("@DepartmentDescription", string.IsNullOrWhiteSpace(departmentDescription) ? (object)DBNull.Value : departmentDescription);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Department added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            AddUser(firstName, lastName, "DepartmentHead", departmentID, true);

                            // Navigate back to Department List
                            Department departmentPage = new Department(SessionManager.UserRole);
                            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

                            if (mainWindow != null)
                            {
                                mainWindow.AnimateContentChange(departmentPage);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Failed to add department. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // New Function to Add User to the Users Table
        public void AddUser(string firstName, string lastName, string role, string departmentID, bool isActive)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO Users (UserFirst, UserLast, UserName, Role, DepartmentID, IsActive)
                        VALUES (@UserFirst, @UserLast, @UserName, @Role, @DepartmentID, @IsActive);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserFirst", firstName);
                        cmd.Parameters.AddWithValue("@UserLast", lastName);
                        cmd.Parameters.AddWithValue("@UserName", firstName);
                        cmd.Parameters.AddWithValue("@Role", string.IsNullOrWhiteSpace(role) ? (object)DBNull.Value : role);
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("User added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to add user. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
