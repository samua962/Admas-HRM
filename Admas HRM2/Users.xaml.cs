using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Admas_HRM2
{
    public partial class Users : UserControl
    {
        // Public connection string used by both the control and DAL.

        public Users()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                dgUsers.ItemsSource = UserDAL.GetAllUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event Handlers
        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            UserForm form = new UserForm();
            HR_Admin mainWindow = Application.Current.MainWindow as HR_Admin;
            if (mainWindow != null)
            {
                mainWindow.AnimateContentChange(form);
            }
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is User selectedUser)
            {
                Edit_Users editUserControl = new Edit_Users(selectedUser);
                HR_Admin mainWindow = Application.Current.MainWindow as HR_Admin;
                if (mainWindow != null)
                {
                    mainWindow.AnimateContentChange(editUserControl);
                }
            }
            else
            {
                MessageBox.Show("Please select a user to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is User selectedUser)
            {
                if (MessageBox.Show("Are you sure you want to delete this user?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        UserDAL.DeleteUser(selectedUser.UserID);
                        LoadUsers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is User selectedUser)
            {
                try
                {
                    var newPassword = UserDAL.ResetPassword(selectedUser.UserID);
                    MessageBox.Show($"Password reset successfully. New password: {newPassword}",
                        "Password Reset", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error resetting password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var searchTerm = txtSearch.Text;
            dgUsers.ItemsSource = UserDAL.SearchUsers(searchTerm);
        }
    }

    public class User
    {
        public int UserID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string DepartmentID { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }

        // Computed property for display
        public string AccountStatus => IsActive ? "Active" : "Inactive";
    }

    public static class UserDAL
    {
        public static List<User> GetAllUsers()
        {
            var users = new List<User>();

            using (var conn = new SqlConnection(Connection.connectionString))
            {
                // Note: Make sure the table has a UserID column if you're using it.
                var cmd = new SqlCommand(@"
                    SELECT UserID, UserFirst, UserLast, UserName, PasswordHash, 
                           Role, DepartmentID, IsActive, DateCreated
                    FROM Users", conn);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserID = (int)reader["UserID"],
                            FName = reader["UserFirst"].ToString(),
                            LName = reader["UserLast"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            Role = reader["Role"].ToString(),
                            DepartmentID = reader["DepartmentID"].ToString(),
                            IsActive = (bool)reader["IsActive"],
                            DateCreated = (DateTime)reader["DateCreated"]
                        });
                    }
                }
            }
            return users;
        }

        public static void AddUser(User user)
        {
            using (var conn = new SqlConnection(Connection.connectionString))
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO Users 
                        (UserFirst, UserLast, UserName, PasswordHash, Role, DepartmentID, IsActive)
                    VALUES 
                        (@First, @Last, @UserName, @Hash, @Role, @DeptID, @Active)", conn);

                cmd.Parameters.AddWithValue("@First", user.FName);
                cmd.Parameters.AddWithValue("@Last", user.LName);
                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@Hash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@DeptID", user.DepartmentID);
                cmd.Parameters.AddWithValue("@Active", user.IsActive);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateUser(User user)
        {
            using (var conn = new SqlConnection(Connection.connectionString))
            {
                var cmd = new SqlCommand(@"
                    UPDATE Users SET 
                        UserFirst = @First,
                        UserLast = @Last,
                        UserName = @UserName,
                        PasswordHash = @PasswordHash,
                        Role = @Role,
                        DepartmentID = @DeptID,
                        IsActive = @Active
                    WHERE UserID = @ID", conn);

                cmd.Parameters.AddWithValue("@ID", user.UserID);
                cmd.Parameters.AddWithValue("@First", user.FName);
                cmd.Parameters.AddWithValue("@Last", user.LName);
                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@DeptID", user.DepartmentID);
                cmd.Parameters.AddWithValue("@Active", user.IsActive);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteUser(int userId)
        {
            using (var conn = new SqlConnection(Connection.connectionString))
            {
                var cmd = new SqlCommand("DELETE FROM Users WHERE UserID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", userId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static string ResetPassword(int userId)
        {
            var newPassword = "1212";
            using (var conn = new SqlConnection(Connection.connectionString))
            {
                var cmd = new SqlCommand("UPDATE Users SET PasswordHash = @Hash WHERE UserID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", userId);
                cmd.Parameters.AddWithValue("@Hash", newPassword);  // Consider hashing this password before storing.
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return newPassword;
        }

        private static string GenerateTempPassword()
        {
            // Generate a random 8-character password.
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static List<User> SearchUsers(string searchTerm)
        {
            var users = new List<User>();
            using (var conn = new SqlConnection(Connection.connectionString))
            {
                var cmd = new SqlCommand(@"
                    SELECT UserID, UserFirst, UserLast, UserName, PasswordHash, 
                           Role, DepartmentID, IsActive, DateCreated
                    FROM Users 
                    WHERE UserFirst LIKE @Search OR 
                          UserLast LIKE @Search OR 
                          UserName LIKE @Search", conn);
                cmd.Parameters.AddWithValue("@Search", $"%{searchTerm}%");

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserID = (int)reader["UserID"],
                            FName = reader["UserFirst"].ToString(),
                            LName = reader["UserLast"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            Role = reader["Role"].ToString(),
                            DepartmentID = reader["DepartmentID"].ToString(),
                            IsActive = (bool)reader["IsActive"],
                            DateCreated = (DateTime)reader["DateCreated"]
                        });
                    }
                }
            }
            return users;
        }
    }
}
