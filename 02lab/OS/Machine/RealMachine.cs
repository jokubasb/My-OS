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
    public class RealMachine
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
    }
}
