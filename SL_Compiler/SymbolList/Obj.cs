using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_Compiler.SymbolList
{
    public class Obj
    {
        public enum Kind
        {
            Con, Par, Var, Type, Meth, Prog
        }
        public string name;
        public Kind kind; // Con, Type, Var, ...
        public Struct type;
        public int adr; // Var, Valpar, Varpar, Field, Proc: address
        public int level; // Var: declaration level (0 = global)
        public int nPars; // for Meth
        public List<Obj> locals = new List<Obj>();

        public Obj(string name, Kind kind, Struct type)
        {
            this.kind = kind;
            this.name = name;
            this.type = type;
        }
    }
}
