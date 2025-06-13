using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroApp.Classes;

namespace NeuroApp.Interfaces
{
    public interface IDatabaseActions
    {
        Task<List<Sales>> GetSalesFromDatabaseAsync();
        Task VerifyAndSave(Sales sales);
        Task<HashSet<string>> GetDeletedOsCodesAsync();
        Task<List<Tag>> GetTagsForOsAsync(string osCode);
        int CalculatePriority(string status);
        Task<bool> UpdatePriorityAsync(string osCode, bool isManual);
        Task<bool> RemoveOsAsync(string osCode);
        Task<bool> PauseOsAsync(string osCode, Status status, string type);
        Task<bool> UnpauseOsAsync(string osCode);
    }
}
