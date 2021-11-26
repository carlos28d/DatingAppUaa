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
    public class MessagesControllerTests
    {
        private string apiRoute = "api/messages";
        private readonly HttpClient _client;
        private HttpResponseMessage httpResponse;
        private string requestUri;
        private string registerObject;
        private HttpContent httpContent;

        public MessagesControllerTests()
        {
            _client = TestHelper.Instance.Client;
        }

        [Theory]
        [InlineData("BadRequest", "mayo", "Pa$$w0rd", "mayo", "Message")]
        public async Task CreateMessage_ShouldReturnBadRequest(string statusCode, string username, string password, string recipientUsername,string content)
        {
            var userDto = await LoginHelper.Login(username, password);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var messageDto = new MessageDto { 
                RecipientUsername= recipientUsername,
                Content = content
            };

            registerObject = GetMessageObject(messageDto);
            httpContent = GetHttpContent(registerObject);

            requestUri = $"{apiRoute}";
            httpResponse = await _client.PostAsync(requestUri, httpContent);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("NotFound", "lois", "Pa$$w0rd", "pedritosola", "Saludos")]
        public async Task CreateMessage_ShouldReturnNotFound(string statusCode, string username, string password, string recipientUsername, string content)
        {
            var userDto = await LoginHelper.Login(username, password);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var messageDto = new MessageDto
            {
                RecipientUsername = recipientUsername,
                Content = content
            };

            registerObject = GetMessageObject(messageDto);
            httpContent = GetHttpContent(registerObject);

            requestUri = $"{apiRoute}";
            httpResponse = await _client.PostAsync(requestUri, httpContent);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("OK", "lois", "Pa$$w0rd")]
        public async Task GetMessagesForUser_ShouldReturnOK(string statusCode, string username, string password)
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
        [InlineData("OK", "lois", "Pa$$w0rd", "lisa", "Hola")]
        public async Task CreateMessage_ShouldReturnOK(string statusCode, string username, string password, string recipientUsername, string content)
        {
            var userDto = await LoginHelper.Login(username, password);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var messageDto = new MessageDto
            {
                RecipientUsername = recipientUsername,
                Content = content
            };

            registerObject = GetMessageObject(messageDto);
            httpContent = GetHttpContent(registerObject);
            
            requestUri = $"{apiRoute}";
            httpResponse = await _client.PostAsync(requestUri, httpContent);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("OK", "lois", "Pa$$w0rd", "Outbox")]
        public async Task GetMessagesForUserFromQuery_ShouldReturnsOK(string statusCode, string username, string password, string container)
        {
            var userDto = await LoginHelper.Login(username, password);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            requestUri = $"{apiRoute}"+ "?container="+container;
            httpResponse = await _client.GetAsync(requestUri);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("OK", "lois", "Pa$$w0rd", "lisa")]
        public async Task GetMessagesThread_ShouldReturnOK(string statusCode, string username, string password, string user2)
        {
            var userDto = await LoginHelper.Login(username, password);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            requestUri = $"{apiRoute}/thread/" + user2;
            httpResponse = await _client.GetAsync(requestUri);
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("OK", "lois", "Pa$$w0rd", "lisa", "Hola")]
        public async Task DeleteMessage_ShouldReturnsOK(string statusCode, string username, string password, string recipientUsername, string content)
        {
            var userDto = await LoginHelper.Login(username, password);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var messageDto = new MessageDto
            {
                RecipientUsername = recipientUsername,
                Content = content
            };

            registerObject = GetMessageObject(messageDto);
            httpContent = GetHttpContent(registerObject);

            requestUri = $"{apiRoute}";

            var result = await _client.PostAsync(requestUri, httpContent);

            var messageJson = await result.Content.ReadAsStringAsync();
            var message = messageJson.Split(',');
            var id = message[0].Split("\"")[2].Split(":")[1];

            requestUri = $"{apiRoute}/"+id;

            userDto = await LoginHelper.Login(recipientUsername, password);

            httpResponse = await _client.DeleteAsync(requestUri);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            httpResponse = await _client.DeleteAsync(requestUri);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("Unauthorized", "lois", "Pa$$w0rd", "lisa", "Hola", "todd")]
        public async Task DeleteMessage_ShouldReturnUnauthorized(string statusCode, string username, string password, string recipientUsername, string content, string unauthorized_user)
        {
            var userDto = await LoginHelper.Login(username, password);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var messageDto = new MessageDto
            {
                RecipientUsername = recipientUsername,
                Content = content
            };

            registerObject = GetMessageObject(messageDto);
            httpContent = GetHttpContent(registerObject);

            requestUri = $"{apiRoute}";

            var result = await _client.PostAsync(requestUri, httpContent);
            var messageJson = await result.Content.ReadAsStringAsync();
            var message = messageJson.Split(',');
            var id = message[0].Split("\"")[2].Split(":")[1];

            requestUri = $"{apiRoute}/" + id;

            userDto = await LoginHelper.Login(unauthorized_user, password);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            httpResponse = await _client.DeleteAsync(requestUri);
            
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        #region Privated methods
        
        private static string GetMessageObject(MessageDto message)
        {
            var entityObject = new JObject()
            {
                { nameof(message.RecipientUsername), message.RecipientUsername },
                { nameof(message.Content), message.Content }
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