using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using PaylocityApiTests;
using PaylocityUITests.pages;
using PaylocityAutomation;

namespace PaylocityUITests
{
    public class UiTests : TestBase
    {
        private string password;
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            //its a bad practice to store credentials in code so load it from the environment
            password = Environment.GetEnvironmentVariable("paylocityPassword");
            if (password == null)
            {
                throw new Exception("Add your dashboard password to an environment variable named \"paylocityPassword\"");
            }
            var path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chromeDriver\\chromedriver.exe"));
            driver = new ChromeDriver(path);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(0);
        }

        [TearDown]
        public void Teardown()
        {
            driver.Close();
            driver.Quit();
        }

        /// <summary>
        /// Basic UI smoke test Add an Employee and check that it was added correctly
        /// TODO: Impelement other smoke tests Such as an error messaging, basic update and basic delete. As well as other browsers
        /// </summary>
        [Test]
        public void AddEmployee()
        {
            var loginPage = new LoginPage(driver);
            loginPage.EnterUserName("TestUser367");
            loginPage.EnterPassword(password);

            var dashboardPage = loginPage.Login();

            var firstName = Guid.NewGuid().ToString("N"); //make it really unique so i can find it in the list
            var lastName = "UITest";
            var depedants = 3;

            dashboardPage.AddEmployee(firstName, lastName, depedants);
            Employee newEmployee = dashboardPage.FindEmployeeByName(firstName);
            Assert.That(newEmployee.firstName, Is.EqualTo(firstName));
            Assert.That(newEmployee.lastName, Is.EqualTo(lastName));
            Assert.That(newEmployee.dependants, Is.EqualTo(depedants));
            Assert.That(newEmployee.salary, Is.EqualTo(52000.00));
            Assert.That(newEmployee.gross, Is.EqualTo(2000.00));
            Assert.That(newEmployee.benefits, Is.EqualTo(96.15));
            Assert.That(newEmployee.net, Is.EqualTo(1903.85));
        }
    }
}