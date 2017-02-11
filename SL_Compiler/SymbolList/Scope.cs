using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_Compiler.SymbolList
{
    public class Scope
    {
        public Scope outer;
        private int varSizeInBytes = 0;
        private int parSize = 0;
        public List<Obj> locals = new List<Obj>();

        public int VarSizeInBytes { get { return varSizeInBytes; } }
        public int ParSize { get { return parSize; } }
        public Scope Outer { get { return outer; } }
        public List<Obj> Params = new List<Obj>();

        internal Obj FindLocal(string name)
        {
            if (locals.Count == 0)
                return null;
            return locals.Find(x => x.name == name);
        }

        internal Obj FindGlobal(String name)
        {
            Obj res = FindLocal(name);
            if (res == null && outer != null)
            {
                res = outer.FindGlobal(name);
            }
            return res;
        }

        internal Scope(Scope outer)
        {
            this.outer = outer;
        }

        internal void Insert(Obj o)
        {
            locals.Add(o);
            if (o.kind == Obj.Kind.Var)
            {
                if (o.type == Tab.intType)
                    varSizeInBytes += 4;
                else // chartype
                    varSizeInBytes++;
            }
            else if (o.kind == Obj.Kind.Par)
                parSize++;
        }
    }
}
