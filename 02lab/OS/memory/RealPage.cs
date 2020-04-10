using OS.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.memory
{
    public class RealPage : Page
    {
        private void ResetMemory()
        {
            Memory = new ObservableCollection<Word>();
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
                //throw new AllocationException("");
            }
            if (IsAllocated)
            {
                //throw new AllocationException("");
            }
            if (allocateFor == null)
            {
                throw new NullReferenceException("");
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
                //throw new AllocationException("");
            }
            if (deallocateFrom == null)
            {
                throw new NullReferenceException("");
            }
            if (AllocatedToPage != deallocateFrom)
            {
                //throw new AllocationException("");
            }
            if (deallocateFrom.AllocatedToPage == this)
            {
                deallocateFrom.Deallocate(this);
            }
            AllocatedToPage = null;
        }
    }
}
