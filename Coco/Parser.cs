using SL_Compiler.SymbolList;
using System.Collections;
using SL_Compiler.Codegen;
using System.Collections.Generic;


using System;



public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _number = 2;
	public const int _charCon = 3;
	public const int maxT = 34;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public Tab tab;
Code code;
public byte[] ByteCode { get { return code.ByteCode; } }
readonly int MAX_VARS = 127;

bool IsBuiltIn(Operand x)
{
	if(x.obj == Tab.CHRObj || x.obj == Tab.ORDObj)
		return true;
	return false;
}
/*--------------------------------------------------------------------------*/


	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
		code = new Code(this);
		tab = new Tab(this);
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void SL() {
		Expect(4);
		tab.OpenScope(); int progStart = 0; code.PutJMP(ref progStart);
		while (la.kind == 8 || la.kind == 12) {
			Declaration();
		}
		Expect(5);
		code.Fixup(progStart);
		           
		StatSeq(null);
		Expect(6);
		Expect(7);
		tab.CloseScope(); code.RET(); 
	}

	void Declaration() {
		if (la.kind == 8) {
			VarDecl();
		} else if (la.kind == 12) {
			ProcDecl();
		} else SynErr(35);
	}

	void StatSeq(Obj method) {
		Statement(method);
		while (la.kind == 10) {
			Get();
			Statement(method);
		}
	}

	void VarDecl() {
		List<string> l = new List<string>();
		Expect(8);
		while (la.kind == 1) {
			l.Clear(); 
			IdList(l);
			Expect(9);
			Struct type; 
			Type(out type);
			foreach(string var in l) 
			{
			tab.Insert(var, Obj.Kind.Var, type);
			} 
			
			Expect(10);
		}
	}

	void ProcDecl() {
		Struct type = Tab.noType;
		Expect(12);
		Expect(1);
		string ident = t.val; Obj method = tab.Insert(ident, Obj.Kind.Meth, type); 
		tab.OpenScope();
		if (la.kind == 13) {
			Parameters(out type);
		}
		Expect(10);
		tab.SetReturnType(ident, type);
		
		while (la.kind == 8) {
			VarDecl();
		}
		method.adr = code.pc; 
		 method.nPars = tab.curScope.ParSize;
		method.locals = tab.curScope.locals;
		if (la.kind == 5) {
			Get();
			code.ENTER(tab.curScope.VarSizeInBytes); 
			StatSeq(method);
		}
		Expect(6);
		if(method.type == Tab.noType)
		{
		code.LEAVE();
		code.RET(method.nPars*4);
		}
		else
		code.TRAP();
		
		Expect(1);
		string endIdent = t.val; 
		if(!endIdent.Equals(ident))
		SemErr("Proc idents do not match");
		
		Expect(10);
		tab.CloseScope(); 
		Reg.FreeAllRegs();
		
	}

	void IdList(List<string> list) {
		Expect(1);
		list.Add(t.val); 
		while (la.kind == 11) {
			Get();
			Expect(1);
			list.Add(t.val); 
		}
		if(list.Count > MAX_VARS)
		SemErr("Too many vars"); 
	}

	void Type(out Struct type) {
		Expect(1);
		Obj obj = tab.Find(t.val);
		if(obj.kind != Obj.Kind.Type)
		SemErr("Type expected");
		type = obj.type;		
	}

	void Parameters(out Struct type) {
		type = Tab.noType;
		Expect(13);
		if (la.kind == 1 || la.kind == 8) {
			Param();
			while (la.kind == 10) {
				Get();
				Param();
			}
		}
		Expect(14);
		if (la.kind == 9) {
			Get();
			Type(out type);
		}
	}

	void Param() {
		List<string> l = new List<string>(); Struct type;
		if (la.kind == 8) {
			Get();
		}
		IdList(l);
		Expect(9);
		Type(out type);
		foreach(string var in l) 
		{
		tab.Insert(var, Obj.Kind.Par, type);
		} 
		
	}

	void Statement(Obj method) {
		Reg.FreeAllRegs(); 
		Operand x = null, y; 
		if (StartOf(1)) {
			if (la.kind == 1) {
				Get();
				Obj o = tab.Find(t.val);
				x = code.VarOperand(o);
				
				if (la.kind == 15) {
					Get();
					Expression(out y);
					if (!x.AssignableTo(y, this))
					return;
					code.Load(y);
					code.PutMOV(x, y);
					Reg.FreeAllRegs();
					
				} else if (IsBuiltIn(x)) {
					Expect(13);
					Expression(out x);
					Expect(14);
				} else if (la.kind == 13) {
					if(x.obj == Tab.CHRObj || x.obj == Tab.ORDObj)
					return;
					if(x.reg != Reg.none) //save EDI if predefined procedure gets called
					{
					Operand ediOp = new Operand();
					ediOp.reg = Reg.EDI;
					code.PUSH(ediOp);
					}
					
					ActParameters(x);
					code.CALL(x);
					if(x.reg != Reg.none) //restore EDI if predefined procedure gets called
					{
					Operand ediOp = new Operand();
					ediOp.reg = Reg.EDI;
					code.POP(ediOp);
					}
					Reg.FreeAllRegs();
					
				} else SynErr(36);
			} else if (la.kind == 16) {
				Get();
				Code.CompOp op = Code.CompOp.eq; int else_ = 0, end = 0;
				Condition(out op);
				code.PutJcc((int)op, Code.fjmp, Code.signed, ref else_); 
				Expect(17);
				StatSeq(method);
				while (la.kind == 18) {
					Get();
					code.PutJMP(ref end); code.Fixup(else_); else_ = 0; 
					Condition(out op);
					code.PutJcc((int)op, Code.fjmp, Code.signed, ref else_); 
					Expect(17);
					StatSeq(method);
				}
				if (la.kind == 6 || la.kind == 19) {
					if (la.kind == 19) {
						Get();
						code.PutJMP(ref end); code.Fixup(else_); 
						StatSeq(method);
					} else {
						code.Fixup(else_); 
					}
				}
				Expect(6);
				code.Fixup(end); 
			} else if (la.kind == 20) {
				Get();
				Code.CompOp op = Code.CompOp.eq; int start = code.pc; int end = 0; 
				Condition(out op);
				code.PutJcc((int)op, Code.fjmp, Code.signed, ref end); 
				Expect(21);
				StatSeq(method);
				Expect(6);
				code.PutJMP(ref start); 
				code.Fixup(end); 
			} else {
				Get();
				bool expressionFound = false; 
				if (StartOf(2)) {
					Expression(out x);
					if(method.type == Tab.noType)
					SemErr("void method must not return a value");
					if(method.type != x.type)
					SemErr("return type must match method type");
					expressionFound = true;
					code.Load(x);
					Operand eaxOpd = code.RegOpd(0); //load return value into eax
					code.PutMOV(eaxOpd, x);
					code.LEAVE();
					 code.RET(method.nPars*4);
					
				}
				if(method.type != Tab.noType && !expressionFound) //a return val is required but not found 
				SemErr("Return expression required");
				
			}
		}
	}

	void Expression(out Operand x) {
		Operand y; Dyadic.Ops op; bool neg = false;
		if (la.kind == 32 || la.kind == 33) {
			Addop(out neg);
		}
		Term(out x);
		if(neg)
		{
		if(x.type == Tab.intType)
		{
		if(x.kind == Operand.Kind.Con) 
		x.val = -x.val;
		else
		{
		code.Load(x);
		code.PutNEG(x);
		}
		}
		else
		SemErr("an error");
		}
		
		while (la.kind == 32 || la.kind == 33) {
			Addop(out neg);
			if(neg)
			op = Dyadic.Ops.Minus;
			else
			op = Dyadic.Ops.Plus;
			
			Term(out y);
			if (x.type == Tab.intType && y.type == Tab.intType) 
			code.GenOp(op, x, y);
			else 
			SemErr("an error");
			
		}
	}

	void ActParameters(Operand x) {
		Operand ap = new Operand(); 
		Expect(13);
		if(x.kind != Operand.Kind.Meth)
		SemErr("called object is not a method");
		List<Obj> fp = x.locals;
		int aPars = 0;
		
		if (StartOf(2)) {
			Expression(out ap);
			if(aPars + 1 > x.nPars)
			{
			SemErr("Number of actual and formal parameters does not match");
			return;
			}
			if(ap.type != fp[aPars++].type)
			SemErr("parameter type mismatch");
			code.Load(ap); code.PUSH(ap); 
			Reg.FreeReg(ap.reg);
			while (la.kind == 11) {
				Get();
				Expression(out ap);
				if(aPars + 1> x.nPars)
				{
				SemErr("Number of actual and formal parameters does not match");
				return;
				}
				if(ap.type != fp[aPars++].type)
				SemErr("parameter type mismatch"); 
				
				code.Load(ap); code.PUSH(ap); 
				Reg.FreeReg(ap.reg);
			}
		}
		if(aPars != x.nPars)
		SemErr("Number of actual and formal parameters does not match");
		
		Expect(14);
	}

	void Condition(out Code.CompOp op) {
		Operand x; Operand y; op = Code.CompOp.eq;
		Expression(out x);
		Relop(out op);
		Expression(out y);
		code.Load(x);
		code.Load(y);
		code.GenOp(Dyadic.Ops.Compare, x, y);
		
	}

	void Relop(out Code.CompOp op) {
		op = Code.CompOp.eq; 
		switch (la.kind) {
		case 26: {
			Get();
			op = Code.CompOp.eq; 
			break;
		}
		case 27: {
			Get();
			op = Code.CompOp.neq; 
			break;
		}
		case 28: {
			Get();
			op = Code.CompOp.lt; 
			break;
		}
		case 29: {
			Get();
			op = Code.CompOp.gt; 
			break;
		}
		case 30: {
			Get();
			op = Code.CompOp.geq; 
			break;
		}
		case 31: {
			Get();
			op = Code.CompOp.leq; 
			break;
		}
		default: SynErr(37); break;
		}
	}

	void Addop(out bool neg) {
		neg = false; 
		if (la.kind == 32) {
			Get();
			neg = false; 
		} else if (la.kind == 33) {
			Get();
			neg = true; 
		} else SynErr(38);
	}

	void Term(out Operand x) {
		Operand y; Dyadic.Ops op;
		Factor(out x);
		while (la.kind == 23 || la.kind == 24 || la.kind == 25) {
			if (la.kind == 23) {
				Get();
				op = Dyadic.Ops.Mult; 
			} else if (la.kind == 24) {
				Get();
				op = Dyadic.Ops.Div;  
			} else {
				Get();
				op = Dyadic.Ops.Mod;  
			}
			Factor(out y);
			if(x.type == Tab.intType && y.type == Tab.intType)
			code.GenOp(op, x, y);
			else
			SemErr("error");
			
		}
	}

	void Factor(out Operand x) {
		x = new Operand(); Operand ap = new Operand();
		if (la.kind == 1) {
			Get();
			Obj o = tab.Find(t.val);
			x = code.VarOperand(o);
			if(x.type.kind == Struct.Kind.None)
			SemErr("Invalid call of void method");
			
			if (la.kind == 13) {
				if (IsBuiltIn(x)) {
					Expect(13);
					Expression(out ap);
					if(x.obj == Tab.CHRObj && ap.type != Tab.intType)
					SemErr("Parameter type for CHR is not correct");
					if(x.obj == Tab.ORDObj && ap.type != Tab.charType)
					SemErr("Parameter type for CHR is not correct");
					if(x.obj == Tab.CHRObj)
					ap.type = Tab.charType;
					else if(x.obj == Tab.ORDObj)
					ap.type = Tab.intType;
					else
					SemErr("error");
					x = ap;
					
					Expect(14);
				} else {
					ActParameters(x);
					code.CALL(x); 
				}
			}
		} else if (la.kind == 2) {
			Get();
			x = code.ConOpd(Convert.ToInt32(t.val)); 
		} else if (la.kind == 3) {
			Get();
			char s = t.val[1]; x = code.ConOpd(s); x.type = Tab.charType;
		} else if (la.kind == 13) {
			Get();
			Expression(out x);
			Expect(14);
		} else SynErr(39);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		SL();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text
	public List<string> ErrorList = new List<string>();
	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "number expected"; break;
			case 3: s = "charCon expected"; break;
			case 4: s = "\"PROGRAM\" expected"; break;
			case 5: s = "\"BEGIN\" expected"; break;
			case 6: s = "\"END\" expected"; break;
			case 7: s = "\".\" expected"; break;
			case 8: s = "\"VAR\" expected"; break;
			case 9: s = "\":\" expected"; break;
			case 10: s = "\";\" expected"; break;
			case 11: s = "\",\" expected"; break;
			case 12: s = "\"PROCEDURE\" expected"; break;
			case 13: s = "\"(\" expected"; break;
			case 14: s = "\")\" expected"; break;
			case 15: s = "\":=\" expected"; break;
			case 16: s = "\"IF\" expected"; break;
			case 17: s = "\"THEN\" expected"; break;
			case 18: s = "\"ELSIF\" expected"; break;
			case 19: s = "\"ELSE\" expected"; break;
			case 20: s = "\"WHILE\" expected"; break;
			case 21: s = "\"DO\" expected"; break;
			case 22: s = "\"RETURN\" expected"; break;
			case 23: s = "\"*\" expected"; break;
			case 24: s = "\"/\" expected"; break;
			case 25: s = "\"%\" expected"; break;
			case 26: s = "\"=\" expected"; break;
			case 27: s = "\"#\" expected"; break;
			case 28: s = "\"<\" expected"; break;
			case 29: s = "\">\" expected"; break;
			case 30: s = "\">=\" expected"; break;
			case 31: s = "\"<=\" expected"; break;
			case 32: s = "\"+\" expected"; break;
			case 33: s = "\"-\" expected"; break;
			case 34: s = "??? expected"; break;
			case 35: s = "invalid Declaration"; break;
			case 36: s = "invalid Statement"; break;
			case 37: s = "invalid Relop"; break;
			case 38: s = "invalid Addop"; break;
			case 39: s = "invalid Factor"; break;

			default: s = "error " + n; break;
		}
		AddToList(errMsgFormat, line, col, s);
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		AddToList(errMsgFormat, line, col, s);
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		ErrorList.Add(s);
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
	
	private void AddToList(string errMsgFormat, int line, int col, string s) {
		string newS = String.Format(errMsgFormat, line, col, s);
		ErrorList.Add(newS);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
