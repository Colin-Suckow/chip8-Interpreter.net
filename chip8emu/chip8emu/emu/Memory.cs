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

        public byte[,] screenBuffer; //Holds screen state for rendering

        ushort programStart = 0x200;

        public ushort opcode; //Stores current opcode - Might be moved in the future

        public Memory() {
            InitializeMemory();
            InitializeRegisters();
        }

        public ushort ReadByte(int location) {
            if (location < 0) location = 0;
            if (location > 4095) location = 4095;
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

        public void LoadBytes(int location, byte[] data)
        {
            for(int i = 0; i < data.Length; i++)
            {
                WriteByte(location + i, data[i]);
            }
        }

        private void InitializeRegisters() {
            V = new byte[16];
            stack = new ushort[16];
            screenBuffer = new byte[64,32]; //63 by 31 pixel display, counting 0
            I = 0;
            PC = programStart;
            SP = 0;
            delay = 0;
            sound = 0;
            LoadBytes(0, fontData);
        }


        public void UpdateTimers() {
            if (delay != 0) delay--;
            if (sound != 0) sound--;
        }


        byte[] fontData =
        {
            //0
            0xF0,
            0x90,
            0x90,
            0x90,
            0xF0,

            //1
            0x20,
            0x60,
            0x20,
            0x20,
            0x70,

            //2
            0xF0,
            0x10,
            0xF0,
            0x80,
            0xF0,

            //3
            0xF0,
            0x10,
            0xF0,
            0x10,
            0xF0,
            
            //4
            0x90,
            0x90,
            0xF0,
            0x10,
            0x10,

            //5
            0xF0,
            0x80,
            0xF0,
            0x10,
            0xF0,

            //6
            0xF0,
            0x80,
            0xF0,
            0x90,
            0xF0,

            //7
            0xF0,
            0x10,
            0x20,
            0x40,
            0x40,

            //8
            0xF0,
            0x90,
            0xF0,
            0x90,
            0xF0,

            //9
            0xF0,
            0x90,
            0xF0,
            0x10,
            0xF0,

            //A
            0xF0,
            0x90,
            0xF0,
            0x90,
            0x90,

            //B
            0xE0,
            0x90,
            0xE0,
            0x90,
            0xE0,

            //C
            0xF0,
            0x80,
            0x80,
            0x80,
            0xF0,

            //D
            0xE0,
            0x90,
            0x90,
            0x90,
            0xE0,

            //E
            0xF0,
            0x80,
            0xF0,
            0x80,
            0xF0,

            //F
            0xF0,
            0x80,
            0xF0,
            0x80,
            0x80
        };

    }
}
