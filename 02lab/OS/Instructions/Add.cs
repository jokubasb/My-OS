using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OS.Machine;

namespace OS.Instructions
{
    class ADD
    {
        uint a;
        uint b;
        
        public void Execute(VirtualMachine vm)
        {
            addAB(vm);
        }

        public void addAB(VirtualMachine vm)
        {
            a = vm.A;
            b = vm.B;
            vm.A = a + b;
            if (a + b > 4294967295)
            {
                vm.CF = true;
            }
            else vm.CF = false;
            if ((a + b) % 4294967296 == 0)
            {
                vm.ZF = true;
            }
            else vm.ZF = false;
        }
    }
}
