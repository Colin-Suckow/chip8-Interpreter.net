using chip8emu.emu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace chip8emu
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState oldState;
        Texture2D canvas;

        private SpriteFont font;

        string filePath = "C:\\Users\\CS\\Downloads\\BC_test.ch8";
        //BC_test
        CPU cpu;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            cpu = new CPU();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            cpu.SetupSystem();
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("debug");

            byte[] data = File.ReadAllBytes(filePath);

            for(int i = 0; i < data.Length - 1; i+=2)
            {
                Console.WriteLine(data[i].ToString("X") + data[i + 1].ToString("X"));
            }

            cpu.LoadProgram(data);

            

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState newState = Keyboard.GetState();

            //if(oldState.IsKeyDown(Keys.Space) && newState.IsKeyUp(Keys.Space))
            //{
                cpu.StepProcessor();
                cpu.StepTimers();
            //}
           
            oldState = newState;

            //Console.WriteLine(cpu.memory.opcode);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
           

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null);
            this.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            Rectangle renderWindowRect = new Rectangle(40, 50, 64 * 10, 32 * 10);

            spriteBatch.DrawString(font, getMonitorText(-3), new Vector2(700, 140), Color.Black);
            spriteBatch.DrawString(font, getMonitorText(-2), new Vector2(700, 160), Color.Black);
            spriteBatch.DrawString(font, getMonitorText(-1), new Vector2(700, 180), Color.Black);
            spriteBatch.DrawString(font, getMonitorText(0), new Vector2(700, 200), Color.Red);
            spriteBatch.DrawString(font, getMonitorText(1), new Vector2(700, 220), Color.Black);
            spriteBatch.DrawString(font, getMonitorText(2), new Vector2(700, 240), Color.Black);
            spriteBatch.DrawString(font, getMonitorText(3), new Vector2(700, 260), Color.Black);

            spriteBatch.Draw(RenderScreen(GraphicsDevice, cpu.GetScrenBuffer(), 10), renderWindowRect, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        String getMonitorText(int position)
        {
            return "0x" + (cpu.memory.PC + position * 2).ToString("X") + " : " + cpu.memory.ReadByte(cpu.memory.PC + position * 2).ToString("X") + cpu.memory.ReadByte(cpu.memory.PC + (position * 2) + 1).ToString("X");
        }

        RenderTarget2D RenderScreen(GraphicsDevice device, byte[,] screenBuffer, int scaleFactor)
        {
            int width = screenBuffer.GetLength(0);
            int height = screenBuffer.GetLength(1);
            RenderTarget2D texture = new RenderTarget2D(device, width, height);
            Color[] textureData = new Color[width * height];

            for(int i = 0; i < textureData.Length; i++)
            {
                int textureX = i % width;
                int textureY = i / width;
                
                
                if(screenBuffer[textureX, textureY] != 0)
                {
                    textureData[i] = Color.White;
                } else
                {
                    textureData[i] = Color.Black;
                }
                
            }


            GraphicsDevice.Textures[0] = null;
            texture.SetData(textureData);
            

            return texture;
        }

        void PrintScreenBuffer()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            for(int i = 0; i < cpu.memory.screenBuffer.GetLength(1); i++)
            {
                for(int j = 0; j < cpu.memory.screenBuffer.GetLength(0); j++)
                {
                    if(cpu.memory.screenBuffer[j, i] == 1)
                    {
                        Console.Write("#");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine("");
            }
        }
    }
}
