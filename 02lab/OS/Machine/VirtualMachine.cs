﻿using OS.Instructions;
using OS.memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Machine
{
    class VirtualMachine
    {
        public string Name { get; set; }
        public int PTR { get; set; }
        public int PC { get; set; }
        public int SP { get; set; }
        public uint A { get; set; }
        public uint B { get; set; }
        public byte PID { get; set; }
        public bool CF { get; set; }
        public bool ZF { get; set; }
        public bool OF { get; set; }
        public bool IsFinished { get; set; }
        private enum Segments { DS, PS, SS };

        private readonly RealMachine rm;
        public PageTable pg { get; private set; }

        public VirtualMachine(VirtualMachine oldVirtualMachine)
        {
            this.PC = oldVirtualMachine.PC;
            this.SP = oldVirtualMachine.SP;
            this.IsFinished = oldVirtualMachine.IsFinished;
            this.Name = oldVirtualMachine.Name;
            this.pg = new PageTable(oldVirtualMachine.pg);
            this.rm = oldVirtualMachine.rm;

            this.pg = new PageTable(oldVirtualMachine.pg);
        }

        public VirtualMachine(RealMachine realMachine)
        {
            rm = realMachine;
            pg = new PageTable(rm);
            PC = 0;
            SP = 0;

        }

        public void DoInstruction()
        {
            if (IsFinished)
                return;

            PC++;
            string command = ""; //kazkaip gaunam komanda

            if (command.StartsWith("ADD"))
            {
                //kazkaip gaunam reiksmes i registrus a ir b
                var add = new ADD();
                add.addAB(this);
                //add.Execute(this);
                return;
            }

            if (command.StartsWith("SUB"))
            {
                //kazkaip gaunam reiksmes i registrus a ir b
                var sub = new SUB();
                sub.subAB(this);
                //add.Execute(this);
                return;
            }
            //TODO kitos komndos
        }
        public void ReleaseResources()
        {
            pg.DeallocateAllPages();
        }

    }

}
