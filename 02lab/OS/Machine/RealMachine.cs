using OS.memory;
using OS.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OS.Machine
{
    class RealMachine
    {
        private int realMemorySize = Settings.Default.PageSize * Settings.Default.RealPagesCount;
        private RealPage[] memoryPages = new RealPage[Settings.Default.RealPagesCount];
        private Dictionary<RealPage, int> pagesIndexes = new Dictionary<RealPage, int>();
        public ObservableCollection<VirtualMachine> VirtualMachines { get; set; }

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


        /// <summary>
        /// Reads <see cref="Memory.Word"/> from memory at given physical address
        /// </summary>
        /// <param name="addr">physical memory address</param>
        /// <returns>word that was writen in given address</returns>
        /// <exception cref="IndexOutOfRangeException"> Throws exception if address is out of memory bounds</exception>
        public Word ReadMem(int addr)
        {
            if (addr < 0 || addr > realMemorySize - 1)
            {
                throw new IndexOutOfRangeException(
                    "Cannot access memory that is out of bounds, memory range: [0.." + (realMemorySize - 1) + "] " +
                    "tried to access: " + addr);
            }
            var pageNr = addr / Settings.Default.PageSize;
            var pageShift = addr % Settings.Default.PageSize;
            return MemoryPages[pageNr][pageShift];
        }

        /// <summary>
        /// Writes <see cref="Memory.Word"/> into memory at given physical address
        /// </summary>
        /// <param name="addr">physical memory address</param>
        /// <param name="data"><see cref="Memory.Word"/> that needs to be written into given address</param>
        /// <exception cref="IndexOutOfRangeException"> Throws exception if address is out of memory bounds</exception>
        public void WriteMem(int addr, Word data)
        {
            if (addr < 0 || addr > realMemorySize - 1)
            {
                throw new IndexOutOfRangeException(
                    "Cannot access memory that is out of bounds, memory range: [0.." + (realMemorySize - 1) + "] " +
                    "tried to access: " + addr);
            }
            var pageNr = addr / Settings.Default.PageSize;
            var pageShift = addr % Settings.Default.PageSize;
            MemoryPages[pageNr][pageShift] = data;

        }

        internal void AllocatePage(int pageNr, Page allocateToPage)
        {
            if (pageNr < 0 || pageNr > Settings.Default.RealPagesCount - 1)
            {
                throw new IndexOutOfRangeException("page number (" + pageNr + ") is out of range, page number must be between [0.." + (Settings.Default.RealPagesCount - 1) + "]");
            }
            MemoryPages[pageNr].Allocate(allocateToPage);
        }

        internal void DeallocatePage(int pageNr, Page deallocateFromPage)
        {
            if (pageNr < 0 || pageNr > Settings.Default.RealPagesCount - 1)
            {
                throw new IndexOutOfRangeException("page number (" + pageNr + ") is out of range, page number must be between [0.." + (Settings.Default.RealPagesCount - 1) + "]");
            }
            MemoryPages[pageNr].Deallocate(deallocateFromPage);
        }

        internal bool IsPageAllocated(int pageNr)
        {
            return MemoryPages[pageNr].IsAllocated;
        }


        public VirtualMachine ForkVirtualMachine(VirtualMachine virtualMachine)
        {
            VirtualMachine vm;

            try
            {
                vm = new VirtualMachine(virtualMachine);
                VirtualMachines.Add(vm);
                return vm;
            }
            catch (Exception)
            {
                if (virtualMachine != null)
                {
                    virtualMachine.ReleaseResources();
                }
                throw new InsufficientMemoryException("No more memmory");
            }

        }

        public void ExecuteAction(VirtualMachine virtualMachine)
        {
            try
            {
                virtualMachine.DoInstruction();
            }
            catch (Exception exception)
            {
                virtualMachine.ReleaseResources();
                VirtualMachines.Remove(virtualMachine);
                MessageBox.Show("Ooops... your program have crased\n" + exception.Message, "Program " + virtualMachine.Name + "(" + virtualMachine.PID + ") have occured error");
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
                        MessageBox.Show("Ooops... your program have crased\n" + exception.Message, "Program " + VirtualMachines[i].Name + "(" + VirtualMachines[i].PID + ") have occured error");
                    }

                }
            }
        }
    }
}
