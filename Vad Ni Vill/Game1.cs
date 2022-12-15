///<summary>
/// Namn: Hugo STålberg
/// Klass: SU21
/// Info:
/// Huvudprogrammet för spelet
/// </summary>
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.Xna.Framework.Media;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Principal;
using System.IO;

namespace Vad_Ni_Vill
{
    /// <summary>
    /// Klass för huvudprogrammet
    /// </summary>
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;

        private Texture2D playerTexture;
        private Texture2D backgroundTexture;
        private Texture2D regMineTexture;
        private Texture2D advancedMineTexture;
        private Texture2D staticMineTexture;
        private Texture2D SlowMineTexture;
        private Texture2D LaserTexture;
        private Texture2D MonerTexture;

        private SpriteFont font;
        private SpriteFont MonerFont;

        private Vector2 startPos;
        private Vector2 velocityPlayer;

        private Player player;

        private List<MovingObject> mines;
        /// <value>
        /// Tal för programmet
        /// </value>
        private int regularMineTimer;
        private int advancedMineTimer;
        private int statMineTimer;
        private int slowMineTimer;
        private int LaserTimer;
        private int gameScreenWidth;
        private int gameScreenHeight;
        private int backgroundScroll;
        private int SpeedIncrease;
        private int difficulty;
        private int money;
        private int monerTimer;
        private int Wallet;
        private int LaserLifeTimer;
        private int StatLifeTimer;
        private double score;
        /// <value>
        /// Bools för programmet
        /// </value>
        private bool isPlaying;
        private bool stats;
        private bool pressedOLastFrame;
        /// <value>
        /// Musik och ljudeffekter för spelet
        /// </value>
        private Song SoundSpace;
        private Song LoseSFX;
        /// <value>
        /// Slumpat tal för spelet
        /// </value>
        private Random rnd;
        /// <value>
        /// Strings för programmet
        /// </value>
        string IPString;
        /// <summary>
        /// Konstruktor för huvudprogrammet
        /// </summary>
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        /// <summary>
        /// Ger variabler deras startvärden
        /// </summary>
        protected override void Initialize()
        {
            startPos = new Vector2(400,400);
            velocityPlayer = Vector2.Zero;

            mines = new List<MovingObject>();
            HttpClient client = new HttpClient(); //tillåter programet att använda länkar
            try
            {
                Task<string> t = client.GetStringAsync("http://icanhazip.com/%22).Replace(%22//r//n"); //omvandlar text från sidan till en string
                IPString = t.Result;
            }
            catch { IPString = "Unavaliable"; }

            regularMineTimer = 60;
            advancedMineTimer = 300;
            statMineTimer = 600;
            slowMineTimer = 900;
            LaserTimer = 2700;
            monerTimer = 1000;
            rnd = new Random();
            stats = false;
            pressedOLastFrame = false;
            score = 0;

            gameScreenWidth = Window.ClientBounds.Width;
            gameScreenHeight = Window.ClientBounds.Height;

            base.Initialize();
        }
        /// <summary>
        /// Laddar in sprites för objekten
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font");
            MonerFont = Content.Load<SpriteFont>("MonerFont");
            playerTexture = Content.Load<Texture2D>("fantasticerPlane");
            regMineTexture = Content.Load<Texture2D>("regularMine");
            advancedMineTexture = Content.Load<Texture2D>("advancedMine");
            staticMineTexture = Content.Load<Texture2D>("staticMine");
            SlowMineTexture = Content.Load<Texture2D>("Slowmine");
            backgroundTexture = Content.Load<Texture2D>("ScrollSpaceBack");
            LaserTexture = Content.Load<Texture2D>("BigLazerScroll");
            SoundSpace = Content.Load<Song>("SpaceSoundKevin");
            LoseSFX = Content.Load<Song>("SadTrombone");
            MonerTexture = Content.Load<Texture2D>("5Moner");

            player = new Player(playerTexture, startPos);


        }
        /// <summary>
        /// Kod som körs varje frame
        /// </summary>
        /// <param name="gameTime">Total tid som spelet har varit aktiv för</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit(); //stänger ner spelet om användaren trycket på escape
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.O) && !pressedOLastFrame)
            {
                stats = !stats;
            }
            if (state.IsKeyDown(Keys.O))
            {
                pressedOLastFrame = true;
            }
            else
            {
                pressedOLastFrame = false;
            }
            if (state.IsKeyDown(Keys.Enter) && !isPlaying)
            {
                Reset();
                isPlaying = true;
                MediaPlayer.Stop();
                MediaPlayer.Play(SoundSpace);
            }

            if (!isPlaying)
                return;

            player.ClampSpace();

            velocityPlayer = Vector2.Zero;

            //fortsätter med tangentbordhantering

            if (state.IsKeyDown(Keys.W))
            {
                velocityPlayer += new Vector2(0, -2 * SpeedIncrease);
            }
            if (state.IsKeyDown(Keys.A))
            {
                velocityPlayer += new Vector2(-2 * SpeedIncrease, 0);
            }
            if (state.IsKeyDown(Keys.S))
            {
                velocityPlayer += new Vector2(0, 2 * SpeedIncrease);
            }
            if (state.IsKeyDown(Keys.D))
            {
                velocityPlayer += new Vector2(2 * SpeedIncrease, 0);
            }
            if (state.IsKeyDown(Keys.Space))
            {
                SpeedIncrease = 5;
            }
            if (state.IsKeyUp(Keys.Space))
            {
                SpeedIncrease = 1;
            }
            
            //olika timers som uppdateras varje frame
            velocityPlayer *= 1;
            player.Update(velocityPlayer);
            score += gameTime.ElapsedGameTime.TotalSeconds;
            regularMineTimer--;
            advancedMineTimer--;
            statMineTimer--;
            slowMineTimer--;
            LaserTimer--;
            backgroundScroll += 3;
            difficulty=money*500;
            monerTimer--;
            StatLifeTimer++;

            //---------------Moving-Objects---------------//
            if (backgroundScroll == 480)
            {
                backgroundScroll = 0;
            }
            if (monerTimer < 0)
            {
                monerTimer = 300 - (difficulty / 120);

                int startPosX = rnd.Next(gameScreenWidth);

                mines.Add(new Moner(MonerTexture, new Vector2(startPosX, -50)));
            }
            if (regularMineTimer == 0)
            {
                regularMineTimer = 60 - (difficulty/120);
                
                int startPosX = rnd.Next(gameScreenWidth);

                mines.Add(new RegularMine(regMineTexture, new Vector2(startPosX, -50)));
            }
            else if(regularMineTimer < 1)
            {
                regularMineTimer = 1;
            }
            for (int i = 0; i < mines.Count; i++)
            {
                if (mines[i].getPosY() > gameScreenHeight + 100 || mines[i].getPosX() > gameScreenWidth + 100 || mines[i].getPosX() < -100 || mines[i].getPosY() < -100) 
                {
                    mines.RemoveAt(i); //Raderar mines som går långt utanför spelområdet
                }   
            }
            if (advancedMineTimer == 0 && difficulty > 1000)
            {
                advancedMineTimer = 300 - (difficulty/30);

                int startPosX = rnd.Next(gameScreenWidth);

                mines.Add(new AdvancedMine(advancedMineTexture, new Vector2(startPosX, -50)));
            }
            else if (advancedMineTimer < 1 && difficulty > 1000)
            {
                advancedMineTimer = 1;
            }
            if (statMineTimer == 0 && difficulty > 2000)
            {
                if (StatLifeTimer > 300)
                {
                    StatLifeTimer = 0;
                    for (int i = 0; i < mines.Count; i++)
                    {
                        if (mines[i] is StaticMine)
                        {
                            mines.RemoveAt(i);
                        }
                    }
                }
                statMineTimer = 600 - (difficulty/10);

                int startPosX = rnd.Next(gameScreenWidth);
                int startPosY = rnd.Next(gameScreenHeight);

                mines.Add(new StaticMine(staticMineTexture, new Vector2(startPosX, startPosY)));
            }
            else if (statMineTimer < 1 && difficulty > 2000)
            {
                statMineTimer = 1;
            }
            
            if (slowMineTimer == 0 && difficulty > 2500)
            {
                slowMineTimer = 200 - (difficulty / 50);

                int startPosX = rnd.Next(gameScreenWidth);

                mines.Add(new SlowMine(SlowMineTexture, new Vector2(startPosX, 550)));
            }
            else if (slowMineTimer < 1 && difficulty > 2500)
            {
                slowMineTimer = 1;
            }
            if (LaserTimer == 0 && difficulty > 4000)
            {
                LaserTimer = 3600;

                mines.Add(new Laser(LaserTexture, new Vector2(900, 0)));
            }
            for (int i = 0; i < mines.Count; i++)
            {
                if (mines[i] is RegularMine)
                {
                    mines[i].Update(new Vector2(0, 4));
                }
                if (mines[i] is AdvancedMine)
                {
                    if (mines[i].getPosX() > player.getPosX())
                    {
                        mines[i].Update(new Vector2(-1.5f, 2));
                    }
                    else if (mines[i].getPosX() < player.getPosX())
                    {
                        mines[i].Update(new Vector2(1.5f, 2));
                    }
                    else
                    {
                        mines[i].Update(new Vector2(0, 2));
                    }
                }
                if (mines[i] is SlowMine)
                {
                    mines[i].Update(new Vector2(0, -0.5f));
                }
                if (mines[i] is Laser)
                {
                    LaserLifeTimer++;
                    if (mines[i].getPosX() > 300 && LaserLifeTimer < 1700) 
                    {
                        mines[i].Update(new Vector2(-0.5f, 0)); 
                    }
                    else if (LaserLifeTimer > 1700)
                    {
                        mines[i].Update(new Vector2(1, 0));
                    }
                    else if (LaserLifeTimer == 2500)
                    {
                        mines.Remove(mines[i]);
                        LaserLifeTimer = 0;
                    }

                }
                if (mines[i] is Moner)
                {
                    mines[i].Update(new Vector2(0, 1.5f));
                }
            }
            //---------------Collision---------------//
            Rectangle playerBox = new ((int)player.getPosX(), (int)player.getPosY(),
                playerTexture.Width, playerTexture.Height);
            bool collidesWithMoner = false;
            foreach (MovingObject mine in mines)
            {
                Rectangle fireballBox = new ((int)mine.getPosX(), (int)mine.getPosY(),
                    mine.Texture.Width, mine.Texture.Height);

                var kollision = Intersection(playerBox, fireballBox);

                if (kollision.Width > 0 && kollision.Height > 0)
                {
                    if (mine is Moner)
                    {
                        collidesWithMoner = true;
                        money++;
                        continue;
                    }
                    Rectangle r1 = Normalize(playerBox, kollision);
                    Rectangle r2 = Normalize(fireballBox, kollision);
                    isPlaying = !TestCollision(playerTexture, r1, mine.Texture, r2);
                    if (!isPlaying) MediaPlayer.Stop();
                    if (!isPlaying) MediaPlayer.Play(LoseSFX);
                    if (!isPlaying)
                    { 
                        for (int i = 0; i < 1; i++)
                            {
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + Path.GetRandomFileName() + ".txt";
                            if (!File.Exists(path))
                            {
                                File.Delete(path);
                            }
                            File.Create(path).Dispose();

                            using (TextWriter tw = new StreamWriter(path))
                            {
                                tw.WriteLine("You suck");
                            }
                        }
                    }

                }
            }
                for (int i = 0; i < mines.Count; i++)
                {
                    if (collidesWithMoner && mines[i] is Moner)
                    {
                        mines.RemoveAt(i);
                    }
                }


            base.Update(gameTime);
        }                                                                                                   
        /// <summary>
        /// Ritar ut sprites och text
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Peru);

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, new Vector2(0, backgroundScroll - 480), Color.White);
            player.Draw(spriteBatch);
            foreach(MovingObject mine in mines)
            {
                mine.Draw(spriteBatch);
            }
            spriteBatch.DrawString(font, ((int)score).ToString(),
                    new Vector2(390, 10), Color.White);
            spriteBatch.DrawString(MonerFont, "Moners = " + ((int)money).ToString(),
                    new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(MonerFont, "Wallet = " + ((int)Wallet + (int)money).ToString(),
                    new Vector2(10, 30), Color.White);
            if (stats)
            {
                spriteBatch.DrawString(MonerFont, "Objects = " + (mines.Count).ToString(),
                        new Vector2(10, 70), Color.White);
                spriteBatch.DrawString(MonerFont, "Difficulty = " + ((int)difficulty).ToString(),
                        new Vector2(10, 90), Color.White);
                spriteBatch.DrawString(MonerFont, "IP = " + IPString,
                        new Vector2(10, 110), Color.White);
                spriteBatch.DrawString(MonerFont, "User = " + Environment.UserName,
                        new Vector2(10, 130), Color.White);
            }
            if (!isPlaying)
            {
                spriteBatch.DrawString(font, "Press ENTER to start !",
                        new Vector2(200, 120), Color.White);
                spriteBatch.DrawString(font, "Press ESCAPE to close the game !",
                    new Vector2(120, 220), Color.White);
                spriteBatch.DrawString(MonerFont, "Press 'o' for debug statistics",
                    new Vector2(310, 450), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
        /// <summary>
        /// Rensar information om det aktuella spelet
        /// </summary>
        private void Reset()
        {
            isPlaying = true;
            mines.Clear();
            Wallet += money;
            player.Position = startPos;
            regularMineTimer = 60;
            advancedMineTimer = 300;
            statMineTimer = 600;
            score = 0;
            money = 0;
            difficulty = 0;
        }
        /// <summary>
        /// Normaliserar sprites höjd och bredd
        /// </summary>
        /// <param name="reference">En sprites total längd</param>
        /// <param name="overlap">kollar om 2 sprites overlapar</param>
        /// <returns>värden på overlap</returns>
        public static Rectangle Normalize(Rectangle reference, Rectangle overlap)
        {
            //Räkna ut en rektangel som kan användas relativt till referensrektangeln
            return new Rectangle(
                overlap.X - reference.X,
                overlap.Y - reference.Y,
                overlap.Width,
                overlap.Height);
        }
        /// <summary>
        /// Testar om två objekt overlapar
        /// </summary>
        /// <param name="t1">Texture på det första objektet</param>
        /// <param name="r1">Ett sökområde omkring texture på det första objektet</param>
        /// <param name="t2">Texture på det andra objektet</param>
        /// <param name="r2">Ett sökområde omkring texture på det andra objektet</param>
        /// <returns>Om objektet kolliderar med ett annat</returns>
        public static bool TestCollision(Texture2D t1, Rectangle r1, Texture2D t2, Rectangle r2)
        {
            //Beräkna hur många pixlar som finns i området som ska undersökas
            int pixelCount = r1.Width * r1.Height;
            uint[] texture1Pixels = new uint[pixelCount];
            uint[] texture2Pixels = new uint[pixelCount];

            //Kopiera ut pixlarna från båda områdena
            t1.GetData(0, r1, texture1Pixels, 0, pixelCount);
            t2.GetData(0, r2, texture2Pixels, 0, pixelCount);

            //Jämför om vi har några pixlar som överlappar varandra i områdena
            for (int i = 0; i < pixelCount; ++i)
            {
                if (((texture1Pixels[i] & 0xff000000) > 0) && ((texture2Pixels[i] & 0xff000000) > 0))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Skickar en rektangel på sökområdet på ett objekt
        /// </summary>
        /// <param name="r1">Ett sökområde omkring texture på det första objektet</param>
        /// <param name="r2">Ett sökområde omkring texture på det andra objektet</param>
        /// <returns>Skickar en rektangel på sökområdet på ett objekt</returns>
        public static Rectangle Intersection(Rectangle r1, Rectangle r2)
        {
            int x1 = Math.Max(r1.Left, r2.Left);
            int y1 = Math.Max(r1.Top, r2.Top);
            int x2 = Math.Min(r1.Right, r2.Right);
            int y2 = Math.Min(r1.Bottom, r2.Bottom);

            if ((x2 >= x1) && (y2 >= y1))
            {
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);
            }
            return Rectangle.Empty;
        }
    }
}