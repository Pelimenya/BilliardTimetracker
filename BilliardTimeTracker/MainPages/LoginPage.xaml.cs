﻿using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BilliardTimeTracker.Context;
using BilliardTimeTracker.LogicAndPartialModels;

namespace BilliardTimeTracker.MainPages;

public partial class LoginPage : Page
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object sender, RoutedEventArgs e)
    {
        string enteredLogin = login.Text;
        string enteredPassword = password.Password;

        using SHA256 sha256 = SHA256.Create();
        byte[] hashedPasswordBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
        string hashedPassword = BitConverter.ToString(hashedPasswordBytes).Replace("-", "").ToLower();

        using (var context = new ContextDB())
        {
            var user = context.Users.FirstOrDefault(x => x.Username == enteredLogin);
            if (user != null && user.PasswordHash == hashedPassword)
            {
                // Сохранение информации о текущем пользователе
                UserSession.Instance.SetUser(user.UserId, user.Username, user.Role);

                if (user.Role == "Admin")
                {
                    NavigationService.Navigate(new AdminMenu());
                }
                else
                {
                    NavigationService.Navigate(new UserMenu());
                }
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }
    }
}

