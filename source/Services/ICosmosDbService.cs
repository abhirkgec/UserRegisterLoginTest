namespace UserApi.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UserApi.Model;

    public interface ICosmosDbService
    {
        Task<User> GetItemAsync(string id);
        Task AddItemAsync(User user);
        Task<Model.User> GetItemByUserNameAsync(string userName);
        Task<Model.User> Authenticate(string userName, string password);

    }
}
