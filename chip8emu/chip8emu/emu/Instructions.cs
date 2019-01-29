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
        // Chip8 Technical Reference
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

        /// <summary>
        /// 7xkk
        /// Set Vx = Vx + kk
        /// Adds the value kk to the value of register Vx, then stores the result in Vx
        /// </summary>
        public void ADD_BYTE()
        {
            byte register = (byte) ((memory.opcode & 0x0F00) >> 8);
            int sum = (memory.V[register] + (memory.opcode & 0x00FF)); 
            if (sum > 255) sum = 255; //Cap sum at 255, not the most efficent, but whatever
            memory.V[register] = (byte)sum;
        }   

        /// <summary>
        /// 8xy0
        /// Set Vx = Vy
        /// Stores the value of register Vy in register Vx
        /// </summary>
        public void LD_VY()
        {
            memory.V[(memory.opcode & 0x0F00) >> 8] = (byte) memory.V[(memory.opcode & 0x00F0) >> 4];
        }

        /// <summary>
        /// 8xy1
        /// Set Vx = Vx OR Vy
        /// Performs a bitwise OR on the values of Vx and Vy, then stores the result in Vx
        /// </summary>
        public void OR()
        {
            memory.V[(memory.opcode & 0x0F00) >> 8] = (byte) (memory.V[(memory.opcode & 0x0F00) >> 8] | memory.V[(memory.opcode & 0x00F0) >> 4]);
        }

        /// <summary>
        /// 8xy2
        /// Set Vx = Vx AND Vy
        /// Performs a bitwise AND on the values of Vx and Vy, then stores the result in Vx
        /// </summary>
        public void AND()
        {
            memory.V[(memory.opcode & 0x0F00) >> 8] = (byte)(memory.V[(memory.opcode & 0x0F00) >> 8] & memory.V[(memory.opcode & 0x00F0) >> 4]);
        }

        /// <summary>
        /// 8xy3
        /// Set Vx = Vx XOR Vy
        /// Performs a bitwise XOR on the values of Vx and Vy, then stores the result in Vx
        /// </summary>
        public void XOR()
        {
            memory.V[(memory.opcode & 0x0F00) >> 8] = (byte)(memory.V[(memory.opcode & 0x0F00) >> 8] ^ memory.V[(memory.opcode & 0x00F0) >> 4]);
        }

        /// <summary>
        /// 8xy4
        /// Set Vx = Vx + Vy, set VF = carry
        /// The values of Vx and Vy are added together. If the result is greater than 8 bits,
        /// VF is set to 1, otherwise 0. Only the lowest 8 bits of the results are kept, and 
        /// stored in Vx.
        /// </summary>
        public void ADD()
        {
            int sum = memory.V[(memory.opcode & 0x0F00) >> 8] + memory.V[(memory.opcode & 0x00F0) >> 4];
            if (sum > 255)
            {
                memory.V[0xF] = 1;
                sum = 255;
            }
            memory.V[(memory.opcode & 0x0F00) >> 8] = (byte) sum;
        }

        /// <summary>
        /// 8xy5
        /// Set Vx = Vx - Vy, set VF = NOT borrow
        /// If Vx > Vy, then VF is set to 1, otherwise, 0. Then Vy is subtracted from Vx, and the
        /// results stored in Vx.
        /// </summary>
        public void SUB()
        {
            memory.V[0xF] = 1;
            int difference = memory.V[(memory.opcode & 0x0F00) >> 8] - memory.V[(memory.opcode & 0x00F0) >> 4];
            if(difference < 0)
            {
                memory.V[0xF] = 0; //Flag is not borrow
                difference = 0;
            }
            memory.V[(memory.opcode & 0x0F00) >> 8] = (byte) difference;
        }

        /// <summary>
        /// 8xy6
        /// Set Vx = Vx shifted right 1
        /// If the least-significant bit of Vx is 1, then VF is set to 1, otherwise 0.
        /// Then Vx is divided by 2
        /// </summary>
        public void SHR()
        {
            memory.V[0xF] = 0;
            if ((memory.V[(memory.opcode & 0x0F00) >> 8] & 0b0001) == 1) memory.V[0xF] = 1;
            memory.V[(memory.opcode & 0x0F00) >> 8] /= 2;
        }

        /// <summary>
        /// 8xy7
        /// Set Vx = Vy - Vx, set VF = NOT borrow
        /// Similar to SUB, just flip Vx and Vy
        /// </summary>
        public void SUBN()
        {
            memory.V[0xF] = 1;
            int difference = memory.V[(memory.opcode & 0x00F0) >> 4] - memory.V[(memory.opcode & 0x0F00) >> 8];
            if (difference < 0)
            {
                memory.V[0xF] = 0; //Flag is not borrow
                difference = 0;
            }
            memory.V[(memory.opcode & 0x0F00) >> 8] = (byte) difference;
        }

        /// <summary>
        /// 8xyE
        /// Set Vx = Vx shifted left 1
        /// If the most significant bit of Vx is 1, then VF is set to 1, otherwise to 0.
        /// Then Vx is multiplied by 2
        /// </summary>
        public void SHL()
        {
            memory.V[0xF] = 0;
            if ((memory.V[(memory.opcode & 0x0F00) >> 8] & 0b1000 >> 3) == 1) memory.V[0xF] = 1;
            memory.V[(memory.opcode & 0x0F00) >> 8] *= 2;
        }
    }
}
