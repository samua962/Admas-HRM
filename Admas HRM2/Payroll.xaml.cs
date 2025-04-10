using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // Use ObservableCollection
using System.Data.SqlClient;
using System.Linq; // Import LINQ
using System.Windows;
using System.Windows.Controls;

namespace Admas_HRM2
{
    public partial class Payroll : UserControl
    {
        private List<EmployeePayroll> EmployeePayrollList; // Original list
        private ObservableCollection<EmployeePayroll> FilteredEmployeePayrollList; // Filtered list

        public Payroll()
        {
            InitializeComponent();
            LoadEmployees();
        }

        // Load employees into DataGrid
        private void LoadEmployees()
        {
            EmployeePayrollList = new List<EmployeePayroll>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = "SELECT EmployeeID, FirstName, LastName, Username, Department, Role, Title, Gender, DOB, JoiningDate, Education, Salary, Status, HireType, Description, ContactEmail, PhoneNumber, Address, ProfileImage, AttachedFile, CreatedAt, UpdatedAt, DepartmentID FROM EmployeeTable";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeePayrollList.Add(new EmployeePayroll
                                {
                                    EmployeeID = reader.GetString(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Username = reader.GetString(3),
                                    Department = reader.GetString(4),
                                    Role = reader.GetString(5),
                                    Title = reader.GetString(6),
                                    Gender = reader.GetString(7),
                                    DOB = reader.GetDateTime(8),
                                    JoiningDate = reader.GetDateTime(9),
                                    Education = reader.GetString(10),
                                    Salary = reader.GetDecimal(11),
                                    Status = reader.GetString(12),
                                    HireType = reader.GetString(13),
                                    Description = reader.GetString(14),
                                    ContactEmail = reader.GetString(15),
                                    PhoneNumber = reader.GetString(16),
                                    Address = reader.GetString(17),
                                    ProfileImage = reader.IsDBNull(18) ? null : (byte[])reader[18], // Handle NULL values
                                    AttachedFile = reader.IsDBNull(19) ? null : (byte[])reader[19],
                                    CreatedAt = reader.GetDateTime(20),
                                    UpdatedAt = reader.GetDateTime(21),
                                    DepartmentID = reader.GetString(22)
                                });
                            }
                        }
                    }
                }

                // Set the filtered list initially to the full list
                FilteredEmployeePayrollList = new ObservableCollection<EmployeePayroll>(EmployeePayrollList);

                // Bind DataGrid
                dataGridPayroll.ItemsSource = FilteredEmployeePayrollList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employees: " + ex.Message);
            }
        }

        private void dataGridPayroll_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            // Add your salary calculation logic here
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Add your save logic here
        }

        private void btnUploadBill_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Bill File",
                Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                MessageBox.Show($"Selected file: {openFileDialog.FileName}");
            }
        }

        private void txtTotalSalary_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Handle total salary text changed if needed
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower(); // Get search text and convert to lowercase
            FilterEmployees(searchText);
        }

        private void FilterEmployees(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                FilteredEmployeePayrollList = new ObservableCollection<EmployeePayroll>(EmployeePayrollList); // No filter applied
            }
            else
            {
                var filteredList = EmployeePayrollList
                    .Where(emp => emp.FirstName.ToLower().Contains(searchText) ||
                                  emp.LastName.ToLower().Contains(searchText) ||
                                  emp.Department.ToLower().Contains(searchText))
                    .ToList();

                FilteredEmployeePayrollList = new ObservableCollection<EmployeePayroll>(filteredList); // Apply filter
            }

            // Bind filtered list to DataGrid
            dataGridPayroll.ItemsSource = FilteredEmployeePayrollList;
        }

        private void Detail_click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var payrollRecord = button?.DataContext as EmployeePayroll;

            if (payrollRecord != null)
            {
                if (HasPayrollForCurrentMonth(payrollRecord.EmployeeID))
                {
                    // Pass the EmployeeID to the Payroll_Detail user control
                    Payroll_Detail payrollDetail = new Payroll_Detail(payrollRecord.EmployeeID);

                    // Navigate to Payroll_Detail with animation
                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    if (mainWindow != null)
                    {
                        mainWindow.AnimateContentChange(payrollDetail);
                    }
                }
                else
                {
                    MessageBox.Show("No payroll record found for this employee.", "Payroll Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private bool HasPayrollForCurrentMonth(string employeeID)
        {
            bool recordExists = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT COUNT(*) 
                FROM Payroll 
                WHERE EmployeeID = @EmployeeID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                        int count = (int)cmd.ExecuteScalar();
                        recordExists = count > 0;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected Error: " + ex.Message);
            }
            return recordExists;
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var payrollClicked = button?.DataContext as EmployeePayroll;

            if (payrollClicked != null) // Changed from payrollRecord to payrollClicked
            {
                // Pass the selected payroll record to the Payroll_Mark user control
                Payrollmark payrollMark = new Payrollmark(payrollClicked);

                // Navigate to Payroll_Mark with animation
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.AnimateContentChange(payrollMark); // Corrected variable name
                }
            }
        }


        // Employee Model
        public class EmployeePayroll
        {
            public string EmployeeID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Username { get; set; }
            public string Department { get; set; }
            public string Role { get; set; }
            public string Title { get; set; }
            public string Gender { get; set; }
            public DateTime DOB { get; set; }
            public DateTime JoiningDate { get; set; }
            public string Education { get; set; }
            public decimal Salary { get; set; }
            public string Status { get; set; }
            public string HireType { get; set; }
            public string Description { get; set; }
            public string ContactEmail { get; set; }
            public string PhoneNumber { get; set; }
            public string Address { get; set; }
            public byte[] ProfileImage { get; set; } // Storing Image as Byte Array
            public byte[] AttachedFile { get; set; } // Storing Files as Byte Array
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public string DepartmentID { get; set; } // Ensure this is a string
        }
    }
}
