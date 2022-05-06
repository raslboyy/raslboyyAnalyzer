using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = AnalyzerTemplate.Test.CSharpCodeFixVerifier<
    AnalyzerTemplate.AnalyzerTemplateAnalyzer,
    AnalyzerTemplate.AnalyzerTemplateCodeFixProvider>;

namespace AnalyzerTemplate.Test
{
    [TestClass]
    public class AnalyzerTemplateUnitTest
    {

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task TestMethod2()
        {
            var test = @"
using System;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = 0;
            if (a == 1) { a++; }
            else if (a == 2) { a++; }
            else {
                [|if|] (a == 0) { a++; }
                else { a++; }
            }
        }
    }
}";

            var fixtest = @"
using System;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = 0;
            if (a == 1) 
            {
                a++;
            }
            else if (a == 2) 
            {
                a++;
            }
            else if (a == 0)    
            {
                a++;
            }
            else 
            {
                a++;
            }
        }
    }
}";

            //var expected = VerifyCS.Diagnostic("AnalyzerTemplate").WithLocation(0).WithArguments("TypeName");
            await VerifyCS.VerifyCodeFixAsync(test, fixtest);
        }
    }
}
