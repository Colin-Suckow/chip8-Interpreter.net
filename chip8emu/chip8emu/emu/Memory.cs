using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chip8emu.emu
{

    public class Memory
    {
        private ushort[] map;
        public Dictionary<string, ushort> registers;  //TODO: Decide if a public dictionary is a better idea then a method

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
            registers = new Dictionary<string, ushort>
            {
                //General registers
                { "V0", 0 },
                { "V1", 0 },
                { "V2", 0 },
                { "V3", 0 },
                { "V4", 0 },
                { "V5", 0 },
                { "V6", 0 },
                { "V7", 0 },
                { "V8", 0 },
                { "V9", 0 },
                { "VA", 0 },
                { "VB", 0 },
                { "VC", 0 },
                { "VD", 0 },
                { "VE", 0 },
                { "VF", 0 },

                //Address register - only 16 bit register
                { "I", 0 },

                //Program counter
                { "PC", 0 },

                //Stack pointer
                { "SP", 0 },

                //Delay and sound timers
                { "Delay", 0 },
                { "Sound", 0 }
            };
        }


        public void UpdateTimers() {
            if (registers["Delay"] != 0) registers["Delay"]--;
            if (registers["Sound"] != 0) registers["Sound"]--;
        }

    }
}
