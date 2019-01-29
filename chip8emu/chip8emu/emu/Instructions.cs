using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chip8emu.emu
{
    public class Instructions
    {

        // http://devernay.free.fr/hacks/chip8/C8TECH10.HTM#1.0

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
        /// 00EE
        /// Return from a subroutine
        /// The interpreter sets the program counter to the address at the top of the stack, then subtracts 1 from the stack pointer.
        /// </summary>
        public void RET()
        {
            memory.PC = memory.SP;
            memory.SP--;
        }

        /// <summary>
        /// 1nnn
        /// Jump to location nnn
        /// The interpreter sets the program counter to nnn
        /// </summary>
        public void JP()
        {
            memory.PC = (ushort) (memory.opcode & 0x0FFF);
            memory.SP--;
        }

        /// <summary>
        /// 2nnn
        /// Call subroutine at nnn
        /// The interpreter increments the stack pointer, then puts the current
        /// PC on the top of the stack. The PC is then set to nnn
        /// </summary>
        public void CALL()
        {
            memory.SP++;
            memory.stack[memory.SP] = memory.PC;
            memory.PC = (ushort)(memory.opcode & 0x0FFF);
        }

        /// <summary>
        /// 3xkk
        /// Skip next instruction if Vx = kk
        /// The interpreter compares register Vx to kk, and if they are equal,
        /// increments the program counter by 2.
        /// </summary>
        public void SE_BYTE()
        {
            if (memory.V[(memory.opcode & 0x0F00) >> 8] == (memory.opcode & 0x00FF))
            {
                memory.PC += 2;
            }
        }

        /// <summary>
        /// 4xkk
        /// Skip next instruction if Vx != kk
        /// The interpreter compares register Vx to kk, and if they are not equal, increments
        /// the program counter by 2
        /// </summary>
        public void SNE()
        {
            if (memory.V[(memory.opcode & 0x0F00) >> 8] != (memory.opcode & 0x00FF))
            {
                memory.PC += 2;
            }
        }

        /// <summary>
        /// 5xy0
        /// Skip next instruction if Vx = Vy
        /// The interpreter compares register Vx to register Vy, and if they are equal, increments
        /// the program counter by 2
        /// </summary>
        public void SE_VY()
        {
            if (memory.V[(memory.opcode & 0x0F00) >> 8] == memory.V[(memory.opcode & 0x00F0) >> 4])
            {
                memory.PC += 2;
            }
        }

        /// <summary>
        /// 6xkk
        /// Set Vx = kk
        /// The interpreter puts the value kk into register Vx
        /// </summary>
        public void LD_BYTE()
        {
            memory.V[(memory.opcode & 0x0F00) >> 8] = (byte) (memory.opcode & 0x00FF);
        }
    }
}
