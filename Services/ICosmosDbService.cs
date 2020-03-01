using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManager.Models;

namespace UserManager.Services
{
    public interface ICosmosDbService<T> where T : IEntity, new()
    {
        Task AddEntityAsync(T item);
        Task<T> GetEntityAsync(string id);
        Task<IEnumerable<T>> GetEntitiesAsync(string query);
        Task DeleteEntityAsync(string id);
        Task UpdateEntityAsync(string id, T item);
    }
}
