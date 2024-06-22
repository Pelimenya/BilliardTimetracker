using BilliardTimeTracker.Models;
using BilliardTimeTracker.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        private void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            User user = (User)button.DataContext;

            try
            {
                if (user.UserId == 0) // Проверка на 0 не требуется, так как UserId будет автоматически сгенерирован
                {
                    _context.Users.Add(user); // Добавление нового пользователя в контекст базы данных
                }
                else
                {
                    _context.Users.Update(user); // Обновление существующего пользователя
                }

                _context.SaveChanges(); // Сохранение изменений в базе данных
                MessageBox.Show("Пользователь успешно сохранен.");
                LoadUsers(); // Обновить список пользователей после сохранения
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
                        _context.Users.Remove(selectedUser); // Удаление пользователя из контекста базы данных
                        _context.SaveChanges(); // Сохранение изменений
                        Users.Remove(selectedUser); // Удаление пользователя из коллекции
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
            // Создание нового объекта User без присвоения UserId
            User newUser = new User
            {
                Username = "Новый пользователь",
                FullName = "Полное имя",
                Role = "Роль"
            };

            Users.Add(newUser); // Добавление в коллекцию для отображения в DataGrid
            UsersDataGrid.SelectedItem = newUser;
            UsersDataGrid.ScrollIntoView(newUser);
        }
    }
}