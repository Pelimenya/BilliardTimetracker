using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using BilliardTimeTracker.Context;
using BilliardTimeTracker.LogicAndPartialModels;
using BilliardTimeTracker.Models;

namespace BilliardTimeTracker.MainPages
{
    public partial class DashboardPage : Page
    {
        private DispatcherTimer _timer;
        public DashboardPage()
        {
            InitializeComponent();
            StartTimer();
        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); // Интервал обновления в 1 секунду
            _timer.Tick += (sender, args) => LoadData();
            _timer.Start();
        }
        
        private void LoadData()
        {
            using (var context = new ContextDB())
            {
                // Получение текущей игровой сессии пользователя
                var currentUserId = UserSession.Instance.UserId;
                var currentTime = DateTime.Now;
                var currentSession = context.Sessions
                    .Where(s => s.UserId == currentUserId && s.StartTime <= currentTime && (s.EndTime == null || s.EndTime >= currentTime))
                    .OrderByDescending(s => s.StartTime)
                    .FirstOrDefault();

                if (currentSession != null)
                {
                    var table = context.Tables.FirstOrDefault(t => t.TableId == currentSession.TableId);
                    var duration = currentTime - currentSession.StartTime;
                    var ratePerHour = context.TableRates
                        .Where(tr => tr.TableId == currentSession.TableId)
                        .Join(context.Rates, tr => tr.RateId, r => r.RateId, (tr, r) => r.RatePerHour)
                        .FirstOrDefault();
                    var cost = (decimal)duration.TotalHours * ratePerHour;

                    // Заполнение данных текущей игровой сессии
                    var currentSessionText = $"Стол: {table?.TableName}\n" +
                                             $"Время начала: {currentSession.StartTime}\n" +
                                             $"Длительность: {duration.Hours} час(ов) {duration.Minutes} минут(ы)\n" +
                                             $"Стоимость: {cost:F2} ₽";
                    CurrentSessionText.Text = currentSessionText;
                }
                else
                {
                    CurrentSessionText.Text = "Нет активной сессии";
                }

                // Получение последних игр пользователя
                var recentGames = context.Sessions
                    .Where(s => s.UserId == currentUserId && s.EndTime != null)
                    .OrderByDescending(s => s.StartTime)
                    .Take(5)
                    .Select(s => new
                    {
                        TableName = s.Table.TableName,
                        s.StartTime,
                        Duration = (s.EndTime - s.StartTime).Value.TotalMinutes,
                        s.Cost
                    })
                    .ToList();

                RecentGamesList.ItemsSource = recentGames;
            }
        }
    }
}
