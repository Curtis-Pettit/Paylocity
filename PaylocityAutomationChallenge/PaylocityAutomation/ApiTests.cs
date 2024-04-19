using PaylocityApiTests;
using System.Net;
using System.Text.Json;

namespace PaylocityAutomation
{
    public class ApiTests : TestBase
    {

        /// <summary>
        /// Basic Smoke test Add endpoint
        /// Add a new employee, check that it was added.
        /// Also implicitly tests the Get Endpoint
        /// TODO: Implement other Add scenarios, suchs as duplicate names, names too long, invalid number of dependants
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AddEmployeeTest()
        {
            var firstName = "AddTestFirst";
            var lastName = "AddTestLast";

            var request = CreateHttpRequestMessage(
                HttpMethod.Post,
                employeesEndpointUri.AbsoluteUri,
                CreateEmployeeJson(firstName, lastName));

            var response = await _httpClient.SendAsync(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var returnedEmployee = JsonSerializer.Deserialize<Employee>(response.Content.ReadAsStream());
            Assert.That(returnedEmployee, Is.Not.Null);
            Assert.Multiple(() =>
                {
                    Assert.That(returnedEmployee.firstName, Is.EqualTo(firstName));
                    Assert.That(returnedEmployee.lastName, Is.EqualTo(lastName));

                });
            //Don't trust the returned value is the same as what is stored
            var newEmployee = await GetEmployee(returnedEmployee.id);

            Assert.Multiple(() =>
            {
                Assert.That(newEmployee.firstName, Is.EqualTo(firstName));
                Assert.That(newEmployee.lastName, Is.EqualTo(lastName));

            });
        }

        /// <summary>
        /// Basic check that Authorization is Required to use this API
        /// TODO: Implement same for other endpoints and other ways the authorization may be wrong (expired, malformated, incorrect)
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task MissingAuthTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, employeesEndpointUri.AbsoluteUri);
            var response = await _httpClient.SendAsync(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }


        /// <summary>
        /// Basic smoke test for Update endpoint.
        /// Add an employee, update name and dependants, check that changes were saved
        /// TODO: Other update scenarios suchs as duplicate names, names too long, invalid number of dependants, udating entry that does not exist.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task UpdateEmployeeTest()
        {
            var originalFirstName = "OriginalFirst";
            var originalLastName = "OrginalLast";

            var newFirstName = "newFirst";
            var newLastName = "newLast";

            var id = await AddEmployee(originalFirstName, originalLastName);

            var request = CreateHttpRequestMessage(
              HttpMethod.Put,
               employeesEndpointUri.AbsoluteUri,
              CreateEmployeeJson(newFirstName, newLastName, 5, id));

            var response = await _httpClient.SendAsync(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var returnedEmployee = JsonSerializer.Deserialize<Employee>(response.Content.ReadAsStream());
            Assert.That(returnedEmployee, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(returnedEmployee.firstName, Is.EqualTo(newFirstName));
                Assert.That(returnedEmployee.lastName, Is.EqualTo(newLastName));

            });
            //Don't trust the returned value is the same as what is stored
            var updatedEmployee = await GetEmployee(returnedEmployee.id);

            Assert.Multiple(() =>
            {
                Assert.That(updatedEmployee.firstName, Is.EqualTo(newFirstName));
                Assert.That(updatedEmployee.lastName, Is.EqualTo(newLastName));
            });
        }
        /// <summary>
        /// Basic smoke test for Delete enpoint. Add an employee, delete it, and check that you cannot get the employee anymore.
        /// TODO: Other Delete scenarios such as deleting an object which does't exist
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task DeleteEmployeeTest()
        {
            var firstName = "DeleteFirst";
            var lastName = "DeleteLast";

            var id = await AddEmployee(firstName, lastName);

            var request = CreateHttpRequestMessage(
              HttpMethod.Delete,
                $"{employeesEndpointUri.AbsoluteUri}/{id:D}");

            var response = await _httpClient.SendAsync(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var getRequest = CreateHttpRequestMessage(HttpMethod.Get, $"{employeesEndpointUri.AbsoluteUri}/{id:D}");

            var getResponse = await _httpClient.SendAsync(getRequest);

            Assert.That((await response.Content.ReadAsStringAsync()), Is.Empty);
            //BUGBUG test fails because api returns 200OK instead of 404
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Known Bug: See issue #8 in my report ");
        }

        /// <summary>
        /// Basic smoke test for Get Employee List. Add 2 employees and make sure they show up in the list.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetAllEmployeesTest()
        {
            var lastName = "GetAlllLast";

            var employee1Name = Guid.NewGuid().ToString("N");
            var employee2Name = Guid.NewGuid().ToString("N");

            var id1 = await AddEmployee(employee1Name, lastName, 1);
            var id2 = await AddEmployee(employee2Name, lastName, 2);

            var request = CreateHttpRequestMessage(HttpMethod.Get, employeesEndpointUri.AbsoluteUri);

            var response = await _httpClient.SendAsync(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var employeeList = JsonSerializer.Deserialize<List<Employee>>(response.Content.ReadAsStream());

            //It would be better if we knew and could verify all the results that should be in the list; but I don't have a way to get that list aside from the enpoint itself
            //forcing it by deleting everything seems too intrusive for a possibly shared environment and would prevent the tests from running in parrallel. 
            CollectionAssert.IsNotEmpty(employeeList);

            var returnedEmployee1 = employeeList.Find((x) => x.id == id1);
            Assert.That(returnedEmployee1, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(returnedEmployee1.firstName, Is.EqualTo(employee1Name));
                Assert.That(returnedEmployee1.lastName, Is.EqualTo(lastName));
                Assert.That(returnedEmployee1.dependants, Is.EqualTo(1));
            });

            var returnedEmployee2 = employeeList.Find((x) => x.id == id2);
            Assert.That(returnedEmployee2, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(returnedEmployee2.firstName, Is.EqualTo(employee2Name));
                Assert.That(returnedEmployee2.lastName, Is.EqualTo(lastName));
                Assert.That(returnedEmployee2.dependants, Is.EqualTo(2));
            });
        }
    }
}