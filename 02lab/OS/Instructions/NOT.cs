using OS.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Instructions
{
    class NOT
    {
        uint a;
        uint b;

        void Execute(VirtualMachine vm)
        {
            notA(vm);
        }

        void notA(VirtualMachine vm)
        {
            a = vm.A;
            vm.A = ~a;
        }
    }
}
