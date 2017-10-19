using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace LemmingsRevival
{
    //Enumeration of the GameStates
    public enum GameState
    {
        SplashScreen,
        Menu,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        Pause,
        GameOver,
        Credits
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Rectangle mainFrame; //Screensize
        SpriteFont mFont; //Custom-Font

        //Splashscreen, menu, start & exit buttons
        Texture2D splashScreen;
        Texture2D mBackground;
        Texture2D credits;
        Texture2D startButton;
        Texture2D exitButton;
        Texture2D creditsButton;
        
        //Creditscreen
        Texture2D backButton;
        
        //Gameoverscreen
        Texture2D gameover;
        Texture2D retryButton;

        //Background & SRH-Tower
        Texture2D background;
        Texture2D background2;
        Texture2D tower;
        Texture2D pyramid;

        //Level
        Texture2D level1;
        Texture2D level2;
        Texture2D level3;
        Texture2D level4;
        Texture2D level5;
        Texture2D currentLevel;

        //UI
        Texture2D UI;
        Texture2D speedButton;
        Texture2D blockButton;
        Texture2D parachuteButton;
        Texture2D phaseButton;
        Texture2D continueButton;
        Texture2D pauseButton;
        Texture2D endButton;

        //Minimap
        Texture2D minimap;
        Texture2D blackbg;

        //Rectangles for CheckClick() Event
        Rectangle startButtonRect;
        Rectangle exitButtonRect;
        Rectangle backRect;
        Rectangle creditsButtonRect;
        Rectangle parachuteRect;
        Rectangle phaseRect;
        Rectangle blockRect;
        Rectangle finishRect;
        Rectangle finishTrigger;
        Rectangle speedRect;
        Rectangle continueRect;
        Rectangle retryRect;
        Rectangle pauseRetryRect;
        Rectangle pauseRect;
        Rectangle endRect;

        Rectangle noEscape; //Needed because game crashes if lemming phases at lowest level
        Rectangle minimapRect; //Minimap

        //New lemming (instance of Lemming class)
        List<Lemming> Lemmings = new List<Lemming>();

        //Removal List
        List<Lemming> LemmingsToRemove = new List<Lemming>();

        //Spawn & Exit Trigger
        Vector2 spawn;
        Texture2D portal;
        Texture2D finish;
        
        public Camera cam = new Camera(); //Main Camera "cam"
        public Vector2 parallax; //Parallax-Scrolling

        //Per-Pixel colordata for collision  
        public Color[] level1TextureData;  
        public Color[] level2TextureData;
        public Color[] level3TextureData;
        public Color[] level4TextureData;
        public Color[] level5TextureData;
        public Color[] currentTextureData;

        MouseState previousMouseState; //Previous Mouse-State
        KeyboardState oldState;  //Previous Keyboard-State
        GameState gameState; //GameStates
        State state = State.None; //Standard AbilityState
        
        bool speedClicked; //Has the speed button been clicked?
        bool gamePaused; //Is the game Paused?

        int startCounter = 180 * 1000; //120 Second counter
        int counter = 0; //Initialize counter
        int spawnRate; //Spawnrate
        int counterX; //Counter multiplier
        int lemmingsSaved; //Counter how many Lemmings have been saved
        int lemmingsDead; //Counter how many Lemmings were killed
        int totalLemmingsSaved; //Counter how many Lemmings were killed in total
        int totalLemmingsKilled; //Counter how many Lemmings have been saved in total

        //Max uses of the abilities
        int maxUsesParachute;
        int maxUsesPhase;
        int maxUsesBlock;

        Song song; // Background music
        
        DateTime gameendtime; //Time the game runs until gameoverscreen is drawed
        
        public Game1()
        { 
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content"; //Content root directory LemmingsRevivalContent (Content)
            
            graphics.PreferredBackBufferWidth = 1280; //Set window-size to 1280x720 
            graphics.PreferredBackBufferHeight = 720;

            this.IsMouseVisible = true; //Show cursor
            cam.position = new Vector2(0.0f, 0.0f); //Set camera-position to 0px, 0px
            parallax = new Vector2(1.0f); //Set Scroll-Parallax value to 1.0f
            gameState = GameState.SplashScreen; //GameState starting with SplashScreen
            counterX = 1; //Standard speed
            speedClicked = false; //4x speed is deactivated at start
        }

        protected override void Initialize()
        {    
            base.Initialize();
        }
       
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Splashscreen, menu, start & exit buttons
            splashScreen = Content.Load<Texture2D>("Textures\\Menu\\splashscreen");
            mBackground = Content.Load<Texture2D>("Textures\\Menu\\menu");
            startButton = Content.Load<Texture2D>("Textures\\Menu\\startbutton");
            exitButton = Content.Load<Texture2D>("Textures\\Menu\\exitbutton");
            creditsButton = Content.Load<Texture2D>("Textures\\Menu\\creditsbutton");
            

            //Start & Exit Buttons
            startButtonRect = new Rectangle(GraphicsDevice.Viewport.Width / 2 - 250, GraphicsDevice.Viewport.Height - 250, 220, 110);
            exitButtonRect = new Rectangle(GraphicsDevice.Viewport.Width / 2 + 50, GraphicsDevice.Viewport.Height - 250, 220, 110);
            creditsButtonRect = new Rectangle(GraphicsDevice.Viewport.Width - 220 , GraphicsDevice.Viewport.Height - 110, 220, 110);

            //Credit Screen
            credits = Content.Load<Texture2D>("Textures\\Menu\\credits");
            backButton = Content.Load<Texture2D>("Textures\\Menu\\back");
            backRect = new Rectangle(5 , GraphicsDevice.Viewport.Height - 110, 220, 110);

            //Gameoverscreen
            gameover = Content.Load<Texture2D>("Textures\\Menu\\gameover");
            retryButton = Content.Load<Texture2D>("Textures\\Menu\\retry");
            retryRect = new Rectangle((GraphicsDevice.Viewport.Width / 2) + 75, (GraphicsDevice.Viewport.Height / 2) + 165, 220, 110);

            //Pause-screen
            continueButton = Content.Load<Texture2D>("Textures\\Menu\\continue");
            continueRect = new Rectangle(320, 300, 220, 110);
            pauseRetryRect = new Rectangle(660 , 300, 220, 110);
            
            //Background & SRH-Tower
            background = Content.Load<Texture2D>("Textures\\Background\\background");
            background2 = Content.Load<Texture2D>("Textures\\Background\\background2");  
            tower = Content.Load<Texture2D>("Textures\\Background\\srhtower");
            pyramid = Content.Load<Texture2D>("Textures\\Background\\pyramid");

            //Level
            level1 = Content.Load<Texture2D>("Textures\\Levels\\level1");
            level2 = Content.Load<Texture2D>("Textures\\Levels\\level2");
            level3 = Content.Load<Texture2D>("Textures\\Levels\\level3");
            level4 = Content.Load<Texture2D>("Textures\\Levels\\level4");
            level5 = Content.Load<Texture2D>("Textures\\Levels\\level5");

            //Spawn & Exit Trigger
            spawn = new Vector2(690, 120);
            
            portal = Content.Load<Texture2D>("Textures\\Trigger\\portal");
            finish = Content.Load<Texture2D>("Textures\\Trigger\\dixie");
            finishRect = new Rectangle(1150, 610, 161, 234);
            finishTrigger = new Rectangle(finishRect.X + (finishRect.Width / 2), finishRect.Y + (finishRect.Height / 2), 10, 10);

            //UI
            UI = Content.Load<Texture2D>("Textures\\GameUI\\UI");
            speedButton = Content.Load<Texture2D>("Textures\\GameUI\\increasespeed");
            blockButton = Content.Load<Texture2D>("Textures\\GameUI\\block");
            parachuteButton = Content.Load<Texture2D>("Textures\\GameUI\\parachute");
            phaseButton = Content.Load<Texture2D>("Textures\\GameUI\\phase");
            pauseButton = Content.Load<Texture2D>("Textures\\GameUI\\pause");
            endButton = Content.Load<Texture2D>("Textures\\GameUI\\end");

            parachuteRect = new Rectangle(470, 650, 150, 60);
            phaseRect = new Rectangle(640, 650, 150, 60);
            blockRect = new Rectangle(800, 650, 150, 60);
            speedRect = new Rectangle(10, 650, 150, 60);
            pauseRect = new Rectangle(10, 10, 34, 40);
            endRect = new Rectangle(1120, 570, 150, 60);

            //Minimap
            minimap = Content.Load<Texture2D>("Textures\\Levels\\level1");
            blackbg = Content.Load<Texture2D>("Textures\\Levels\\blackbg");
            minimapRect = new Rectangle(1070, 10, 200, 100);

            //Define max uses of the abilities
            maxUsesParachute = 20;
            maxUsesPhase = 20;
            maxUsesBlock = 0;

            //Read Per-Pixel colordata for collision from the level textures
            level1TextureData = new Color[level1.Width * level1.Height];
            level1.GetData(level1TextureData);

            level2TextureData = new Color[level2.Width * level2.Height];
            level2.GetData(level2TextureData);

            level3TextureData = new Color[level3.Width * level3.Height];
            level3.GetData(level3TextureData);

            level4TextureData = new Color[level4.Width * level4.Height];
            level4.GetData(level4TextureData);

            level5TextureData = new Color[level5.Width * level5.Height];
            level5.GetData(level5TextureData);

            //Set mainFrame rectangle size to background width and height (2000px x 1000px)
            mainFrame = new Rectangle(0, 0, 2000, 1000);

            //Custom Font MyFont
            mFont = Content.Load<SpriteFont>("MyFont");

            //Set counter to startCounter
            counter = startCounter;

            //Setting Spawnrate to 5 seconds
            spawnRate = 5;
            
            //Set initial level to level1
            currentLevel = level1;

            //Set initial Texture data to level1
            currentTextureData = new Color[level1.Width * level1.Height];
            level1.GetData(currentTextureData);

            //Game is running at beginning
            gamePaused = false;

            //Background music
            song = Content.Load<Song>("GameMusicLoop");
            MediaPlayer.Volume = 0.3f; // Volume = 30%
            MediaPlayer.IsRepeating = true;

            //No escape rectangle at the bottom of the screen
            noEscape = new Rectangle(0, mainFrame.Height - 10, 2000, 100);
        }

        protected override void UnloadContent()
        { }
        
        protected override void Update(GameTime gameTime)
        {
            //Mouse and Keyboard-States
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            #region CameraMovement
            //Offset for screen-edges
            int offsetX = 50; 
            int offsetY = 30;

            if (Mouse.GetState().X > GraphicsDevice.Viewport.Width - offsetX) //Right side
            {
                if (cam.position.X + GraphicsDevice.Viewport.Width < mainFrame.Width - 360 && speedClicked == false && gamePaused == false)
                    cam.position.X += 4; //Move camera on the x-axis to the right by 4px

                else if (cam.position.X + GraphicsDevice.Viewport.Width < mainFrame.Width - 360 && speedClicked == true && gamePaused == false)
                    cam.position.X += 1; //Move camera on the x-axis to the right by 1px
            }

            else if (Mouse.GetState().X > (GraphicsDevice.Viewport.Width - GraphicsDevice.Viewport.Width) && Mouse.GetState().X < offsetX) //Left side
            {
                if (cam.position.X > mainFrame.X && speedClicked == false && gamePaused == false)
                    cam.position.X -= 4; //Move camera on the x-axis to the left by 4px

                else if (cam.position.X > mainFrame.X && speedClicked == true && gamePaused == false)
                    cam.position.X -= 1; //Move camera on the x-axis to the left by 1px
            }

            else if (Mouse.GetState().Y > GraphicsDevice.Viewport.Height - offsetY) //Bottom side
            {
                if (cam.position.Y + GraphicsDevice.Viewport.Height < mainFrame.Height - 140 && speedClicked == false && gamePaused == false)
                    cam.position.Y += 4; //Move camera on the y-axis down by 4px

                else if (cam.position.Y + GraphicsDevice.Viewport.Height < mainFrame.Height - 140 && speedClicked == true && gamePaused == false)
                    cam.position.Y += 1; //Move camera on the y-axis down by 1px
            }

            else if (Mouse.GetState().Y > (GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Height) && Mouse.GetState().Y < offsetY) //Top side
            {
                if (cam.position.Y > mainFrame.Y && speedClicked == false && gamePaused == false)
                    cam.position.Y -= 4; //Move camera on the y-axis up by 4px

                else if (cam.position.Y > mainFrame.Y && speedClicked == true && gamePaused == false)
                    cam.position.Y -= 1; //Move camera on the y-axis up by 1px
            }
            #endregion

            #region Abilities
            if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                foreach (Lemming l in this.Lemmings) //for every Lemming in ArrayList Lemmings
                {
                    if (l.isHovered) //if the mouse is hovering over the lemming instance
                    {
                        switch (state)
                        {
                            case State.Parachute:
                                if (maxUsesParachute > 0) //If max uses >0 ...
                                {
                                    l.parachute(); //... then apply Parachute to lemming
                                    maxUsesParachute -= 1;
                                }
                                else { }
                                break;
                            case State.Block:
                                if (!l.falling && maxUsesBlock > 0) //If max uses >0 ...
                                {
                                    l.block(); //... then apply Block to lemming
                                    maxUsesBlock -= 1;
                                }
                                else { }
                                break;
                            case State.Phase:
                                if (maxUsesPhase > 0) //If max uses >0 ...
                                {
                                    l.phase(); //... then apply Phase to lemming
                                    maxUsesPhase -= 1;
                                }
                                else { } //Else dont apply ever again
                                break;
                        }
                        break;
                    }
                }
                state = State.None; //Standard state is none
            }
            #endregion

            #region MouseCheckClick
            //Check and update GameStates accordingly
            switch (gameState)
            {
                case GameState.SplashScreen: //If splashscreen is displayed...
                    if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                        gameState = GameState.Menu;
                    break;

                case GameState.Menu: //If menu is displayed...
                    if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                        CheckClick(mouseState.X, mouseState.Y); //call CheckClick function
                    break;

                case GameState.Level1: //If Level1 is displayed...
                    if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                        CheckClick(mouseState.X, mouseState.Y); //call CheckClick function
                    break;

                case GameState.Level2: //If Level2 is displayed...
                    if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                        CheckClick(mouseState.X, mouseState.Y); //call CheckClick function
                    break;

                case GameState.Level3: //If Level3 is displayed...
                    if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                        CheckClick(mouseState.X, mouseState.Y); //call CheckClick function
                    break;

                case GameState.Level4: //If Level4 is displayed...
                    if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                        CheckClick(mouseState.X, mouseState.Y); //call CheckClick function
                    break;

                case GameState.Level5: //If Level5 is displayed...
                    if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                        CheckClick(mouseState.X, mouseState.Y); //call CheckClick function
                    break;

                case GameState.GameOver: //If GameOver is displayed...
                    if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                        CheckClick(mouseState.X, mouseState.Y); //call CheckClick function
                    break;

                case GameState.Credits: //If GameOver is displayed...
                    if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                        CheckClick(mouseState.X, mouseState.Y); //call CheckClick function
                    break;
            }
            #endregion

            #region LevelSwitches
            if (gameState == GameState.Level1 && previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released && 
                endRect.Intersects(new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 10, 10)) && lemmingsSaved >= 10
            || (gameState == GameState.Level1 && Lemmings.Count == 0 && lemmingsSaved >= 10) || counter <= 0 && lemmingsSaved >= 10)
            {
                currentLevel = level2; //Set currentLevel to level2
                Lemmings.Clear();  //Delete all Lemmings from the screen
                totalLemmingsSaved += lemmingsSaved;
                totalLemmingsKilled += lemmingsDead;
                lemmingsSaved = 0; //Reset Lemmings counters
                lemmingsDead = 0;
                speedClicked = false; //Disable speed
                currentTextureData = new Color[level2.Width * level2.Height]; //Set textureData to Level2
                level2.GetData(currentTextureData);
                counter = 180 * 1000;
                finishRect = new Rectangle(1850, 600, 201, 293);
                finishTrigger = new Rectangle(finishRect.X + (finishRect.Width / 2), finishRect.Y + (finishRect.Height / 2), 10, 10);
                maxUsesParachute = 20;
                maxUsesPhase = 0;
                maxUsesBlock = 0;
                minimap = Content.Load<Texture2D>("Textures\\Levels\\level2");
                gameState = GameState.Level2;
            }

            else if (gameState == GameState.Level2 && previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released &&
                endRect.Intersects(new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 10, 10)) && lemmingsSaved >= 10
            || (gameState == GameState.Level2 && Lemmings.Count == 0 && lemmingsSaved >= 10) || counter <= 0 && lemmingsSaved >= 10)
            {
                currentLevel = level3; //Set currentLevel to level3
                Lemmings.Clear(); //Delete all Lemmings from the screen
                totalLemmingsSaved += lemmingsSaved;
                totalLemmingsKilled += lemmingsDead;
                lemmingsSaved = 0; //Reset Lemmings counters
                lemmingsDead = 0;
                speedClicked = false; //Disable speed
                currentTextureData = new Color[level3.Width * level3.Height]; //Set textureData to Level3
                level3.GetData(currentTextureData);
                counter = 180 * 1000;
                finishRect = new Rectangle(1650, 630, 201, 293);
                finishTrigger = new Rectangle(finishRect.X + (finishRect.Width / 2), finishRect.Y + (finishRect.Height / 2), 10, 10);
                maxUsesParachute = 0;
                maxUsesPhase = 0;
                maxUsesBlock = 1;
                minimap = Content.Load<Texture2D>("Textures\\Levels\\level3");
                gameState = GameState.Level3;
            }

            else if (gameState == GameState.Level3 && previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released &&
                endRect.Intersects(new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 10, 10)) && lemmingsSaved >= 10
            || (gameState == GameState.Level3 && Lemmings.Count == 0 && lemmingsSaved >= 10) || counter <= 0 && lemmingsSaved >= 10)
            {
                currentLevel = level4; //Set currentLevel to level4
                Lemmings.Clear(); //Delete all Lemmings from the screen
                totalLemmingsSaved += lemmingsSaved;
                totalLemmingsKilled += lemmingsDead;
                lemmingsSaved = 0; //Reset Lemmings counters
                lemmingsDead = 0;
                speedClicked = false; //Disable speed
                currentTextureData = new Color[level4.Width * level4.Height]; //Set textureData to Level4
                level4.GetData(currentTextureData);
                counter = 180 * 1000;
                finishRect = new Rectangle(1650, 630, 201, 293);
                finishTrigger = new Rectangle(finishRect.X + (finishRect.Width / 2), finishRect.Y + (finishRect.Height / 2), 10, 10);
                maxUsesParachute = 20;
                maxUsesPhase = 20;
                maxUsesBlock = 20;
                minimap = Content.Load<Texture2D>("Textures\\Levels\\level4");
                gameState = GameState.Level4;
            }

            else if (gameState == GameState.Level4 && previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released &&
                endRect.Intersects(new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 10, 10)) && lemmingsSaved >= 10
            || (gameState == GameState.Level4 && Lemmings.Count == 0 && lemmingsSaved >= 10) || counter <= 0 && lemmingsSaved >= 10)
            {
                currentLevel = level5; //Set currentLevel to level5
                spawn = new Vector2(300, 50);
                Lemmings.Clear(); //Delete all Lemmings from the screen
                totalLemmingsSaved += lemmingsSaved;
                totalLemmingsKilled += lemmingsDead;
                lemmingsSaved = 0; //Reset Lemmings counters
                lemmingsDead = 0;
                speedClicked = false; //Disable speed
                currentTextureData = new Color[level5.Width * level5.Height]; //Set textureData to Level5
                level5.GetData(currentTextureData);
                counter = 180 * 1000;
                finishRect = new Rectangle(1000, 570, 201, 293);
                finishTrigger = new Rectangle(finishRect.X + (finishRect.Width / 2), finishRect.Y + (finishRect.Height / 2), 10, 10);
                maxUsesParachute = 20;
                maxUsesPhase = 20;
                maxUsesBlock = 20;
                minimap = Content.Load<Texture2D>("Textures\\Levels\\level5");
                gameState = GameState.Level5;
            }

            else if (gameState == GameState.Level5 && previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released &&
                endRect.Intersects(new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 10, 10)) && lemmingsSaved >= 10
            || (gameState == GameState.Level5 && Lemmings.Count == 0 && lemmingsSaved >= 10) || counter <= 0 && lemmingsSaved >= 10)
            {
                gameState = GameState.GameOver; //After level 5 has finished you won the game and it triggers the game over for now ;)
            }
            #endregion

            previousMouseState = mouseState; //previous mouse state is now mouseState

            if (gameState == GameState.Menu || gameState == GameState.SplashScreen) //Don't update
            {
            }

            else //Else (aka. gameState == GameState.Game) do the update
            {
                foreach (Lemming L in Lemmings) //for every Lemming in ArrayList Lemmings
                {
                    L.Update(gameTime); //Update lemmings

                    if (L.selfRectangle.Intersects(finishTrigger)) //Remove Lemmings instance on collision
                    {
                        LemmingsToRemove.Add(L); //Add Lemming to the Delete-ArrayList (manual dispose list)
                        lemmingsSaved++; //Increase lemmingsSaved counter
                    }

                    else if (L.selfRectangle.Intersects(noEscape))
                    {
                        LemmingsToRemove.Add(L);
                        lemmingsDead++;
                    }
                }
                foreach (var RemoveLemming in LemmingsToRemove) //for every RemoveLemming in ArrayList LemmingsToRemove
                {
                    Lemmings.Remove(RemoveLemming); //delete L (Lemming) from ArrayList
                }
                
                //Gamecounter
                counter -= counterX * gameTime.ElapsedGameTime.Milliseconds; //subtract the gametime from counter
                if (counter <= 0 && lemmingsSaved < 10) 
                {
                    gameState = GameState.GameOver; //when counter reaches zero set gameState to GameOver screen
                    counter = startCounter; //reset counter
                }


                //Lemmings die (dissappear) if fall counter is > 4 seconds
                foreach (Lemming L in Lemmings) //for every Lemming in ArrayList Lemmings
                {
                    if (L.falling == false && L.fallCounter < 4000) { 
                        L.fallCounter = 0;
                    }
                    if (L.falling == true && L.isParachuting == false) {
                        L.fallCounter += gameTime.ElapsedGameTime.Milliseconds * counterX;
                    }
                    if (L.fallCounter >= 4000  && L.falling == false && L.isParachuting == false)
                    {
                        LemmingsToRemove.Add(L); //when counter reaches zero set gameState to GameOver screen
                        lemmingsDead++;
                    }
                }

                if (lemmingsDead > 10)
                {
                    gameState = GameState.GameOver;
                }

            //Draw Lemming every 5 seconds
            if (gameendtime <= DateTime.Now && Lemmings.Count < (20 - lemmingsSaved - lemmingsDead) && gamePaused == false) //If Time.Now + 5 seconds is smaller or equal to DateTime.Now ... spawn a lemming
                {
                    Lemmings.Add(new Lemming(Content.Load<Texture2D>("Textures\\Lemmings\\flashsprite"), //Add new lemming to Lemmings ArrayList
                                             Content.Load<Texture2D>("Textures\\GameUI\\clicked"), //hand over lemming overlay
                                             spawn, 143, 54, //hand over draw position & Sprite width and height
                                             new Rectangle(0, 0, currentLevel.Width, currentLevel.Height), //hand over collision data from the terrain
                                             currentTextureData, cam)); //hand over texture data from the terrain & current camera
                    gameendtime = DateTime.Now + TimeSpan.FromSeconds(spawnRate); //Add 5 seconds again
                }

            #region speedButton
            if (speedClicked) //If the 4x speed button has been pressed...
            {             
                foreach (Lemming L in Lemmings)
                {
                    L.divideAnim = 0.75f; //animation 4x speed (from /4 to /1)
                }

                TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 240.0f); //mutliply time by 4 (60 * 4)
                counterX = 4; //Subtract the current time 4x from elapsed game time
                spawnRate = 1; //Set spawn rate to 1 (Not quite correct, I know!) (actually 5/4 = 1.25)
            }

            else if (speedClicked == false && gamePaused == false)
            {
                foreach (Lemming L in Lemmings)
                {
                    L.divideAnim = 3.0f; //animation 4x speed
                    L.velocityMultiplier = 1;
                }
                TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f); //Set time scale back to normal
                counterX = 1; //Set counter subtract speed back to normal
                spawnRate = 5; //Set spawnRate back to normal
            }
                #endregion
            
               if (gamePaused || gameState == GameState.GameOver)
            {
                foreach (Lemming L in Lemmings)
                {
                    L.divideAnim = 1000.0f; //animation 4x speed
                    L.velocityMultiplier = 0;
                }
                counterX = 0; //Set counter subtract speed back to normal       
                speedClicked = false;
            }

                #region BlockFix
                foreach (Lemming L in Lemmings)
            {
                if (L.isBlocking == true)
                {
                    foreach (Lemming L2 in Lemmings)
                    {
                       if (L2.isBlocking == false && L.selfRectangle.Intersects(L2.selfRectangle)){
                            L2.right = !L2.right;
                            if (L2.right) //If the initial direction is right and the lemming is not falling
                            {
                                L2.animateRight(gameTime);
                                L2.velocity = new Vector2(3, 0);
                                L2.position = L2.position + L2.velocity;
                                L2.velocity = new Vector2(2 * L2.velocityMultiplier, 0);
                            }
                            else if (!L2.right) //If the direction is left and the lemming is not falling
                            {
                                L2.animateLeft(gameTime);
                                L2.velocity = new Vector2(-3, 0);
                                L2.position = L2.position + L2.velocity;
                                L2.velocity = new Vector2(-2 * L2.velocityMultiplier, 0);
                            }
                        }
                    }  
                }
            }
                #endregion

                Shortcuts(); //Call the Keyboard Shortcut function 

            base.Update(gameTime);

            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue); //Standard output = background in color: ConrnflowerBlue

            switch (gameState)
            {
                case GameState.SplashScreen: //Draw splashscreens
                    spriteBatch.Begin();
                    spriteBatch.Draw(splashScreen, new Vector2(0, 0), Color.White);
                    spriteBatch.End();
                    break;
                    
                case GameState.Menu: //Draw menu
                    spriteBatch.Begin();
                    spriteBatch.Draw(mBackground, new Vector2(0, 0), Color.White);
                    spriteBatch.Draw(startButton, startButtonRect, Color.White);
                    spriteBatch.Draw(exitButton, exitButtonRect, Color.White);
                    spriteBatch.Draw(creditsButton, creditsButtonRect, Color.White);
                    spriteBatch.End();
                    break;

                #region Level1
                case GameState.Level1: //Draw background with parallax 1.0f
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                                  null, null, null, null, cam.ViewMatrix(parallax));
                    spriteBatch.Draw(background, new Rectangle(0, 0, 1800, 900), Color.White);
                    spriteBatch.End();

                    //Draw SRH-Tower with parallax 1.5f
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                                      null, null, null, null, cam.ViewMatrix(parallax * 1.5f));
                    spriteBatch.Draw(tower, new Vector2(1150, 0), Color.White);
                    spriteBatch.End();

                    //Draw level with parallax 2.0f
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                                      null, null, null, null, cam.ViewMatrix(parallax * 2.0f));
                    spriteBatch.Draw(level1, mainFrame, Color.White);

                    //Draw Lemming with parallax 2.0f via own Draw() method
                    foreach (Lemming L in Lemmings)
                    {
                        L.Draw(spriteBatch);
                    }

                    //Draw triggers with parallax 2.0f
                    spriteBatch.Draw(portal, new Vector2(650, spawn.Y -150), Color.White);
                    
                    spriteBatch.Draw(finish, finishRect, Color.White);
                    spriteBatch.End();

                    //Draw UI without parallax
                    spriteBatch.Begin();
                    spriteBatch.Draw(UI, new Vector2(0, 635), Color.White);
                    spriteBatch.Draw(pauseButton, pauseRect, Color.White);
                    spriteBatch.Draw(speedButton, new Rectangle(10, 650, 150, 60), Color.White);

                    if (lemmingsSaved >= 10) { 
                    spriteBatch.Draw(endButton, endRect, Color.White);
                    }


                    //Minimap
                    spriteBatch.Draw(blackbg, minimapRect, Color.White);
                    spriteBatch.Draw(minimap, minimapRect, Color.White);

                    //Ability buttons
                    spriteBatch.Draw(parachuteButton, new Rectangle(470 , 650, 150, 60), Color.White);
                    spriteBatch.Draw(phaseButton, new Rectangle(640, 650, 150, 60), Color.White);
                    spriteBatch.Draw(blockButton, new Rectangle(800, 650, 150, 60), Color.White);

                    //Timer
                    spriteBatch.DrawString(mFont, "Time left: " + ((int)(counter/1000)).ToString() + " seconds", new Vector2(965, 670), Color.White);

                    //Max uses
                    spriteBatch.DrawString(mFont, maxUsesParachute.ToString(), new Vector2(580, 690), Color.White);
                    spriteBatch.DrawString(mFont, maxUsesPhase.ToString(), new Vector2(750, 690), Color.White);
                    spriteBatch.DrawString(mFont, maxUsesBlock.ToString(), new Vector2(910, 690), Color.White);

                    //Lemming counter
                    spriteBatch.DrawString(mFont, Lemmings.Count.ToString() + " Lemmings to save", new Vector2(180, 640), Color.White);
                    spriteBatch.DrawString(mFont, lemmingsSaved.ToString() + " Lemmings saved", new Vector2(180, 665), Color.White);
                    spriteBatch.DrawString(mFont, lemmingsDead.ToString() + " Lemmings dead", new Vector2(180, 690), Color.White);

                    spriteBatch.End();
                    
                    if (gamePaused)
                    {
                        MediaPlayer.Pause();
                        spriteBatch.Begin();
                        spriteBatch.DrawString(mFont, "Game is paused!", new Vector2(520, 200), Color.White);
                        spriteBatch.Draw(retryButton, pauseRetryRect, Color.White);
                        spriteBatch.Draw(continueButton, continueRect, Color.White);
                        spriteBatch.End();
                    }
                    else { MediaPlayer.Resume(); }
                    break;
                #endregion

                #region Level2
                case GameState.Level2: //Draw background with parallax 1.0f
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                                  null, null, null, null, cam.ViewMatrix(parallax));
                    spriteBatch.Draw(background2, new Rectangle(0, 0, 1800, 900), Color.White);
                    spriteBatch.End();

                    //Draw pyramid with parallax 1.5f
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                                      null, null, null, null, cam.ViewMatrix(parallax * 1.5f));
                    spriteBatch.Draw(pyramid, new Vector2(1150, 400), Color.White);
                    spriteBatch.End();

                    //Draw level with parallax 2.0f
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                                      null, null, null, null, cam.ViewMatrix(parallax * 2.0f));
                    spriteBatch.Draw(level2, mainFrame, Color.White);


                    //Draw Lemming with parallax 2.0f via own Draw() method
                    foreach (Lemming L in Lemmings)
                    {
                        L.Draw(spriteBatch);
                    }

                    //Draw triggers with parallax 2.0f
                    spriteBatch.Draw(portal, new Vector2(650, spawn.Y - 150), Color.White);
                    spriteBatch.Draw(finish, finishRect, Color.White);
                    spriteBatch.End();

                    //Draw UI without parallax
                    spriteBatch.Begin();
                    spriteBatch.Draw(UI, new Vector2(0, 635), Color.White);
                    spriteBatch.Draw(pauseButton, pauseRect, Color.White);
                    spriteBatch.Draw(speedButton, new Rectangle(10, 650, 150, 60), Color.White);

                    if (lemmingsSaved >= 10)
                    {
                        spriteBatch.Draw(endButton, endRect, Color.White);
                    }

                    //Minimap
                    spriteBatch.Draw(blackbg, minimapRect, Color.White);
                    spriteBatch.Draw(minimap, minimapRect, Color.White);

                    //Ability buttons
                    spriteBatch.Draw(parachuteButton, new Rectangle(470, 650, 150, 60), Color.White);
                    spriteBatch.Draw(phaseButton, new Rectangle(640, 650, 150, 60), Color.White);
                    spriteBatch.Draw(blockButton, new Rectangle(800, 650, 150, 60), Color.White);

                    //Timer
                    spriteBatch.DrawString(mFont, "Time left: " + ((int)(counter / 1000)).ToString() + " seconds", new Vector2(965, 670), Color.White);

                    //Max uses
                    spriteBatch.DrawString(mFont, maxUsesParachute.ToString(), new Vector2(580, 690), Color.White);
                    spriteBatch.DrawString(mFont, maxUsesPhase.ToString(), new Vector2(750, 690), Color.White);
                    spriteBatch.DrawString(mFont, maxUsesBlock.ToString(), new Vector2(910, 690), Color.White);

                    //Lemming counter
                    spriteBatch.DrawString(mFont, Lemmings.Count.ToString() + " Lemmings to save", new Vector2(180, 640), Color.White);
                    spriteBatch.DrawString(mFont, lemmingsSaved.ToString() + " Lemmings saved", new Vector2(180, 665), Color.White);
                    spriteBatch.DrawString(mFont, lemmingsDead.ToString() + " Lemmings dead", new Vector2(180, 690), Color.White);

                    spriteBatch.End();


                    if (gamePaused)
                    {
                        MediaPlayer.Pause();
                        spriteBatch.Begin();
                        spriteBatch.DrawString(mFont, "Game is paused!", new Vector2(520, 200), Color.White);
                        spriteBatch.Draw(retryButton, pauseRetryRect, Color.White);
                        spriteBatch.Draw(continueButton, continueRect, Color.White);
                        spriteBatch.End();
                    }
                    else { MediaPlayer.Resume(); }
                    break;
                #endregion

                #region Level3
                case GameState.Level3: //Draw background with parallax 1.0f
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                                  null, null, null, null, cam.ViewMatrix(parallax));
                    spriteBatch.Draw(background2, new Rectangle(0, 0, 1800, 900), Color.White);
                    spriteBatch.End();

                    //Draw pyramid with parallax 1.5f
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                                      null, null, null, null, cam.ViewMatrix(parallax * 1.5f));
                    spriteBatch.Draw(pyramid, new Vector2(1150, 400), Color.White);
                    spriteBatch.End();

                    //Draw level with parallax 2.0f
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                                      null, null, null, null, cam.ViewMatrix(parallax * 2.0f));
                    spriteBatch.Draw(level3, mainFrame, Color.White);


                    //Draw Lemming with parallax 2.0f via own Draw() method
                    foreach (Lemming L in Lemmings)
                    {
                        L.Draw(spriteBatch);
                    }

                    //Draw triggers with parallax 2.0f
                    spriteBatch.Draw(portal, new Vector2(650, spawn.Y - 150), Color.White);
                    spriteBatch.Draw(finish, finishRect, Color.White);
                    spriteBatch.End();

                    //Draw UI without parallax
                    spriteBatch.Begin();
                    spriteBatch.Draw(UI, new Vector2(0, 635), Color.White);
                    spriteBatch.Draw(pauseButton, pauseRect, Color.White);
                    spriteBatch.Draw(speedButton, new Rectangle(10, 650, 150, 60), Color.White);

                    if (lemmingsSaved >= 10)
                    {
                        spriteBatch.Draw(endButton, endRect, Color.White);
                    }

                    //Minimap
                    spriteBatch.Draw(blackbg, minimapRect, Color.White);
                    spriteBatch.Draw(minimap, minimapRect, Color.White);

                    //Ability buttons
                    spriteBatch.Draw(parachuteButton, new Rectangle(470, 650, 150, 60), Color.White);
                    spriteBatch.Draw(phaseButton, new Rectangle(640, 650, 150, 60), Color.White);
                    spriteBatch.Draw(blockButton, new Rectangle(800, 650, 150, 60), Color.White);

                    //Timer
                    spriteBatch.DrawString(mFont, "Time left: " + ((int)(counter / 1000)).ToString() + " seconds", new Vector2(965, 670), Color.White);

                    //Max uses
                    spriteBatch.DrawString(mFont, maxUsesParachute.ToString(), new Vector2(580, 690), Color.White);
                    spriteBatch.DrawString(mFont, maxUsesPhase.ToString(), new Vector2(750, 690), Color.White);
                    spriteBatch.DrawString(mFont, maxUsesBlock.ToString(), new Vector2(910, 690), Color.White);

                    //Lemming counter
                    spriteBatch.DrawString(mFont, Lemmings.Count.ToString() + " Lemmings to save", new Vector2(180, 640), Color.White);
                    spriteBatch.DrawString(mFont, lemmingsSaved.ToString() + " Lemmings saved", new Vector2(180, 665), Color.White);
                    spriteBatch.DrawString(mFont, lemmingsDead.ToString() + " Lemmings dead", new Vector2(180, 690), Color.White);

                    spriteBatch.End();


                    if (gamePaused)
                    {
                        MediaPlayer.Pause();
                        spriteBatch.Begin();
                        spriteBatch.DrawString(mFont, "Game is paused!", new Vector2(520, 200), Color.White);
                        spriteBatch.Draw(retryButton, pauseRetryRect, Color.White);
                        spriteBatch.Draw(continueButton, continueRect, Color.White);
                        spriteBatch.End();
                    }
                    else { MediaPlayer.Resume(); }
                    break;
                #endregion

                #region Level4
                case GameState.Level4: //Draw background with parallax 1.0f
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                                  null, null, null, null, cam.ViewMatrix(parallax));
                    spriteBatch.Draw(background, new Rectangle(0, 0, 1800, 900), Color.White);
                    spriteBatch.End();

                    //Draw SRH-Tower with parallax 1.5f
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                                      null, null, null, null, cam.ViewMatrix(parallax * 1.5f));
                    spriteBatch.Draw(tower, new Vector2(1150, 0), Color.White);
                    spriteBatch.End();

                    //Draw level with parallax 2.0f
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                                      null, null, null, null, cam.ViewMatrix(parallax * 2.0f));
                    spriteBatch.Draw(level4, mainFrame, Color.White);


                    //Draw Lemming with parallax 2.0f via own Draw() method
                    foreach (Lemming L in Lemmings)
                    {
                        L.Draw(spriteBatch);
                    }

                    //Draw triggers with parallax 2.0f
                    spriteBatch.Draw(portal, new Vector2(650, spawn.Y - 150), Color.White);
                    spriteBatch.Draw(finish, finishRect, Color.White);
                    spriteBatch.End();

                    //Draw UI without parallax
                    spriteBatch.Begin();
                    spriteBatch.Draw(UI, new Vector2(0, 635), Color.White);
                    spriteBatch.Draw(pauseButton, pauseRect, Color.White);
                    spriteBatch.Draw(speedButton, new Rectangle(10, 650, 150, 60), Color.White);

                    if (lemmingsSaved >= 10)
                    {
                        spriteBatch.Draw(endButton, endRect, Color.White);
                    }

                    //Minimap
                    spriteBatch.Draw(blackbg, minimapRect, Color.White);
                    spriteBatch.Draw(minimap, minimapRect, Color.White);

                    //Ability buttons
                    spriteBatch.Draw(parachuteButton, new Rectangle(470, 650, 150, 60), Color.White);
                    spriteBatch.Draw(phaseButton, new Rectangle(640, 650, 150, 60), Color.White);
                    spriteBatch.Draw(blockButton, new Rectangle(800, 650, 150, 60), Color.White);

                    //Timer
                    spriteBatch.DrawString(mFont, "Time left: " + ((int)(counter / 1000)).ToString() + " seconds", new Vector2(965, 670), Color.White);

                    //Max uses
                    spriteBatch.DrawString(mFont, maxUsesParachute.ToString(), new Vector2(580, 690), Color.White);
                    spriteBatch.DrawString(mFont, maxUsesPhase.ToString(), new Vector2(750, 690), Color.White);
                    spriteBatch.DrawString(mFont, maxUsesBlock.ToString(), new Vector2(910, 690), Color.White);

                    //Lemming counter
                    spriteBatch.DrawString(mFont, Lemmings.Count.ToString() + " Lemmings to save", new Vector2(180, 640), Color.White);
                    spriteBatch.DrawString(mFont, lemmingsSaved.ToString() + " Lemmings saved", new Vector2(180, 665), Color.White);
                    spriteBatch.DrawString(mFont, lemmingsDead.ToString() + " Lemmings dead", new Vector2(180, 690), Color.White);

                    spriteBatch.End();


                    if (gamePaused)
                    {
                        MediaPlayer.Pause();
                        spriteBatch.Begin();
                        spriteBatch.DrawString(mFont, "Game is paused!", new Vector2(520, 200), Color.White);
                        spriteBatch.Draw(retryButton, pauseRetryRect, Color.White);
                        spriteBatch.Draw(continueButton, continueRect, Color.White);
                        spriteBatch.End();
                    }
                    else { MediaPlayer.Resume(); }
                    break;
                #endregion

                #region Level5
                case GameState.Level5: //Draw background with parallax 1.0f
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                                  null, null, null, null, cam.ViewMatrix(parallax));
                    spriteBatch.Draw(background2, new Rectangle(0, 0, 1800, 900), Color.White);
                    spriteBatch.End();

                    //Draw SRH-Tower with parallax 1.5f
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                                      null, null, null, null, cam.ViewMatrix(parallax * 1.5f));
                    spriteBatch.Draw(tower, new Vector2(1150, 0), Color.White);
                    spriteBatch.End();

                    //Draw level with parallax 2.0f
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                                      null, null, null, null, cam.ViewMatrix(parallax * 2.0f));
                    spriteBatch.Draw(level5, mainFrame, Color.White);


                    //Draw Lemming with parallax 2.0f via own Draw() method
                    foreach (Lemming L in Lemmings)
                    {
                        L.Draw(spriteBatch);
                    }

                    //Draw triggers with parallax 2.0f
                    spriteBatch.Draw(portal, new Vector2(650, spawn.Y - 150), Color.White);
                    spriteBatch.Draw(finish, finishRect, Color.White);
                    spriteBatch.End();

                    //Draw UI without parallax
                    spriteBatch.Begin();
                    spriteBatch.Draw(UI, new Vector2(0, 635), Color.White);
                    spriteBatch.Draw(pauseButton, pauseRect, Color.White);
                    spriteBatch.Draw(speedButton, new Rectangle(10, 650, 150, 60), Color.White);

                    if (lemmingsSaved >= 10)
                    {
                        spriteBatch.Draw(endButton, endRect, Color.White);
                    }

                    //Minimap
                    spriteBatch.Draw(blackbg, minimapRect, Color.White);
                    spriteBatch.Draw(minimap, minimapRect, Color.White);

                    //Ability buttons
                    spriteBatch.Draw(parachuteButton, new Rectangle(470, 650, 150, 60), Color.White);
                    spriteBatch.Draw(phaseButton, new Rectangle(640, 650, 150, 60), Color.White);
                    spriteBatch.Draw(blockButton, new Rectangle(800, 650, 150, 60), Color.White);

                    //Timer
                    spriteBatch.DrawString(mFont, "Time left: " + ((int)(counter / 1000)).ToString() + " seconds", new Vector2(965, 670), Color.White);

                    //Max uses
                    spriteBatch.DrawString(mFont, maxUsesParachute.ToString(), new Vector2(580, 690), Color.White);
                    spriteBatch.DrawString(mFont, maxUsesPhase.ToString(), new Vector2(750, 690), Color.White);
                    spriteBatch.DrawString(mFont, maxUsesBlock.ToString(), new Vector2(910, 690), Color.White);

                    //Lemming counter
                    spriteBatch.DrawString(mFont, Lemmings.Count.ToString() + " Lemmings to save", new Vector2(180, 640), Color.White);
                    spriteBatch.DrawString(mFont, lemmingsSaved.ToString() + " Lemmings saved", new Vector2(180, 665), Color.White);
                    spriteBatch.DrawString(mFont, lemmingsDead.ToString() + " Lemmings dead", new Vector2(180, 690), Color.White);

                    spriteBatch.End();


                    if (gamePaused)
                    {
                        MediaPlayer.Pause();
                        spriteBatch.Begin();
                        spriteBatch.DrawString(mFont, "Game is paused!", new Vector2(520, 200), Color.White);
                        spriteBatch.Draw(retryButton, pauseRetryRect, Color.White);
                        spriteBatch.Draw(continueButton, continueRect, Color.White);
                        spriteBatch.End();
                    }
                    else { MediaPlayer.Resume(); }
                    break;
                #endregion

                //Draw Game Over screen
                case GameState.GameOver:
                    spriteBatch.Begin();
                    spriteBatch.Draw(gameover, new Vector2(0, 0), Color.White);
                    spriteBatch.Draw(retryButton, retryRect, Color.White);
                    spriteBatch.DrawString(mFont, "You saved: " + totalLemmingsSaved.ToString() + " lemmings.", new Vector2(965, 670), Color.White);
                    spriteBatch.DrawString(mFont, "You killed: " +totalLemmingsKilled.ToString() + " lemmings ", new Vector2(965, 690), Color.White);
                    spriteBatch.End();
                    break;

                //Draw Credits screen
                case GameState.Credits:
                    spriteBatch.Begin();
                    spriteBatch.Draw(credits, new Vector2(0, 0), Color.White);
                    spriteBatch.Draw(backButton, backRect, Color.White);
                    spriteBatch.End();
                    break;
                    
            }
            base.Draw(gameTime);
        }

        void CheckClick(int x, int y) //Method to check which of the rectangles has been clicked
        {
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);
            

            if (gameState == GameState.Menu || gameState == GameState.SplashScreen)
            {
                if (mouseClickRect.Intersects(startButtonRect))
                { //And when startButton is pressed...
                    gameState = GameState.Level1; //Change GameState to "Game"
                    MediaPlayer.Play(song);
                }

                else if (mouseClickRect.Intersects(exitButtonRect)) //If exitButton is pressed...
                {
                    Exit(); //Exit the game
                }

                else if (mouseClickRect.Intersects(creditsButtonRect))
                    gameState = GameState.Credits;
            }


            if (gameState == GameState.GameOver)
            {
                MediaPlayer.Stop();

                if (mouseClickRect.Intersects(retryRect)) {
                    MediaPlayer.Stop();
                    MediaPlayer.Play(song);
                    currentLevel = level1; //Set curresntLevel to level2
                    spawn = new Vector2(690, 120);
                    Lemmings.Clear();  //Delete all Lemmings from the screen
                    lemmingsSaved = 0; //Reset Lemmings counters
                    lemmingsDead = 0;
                    totalLemmingsSaved = 0;
                    totalLemmingsKilled = 0;
                    speedClicked = false; //Disable speed
                    currentTextureData = new Color[level1.Width * level1.Height]; //Set textureData to Level2
                    level1.GetData(currentTextureData);
                    counter = 180 * 1000;
                    maxUsesBlock = 20;
                    maxUsesParachute = 20;
                    maxUsesPhase = 20;
                    minimap = Content.Load<Texture2D>("Textures\\Levels\\level1");
                    gameState = GameState.Level1;
                }
            }

            if (gameState == GameState.Credits)
            {
                if (mouseClickRect.Intersects(backRect))
                    gameState = GameState.Menu;
            }

            else {

                if (mouseClickRect.Intersects(phaseRect)) //Phase ability
                    state = State.Phase;

                else if (mouseClickRect.Intersects(parachuteRect)) //Parachute ability
                    state = State.Parachute;

                else if (mouseClickRect.Intersects(blockRect)) //Block ability
                    state = State.Block;

                else if (mouseClickRect.Intersects(speedRect)) //4x Speed
                {
                    speedClicked = !speedClicked;
                }

                //-------------------------------------------------------------------------------------------------------

                if (mouseClickRect.Intersects(pauseRetryRect) && gamePaused == true) //Reset game
                {
                    MediaPlayer.Stop();
                    MediaPlayer.Play(song);
                    foreach (Lemming L in Lemmings) //for every Lemming in ArrayList Lemmings
                    {
                        LemmingsToRemove.Add(L); //Add Lemming to the Delete-ArrayList (manual dispose list) 
                    }
                    foreach (var RemoveLemming in LemmingsToRemove) //for every RemoveLemming in ArrayList LemmingsToRemove
                    {
                        Lemmings.Remove(RemoveLemming); //delete L (Lemming) from ArrayList
                    }
                    totalLemmingsSaved = 0;
                    totalLemmingsKilled = 0;
                    lemmingsSaved = 0;
                    speedClicked = false;
                    currentLevel = level1;
                    counter = 180 * 1000;
                    currentTextureData = new Color[level1.Width * level1.Height];
                    level1.GetData(currentTextureData);
                    gamePaused = false;
                    gameState = GameState.Level1;
                    minimap = Content.Load<Texture2D>("Textures\\Levels\\level1");
                    maxUsesBlock = 20;
                    maxUsesParachute = 20;
                    maxUsesPhase = 20;
                    spawn = new Vector2(690, 120);
                }

                if (mouseClickRect.Intersects(continueRect) && gamePaused == true) //Continue game
                    gamePaused = !gamePaused;
                
                if (mouseClickRect.Intersects(pauseRect) && gamePaused == false) //Pause game
                    gamePaused = !gamePaused;

                else if (mouseClickRect.Intersects(pauseRect) && gamePaused == true) //Unpause game
                    gamePaused = !gamePaused;

            }
        }
        
        void Shortcuts()
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (gameState == GameState.Menu || gameState == GameState.SplashScreen)
            {
                //Do nothing for now...
            }

            else
            {
                if (keyboard.IsKeyDown(Keys.D1)) // Is the SPACE key pressed?
                {
                    if (!oldState.IsKeyDown(Keys.D1)) // Key was just pressed
                        speedClicked = !speedClicked;
                }
                else if (oldState.IsKeyDown(Keys.D1)) { }// Key has just been released.
                
                if (keyboard.IsKeyDown(Keys.Escape)) //Game Paused
                {
                    if (!oldState.IsKeyDown(Keys.Escape)) 
                        gamePaused = !gamePaused;
                }
                else if (oldState.IsKeyDown(Keys.Escape)) { }//Escape released
                
                if (keyboard.IsKeyDown(Keys.D2)) //Parachute key
                    state = State.Parachute;

                if (keyboard.IsKeyDown(Keys.D3)) //Phase key
                    state = State.Phase;

                if (keyboard.IsKeyDown(Keys.D4)) //Block key
                    state = State.Block;

                oldState = keyboard; // Update states
            }
        }
    }
}
