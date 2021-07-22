//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using UserApi.Model;
//using UserApi.Service;

//namespace UserApi
//{
//    public static class UserValidation
//    {
//        public static async Task<bool> CheckIfUserExists(User user, ICosmosDbService cosmosdb)
//        {
//            var result= await cosmosdb.GetItemByUserNameAsync(user.UserName);
//            if (result != null)
//                return true;
//            else
//                return false;
//        }
//    }
//}
