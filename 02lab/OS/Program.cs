using OS.memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OS.Instructions;
using OS.Machine;

namespace OS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            RealMachine rm = new RealMachine();
            rm.runProgram("test.txt", true);
        }
    }
}
