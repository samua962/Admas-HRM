using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
    /// Interaction logic for AddEmployDepartment.xaml
    /// </summary>
    public partial class AddEmployDepartment : UserControl
    {        // Store the file paths
        private string imageFilePath;
        private string attachedFilePath;
        public AddEmployDepartment()
        {
            InitializeComponent();
            LoadDepartments();
        }
        private byte[] ConvertFileToBytes(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllBytes(filePath);
            }
            return null;
        }

        private void btnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                imageFilePath = openFileDialog.FileName; // Store the full file path
                image.Text = System.IO.Path.GetFileName(imageFilePath);
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
                attachedFilePath = openFileDialog.FileName; // Store the full file path
                file.Text = System.IO.Path.GetFileName(attachedFilePath);
            }
        }

        private void btnSave(object sender, RoutedEventArgs e)
        {
            string department="";
            
           
                try
                {
                    using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                    {
                        conn.Open();

                        string queryCheck = $"SELECT DepartmentName FROM DepartmentTable WHERE DepartmentID=@depId  ";
                        using (SqlCommand cmdD = new SqlCommand(queryCheck, conn))
                        {
                            cmdD.Parameters.AddWithValue("@depId", SessionManager.DepartmentID);

                            using (SqlDataReader reader = cmdD.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Retrieve user info
                                     department = reader["DepartmentName"].ToString();

                                }
                                
                            }
                        }
                        if (txtDepartmentID.Text != SessionManager.DepartmentID || cmbDepartment.Text!=department)
                        {
                            MessageBox.Show("Wrong Department Name or Id!", "Warnning", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                        }

                        string query = @"
                    INSERT INTO EmployeeTable (
                        EmployeeID, FirstName, LastName, Username, Department, Role, Title, Gender, DOB, JoiningDate, Education, Salary,
                        Status, HireType, Description, ContactEmail, PhoneNumber, Address, ProfileImage, AttachedFile, CreatedAt, UpdatedAt, DepartmentID
                    ) VALUES (
                        @EmployeeID, @FirstName, @LastName, @Username, @Department, @Role, @Title, @Gender, @DOB, @JoiningDate, @Education, @Salary,
                        @Status, @HireType, @Description, @ContactEmail, @PhoneNumber, @Address, @ProfileImage, @AttachedFile, GETDATE(), GETDATE(), @DepartmentID
                    )";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeID", txtEmployeeID.Text.Trim());
                            cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                            cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                            cmd.Parameters.AddWithValue("@Department", cmbDepartment.Text.Trim());
                            cmd.Parameters.AddWithValue("@DepartmentID", txtDepartmentID.Text.Trim());
                            cmd.Parameters.AddWithValue("@Role", cmbRole.Text.Trim());
                            cmd.Parameters.AddWithValue("@Title", cmbTitle.Text.Trim());
                            cmd.Parameters.AddWithValue("@Gender", cmbGender.Text.Trim());
                            cmd.Parameters.AddWithValue("@DOB", dpDOB.SelectedDate ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@JoiningDate", dpJoiningDate.SelectedDate ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Education", txtEducation.Text.Trim());
                            cmd.Parameters.AddWithValue("@Salary", Convert.ToDecimal(txtSalary.Text.Trim()));
                            cmd.Parameters.AddWithValue("@Status", cmbStatus.Text.Trim());
                            cmd.Parameters.AddWithValue("@HireType", cmbHireType.Text.Trim());
                            cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                            cmd.Parameters.AddWithValue("@ContactEmail", txtEmail.Text.Trim());
                            cmd.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text.Trim());
                            cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());

                            // Convert image and file to byte arrays
                            byte[] imageBytes = ConvertFileToBytes(imageFilePath);
                            byte[] fileBytes = ConvertFileToBytes(attachedFilePath);

                            cmd.Parameters.AddWithValue("@ProfileImage", imageBytes ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@AttachedFile", fileBytes ?? (object)DBNull.Value);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Employee added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    clearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        

        private void LoadDepartments()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = "SELECT DepartmentName FROM DepartmentTable"; // Adjust column name if necessary

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cmbDepartment.Items.Clear(); // Clear existing items

                        while (reader.Read())
                        {
                            cmbDepartment.Items.Add(reader.GetString(0)); // Add department name
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading departments: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel(object sender, RoutedEventArgs e)
        {
            EmployDepartment employDepartment = new EmployDepartment(SessionManager.UserRole, SessionManager.DepartmentID);
            HR_Department hR_Department = Application.Current.MainWindow as HR_Department;



            if (hR_Department != null)
            {
                hR_Department.AnimateContentChange(employDepartment);
            }
        }

        private void clearFields()
        {
            txtEmployeeID.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtUsername.Clear();
            cmbDepartment.SelectedIndex = -1;
            cmbRole.SelectedIndex = -1;
            cmbTitle.SelectedIndex = -1;
            cmbGender.SelectedIndex = -1;
            dpDOB.SelectedDate = null;
            dpJoiningDate.SelectedDate = null;
            txtEducation.Clear();
            txtSalary.Clear();
            cmbStatus.SelectedIndex = -1;
            cmbHireType.SelectedIndex = -1;
            txtDescription.Clear();
            txtEmail.Clear();
            txtPhoneNumber.Clear();
            txtAddress.Clear();
            image.Text = "";
            file.Text = "";
            txtDepartmentID.Clear();
            imageFilePath = null; // Reset the image file path
            attachedFilePath = null; // Reset the attached file path
        }

        private void cmbDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // You can add additional logic here if needed
        }

    }
}
