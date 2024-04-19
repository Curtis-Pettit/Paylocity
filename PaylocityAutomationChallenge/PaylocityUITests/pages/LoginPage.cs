using OpenQA.Selenium;


namespace PaylocityUITests.pages
{
    public class LoginPage : BasePage
    {
        private readonly string loginUrl = "https://wmxrwq14uc.execute-api.us-east-1.amazonaws.com/Prod/Account/Login";
        private const string usernameId = "Username";
        private const string passwordId = "Password";
        private const string loginButtonText = "Log In";

        private IWebElement GetUsernameField()
        {
            return GetWebElement(By.Id(usernameId), "Username");
        }

        private IWebElement GetPasswordField()
        {
            return GetWebElement(By.Id(passwordId), "Password");
        }

        private IWebElement GetLoginButton()
        {
            var loginButton = GetButtonByText(loginButtonText);
            return loginButton;
        }

        /// <summary>
        /// Clears the username field and enters the name provided username on the login page
        /// </summary>
        /// <param name="userName">username to enter</param>
        public void EnterUserName(string userName)
        {
            GetUsernameField().Clear();
            GetUsernameField().SendKeys(userName);
        }

        /// <summary>
        /// Clears the password field and enters the password provided on the login page
        /// </summary>
        /// <param name="password">password to enter</param>
        public void EnterPassword(string password)
        {
            GetPasswordField().Clear();
            GetPasswordField().SendKeys(password);
        }

        /// <summary>
        /// Logs the user in to the dashboard
        /// Note: username and password must already be entered
        /// </summary>
        /// <returns>Dashboard that Loging in redirects to</returns>
        public DashboardPage Login()
        {
            GetLoginButton().Click();
            return new DashboardPage(_driver);
        }


        public LoginPage(IWebDriver driver) : base(driver)
        {
            driver.Url = loginUrl;
        }
    }
}