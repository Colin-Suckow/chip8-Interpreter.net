﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using chip8emu.emu;

namespace chip8emuTests
{
    [TestClass]
    public class InstructionsTest
    {   
        Memory memory;
        Instructions instructions;

        [TestInitialize]
        public void Initialize()
        {
            memory = new Memory();
            instructions = new Instructions(memory);
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void CLS_Test()
        {
            //TODO: Tie into monogame to check if it actually cleared screen
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void RET_Test()
        {
            byte testLocation = 5;
            byte testValue = 10;

            memory.SP = testValue;
            memory.stack[testLocation] = testValue;
            instructions.RET();

            Assert.AreEqual(memory.stack[testLocation], testValue, 0, "Did not set PC equal to the stack value at SP");
            Assert.AreEqual(memory.SP, testValue - 1, 0, "Did not deincrement SP properly");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void JP_Test()
        {
            ushort testOpcode = 0x1111; //Op code to set PC to 0x0111
            memory.opcode = testOpcode;
            memory.PC = 0x0;
            memory.SP = 10;

            instructions.JP();

            Assert.AreEqual(0x0111, memory.PC, 0, "Did not properly set PC to opcode value");
            Assert.AreEqual(9, memory.SP, 0, "Did not properly deincrement SP");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void CALL_Test()
        {
            ushort testOpcode = 0x2111;
            memory.opcode = testOpcode;
            memory.SP = 0;
            memory.PC = testOpcode;

            instructions.CALL();

            Assert.AreEqual(1, memory.SP, 0, "Did not properly increment SP");
            Assert.AreEqual(testOpcode, memory.stack[memory.SP], 0, "Did not properly store current opcode");
            Assert.AreEqual(0x0111, memory.PC, 0, "Did not properly set PC to input value");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void SE_Test()
        {
            byte testValue = 0xaa;
            int testRegister = 2;
            ushort testOpcode = 0x32aa;
            memory.opcode = testOpcode;
            memory.PC = 0;
            memory.V[testRegister] = 0;

            instructions.SE_BYTE();

            Assert.AreEqual(0, memory.PC, 0, "PC changed values");

            memory.V[testRegister] = testValue;

            instructions.SE_BYTE();
            
            Assert.AreEqual(2, memory.PC, 0, "PC did not increment properly");      

        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void SNE_Test()
        {
            byte testValue = 0xaa;
            int testRegister = 2;
            ushort testOpcode = 0x42aa;
            memory.opcode = testOpcode;
            memory.PC = 0;
            memory.V[testRegister] = 0;

            instructions.SNE();

            Assert.AreEqual(2, memory.PC, 0, "PC did not increment properly");
            

            memory.V[testRegister] = testValue;
            memory.PC = 0;

            instructions.SNE();

            Assert.AreEqual(0, memory.PC, 0, "PC changed values");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void SE_VY_Test()
        {
            ushort testOpcode = 0x5120; //Compare registers 1 and 2
            memory.opcode = testOpcode;
            memory.PC = 0;
            memory.V[1] = 2;
            memory.V[2] = 3;
            instructions.SE_VY();
            Assert.AreEqual(0, memory.PC, 0, "PC changed values");

            memory.PC = 0;
            memory.V[1] = 2;
            memory.V[2] = 2;
            instructions.SE_VY();
            Assert.AreEqual(2, memory.PC, 0, "Pc did not increment properly");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void LD_BYTE_Test()
        {
            ushort testOpcode = 0x61aa; //Load aa into V1
            memory.opcode = testOpcode;
            memory.V[1] = 0;
            instructions.LD_BYTE();

            Assert.AreEqual(0xaa, memory.V[1], 0, "Did not load byte into register properly");
        }

    }
}
