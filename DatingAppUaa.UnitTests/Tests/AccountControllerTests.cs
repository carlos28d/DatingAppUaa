using DatingApp.Api.DTOs;
using DatingAppUaa.UnitTests.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DatingAppUaa.UnitTests.Tests
{
    public class AccountControllerTests
    {
        private string apiRoute = "api/account";
        private readonly HttpClient _client;
        private HttpResponseMessage httpResponse;
        private string requestUri;
        private string registerObject;
        private HttpContent httpContent;
        public AccountControllerTests()
        {
            _client = TestHelper.Instance.Client;
        }

        [Theory]
        [InlineData("OK", "Arturo", "Arthur", "Male", "1990-05-01", "Aguascalientes", "Mexico", "Pa$$w0rd")]
        [InlineData("OK", "Barbara", "Barbara", "Female", "1997-05-05", "Canberra", "Australia", "Pa$$w0rd")]
        public async Task Register_ShouldReturnOK(string statusCode, string username, string knownAs, string gender, DateTime dateOfBirth, string city, string country, string password)
        {
            requestUri = $"{apiRoute}/register";
            var registerDto = new RegisterDto{
                Username = username,
                KnownAs = knownAs,
                Gender = gender,
                DateOfBirth = dateOfBirth,
                City = city,
                Country = country,
                Password = password
            };
            
            registerObject = GetRegisterObject(registerDto);
            httpContent = GetHttpContent(registerObject);
            httpResponse = await _client.PostAsync(requestUri, httpContent);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("BadRequest", "Lisa", "Lisa", "female", "1956-07-22", "Greenbush", "Martinique", "Pa$$w0rd")]
        [InlineData("BadRequest", "Karen", "Karen", "female", "1955-10-12", "Celeryville", "Grenada", "Pa$$w0rd")]
        public async Task Register_ShouldFail(string statusCode, string username, string knownAs, string gender, DateTime dateOfBirth, string city, string country, string password)
        {
            requestUri = $"{apiRoute}/register";
            var registerDto = new RegisterDto{
                Username = username,
                KnownAs = knownAs,
                Gender = gender,
                DateOfBirth = dateOfBirth,
                City = city,
                Country = country,
                Password = password
            };
            
            registerObject = GetRegisterObject(registerDto);
            httpContent = GetHttpContent(registerObject);
            httpResponse = await _client.PostAsync(requestUri, httpContent);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("OK", "karen", "Pa$$w0rd")]
        public async Task Login_ShouldReturnOK(string statusCode, string username, string password)
        {
            requestUri = $"{apiRoute}/login";
            var loginDto = new LoginDto
            {
                Username = username,
                Password = password
            };

            registerObject = GetLoginObject(loginDto);
            httpContent = GetHttpContent(registerObject);
            httpResponse = await _client.PostAsync(requestUri, httpContent);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("Unauthorized", "poncho", "Wr0ng_Pa$$w0rd")]
        public async Task Login_ShouldReturnUnauthorized(string statusCode, string username, string password)
        {
            requestUri = $"{apiRoute}/login";
            var loginDto = new LoginDto{
                Username = username,
                Password = password
            };

            registerObject = GetLoginObject(loginDto);
            httpContent = GetHttpContent(registerObject);
            httpResponse = await _client.PostAsync(requestUri, httpContent);

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

        private static StringContent GetHttpContent(string objectToEncode)
        {
            return new StringContent(objectToEncode, Encoding.UTF8, "application/json");
        }

        #endregion
    }
}