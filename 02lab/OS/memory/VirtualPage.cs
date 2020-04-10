using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.memory
{
    public class VirtualPage : Page
    {
        public VirtualPage(int pageNr) : base(pageNr)
        {
        }
        protected override bool IsMemoryAccesable()
        {
            return IsAllocated;
        }
        public override void Allocate(Page allocateFor)
        {
            if (AllocatedToPage == allocateFor)
            {
                return;
            }

            if (!(allocateFor is RealPage))
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
            AllocatedToPage = allocateFor;
            if (allocateFor.AllocatedToPage != this)
            {
                allocateFor.Allocate(this);
                this.Memory = allocateFor.Memory;
            }
        }

        public override void Deallocate(Page deallocateFrom)
        {
            if (deallocateFrom == null)
            {
                throw new NullReferenceException("");
            }
            if (!(deallocateFrom is RealPage))
            {
                //throw new AllocationException("");
            }
            if (AllocatedToPage != deallocateFrom)
            {
                //throw new AllocationException("");
            }

            if (deallocateFrom.AllocatedToPage == this)
            {
                AllocatedToPage = null;
                deallocateFrom.Deallocate(this);
            }
            this.Memory = null;
        }
    }
}
