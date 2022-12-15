/// <summary>
/// Namn: Hugo Stålberg
/// Klass: SU21
/// Info:
/// Innehåller klasser för programmet Game1
/// </summary>
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vad_Ni_Vill
{
    /// <summary>
    /// Klass för Rörliga objekt
    /// </summary>
    class MovingObject
    {
        /// <value>
        /// Positioner på objekt
        /// </value>
        public Vector2 Position { get; set; }
        /// <value>
        /// Texturer på objekt
        /// </value>
        public Texture2D Texture { get; set; }
        /// <summary>
        /// Konstruktor för klassen MovingObject
        /// </summary>
        /// <param name="position">Position på objekt</param>
        /// <param name="texture">Texture på ett objekt</param>
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
        /// <summary>
        /// Returnerar ett objekts X Position
        /// </summary>
        /// <returns>Returnerar ett objekts X Position</returns>
        public float getPosX()
        {
            return Position.X;
        }
        /// <summary>
        /// Returnerar ett objekts Y Position
        /// </summary>
        /// <returns>Returnerar ett objekts Y Position</returns>
        public float getPosY()
        {
            return Position.Y;
        }
    }

    class Player : MovingObject
    {
        /// <summary>
        /// Ger spelaren parametrarna texure och position
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public Player(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
        /// <summary>
        /// begränsar spelområdet till fönstrets bredd och höjd
        /// </summary>
        public void ClampSpace()
        {
            Position = new Vector2(Math.Clamp(Position.X, 0, 800-Texture.Width), Math.Clamp(Position.Y, 0, 480 - Texture.Height));
        }
    }

    class AdvancedMine : MovingObject
    {
        /// <summary>
        /// Ger AdvancedMine parametrarna texure och position
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public AdvancedMine(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    class RegularMine : MovingObject
    {
        /// <summary>
        /// Ger RegularMine parametrarna texure och position
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public RegularMine(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    class StaticMine : MovingObject
    {
        /// <summary>
        /// Ger StaticMine parametrarna texure och position
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public StaticMine(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    class SlowMine : MovingObject
    {
        /// <summary>
        /// Ger SlowMine parametrarna texure och position
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public SlowMine(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    class Laser : MovingObject
    {
        /// <summary>
        /// Ger Laser parametrarna texure och position
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public Laser(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    class Moner : MovingObject
    {
        /// <summary>
        /// Ger Moner parametrarna texure och position
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public Moner(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
}
