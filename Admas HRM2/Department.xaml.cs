using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq; // Make sure to include LINQ for .Where
using System.Windows;
using System.Windows.Controls;

namespace Admas_HRM2
{
    public partial class Department : UserControl
    {
        public List<DepartmentModel> Departments { get; set; } // Property to hold department data
        private string userRole;

        public Department(string role)
        {
            InitializeComponent();
            LoadDepartmentData();
            userRole = role;
            ConfigureColumnVisibility();
        }

        // Load all Department Data from Database
        private void LoadDepartmentData()
        {
            Departments = new List<DepartmentModel>(); // Initialize the Departments list

            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = "SELECT DepartmentID, DepartmentName, Manager, Description, CreatedDate, Status FROM DepartmentTable";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Departments.Add(new DepartmentModel
                            {
                                DepartmentID = reader.GetString(0), // Adjust if DepartmentID is not a string
                                Name = reader.GetString(1),
                                Head = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CreatedDate = reader.GetDateTime(4),
                                Status = reader.GetString(5)
                            });
                        }
                    }
                }

                // Bind the list to the DataGrid
                dgDepartments.ItemsSource = Departments;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading departments: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Add Department Button Click Handler
        private void AddDepartment_Click(object sender, RoutedEventArgs e)
        {
            Add_Department addDepartment = new Add_Department();
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                mainWindow.AnimateContentChange(addDepartment);
            }
        }

        // Search Button Click Handler
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearchDepartment.Text.ToLower();

            if (Departments != null) // Ensure Departments is populated
            {
                var filteredDepartments = Departments.Where(department =>
                    (department.Name != null && department.Name.ToLower().Contains(searchText)) ||
                    (department.Head != null && department.Head.ToLower().Contains(searchText)))
                    .ToList();

                dgDepartments.ItemsSource = filteredDepartments; // Update DataGrid with filtered results
            }
        }

        // SelectionChanged Event Handler
        private void dgDepartments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle department selection changes here if needed
        }

        // Edit Button Click Handler
        private void EditDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is DepartmentModel selectedDepartment)
            {
                Edit_Department editDepartment = new Edit_Department(selectedDepartment); // Pass selected department
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

                if (mainWindow != null)
                {
                    mainWindow.AnimateContentChange(editDepartment);
                }
            }
        }

        // Delete Button Click Handler
        private void DeleteDepartment_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            DepartmentModel selectedDepartment = button?.Tag as DepartmentModel;

            if (selectedDepartment != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete the Department: {selectedDepartment.Name}?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteDepartment(selectedDepartment.DepartmentID);
                    LoadDepartmentData(); // Refresh DataGrid
                }
            }
        }

        private void DeleteDepartment(string departmentID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM DepartmentTable WHERE DepartmentID = @DepartmentID"; // Ensure table name is correct

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Department deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Department not found or cannot be deleted.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting department: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // View Details Button Click Handler
        private void ViewDepartmentDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is DepartmentModel selectedDepartment)
            {
                // Pass the correct DepartmentModel object
                Department_Detail departmentDetail = new Department_Detail(selectedDepartment);
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.AnimateContentChange(departmentDetail);
                }
            }
        }

        private void ConfigureColumnVisibility()
        {
            // Hide action columns based on role
            var editColumn = dgDepartments.Columns
                .FirstOrDefault(c => c.Header.ToString() == "Edit");
            var deleteColumn = dgDepartments.Columns
                .FirstOrDefault(c => c.Header.ToString() == "Delete");

            if (editColumn != null)
                editColumn.Visibility = userRole == "HRManager" || userRole == null ?
                    Visibility.Visible : Visibility.Collapsed;

            if (deleteColumn != null)
                deleteColumn.Visibility = userRole == "HRManager" || userRole == null ?
                    Visibility.Visible : Visibility.Collapsed;
        }


        public class DepartmentModel
        {
            public string DepartmentID { get; set; } // Adjust type if necessary
            public string Name { get; set; }
            public string Head { get; set; }
            public string Description { get; set; }
            public DateTime CreatedDate { get; set; }
            public string Status { get; set; }
        }

        private void txtSearchDepartment_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
