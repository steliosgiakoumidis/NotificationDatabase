

using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotificationApi.DatabaseLayer
{
    public interface IDatabaseAccess<T>
    {
         Task AddItem(T item);
         Task DeleteItem(T item);
         Task EditItem(T item);
         Task<List<T>> GetAllRecords(T item);
         Task<T> GetSingleRecord(T item, int userId);
    }
}