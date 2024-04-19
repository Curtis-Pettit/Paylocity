using OpenQA.Selenium;
using System.Diagnostics;

namespace PaylocityUITests.pages
{
    public class BasePage
    {
        protected IWebDriver _driver;
        protected readonly int _longWait = 10;

        public BasePage(IWebDriver driver)
        {
            _driver = driver;
        }

        /// <summary>
        /// Repeats an action until the condition is true
        /// </summary>
        /// <param name="conditionToWaitFor">lamba expression that returns true when its safe to continue</param>
        /// <param name="timeoutInSeconds">number of seconds to wait for the condition before giving up</param>
        /// <param name="pollingIntervalInMiliseconds">number of miliseconds to wait between attempts to evaluation <paramref name="conditionToWaitFor"/>"/> </param>
        /// <param name="errorDescription">description that will be in the exception message should the timeout expire</param>
        /// <exception cref="TimeoutException">Exception thrown when <paramref name="conditionToWaitFor"/> is not true before <paramref name="timeoutInSeconds"/> runs out </exception>
        public void PollingWait(Func<bool> conditionToWaitFor, int timeoutInSeconds, int pollingIntervalInMiliseconds = 100, string? errorDescription = null)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            do
            {
                var result = conditionToWaitFor();
                if (result)
                {
                    return;
                }
                else
                {
                    Task.Delay(pollingIntervalInMiliseconds).Wait();
                }

            } while (stopwatch.ElapsedMilliseconds < timeoutInSeconds * 1000);
            string errorMessage = $"Waited {timeoutInSeconds} seconds for {errorDescription}";
            throw new TimeoutException(errorMessage);
        }

        protected IWebElement GetWebElement(By selector, string name, IWebElement root = null)
        {

            IWebElement element = null;
            PollingWait(() =>
            {
                try
                {

                    if (root is null)
                    {
                        element = _driver.FindElement(selector);
                    }
                    else
                    {
                        element = root.FindElement(selector);
                    }

                    return true;

                }
                catch (NoSuchElementException)
                {
                    return false;

                }
            }, _longWait, errorDescription: $"{name} to be found by {selector.Mechanism} : {selector.Criteria}");

            PollingWait(() => element.Displayed, _longWait, errorDescription: $"{name} element to be displayed");
            PollingWait(() => element.Enabled, _longWait, errorDescription: $"{name} element to be enabled");
            return element;
        }
        protected IWebElement GetButtonByText(string text, IWebElement root = null)
        {

            IEnumerable<IWebElement> buttonList = null;

            PollingWait(() =>
            {
                if (root is null)
                {

                    //This could be done with an XPath query but I prefer to do it in C# so I have more control over how the errors are surfaced
                    buttonList = _driver.FindElements(By.ClassName("btn")).Where<IWebElement>((element) => element.Text == text);
                }
                else
                {
                    buttonList = root.FindElements(By.ClassName("btn")).Where<IWebElement>((element) => element.Text == text);
                }

                return buttonList.Count() != 0;
            }, _longWait, errorDescription: $"Login button to be found in the DOM");

            if (buttonList.Count() > 1)
            {
                throw new Exception($"Found more than one {text} Button");
            }
            var button = buttonList.First();
            PollingWait(() => button.Displayed, _longWait, errorDescription: $"{text} Button to be displayed");
            PollingWait(() => button.Enabled, _longWait, errorDescription: $"{text} Button to be enabled");
            return button;
        }
    }
}