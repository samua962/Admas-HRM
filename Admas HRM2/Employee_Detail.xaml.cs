using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Admas_HRM2
{
    public partial class Employee_Detail : UserControl
    {

        public Employee_Detail(string employeeID)
        {
            InitializeComponent();
            LoadEmployeeDetails(employeeID);
        }

        private void LoadEmployeeDetails(string employeeID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = "SELECT FirstName, LastName, EmployeeID, Department, Role, Gender, Title, DOB, JoiningDate, Username, Education, HireType, Salary, Status, Description, ContactEmail, PhoneNumber, Address, ProfileImage, AttachedFile FROM EmployeeTable WHERE EmployeeID = @EmployeeID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Load text values
                                txtFirstName.Text = reader.GetString(0);
                                txtLastName.Text = reader.GetString(1);
                                txtEmployeeID.Text = reader.GetString(2);
                                txtDepartment.Text = reader.GetString(3);
                                txtRole.Text = reader.GetString(4);
                                txtGender.Text = reader.GetString(5);
                                txtTitle.Text = reader.GetString(6);
                                txtDOB.Text = reader.GetDateTime(7).ToString("d");
                                txtJoiningDate.Text = reader.GetDateTime(8).ToString("d");
                                txtUsername.Text = reader.GetString(9);
                                txtEducation.Text = reader.GetString(10);
                                txtHireType.Text = reader.GetString(11);
                                txtSalary.Text = reader.GetDecimal(12).ToString("C");
                                txtStatus.Text = reader.GetString(13);
                                txtDescription.Text = reader.IsDBNull(14) ? "N/A" : reader.GetString(14);
                                txtEmail.Text = reader.GetString(15);
                                txtPhoneNumber.Text = reader.GetString(16);
                                txtAddress.Text = reader.IsDBNull(17) ? "N/A" : reader.GetString(17);

                                // Load profile image
                                if (!reader.IsDBNull(18))
                                {
                                    byte[] imageData = (byte[])reader[18];
                                    imgProfile.Source = LoadImageFromBytes(imageData);
                                }

                                // Load file attachment
                                if (!reader.IsDBNull(19))
                                {
                                    byte[] fileData = (byte[])reader[19];
                                    string savedFilePath = SaveFileLocally(fileData);
                                    LoadFileAttachmentIntoUI(savedFilePath);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employee details: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private BitmapImage LoadImageFromBytes(byte[] imageData)
        {
            BitmapImage image = new BitmapImage();
            using (var stream = new MemoryStream(imageData))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
            }
            return image;
        }

        private string SaveFileLocally(byte[] fileData)
        {
            string tempFolderPath = Path.Combine(Path.GetTempPath(), "HRM_Files");

            if (!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);

            string filePath = Path.Combine(tempFolderPath, Guid.NewGuid().ToString()); // Use a unique name for each file
            File.WriteAllBytes(filePath, fileData);

            return filePath;
        }

        private void LoadFileAttachmentIntoUI(string filePath)
        {
            btnFileAttachment.Tag = filePath; // Store file path in the tag for later use
        }

        private void OpenFile(string filePath)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
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
            Employee editEmployee = new Employee(SessionManager.UserRole);
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                mainWindow.AnimateContentChange(editEmployee);
            }
        }

        private void btnFileAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (btnFileAttachment.Tag is string filePath)
            {
                OpenFile(filePath); // Open the file when the button is clicked
            }
        }
    }
}
