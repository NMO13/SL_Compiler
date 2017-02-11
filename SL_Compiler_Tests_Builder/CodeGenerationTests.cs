using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SL_Compiler_Tests_Builder
{
    [TestClass]
    public class CodeGenerationTests
    {
        [TestMethod]
        public void GlobalAssignment()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\GlobalAssignment.sl");
        }

        [TestMethod]
        public void UndefNameMeth()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 3 col 1: Method not found");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\UndefNameMeth.sl");
        }

        [TestMethod]
        public void GlobalAssignment2()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\GlobalAssignment2.sl");
        }

        [TestMethod]
        public void LocalAssignment()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\LocalAssignment.sl");
        }

        [TestMethod]
        public void SimpleAdd()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\SimpleAdd.sl");
        }

        [TestMethod]
        public void SimpleMeth()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\SimpleMeth.sl");
        }

        [TestMethod]
        public void SimpleMeth2()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\SimpleMeth2.sl");
        }

        [TestMethod]
        public void PutTest()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\PutTest.sl");
        }

        [TestMethod]
        public void Param1()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\Param1.sl");
        }

        [TestMethod]
        public void ManyParams()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\ManyParams.sl");
        }

        [TestMethod]
        public void Locals()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\Locals.sl");
        }

        [TestMethod]
        public void WrongMethAssign1()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 8 col 9: Left operand is not a variable");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\WrongMethAssign1.sl");
        }

        [TestMethod]
        public void WrongMethAssign2()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 8 col 10: called object is not a method");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\WrongMethAssign2.sl");
        }

        [TestMethod]
        public void WrongMethAssign3()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 5 col 10: void method must not return a value");
            TestSupport.ExpectError("-- line 8 col 7: Invalid call of void method");
            TestSupport.ExpectError("-- line 8 col 11: Members are not compatible");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\WrongMethAssign3.sl");
        }

        [TestMethod]
        public void WrongMethAssign4()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 8 col 11: Members are not compatible");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\WrongMethAssign4.sl");
        }

        [TestMethod]
        public void WrongReturn()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 5 col 10: return type must match method type");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\WrongReturn.sl");
        }

        [TestMethod]
        public void NoReturn()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 5 col 3: Return expression required");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\NoReturn.sl");
        }

        [TestMethod]
        public void NoReturn2()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\NoReturn2.sl");
        }

        [TestMethod]
        public void WrongReturn2()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 5 col 10: void method must not return a value");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\WrongReturn2.sl");
        }

        [TestMethod]
        public void WrongParamType()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 7 col 6: parameter type mismatch");
            TestSupport.ExpectError("-- line 7 col 9: parameter type mismatch");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\WrongParamType.sl");
        }

        [TestMethod]
        public void WrongNumberActParameters()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 8 col 11: Number of actual and formal parameters does not match");
            TestSupport.ExpectError("-- line 9 col 17: Number of actual and formal parameters does not match");

            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\WrongNumberActParameters.sl");
        }

        [TestMethod]
        public void SimpleExpression1()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\SimpleExpression1.sl");
        }

        [TestMethod]
        public void SimpleExpression2()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\SimpleExpression2.sl");
        }

        [TestMethod]
        public void SimpleExpression3()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\SimpleExpression3.sl");
        }

        [TestMethod]
        public void SimpleExpression4()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\SimpleExpression4.sl");
        }

        [TestMethod]
        public void Branch1()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\Branch1.sl");
        }

        [TestMethod]
        public void Branch2()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\Branch2.sl");
        }

        [TestMethod]
        public void Branch3()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\Branch3.sl");
        }

        [TestMethod]
        public void While1()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\While1.sl");
        }

        [TestMethod]
        public void ORDCHRTest()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\ORDCHRTest.sl");
        }

        [TestMethod]
        public void BiggerExample()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\BiggerExample.sl");
        }

        [TestMethod]
        public void Faculty()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\Faculty.sl");
        }

        [TestMethod]
        public void Faculty1()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\CodeGeneration\Faculty1.sl");
        }
    }
}
