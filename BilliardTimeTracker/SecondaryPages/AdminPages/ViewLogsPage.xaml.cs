using BilliardTimeTracker.Context;
using BilliardTimeTracker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BilliardTimeTracker.SecondaryPages.AdminPages
{
    public partial class ViewLogsPage : Page
    {
        private readonly ContextDB _context;
        private ObservableCollection<Session> _sessions;

        public ViewLogsPage()
        {
            InitializeComponent();
            _context = new ContextDB();
            LoadSessions();
            CalculateStatistics();
        }

        private void LoadSessions()
        {
            _sessions = new ObservableCollection<Session>(_context.Sessions
                .Include(s => s.User)
                .Include(s => s.Table)
                .ToList());
            SessionsDataGrid.ItemsSource = _sessions;
        }

        private void CalculateStatistics()
        {
            if (_sessions == null || !_sessions.Any())
                return;

            // Общее количество сессий
            int totalSessions = _sessions.Count;
            TotalSessionsTextBlock.Text = $"Общее количество сессий: {totalSessions}";

            // Общая выручка за все сессии
            decimal? totalRevenue = _sessions.Sum(s => s.Cost);
            TotalRevenueTextBlock.Text = $"Общая выручка: {totalRevenue:C}";

            // Средняя продолжительность сессии в минутах
            var sessionsWithTime = _sessions.Where(s => s.StartTime != null && s.EndTime != null);
            if (sessionsWithTime.Any())
            {
                double averageDurationMinutes =
                    sessionsWithTime.Average(s => (s.EndTime.Value - s.StartTime).TotalMinutes);
                TimeSpan averageDuration = TimeSpan.FromMinutes(averageDurationMinutes);
                AverageDurationTextBlock.Text =
                    $"Средняя продолжительность сессии: {averageDuration.Hours} ч {averageDuration.Minutes} мин";
            }
        }
    }
}