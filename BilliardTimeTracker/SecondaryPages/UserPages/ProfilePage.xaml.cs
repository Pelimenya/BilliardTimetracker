using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BilliardTimeTracker.Context;
using BilliardTimeTracker.LogicAndPartialModels;
using BilliardTimeTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BilliardTimeTracker.MainPages
{
    public partial class ProfilePage : Page
    {
        private User _currentUser;

        public ProfilePage()
        {
            InitializeComponent();
            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            // Загрузка профиля текущего пользователя
            var currentUserId = UserSession.Instance.UserId; // Предполагается, что UserSession хранит информацию о текущем пользователе
            using (var context = new ContextDB())
            {
                _currentUser = context.Users.Find(currentUserId);
                if (_currentUser != null)
                {
                    DataContext = _currentUser; // Привязываем данные текущего пользователя к контексту данных страницы
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить профиль пользователя.");
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser != null)
            {
                _currentUser.Username = UsernameTextBox.Text;

                if (!string.IsNullOrEmpty(PasswordBox.Password))
                {
                    _currentUser.PasswordHash = ComputeSHA256Hash(PasswordBox.Password);
                }

                using (var context = new ContextDB())
                {
                    context.Entry(_currentUser).State = EntityState.Modified;
                    context.SaveChanges();
                    MessageBox.Show("Профиль успешно обновлен.");
                }
            }
            else
            {
                MessageBox.Show("Не удалось сохранить изменения профиля пользователя.");
            }
        }

        private string ComputeSHA256Hash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
