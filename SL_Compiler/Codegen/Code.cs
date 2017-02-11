using SL_Compiler.SymbolList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_Compiler.Codegen
{
    class Code 
    { 
        public Code(Parser p)
        {
            m_Parser = p;
            Reg.m_Parser = m_Parser;
        }

        private Parser m_Parser;
        private byte[] code = new byte[3000];
        public int pc = 0;
        private readonly int numStdMeths = 3;

        public byte[] ByteCode { get { return code; } }
        public void Put (int x) 
        {
            code[pc++] = (byte)x;
        }
        public void Put2 (int x) 
        {
            Put(x); Put(x >> 8); // little endian order
        }
        public void Put4 (int x) 
        {
            Put2(x); Put2(x >> 16);
        }
        public void PutConst (int size, int x) 
        {
            if (size == 1) Put(x);
            else if (size == 2) Put2(x);
            else Put4(x);
        }

        public void Put4(int addr, int x)
        {
            Put2(addr, x); Put2(addr+2, x >> 16);
        }

        public void Put2(int addr, int x)
        {
            Put(addr, x); Put(addr + 1, x >> 8);
        }

        public void Put(int addr, int x)
        {
            code[addr] = (byte)x;
        }

        public int Get4(int addr)
        {
            int x = 0;
            x |= code[addr + 3];
            x = x << 8;
            x |= code[addr + 2];
            x = x << 8;
            x |= code[addr + 1];
            x = x << 8;
            x |= code[addr];
            return x;
        }

        void PutOpd (int reg, Operand x)
        {
            //----- put modrm byte
            int rm = 0, mod = 0;
            if (x.inx == Reg.none)
            {
                switch (x.kind) {
                    case Operand.Kind.Abs:
                        mod = 0; rm = 5; break;
                    case Operand.Kind.Reg:
                        mod = 3; rm = x.reg; break;
                    case Operand.Kind.RegRel:
                        Debug.Assert(x.reg != Reg.ESP);
                        mod = Mod(x.adr); rm = x.reg; break;
                }
            } 
            else { // indexed
                Debug.Assert(x.inx != Reg.ESP && (x.kind == Operand.Kind.Abs || x.kind == Operand.Kind.RegRel));
                rm = 4;
                if (x.kind == Operand.Kind.Abs)
                {
                    mod = 0; x.reg = Reg.EBP;
                } 
                else 
                {
                    mod = Mod(x.adr);
                }
            }
            Put((mod << 6) + (reg << 3) + rm);
            //----- put SIB byte
            if (x.inx != Reg.none) 
                Put((x.scale << 6) + (x.inx << 3) + x.reg);
            //----- put displacement
            if (mod == 0 && rm == 5) 
                PutConst(4, x.adr); // absolute address
            else if (mod == 0 && rm == 4 && x.reg == Reg.EBP) 
                PutConst(4, x.adr); // absolute indexed
            else if (mod == 1) 
                PutConst(1, x.adr);
            else if (mod == 2) 
                PutConst(4, x.adr);
        }

        int Mod(int n)
        {
            if (n == 0) return 0;
            else if (n >= -128 && n < 127) return 1;
            else return 2;
        }

        //////// Create Operands

        public Operand VarOperand(Obj obj)
        {
            // obj.kind in {Var, VarPar}
            Operand x = new Operand();
            if (obj.kind == Obj.Kind.Meth)
            {
                if (obj == Tab.putObj || obj == Tab.putLnObj || obj == Tab.assertObj)
                {
                    x.kind = Operand.Kind.RegRel; x.reg = Reg.EDI;
                    x.obj = obj;
                    x.adr = obj.adr; x.inx = Reg.none;
                    x.type = Tab.intType; // move a 32bit address
                    Load(x); // move method address into a register 
                    x.nPars = obj.nPars;
                    x.kind = Operand.Kind.Meth;
                    x.locals = obj.locals;
                    return x;
                }
                x.kind = Operand.Kind.Meth;
                x.adr = obj.adr;
                x.type = obj.type;
                x.nPars = obj.nPars;
                x.locals = obj.locals;
                x.reg = Reg.none;
                x.obj = obj;
                return x;
            }

            if (obj.level == 0)
            { // global
                //x.kind = Operand.Abs; x.reg = Reg.none; x.adr = obj.adr+8; x.inx = Reg.none;
                x.kind = Operand.Kind.RegRel; x.reg = Reg.EDI; 
                x.adr = obj.adr + numStdMeths*4; x.inx = Reg.none;
            }
            else
            { // local
                x.kind = Operand.Kind.RegRel; x.reg = Base(m_Parser.tab.curLevel - obj.level);
                x.adr = obj.adr; x.inx = Reg.none;
            }
            //if (obj.kind == Obj.Kind.Var) { x.type = Tab.intType; Load(x); x.kind = Operand.RegRel; }
            x.type = obj.type;
            return x;
        }

        int Base(int d) //todo test this
        { // return the base register d levels up the static link chain
            if (d == 0) return Reg.EBP;
            Operand r = new Operand();
            r.kind = Operand.Kind.RegRel; r.reg = Reg.EBP; r.adr = 8; r.inx = Reg.none; r.type = Tab.intType;
            while (d > 0)
            {
                Load(r); r.kind = Operand.Kind.RegRel; r.adr = 8;
                d--;
            }
            return r.reg;
        }

        public void Load (Operand x) {
            if (x.kind == Operand.Kind.Reg) return;
            Operand r = null; // destination register
            if (x.kind == Operand.Kind.RegRel)
            {
                if (x.reg == Reg.EBP || x.reg == Reg.EDI)
                    r = RegOpd(Reg.any);
                else
                    r = RegOpd(x.reg);
                r.type = x.type; PutMOV(r, x);
                Reg.FreeReg(x.inx);
            }
            else if (x.kind == Operand.Kind.Con || x.kind == Operand.Kind.Abs)
            {
                r = RegOpd(Reg.any); r.type = x.type;
                PutMOV(r, x);
                Reg.FreeReg(x.inx);
            }
            else if (x.kind == Operand.Kind.Meth) // return value is in eax
            {
                r = RegOpd(Reg.EAX); // not needed actually
            }
            else
                m_Parser.SemErr("load command not correct");
            x.kind = Operand.Kind.Reg; x.reg = r.reg; x.adr = 0; x.inx = Reg.none;
        }

        public Operand RegOpd(int reg)
        {
            string error;
            Operand x = new Operand();
            x.kind = Operand.Kind.Reg; x.type = Tab.intType; x.adr = 0; x.inx = Reg.none;
            if (reg == Reg.any)
                x.reg = Reg.GetReg(out error);
            else
            {
                Reg.GetReg(reg, out error);
                x.reg = reg;
            }
            if (error != null)
                m_Parser.SemErr(error);
            return x;
        }

        public Operand RegRelOpd(int reg, int adr)
        {
            Operand x = new Operand();
            x.kind = Operand.Kind.RegRel; x.reg = reg; x.adr = adr; x.inx = Reg.none;
            return x;
        }

        public void PutMOV(Operand x, Operand y)
        { // MOV x, y
           // Debug.Assert(x.type.size == y.type.size);
            int sizeFlag = PutPrefix(x);
            if (y.kind == Operand.Kind.Con)
            {
                if (x.kind == Operand.Kind.Reg)
                { // r := imm
                    Put(0xB0 + (sizeFlag << 3) + x.reg);
                    PutConst(y.type.size, y.val);
                }
                else
                { // m := imm
                    Put(0xC6 + sizeFlag); PutOpd(0, x);
                    PutConst(y.type.size, y.val);
                }
            }
            else if (x.kind == Operand.Kind.Reg)
            { // r := rm
                Put(0x8A + sizeFlag); PutOpd(x.reg, y);
            }
            else if (y.kind == Operand.Kind.Reg)
            { // rm := r
                Put(0x88 + sizeFlag); PutOpd(y.reg, x);
            }
            else m_Parser.SemErr("some error");
        }

        int PutPrefix(Operand x) 
        {
            if (x.type.size == 1) 
                return 0;
            else if (x.type.size == 2) 
            { 
                Put(0x66); 
                return 1; 
            }
            else if (x.type.size == 4) 
                return 1;
            else 
            {
                m_Parser.SemErr("some error");
                return 0; 
            }
        }

        public void PutNEG(Operand x)
        {
            int sizeFlag = PutPrefix(x);
            Put(0xF6 + sizeFlag); PutOpd(0x3, x);
        }

        public void GenOp(Dyadic.Ops op, Operand x, Operand y)
        {
            if (x.kind == Operand.Kind.Con && y.kind == Operand.Kind.Con)
            {
                switch (op)
                {
                    case Dyadic.Ops.Plus: x.val += y.val; break;
                    case Dyadic.Ops.Minus: x.val -= y.val; break;
                    case Dyadic.Ops.Mult: x.val *= y.val; break;
                    case Dyadic.Ops.Div: x.val /= y.val; break;
                }
            }
            else
            {
                Load(x);
                switch (op)
                {
                    case Dyadic.Ops.Plus: PutDyadic(Dyadic.ALUOps.ADD, x, y); FreeOpd(y); break;
                    case Dyadic.Ops.Minus: PutDyadic(Dyadic.ALUOps.SUB, x, y); FreeOpd(y); break;
                    case Dyadic.Ops.Compare: PutDyadic(Dyadic.ALUOps.CMP, x, y); FreeOpd(x); FreeOpd(y); break;
                    case Dyadic.Ops.Mult: PutMUL(x, y); FreeOpd(y); break;
                    case Dyadic.Ops.Div: PutDIV(x, y); Reg.FreeReg(y.reg); break;
                    case Dyadic.Ops.Mod: PutMOD(x, y); Reg.FreeReg(y.reg); break;
                }
            }
        }

        private void PutDIV(Operand x, Operand y)
        {
            if (x.type == y.type && y.type.kind == Struct.Kind.Int)
            {
                if (x.reg == Reg.EDX)
                {
                    Operand ecxOpd = RegOpd(Reg.ECX);
                    PutMOV(ecxOpd, x);
                    FreeOpd(x);
                    x.reg = Reg.ECX;
                }
                Operand edxOpd = RegOpd(Reg.EDX);
                Operand eaxOpd = RegOpd(Reg.EAX);
                PutDyadic(Dyadic.ALUOps.XOR, edxOpd, edxOpd); // set edx to 0
                PutMOV(eaxOpd, x); // move second operand into eax
                Load(y);
                int sf = PutPrefix(y);
                Put(0xF6 + sf); PutOpd(0x6, y); // divide
                PutMOV(x, eaxOpd); // move result back into register
                FreeOpd(eaxOpd);
                FreeOpd(edxOpd);

            }
            else
                m_Parser.SemErr("Both operands have to be ints");
        }

        private void PutMOD(Operand x, Operand y)
        {
            if (x.type == y.type && y.type.kind == Struct.Kind.Int)
            {
                if (x.reg == Reg.EDX)
                {
                    Operand ecxOpd = RegOpd(Reg.ECX);
                    PutMOV(ecxOpd, x);
                    FreeOpd(x);
                    x.reg = Reg.ECX;
                }
                Operand edxOpd = RegOpd(Reg.EDX);
                Operand eaxOpd = RegOpd(Reg.EAX);
                PutDyadic(Dyadic.ALUOps.XOR, edxOpd, edxOpd); // set edx to 0
                PutMOV(eaxOpd, x); // move second operand into eax
                Load(y);
                int sf = PutPrefix(y);
                Put(0xF6 + sf); PutOpd(0x6, y); // divide
                PutMOV(x, edxOpd); // move result back into register
                FreeOpd(eaxOpd);
                FreeOpd(edxOpd);

            }
            else
                m_Parser.SemErr("Both operands have to be ints");
        }

        private void PutMUL(Operand x, Operand y)
        {
            if (x.type == y.type && y.type.kind == Struct.Kind.Int)
            {
                // todo not correct: x * foo(x) is not working that way
                // because Load(y) = Load(foo) needs eax for return value which is already...
                // ... allocated by eaxOpd.
                // one solution: Load(y) first
                // if(x.reg != eax): push current eax value on the stack
                // move x value into eax
                // apply multiplication
                // move result back into old register
                // pop into eax
                // for edx: if edx is already reserved: push on stack, multiply pop into edx back
                if (x.reg == Reg.EDX)
                {
                    Operand ecxOpd = RegOpd(Reg.ECX);
                    PutMOV(ecxOpd, x);
                    FreeOpd(x);
                    x.reg = Reg.ECX;
                }
                Operand eaxOpd = RegOpd(Reg.EAX);
                Operand edxOpd = RegOpd(Reg.EDX);
                PutMOV(eaxOpd, x); // move second operand into eax
                Load(y);
                int sf = PutPrefix(y);
                Put(0xF6 + sf); PutOpd(0x4, y); // multiply (edx will be overridden)
                PutMOV(x, eaxOpd); // move result back into register
                FreeOpd(eaxOpd);
                FreeOpd(edxOpd);
            }
            else
                m_Parser.SemErr("Both operands have to be ints");

        }


        void PutDyadic (Dyadic.ALUOps op, Operand x, Operand y) 
        {
            int sf = PutPrefix(x);
            if (x.kind == Operand.Kind.Reg && x.reg == Reg.EAX && y.kind == Operand.Kind.Con && x.type.size == y.type.size)
            {
                // EAX := EAX op imm
                Put((int)op + 4 + sf); PutConst(y.type.size, y.val);
            }
            else if (y.kind == Operand.Kind.Con)
            {
                if (x.type.size > 1 && -128 <= y.val && y.val <= 127)
                { // rm := rm op signextend(imm8)
                    Put(0x82 + sf); PutOpd((int)op / 8, x); PutConst(1, y.val);
                }
                else if (x.type.size == y.type.size)
                { // rm := rm op imm
                    Put(0x80 + sf); PutOpd((int)op / 8, x); PutConst(y.type.size, y.val);
                }
                else m_Parser.SemErr("some error");
            }
            else if (x.kind == Operand.Kind.Reg)
            { // r := r op rm
                Put((int)op + 2 + sf); PutOpd(x.reg, y);
            }
            else if (y.kind == Operand.Kind.Reg)
            { // rm := rm op r
                Put((int)op + sf); PutOpd(y.reg, x);
            }
            else 
                m_Parser.SemErr("some error");
        }

        public Operand ConOpd(int val)
        {
            Operand x = new Operand();
            x.kind = Operand.Kind.Con; x.reg = Reg.none;  x.val = val; x.adr = 0; x.inx = Reg.none; x.type = Tab.intType;
            return x;
        }

        public void PutJMP(ref int adr)
        {
            if (adr > 0)
            { // backward jump
                int d = (pc + 2) - adr;
                if (d <= 127)
                {
                    Put(0xEB); PutConst(1, -d);
                }
                else
                {
                    Put(0xE9); PutConst(4, -(d + 4));
                }
            }
            else
            { // forward jump
                Put(0xE9); PutConst(4, adr);
                adr = -(pc - 4);
            }
        }

        public void Fixup(int adr)
        {
            while (adr < 0)
            {
                adr = -adr;
                int x = Get4(adr);
                Put4(adr, pc - (adr + 4));
                adr = x;
            }
        }

        public void RET()
        {
            Put(0xC3);
        }

        public void RET(int numBytes)
        {
            Put(0xC2);
            Put2(numBytes);
        }

        public void FreeOpd(Operand r)
        { // deallocate all registers held by r
            if (r.kind == Operand.Kind.Reg || r.kind == Operand.Kind.RegRel)
            {
                Reg.FreeReg(r.reg); r.reg = Reg.none;
            }
            if (r.inx != Reg.none)
            {
                Reg.FreeReg(r.inx); r.inx = Reg.none;
            }
        }

        public void PutJcc(int op, bool neg, bool signed, ref int adr)
        {
            if (signed) op = tab[op]; else op = tab[op + 6];
            if (neg) op = op ^ 1;
            if (adr > 0)
            { // backward jump
                int d = (pc + 2) - adr;
                if (d <= 127)
                {
                    Put(op); PutConst(1, -d);
                }
                else
                {
                    Put(0x0F); Put(op + 0x10); PutConst(4, -(d + 4));
                }
            }
            else
            { // forward jump
                Put(0x0F); Put(op + 0x10); PutConst(4, adr);
                adr = -(pc - 4);
            }
        }

        public enum CompOp { eq, neq, lt, geq, gt, leq }

        static int[] tab = {
            0x74, // JE
            0x75, // JNE
            0x7C, // JL
            0x7D, // JGE
            0x7F, // JG
            0x7E, // JLE
            0x74, // JZ
            0x75, // JNZ
            0x72, // JB
            0x73, // JAE
            0x77, // JA
            0x76 // JBE
            };


        public static readonly bool fjmp = true, tjmp = false, signed = true, unsigned = false;

        public void CALL(Operand proc)
        {
            if (proc.reg != Reg.none) // for predefined fuctions
            {
                this.Put(0xFF);
                int rm32 = 0xD0 | proc.reg; // 11(mod) 010(opcode extension /2) reg (xxx)
                this.Put(rm32);
            }
            else
            {
                this.Put(0xE8);
                this.Put4(proc.adr - (pc + 4));
            }
        }

        public void LEAVE()
        {
            this.Put(0xC9);
        }

        public void TRAP()
        {
            this.Put(0xCC);
        }

        public void ENTER(int varsize)
        {
            this.Put(0xC8);
            this.Put2(varsize);
            this.Put(0x00);
        }

        public void PUSH(Operand opd)
        {
            this.Put(0xFF);
            int rm32 = 0xF0 | opd.reg; //11(mod) 110(opcode extension /6) reg(xxx)
            this.Put(rm32);
        }

        public void POP(Operand opd)
        {
            this.Put(0x8F);
            int rm32 = 0xC0 | opd.reg;
            this.Put(rm32);
        }
    }
}
