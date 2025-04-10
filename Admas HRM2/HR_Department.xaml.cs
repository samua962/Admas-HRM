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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Admas_HRM2
{
    /// <summary>
    /// Interaction logic for HR_Department.xaml
    /// </summary>
    public partial class HR_Department : Window
    {
        public HR_Department()
        {
            InitializeComponent();
            txtUsername.Text = name;
            LoadDashboard();
        }
        string name = "Welcom, "+SessionManager.UserFirst+" "+SessionManager.UserLast;
        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                HighlightSelectedButton(clickedButton);

                switch (clickedButton.Name)
                {
                    case "btnDashboard":
                        AnimateContentChange(new Dashboard());
                        break;
                    case "btnEmployees":
                        AnimateContentChange(new EmployDepartment(SessionManager.UserRole,SessionManager.DepartmentID));
                        break;
                    case "btnPayroll":
                        AnimateContentChange(new Payroll());
                        break;
                    case "btnAttendance":
                        AnimateContentChange(new Attendance());
                        break;
                    case "btnReports":
                        AnimateContentChange(new Report());
                        break;
                }
            }
        }

        private void HighlightSelectedButton(Button selectedButton)
        {
            SolidColorBrush selectedColor = new SolidColorBrush(Color.FromRgb(52, 152, 219)); // Blue
            SolidColorBrush defaultColor = new SolidColorBrush(Colors.Transparent);

            foreach (var child in SidebarMenu.Children)
            {
                if (child is Button button)
                {
                    button.Background = button == selectedButton ? selectedColor : defaultColor;
                }
            }
        }
        public void AnimateContentChange(UserControl newControl)
        {
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.2)));
            fadeOut.Completed += (s, e) =>
            {
                MainContentControl.Content = newControl;
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.25)));
                newControl.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };

            if (MainContentControl.Content is UserControl currentControl)
            {
                currentControl.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
            else
            {
                newControl.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                MainContentControl.Content = newControl;
            }
        }
        private void LoadDashboard()
        {
            AnimateContentChange(new Dashboard());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out?",
                                                      "Confirm Logout",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SessionManager.ClearSession();
                login login = new login();
                this.Close();
                login.Show();
            }
        }
    }
}
