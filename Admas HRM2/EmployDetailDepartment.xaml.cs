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
    /// Interaction logic for EmployDetailDepartment.xaml
    /// </summary>
    public partial class EmployDetailDepartment : UserControl
    {
        public EmployDetailDepartment(string employeeID)
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
            string tempFolderPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "HRM_Files");

            if (!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);

            string filePath = System.IO.Path.Combine(tempFolderPath, Guid.NewGuid().ToString()); // Use a unique name for each file
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
            EmployDepartment employDepartment = new EmployDepartment(SessionManager.UserRole, SessionManager.DepartmentID);
            HR_Department hR_Department = Application.Current.MainWindow as HR_Department;



            if (hR_Department != null)
            {
                hR_Department.AnimateContentChange(employDepartment);
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
