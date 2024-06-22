using BilliardTimeTracker.Context;
using BilliardTimeTracker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BilliardTimeTracker.SecondaryPages.AdminPages
{
    public partial class ViewLogsPage : Page, INotifyPropertyChanged
    {
        private readonly ContextDB _context;
        private ObservableCollection<Session> _sessions;
        private DateTime? _selectedDate;

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime? SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
                FilterSessions();
                CalculateStatistics();
            }
        }

        public ViewLogsPage()
        {
            InitializeComponent();
            _context = new ContextDB();
            LoadSessions();
            CalculateStatistics(); // Вызываем CalculateStatistics для начального расчета статистики без фильтрации
            DataContext = this;
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

            IQueryable<Session> sessionsToCalculate;

            if (SelectedDate.HasValue)
            {
                DateTime selectedDay = SelectedDate.Value.Date;
                sessionsToCalculate = _sessions.Where(s => s.StartTime.Date == selectedDay).AsQueryable();
            }
            else
            {
                sessionsToCalculate = _sessions.AsQueryable();
            }

            // Общее количество сессий
            int totalSessions = sessionsToCalculate.Count();
            TotalSessionsTextBlock.Text = $"Общее количество сессий: {totalSessions}";

            // Общая выручка за все сессии
            decimal? totalRevenue = sessionsToCalculate.Sum(s => s.Cost);
            TotalRevenueTextBlock.Text = $"Общая выручка: {totalRevenue:C}";

            // Средняя продолжительность сессии в минутах
            var sessionsWithTime = sessionsToCalculate.Where(s => s.StartTime != null && s.EndTime != null);
            if (sessionsWithTime.Any())
            {
                double averageDurationMinutes =
                    sessionsWithTime.Average(s => (s.EndTime.Value - s.StartTime).TotalMinutes);
                TimeSpan averageDuration = TimeSpan.FromMinutes(averageDurationMinutes);
                AverageDurationTextBlock.Text =
                    $"Средняя продолжительность сессии: {averageDuration.Hours} ч {averageDuration.Minutes} мин";
            }
            else
            {
                AverageDurationTextBlock.Text = "Средняя продолжительность сессии: нет данных";
            }
        }

        private void FilterSessions()
        {
            if (_sessions == null)
                return;

            if (SelectedDate.HasValue)
            {
                DateTime selectedDay = SelectedDate.Value.Date;
                var filteredSessions = _sessions.Where(s => s.StartTime.Date == selectedDay).ToList();
                SessionsDataGrid.ItemsSource = filteredSessions;
            }
            else
            {
                SessionsDataGrid.ItemsSource = _sessions;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}