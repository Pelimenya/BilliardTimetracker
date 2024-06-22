using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BilliardTimeTracker.Context;
using BilliardTimeTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BilliardTimeTracker.SecondaryPages.AdminPages
{
    public partial class ManageUsersPage : Page
    {
        private ObservableCollection<User> Users { get; set; }
        private ContextDB _context;

        public ManageUsersPage()
        {
            InitializeComponent();
            _context = new ContextDB();
            LoadUsers();
        }

        private void LoadUsers(string searchQuery = null)
        {
            var usersQuery = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                usersQuery = usersQuery.Where(u => u.Username.Contains(searchQuery) || u.FullName.Contains(searchQuery));
            }

            var users = usersQuery.ToList();
            Users = new ObservableCollection<User>(users);
            UsersDataGrid.ItemsSource = Users;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers(SearchTextBox.Text);
        }

        private async void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                await _context.SaveChangesAsync();
                MessageBox.Show("Пользователь успешно сохранен.");
                LoadUsers(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении пользователя: {ex.Message}");
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя: {selectedUser.Username}?", "Удаление пользователя", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Users.Remove(selectedUser);
                        _context.SaveChanges();
                        Users.Remove(selectedUser);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}");
                    }
                }
            }
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewUsernameTextBox.Text) ||
                string.IsNullOrEmpty(NewFullNameTextBox.Text) ||
                string.IsNullOrEmpty(NewRoleTextBox.Text) ||
                string.IsNullOrEmpty(NewPasswordBox.Password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            using SHA256 sha256 = SHA256.Create();
            byte[] hashedPasswordBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(NewPasswordBox.Password));
            string hashedPassword = BitConverter.ToString(hashedPasswordBytes).Replace("-", "").ToLower();

            User newUser = new User
            {
                Username = NewUsernameTextBox.Text,
                FullName = NewFullNameTextBox.Text,
                Role = NewRoleTextBox.Text,
                PasswordHash = hashedPassword
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            LoadUsers();

            // Очистка полей ввода
            NewUsernameTextBox.Text = string.Empty;
            NewFullNameTextBox.Text = string.Empty;
            NewRoleTextBox.Text = string.Empty;
            NewPasswordBox.Password = string.Empty;

            MessageBox.Show("Новый пользователь добавлен.");
        }
    }
}