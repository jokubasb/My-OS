using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Machine
{
    public class CPU
    {
        public int PI { get; set;}
        public int TI { get; set; }
        public int SI { get; set; }
        public int MODE { get; set; }

        //int SF { get; set; } //CF ZF OF

        public int CS { get; set; }
        public int SS { get; set; }
        public int DS { get; set; }
        public int DSTI { get; set; }
        public int SRCI { get; set; }


        //cj nereiks situ
        /*
        public void changeZF(bool value)
        {
            string binary = Convert.ToString(SF, 2);
            if (value == true)
            {
                binary = '1' + binary.Remove(2, 1);
            }
            else
            {
                binary = '0' + binary.Remove(2, 1);
            }
            
        }

        public void changeCF(bool value)
        {
            string binary = Convert.ToString(SF, 2);
            if (value == true)
            {
                binary = '1' + binary.Remove(0, 1);
            }
            else
            {
                binary = '0' + binary.Remove(0, 1);
            }

        }

        public void changeOF(bool value)
        {
            string binary = Convert.ToString(SF, 2);
            if (value == true)
            {
                binary = '1' + binary.Remove(1, 1);
            }
            else
            {
                binary = '0' + binary.Remove(1, 1);
            }
        }
        */
    }
}
