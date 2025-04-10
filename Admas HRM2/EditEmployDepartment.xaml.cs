using Microsoft.Win32;
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
    /// Interaction logic for EditEmployDepartment.xaml
    /// </summary>
    public partial class EditEmployDepartment : UserControl
    {
        private string employeeID;
        public EditEmployDepartment(EmployeeModel1 employee)
        {
            InitializeComponent();
            LoadEmployeeData(employee);
            //LoadDepartments();
        }


        private void LoadEmployeeData(EmployeeModel1 employee)
        {
            if (employee != null)
            {
                employeeID = employee.EmployeeID; // Store ID for update query
                txtEmployeeID.Text = employee.EmployeeID;
                txtFirstName.Text = employee.FirstName;
                txtLastName.Text = employee.LastName;
                txtUsername.Text = employee.Username;
                //cmbDepartment.SelectedItem = employee.Department; // Set selected department
                //txtDepartmentID.Text = employee.DepartmentID; // Assuming this should be set or modified elsewhere
                cmbRole.Text = employee.Position;
                cmbTitle.Text = employee.Title;
                cmbGender.Text = employee.Gender;
                dpDOB.SelectedDate = employee.DOB;
                dpJoiningDate.SelectedDate = employee.JoiningDate;
                txtEducation.Text = employee.Education;
                txtSalary.Text = employee.Salary.ToString();
                cmbStatus.Text = employee.Status;
                cmbHireType.Text = employee.HireType;
                txtDescription.Text = employee.Description;
                txtContactEmail.Text = employee.ContactEmail;
                txtPhoneNumber.Text = employee.PhoneNumber;
                txtAddress.Text = employee.Address;
            }
        }

        private void btnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif" // Specify image file types
            };

            if (openFileDialog.ShowDialog() == true)
            {
                image.Text = System.IO.Path.GetFileName(openFileDialog.FileName); // Display the image file name
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnCancel(); // Renamed to btnCancel for clarity
        }

        private void btnCancel()
        {
            EmployDepartment employDepartment = new EmployDepartment(SessionManager.UserRole, SessionManager.DepartmentID);
            HR_Department hR_Department = Application.Current.MainWindow as HR_Department;
         


            if (hR_Department != null)
            {
                hR_Department.AnimateContentChange(employDepartment);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // You can handle additional update logic here if necessary
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Save logic can be handled here if separate from Update
        }

        private void btnAttachFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select a File",
                Filter = "All Files (*.*)|*.*" // You can adjust the filter to specific file types
            };

            if (openFileDialog.ShowDialog() == true)
            {
                file.Text = System.IO.Path.GetFileName(openFileDialog.FileName); // Display the file name
            }
        }

        private void UpdateEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = @"
                    UPDATE EmployeeTable 
                    SET FirstName = @FirstName, LastName = @LastName, Username = @Username, 
                        Role = @Position, Title = @Title, Gender = @Gender, 
                        DOB = @DOB, JoiningDate = @JoiningDate, Education = @Education, Salary = @Salary, 
                        Status = @Status, HireType = @HireType, Description = @Description, 
                        ContactEmail = @ContactEmail, PhoneNumber = @PhoneNumber, Address = @Address
                    WHERE EmployeeID = @EmployeeID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                        cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                        //cmd.Parameters.AddWithValue("@Department", cmbDepartment.Text.Trim());

                        // Retrieve DepartmentID based on selected department
                        //cmd.Parameters.AddWithValue("@DepartmentID", GetDepartmentID(cmbDepartment.Text.Trim()));

                        cmd.Parameters.AddWithValue("@Position", cmbRole.Text.Trim());
                        cmd.Parameters.AddWithValue("@Title", cmbTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@Gender", cmbGender.Text.Trim());
                        cmd.Parameters.AddWithValue("@DOB", dpDOB.SelectedDate);
                        cmd.Parameters.AddWithValue("@JoiningDate", dpJoiningDate.SelectedDate);
                        cmd.Parameters.AddWithValue("@Education", txtEducation.Text.Trim());
                        cmd.Parameters.AddWithValue("@Salary", decimal.Parse(txtSalary.Text.Trim()));
                        cmd.Parameters.AddWithValue("@Status", cmbStatus.Text.Trim());
                        cmd.Parameters.AddWithValue("@HireType", cmbHireType.Text.Trim());
                        cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@ContactEmail", txtContactEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Employee updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            btnCancel();
                        }
                        else
                        {
                            MessageBox.Show("Failed to update employee.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating employee: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetDepartmentID(string departmentName)
        {
            string departmentID = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = "SELECT DepartmentID FROM DepartmentTable WHERE DepartmentName = @DepartmentName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentName", departmentName);
                        departmentID = cmd.ExecuteScalar()?.ToString(); // Retrieve DepartmentID based on name
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving Department ID: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return departmentID;
        }

        //private void LoadDepartments()
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(Connection.connectionString))
        //        {
        //            conn.Open();
        //            string query = "SELECT DepartmentName FROM DepartmentTable"; // Adjust column name if necessary

        //            using (SqlCommand cmd = new SqlCommand(query, conn))
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                cmbDepartment.Items.Clear(); // Clear existing items

        //                while (reader.Read())
        //                {
        //                    cmbDepartment.Items.Add(reader.GetString(0)); // Add department name
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error loading departments: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}
    }
}
