using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_Compiler.Codegen
{
    class Dyadic
    {
        public enum Ops { Plus, Minus, Mult, Div, Mod, Compare }

        public enum ALUOps
        {
            ADD = 0x0, ADC = 0x10, SUB = 0x28, SBB = 0x18, AND = 0x20,
            OR = 0x08, XOR = 0x30, CMP = 0x38
        }
        //public enum Arithmetic
    }
}
