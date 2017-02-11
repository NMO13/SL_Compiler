using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_Compiler.SymbolList
{
    public class Struct
    {
        public enum Kind
        {
            None, Int, Char
        }
        public Kind kind; // None, Int, Char, ...
        public int size = 0;

        public Struct(Kind kind, int size)
        {
            this.kind = kind;
            this.size = size;
        }
    }
}
