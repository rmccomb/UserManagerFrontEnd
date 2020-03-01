using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UserManager.Models;
using User = UserManager.Models.User;

namespace UserManager.Services
{
    public interface IUserService //: ICosmosDbService<User>
    {
        Task<string> ValidateUser(UserLogin login);
        Task<bool> ValidateToken(string token);
        Task<IEnumerable<User>> GetUsersAsync(string token);

        Task RegisterUser(User user);

        Task<User> GetUserAsync(string token, string email);

        Task UpdateUserAsync(string token, User user);

        Task DeleteUserAsync(string token, string email);
    }

    public class UserService : IUserService
    {
        public static IUserService InitializeClientInstanceAsync(IConfigurationSection configSection)
        {
            // Connect to configured database and create container if required

            string connectionString = configSection.GetSection("PrimaryConnectionString").Value;
            //string databaseName = configSection.GetSection("DatabaseName").Value;
            //string containerName = configSection.GetSection("ContainerName").Value;
            //string partitionKey = configSection.GetSection("PartitionKey").Value;

            //var clientBuilder = new CosmosClientBuilder(connectionString);
            //CosmosClient client = clientBuilder.WithConnectionModeDirect().Build();
            //DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            //await database.Database.CreateContainerIfNotExistsAsync(containerName, partitionKey);

            var service = new UserService(connectionString);
            return service;
        }

        //public UserService(
        //    CosmosClient dbClient,
        //    string databaseName,
        //    string containerName)
        //    : base(dbClient, databaseName, containerName)
        //{
        //    Query = $"SELECT * FROM c WHERE c.type = '{new User().type}' AND c.isDeleted = false";
        //}

        private string _connectionUri;

        public UserService(string connectionString)
        {
            _connectionUri = connectionString;
        }

        //public IQueryable<User> GetJobIterator() => _container
        //    .GetItemLinqQueryable<User>()
        //    .Where(a => a.isDeleted == false && a.CreatedDate != null && a.type == "user");


        private HttpClient GetHttpClient(string token)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_connectionUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Cookie", $"{token}");
            return client;
        }

        public async Task<string> ValidateUser(UserLogin login)
        {
            HttpClient client = GetHttpClient(null);

            try
            {
                // Call http handler in ecn_users web app
                HttpResponseMessage response = await client.PostAsJsonAsync("signin", login);
                response.EnsureSuccessStatusCode();

                var cookie = response.Headers.First(h => h.Key == "Set-Cookie");
                var token = cookie.Value.First(c => c.StartsWith("token"));
                return token;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                client.CancelPendingRequests();
                throw ex;
            }
            finally
            {
                client.Dispose();
            }
        }

        public async Task RegisterUser(User user)
        {
            HttpClient client = GetHttpClient(null);

            try
            {
                // Call http handler in ecn_users web app
                HttpResponseMessage response = await client.PutAsJsonAsync("user", user);
                response.EnsureSuccessStatusCode();

            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                client.CancelPendingRequests();
                throw ex;
            }
            finally
            {
                client.Dispose();
            }
        }

        public async Task<bool> ValidateToken(string token)
        {
            var client = GetHttpClient(token);

            try
            {
                // Call http handler in ecn_users web app
                HttpResponseMessage response = await client.GetAsync("validatetoken");
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                client.CancelPendingRequests();
                throw ex;
            }
            finally
            {
                client.Dispose();
            }
        }

        public async Task<IEnumerable<User>> GetUsersAsync(string token)
        {
            var client = GetHttpClient(token);

            try
            {
                // Call http handler in ecn_users web app
                HttpResponseMessage response = await client.GetAsync("users");
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<IEnumerable<User>>(body);
                return users;
            }
            catch (Exception ex)
            {
                client.CancelPendingRequests();
                throw ex;
            }
            finally
            {
                client.Dispose();
            }
        }

        public async Task<User> GetUserAsync(string token, string email)
        {
            var client = GetHttpClient(token);

            try
            {
                // Call http handler in ecn_users web app
                HttpResponseMessage response = await client.GetAsync($"user/{email}");
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(body);
                return user;
            }
            catch (Exception ex)
            {
                client.CancelPendingRequests();
                throw ex;
            }
            finally
            {
                client.Dispose();
            }
        }

        public async Task UpdateUserAsync(string token, User user)
        {
            HttpClient client = GetHttpClient(token);

            try
            {
                // Call http handler in ecn_users web app
                HttpResponseMessage response = await client.PostAsJsonAsync("user", user);
                response.EnsureSuccessStatusCode();

            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                client.CancelPendingRequests();
                throw ex;
            }
            finally
            {
                client.Dispose();
            }
        }

        public async Task DeleteUserAsync(string token, string email)
        {
            HttpClient client = GetHttpClient(token);

            try
            {
                // Call http handler in ecn_users web app
                HttpResponseMessage response = await client.DeleteAsync($"user/{email}");
                response.EnsureSuccessStatusCode();

            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                client.CancelPendingRequests();
                throw ex;
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
