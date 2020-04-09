using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OS.Machine;

namespace OS.Instructions
{
    class SUB
    {
        uint a;
        uint b;

        public void Execute(VirtualMachine vm, CPU cpu, PagingDevice pd)
        {
            //vm.PC++;
            subAB(vm);

        }

        public void subAB(VirtualMachine vm)
        {
            a = vm.A;
            b = vm.B;
            vm.A = a - b;
            if (a < b)
            {
                vm.CF = true;
            }
            else vm.CF = false;
            if (a == b)
            {
                vm.ZF = true;
            }
            else vm.ZF = false;
        }
    }
}
