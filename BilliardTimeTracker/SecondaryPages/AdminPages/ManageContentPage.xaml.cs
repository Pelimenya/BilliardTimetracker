using BilliardTimeTracker.Context;
using BilliardTimeTracker.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace BilliardTimeTracker.SecondaryPages.AdminPages
{
    public partial class ManageContentPage : Page
    {
        private ContextDB _context;
        private ObservableCollection<Table> _tables;
        private ObservableCollection<Session> _bookings;

        public int SelectedTableId { get; set; }

        public ManageContentPage()
        {
            InitializeComponent();
            _context = new ContextDB();
            LoadTables();
            LoadBookings();
        }

        private void LoadTables()
        {
            _tables = new ObservableCollection<Table>(_context.Tables.ToList());
            TableComboBox.ItemsSource = _tables;
        }

        private void LoadBookings()
        {
            _bookings = new ObservableCollection<Session>(_context.Sessions
                .Include(s => s.User)
                .Include(s => s.Table)
                .ToList());
            BookingsDataGrid.ItemsSource = _bookings;
        }

        private void BookTable_Click(object sender, RoutedEventArgs e)
        {
            if (TableComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Пожалуйста, выберите стол для бронирования.");
                return;
            }

            if (!BookingDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Пожалуйста, выберите дату бронирования.");
                return;
            }

            if (StartTimePicker.SelectedDateTime == null || EndTimePicker.SelectedDateTime == null)
            {
                MessageBox.Show("Пожалуйста, выберите время начала и окончания бронирования.");
                return;
            }

            DateTime bookingDate = BookingDatePicker.SelectedDate.Value;
            TimeSpan startTime = StartTimePicker.SelectedDateTime.Value.TimeOfDay;
            TimeSpan endTime = EndTimePicker.SelectedDateTime.Value.TimeOfDay;

            DateTime startDateTime = bookingDate.Date.Add(startTime);
            DateTime endDateTime = bookingDate.Date.Add(endTime);

            if (startDateTime >= endDateTime)
            {
                MessageBox.Show("Время начала бронирования должно быть раньше времени окончания.");
                return;
            }

            int tableId = (int)TableComboBox.SelectedValue;
            SelectedTableId = tableId; // Установка выбранного стола

            string userName = UserNameTextBox.Text.Trim();

            User user = _context.Users.FirstOrDefault(u => u.Username == userName);

            if (user == null)
            {
                MessageBox.Show("Выбранный пользователь не существует. Пожалуйста, выберите другого пользователя.");
                return;
            }

            try
            {
                var existingBookings = _context.Sessions
                    .Where(s => s.TableId == tableId &&
                                ((startDateTime >= s.StartTime && startDateTime < s.EndTime) ||
                                 (endDateTime > s.StartTime && endDateTime <= s.EndTime)))
                    .ToList();

                if (existingBookings.Any())
                {
                    MessageBox.Show("Выбранное время уже забронировано. Пожалуйста, выберите другое время.");
                    return;
                }
                
                Session newBooking = new Session
                {
                    TableId = tableId,
                    StartTime = startDateTime,
                    EndTime = endDateTime,
                    UserId = user.UserId, 
                    Cost = CalculateBookingCost(startDateTime, endDateTime) 
                };

                _context.Sessions.Add(newBooking);
                _context.SaveChanges();

                MessageBox.Show("Стол успешно забронирован.");

                // Обновление списка бронирований
                LoadBookings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при бронировании стола: {ex.Message}");
            }
        }

        private decimal CalculateBookingCost(DateTime start, DateTime end)
        {
            TimeSpan duration = end - start;
            int hours = (int)Math.Ceiling(duration.TotalHours); // Примерный расчет по часам

      
            TableRate tableRate = _context.TableRates
                .Include(tr => tr.Rate)
                .FirstOrDefault(tr => tr.TableId == SelectedTableId);

            if (tableRate != null && tableRate.Rate != null)
            {
                decimal ratePerHour = tableRate.Rate.RatePerHour;
                return ratePerHour * hours;
            }

            return 0; 
        }
    }
}