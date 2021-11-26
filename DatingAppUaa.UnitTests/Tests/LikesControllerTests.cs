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
    public class LikesControllerTests
    {
        private string apiRoute = "api/likes";
        private readonly HttpClient _client;
        private HttpResponseMessage httpResponse;
        private string requestUri;
        private string registerObject;
        private HttpContent httpContent;

        public LikesControllerTests()
        {
            _client = TestHelper.Instance.Client;
        }

        [Theory]
        [InlineData("NotFound", "karen", "Pa$$w0rd","kratos")]
        public async Task AddLike_ShouldReturnNotFound(string statusCode, string username, string password,string userLiked)
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

            requestUri = $"{apiRoute}/"+userLiked;
            httpResponse = await _client.PostAsync(requestUri, httpContent);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("BadRequest", "karen", "Pa$$w0rd", "karen")]
        public async Task AddLike_ShouldReturnBadRequest(string statusCode, string username, string password, string userLiked)
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

            requestUri = $"{apiRoute}/" + userLiked;
            httpResponse = await _client.PostAsync(requestUri, httpContent);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("OK", "lois", "Pa$$w0rd", "mayo")]
        public async Task AddLike_ShouldReturnOK(string statusCode, string username, string password, string userLiked)
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

            requestUri = $"{apiRoute}/" + userLiked;
            httpResponse = await _client.PostAsync(requestUri, httpContent);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("BadRequest", "lois", "Pa$$w0rd", "todd")]
        public async Task AddLike_ShouldReturnBadRequest_Again(string statusCode, string username, string password, string userLiked)
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

            requestUri = $"{apiRoute}/" + userLiked;
            httpResponse = await _client.PostAsync(requestUri, httpContent);
            httpResponse = await _client.PostAsync(requestUri, httpContent);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("OK", "mayo", "Pa$$w0rd")]
        public async Task GetUserLikes_ShouldReturnOK(string statusCode, string username, string password)
        {
            var userDto = await LoginHelper.Login(username, password);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            requestUri = $"{apiRoute}" + "?predicated=likedBy";
            httpResponse = await _client.GetAsync(requestUri);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        #region Privated methods

        private static string GetRegisterObject(RegisterDto registerDto)
        {
            var entityObject = new JObject()
            {
                { nameof(registerDto.Username), registerDto.Username },
                { nameof(registerDto.KnownAs), registerDto.KnownAs },
                { nameof(registerDto.Gender), registerDto.Gender },
                { nameof(registerDto.DateOfBirth), registerDto.DateOfBirth },
                { nameof(registerDto.City), registerDto.City },
                { nameof(registerDto.Country), registerDto.Country },
                { nameof(registerDto.Password), registerDto.Password }
            };
            return entityObject.ToString();
        }

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