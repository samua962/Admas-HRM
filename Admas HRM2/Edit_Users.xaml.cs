using System;
using System.Collections.Generic;
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
    /// Interaction logic for Edit_Users.xaml
    /// </summary>
    public partial class Edit_Users : UserControl
    {
        private User currentUser;  // Store the user being edited

        public Edit_Users(User user)
        {
            InitializeComponent();
            currentUser = user;
            PopulateFields();
        }

        private void PopulateFields()
        {
            if (currentUser != null)
            {
                txtFirstName.Text = currentUser.FName;
                txtLastName.Text = currentUser.LName;
                txtUserName.Text = currentUser.UserName;
                cmbRole.Text = currentUser.Role;
                txtDepartmentID.Text = currentUser.DepartmentID;
                chkIsActive.IsChecked = currentUser.IsActive;
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                currentUser.FName = txtFirstName.Text;
                currentUser.LName = txtLastName.Text;
                currentUser.UserName = txtUserName.Text;
                currentUser.Role = cmbRole.Text;
                currentUser.DepartmentID = txtDepartmentID.Text;
                currentUser.IsActive = chkIsActive.IsChecked ?? false;

                UserDAL.UpdateUser(currentUser);

                MessageBox.Show("User updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            HR_Admin mainWindow = Application.Current.MainWindow as HR_Admin;
            if (mainWindow != null)
            {
                mainWindow.AnimateContentChange(new Users());
            }
        }
    }

}
