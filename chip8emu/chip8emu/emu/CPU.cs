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
        Instructions instructions;

        public CPU()
        {
            SetupSystem();
        }

        public void StepProcessor()
        {
            memory.PC += 2;
            CallOpcode(memory.opcode);
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

        public void CallOpcode(ushort opcode)
        {

        }
    }
}
