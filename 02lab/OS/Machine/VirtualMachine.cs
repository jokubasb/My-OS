using OS.Instructions;
using OS.memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Machine
{
    public class VirtualMachine
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
        public int CS;
        public int DS;
        public int SS;
        public int PagesNumber { get; set; }

        private readonly RealMachine rm;
        public PageTable pg { get; private set; }

        public VirtualMachine(VirtualMachine oldVirtualMachine)
        {
            this.PC = oldVirtualMachine.PC;
            this.SP = oldVirtualMachine.SP;
            this.IsFinished = oldVirtualMachine.IsFinished;
            this.Name = oldVirtualMachine.Name;
            this.pg = new PageTable(oldVirtualMachine.pg, oldVirtualMachine.PagesNumber);
            this.rm = oldVirtualMachine.rm;
        }

        public VirtualMachine(RealMachine realMachine, int CodeSize, int MaxPages)
        {
            rm = realMachine;
            pg = new PageTable(rm, MaxPages);
            PC = 0;
            SP = 0;
            PagesNumber = MaxPages;
            CS = 0;
            DS = CodeSize;
            SS = ((MaxPages - CodeSize) - (MaxPages - CodeSize) / 2) + CodeSize;
        }

        public void LoadProgramToMemmory(string file)
        {
            string[] lines;
            bool DataSection = false;
            bool CodeSection = false;
            try
            {
                lines = File.ReadAllLines(@file);
            }
            catch(FileNotFoundException exc)
            {
                Console.WriteLine(exc.Message);
                return;
            }
            foreach (string line in lines)
            {
                Console.WriteLine(line);
                if(line.Equals(".data"))
                {
                    DataSection = true;
                    CodeSection = false;
                }
                else if (line.Equals(".code"))
                {
                    CodeSection = true;
                    DataSection = false;
                }
                else
                {
                    if(DataSection)
                    {
                        // writing data to Data Segment pointed to by DS
                    }
                    else if(CodeSection)
                    {
                        // writing code to Code Segment pointed to by CS
                    }
                    else
                    {
                        Console.WriteLine("bad structure"); // exception in future
                        return;
                    }
                }
            }

        }

        public void DoInstruction()
        {
            if (IsFinished)
                return;

            PC++;
            string command = "";

            if (command.StartsWith("ADD"))
            {
                var add = new ADD();
                add.addAB(this);
                //add.Execute(this);
                return;
            }

            if (command.StartsWith("SUB"))
            {
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
