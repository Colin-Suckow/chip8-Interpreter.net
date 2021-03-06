﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chip8emu.util;

namespace chip8emu.emu
{
    public class Instructions
    {
        // Chip8 Technical Reference
        // http://devernay.free.fr/hacks/chip8/C8TECH10.HTM#1.0

        Memory memory;

        Random rand;
        util.util util; //TODO: Figure out abstract class

        public Instructions(Memory memory)
        {
            this.memory = memory;
            rand = new Random();
            util = new util.util();
        }

        ///Clears the display
        public void CLS()
        {
            for(int i = 0; i < memory.screenBuffer.GetLength(0); i++)
            {
                for(int j = 0; j < memory.screenBuffer.GetLength(1); j++)
                {
                    memory.screenBuffer[i, j] = 0;
                }
            }
        }

        /// <summary>
        /// 00EE
        /// Return from a subroutine
        /// The interpreter sets the program counter to the address at the top of the stack, then subtracts 1 from the stack pointer.
        /// </summary>
        public void RET()
        {
            memory.PC = memory.stack[memory.SP];
            if(memory.SP > 0)
            {
                memory.SP--;
            }
            
        }

        /// <summary>
        /// 1nnn
        /// Jump to location nnn
        /// The interpreter sets the program counter to nnn
        /// </summary>
        public void JP()
        {
            memory.PC = (ushort) (memory.opcode & 0x0FFF);
           
            memory.PC -= 2; //JP shouldn't increment PC, so subtract to cancel out the addition later
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
            memory.PC -= 2; //Call shouldn't increment PC, so subtract to cancel out later addition
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
        public void SNE_BYTE()
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
            memory.V[(memory.opcode & 0x0F00) >> 8] += (byte) (memory.opcode & 0x00FF);
        }   

        /// <summary>
        /// 8xy0
        /// Set Vx = Vy
        /// Stores the value of register Vy in register Vx
        /// </summary>
        public void LD_VY()
        {
            memory.V[(memory.opcode & 0x0F00) >> 8] = memory.V[(memory.opcode & 0x00F0) >> 4];
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
            memory.V[(memory.opcode & 0x0F00) >> 8] = (byte) (memory.V[(memory.opcode & 0x0F00) >> 8] & memory.V[(memory.opcode & 0x00F0) >> 4]);
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
            } else
            {
                memory.V[0xF] = 0;
            }
            sum = sum & 0x00FF;
            memory.V[(memory.opcode & 0x0F00) >> 8] = (byte)(sum);
        }

        /// <summary>
        /// 8xy5
        /// Set Vx = Vx - Vy, set VF = NOT borrow
        /// If Vx > Vy, then VF is set to 1, otherwise, 0. Then Vy is subtracted from Vx, and the
        /// results stored in Vx.
        /// </summary>
        public void SUB()
        {
            ;
            int difference = memory.V[(memory.opcode & 0x0F00) >> 8] - memory.V[(memory.opcode & 0x00F0) >> 4];
            if (memory.V[(memory.opcode & 0x00F0) >> 4] < memory.V[(memory.opcode & 0x0F00) >> 8])
            {
                memory.V[0xF] = 1;
            } else
            {
                memory.V[0xF] = 0;
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
            
            int difference = memory.V[(memory.opcode & 0x00F0) >> 4] - memory.V[(memory.opcode & 0x0F00) >> 8];
            if (memory.V[(memory.opcode & 0x00F0) >> 4] > memory.V[(memory.opcode & 0x0F00) >> 8])
            {
                memory.V[0xF] = 1;
            } else
            {
                memory.V[0xF] = 0;
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
            if ((memory.V[(memory.opcode & 0x0F00) >> 8] & 0x80) == 0x80) memory.V[0xF] = 1;
            memory.V[(memory.opcode & 0x0F00) >> 8] *= 2;
        }

        /// <summary>
        /// 9xy0
        /// Skip next instruction if Vx != Vy
        /// The values of Vx and Vy are compared, and if they are not equal, the program
        /// counter is increased by 2
        /// </summary>
        public void SNE_VY()
        {
            if (memory.V[(memory.opcode & 0x0F00) >> 8] != memory.V[(memory.opcode & 0x00F0) >> 4]) memory.PC += 2;
        }

        /// <summary>
        /// Annn
        /// Set I = nnn
        /// The value of the I register is set to nnn.
        /// </summary>
        public void LD_I()
        {
            memory.I = (ushort) (memory.opcode & 0x0FFF);
        }

        /// <summary>
        /// Bnnn
        /// Jump to location nnn + v0
        /// The program counter is set to nnn plus the value of V0
        /// </summary>
        public void JP_V0()
        {
            memory.PC = (ushort) ((memory.opcode & 0x0FFF) + memory.V[0]);
        }

        /// <summary>
        /// Cxkk
        /// Set Vx = random byte AND kk
        /// The interpreter generates a random number from 0 to 255, which is then ANDed with
        /// the value kk. The results are then stored in Vx
        /// TODO: Figure out how to test
        /// </summary>
        public void RND()
        {
            int random = rand.Next(0, 255) & (memory.opcode & 0x00FF);
            memory.V[(memory.opcode & 0x0F00) >> 8] = (byte) random;
        }

        /// <summary>
        /// Dxyn
        /// Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision
        /// Check docs for full details, too long to put here
        /// </summary>
        public void DRW()
        {
            short x = memory.V[(memory.opcode & 0x0F00) >> 8];
            short y = memory.V[(memory.opcode & 0x00F0) >> 4];
            short height = (short)(memory.opcode & 0x000F);
            byte pixel;

            memory.V[0xF] = 0;
            for (int yline = 0; yline < height; yline++)
            {
                pixel = (byte)memory.ReadByte(memory.I + yline);
                for (int xline = 0; xline < 8; xline++)
                {
                    if ((pixel & (0x80 >> xline)) != 0)
                    {

                        if ((memory.screenBuffer[(x + xline) % 64, (y + yline) % 32]) != 0)
                        {
                            memory.V[0xF] = 1;
                        }

                        memory.screenBuffer[(x + xline) % 64, (y + yline) % 32] = (byte)(memory.screenBuffer[(x + xline) % 64, (y + yline) % 32] ^ 1);
                    }
                }
            }
        }

        /// <summary>
        /// Ex9E
        /// Skip the next value if the key with the value of Vx is pressed
        /// Checks the keyboard, if the key corresponding to the value of Vx is currently
        /// in the down position, PC is increased by 2.
        /// </summary>
        public void SKP()
        {
            int key = memory.V[(memory.opcode & 0x0F00) >> 8];
            if(memory.keyState[key])
            {
                memory.PC += 2;
            }
        }


        /// <summary>
        /// ExA1
        /// Skip the next value if the key with the value of Vx is not pressed
        /// Checks the keyboard, if the key corresponding to the value of Vx is currently
        /// in the up position, PC is increased by 2.
        /// </summary>
        public void SKNP()
        {
            int key = memory.V[(memory.opcode & 0x0F00) >> 8];
            if (!memory.keyState[key])
            {
                memory.PC += 2;
            }
        }

        /// <summary>
        /// Fx0A
        /// Wait for a keypress and store the result in register VX
        /// </summary>
        public void KEY_HALT()
        {
            Boolean keyPressed = false;
            for(int i = 0; i < memory.keyState.Length; i++)
            {
                if(memory.keyState[i])
                {
                    keyPressed = true;
                    memory.V[(memory.opcode % 0x0F00) >> 8] = (byte) i;
                    break;
                } 
            }
            if (!keyPressed) memory.PC -= 2;
        }

        /// <summary>
        /// Fx07
        /// Set Vx = delay timer value
        /// The value of DT is placed into Vx
        /// </summary>
        public void LD_VX_DT()
        {
            memory.V[(memory.opcode & 0x0F00) >> 8] = memory.delay;
        }

        /// <summary>
        /// Fx15
        /// Set delay timer = Vx
        /// Delay is set equal to the value of Vx
        /// </summary>
        public void LD_DT()
        {
            memory.delay = memory.V[(memory.opcode & 0x0F00) >> 8];
        }

        /// <summary>
        /// Fx18
        /// Set sound timer = Vx
        /// Sound is set equal to the value of Vx
        /// </summary>
        public void LD_ST()
        {
            memory.sound = memory.V[(memory.opcode & 0x0F00) >> 8];
        }

        /// <summary>
        /// Fx1E
        /// Set I = I + Vx
        /// The values of I and Vx are added, and the results are stored in I
        /// </summary>
        public void ADD_I()
        {
            memory.I += memory.V[(memory.opcode & 0x0F00) >> 8];
        }

        /// <summary>
        /// Fx29
        /// Set I = location of sprite for digit Vx
        /// The value of I is set to the memory location for the font sprite of the value in Vx
        /// </summary>
        public void LD_F()
        {
            int num = memory.V[(memory.opcode & 0x0F00) >> 8];
            memory.I = (ushort) (num * 5);
        }

        /// <summary>
        /// Fx33
        /// Store BCD representation of Vx in memory locations I, I + 1, and I + 2
        /// The interpreter takes the decimal value of Vx, and 
        /// places the hundreds digit in memory at location in I, the tens digit at location I+1, 
        /// and the ones digit at location I+2.
        /// </summary>
        public void LD_B()
        {
            int[] BCD = util.GetIntArray( memory.V[(memory.opcode & 0x0F00) >> 8] );
            Console.WriteLine("Array length: " + BCD.Length);
            for (int i = 0; i < BCD.Length; i++)
            {
                Console.WriteLine(BCD[i]);
                memory.WriteByte(memory.I + i, (ushort)BCD[i]);
            }
        }

        /// <summary>
        /// Fx55
        /// Store register V0 through Vx in memory starting at location I
        /// The interpreter copies the values of registers V0 through Vx into memory, starting at the address in I.
        /// </summary>
        public void LD_TI()
        {
            int length = (memory.opcode & 0x0F00) >> 8;
            for(int i = 0; i <= length; i++)
            {
                memory.WriteByte(memory.I + i, memory.V[i]);
            }
        }

        /// <summary>
        /// Fx65
        /// Read registers V0 through Vx from memory starting at I
        /// The interpreter reads values from memory starting at location I into registers V0 through Vx
        /// </summary>
        public void LD_TV()
        {
            int length = (memory.opcode & 0x0F00) >> 8;
            for(int i = 0; i <= length; i++)
            {
                memory.V[i] = (byte) memory.ReadByte(memory.I + i);
            }
        }

    }
}
