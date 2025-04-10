using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static Admas_HRM2.Payroll;

namespace Admas_HRM2
{
    public partial class Payrollmark : UserControl
    {
        private string _employeeID;
        private decimal _salary;
        private decimal _deductions = 0; // Initialize deductions
        private byte[] fileBytes; // Store the byte array of the uploaded file

        public Payrollmark(EmployeePayroll employee)
        {
            InitializeComponent();
            LoadEmployeeDetails(employee);
        }

        public void LoadEmployeeDetails(EmployeePayroll employee)
        {
            if (employee == null)
            {
                MessageBox.Show("Employee details not found.");
                return;
            }

            _employeeID = employee.EmployeeID;
            _salary = employee.Salary;

            txtFirstName.Text = employee.FirstName;
            txtLastName.Text = employee.LastName;
            txtDepartment.Text = employee.Department;
            txtSalary.Text = _salary.ToString("C", CultureInfo.CurrentCulture);
            //txtDeductions.Text = _deductions.ToString("C", CultureInfo.CurrentCulture);

            // Calculate Net Pay (Salary - Deductions - Tax)
            decimal taxAmount = (_salary - _deductions) * 0.15m;
            decimal netPay = _salary - _deductions - taxAmount;
            txtSalaryWithTax.Text = netPay.ToString("C", CultureInfo.CurrentCulture);
        }

        private void btnMarkAsPaid_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_employeeID))
            {
                MessageBox.Show("Error: Employee ID is missing.");
                return;
            }

            // Ensure fileBytes is not null before inserting
            if (fileBytes == null)
            {
                MessageBox.Show("Error: Please upload a file before marking as paid.");
                return;
            }

            InsertPayrollRecord(_employeeID, _salary, _deductions, fileBytes);
        }

        private void InsertPayrollRecord(string employeeID, decimal salary, decimal deductions, byte[] fileBytes)
        {
            decimal taxAmount = (salary - deductions) * 0.15m;
            decimal netPay = salary - deductions - taxAmount;
            DateTime paymentDate = DateTime.Now;

            // Check if the employee has already been paid this month
            if (IsPaymentAlreadyMade(employeeID, paymentDate))
            {
                MessageBox.Show("Error: Payroll has already been marked as paid for this month.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Payroll (EmployeeID, Salary, Deductions, PaymentDate, TaxAmount, NetPay, AttachedFile) VALUES (@EmployeeID, @Salary, @Deductions, @PaymentDate, @TaxAmount, @NetPay, @AttachedFile)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                        cmd.Parameters.AddWithValue("@Salary", salary);
                        cmd.Parameters.AddWithValue("@Deductions", deductions);
                        cmd.Parameters.AddWithValue("@PaymentDate", paymentDate);
                        cmd.Parameters.AddWithValue("@TaxAmount", taxAmount);
                        cmd.Parameters.AddWithValue("@NetPay", netPay);
                        cmd.Parameters.AddWithValue("@AttachedFile", fileBytes ?? (object)DBNull.Value); // Store the byte array

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Payroll record inserted successfully.");
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected Error: " + ex.Message);
            }
        }

        private bool IsPaymentAlreadyMade(string employeeID, DateTime paymentDate)
        {
            int year = paymentDate.Year;
            int month = paymentDate.Month;

            using (SqlConnection conn = new SqlConnection(Connection.connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Payroll WHERE EmployeeID = @EmployeeID AND YEAR(PaymentDate) = @Year AND MONTH(PaymentDate) = @Month";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@Month", month);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // Return true if payment exists
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.AnimateContentChange(new Payroll()); // Ensure Payroll is the correct type or instance
            }
        }

        private void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select a File",
                Filter = "All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName; // Get the selected file path
                fileBytes = File.ReadAllBytes(filePath); // Read the file into a byte array
                txtFileName.Text = System.IO.Path.GetFileName(filePath); // Display the file name in the TextBlock
            }
        }
    }
}
