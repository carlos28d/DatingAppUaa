using DatingApp.Api.DTOs;
using DatingAppUaa.UnitTests.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DatingAppUaa.UnitTests.Tests
{
    public class AdminControllerTests
    {
        private string apiRoute = "api/admin";
        private readonly HttpClient _client;
        private HttpResponseMessage httpResponse;
        private string requestUri;
        private string registerObject;
        private HttpContent httpContent;

        public AdminControllerTests()
        {
            _client = TestHelper.Instance.Client;
        }

        [Theory]
        [InlineData("OK", "admin", "Pa$$w0rd")]
        public async Task GetUsersWithRoles_ShouldReturnOK(string statusCode, string username, string password)
        {
            var userDto = await LoginHelper.Login(username, password);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            requestUri = $"{apiRoute}/users-with-roles";
            httpResponse = await _client.GetAsync(requestUri);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("OK", "admin", "Pa$$w0rd", "mayo", "Moderator,Member")]
        public async Task EditRoles_ShouldReturnOK(string statusCode, string username, string password,string user2,string roles)
        {
            var loginDto = new LoginDto
            {
                Username = username,
                Password = password
            };

            registerObject = GetLoginObject(loginDto);
            httpContent = GetHttpContent(registerObject);

            var result = await _client.PostAsync("api/account/login", httpContent);
            var userJson = await result.Content.ReadAsStringAsync();
            var user = userJson.Split(',');
            var token = user[1].Split("\"")[3];

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            requestUri = $"{apiRoute}/edit-roles/"+user2+"?roles="+roles;
            var data = "roles="+roles;
            httpResponse = await _client.PostAsync(requestUri,httpContent);
            
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        #region Privated methods

        private static string GetLoginObject(LoginDto loginDto)
        {
            var entityObject = new JObject()
            {
                { nameof(loginDto.Username), loginDto.Username },
                { nameof(loginDto.Password), loginDto.Password }
            };
            return entityObject.ToString();
        }

        private StringContent GetHttpContent(string objectToEncode)
        {
            return new StringContent(objectToEncode, Encoding.UTF8, "application/json");
        }

        #endregion
    }
}