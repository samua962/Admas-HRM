using Microsoft.Win32;
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
    /// Interaction logic for Admin_Add_Employee.xaml
    /// </summary>
    public partial class Admin_Add_Employee : UserControl
    {
        public Admin_Add_Employee()
        {
            InitializeComponent();
        }

        private void txtFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnSave(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel(object sender, RoutedEventArgs e)
        {

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
                file.Text = openFileDialog.FileName; // Assuming 'file' is a TextBox
            }
        }

        private void btnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                image.Text = openFileDialog.FileName; // Assuming 'image' is a TextBox
            }
        }
    }
}
