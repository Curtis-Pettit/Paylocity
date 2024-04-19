using PaylocityAutomation;
using System.Text.Json;

namespace PaylocityApiTests
{
    public class TestBase
    {
        private string _basicAuthCredentials;
        protected Uri employeesEndpointUri = new Uri("https://wmxrwq14uc.execute-api.us-east-1.amazonaws.com/Prod/api/employees");

        protected HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            //its a bad practice to store credentials in code so load it from the environment
            _basicAuthCredentials = Environment.GetEnvironmentVariable("paylocityAuth");
            if (_basicAuthCredentials == null)
            {
                throw new Exception("Add your basic auth credientials to an environment variable named \"paylocityAuth\"");
            }

            _httpClient = new HttpClient();
        }
        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
        }

        #region Utilities

        protected StringContent CreateEmployeeJson(string name, string lastName, int dependants = 0, Guid? id = null)
        {
            Employee employee;
            if (id.HasValue)
            {
                employee = new Employee
                {
                    firstName = name,
                    lastName = lastName,
                    dependants = dependants,
                    id = id.Value
                };
            }
            else
            {
                employee = new Employee
                {
                    firstName = name,
                    lastName = lastName,
                    dependants = dependants,
                };
            }
            return new StringContent(JsonSerializer.Serialize(employee), null, "application/json");
        }

        protected HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string uri, HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, uri);
            request.Headers.Add("Authorization", $"Basic {_basicAuthCredentials}");
            if (content != null)
            {
                request.Content = content;
            }
            return request;
        }

        protected async Task<Guid> AddEmployee(string firstName, string lastName, int dependants = 0)
        {
            var request = CreateHttpRequestMessage(
                HttpMethod.Post,
                employeesEndpointUri.AbsoluteUri,
                CreateEmployeeJson(firstName, lastName, dependants));

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var returnedEmployee = JsonSerializer.Deserialize<Employee>(response.Content.ReadAsStream());
            if (returnedEmployee == null)
            {
                throw new Exception($"Failed to correctly add new employee. Response was {await response.Content.ReadAsStringAsync()}");
            }
            return returnedEmployee.id;
        }

        protected async Task<Employee> GetEmployee(Guid id)
        {
            var request = CreateHttpRequestMessage(HttpMethod.Get, $"{employeesEndpointUri.AbsoluteUri}/{id:D}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var employee = JsonSerializer.Deserialize<Employee>(response.Content.ReadAsStream());
            if (employee == null)
            {
                throw new Exception($"Failed to get employee with id {id:D}. Response was {await response.Content.ReadAsStringAsync()}");
            }
            return employee;
        }
        #endregion Utilities
    }
}