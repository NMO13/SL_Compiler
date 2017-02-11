using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SL_Compiler_Tests_Builder
{
    [TestClass]
    public class SymTabTests
    {
        [TestMethod]
        public void DoubleDeclLocal()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 3 col 13: i already defined");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\DoubleDeclLocal.sl");
        }

        [TestMethod]
        public void DoubleDeclMeth()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 4 col 12: foo already defined");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\DoubleDeclMeth.sl");
        }

        [TestMethod]
        public void DoubleDeclVar()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 2 col 10: i already defined");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\DoubleDeclVar.sl");
        }

        [TestMethod]
        public void TooManyVars()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 3 col 501: Too many vars");
            TestSupport.ExpectError("-- line 3 col 505: i already defined");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\TooManyVars.sl");
        }

        [TestMethod]
        public void TooManyVars2()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 2 col 500: Too many vars");
            TestSupport.ExpectError("-- line 2 col 504: i already defined");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\TooManyVars2.sl");
        }

        [TestMethod]
        public void SameName()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 3 col 10: i already defined");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\SameName.sl");
        }

        [TestMethod]
        public void UndefNameType()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 2 col 9: UNKNOWN not found");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\UndefNameType.sl");
        }

        [TestMethod]
        public void WrongMethDecl1()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 2 col 20: PROG not found");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\WrongMethDecl1.sl");
        }

        [TestMethod]
        public void MethDecl()
        {
            TestSupport.Init();
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\MethDecl.sl");
        }

        [TestMethod]
        public void NoType()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 3 col 9: Type expected");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\NoType.sl");
        }

        [TestMethod]
        public void IllegalMethodStart1()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 4 col 2: \"END\" expected");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\IllegalMethodStart1.sl");
        }

        [TestMethod]
        public void IllegalMethodStart2()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 4 col 2: \"END\" expected");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\IllegalMethodStart2.sl");
        }

        [TestMethod]
        public void IllegalStatement()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 4 col 2: \"END\" expected");
            TestSupport.ExpectError("-- line 6 col 2: \"BEGIN\" expected");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\IllegalStatement.sl");
        }

        [TestMethod]
        public void IncorrectIdent()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 4 col 6: Proc idents do not match");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\IncorrectIdent.sl");
        }

        [TestMethod]
        public void InvalidBody()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 3 col 1: \"END\" expected");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\InvalidBody.sl");
        }

        [TestMethod]
        public void RecoverDecl()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 2 col 7: ident expected");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\RecoverDecl.sl");
        }

        [TestMethod]
        public void SameName2()
        {
            TestSupport.Init();
            TestSupport.ExpectError("-- line 3 col 9: i already defined");
            TestSupport.Parse(@"C:\Martin\Uni\Übersetzerbau2\SL-Compiler\SL_Compiler\Tests\SymTab\SameName2.sl");
        }
    }
}
