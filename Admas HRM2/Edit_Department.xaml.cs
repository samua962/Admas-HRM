using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using static Admas_HRM2.Department;

namespace Admas_HRM2
{
    /// <summary>
    /// Interaction logic for Edit_Department.xaml
    /// </summary>
    public partial class Edit_Department : UserControl
    {
        private DepartmentModel currentDepartment;

        public Edit_Department(DepartmentModel department)
        {
            InitializeComponent();
            currentDepartment = department ?? throw new ArgumentNullException(nameof(department));
            PopulateFields();
        }

        private void PopulateFields()
        {
            if (currentDepartment != null)
            {
                txtDepartmentID.Text = currentDepartment.DepartmentID.ToString(); // Ensure it's a string
                txtDepartmentName.Text = currentDepartment.Name;
                txtDepartmentHead.Text = currentDepartment.Head;
                cbDepartmentStatus.Text = currentDepartment.Status;
                txtDepartmentDescription.Text = currentDepartment.Description;
            }
        }

        private void UpdateDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtDepartmentID.Text, out int departmentID))
            {
                MessageBox.Show("Invalid Department ID", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE DepartmentTable 
                                    SET DepartmentName = @Name, 
                                        Manager = @Head, 
                                        Description = @Description, 
                                        Status = @Status 
                                    WHERE DepartmentID = @DepartmentID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                        cmd.Parameters.AddWithValue("@Name", txtDepartmentName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Head", txtDepartmentHead.Text.Trim());
                        cmd.Parameters.AddWithValue("@Description", txtDepartmentDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@Status", cbDepartmentStatus.Text.Trim());

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Department updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            Cancel();
                        }
                        else
                        {
                            MessageBox.Show("Update failed. Please check your inputs.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating department: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Cancel()
        {
            Department departmentPage = new Department(SessionManager.UserRole);
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                mainWindow.AnimateContentChange(departmentPage);
            }
        }
    }
}
