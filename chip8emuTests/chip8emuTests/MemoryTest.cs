using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using chip8emu.emu;

namespace chip8emuTests
{
    [TestClass]
    public class MemoryTest
    {
        [TestMethod]
        public void Memory_Write_And_Read()
        {
            Memory memory = new Memory();
            ushort writeValue = 10;

            memory.WriteByte(0, writeValue);
            memory.WriteByte(4095, writeValue);

            ushort memLocation1 = memory.ReadByte(0);
            ushort memLocation2 = memory.ReadByte(4095);

            Assert.AreEqual(writeValue, memLocation1, 0, "Failed lower memory bound check");
            Assert.AreEqual(writeValue, memLocation2, 0, "Failed upper memory bound check");  
        }

        [TestMethod]
        public void Register_Write_And_Read()
        {
            Memory memory = new Memory();
            byte writeValue = 10;

            memory.V[0x0] = writeValue;

            Assert.AreEqual(writeValue, memory.V[0x0]);
        }

        [TestMethod]
        public void Delay_And_Sound_Update()
        {
            Memory memory = new Memory();

            //Cycle the timers once and check to make sure they dont deincrement when their value is 0
            memory.UpdateTimers(); 
            Assert.AreEqual(0, memory.delay, 0, "Deincremented when 0");
            Assert.AreEqual(0, memory.sound, 0, "Deincremented when 0");

            byte writeValue = 10;

            memory.delay = writeValue;
            memory.sound = writeValue;
            memory.UpdateTimers();

            Assert.AreEqual(writeValue - 1, memory.delay, 0, "Did not deincrement delay properly");
            Assert.AreEqual(writeValue -1, memory.sound, 0, "Did not deincrement sound properly");

        }
    }
}
