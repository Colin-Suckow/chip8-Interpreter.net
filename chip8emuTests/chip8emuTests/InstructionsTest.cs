using System;
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
        public void SNE_BYTE_Test()
        {
            byte testValue = 0xaa;
            int testRegister = 2;
            ushort testOpcode = 0x42aa;
            memory.opcode = testOpcode;
            memory.PC = 0;
            memory.V[testRegister] = 0;

            instructions.SNE_BYTE();

            Assert.AreEqual(2, memory.PC, 0, "PC did not increment properly");
            

            memory.V[testRegister] = testValue;
            memory.PC = 0;

            instructions.SNE_BYTE();

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

        [TestMethod]
        [TestCategory("Instruction")]
        public void ADD_BYTE_Test()
        {
            ushort testOpcode = 0x71aa; //Add aa to V1
            memory.opcode = testOpcode;
            memory.V[1] = 5;
            instructions.ADD_BYTE();

            Assert.AreEqual(5 + 0xaa, memory.V[1], 0, "Did not add byte into register properly");

            //Check overflow condition - should top out at 255
            memory.V[1] = 250;
            instructions.ADD_BYTE();
            Assert.AreEqual(255, memory.V[1], 0, "Did not overflow correctly");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void LD_VY_Test()
        {
            ushort testOpcode = 0x8130; //Load V3 into V1
            memory.opcode = testOpcode;
            memory.V[1] = 0;
            memory.V[3] = 1;
            instructions.LD_VY();

            Assert.AreEqual(1, memory.V[1], 0, "Did not load VY into register properly");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void OR_Test()
        {
            ushort testOpcode = 0x8121; //Performs OR on V1 and V2
            memory.opcode = testOpcode;
            memory.V[1] = 0x0F;
            memory.V[2] = 0xF0;
            instructions.OR();

            // 0x0F AND 0xF0 = 0xFF
            Assert.AreEqual(0xFF, memory.V[1], 0, "Did not perform OR correctly");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void AND_Test()
        {
            ushort testOpcode = 0x8122; //Performs AND on V1 and V2
            memory.opcode = testOpcode;
            memory.V[1] = 0x0F;
            memory.V[2] = 0xF0;
            instructions.AND();

            // 0x0F AND 0xF0 = 0x00
            Assert.AreEqual(0x00, memory.V[1], 0, "Did not perform OR correctly");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void XOR_Test()
        {
            ushort testOpcode = 0x8122; //Performs XOR on V1 and V2
            memory.opcode = testOpcode;
            memory.V[1] = 0x0F;
            memory.V[2] = 0xF0;
            instructions.XOR();

            // 0x0F XOR 0xF0 = 0xFF
            Assert.AreEqual(0xFF, memory.V[1], 0, "Did not perform XOR correctly");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void ADD_Test()
        {
            ushort testOpcode = 0x8124; //Adds V2 to V1
            memory.opcode = testOpcode;
            memory.V[1] = 10;
            memory.V[2] = 20;
            memory.V[0xF] = 0;

            instructions.ADD();

            Assert.AreEqual(30, memory.V[1], 0, "Did not add correctly");

            memory.V[1] = 250;
            memory.V[2] = 50;
            memory.V[0xF] = 0;

            instructions.ADD();

            Assert.AreEqual(255, memory.V[1], 0, "Did not add correctly at overflow");
            Assert.AreEqual(1, memory.V[0xF], 0, "Did not correctly set overflow flag");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void SUB_Test()
        {
            ushort testOpcode = 0x8124; //Subtracts V2 from V1
            memory.opcode = testOpcode;
            memory.V[1] = 20;
            memory.V[2] = 10;
            memory.V[0xF] = 0;

            instructions.SUB();

            Assert.AreEqual(10, memory.V[1], 0, "Did not subtract correctly");
            Assert.AreEqual(1, memory.V[0xF], 0, "Did not correctly set underflow flag");

            memory.V[1] = 5;
            memory.V[2] = 10;
            memory.V[0xF] = 0;

            instructions.SUB();

            Assert.AreEqual(0, memory.V[1], 0, "Did not subtract correctly at underflow");
            Assert.AreEqual(0, memory.V[0xF], 0, "Did not correctly set underflow flag");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void SHR_Test()
        {
            ushort testOpcode = 0x8126; //Subtracts V2 from V1
            memory.opcode = testOpcode;
            memory.V[1] = 0xF0;
            memory.V[0xF] = 0;

            instructions.SHR();

            Assert.AreEqual(0xF0 / 2, memory.V[1], 0, "Did not shift correctly");

            memory.V[1] = 0xF1;
            memory.V[0xF] = 0;

            instructions.SHR();

            Assert.AreEqual(0xF1 / 2, memory.V[1], 0, "Did not shift correctly with flag");
            Assert.AreEqual(1, memory.V[0xF], 0, "Did not correctly set shift flag");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void SUBN_Test()
        {
            ushort testOpcode = 0x8127; //Subtracts V2 from V1
            memory.opcode = testOpcode;
            memory.V[1] = 10;
            memory.V[2] = 20;
            memory.V[0xF] = 0;

            instructions.SUBN();

            Assert.AreEqual(10, memory.V[1], 0, "Did not subtract correctly");
            Assert.AreEqual(1, memory.V[0xF], 0, "Did not correctly set underflow flag");

            memory.V[1] = 10;
            memory.V[2] = 5;
            memory.V[0xF] = 0;

            instructions.SUBN();

            Assert.AreEqual(0, memory.V[1], 0, "Did not subtract correctly at underflow");
            Assert.AreEqual(0, memory.V[0xF], 0, "Did not correctly set underflow flag");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void SHL_Test()
        {
            ushort testOpcode = 0x812E; //Subtracts V2 from V1
            memory.opcode = testOpcode;
            memory.V[1] = 0x0F;
            memory.V[0xF] = 0;

            instructions.SHL();

            Assert.AreEqual(0x0F * 2, memory.V[1], 0, "Did not shift correctly");

            memory.V[1] = 0x1F;
            memory.V[0xF] = 0;

            instructions.SHL();

            Assert.AreEqual(0x1F * 2, memory.V[1], 0, "Did not shift correctly with flag");
            Assert.AreEqual(1, memory.V[0xF], 0, "Did not correctly set shift flag");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void SNE_VY_Test()
        {
            
            ushort testOpcode = 0x9120; //Compares V1 and V2
            memory.opcode = testOpcode;
            memory.PC = 0;
            memory.V[1] = 0;
            memory.V[2] = 0;

            instructions.SNE_VY();

            Assert.AreEqual(0, memory.PC, 0, "PC changed values");


            memory.V[1] = 1;
            memory.PC = 0;

            instructions.SNE_VY();

            Assert.AreEqual(2, memory.PC, 0, "PC did not increment properly");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void LD_I_Test()
        {
            ushort testOpcode = 0xAFFF; //Set register to 0xFFF
            memory.opcode = testOpcode;
            memory.I = 0;

            instructions.LD_I();

            Assert.AreEqual(0xFFF, memory.I, 0, "Did not load I register properly");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void JP_V0_Test()
        {
            ushort testOpcode = 0xBaaa; //set pc to aaa pus V0
            memory.opcode = testOpcode;
            memory.V[0] = 5;
            memory.PC = 0;
            instructions.JP_V0();

            Assert.AreEqual(0xaaa + 5, memory.PC, 0, "Did not jump correctly");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void RND_Test()
        {
            //Can't really test randomness, so leaving blank
            //unless I think of something
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void DRW_Test()
        {
            //TODO: Tie into monogame to test drawing patterns
        }

        /*
         * TODO: Make tests for SKP and SKNP, these require keyboard input
         */


        [TestMethod]
        [TestCategory("Instruction")]
        public void LD_DT_Test()
        {
            ushort testOpcode = 0xF115; //Set delay to the value in V1
            memory.opcode = testOpcode;
            memory.V[1] = 0xFF;
            memory.delay = 0;
            instructions.LD_DT();

            Assert.AreEqual(0xFF, memory.delay, 0, "Did not set delay correctly");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void LD_ST_Test()
        {
            ushort testOpcode = 0xF118; //Set sound to the value in V1
            memory.opcode = testOpcode;
            memory.V[1] = 0xFF;
            memory.sound = 0;
            instructions.LD_ST();

            Assert.AreEqual(0xFF, memory.sound, 0, "Did not set sound correctly");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void ADD_I_Test()
        {
            ushort testOpcode = 0xF11E; //Add I to value in V1
            memory.opcode = testOpcode;
            memory.I = 5;
            memory.V[1] = 5;
            instructions.ADD_I();

            Assert.AreEqual(10, memory.I, 0, "Did not add to I correctly");
        }
        
        [TestMethod]
        [TestCategory("Instruction")]
        public void LD_F()
        {
            //TODO: Test for correct font loading once I add fonts to memory
        }
        
        [TestMethod]
        [TestCategory("Instruction")]
        public void LD_B()
        {
            ushort testOpcode = 0xF133; //Store BCD of value in V1
            memory.opcode = testOpcode;
            memory.V[1] = 123;
            memory.I = 1000; //Set to 1000 to avoid font data
            memory.WriteByte(memory.I, 0);
            memory.WriteByte(memory.I + 1, 0);
            memory.WriteByte(memory.I + 2, 0);
            instructions.LD_B();
            
            Assert.AreEqual(1, memory.ReadByte(memory.I), 0, "Hundreds place inccorect");
            Assert.AreEqual(2, memory.ReadByte(memory.I + 1), 0, "Tens place inccorect");
            Assert.AreEqual(3, memory.ReadByte(memory.I + 2), 0, "Ones place inccorect");
            
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void LD_TI_Test()
        {
            ushort testOpcode = 0xF355; //Store the data in V0 - V4 at position I
            memory.opcode = testOpcode;
            memory.I = 1000;
            memory.V[0] = 1;
            memory.V[1] = 2;
            memory.V[2] = 3;
            memory.V[3] = 4;

            instructions.LD_TI();

            Assert.AreEqual(1, memory.ReadByte(1000), 0, "Did not set 1st byte correctly");
            Assert.AreEqual(2, memory.ReadByte(1000 + 1), 0, "Did not set 2nd byte correctly");
            Assert.AreEqual(3, memory.ReadByte(1000 + 2), 0, "Did not set 3rd byte correctly");
            Assert.AreEqual(4, memory.ReadByte(1000 + 3), 0, "Did not set 4th byte correctly");
        }

        [TestMethod]
        [TestCategory("Instruction")]
        public void LD_TV()
        {
            ushort testOpcode = 0xF365; //Read 4 bytes into registers starting at I
            int startLocation = 1000;
            memory.WriteByte(startLocation, 1);
            memory.WriteByte(startLocation + 1, 2);
            memory.WriteByte(startLocation + 2, 3);
            memory.WriteByte(startLocation + 3, 4);
            memory.I = (ushort) startLocation;

            memory.V[0] = 0;
            memory.V[1] = 0;
            memory.V[2] = 0;
            memory.V[3] = 0;

            instructions.LD_TV();

            Assert.AreEqual(1, memory.ReadByte(startLocation), 0, "Did not read 1st byte correctly");
            Assert.AreEqual(2, memory.ReadByte(startLocation + 1), 0, "Did not read 2nd byte correctly");
            Assert.AreEqual(3, memory.ReadByte(startLocation + 2), 0, "Did not read 3rd byte correctly");
            Assert.AreEqual(4, memory.ReadByte(startLocation + 3), 0, "Did not read 4th byte correctly");
        }
    }
}
