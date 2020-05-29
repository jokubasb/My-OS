using OS.Instructions;
using OS.memory;
using OS.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
                    int lineNo = 1;

                    // parsing data
                    int pos = DS * Settings.Default.PageSize;
                    int end = SS * Settings.Default.PageSize - 1;
                    while (!(line = sr.ReadLine()).Equals(Settings.Default.ProgramCodeSectionSymbol))
                    {
                        validateLine(file, line, ref lineNo);
                        if (pos > end)
                        {
                            throw new ParsingException(file, "data section exceeds its limit", lineNo);
                        }
                        rm.WriteMem(pg.GetPhysicalAddress(pos), new Word(line));
                        pos++;
                    }

                    // parsing code
                    pos = 0;
                    end = DS * Settings.Default.PageSize - 1;
                    while (!(line = sr.ReadLine()).Equals(Settings.Default.ProgramEndSymbol))
                    {
                        validateLine(file, line, ref lineNo);
                        if (pos > end)
                        {
                            throw new ParsingException(file, "code section exceeds its limit", lineNo);
                        }
                        rm.WriteMem(pg.GetPhysicalAddress(pos), new Word(line));
                        pos++;
                    }
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

        public int exec()
        {
            if (IsFinished)
            {
                return 0;
            }

            rm.TI--;    // decrement timer each step
            string command = rm.ReadMem(pg.GetPhysicalAddress(PC)).GetString().TrimStart();     // fetch instruction

            // --------------------------------------- ARITHMETIC
            if (command.StartsWith("ADD"))
            {
                if (A + B > UInt32.MaxValue)
                {
                    CF = true;
                }
                else CF = false;
                if ((A + B) % UInt32.MaxValue == 0)
                {
                    ZF = true;
                }
                else ZF = false;
                return 0;
            }

            if (command.StartsWith("SUB"))
            {
                if (A < B)
                {
                    CF = true;
                }
                else CF = false;
                if (A == B)
                {
                    ZF = true;
                }
                else ZF = false;
                return 0;
            }

            if (command.StartsWith("MUL"))
            {
                if ((A * B) % UInt32.MaxValue == 0)
                {
                    ZF = true;
                }
                else ZF = false;
                return 0;
            }

            if (command.StartsWith("DIV"))
            {
                if (A / B == 0)
                {
                    ZF = true;
                }
                else ZF = false;
                return 0;
            }

            // --------------------------------------- DATA COPY
            if (command.StartsWith("MA"))
            {
                int addr = Convert.ToInt32(command.Substring(2, 2), 16);
                A = rm.ReadMem(addr).GetInt();
                return 0;
            }

            if (command.StartsWith("MB"))
            {
                int addr = Convert.ToInt32(command.Substring(2, 2), 16);
                B = rm.ReadMem(addr).GetInt();
                return 0;
            }

            // --------------------------------------- LOGICAL
            if (command.StartsWith("AND"))
            {
                A = A & B;
                return 0;
            }

            if (command.StartsWith("OR"))
            {
                A = A | B;
                return 0;
            }

            if (command.StartsWith("XOR"))
            {
                A = A ^ B;
                return 0;
            }

            if (command.StartsWith("NOT"))
            {
                A = ~A;
                return 0;
            }

            // --------------------------------------- COMPARE
            if (command.StartsWith("CMP"))
            {
                uint res = A - B;
                if (res > A)
                {
                    CF = true;
                    ZF = false;
                    OF = true;
                }
                else if (res == 0)
                {
                    CF = false;
                    ZF = true;
                    OF = false;
                }
                else
                {
                    CF = false;
                    ZF = false;
                    OF = false;
                }
                return 0;
            }

            // --------------------------------------- BRANCH
            if (command.StartsWith("JC"))
            {
                if (CF)
                {
                    int addr = Convert.ToInt32(command.Substring(2, 2), 16);
                    PC = (short)addr;
                }
                return 0;
            }

            if (command.StartsWith("NC"))
            {
                if (!CF)
                {
                    int addr = Convert.ToInt32(command.Substring(2, 2), 16);
                    PC = (short)addr;
                }
                return 0;
            }

            if (command.StartsWith("JZ"))
            {
                if (ZF)
                {
                    int addr = Convert.ToInt32(command.Substring(2, 2), 16);
                    PC = (short)addr;
                }
                return 0;
            }

            if (command.StartsWith("NZ"))
            {
                if (!ZF)
                {
                    int addr = Convert.ToInt32(command.Substring(2, 2), 16);
                    PC = (short)addr;
                }
                return 0;
            }

            if (command.StartsWith("JE"))
            {
                if (ZF)
                {
                    int addr = Convert.ToInt32(command.Substring(2, 2), 16);
                    PC = (short)addr;
                }
                return 0;
            }

            if (command.StartsWith("JG"))
            {
                if (!CF)
                {
                    int addr = Convert.ToInt32(command.Substring(2, 2), 16);
                    PC = (short)addr;
                }
                return 0;
            }

            if (command.StartsWith("JL"))
            {
                if (CF)
                {
                    int addr = Convert.ToInt32(command.Substring(2, 2), 16);
                    PC = (short)addr;
                }
                return 0;
            }

            if (command.StartsWith("BR"))
            {
                int addr = Convert.ToInt32(command.Substring(2, 2), 16);
                PC = (short)addr;
                return 0;
            }

            // --------------------------------------- I/O
            if (command.StartsWith("PD"))
            {
                rm.TI -= 2;     // I/O operations take 3 times
                int addr = Convert.ToInt32(command.Substring(2, 2), 16);
                if (addr >= 0 && addr <= 255)
                {
                    rm.SI = 2;
                }
                else
                {
                    rm.PI = 1;  // wrong addres
                }
                return addr;
            }

            if (command.StartsWith("GD"))
            {
                rm.TI -= 2;     // I/O operations take 3 times
                int addr = Convert.ToInt32(command.Substring(2, 2), 16);
                if (addr >= 0 && addr <= 255)
                {
                    rm.SI = 1;
                }
                else
                {
                    rm.PI = 1;  // wrong addres
                }
                return addr;
            }

            // --------------------------------------- EXIT
            if (command.StartsWith("HALT"))
            {
                rm.SI = 3;
                return 0;
            }

            PC++;       // increment program counter each step

            return 0;
        }
        public void ReleaseResources()
        {
            pg.DeallocateAllPages();
        }

    }

}
