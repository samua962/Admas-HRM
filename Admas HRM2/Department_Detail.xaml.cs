using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using static Admas_HRM2.Department;

namespace Admas_HRM2
{
    public partial class Department_Detail : UserControl
    {
        private DepartmentModel currentDepartment;

        public Department_Detail(DepartmentModel department)
        {
            InitializeComponent();
            currentDepartment = department;
            LoadDepartmentDetails();
        }

        private void LoadDepartmentDetails()
        {
            if (currentDepartment != null)
            {
                txtDepartmentName.Text = currentDepartment.Name;
                txtDepartmentDescription.Text = currentDepartment.Description;
                txtManager.Text = currentDepartment.Head;
                txtStatus.Text = currentDepartment.Status;
                txtCreatedDate.Text = currentDepartment.CreatedDate.ToString("yyyy-MM-dd");
            }
            else
            {
                MessageBox.Show("Invalid department data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.AnimateContentChange(new Department(SessionManager.UserRole));
            }
        }
    }
}
