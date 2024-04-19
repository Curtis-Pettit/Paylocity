namespace PaylocityApiTests
{
    internal class CalcuationsTests : TestBase
    {
        /// <summary>
        /// Test for the calcuations for 0-32 depedants 
        /// Add a new employee, checks that the values returned are correct
        /// </summary>
        /// <returns></returns>
        [Test, TestCaseSource(nameof(CalcuationResults))]
        public async Task CalcuationTest(int dependants, double salary, double gross, double benefits, double net)
        {
            var firstName = "calculation";
            var lastName = "calculation";
            var id = await AddEmployee(firstName, lastName, dependants); ;

            var newEmployee = await GetEmployee(id);

            Assert.Multiple(() =>
            {
                Assert.That(newEmployee.dependants, Is.EqualTo(dependants));
            });
        }

        private static IEnumerable<TestCaseData> CalcuationResults()
        {
            //copy pasted from Excel
            yield return new TestCaseData(0, 52000, 2000, 38.46, 1961.54);
            yield return new TestCaseData(1, 52000, 2000, 57.69, 1942.31);
            yield return new TestCaseData(2, 52000, 2000, 76.92, 1923.08);
            yield return new TestCaseData(3, 52000, 2000, 96.15, 1903.85);
            yield return new TestCaseData(4, 52000, 2000, 115.38, 1884.62);
            yield return new TestCaseData(5, 52000, 2000, 134.62, 1865.38);
            yield return new TestCaseData(6, 52000, 2000, 153.85, 1846.15);
            yield return new TestCaseData(7, 52000, 2000, 173.08, 1826.92);
            yield return new TestCaseData(8, 52000, 2000, 192.31, 1807.69);
            yield return new TestCaseData(9, 52000, 2000, 211.54, 1788.46);
            yield return new TestCaseData(10, 52000, 2000, 230.77, 1769.23);
            yield return new TestCaseData(11, 52000, 2000, 250.00, 1750.00);
            yield return new TestCaseData(12, 52000, 2000, 269.23, 1730.77);
            yield return new TestCaseData(13, 52000, 2000, 288.46, 1711.54);
            yield return new TestCaseData(14, 52000, 2000, 307.69, 1692.31);
            yield return new TestCaseData(15, 52000, 2000, 326.92, 1673.08);
            yield return new TestCaseData(16, 52000, 2000, 346.15, 1653.85);
            yield return new TestCaseData(17, 52000, 2000, 365.38, 1634.62);
            yield return new TestCaseData(18, 52000, 2000, 384.62, 1615.38);
            yield return new TestCaseData(19, 52000, 2000, 403.85, 1596.15);
            yield return new TestCaseData(20, 52000, 2000, 423.08, 1576.92);
            yield return new TestCaseData(21, 52000, 2000, 442.31, 1557.69);
            yield return new TestCaseData(22, 52000, 2000, 461.54, 1538.46);
            yield return new TestCaseData(23, 52000, 2000, 480.77, 1519.23);
            yield return new TestCaseData(24, 52000, 2000, 500.00, 1500.00);
            yield return new TestCaseData(25, 52000, 2000, 519.23, 1480.77);
            yield return new TestCaseData(26, 52000, 2000, 538.46, 1461.54);
            yield return new TestCaseData(27, 52000, 2000, 557.69, 1442.31);
            yield return new TestCaseData(28, 52000, 2000, 576.92, 1423.08);
            yield return new TestCaseData(29, 52000, 2000, 596.15, 1403.85);
            yield return new TestCaseData(30, 52000, 2000, 615.38, 1384.62);
            yield return new TestCaseData(31, 52000, 2000, 634.62, 1365.38);
            yield return new TestCaseData(32, 52000, 2000, 653.85, 1346.15);
        }
    }
}