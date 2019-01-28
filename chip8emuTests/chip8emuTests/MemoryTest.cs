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

        }
    }
}
