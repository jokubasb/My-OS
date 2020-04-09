using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.memory
{
    public class PageTable
    {
        private readonly RealMachine rm;
        private Random random = new Random();

        private VirtualPage[] m_virtualPages = new VirtualPage[Settings.Default.VirtualPagesCount];

        public VirtualPage[] VirtualPages
        {
            get { return m_virtualPages; }
            private set { m_virtualPages = value; }
        }

        public PageTable(RealMachine realMachine)
        {
            rm = realMachine;
            for (int i = 0; i < Settings.Default.VirtualPagesCount; i++)
            {
                VirtualPages[i] = new VirtualPage(i);
            }
        }

        public PageTable(PageTable oldPageTable) : this(oldPageTable.rm)
        {
            for (int i = 0; i < Settings.Default.VirtualPagesCount; i++)
            {
                var oldPage = oldPageTable.VirtualPages[i];
                if (oldPage.IsAllocated)
                {
                    for (int j = 0; j < oldPage.Memory.Count; j++)
                    {
                        this[i].Memory[j] = oldPage.Memory[j];
                    }
                }
            }
        }


        private RealPage FindFreePage()
        {
            int startPage = random.Next(0, Settings.Default.RealPagesCount);

            int currentPage = startPage;
            do
            {

                if (!rm.IsPageAllocated(currentPage))
                {
                    return rm.MemoryPages[currentPage];
                }
                currentPage++;
                currentPage %= Settings.Default.RealPagesCount;
            } while (currentPage != startPage);

            throw new InsufficientMemoryException("could not find free page");
        }


        public int GetPhysicalAddress(int virtualAddress)
        {
            int virtualBlockNr = virtualAddress / Settings.Default.PageSize;
            int shift = virtualAddress % Settings.Default.PageSize;

            if (this[virtualBlockNr] == null)
            {
                // throw new InvalidConstraintException("real page can not be null");
            }
            var realBlockNr = rm.GetPageIndex(this[virtualBlockNr]);
            return realBlockNr * Settings.Default.PageSize + shift;
        }


        public bool IsAllocatedPage(int virtualPageIndex)
        {
            return VirtualPages[virtualPageIndex].IsAllocated;
        }

        public RealPage this[int index]
        {
            get
            {
                if (index < 0 || index > Settings.Default.VirtualPagesCount - 1)
                {
                    throw new IndexOutOfRangeException("Index must be in range [0.." + (Settings.Default.VirtualPagesCount - 1) + "], current index: " + index);
                }
                if (!VirtualPages[index].IsAllocated)
                {
                    VirtualPages[index].Allocate(FindFreePage());
                }
                return (RealPage)VirtualPages[index].AllocatedToPage;
            }
        }

        public void DeallocateAllPages()
        {
            foreach (var virtualPage in VirtualPages)
            {
                var realPage = virtualPage.AllocatedToPage;
                if (realPage != null)
                {
                    virtualPage.Deallocate(realPage);
                }

            }
        }
    }
}
