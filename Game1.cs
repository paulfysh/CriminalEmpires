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

using System.Collections;

namespace CriminalEmpires
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;

        Texture2D alertTexture;
        Texture2D gameOverTexture;
        Texture2D menuTexture;
        Texture2D officeTexture;
        Texture2D playerTexture;
        Texture2D NPCTexture;
        SpriteFont font;

        TimeSpan menuUpdateSpeed;
        TimeSpan lastMenuUpdate;

        TimeSpan dayDuration;
        TimeSpan lastDayUpdate;
        int currentDay;

        bool newPress;

        Map level;
        Player playerObject;
        GameMenu menu;
        NPC talkingTo;

        RandomGameEvents gameEvents;

        ArrayList alerts = new ArrayList();


        int screenWidth;
        int screenHeight;

        bool gameOver;
        string gameOverText;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 1000;

            menuUpdateSpeed = TimeSpan.FromSeconds(0.20f);
            dayDuration = TimeSpan.FromSeconds(4.0f);
            currentDay = 1;
            gameOverText = "";

            newPress = true;
            talkingTo = null;

            gameOver = false;
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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;

            officeTexture = Content.Load<Texture2D>("OfficeTemplate");
            playerTexture = Content.Load<Texture2D>("You");
            NPCTexture = Content.Load<Texture2D>("NPC");
            font = Content.Load<SpriteFont>("Georgia");
            menuTexture = CreateRectangle(400, 400);
           
            screenWidth = device.PresentationParameters.BackBufferWidth;
            screenHeight = device.PresentationParameters.BackBufferHeight;

            gameOverTexture = CreateRectangle(screenWidth, screenHeight);
            alertTexture = CreateRectangle(screenWidth / 2, screenHeight / 2);

            playerObject = new Player(playerTexture, officeTexture);
            level = new Map(officeTexture, screenWidth, screenHeight, playerObject.getLocation());
            menu = new GameMenu();

            gameEvents = new RandomGameEvents(playerObject);
        }

        private Texture2D CreateRectangle(int width, int height)
        {
            Texture2D rectangleTexture = new Texture2D(GraphicsDevice, width, height);
            Color[] color = new Color[width * height];
            for (int i = 0; i < color.Length; i++)
            {
                color[i] = new Color(0, 0, 0, 255);
            }

            rectangleTexture.SetData(color);
            return rectangleTexture;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            Vector2 nextLocation = playerObject.getLocation();

            KeyboardState keybState = Keyboard.GetState();
            if (keybState.GetPressedKeys().Count() == 0)
            {
                newPress = true;
            }

            if (!menu.isDisplayed)
            {
                if (alerts.Count == 0)
                {
                    if (keybState.IsKeyDown(Keys.Left))
                    {
                        nextLocation = playerObject.GetMove(Player.Direction.left);
                        handleMove(nextLocation);
                    }
                    if (keybState.IsKeyDown(Keys.Right))
                    {
                        nextLocation = playerObject.GetMove(Player.Direction.right);
                        handleMove(nextLocation);
                    }
                    if (keybState.IsKeyDown(Keys.Up))
                    {
                        nextLocation = playerObject.GetMove(Player.Direction.up);
                        handleMove(nextLocation);
                    }
                    if (keybState.IsKeyDown(Keys.Down))
                    {
                        nextLocation = playerObject.GetMove(Player.Direction.down);
                        handleMove(nextLocation);
                    }
                }
                else
                {
                    if (keybState.IsKeyDown(Keys.Enter))
                    {
                        alerts.Clear();
                    }
                }

            }
            else
            {
                if (((gameTime.TotalGameTime - lastMenuUpdate) > menuUpdateSpeed) || newPress)
                {
                    lastMenuUpdate = gameTime.TotalGameTime;
                    if (keybState.IsKeyDown(Keys.Up))
                    {
                        menu.decreaseSelection();
                        newPress = false;
                    }
                    if (keybState.IsKeyDown(Keys.Down))
                    {
                        menu.increaseSelection();
                        newPress = false;
                    }
                    if (keybState.IsKeyDown(Keys.Enter))
                    {
                        if (menu.GetSelection == "Ok")
                        {
                            menu.isDisplayed = false;
                        }
                        else
                        {
                            //I genuinely dont know how this is happening, but I keep getting an exception about talkingTo being null.
                            if (talkingTo != null)
                            {
                                if (talkingTo.processSelectedItem(menu.GetSelection, playerObject))
                                {
                                    menu.isDisplayed = false;

                                    //do this now because it might change as a result of the menu choice. 
                                    playerObject.handlePoliceHeat();
                                }
                            }
                            else
                            {
                                menu.isDisplayed = false;
                            }
                        }

                        if(talkingTo != null)
                        {
                            //resets the menu if we change the contents.
                            menu.menuOptions = talkingTo.getMenuOptions();
                            menu.menuText = talkingTo.getMenuText();
                        }

                        newPress = false;
                    }
                }
            }

            level.updateDisplayRegion(playerObject.getLocation());

            //dont want the game to run while we are in menus or dealing with alerts, thats lame.
            if (alerts.Count == 0 && !menu.isDisplayed)
            {
                if ((gameTime.TotalGameTime - lastDayUpdate) > dayDuration)
                {
                    lastDayUpdate = gameTime.TotalGameTime;
                    currentDay++;
                    newDay();
                }
            }
            else
            {
                lastDayUpdate = gameTime.TotalGameTime;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void newDay()
        {
            alerts.Clear();
            if (!playerObject.handleDailyIncome())
            {
                //we have negative money, game over man, game over.
                gameOver = true;
                gameOverText = "Your money dropped below 0, You cant run an empire without scratch";
            }

            //check for police issues.
            int policeBribe = gameEvents.checkForPoliceInvolvement();
            if (policeBribe > 0)
            {
                if (playerObject.spendMoney(policeBribe))
                {
                    //alert the player somehow
                    alerts.Add("You had to pay the police $" + policeBribe + " to keep them off your back");
                }
                else
                {
                    //die and go to prison
                    gameOver = true;
                    gameOverText = "You pissed off the police and didnt have the funds to placate them, better watch out for that...";
                }
            }

            // check for enforcer stuff
            string enforcerResult = gameEvents.checkForEnforcerIssues();
            if (enforcerResult != "")
            {
                alerts.Add(enforcerResult);
            }

            //handle any jobs you had tasked
            string jobString = playerObject.doJob();
            if(jobString != "")
            {
                alerts.Add(jobString);
            }

            //handle random events somehow. 
            //Give the player a break and wait until day 10 before random stuff starts happening. 
            if (currentDay > 10)
            {
                string randomEventResult = gameEvents.generateRandomEvent();
                if (randomEventResult != "")
                {
                    alerts.Add(randomEventResult);
                }
            }

            if (playerObject.getCityControl() >= 100)
            {
                gameOver = true;
                gameOverText = "You win, you now control this city, maybe now you can look to expand yet further...";
            }
            else if (playerObject.getCityControl() <= 0)
            {
                gameOver = true;
                gameOverText = "Your empire has crumbled, no part of the city accepts you as its master and soon your rivals will come for you...";
            }

            //repeated code, look into it.
            if (playerObject.getMoney() <= 0)
            {
                gameOver = true;
                gameOverText = "Your money dropped below 0, You cant run an empire without scratch";
            }
            //}
        }

        //moved to a method because with the new do commit model, it cant handle two key presses at the same time which made movement annoying. 
        private void handleMove(Vector2 nextLocation)
        {
            talkingTo = npcCollision(nextLocation);

            if (talkingTo == null)
            {
                playerObject.DoMove(level);
            }
            else
            {
                //we have hit an NPC, bring up dialog.
                menu.isDisplayed = true;
                menu.menuOptions = talkingTo.getMenuOptions();
                //.string menuText = talkingTo.getMenuText();
                menu.menuText = talkingTo.getMenuText();
                playerObject.CancelMove();
            }
        }

        #region Drawing

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (alerts.Count != 0)
            {
                drawAlerts();
                drawBasicText();
            }
            else if (!gameOver)
            {
                drawBackground();
                drawPlayer();
                drawNPCs();
                drawBasicText();
                drawMenu();
            }
            else
            {
                drawGameOver();
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void drawAlerts()
        {
            int middleX = (screenWidth / 2) - ((int)alertTexture.Width / 2); 
            int middleY = (screenHeight / 2) - ((int)alertTexture.Height / 2);

            spriteBatch.Draw(alertTexture, new Vector2(middleX, middleY), Color.White);

            middleX += 5;
            middleY += 5; 

            foreach (string alertMessage in alerts)
            {
                foreach (string currString in menuTextDivision(alertMessage, alertTexture))
                {
                    spriteBatch.DrawString(font, currString, new Vector2(middleX, middleY), Color.Red);
                    middleY += (int)font.MeasureString(currString).Y;
                }
   

                //spriteBatch.DrawString(font, alertMessage, new Vector2(middleX, middleY), Color.Red);
                //middleY += (int)font.MeasureString(alertMessage).Y;
            }

            spriteBatch.DrawString(font, "Ok", new Vector2(middleX, middleY), Color.Green);

        }

        private void drawGameOver()
        {
            spriteBatch.Draw(gameOverTexture, new Vector2(0, 0), Color.White);

            Vector2 stringSize = font.MeasureString(gameOverText);
            int middleX = (screenWidth / 2) - ((int)stringSize.X / 2);
            int middleY = (screenHeight / 2) - ((int)stringSize.Y / 2);

            spriteBatch.DrawString(font, gameOverText, new Vector2(middleX, middleY), Color.Red);
        }

        private void drawBackground()
        {
            Rectangle backgroundRectangle = new Rectangle(0, 0, officeTexture.Width, officeTexture.Height);
            spriteBatch.Draw(officeTexture, backgroundRectangle, level.getDisplayRectangle(), Color.White);
        }

        private void drawPlayer()
        {
            //spriteBatch.Draw(playerTexture, new Vector2((screenWidth / 2), (screenHeight / 2)), Color.White);

            /*if (level.playerIsCentered())
            {
                spriteBatch.Draw(playerTexture, new Vector2((screenWidth / 2), (screenHeight / 2)), Color.White);
            }
            else
            {*/
            //we need to figure out how far the screen is offset from the expected
            //try (playerx - screen top x) (player y - screen top y)
            int xVal = (int)(playerObject.getLocation().X - level.getDisplayRectangle().X);
            int yVal = (int)(playerObject.getLocation().Y - level.getDisplayRectangle().Y);
            spriteBatch.Draw(playerTexture, new Vector2(xVal, yVal), Color.White);
            //}

        }

        private void drawNPCs()
        {
            foreach (NPC currNPC in playerObject.getNPCs())
            {
                currNPC.isVisible = false;
                if (currNPC.isAvaliable)
                {
                    Vector2 npcLocation = currNPC.getLocation();
                    Rectangle CurrMapDisplayed = level.getDisplayRectangle();
                    if (objectWithinScreenBounds(npcLocation, CurrMapDisplayed))
                    {
                        int xVal = (int)(npcLocation.X - CurrMapDisplayed.X);
                        int yVal = (int)(npcLocation.Y - CurrMapDisplayed.Y);

                        currNPC.isVisible = true;
                        spriteBatch.Draw(NPCTexture, new Vector2(xVal, yVal), Color.White);
                    }
                }
            }
        }

        private void drawMenu()
        {
            if (menu.isDisplayed)
            {
                spriteBatch.Draw(menuTexture, new Vector2(10, 30), Color.White);
                int currentY = 35;

                foreach (string currString in menuTextDivision(menu.menuText, menuTexture))
                {
                    spriteBatch.DrawString(font, currString, new Vector2(15, currentY), Color.Red);
                    currentY += (int)font.MeasureString(currString).Y;
                }

                currentY += 10;

                int menuItemNumber = 0;
                foreach (string menuOption in menu.menuOptions)
                {
                    if (menuItemNumber == menu.selectedPosition)
                    {
                        foreach (string currString in menuTextDivision(menuOption, menuTexture))
                        {
                            spriteBatch.DrawString(font, currString, new Vector2(15, currentY), Color.Green);
                            currentY += (int)font.MeasureString(currString).Y;
                        }
                    }
                    else
                    {
                        foreach (string currString in menuTextDivision(menuOption, menuTexture))
                        {
                            spriteBatch.DrawString(font, currString, new Vector2(15, currentY), Color.Red);
                            currentY += (int)font.MeasureString(currString).Y;
                        }   
                    }
                    //currentY += (int)font.MeasureString(menuOption).Y;
                    menuItemNumber++;
                }
            }
        }

        private void drawBasicText()
        {
            string drawText = "Cash: $" + playerObject.getMoney() + "    Last Money Change: $" + playerObject.getLastMoneyChange() + "     Day: " + currentDay + "     Current Police Heat: " + playerObject.getPoliceHeat() + "     Current City Controlled: " + playerObject.getCityControl() + "%";
            spriteBatch.DrawString(font, drawText, new Vector2(10, 10), Color.Red);
        }

        #endregion

        #region helpers

        private ArrayList menuTextDivision(string text, Texture2D tex)
        {
            ArrayList textSections = new ArrayList();
            if (font.MeasureString(text).X > tex.Width)
            {
                int lastSpace = 0;
                int currentSpace = 0;
                int lastStringPoint = 0;

                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == ' ')
                    {
                        lastSpace = currentSpace;
                        currentSpace = i;

                        if (font.MeasureString(text.Substring(lastStringPoint, (currentSpace - lastStringPoint))).X > tex.Width)
                        {
                            textSections.Add(text.Substring(lastStringPoint, (lastSpace - lastStringPoint)).Trim());
                            lastStringPoint = lastSpace;
                        }
                    }
                }
                textSections.Add(text.Substring(lastStringPoint).Trim());
            }
            else
            {
                textSections.Add(text);
            }

            return textSections;
        }

        private bool objectWithinScreenBounds(Vector2 objectLocation, Rectangle screenRectangle)
        {
            if ((objectLocation.X > screenRectangle.X) && (objectLocation.X < (screenRectangle.X + screenWidth))
                && (objectLocation.Y > screenRectangle.Y) && (objectLocation.Y < (screenRectangle.Y + screenHeight)))
            {
                return true;
            }
            else if ((objectLocation.X + NPCTexture.Width > screenRectangle.X) && (objectLocation.X < (screenRectangle.X + screenWidth))
                && (objectLocation.Y + NPCTexture.Height > screenRectangle.Y) && (objectLocation.Y < (screenRectangle.Y + screenHeight)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private NPC npcCollision(Vector2 nextLocation)
        {
            Matrix playerMat = Matrix.CreateTranslation(nextLocation.X, nextLocation.Y, 0);
            foreach (NPC currNPC in playerObject.getNPCs())
            {
                if (currNPC.isAvaliable && currNPC.isVisible)
                {
                    Matrix NPCMat = Matrix.CreateTranslation(currNPC.getLocation().X, currNPC.getLocation().Y, 0);

                    Color[,] NPCColors = Helpers.TextureTo2DArray(NPCTexture);
                    Color[,] PlayerColors = Helpers.TextureTo2DArray(playerTexture);

                    Vector2 collisionPoint = Helpers.TexturesCollide(NPCColors, NPCMat, PlayerColors, playerMat);

                    if (collisionPoint.X != -1)
                    {
                        return currNPC;
                    }
                }
            }
            return null;
        }

        #endregion

    }


}
