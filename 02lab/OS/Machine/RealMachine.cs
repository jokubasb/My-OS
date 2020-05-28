using OS.memory;
using OS.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        public ObservableCollection<VirtualMachine> VirtualMachines { get; set; }
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

        public RealPage[] MemoryPages { get; }

        public RealMachine(ObservableCollection<VirtualMachine> virtualMachines)
            : this()
        {
            VirtualMachines = virtualMachines;
        }

        public RealMachine()
        {
            VirtualMachines = new ObservableCollection<VirtualMachine>();
            for (int i = 0; i < memoryPages.Length; i++)
            {
                var realPage = new RealPage(i);
                memoryPages[i] = realPage;
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

        public void ExecuteAction(VirtualMachine virtualMachine)
        {
            try
            {
                virtualMachine.DoInstruction();
                CheckInterrupts(virtualMachine, "", false);
            }
            catch (Exception exception)
            {
                //virtualMachine.ReleaseResources();
                VirtualMachines.Remove(virtualMachine);
                MessageBox.Show("");
            }
        }

        public void FullyRunAllPrograms()
        {
            while (VirtualMachines.Any(x => !x.IsFinished))
            {
                RunVirtualMachinesUntilTimerInterupt();
            }
        }

        public void RunVirtualMachinesUntilTimerInterupt()
        {
            for (int i = 0; i < VirtualMachines.Count; i++)
            {
                try
                {
                    if (VirtualMachines[i].IsFinished)
                    {
                        continue;
                    }
                    TI = Settings.Default.TimerStartValue;
                    for (; TI > 0; TI--)
                    {
                        if (!VirtualMachines[i].IsFinished)
                        {
                            ExecuteAction(VirtualMachines[i]);

                        }
                    }
                }
                catch (Exception exception)
                {
                    if (i < VirtualMachines.Count)
                    {
                        VirtualMachines[i].ReleaseResources();
                        VirtualMachines.Remove(VirtualMachines[i]);
                        MessageBox.Show("");
                    }

                }
            }
        }
        bool test()
        {
            if((PI + SI) > 0 || TI == 0)
            {
                return true;
            }
            return false;
        }
        void CheckInterrupts(VirtualMachine vm, string command, bool trace)
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
                    using (StreamWriter wr = new StreamWriter("printer.txt"))
                    {
                        wr.WriteLine("get real data");
                    }
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
            PrintData();
            PrintVirtualData();
            PrintPageTable();
            Console.WriteLine("Exit statis: " + status);
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
        void PrintData()
        {
            int pagesCount = Settings.Default.PageSize;
            int page = 0;
            foreach(var mp in MemoryPages)
            {
                Console.Write(page + " ");
                for(int i = 0; i < pagesCount; ++i)
                {
                    Console.WriteLine("" + mp[i]);
                }
                ++page;
            }
        }
        void PrintVirtualData()
        {
            int pagesCount = Settings.Default.PageSize;
            int page = 0;
            //TODO virtual pages
            foreach (var mp in vm.MemoryPages)
            {
                Console.Write(page + " ");
                for (int i = 0; i < pagesCount; ++i)
                {
                    Console.WriteLine("" + mp[i]);
                }
                ++page;
            }
        }
        void PrintPageTable()
        {
            
        }
        void checkTrace(bool trace)
        {
            string waitCommand;
            if(trace = true)
            {
                while (true)
                {
                    Console.WriteLine("Press any symbol and enter to continue  or 'nextcom', 'rmdata', 'vmdata', 'pagetable', 'rmreg', 'vmreg' commands to trace");
                    waitCommand = Console.ReadLine();
                    if(waitCommand == "nextcom")
                    {

                    }
                    else if (waitCommand == "rmdata")
                    {
                        PrintData();
                    }
                    else if (waitCommand == "vmdata")
                    {
                        PrintVirtualData();
                    }
                    else if (waitCommand == "pagetable")
                    {
                        PrintPageTable();
                    }
                    else if (waitCommand == "rmreg")
                    {
                        PrintRegisters();
                    }
                    else if (waitCommand == "vmreg")
                    {
                        PrintVirtualRegisters(); 
                    }
                    else 
                    {
                        break;
                    }

                }
                waitCommand = "";
            }
        }
    }
}
