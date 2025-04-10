using System;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using Microsoft.Win32;

namespace Admas_HRM2
{
    public partial class Payroll_Detail : UserControl
    {
        private string employeeID;
        private string tempFolderPath = Path.Combine(Path.GetTempPath(), "HRM_Files");

        public Payroll_Detail(string empID)
        {
            InitializeComponent();
            employeeID = empID;
            LoadPayrollDetails(employeeID);

        }

        private string GetFileExtension(byte[] fileData)
        {
            if (fileData.Length < 4) return ".bin"; // Default unknown file type

            if (fileData[0] == 0x25 && fileData[1] == 0x50 && fileData[2] == 0x44 && fileData[3] == 0x46)
                return ".pdf"; // PDF
            if (fileData[0] == 0xFF && fileData[1] == 0xD8)
                return ".jpg"; // JPEG
            if (fileData[0] == 0x89 && fileData[1] == 0x50 && fileData[2] == 0x4E && fileData[3] == 0x47)
                return ".png"; // PNG
            if (fileData[0] == 0x50 && fileData[1] == 0x4B && fileData[2] == 0x03 && fileData[3] == 0x04)
                return ".zip"; // ZIP (May also be DOCX/XLSX)
            if (fileData[0] == 0xD0 && fileData[1] == 0xCF && fileData[2] == 0x11 && fileData[3] == 0xE0)
                return ".doc"; // Old DOC/XLS files
            if (fileData[0] == 0x47 && fileData[1] == 0x49 && fileData[2] == 0x46)
                return ".gif"; // GIF
            if (fileData[0] == 0x49 && fileData[1] == 0x44 && fileData[2] == 0x33)
                return ".mp3"; // MP3 Audio
            if (fileData[0] == 0x52 && fileData[1] == 0x49 && fileData[2] == 0x46 && fileData[3] == 0x46)
                return ".wav"; // WAV Audio

            return ".bin"; // Default binary file
        }

        private void LoadPayrollDetails(string employeeID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    e.FirstName, 
                    e.LastName,
                    e.Department, 
                    e.HireType,
                    p.Salary, 
                    SUM(p.Salary) OVER (PARTITION BY e.EmployeeID) AS TotalSalary,
                    p.TaxAmount, 
                    p.NetPay, 
                    p.PaymentDate,
                    p.AttachedFile
                FROM 
                    EmployeeTable e
                INNER JOIN 
                    Payroll p ON e.EmployeeID = p.EmployeeID
                WHERE 
                    e.EmployeeID = @EmployeeID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string fName = reader["FirstName"]?.ToString() ?? "N/A";
                                string lName = reader["LastName"]?.ToString() ?? "N/A";
                                txtFullName.Text = fName + " " + lName;
                                txtDepartment.Text = reader["Department"]?.ToString() ?? "N/A";

                                decimal salary = reader["Salary"] as decimal? ?? 0;
                                decimal taxDeduction = reader["TaxAmount"] as decimal? ?? 0;
                                decimal netPay = reader["NetPay"] as decimal? ?? 0;
                                DateTime paymentDate = reader["PaymentDate"] as DateTime? ?? DateTime.MinValue;
                                string hireType = reader["HireType"]?.ToString() ?? "N/A";

                                string strSalary = "Gross Salary: " + salary.ToString(CultureInfo.CurrentCulture);
                                string strTax = "Tax: " + taxDeduction.ToString(CultureInfo.CurrentCulture);
                                txtSalary.Text = strSalary;
                                txtDeductions.Text = strTax;
                                txtNetSalary.Text = netPay.ToString(CultureInfo.CurrentCulture);
                                txtPaymentDate.Text = paymentDate == DateTime.MinValue ? "N/A" : paymentDate.ToString("d", CultureInfo.CurrentCulture);
                                txtType.Text = hireType;

                                // Overall Payment (individual salary - deductions)
                                decimal overallPayment = salary - taxDeduction;
                                //txtOverallPayment.Text = overallPayment.ToString(CultureInfo.CurrentCulture);

                                // Load Total Salary from the query
                                txtOverallPayment.Text = reader["TotalSalary"]?.ToString() ?? "N/A";

                                // Load file attachment
                                if (!reader.IsDBNull(8)) // Update index to match the correct column for AttachedFile
                                {
                                    byte[] fileData = (byte[])reader["AttachedFile"];
                                    string savedFilePath = SaveFileLocally(fileData);
                                    btnOpenFile.Tag = savedFilePath; // Store file path for later use
                                    txtFileType.Text = GetFileExtension(fileData);
                                }
                            }
                            else
                            {
                                MessageBox.Show("No payroll record found for this employee.", "Payroll Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
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
        }


        private string SaveFileLocally(byte[] fileData)
        {
            if (!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);

            string filePath = Path.Combine(tempFolderPath, Guid.NewGuid().ToString()); // Unique file name
            File.WriteAllBytes(filePath, fileData);
            return filePath;
        }

        private void OpenFile(string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            closebtn();
        }

        private void closebtn()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.AnimateContentChange(new Payroll());
            }
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (btnOpenFile.Tag is string filePath)
            {
                OpenFile(filePath); // Open the file when the button is clicked
            }
        }

        private void SearchPayrollDetailsByDate(DateTime paymentDate)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    e.FirstName, 
                    e.LastName,
                    e.Department, 
                    p.Salary, 
                    p.Deductions, 
                    p.NetPay, 
                    p.PaymentDate,
                    p.AttachedFile
                FROM 
                    EmployeeTable e
                INNER JOIN 
                    Payroll p ON e.EmployeeID = p.EmployeeID
                WHERE 
                    CAST(p.PaymentDate AS DATE) = @PaymentDate AND e.EmployeeID = @EmployeeID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PaymentDate", paymentDate.Date); // Ensure only the date part is passed
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Load payroll details into the UI
                                string fName = reader["FirstName"]?.ToString() ?? "N/A";
                                string lName = reader["LastName"]?.ToString() ?? "N/A";
                                txtFullName.Text = fName + " " + lName;
                                txtDepartment.Text = reader["Department"]?.ToString() ?? "N/A";

                                decimal salary = reader["Salary"] as decimal? ?? 0;
                                decimal deductions = reader["Deductions"] as decimal? ?? 0;
                                decimal netPay = reader["NetPay"] as decimal? ?? 0;
                                DateTime paymentDateResult = reader["PaymentDate"] as DateTime? ?? DateTime.MinValue;

                                txtSalary.Text = salary.ToString("C", CultureInfo.CurrentCulture);
                                txtDeductions.Text = deductions.ToString("C", CultureInfo.CurrentCulture);
                                txtNetSalary.Text = netPay.ToString("C", CultureInfo.CurrentCulture);
                                txtPaymentDate.Text = paymentDateResult == DateTime.MinValue ? "N/A" : paymentDateResult.ToString("d", CultureInfo.CurrentCulture);

                                // Calculate Overall Payment
                                decimal overallPayment = salary - deductions;
                                txtOverallPayment.Text = overallPayment.ToString("C", CultureInfo.CurrentCulture);

                                // Load file attachment
                                if (!reader.IsDBNull(6))
                                {
                                    byte[] fileData = (byte[])reader["AttachedFile"];
                                    string savedFilePath = SaveFileLocally(fileData);
                                    btnOpenFile.Tag = savedFilePath; // Store file path for later use
                                    //txtFileType.Text = GetFileExtension(fileData);
                                }
                            }
                            else
                            {
                                MessageBox.Show("No payroll record found for this employee on the selected date.");
                            }
                        }
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
        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = dpSearchDate.SelectedDate;

            if (selectedDate.HasValue)
            {
                SearchPayrollDetailsByDate(selectedDate.Value);
            }
            else
            {
                MessageBox.Show("Please select a date to search.");
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
