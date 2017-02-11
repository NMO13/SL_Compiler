using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_Compiler.Codegen
{
    class Reg
    {
        public const int EAX = 0, ECX = 1, EDX = 2, EBX = 3, ESP = 4, EBP = 5, ESI = 6, EDI = 7;
        public const int none = -1, any = -1;
        private static bool[] free = { true, true, true, true, false, false, true, false };

        public static Parser m_Parser;
        public static int GetReg(out string error)
        {
            error = null;
            int r = EAX;
            if (free[EBX]) r = EBX;
            else if (free[EDX]) r = EDX;
            else if (free[ECX]) r = ECX;
            else if (free[EAX]) r = EAX;
            else if (free[ESI]) r = ESI;
            else if (free[EDI]) r = EDI;
            else error = "out of registers";
            free[r] = false;
            return r;
        }
        public static void GetReg(int r, out string error)
        {
            error = null;
            if (free[r])
                free[r] = false;
            else
                m_Parser.SemErr("cannot allocate register " + r);
        }

        public static bool IsFree(int r)
        {
            return free[r];
        }

        public static void FreeReg(int r)
        {
            if (r < 0 || r == EDI)
                return;
            if (r != ESP && r != EBP && !free[r])
            {
                free[r] = true;
            }
            else
                m_Parser.SemErr("cannot deallocate register " + r);
        }

        public static void FreeAllRegs()
        {
            free[EAX] = true; free[EBX] = true; free[ECX] = true;
            free[EDX] = true; free[ESI] = true;
        }
    }
}
