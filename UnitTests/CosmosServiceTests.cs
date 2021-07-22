using Microsoft.Azure.Cosmos;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using UserApi.Service;
using Xunit;

namespace UserApi.Tests
{
    public class CosmosServiceTests
    {
        ICosmosDbService mockCosMosService;
        private Mock<Container> mockcontainer;
        Mock<ItemResponse<Model.User>> mockItemResponse;

        public CosmosServiceTests()
        {
            mockItemResponse = new Mock<ItemResponse<Model.User>>();
            mockItemResponse.Setup(x => x.Resource).Returns(new Model.User() { id = "1234", UserName = "testUser", Password = "Test@123", Fullname = "Test User" });
            mockItemResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
            mockcontainer = new Mock<Container>();
            mockcontainer.Setup(x => x.ReadItemAsync<Model.User>(It.IsAny<string>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), default(CancellationToken))).
                ReturnsAsync(mockItemResponse.Object);


            var unOrderList = new List<Model.User>(){
                new Model.User() { id = "4321", UserName = "user4321", Password = "india@123", Fullname = "user 4321" },
              new Model.User() { id = "1234", UserName = "user1234", Password = "india@123", Fullname = "user 1234" } };
            IOrderedQueryable<Model.User> orderList = unOrderList.AsQueryable().OrderBy(p => p.id.Length>0);
            
            //queryRespnse.Setup(p=>p.)
            
            mockcontainer.Setup(x => x.GetItemLinqQueryable<Model.User>(It.IsAny<bool>(), It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>(), It.IsAny<CosmosLinqSerializerOptions>())).Returns(orderList);
            var CosmosDBClient = new Mock<CosmosClient>();
            CosmosDBClient.Setup(p => p.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockcontainer.Object);
            mockCosMosService = new CosmosDbService(CosmosDBClient.Object, "test", "test");           
            
        }


        


        [Fact]
        public async void GetItemAsyncTest()
        {
            
            Model.User userData = new Model.User() { id = "1234", UserName = "testUser", Password = "Test@123", Fullname = "Test User" };
          var response= await mockCosMosService.GetItemAsync(userData.id);
            Assert.Equal(userData.id, response.id);
            Assert.Equal(userData.UserName, response.UserName);
            Assert.Equal(userData.Fullname , response.Fullname);

        }

        [Fact]
        public async void GetItemByUserNameAsyncTest()
        {

            Model.User userData = new Model.User() { id = "4321", UserName = "user4321", Password = "india@123", Fullname = "user 4321" };
            var response = await mockCosMosService.GetItemByUserNameAsync(userData.UserName);
            Assert.Equal(userData.id, response.id);
            Assert.Equal(userData.UserName, response.UserName);
            Assert.Equal(userData.Fullname, response.Fullname);

        }

    }
}
