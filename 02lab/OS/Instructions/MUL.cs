using OS.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Instructions
{
    class MUL
    {
        uint a;
        uint b;

        public void Execute(VirtualMachine vm, CPU cpu, PagingDevice pd)
        {
            //vm.PC++;
            mulAB(vm);

        }

        void mulAB(VirtualMachine vm)
        {
            a = vm.A;
            b = vm.B;
            vm.A = a * b;
            if ((a * b) % 4294967296 == 0)
            {
                vm.ZF = true;
            }
            else vm.ZF = false;
        }
    }
}
