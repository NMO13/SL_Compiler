using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_Compiler.SymbolList
{
    public class Tab
    {
        // Universe is at level -2!
        public static readonly Struct noType = new Struct(Struct.Kind.None, 0);
        public static readonly Struct intType = new Struct(Struct.Kind.Int, 4);
        public static readonly Struct charType = new Struct(Struct.Kind.Char, 1);
        static Obj noObj;
        public static Obj putObj, putLnObj, CHRObj, ORDObj, assertObj;
        public Scope curScope;

        public int curLevel;
        private static Parser m_Parser;
        public Tab(Parser p)
        {
            curLevel = -2;
            m_Parser = p;
            OpenScope();
            Insert("INTEGER", Obj.Kind.Type, intType);
            Insert("CHAR", Obj.Kind.Type, charType);
            Insert("Put", Obj.Kind.Meth, noType);
            Insert("PutLn", Obj.Kind.Meth, noType);
            Insert("ORD", Obj.Kind.Meth, intType);
            Insert("CHR", Obj.Kind.Meth, charType);
            Insert("Assert", Obj.Kind.Meth, noType);

            putObj = curScope.FindLocal("Put");
            putObj.nPars = 1;
            putObj.adr = 0;
            Obj o = new Obj("expr", Obj.Kind.Var, charType);
            putObj.locals.Add(o);

            putLnObj = curScope.FindLocal("PutLn");
            putLnObj.adr = 4;

            ORDObj = curScope.FindLocal("ORD");
            ORDObj.nPars = 1;
            o = new Obj("ch", Obj.Kind.Var, charType);
            ORDObj.locals.Add(o);

            CHRObj = curScope.FindLocal("CHR");
            CHRObj.nPars = 1;
            o = new Obj("i", Obj.Kind.Var, intType);
            CHRObj.locals.Add(o);

            assertObj = curScope.FindLocal("Assert");
            assertObj.nPars = 2;
            assertObj.adr = 8;
            o = new Obj("exp", Obj.Kind.Var, intType);
            assertObj.locals.Add(o);
            o = new Obj("act", Obj.Kind.Var, intType);
            assertObj.locals.Add(o);


            noObj = new Obj("noObj", Obj.Kind.Var, noType);
        }

        public Obj Insert(string name, Obj.Kind kind, Struct type)
        {
            Obj obj = curScope.FindLocal(name);
            if(obj != null)
                m_Parser.SemErr(name + " already defined");
             
            obj = new Obj(name, kind, type);
            if (kind == Obj.Kind.Par)
            {
                obj.adr = 8; // above EBP is dynamic link and return address
                obj.level = curLevel;
                curScope.Params.ForEach(x => x.adr += 4);
                curScope.Params.Add(obj);

            }
            if (kind == Obj.Kind.Var)
            {
                obj.adr = curScope.VarSizeInBytes;
                obj.level = curLevel;
                if (curLevel > 0) // for local method vars
                    obj.adr = -obj.adr - 4; // EBP points to dynamic link
            }
            curScope.Insert(obj);
            return obj;
        }

        public void SetReturnType(string name, Struct type)
        {
            Scope scope = curScope.outer;
            Obj obj = scope.FindLocal(name);
            if (obj == null)
                m_Parser.SemErr("Method not found");
            obj.type = type;
        }

        public Obj Find (string name) 
        {
            Obj o = curScope.FindGlobal(name);
            if (o == null)
            {
                m_Parser.SemErr(name + " not found");
                o = noObj;
            }

            return o;
        }

        public  Obj FindField (string name, Struct recType)
        {
            return noObj;
        }

        public void OpenScope()
        { // in class Tab
		    Scope s = new Scope(curScope);
		    curScope = s;
		    curLevel++;
	    }
	
	    public void CloseScope() { // in class Tab
		    curScope = curScope.Outer;
		    curLevel--;
	    }
    }
}
