To run these Tests you need 2 environment variables
set paylocityAuth=<basicauth credentials>
set paylocityPassword= <UI password for the TestUser367 account>

AFTER setting environment variables open the PaylocityAutomationChallenge.sln file in VisualStudio 2022 17.9.6
Right Click Solution in Solution Explorere and choose Rebuild Solution
Open Test Explorer (View >TextExplorer)
Click Run All Tests In View 



NOTE: One test fails because of a defect in the product, noted in the Bug Fidning Challenge Report

Each Test has a description in the XML comments as well as ideas for tests I would likey do next if this were a 'real' project instead of an interview