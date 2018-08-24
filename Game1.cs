﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Windows.Graphics.Display;
using System;
using Windows.UI.ViewManagement;

namespace DontBreakTheRubber
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {

        const float SKYRATIO = 2f / 3f;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int score = 0;

        SpriteClass spikeBall;
        SpriteClass balloon;
        Texture2D startGameSplash;
        Texture2D gameOverTexture;

        SpriteFont scoreFont;
        SpriteFont stateFont;

        bool spaceDown;
        bool gameStarted;
        bool gameOver;
        bool timeToSpeedUp;

        float screenWidth;
        float screenHeight;

        float ballBounceSpeed;
        float gravitySpeed;
        float spinSpeed;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;


            screenHeight = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Height);
            screenWidth = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Width);

            spaceDown = false;
            gameStarted = false;
            gameOver = false;
            timeToSpeedUp = false;
            spinSpeed = 7f;

            ballBounceSpeed = ScaleToHighDPI(-1200f);
            gravitySpeed = ScaleToHighDPI(30f);
            score = 0;
            this.IsMouseVisible = false;

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            startGameSplash = Content.Load<Texture2D>("start-splash");
            gameOverTexture = Content.Load<Texture2D>("game-over");


            spikeBall = new SpriteClass(GraphicsDevice, "Content/characterSprite.png", ScaleToHighDPI(1f));
            balloon = new SpriteClass(GraphicsDevice, "Content/characterSprite.png", ScaleToHighDPI(1f));

            scoreFont = Content.Load<SpriteFont>("Score");
            stateFont = Content.Load<SpriteFont>("GameState");
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
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds; // Get time elapsed since last Update iteration


            KeyboardHandler();

            if (gameOver)
            {
                spikeBall.dX = 0;
                spikeBall.dY = 0;
                spikeBall.angle = 100;
                spikeBall.dA = 0;
                balloon.dX = 0;
                balloon.dY = 0;
            }

            spikeBall.Update(elapsedTime);
            balloon.Update(elapsedTime);

            spikeBall.dY += gravitySpeed;

            if (spikeBall.y > screenHeight * SKYRATIO)
            {
                spikeBall.dY = 0;
                spikeBall.y = screenHeight * SKYRATIO;
            }
            
            if (spikeBall.RectangleCollision(balloon))
            {
                float tempAngle = ((float)RadianToDegree(spikeBall.angle) % 360);
                if((tempAngle > 179 || tempAngle == 0) && !gameOver && gameStarted)
                {
                    bounce();
                }
                else
                {
                    gameOver = true;
                }
            }

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue); // Clear the screen

            spriteBatch.Begin();
            if (gameOver)
            {
                // Draw game over texture
                spriteBatch.Draw(gameOverTexture, new Vector2(screenWidth / 2 - gameOverTexture.Width / 2, screenHeight / 4 - gameOverTexture.Width / 2), Color.White);

                String pressEnter = "Press Enter to restart!";

                // Measure the size of text in the given font
                Vector2 pressEnterSize = stateFont.MeasureString(pressEnter);

                // Draw the text horizontally centered
                spriteBatch.DrawString(stateFont, pressEnter, new Vector2(screenWidth / 2 - pressEnterSize.X / 2, screenHeight - 200), Color.White);

                // If the game is over, draw the score in red
                spriteBatch.DrawString(scoreFont, score.ToString(), new Vector2(screenWidth - 100, 50), Color.Red);
            }
            else
            {
                spriteBatch.DrawString(scoreFont, score.ToString(), new Vector2(screenWidth - 100, 50), Color.Black);
            }

            spikeBall.Draw(spriteBatch);
            balloon.Draw(spriteBatch);

            if (!gameStarted)
            {
                // Fill the screen with black before the game starts
                spriteBatch.Draw(startGameSplash, new Rectangle(0, 0, (int)screenWidth, (int)screenHeight), Color.White);

                String title = "PARTY BOUNCE";
                String pressSpace = "Press Space to start";

                // Measure the size of text in the given font
                Vector2 titleSize = stateFont.MeasureString(title);
                Vector2 pressSpaceSize = stateFont.MeasureString(pressSpace);

                // Draw the text horizontally centered
                spriteBatch.DrawString(stateFont, title, new Vector2(screenWidth / 2 - titleSize.X / 2, screenHeight / 3), Color.ForestGreen);
                spriteBatch.DrawString(stateFont, pressSpace, new Vector2(screenWidth / 2 - pressSpaceSize.X / 2, screenHeight / 2), Color.White);
            }

            spriteBatch.End();


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public float ScaleToHighDPI(float f)
        {
            DisplayInformation d = DisplayInformation.GetForCurrentView();
            f *= (float)d.RawPixelsPerViewPixel;
            return f;
        }


        public void StartGame()
        {
    
            spikeBall.x = screenWidth / 2;
            spikeBall.y = screenHeight * SKYRATIO;
            spikeBall.angle = 100;
            spikeBall.dA = 0;
            balloon.x = screenWidth / 2;
            balloon.y = screenHeight * SKYRATIO;
            score = -5;
            spinSpeed = 7f;
        }

        void bounce()
        {
            score++;
            spikeBall.dY = ballBounceSpeed;
            if(score > 0 && (score % 2 == 0))
            {
                spinSpeed *= (float)1.25;
            }
            spikeBall.dA = spinSpeed;
        }

        void KeyboardHandler()
        {
            KeyboardState state = Keyboard.GetState();

            // Quit the game if Escape is pressed.
            if (state.IsKeyDown(Keys.Escape)) Exit();

            // Start the game if Space is pressed.
            if (!gameStarted)
            {
                if (state.IsKeyDown(Keys.Space))
                {
                    StartGame();
                    gameStarted = true;
                    spaceDown = true;
                    gameOver = false;
                }
                return;
            }

            // Restart the game if Enter is pressed
            if (gameOver)
            {
                if (state.IsKeyDown(Keys.Enter))
                {
                    StartGame();
                    gameOver = false;
                }
            }

            // stop spinning if space is pressed
            if (state.IsKeyDown(Keys.Space))
            {
                // stop spinning once space is released
                if (!spaceDown)
                {
                    spikeBall.dA = 0;
                    spikeBall.angle = spikeBall.angle;
                }

                spaceDown = true;
            }
            else spaceDown = false;
        }

        double RadianToDegree(float angle)
        {
            return angle * (180.0 / Math.PI);
        }

    }
}
