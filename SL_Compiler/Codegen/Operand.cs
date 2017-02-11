using SL_Compiler.SymbolList;
using System;
using System.Collections.Generic;

namespace SL_Compiler.Codegen
{
    class Operand
    {
        public enum Kind
        {
            Con, Abs , Reg, RegRel, Meth, None
        }
        public Kind kind; // Con, Abs, Reg, RegRel, Proc
        public Struct type = Tab.noType;
        public int val; // Con: constant value
        public int reg = Reg.none; // Reg, RegRel: register
        public int adr; // Abs: address; RegRel: offset
        public int inx; // Abs, RegRel: index register if not none
        public int scale; // Abs, RegRel: scale factor if inx != non
        public Obj obj; // Only for Meth: Method object from the symbol table.

        public int nPars = 0; // meth only: number of parameters
        public List<Obj> locals = new List<Obj>(); // meth only: list of parameters and declared variables

        public bool AssignableTo(Operand y, Parser p)
        {
            if (type != y.type) { p.SemErr("Members are not compatible"); return false; }
            if (type.size > 4) { p.SemErr("Illegal type"); return false; }
            if (this.kind != Kind.Abs && this.kind != Kind.Reg && this.kind != Kind.RegRel)
            {
                p.SemErr("Left operand is not a variable");
                return false;
            }
            return true;
        }
    }
}
