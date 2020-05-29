using OS.memory;
using OS.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OS.Machine
{
    public class RealMachine
    {
        private int realMemorySize = Settings.Default.PageSize * Settings.Default.RealPagesCount;
        private RealPage[] memoryPages = new RealPage[Settings.Default.RealPagesCount];
        private Dictionary<RealPage, int> pagesIndexes = new Dictionary<RealPage, int>();
        private VirtualMachine vm; //initializuojam kai paleidziam programa

        public int PI { get; set; }
        public int TI { get; set; }
        public int SI { get; set; }
        public int MODE { get; set; }
        //int SF { get; set; } //CF ZF OF
        public int CS { get; set; }
        public int SS { get; set; }
        public int DS { get; set; }
        public int DSTI { get; set; }
        public int SRCI { get; set; }

        public RealPage[] MemoryPages
        {
            get
            {
                return memoryPages;
            }
            set
            {
                memoryPages = value;
            }
        }

        public RealMachine()
        {
            for (int i = 0; i < memoryPages.Length; i++)
            {
                var realPage = new RealPage(i);
                MemoryPages[i] = realPage;
                pagesIndexes.Add(realPage, i);
            }
        }

        public int GetPageIndex(RealPage realPage)
        {
            int index;
            if (!pagesIndexes.TryGetValue(realPage, out index))
            {
                throw new KeyNotFoundException("Could not found specified page");
            }
            return index;
        }
        public Word ReadMem(int addr)
        {
            if (addr < 0 || addr > realMemorySize - 1)
            {
                throw new IndexOutOfRangeException("");
            }
            var pageNr = addr / Settings.Default.PageSize;
            var pageShift = addr % Settings.Default.PageSize;
            return MemoryPages[pageNr][pageShift];
        }

        public void WriteMem(int addr, Word data)
        {
            if (addr < 0 || addr > realMemorySize - 1)
            {
                throw new IndexOutOfRangeException("");
            }
            var pageNr = addr / Settings.Default.PageSize;
            var pageShift = addr % Settings.Default.PageSize;
            MemoryPages[pageNr][pageShift] = data;

        }

        internal void AllocatePage(int pageNr, Page allocateToPage)
        {
            if (pageNr < 0 || pageNr > Settings.Default.RealPagesCount - 1)
            {
                throw new IndexOutOfRangeException("");
            }
            MemoryPages[pageNr].Allocate(allocateToPage);
        }

        internal void DeallocatePage(int pageNr, Page deallocateFromPage)
        {
            if (pageNr < 0 || pageNr > Settings.Default.RealPagesCount - 1)
            {
                throw new IndexOutOfRangeException("");
            }
            MemoryPages[pageNr].Deallocate(deallocateFromPage);
        }

        internal bool IsPageAllocated(int pageNr)
        {
            return MemoryPages[pageNr].IsAllocated;
        }

        public void runProgram(string path, bool trace)
        {
            vm = new VirtualMachine(this, 4, 16);
            try
            {
                vm.LoadProgramToMemmory(path);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return;
            }
            PrintPageTable();
            //TI = Settings.Default.TimerStartValue;  // reset timer
            //while (true)
            //{
            //    checkTrace(trace);
            //    vm.exec();
            //    CheckInterrupts(trace);
            //}
        }
        bool test()
        {
            if((PI + SI) > 0 || TI == 0)
            {
                return true;
            }
            return false;
        }
        void CheckInterrupts(bool trace)
        {
            if (test())
            {
                MODE = 1;

                if (PI > 0)
                {
                    InterruptQuit(1);
                }
                else if (TI <= 0)
                {
                    InterruptQuit(1);
                }
                else if (SI == 1) //gd
                {

                }
                else if (SI == 2) //pd
                {
                    int x1 = 0; //TODO is command string
                    int x2 = 0; //TODO is command string
                    //using (StreamWriter wr = new StreamWriter("printer.txt"))
                    //{
                    //    wr.WriteLine("get real data");
                    //}
                    checkTrace(trace);
                }
                else if (SI == 3) //halt
                {
                    InterruptQuit(0);
                }
                else if (SI == 4)
                {

                }
                else if (SI == 5)
                {

                }

                if(PI > 0)
                {
                    InterruptQuit(0);
                }

                SI = 0;
                PI = 0;

                MODE = 0;
            }
        }
        void InterruptQuit(int status)
        {
            PrintRegisters();
            PrintVirtualRegisters();
            for (int i = 0; i < Settings.Default.RealPagesCount; i++)
            {
                PrintPageContents(i);
            }
            for (int i = 0; i < Settings.Default.VirtualPagesCount; i++)
            {
                PrintVirtualPageContents(i);
            }
            PrintPageTable();
            Console.WriteLine("Exit status: " + status);
        }
        void PrintRegisters()
        {
            Console.WriteLine("Real machine registers:");
            Console.WriteLine("PI = " + PI.ToString("X"));
            Console.WriteLine("TI = " + TI.ToString("X"));
            Console.WriteLine("SI = " + SI.ToString("X"));
            Console.WriteLine("MODE = " + MODE);
            Console.WriteLine("CS = " + CS.ToString("X"));
            Console.WriteLine("SS = " + SS.ToString("X"));
            Console.WriteLine("DS = " + DS.ToString("X"));
            Console.WriteLine("DSTI = " + DSTI.ToString("X"));
            Console.WriteLine("SRCI = " + SRCI.ToString("X"));
        }
        void PrintVirtualRegisters()
        {
            //parasytas global vm
            Console.WriteLine("Virtual machine registers");
            Console.WriteLine("PTR = " + vm.PTR.ToString("X"));
            Console.WriteLine("PC = " + vm.PC.ToString("X"));
            Console.WriteLine("SP = " + vm.SP.ToString("X"));
            Console.WriteLine("A = " + vm.A.ToString("X"));
            Console.WriteLine("B = " + vm.B.ToString("X"));
            Console.WriteLine("PID = " + vm.PID.ToString("X"));
            Console.WriteLine("CF = " + vm.CF);
            Console.WriteLine("ZF = " + vm.ZF);
            Console.WriteLine("OF = " + vm.OF);
            Console.WriteLine("CS = " + vm.CS.ToString("X"));
            Console.WriteLine("DS = " + vm.DS.ToString("X"));
            Console.WriteLine("SS = " + vm.SS.ToString("X"));
            Console.WriteLine("IsFinished = " + vm.IsFinished);
        }
        void PrintPageContents(int pageNr)
        {
            if (pageNr < 0 || pageNr > Settings.Default.RealPagesCount - 1)
            {
                throw new IndexOutOfRangeException("");
            }
            Page p = MemoryPages[pageNr];
            for (int i = 0; i < Settings.Default.PageSize; i++)
            {
                Console.WriteLine(i+ ": " + p[i]);
            }
        }
        void PrintVirtualPageContents(int virtualPageNr)
        {
            PrintPageContents(GetPageIndex(vm.pg[virtualPageNr]));
        }
        void PrintPageTable()
        {
            for (int i = 0; i < Settings.Default.VirtualPagesCount; i++)
            {
                int realPageNr = GetPageIndex(vm.pg[i]);
                Console.WriteLine("virtual page nr: " + i + " real page nr: " + realPageNr);
                PrintPageContents(realPageNr);
            }
        }
        void checkTrace(bool trace)
        {
            if(trace)
            {
                while (true)
                {
                    Console.WriteLine("Press any key to skip or 'nextcom(1)', 'rmdata(2)', 'vmdata(3)', 'pagetable(4)', 'rmreg(5)', 'vmreg(6)' to trace");
                    int input = int.Parse(Console.ReadLine());
                    if (input == 1)
                    {
                        string command = ReadMem(vm.pg.GetPhysicalAddress(vm.PC)).GetString().TrimStart();
                        Console.WriteLine("next command is: "+command);
                    }
                    else if (input == 2)
                    {
                        int nr;
                        Console.WriteLine("Please enter page nr you want to view (0 - "+(Settings.Default.RealPagesCount-1)+")");
                        nr = int.Parse(Console.ReadLine());
                        PrintPageContents(nr);
                    }
                    else if (input == 3)
                    {
                        int nr;
                        Console.WriteLine("Please enter page nr you want to view (0 - " + (Settings.Default.VirtualPagesCount - 1) + ")");
                        nr = int.Parse(Console.ReadLine());
                        PrintVirtualPageContents(nr);
                    }
                    else if (input == 4)
                    {
                        PrintPageTable();
                    }
                    else if (input == 5)
                    {
                        PrintRegisters();
                    }
                    else if (input == 6)
                    {
                        PrintVirtualRegisters(); 
                    }
                    else 
                    {
                        break;
                    }
                }
            }
        }
    }
}
