using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vad_Ni_Vill
{
    class MovingObject
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }

        public MovingObject(Vector2 position, Texture2D texture)
        {
            Position = position;
            Texture = texture;
        }

        public void Update(Vector2 velocity)
        {
            Position += velocity;
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
        public float getPosX()
        {
            return Position.X;
        }
        public float getPosY()
        {
            return Position.Y;
        }
    }

    class Player : MovingObject
    {
        public Player(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }

        public void ClampSpace()
        {
            Position = new Vector2(Math.Clamp(Position.X, 0, 800-Texture.Width), Math.Clamp(Position.Y, 0, 480 - Texture.Height)); //begränsar spelområdet
        }
    }

    class AdvancedMine : MovingObject
    {
        public AdvancedMine(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    class RegularMine : MovingObject
    {
        public RegularMine(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    class StaticMine : MovingObject
    {
        public StaticMine(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    class SlowMine : MovingObject
    {
        public SlowMine(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    class Laser : MovingObject
    {
        public Laser(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    class Moner : MovingObject
    {
        public Moner(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
}
