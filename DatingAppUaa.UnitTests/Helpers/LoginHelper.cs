using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DatingApp.Api.DTOs;
using Newtonsoft.Json.Linq;

namespace DatingAppUaa.UnitTests.Helpers{
    public static class LoginHelper
    {
        public static async Task<UserDto> Login(string username, string password)
        {
            string requestUri = $"api/account/login";
            HttpClient client = TestHelper.Instance.Client;

            var loginDto = new LoginDto
            {
                Username = username,
                Password = password
            };

            var loginObject = GetLoginObject(loginDto);
            var httpContent = GetHttpContent(loginObject);

            var httpResponse = await client.PostAsync(requestUri, httpContent);
            var response = await httpResponse.Content.ReadAsStringAsync();
            var userDto = JsonSerializer.Deserialize<UserDto>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return userDto;
        }

        private static string GetLoginObject(LoginDto loginDto)
        {
            var loginObject = new JObject()
            {
                { nameof(loginDto.Username), loginDto.Username },
                { nameof(loginDto.Password), loginDto.Password }
            };

            return loginObject.ToString();
        }

        public static StringContent GetHttpContent(string objectToEncode)
        {
            return new StringContent(objectToEncode, Encoding.UTF8, "application/json");
        }
    }

}
