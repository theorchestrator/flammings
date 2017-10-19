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
    class Lemming
    {
        //Define variables
        Texture2D texture;
        Texture2D box;
        Color[] textureColor;

        public Vector2 position;
        public Vector2 velocity;

        public Rectangle selfRectangle;
        Rectangle rectangle;
        Rectangle textureRectangle;
        Rectangle blockRectangle;

        //Main camera
        Camera camera;
        
        //Sprite variables and size
        int currentFrame;
        public int frameHeight;
        public int frameWidth;

        //Animation related
        float timer;
        float interval = 75;
        public float divideAnim = 3.0f;
        public float velocityMultiplier = 1;

        //States walk-right, Hovering over lemming, If is falling, if is Parachuting, if is blocking, if is phasing
        public bool isHovered = false;
        public bool right = true;
        public bool falling = true; 
        public bool isParachuting = false;
        public bool isBlocking = false;
        public bool isPhasing = false;
        public bool grounded = false;

        //Fallcounter
        public int fallCounter = 0;

        public Lemming(Texture2D newTexture, Texture2D newBox, Vector2 newPosition, int newFrameHeight, int newFrameWidth, Rectangle textureRectangle, Color[] textureColor, Camera camera)
        { 
            //Apply to member variables
            texture = newTexture; 
            position = newPosition; 
            frameHeight = newFrameHeight;
            frameWidth = newFrameWidth; 
            velocity = new Vector2(3.0f,3.0f);
            this.textureRectangle = textureRectangle; 
            this.textureColor = textureColor;
            box = newBox; 
            this.camera = camera;
        }

        public void Update(GameTime gameTime)
        {
            rectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            position = position + velocity;
            selfRectangle = new Rectangle((int)position.X, (int)position.Y, frameWidth, frameHeight);
            blockRectangle = new Rectangle((int)position.X, (int)position.Y, frameWidth, frameHeight);

            MouseState mouseState = Mouse.GetState();
            Point mousePoint = new Point((int)camera.Pos.X + mouseState.X + (int)camera.Pos.X, (int)camera.position.Y + mouseState.Y + (int)camera.position.Y);

            if (selfRectangle.Contains(mousePoint))
            {
                isHovered = true;
            }
            else { isHovered = false; }
            
            if (IntersectRectangleTexture(selfRectangle, this.textureRectangle, this.textureColor)) //If the lemming collides with the level set falling to false
            {
                grounded = true;

                if (falling == false)
                right = !right;
                falling = false;
                
            }

            if (!IntersectRectangleTexture(new Rectangle(selfRectangle.X, selfRectangle.Bottom, //If the Lemming does not touch the bottom with his feet
                selfRectangle.Width, 1), this.textureRectangle, this.textureColor))
            {
                falling = true;
                grounded = false;
                
            }

            else
            {
                grounded = true;
                falling = false; //Otherwise it is colliding and not falling
            }

            if (falling == true && isBlocking == false) //If the lemming is falling
            {
                if (isParachuting)
                {
                    animateParachute(gameTime);
                }
                else
                {
                    animateFalling(gameTime);
                }

                velocity = new Vector2(0, 1 * velocityMultiplier);
            }


            if (right && falling == false && isBlocking == false) //If the initial direction is right and the lemming is not falling
            {
                animateRight(gameTime);
                velocity = new Vector2(2 * velocityMultiplier, 0);
            }
            else if (!right && falling == false && isBlocking == false) //If the direction is left and the lemming is not falling
            { 
                animateLeft(gameTime);
               velocity = new Vector2(-2 * velocityMultiplier, 0);
            }

            if (isBlocking)
            {
                velocity = new Vector2(0, 0);
                block(gameTime);
            }

            if (isPhasing && falling == false)
            {
                velocity = new Vector2(0, 1);
                block(gameTime);
            }

            if (isPhasing && falling == true) {
                isPhasing = false;
            } 

            else { }
        }

        public void animateRight(GameTime gameTime) //Lemming walk right animation
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / divideAnim;
            if (timer > interval)
            {
                currentFrame++;
                timer = 0;
                if (currentFrame > 2)
                {
                    currentFrame = 0;
                }
            }
        }

        public void animateLeft(GameTime gameTime) //Lemming walk left animation
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / divideAnim;
            
            if (timer > interval)
            {
                currentFrame++;
                timer = 0;
                if (currentFrame < 3 || currentFrame > 5)
                {
                    currentFrame = 3;
                }
            }
        }

        public void animateFalling(GameTime gameTime) //Lemming fall animation
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / divideAnim;
            if (timer > interval)
            {
                currentFrame++;
                timer = 0;
                if (currentFrame < 9 || currentFrame > 10)
                {
                    currentFrame = 9;
                }
            }
        }

        public void animateParachute(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / divideAnim;
            if (timer > interval)
            {
                currentFrame++;
                timer = 0;

                if (!right)
                {
                    if (currentFrame < 11 || currentFrame > 12)
                    {
                        currentFrame = 11;
                    }
                }

                else
                {
                    if (currentFrame < 6 || currentFrame > 7)
                    {
                        currentFrame = 6;
                    }
                }
            }
        }

        public void block(GameTime gameTime) //Blocking animation
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / divideAnim;
            if (timer > interval)
            {
                currentFrame++;
                timer = 0;
                if (currentFrame < 8 || currentFrame > 8)
                {
                    currentFrame = 8;
                }
            }
        }

        public void phase(GameTime gameTime) //Phase animation
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / divideAnim;
            if (timer > interval)
            {
                currentFrame++;
                timer = 0;
                if (currentFrame < 10 || currentFrame > 12)
                {
                    currentFrame = 10;
                }
            }

        }

        public void parachute() //Parachute animation
        {
            isParachuting = true;
        }

        public void block() //Parachute animation
        {
            isBlocking = true;
        }

        public void phase() //Parachute animation
        {
            isPhasing = true;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the lemming
            spriteBatch.Draw(texture, position, rectangle, Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);

            //If the mouse hovers over the lemming then draw box around it
            if(isHovered == true)
            {
                spriteBatch.Draw(box, new Vector2(selfRectangle.X,selfRectangle.Y), Color.White);
            }
            //Otherwise don't
            else { }
        }
        
        //Function to check, whether the Lemming-Instance collides with the level texture
        private static bool IntersectRectangleTexture(Rectangle selfRectangle,Rectangle textureRectangle, Color[] textureColor)
        {
            for (int x = selfRectangle.Left; x < selfRectangle.Right; x++) //Scan on Lemming-Intance for collision from left to right on x-axis
            {
                for (int y = selfRectangle.Top; y < selfRectangle.Bottom; y++) //Scan on Lemming-Intance for collision from top to bottom on y-axis
                {
                    Color color = textureColor[x + y * textureRectangle.Width]; //1D-Array of all pixels in the level
                    if (color.A != 0) //Combine both pixels and if the Lemming-Instance collides with anything other than Alpha (Transparent)
                        return true; //Report collision
                }
            }
            return false; //Otherwise report no collision
        }
    }
}
