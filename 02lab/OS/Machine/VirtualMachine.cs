using OS.Instructions;
using OS.memory;
using OS.Properties;
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

        private void validateLine(string fileName, string line, ref int lineNo)
        {
            lineNo++;
            if (line == null)
            {
                throw new ParsingException(fileName, "unexpected end of file reached", lineNo);
            }
            if (line.Length > 4)
            {
                throw new ParsingException(fileName, "line exceeds its limit(4)", lineNo);
            }
        }

        public void LoadProgramToMemmory(string file)
        {
            StreamReader sr = new StreamReader(@file);
            try
            {
                string line = sr.ReadLine();
                if (line == null || !line.Equals(Settings.Default.ProgramDataSectionSymbol))
                {
                    throw new ParsingException(file, "program must start with '" + Settings.Default.ProgramDataSectionSymbol + "' symbol", 0);
                }
                else
                {
                    // parsing data
                    int lineNo = 1;
                    int pos = DS * Settings.Default.PageSize;
                    int end = SS * Settings.Default.PageSize - 1;
                    bool shouldStop = false;
                    do
                    {
                        line = sr.ReadLine();
                        validateLine(file, line, ref lineNo);
                        if (pos > end)
                        {
                            throw new ParsingException(file, "data section exceeds its limit", lineNo);
                        }
                        if (!line.Equals(Settings.Default.ProgramCodeSectionSymbol))
                        {
                            rm.WriteMem(pg.GetPhysicalAddress(pos), new Word(line));
                        }
                        else
                        {
                            shouldStop = true;
                        }
                        pos++;
                    } while (!shouldStop);

                    // parsing code
                    pos = 0;
                    end = DS - 1;
                    do
                    {
                        line = sr.ReadLine();
                        validateLine(file, line, ref lineNo);
                        if (pos > end)
                        {
                            throw new ParsingException(file, "code section exceeds its limit", lineNo);
                        }
                        if (!line.Equals(Settings.Default.ProgramEndSymbol))
                        {
                            rm.WriteMem(pg.GetPhysicalAddress(pos), new Word(line));
                        }
                        else
                        {
                            shouldStop = true;
                        }
                        pos++;
                    } while (!shouldStop);
                }
            }
            catch (OutOfMemoryException exc)
            {
                pg.DeallocateAllPages();
                throw;
            }
            finally
            {
                sr.Close();
            }

        }

        public void exec()
        {
            if (IsFinished)
                return;

            rm.TI--;    // decrement timer each step
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

            PC++;       // increment program counter each step
        }
        public void ReleaseResources()
        {
            pg.DeallocateAllPages();
        }

    }

}
