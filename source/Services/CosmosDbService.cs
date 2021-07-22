namespace UserApi.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Fluent;
    using Microsoft.Extensions.Configuration;
    using UserApi.Auth;
    using UserApi.Model;

    public class CosmosDbService : ICosmosDbService
    {
        private Container _container;
        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,             
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }
        public async Task AddItemAsync(Model.User user)
        {
            try
            {
                await this._container.CreateItemAsync<Model.User>(user, new PartitionKey(user.id));
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                System.Console.WriteLine("");
            }
            
        }
        
        public async Task<Model.User> GetItemAsync(string id)
        {
            
                ItemResponse<Model.User> response = await this._container.ReadItemAsync<Model.User>(id, new PartitionKey(id));
                return response.Resource;
            
            

        }
        public async Task<Model.User> GetItemByUserNameAsync(string userName)
        {
            try
            {
                IQueryable<Model.User> queryable = this._container.GetItemLinqQueryable<Model.User>(true); //TODO:: need to find better way to query Cosmos DB efficiently and async
                queryable = queryable.Where<Model.User>(item => item.UserName == userName);
                var result = queryable.ToArray().FirstOrDefault();
                
                return result;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<Model.User> Authenticate(string userName, string password)
        {
            IQueryable<Model.User> queryable = this._container.GetItemLinqQueryable<Model.User>(true); //TODO:: need to find better way to query Cosmos DB efficiently and async
            queryable = queryable.Where<Model.User>(item => item.UserName == userName && item.Password == password);
            var result = queryable.ToArray().FirstOrDefault();
            return result;
        }
    }
}
