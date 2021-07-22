using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UserApi.Auth;
using UserApi.Controllers;
using UserApi.Helpers;
using UserApi.Model;
using UserApi.Service;
using UserApi.Services;
using Xunit;

namespace UserApi.Tests
{
    public class LoginControllerTests
    {
        //ICosmosDbService mockCosMosService;
        //private Mock<Container> mockcontainer;
        Mock<ItemResponse<Model.User>> mockItemResponse;
        LoginController loginController;

        public LoginControllerTests()
        {
            mockItemResponse = new Mock<ItemResponse<Model.User>>();
            mockItemResponse.Setup(x => x.Resource).Returns(new Model.User() { id = "1234", UserName = "testUser", Password = "Test@123", Fullname = "Test User" });
            mockItemResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
            var mockCosmosServiceData = new Mock<ICosmosDbService>();
            mockCosmosServiceData.Setup(p => p.AddItemAsync(It.IsAny<Model.User>())).Returns(Task.FromResult(new Model.User() { id = "1234", UserName = "testUser", Password = "Test@123", Fullname = "Test User" }));
            mockCosmosServiceData.Setup(p => p.Authenticate(It.IsAny<string>(),It.IsAny<string>())).Returns(Task.FromResult(new Model.User() { id = "1234", UserName = "testUser", Password = "Test@123", Fullname = "Test User" }));
            mockCosmosServiceData.Setup(p => p.GetItemByUserNameAsync(It.IsAny<string>())).Returns(Task.FromResult<Model.User>(null));

            IJwtUtils mockJwt = new Mock<IJwtUtils>().Object;
            Mock<IOptions<AppSettings>> options = new Mock<IOptions<AppSettings>>();
            options.Setup(p => p.Value).Returns(new AppSettings() { FromEmail = "Test@gmail.com", FromName = "", WelcomeEmailBody = "Test", WelComeEmailSubject = "Test", Secret = "testSecret", SendGridKey = "testKey" });

            IEmailService mockEmailService = new Mock<IEmailService>().Object;

            loginController = new LoginController(mockCosmosServiceData.Object, mockJwt, options.Object, mockEmailService);
        }




        [Fact]
        public async void RegisterTest()
        {

            Model.User userData = new Model.User() { id = "1234", UserName = "testUser", Password = "Test@123", Fullname = "Test User" };
            var response = await loginController.Register(userData);
            Assert.NotNull(response);
            Assert.Equal(201, ((Microsoft.AspNetCore.Mvc.ObjectResult)response).StatusCode);

        }

        [Fact]
        public async void DashboardTest()
        {

            var response = await loginController.Dashboard();
            Assert.NotNull(response);
            Assert.Equal(401, ((Microsoft.AspNetCore.Mvc.ObjectResult)response).StatusCode);

        }
        [Fact]
        public async void GetLoginTest()
        {
            Model.User userData = new Model.User() { id = "1234", UserName = "testUser", Password = "Test@123", Fullname = "Test User" };
            Model.LoginModel loginModel= new Model.LoginModel() { UserName = "testUser", Password = "Test@123" };
            var response = await loginController.Login(loginModel) as OkObjectResult;
            var result = response.Value as AuthenticateResponse;
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(userData.Fullname, result.FullName);
            Assert.Equal(userData.UserName, result.Username);
            Assert.True(result.AuthSuccess);

        }

       
       
    }
}
