using System;
using System.ComponentModel;

namespace NeuroApp.Classes
{
    public class Warranty
    {
        public int Id { get; set; }
        public string? OsCode { get; set; }
        public string Customer { get; set; }
        public string SerialNumber { get; set; }
        public string Device { get; set; }
        public DateTime ServiceDate { get; set; }
        public DateTime WarrantyEndDate { get; set; }
        public int WarrantyMonths { get; set; }
        public string? Observation { get; set; }

        public int RemainingDays
        {
            get
            {
                return (WarrantyEndDate - DateTime.Now).Days;
            }
        }

        public bool IsExpired
        {
            get
            {
                return DateTime.Now > WarrantyEndDate;
            }
        }
    }

    public class Protocol : INotifyPropertyChanged
    {
        private bool _isExpanded;

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }
            }
        }

        public string ProtocolCode { get; set; }
        public string Customer { get; set; }
        public string Title { get; set; }
        public string SerialNumber { get; set; }
        public string Device { get; set; }
        public string Description { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 