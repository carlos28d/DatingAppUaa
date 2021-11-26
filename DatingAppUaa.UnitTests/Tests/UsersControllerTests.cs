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
using Windows.Storage;
using System.IO;

namespace DatingAppUaa.UnitTests.Tests
{
    public class UsersControllerTests
    {
        private string apiRoute = "api/users";
        private readonly HttpClient _client;
        private HttpResponseMessage httpResponse;
        private string requestUri;
        private string registerObject;
        private HttpContent httpContent;

        public UsersControllerTests()
        {
            _client = TestHelper.Instance.Client;
        }

        [Theory]
        [InlineData("OK", "karen", "Pa$$w0rd")]
        public async Task GetUsers_ShouldReturnOK(string statusCode, string username, string password)
        {
            var userDto = await LoginHelper.Login(username, password);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            requestUri = $"{apiRoute}";
            httpResponse = await _client.GetAsync(requestUri);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("OK", "admin", "Pa$$w0rd")]
        public async Task GetUserByUsername_ShouldReturnOK(string statusCode, string username, string password)
        {
            var userDto = await LoginHelper.Login(username, password);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            requestUri = $"{apiRoute}/" + username;
            httpResponse = await _client.GetAsync(requestUri);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("NoContent", "lisa", "Pa$$w0rd", "IntroductionU", "LookingForU", "InterestsU", "CityU", "CountryU")]
        public async Task UpdateUser_ShouldReturnNoContent(string statusCode, string username, string password, string introduction, string lookingFor, string interests, string city, string country)
        {
            var userDto = await LoginHelper.Login(username, password);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var memberUpdateDto = new MemberUpdateDto
            {
                Introduction = introduction,
                LookingFor = lookingFor,
                Interests = interests,
                City = city,
                Country = country
            };

            registerObject = GetMemberObject(memberUpdateDto);
            httpContent = GetHttpContent(registerObject);
            requestUri = $"{apiRoute}";
            httpResponse = await _client.PutAsync(requestUri, httpContent);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("Created", "lisa", "Pa$$w0rd", "goldenretriever.jpg")]
        public async Task AddPhoto_ShouldReturnCreated(string statusCode, string username, string password, string file)
        {
            var userDto = await LoginHelper.Login(username, password);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            MultipartFormDataContent form = new MultipartFormDataContent();
            HttpContent content = new StringContent(file);

            form.Add(content, file);

            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await storageFolder.GetFileAsync(file);

            var stream = await sampleFile.OpenStreamForReadAsync();
            content = new StreamContent(stream);
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "File",
                FileName = sampleFile.Name
            };

            form.Add(content);
            requestUri = $"{apiRoute}"+ "/add-photo";
            httpResponse = await _client.PostAsync(requestUri, form);
            
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        #region Privated methods

        private static string GetMemberObject(MemberUpdateDto memberUpdateDto)
        {
            var entityObject = new JObject()
            {
                { nameof(memberUpdateDto.Introduction), memberUpdateDto.Introduction },
                { nameof(memberUpdateDto.LookingFor), memberUpdateDto.LookingFor },
                { nameof(memberUpdateDto.Interests), memberUpdateDto.Interests },
                { nameof(memberUpdateDto.City), memberUpdateDto.City },
                { nameof(memberUpdateDto.Country), memberUpdateDto.Country }
            };
            return entityObject.ToString();
        }

        private static string GetFileObject(string file)
        {
            var entityObject = new JObject()
            {
                { "File", file}
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