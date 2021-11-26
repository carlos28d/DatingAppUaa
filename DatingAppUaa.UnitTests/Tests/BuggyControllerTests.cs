using DatingAppUaa.UnitTests.Helpers;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace DatingAppUaa.UnitTests.Tests
{
    public class BuggyControllerTests
    {
        private string apiRoute = "api/buggy";
        private readonly HttpClient _client;
        private HttpResponseMessage httpResponse;
        private string requestUri;

        public BuggyControllerTests()
        {
            _client = TestHelper.Instance.Client;
        }

        [Theory]
        [InlineData("OK", "karen", "Pa$$w0rd")]
        public async Task GetSecret_ShouldReturnOK(string statusCode, string username, string password)
        {
            var userDto = await LoginHelper.Login(username, password);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDto.Token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            requestUri = $"{apiRoute}/auth";
            httpResponse = await _client.GetAsync(requestUri);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("NotFound")]
        public async Task GetNotFound_ShouldReturnNotFound(string statusCode)
        {
            requestUri = $"{apiRoute}/not-found";
            httpResponse = await _client.GetAsync(requestUri);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("InternalServerError")]
        public async Task GetServerError_ShouldReturnInternalServerError(string statusCode)
        {
            requestUri = $"{apiRoute}/server-error";
            httpResponse = await _client.GetAsync(requestUri);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("BadRequest")]
        public async Task GetBadRequest_ShouldReturnBadRequest(string statusCode)
        {
            requestUri = $"{apiRoute}/bad-request";
            httpResponse = await _client.GetAsync(requestUri);

            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
        }

        #region Privated methods

        #endregion
    }
}