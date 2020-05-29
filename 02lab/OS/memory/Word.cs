using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.memory
{
    public class Word
    {
        private const int SIZE = 4;
        private readonly byte[] word = new byte[SIZE];

        public Word()
        {
        }

        public Word(Word other)
        {
            this.word = other.word;
        }

        public Word(UInt32 val) : this(val.ToString("X4"))
        {
        }

        public Word(string val)
        {
            this.word = Encoding.ASCII.GetBytes(val);
        }

        public string GetString()
        {
            char[] tmp = new char[this.word.Length];
            for (int i = 0; i < this.word.Length; i++)
            {
                tmp[i] = (char)this.word[i];
            }
            return new string(tmp);
        }

        public UInt32 GetInt()
        {
            UInt32 res = BitConverter.ToUInt32(this.word, 0);
            return res;
        }

        public byte GetByte(int i)
        {
            return this.word[i];
        }

        public void SetByte(int i, byte val)
        {
            this.word[i] = val;
        }

        public byte this[int i]
        {
            get { return GetByte(i); }
            set { SetByte(i, value); }
        }

        public override string ToString()
        {
            return GetString();
        }
    }
}
