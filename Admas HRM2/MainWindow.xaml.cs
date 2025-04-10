using System;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;



namespace Admas_HRM2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            HighlightSelectedButton(btnDashboard);
            LoadDashboard();
        }

        // Handles all button navigation
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
                        AnimateContentChange(new Employee(SessionManager.UserRole));
                        break;
                    case "btnDepartments":
                        AnimateContentChange(new Department(SessionManager.UserRole));
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

        // Change the background of the selected button
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

        // Loads dashboard by default
        private void LoadDashboard()
        {
            AnimateContentChange(new Dashboard());
        }

        // Animates content change with fade effect
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

        // Logout functionality
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


        private void MainContentControl_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }

    }
}
