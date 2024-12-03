using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroApp
{
    public class ServiceOrder
    {
        public enum Status
        {
            waitingBudget = 1,
            budgetReady = 2,
            budgetApproved = 3,
            inProcess = 4,
            waitingQuality = 5,
            finished = 6
        }

        public string Customer { get; set; }
        public int OsNumber { get; set; }
        public Status Status_ { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string? Observation { get; set; }
        public bool IsGuarantee { get; set; }

        public ServiceOrder(string customer, int osNumber, DateTime? arrivalDate, string? observation, bool isGuarantee)
        {
            Customer = customer;
            OsNumber = osNumber;
            ArrivalDate = arrivalDate;
            Observation = observation;
            IsGuarantee = isGuarantee;
        }
    }
}
