using OpenQA.Selenium;
using PaylocityAutomation;

namespace PaylocityUITests.pages
{
    public class DashboardPage : BasePage
    {

        public DashboardPage(IWebDriver driver) : base(driver)
        {
            GetWebElement(By.LinkText("Log Out"), "Logout Button");
            //Note: This make the test require there to be some data in the database before the test is run.
            // Without this when the call returns slowly (at lot of data in the database) tests will move on to the next action Add which doesn't work
            // until the initial query is done even though the buttons are enabled maybe a bug but unlikely to affect end users.
            PollingWait(() =>
            {
                try
                {
                    IReadOnlyCollection<IWebElement> elements = _driver.FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));
                    return elements.Count > 0;
                }
                catch (Exception ex) { return false; }

            }, _longWait, errorDescription: "Rows to show on the dashboard. Test requires at least one row in the table to work");
        }

        /// <summary>
        /// Add an employee with the Dashboard
        /// </summary>
        /// <param name="firstName">first name of the employee</param>
        /// <param name="lastName">last name of employee</param>
        /// <param name="dependants">number of depedants on the employees benefits</param>
        public void AddEmployee(string firstName, string lastName, int dependants)
        {
            var addButton = GetButtonByText("Add Employee");
            addButton.Click();
            var modal = new AddEmployeeModal(_driver);
            modal.EnterFirstName(firstName);
            modal.EnterLastName(lastName);
            modal.EnterDependants(dependants);
            modal.ClickAdd();
        }

        /// <summary>
        /// Gets the detials of an employee from the Dashboard by thier First Name
        /// </summary>
        /// <param name="firstName">first name of the person to get from dashbaord</param>
        /// <returns>Data for the employee fron thier row in the UI</returns>
        public Employee FindEmployeeByName(string firstName)
        {
            IWebElement employee = null;
            PollingWait(() =>
            {
                try
                {
                    IReadOnlyCollection<IWebElement> elements = _driver.FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));
                    employee = elements.First<IWebElement>((element) => element.FindElement(By.CssSelector("td:nth-child(2)")).Text == firstName);
                    return true;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
                catch(StaleElementReferenceException)
                {
                    return false;
                }

            }, _longWait, errorDescription: $"new employee with name {firstName} added to dashboard");

            var data = employee.FindElements(By.TagName("td")).Select((dataCell) => dataCell.Text).ToList<string>();

            return new Employee
            {
                id = Guid.Parse(data[0]),
                firstName = data[1],
                lastName = data[2],
                dependants = int.Parse(data[3]),
                salary = double.Parse(data[4]),
                gross = double.Parse(data[5]),
                benefits = double.Parse(data[6]),
                net = double.Parse(data[7])
            };
        }
    }

    public class AddEmployeeModal : BasePage
    {
        IWebElement modalRoot;
        public AddEmployeeModal(IWebDriver driver) : base(driver)
        {
            modalRoot = GetWebElement(By.ClassName("modal-content"), "Add Employee Modal");
        }

        internal void ClickAdd()
        {
            GetButtonByText("Add", modalRoot).Click();
            PollingWait(() =>
            {
                return !modalRoot.Displayed;
            }, _longWait, errorDescription:"Add employee modal to dissapear after clicking add");
        }

        internal void EnterFirstName(string firstName)
        {
            GetFirstNameField().Clear();
            GetFirstNameField().SendKeys(firstName);
        }

        internal void EnterLastName(string lastName)
        {
            GetLastNameField().Clear();
            GetLastNameField().SendKeys(lastName);

        }
        internal void EnterDependants(int dependants)
        {
            GetDependantsField().Clear();
            GetDependantsField().SendKeys(dependants.ToString());
        }

        private IWebElement GetFirstNameField()
        {
            return GetWebElement(By.Id("firstName"), "Firstname field", modalRoot);
        }

        private IWebElement GetLastNameField()
        {
            return GetWebElement(By.Id("lastName"), "Lastname field", modalRoot);
        }
        private IWebElement GetDependantsField()
        {
            return GetWebElement(By.Id("dependants"), "dependants field", modalRoot);
        }
    }
}