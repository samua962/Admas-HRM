using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace Admas_HRM2
{
    /// <summary>
    /// Interaction logic for EmployDepartment.xaml
    /// </summary>
    public partial class EmployDepartment : UserControl
    {
        public List<EmployeeModel1> Employees { get; set; }
        private string userRole;
        private string departmentId;

        public EmployDepartment(string role, string deptId)
        {
            InitializeComponent();
            userRole = role;
            departmentId = deptId;
            LoadEmployeeData();
        }

        // Load Employee Data from SQL Server filtered by DepartmentID
        private void LoadEmployeeData()
        {
            Employees = new List<EmployeeModel1>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT EmployeeID, FirstName, LastName, Username, Department, Role, Title, 
                               Gender, DOB, JoiningDate, Education, Salary, Status, HireType, 
                               Description, ContactEmail, PhoneNumber, Address, DepartmentID
                        FROM EmployeeTable
                        WHERE DepartmentID = @DepartmentID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Employees.Add(new EmployeeModel1
                                {
                                    EmployeeID = reader.GetString(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Username = reader.GetString(3),
                                    Department = reader.GetString(4),
                                    Position = reader.GetString(5),
                                    Title = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    Gender = reader.GetString(7),
                                    DOB = reader.GetDateTime(8),
                                    JoiningDate = reader.GetDateTime(9),
                                    Education = reader.IsDBNull(10) ? null : reader.GetString(10),
                                    Salary = reader.GetDecimal(11),
                                    Status = reader.GetString(12),
                                    HireType = reader.GetString(13),
                                    Description = reader.IsDBNull(14) ? null : reader.GetString(14),
                                    ContactEmail = reader.GetString(15),
                                    PhoneNumber = reader.GetString(16),
                                    Address = reader.IsDBNull(17) ? null : reader.GetString(17),
                                    DepartmentID = reader.GetString(18)
                                });
                            }
                        }
                    }
                }

                dgEmployees.ItemsSource = Employees; // Bind to DataGrid
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employees: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();
            var filteredEmployees = Employees.Where(emp =>
                emp.FirstName.ToLower().Contains(searchText) ||
                emp.LastName.ToLower().Contains(searchText) ||
                emp.Username.ToLower().Contains(searchText) ||
                emp.Department.ToLower().Contains(searchText)).ToList();

            dgEmployees.ItemsSource = filteredEmployees;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            AddEmployDepartment addEmployee = new AddEmployDepartment();
            HR_Department mainWindow = Application.Current.MainWindow as HR_Department;
            if (mainWindow != null)
            {
                mainWindow.AnimateContentChange(addEmployee);
            }
        }

        private void dgEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem is EmployeeModel1 selectedEmployee)
            {
                // Create the detail view with the selected employee ID
                var employeeDetail = new EmployDetailDepartment(selectedEmployee.EmployeeID);

                // Get the main window based on current user role
                Window mainWindow = Application.Current.MainWindow;

                // Handle navigation based on window type
                if (mainWindow is MainWindow hrManagerWindow)
                {
                    hrManagerWindow.AnimateContentChange(employeeDetail);
                }
                else if (mainWindow is HR_Admin hrAdminWindow)
                {
                    hrAdminWindow.AnimateContentChange(employeeDetail);
                }
                else if (mainWindow is HR_Department departmentHeadWindow)
                {
                    departmentHeadWindow.AnimateContentChange(employeeDetail);
                }
                else
                {
                    MessageBox.Show("Navigation not supported for current user role",
                                  "Error",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an employee to view details.",
                              "Warning",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is EmployeeModel1 selectedEmployee)
            {
                EditEmployDepartment editEmployDepartment = new EditEmployDepartment(selectedEmployee); // Pass employee data
                HR_Department hR_Department = Application.Current.MainWindow as HR_Department;

                if (hR_Department != null)
                {
                    hR_Department.AnimateContentChange(editEmployDepartment);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

            if (sender is Button button && button.Tag is EmployeeModel1 selectedEmployee)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete employee: {selectedEmployee.FirstName} {selectedEmployee.LastName}?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteEmployee(selectedEmployee.EmployeeID);
                    LoadEmployeeData(); // Refresh DataGrid
                }
            }
        }

        // Method to Delete Employee from Database
        private void DeleteEmployee(string employeeID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();

                    // Start a transaction to ensure data consistency
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Step 1: Delete related records from PayrollTable
                            string deletePayrollQuery = "DELETE FROM Payroll WHERE EmployeeID = @EmployeeID";
                            using (SqlCommand cmdPayroll = new SqlCommand(deletePayrollQuery, conn, transaction))
                            {
                                cmdPayroll.Parameters.AddWithValue("@EmployeeID", employeeID);
                                cmdPayroll.ExecuteNonQuery();
                            }

                            // Step 2: Delete the employee record from EmployeeTable
                            string deleteEmployeeQuery = "DELETE FROM EmployeeTable WHERE EmployeeID = @EmployeeID";
                            using (SqlCommand cmdEmployee = new SqlCommand(deleteEmployeeQuery, conn, transaction))
                            {
                                cmdEmployee.Parameters.AddWithValue("@EmployeeID", employeeID);
                                int rowsAffected = cmdEmployee.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit(); // Commit transaction if both deletions succeed
                                    MessageBox.Show("Employee deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    transaction.Rollback(); // Rollback if employee does not exist
                                    MessageBox.Show("Employee not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // Rollback if an error occurs
                            MessageBox.Show("Error deleting employee: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
