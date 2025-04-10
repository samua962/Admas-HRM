using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Admas_HRM2
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public SeriesCollection DepartmentSeries { get; set; }
        public ObservableCollection<string> DepartmentNames { get; set; }

        public SeriesCollection PayrollSeries { get; set; }
        public ObservableCollection<string> PayrollDepartmentNames { get; set; }

        public Dashboard()
        {
            InitializeComponent();

            CurrentDate.Text = DateTime.Now.ToString("MMMM dd, yyyy"); // Display Current Date

            DepartmentNames = new ObservableCollection<string>();
            DepartmentSeries = new SeriesCollection();

            PayrollDepartmentNames = new ObservableCollection<string>();
            PayrollSeries = new SeriesCollection();

            LoadDepartmentData();
            LoadTotalEmployeeCount();
            LoadTotalDepartmentCount();
            LoadPayrollData(); // Load Total Payroll Data

            DataContext = this;
        }

        private void LoadDepartmentData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT d.DepartmentName, COUNT(e.EmployeeID) AS EmployeeCount
                        FROM DepartmentTable d
                        LEFT JOIN EmployeeTable e ON d.DepartmentID = e.DepartmentID
                        GROUP BY d.DepartmentName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DepartmentSeries.Clear();
                        DepartmentNames.Clear();

                        while (reader.Read())
                        {
                            string departmentName = reader.GetString(0);
                            int employeeCount = reader.GetInt32(1);

                            DepartmentNames.Add(departmentName);

                            DepartmentSeries.Add(new PieSeries
                            {
                                Title = departmentName,
                                Values = new ChartValues<int> { employeeCount },
                                DataLabels = true
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading department data: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTotalEmployeeCount()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM EmployeeTable";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        int totalEmployees = (int)cmd.ExecuteScalar();
                        txtTotalEmployee.Text = totalEmployees.ToString(); // Set total employee count
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading total employees: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTotalDepartmentCount()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM DepartmentTable";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        int totalDepartments = (int)cmd.ExecuteScalar();
                        txtTotalDepartments.Text = totalDepartments.ToString(); // Set total department count
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading total departments: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPayrollData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT d.DepartmentName, SUM(p.Salary) AS TotalPayroll
                FROM Payroll p
                JOIN EmployeeTable e ON p.EmployeeID = e.EmployeeID
                JOIN DepartmentTable d ON e.DepartmentID = d.DepartmentID
                GROUP BY d.DepartmentName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        PayrollSeries.Clear();
                        PayrollDepartmentNames.Clear();
                        double totalPayroll = 0; // Initialize totalPayroll

                        ChartValues<double> payrollValues = new ChartValues<double>();

                        while (reader.Read())
                        {
                            string departmentName = reader.GetString(0);
                            double departmentTotalPayroll = Convert.ToDouble(reader.GetDecimal(1)); // Read the department's total payroll

                            PayrollDepartmentNames.Add(departmentName);
                            payrollValues.Add(departmentTotalPayroll);

                            totalPayroll += departmentTotalPayroll; // Accumulate the total payroll
                        }

                        // Set the total payroll to the TextBlock and format it as currency
                        txtTotalPayroll.Text = totalPayroll.ToString();

                        PayrollSeries.Add(new ColumnSeries
                        {
                            Title = "Total Payroll",
                            Values = payrollValues,
                            Fill = System.Windows.Media.Brushes.DodgerBlue,
                            DataLabels = true
                        });

                        // Force UI Refresh
                        Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading payroll data: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
