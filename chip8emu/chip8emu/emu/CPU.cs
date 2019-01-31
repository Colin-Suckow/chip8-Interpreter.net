using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chip8emu.emu
{
    public class CPU
    {
        public Memory memory;
        Instructions instructions;

        public CPU()
        {
            SetupSystem();
        }

        public void StepProcessor()
        {
            
            memory.opcode = (ushort) ((memory.ReadByte(memory.PC) << 8) | memory.ReadByte(memory.PC + 1));
            Console.WriteLine(memory.PC.ToString("X") + " : " + memory.opcode.ToString("X") + " | int: " + memory.opcode.ToString());
            CallOpcode();
            memory.PC += 2;

        }

        //Call at 60hz
        public void StepTimers()
        {
            memory.UpdateTimers();
        }

        public void SetupSystem()
        {
            memory = new Memory();
            instructions = new Instructions(memory);
        }

        public void ResetSystem()
        {
            memory.Reset();
        }

        public void LoadProgram(byte[] data)
        {
            memory.LoadBytes(0x200, data);
        }

        public byte[,] GetScrenBuffer()
        {
            return memory.screenBuffer;
        }

        /// <summary>
        /// Calls proper instruction based on opcode
        /// </summary>
        private void CallOpcode()
        {
            switch ((memory.opcode & 0xF000) >> 12)
            {
                case 0x0:
                    switch(memory.opcode & 0x00FF)
                    {
                        case 0xE0: //CLS
                            break;

                        case 0xEE: //RET
                            instructions.RET();
                            break;

                        default: //Probably SYS
                            break;
                    }
                    break;

                case 0x1: //JP
                    instructions.JP();
                    break;

                case 0x2: //CALL
                    instructions.CALL();
                    break;

                case 0x3: //SE_BYTE
                    instructions.SE_BYTE();
                    break;

                case 0x4: //SNE
                    instructions.SNE_BYTE();
                    break;

                case 0x5: //SE_VY
                    instructions.SE_VY();
                    break;

                case 0x6: //LD_VX
                    instructions.LD_BYTE();
                    break;

                case 0x7: //ADD_BYTE
                    instructions.ADD_BYTE();
                    break;

                case 0x8:
                    switch(memory.opcode & 0x000F)
                    {
                        case 0x0: //LD_VY
                            instructions.LD_VY();
                            break;

                        case 0x1: //OR
                            instructions.OR();
                            break;

                        case 0x2: //AND
                            instructions.ADD();
                            break;

                        case 0x3: //XOR
                            instructions.XOR();
                            break;

                        case 0x4: //ADD
                            instructions.ADD();
                            break;

                        case 0x5: //SUB
                            instructions.SUB();
                            break;

                        case 0x6: //SHR
                            instructions.SHR();
                            break;

                        case 0x7: //SUBN
                            instructions.SUBN();
                            break;

                        case 0xE: //SHL
                            instructions.SHL();
                            break;

                        default:
                            break;

                    }
                    break;

                case 0x9: //SNE_VY
                    instructions.SNE_VY();
                    break;

                case 0xA: //LD_I
                    instructions.LD_I();
                    break;

                case 0xB: //JP_V)
                    instructions.JP_V0();
                    break;

                case 0xC: //RND
                    instructions.RND();
                    break;

                case 0xD: //DRW
                    instructions.DRW();
                    break;

                case 0xE:
                    switch (memory.opcode & 0x000F)
                    {
                        case 0xE: //SKP
                            instructions.SKP();
                            break;

                        case 0x1: //SKNP
                            instructions.SKNP();
                            break;

                        default:
                            break;
                    }
                    break;

                case 0xF:
                    switch (memory.opcode & 0x00FF)
                    {
                        case 0x07: //LD_DELAY
                            instructions.LD_VX_DT();
                            break;

                        case 0x0A: //LD_K
                            break;

                        case 0x15: //LD_DT
                            instructions.LD_DT();
                            break;

                        case 0x18: //LD_ST
                            instructions.LD_ST();
                            break;

                        case 0x1E:
                            instructions.ADD_I();
                            break;

                        case 0x29:
                            instructions.LD_F();
                            break;

                        case 0x33: //LD_B
                            instructions.LD_B();
                            break;

                        case 0x55: //LD_TI
                            instructions.LD_TI();
                            break;

                        case 0x65:
                            instructions.LD_TV();
                            break;

                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
