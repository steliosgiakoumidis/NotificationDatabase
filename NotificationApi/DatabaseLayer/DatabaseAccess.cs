using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NotificationApi.Model;

namespace NotificationApi.DatabaseLayer
{
    public class DatabaseAccess<T> : IDatabaseAccess<T> where T : class, IModelBase
    {
        private NotificationServiceContext _context; 
        public DatabaseAccess(NotificationServiceContext context)
        {
            _context = context;
        }
        public async Task AddItem(T item)
        {
            _context.Set<T>().Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItem(T item)
        {
            //Delete on guid
            _context.Set<T>().Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task EditItem(T item)
        {
            _context.Set<T>().Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task<List<T>> GetAllRecords(T exampleItem)
        {
            return _context.Set<T>().ToList();
        }

        public async Task<T> GetSingleRecord(T item, int id)
        {
            return _context.Set<T>().FirstOrDefault(x => x.Id == id);
        }
    }
}