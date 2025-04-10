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
    /// Interaction logic for Admin_Employee.xaml
    /// </summary>
    public partial class Admin_Employee : UserControl
    {
        public Admin_Employee()
        {
            InitializeComponent();
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            Admin_Add_Employee addEmployee = new Admin_Add_Employee();
            HR_Admin mainWindow = Application.Current.MainWindow as HR_Admin;

            if (mainWindow != null)
            {
                mainWindow.AnimateContentChange(addEmployee);
            }
        }
        // Delete Button Click Handler
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is EmployeeModel selectedEmployee)
            {
                MessageBox.Show($"Delete Employee: {selectedEmployee.Name}");
            }
        }
        // SelectionChanged Event Handler
        private void dgEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgEmployees.SelectedItem is EmployeeModel selectedEmployee)
            {
                //txtEmployeeDetails.Text = $"Name: {selectedEmployee.Name}\nPosition: {selectedEmployee.Position}\nStatus: {selectedEmployee.Status}";
            }
        }
        // Edit Button Click Handler
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is EmployeeModel selectedEmployee)
            {

            }
           // Edit_Employee editEmployee = new Edit_Employee(selectedEmployee);
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                //mainWindow.AnimateContentChange(editEmployee);
            }
        }
        // Search Button Click Handler
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();
            // var filteredEmployees = Employees
            //     .Where(emp => emp.Name.ToLower().Contains(searchText) || emp.Position.ToLower().Contains(searchText))
            //     .ToList();

            // dgEmployees.ItemsSource = filteredEmployees;
        }
        // View Details Button Click Handler
        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is EmployeeModel selectedEmployee)
            {
                MessageBox.Show($"View Details for: {selectedEmployee.Name}");
            }
        }
    }
}
