using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManager.Models;

namespace UserManager.Services
{
    public abstract class CosmosDbService<T> : ICosmosDbService<T> where T : IEntity, new()
    {
        protected readonly Container _container;
        public string Query { get; set; }

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddEntityAsync(T item)
        {
            await this._container.CreateItemAsync<T>(item);
        }

        public async Task DeleteEntityAsync(string id)
        {
            await this._container.DeleteItemAsync<T>(id, new PartitionKey(id));
        }

        public async Task<T> GetEntityAsync(string id)
        {
            try
            {
                ItemResponse<T> response = await this._container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }
        }

        public async Task<IEnumerable<T>> GetEntitiesAsync(string condition)
        {
            string queryString = Query; // Query provided in derived class
            if (condition != null)
            {
                queryString += condition;
            }

            var query = this._container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateEntityAsync(string id, T item)
        {
            await this._container.UpsertItemAsync<T>(item);
        }

    }
}
