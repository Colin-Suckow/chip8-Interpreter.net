using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chip8emu.emu
{

    public class Memory
    {
        private ushort[] map; //Main system memory map - ushort because of the I register
        public byte[] V;  //V0 - VF Registers
        public ushort I; //I register
        public ushort PC; //Program Counter
        public byte SP; //Stack pointer
        public ushort[] stack; //Address stack
        public byte delay; //Delay timer
        public byte sound; //Sound timer

        public ushort opcode; //Stores current opcode - Might be moved in the future

        public Memory() {
            InitializeMemory();
            InitializeRegisters();
        }

        public ushort ReadByte(int location) {
            return map[location];
        }

        public void WriteByte(int location, ushort data) {
            map[location] = data;
        }

        public ushort[] GetMemoryMap() {
            return map;
        }

        public void Reset() {
            InitializeMemory();
            InitializeRegisters();
        }

        private void InitializeMemory() {
            map = new ushort[4096];
        }

        private void InitializeRegisters() {
            V = new byte[0xF];
            stack = new ushort[16];
            I = 0;
            PC = 0;
            SP = 0;
            delay = 0;
            sound = 0;
        }


        public void UpdateTimers() {
            if (delay != 0) delay--;
            if (sound != 0) sound--;
        }

    }
}
