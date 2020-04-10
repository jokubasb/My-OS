using OS.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Instructions
{
    class DIV
    {
        uint a;
        uint b;

        public void Execute(VirtualMachine vm)
        {
            divAB(vm);

        }

        void divAB(VirtualMachine vm)
        {
            a = vm.A;
            b = vm.B;
            vm.A = a / b;
            if (a / b == 0)
            {
                vm.ZF = true;
            }
            else vm.ZF = false;
        }
    }
}
