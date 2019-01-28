using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chip8emu.emu
{
    public class CPU
    {
        Memory memory;

        public CPU()
        {
            memory = new Memory();
        }

        public Memory getMemory()
        {
            return memory;
        }

    }
}
