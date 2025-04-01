using System;

namespace NeuroApp.Classes
{
    public class Warranty
    {
        public int Id { get; set; }
        public string? OsCode { get; set; }
        public string ClientName { get; set; }
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
} 