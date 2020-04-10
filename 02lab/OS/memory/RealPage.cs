using OS.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.memory
{
    class RealPage : Page
    {
        private void ResetMemory()
        {
            Memory = new ObservableCollection<Word>();
            Memory = new Word[Settings.Default.PageSize];
            for (int i = 0; i < Settings.Default.PageSize; i++)
            {
                Memory.Add(new Word());
            }
        }
        public RealPage(int pageNr) : base(pageNr)
        {
            ResetMemory();
        }
        protected override bool IsMemoryAccesable()
        {
            return true;
        }
        public override void Allocate(Page allocateFor)
        {
            if (!(allocateFor is VirtualPage))
            {
                //throw new AllocationException("It is possible to allocate real pages to virtual pages only");
            }
            if (IsAllocated)
            {
                //throw new AllocationException("Cannot allocate page that is already allocated");
            }
            if (allocateFor == null)
            {
                throw new NullReferenceException("Can not allocate to Null reference page");
            }
            if (allocateFor.AllocatedToPage != this)
            {
                AllocatedToPage = allocateFor;
            }
            ResetMemory();
            AllocatedToPage = allocateFor;
            allocateFor.Allocate(this);
        }

        public override void Deallocate(Page deallocateFrom)
        {
            if (!(deallocateFrom is VirtualPage))
            {
                //throw new AllocationException("It is possible to allocate (and deallocate) real pages to virtual pages only");
            }
            if (deallocateFrom == null)
            {
                throw new NullReferenceException("Can not deallocate page from Null reference");
            }
            if (AllocatedToPage != deallocateFrom)
            {
                //throw new AllocationException("Can not deallocate from page, to whom this page is not allocated");
            }
            if (deallocateFrom.AllocatedToPage == this)
            {
                deallocateFrom.Deallocate(this);
            }
            AllocatedToPage = null;
        }
    }
}
