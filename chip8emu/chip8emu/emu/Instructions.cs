using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chip8emu.emu
{
    public class Instructions
    {

        Memory memory;

        public Instructions(Memory memory)
        {
            this.memory = memory;
        }

        ///Clears the display - This probably wont be handled in this class, so this function is probably temporary
        public void CLS()
        {

        }

        /// <summary>
        /// Return from a subroutine
        /// The interpreter sets the program counter to the address at the top of the stack, then subtracts 1 from the stack pointer.
        /// </summary>
        public void RET()
        {
            memory.PC = memory.SP;
            memory.SP--;
        }

        /// <summary>
        /// Jump to location nnn
        /// The interpreter sets the program counter to nnn
        /// </summary>
        public void JP()
        {
            memory.PC = (ushort) (memory.opcode & 0x0FFF);
            memory.SP--;
        }
    }
}
