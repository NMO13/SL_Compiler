using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace SL_Compiler_Tests_Builder
{
    class TestSupport
    {
        private static List<String> expectedErrors = null;
        public static void Init()
        {
            expectedErrors = new List<string>();
        }

        public static void Parse(string filename)
        {
            Scanner scanner = new Scanner(filename);
            Parser parser = new Parser(scanner);
            parser.Parse();
            PrintErrors(expectedErrors, parser.errors.ErrorList);

            if (parser.errors.count == 0)
            {
                string dir = System.IO.Path.GetDirectoryName(filename);
                string combined = System.IO.Path.Combine(dir, System.IO.Path.GetFileNameWithoutExtension(filename) + ".obj");
                File.WriteAllBytes(combined, parser.ByteCode);
            }
        }

        private static void PrintErrors(List<string> expectedErrors, List<string> list)
        {
            Assert.AreEqual(expectedErrors.Count, list.Count);
            for (int i = 0; i < expectedErrors.Count; i++)
            {
                string s1 = expectedErrors[i];
                string s2 = list[i];
                Assert.AreEqual(s1, s2);
            }
        }

        public static void ExpectError(string msg)
        {
            expectedErrors.Add(msg);
        }
    }

}
