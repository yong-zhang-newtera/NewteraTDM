
// ============================================================
//        **** A JSNUnit 2.6 Testing Framework ****
//
//          Written by Troy Taft (email@troytaft.com)
//          (c) Troy Taft, 2006.  Permission is granted to use 
//          this software freely under the conditions of 
//          ValleyHighlands Free License.
//
//     Purpose:  The purpose of this code is to encapsulate
//               objects that are used by the JavaScript Unit
//               Testing Framework.
//
//               This JavaScript file includes the TestDisplay class
//               the Test class, and the Registry class.  It also
//               contains the functions: TestRunner, 
//               DisplayAndRunAllTests, and RunAllTests.
//
// ============================================================
//
//
//
function newTestDisplay() {
    var TestDisplay = new Object;
    TestDisplay.FailCount = 0;
    TestDisplay.TestCount = 0;
    TestDisplay.CheckCount = 0;
    TestDisplay.TestsPass = true;
    TestDisplay.OutputBuffer = "";

    TestDisplay.SetUp = function() { 
        this.FailCount = 0;
        this.TestCount = 0;
        this.CheckCount = 0;
        this.TestsPass = true;
        this.OutputBuffer = "";
    }

    TestDisplay.LogFatalError = function(TestCaseName) {
        this.FailCount ++;
        this.TestsPass = false;
        this.OutputBuffer += "<LI style=\"color: red; font-weight: bold\">";
        this.OutputBuffer += "Fatal Error in " + TestCaseName;
        this.OutputBuffer += "</LI>\n";
    }

    TestDisplay.IncrementTestCounter = function() { 
        this.TestCount++;
    }

    TestDisplay.LogPassingAssertion = function(CaseName, TestTitle) {
        //this.OutputBuffer += "<LI style=\"color: green\">";
        //this.OutputBuffer += TestTitle + ": Passed. " + "(" + CaseName + ")";
        //this.OutputBuffer += "</LI>\n";
        this.CheckCount++;
    }

    TestDisplay.LogFailingAssertion = function(CaseName, TestTitle) {
        this.FailCount ++;
        this.TestsPass = false;
        this.OutputBuffer += "<LI style=\"color: red\">";
        this.OutputBuffer += TestTitle + ": Failed. " + "(" + CaseName + ")";
        this.OutputBuffer += "</LI>\n";
    }

    TestDisplay.LogFailingEqualsAssertion = function(CaseName, Expected, Actual, TestTitle) {
        this.FailCount ++;
        this.TestsPass = false;
        this.OutputBuffer += "<LI style=\"color: red\">";
        this.OutputBuffer += TestTitle + ": Failed. " + "(" + CaseName + ")";
        this.OutputBuffer += "<ul>\n";
        this.OutputBuffer += "<li style=\"color: red\">";
        this.OutputBuffer += "Expected: " + escape(Expected);
        this.OutputBuffer += "</li>\n";
        this.OutputBuffer += "<li style=\"color: red\">";
        this.OutputBuffer += "Was :&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + escape(Actual);
        this.OutputBuffer += "</li>\n";
        this.OutputBuffer += "</ul>\n";
        this.OutputBuffer += "</LI>\n";
    }

    TestDisplay.Output = function() {
        //this.OutputBuffer += "<h2 style=\"color: green\">Pass</h2>\n";
        this.OutputBuffer += "<span style=\"color: green\">Test count: " + this.TestCount + 
            ", Check count: " + this.CheckCount + "</span>\n";
        if(!this.TestsPass) {
            this.OutputBuffer += "<h2 style=\"color: red\">Fail</h2>\n";
            this.OutputBuffer += "<span style=\"color: red\">Fail count: " + this.FailCount + "</span>\n";
        }
    
//        if(this.TestsPass){
//            this.OutputBuffer += "<h2 style=\"color: green\">Pass</h2>\n";
//            this.OutputBuffer += "<span style=\"color: green\">Test count: " + this.TestCount + 
//                ", Check count: " + this.CheckCount + "</span>\n";
//        } else {
//            this.OutputBuffer += "<h2 style=\"color: red\">Fail</h2>\n";
//        }
        return this.OutputBuffer;
    }
    return TestDisplay;
}

function newXmlTestDisplay() {
    var TestDisplay = new Object;
    TestDisplay.FailCount = 0;
    TestDisplay.TestCount = 0;
    TestDisplay.CheckCount = 0;
    TestDisplay.TestsPass = true;
    TestDisplay.OutputBuffer = "";

    TestDisplay.SetUp = function() { 
        this.FailCount = 0;
        this.TestCount = 0;
        this.CheckCount = 0;
        this.TestsPass = true;
        this.OutputBuffer = "";
    }

    TestDisplay.LogFatalError = function(TestCaseName) {
        this.FailCount ++;
        this.TestsPass = false;
        this.OutputBuffer += "<failure>";
        this.OutputBuffer += "<errorType>fatal</errorType><caseName>" + TestCaseName + "</caseName>";
        this.OutputBuffer += "</failure>";
    }

    TestDisplay.IncrementTestCounter = function() { 
        this.TestCount++;
    }

    TestDisplay.LogPassingAssertion = function(CaseName, TestTitle) {
        this.CheckCount++;
    }

    TestDisplay.LogFailingAssertion = function(CaseName, TestTitle) {
        this.FailCount ++;
        this.TestsPass = false;
        this.OutputBuffer += "<failure>";
        this.OutputBuffer += "<testTitle>" + TestTitle + "</testTitle><caseName>" + CaseName + "</caseName>";
        this.OutputBuffer += "</failure>";
    }

    TestDisplay.LogFailingEqualsAssertion = function(CaseName, Expected, Actual, TestTitle) {
        this.FailCount ++;
        this.TestsPass = false;
        this.OutputBuffer += "<failure>";
        this.OutputBuffer += "<testTitle>" + TestTitle + "</testTitle><caseName>" + CaseName + "</caseName>";
        this.OutputBuffer += "<expected>" + escape(Expected);
        this.OutputBuffer += "</expected>";
        this.OutputBuffer += "<actual>";
        this.OutputBuffer += escape(Actual) + "</actual>";
        this.OutputBuffer += "</failure>";
    }

    TestDisplay.Output = function() {
        if(this.TestsPass){
            this.OutputBuffer += "<Pass testCount=\"" + this.TestCount + "\" ";
            this.OutputBuffer += "checkCount=\"" + this.CheckCount + "\"/>";
        }
        return this.OutputBuffer;
    }
    return TestDisplay;
}

function newTest(testName) {
    var Test = new Object;
    Test.Name = testName;
    registry.AddTest(Test);
    
    Test.SetUp = function(TestDisplay) {
        this.TestDisplay = TestDisplay;
    }

    Test.Execute = function() {}

    Test.TearDown = function() {
	    this.TestDisplay.IncrementTestCounter();
    }

    Test.TestCount = function() {return 1;}

    Test.Assert = function(Test, Title) {
        if (Test) {
            this.TestDisplay.LogPassingAssertion(this.Name, Title);
        } else {
            this.TestDisplay.LogFailingAssertion(this.Name, Title);
        }
    }
    
    Test.AreEqual = function(Expected, Actual, Message) {
        if (Expected == Actual) {
            this.TestDisplay.LogPassingAssertion(this.Name, Message);
        } else {
            this.TestDisplay.LogFailingEqualsAssertion(this.Name, Expected, Actual, Message);
        }
    }
    
    Test.AreNotEqual = function (Expected, Actual, Message) {
        this.Assert((Expected != Actual), Message);
    }
    
    Test.Fail = function(Message) {
        this.Assert((false), Message);
    }
    return Test;
}

function TestRunner(Test, TestDisplay)
{
    Test.SetUp(TestDisplay);
    Test.Execute();
    Test.TearDown();
}

function DisplayAndRunAllTests()
{
    var thisTestDisplay = newTestDisplay();
    thisTestDisplay.SetUp();
    RunAllTests(thisTestDisplay);
    return thisTestDisplay.Output();
}

function DisplayAndRunAllTestsInXml()
{
    var thisTestDisplay = newXmlTestDisplay();
    thisTestDisplay.SetUp();
    RunAllTests(thisTestDisplay);
    return thisTestDisplay.Output();
}

var registry = newTestRegistry();
function newTestRegistry() {
    var TestRegistry = new Object;
    TestRegistry.registryArray = new Array();
    
    TestRegistry.AddTest = function(theTest) {
        this.registryArray[this.registryArray.length] = theTest;
    }
    return TestRegistry;
}

function RunAllTests(aTestDisplay)
{
    for (var i=0; i < registry.registryArray.length; i++)
    {
        TestRunner(registry.registryArray[i], aTestDisplay);
    }
}
