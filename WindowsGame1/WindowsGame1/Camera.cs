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
  
    public class Camera
    {

        //Define variables
        protected float zoom; // Camera-Zoom
        public Matrix transform; // Matrixtransformation
        public Vector2 position; // Camera-Position
        protected float rotation; // Camera-Rotation
        public Viewport vp; //Viewport

        public Camera()
        {
            zoom = 1.0f;
            rotation = 0.0f;
            position = Vector2.Zero;
            Origin = new Vector2(vp.Width / 2.0f, vp.Height / 2.0f);
            Zoom = 1.0f;
        }

        public Vector2 Origin { get; set; } //Camera-Origin

        // Camera-Zoom - Negative zoom = horizontal and vertical flipped picture
        public float Zoom 
        {
            get { return zoom; }
            set
            {
                zoom = value;
                if (zoom < 0.1f)
                {
                    zoom = 0.1f;
                }
            } 
        }

        //Move
        public void Move(Vector2 amount)
        {
            position += amount;
        }

        //Camera-Rotation
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        //Camera-Position
        public Vector2 Pos
        {
            get { return position; }
            set { position = value; }
        }

        //Matrix-Transformation with parallax option added
        public Matrix ViewMatrix(Vector2 parallax)
        {
            return Matrix.CreateTranslation(new Vector3(-position * parallax, 0.0f)) *
            Matrix.CreateTranslation(new Vector3(Origin, 0.0f)) *
            Matrix.CreateRotationZ(Rotation) *
            Matrix.CreateScale(Zoom, Zoom, 1) *
            Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }
    }  
}