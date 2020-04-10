using OS.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Instructions
{
    class XOR
    {
        uint a;
        uint b;

        void Execute(VirtualMachine vm)
        {
            xorAB(vm);
        }

        void xorAB(VirtualMachine vm)
        {
            a = vm.A;
            b = vm.B;
            vm.A = a ^ b;
        }
    }
}
