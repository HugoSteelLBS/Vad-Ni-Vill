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

namespace Vad_Ni_Vill
{
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
        private double score;

        private bool isPlaying;
        private bool stats;
        private bool pressedOLastFrame;
        private int LaserLifeTimer;
        private int StatLifeTimer;

        private Song SoundSpace;
        private Song LoseSFX;

        private Random rnd;
        string IPString;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

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

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font");
            MonerFont = Content.Load<SpriteFont>("MonerFont");
            playerTexture = Content.Load<Texture2D>("fantasticPlane");
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
            difficulty++;
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
            if (advancedMineTimer == 0)
            {
                advancedMineTimer = 300 - (difficulty/30);

                int startPosX = rnd.Next(gameScreenWidth);

                mines.Add(new AdvancedMine(advancedMineTexture, new Vector2(startPosX, -50)));
            }
            else if (advancedMineTimer < 1)
            {
                advancedMineTimer = 1;
            }
            if (statMineTimer == 0)
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
            else if (statMineTimer < 1)
            {
                statMineTimer = 1;
            }
            
            if (slowMineTimer == 0)
            {
                slowMineTimer = 200 - (difficulty / 50);

                int startPosX = rnd.Next(gameScreenWidth);

                mines.Add(new SlowMine(SlowMineTexture, new Vector2(startPosX, 550)));
            }
            else if (slowMineTimer < 1)
            {
                slowMineTimer = 1;
            }
            if (LaserTimer == 0)
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
            if (stats)
            {
                spriteBatch.DrawString(MonerFont, "Objects = " + (mines.Count).ToString(),
                        new Vector2(10, 50), Color.White);
                spriteBatch.DrawString(MonerFont, "Difficulty = " + ((int)difficulty).ToString(),
                        new Vector2(10, 70), Color.White);
                spriteBatch.DrawString(MonerFont, "IP = " + IPString,
                        new Vector2(10, 90), Color.White);
                spriteBatch.DrawString(MonerFont, "User = " + Environment.UserName,
                        new Vector2(10, 110), Color.White);
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

        private void Reset()
        {
            isPlaying = true;
            mines.Clear();
            player.Position = startPos;
            regularMineTimer = 60;
            advancedMineTimer = 300;
            statMineTimer = 600;
            score = 0;
            difficulty = 0;
        }
        public static Rectangle Normalize(Rectangle reference, Rectangle overlap)
        {
            //Räkna ut en rektangel som kan användas relativt till referensrektangeln
            return new Rectangle(
                overlap.X - reference.X,
                overlap.Y - reference.Y,
                overlap.Width,
                overlap.Height);
        }

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