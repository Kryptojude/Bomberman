using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Diagnostics;
using System.Resources;


namespace Bomberman
{
    public partial class Form1 : Form
    {
        public class Bomb
        {
            public Rectangle rec;
            public int tick = 0;
            public bool passThrough = true;

            public Bomb(Rectangle rec)
            {
                this.rec = rec;
            }
        }
        public class Explosion
        {
            public int tick = 0;
            public List<Rectangle> recs = new List<Rectangle>();
            public List<string> recType = new List<string>();

            public Explosion() { }
        }
        public class Player
        {
            public int number;
            public Rectangle rec; //Location, Size
            public Point lastPos;
            public int speed = 6;
            public string action = "idle";
            public List<Bomb> bombs = new List<Bomb>();
            public int bombLimit = 1;
            public const int maxBombLimit = 10;
            public int bombRadius = 1;
            public static int maxBombRadius = 10;
            public List<Explosion> explosions = new List<Explosion>();
            public int hp;
            public static int maxHp = 30;
            public Dictionary<string, Bitmap[]> walkCycles = new Dictionary<string, Bitmap[]> { { "up", new Bitmap[8] }, { "right", new Bitmap[8] }, { "down", new Bitmap[8] }, { "left", new Bitmap[8] }, { "idle", new Bitmap[1] } };
            public int walkFrame = 0;
            public int invincibleTicks = -1;
            public int blockPassTicks = -1;
            public int superBombTicks = -1;

            public Player(Rectangle rec, int number)
            {
                this.rec = rec;
                this.number = number;
                hp = maxHp;
            }

            public void SetHp(int setHp)
            {
                hp = setHp;
                if (hp > maxHp)
                    hp = maxHp;
            }
        }

        Player[] p = new Player[] { new Player(new Rectangle(0, 0, ts, ts), 0), new Player(new Rectangle(0, 0, ts, ts), 1) };
        PictureBox statusBar = new PictureBox { Width = 400, Height = ts, BackColor = Color.Gray };
        PictureBox gameBox = new PictureBox { Top = ts, Width = 400, Height = 400 };

        private Size _bodySize;
        public Size BodySize //Property handles resizing of all container elements i.e. form1, gameBox, statusBar and positions the players
        {
            get { return _bodySize; }
            set
            {
                _bodySize = value;

                gameBox.Width = value.Width; //resize gameBox
                gameBox.Height = value.Height -statusBar.Height; //resize gameBox
                statusBar.Width = value.Width; //resize statusBar
                Application.OpenForms["Form1"].Size = new Size(value.Width + 16, value.Height + 39); //Resize window

                p[1].rec.Location = new Point(value.Width -ts, value.Height -ts -statusBar.Height); //Position player1
            }
        }

        static int ts = 50;
        Random random = new Random();
        bool firstStart = true;
        int level = 0;
        Dictionary<string, object[]> keyInfo = new Dictionary<string, object[]> { { "key",    new object[] { Key.Space, Key.W,  Key.S,  Key.A,   Key.D, Key.Enter, Key.Up, Key.Down, Key.Left, Key.Right } },
                                                                                         { "player", new object[] {         0,     0,      0,      0,       0,         1,      1,        1,        1,         1 } },
                                                                                         { "action", new object[] {    "bomb",  "up", "down", "left", "right",    "bomb",   "up",   "down",   "left",   "right" } }, };
        Timer engine = new Timer { Interval = 1000 / framerate };
        int frame = 0;
        static int framerate = 40;
        int itemPos = 11; //Global item movement
        int itemDirection = 1;
        List<Rectangle> itemRecs = new List<Rectangle>();
        List<string> itemType = new List<string>();
        Dictionary<string, Bitmap> itemPics = new Dictionary<string, Bitmap>();
        Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();

        object[] levels = new object[4]; //data for all levels
        int[,] wallData; //data for current level
        List<Rectangle> walls = new List<Rectangle>();
        List<int> wallType = new List<int>();

        public Form1()
        {
            InitializeComponent(); //Starting screen
        }

        private void SelectLevel(object Sender, EventArgs e)
        {
            PictureBox sender = (PictureBox)Sender;
            level = Convert.ToInt32(sender.Name.Substring(1));

            foreach (Control c in startScreen.Controls) //Remove all borders
                if (c is PictureBox)
                    (c as PictureBox).BorderStyle = BorderStyle.None;

            sender.BorderStyle = BorderStyle.FixedSingle; //Add border on clicked element
        }

        public void StartGame(object Sender, EventArgs e)
        {
            if(firstStart)
            {
                GenerateAssets();
                gameBox.Paint += Game_Paint;
                statusBar.Paint += StatusBar_Paint;
                engine.Tick += Engine_Tick;

                levels[0] = new int[15, 23] { { 0,0,2,0,2,2,0,2,0,2,2,2,2,2,0,2,0,2,2,2,0,2,2 },
                                         { 0,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,0,1,2,1,0 },
                                         { 2,2,0,2,0,2,0,1,0,2,0,2,0,2,0,1,2,0,2,2,0,2,2 },
                                         { 0,1,2,1,1,1,1,1,2,1,2,1,2,1,2,1,1,1,1,1,2,1,2 },
                                         { 2,0,2,2,0,2,0,2,2,0,0,1,0,2,0,0,2,2,0,2,0,2,0 },
                                         { 2,1,0,1,2,1,2,1,0,1,2,1,2,1,2,1,2,1,2,1,2,1,2 },
                                         { 2,2,2,1,2,0,2,0,2,2,2,2,2,0,2,2,0,2,0,1,0,2,2 },
                                         { 0,1,2,1,2,1,2,1,1,1,2,1,2,1,1,1,2,1,2,1,2,1,0 },
                                         { 2,2,0,1,0,2,0,2,2,0,2,2,2,2,2,0,2,0,2,1,2,2,2 },
                                         { 2,1,2,1,2,1,2,1,2,1,2,1,2,1,0,1,2,1,2,1,0,1,2 },
                                         { 0,2,0,2,0,2,2,0,0,2,0,1,0,0,2,2,0,2,0,2,2,0,2 },
                                         { 2,1,2,1,1,1,1,1,2,1,2,1,2,1,2,1,1,1,1,1,2,1,0 },
                                         { 2,2,0,2,2,0,2,1,0,2,0,2,0,2,0,1,0,2,0,2,0,2,2 },
                                         { 0,1,2,1,0,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,0 },
                                         { 2,2,0,2,2,2,0,2,0,2,2,2,2,2,0,2,0,2,2,0,2,0,0 }  }; // 0 = empty, 1 = wall, 2 = breakable wall, 3 = item
                levels[1] = new int[11, 11] { { 0,0,0,2,0,2,0,2,0,0,0 },
                                         { 0,1,1,1,2,1,2,1,1,1,0 },
                                         { 0,1,0,2,0,0,0,2,0,1,0 },
                                         { 2,1,0,1,0,1,0,1,0,1,2 },
                                         { 0,2,0,2,0,0,0,2,0,2,0 },
                                         { 2,1,0,1,0,1,0,1,0,1,2 },
                                         { 0,2,0,2,0,0,0,2,0,2,0 },
                                         { 2,1,0,1,0,1,0,1,0,1,2 },
                                         { 0,1,0,2,0,0,0,2,0,1,0 },
                                         { 0,1,1,1,2,1,2,1,1,1,0 },
                                         { 0,0,0,2,0,2,0,2,0,0,0 }, };
                levels[2] = new int[1, 1] { { 1 } };

                firstStart = false;
            }

            wallData = (int[,])levels[level]; //Load selected map into wallData

            //Make wall rectangles from wallData
            for (int y = 0; y < wallData.GetLength(0); y++)
            {
                for (int x = 0; x < wallData.GetLength(1); x++)
                {
                    if (wallData[y, x] == 1 || wallData[y, x] == 2)
                    {
                        wallType.Add(wallData[y, x]);
                        walls.Add(new Rectangle(x * ts, y * ts, ts, ts));
                    }
                }
            }

            BodySize = new Size(wallData.GetLength(1) * ts, wallData.GetLength(0) * ts + ts); //resize based on new wallData
            Controls.Add(gameBox);
            Controls.Add(statusBar);
            Controls.Remove(startScreen);
            gameBox.Focus(); //Take focus away from button

            engine.Start();
        }

        private void GenerateAssets()
        {
            //Create necessary assets
            //Convert garbage resource trash into list/dictionary
            using (ResourceSet resourceSet = Properties.items.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true))
            {
                foreach (DictionaryEntry entry in resourceSet)
                {
                    itemPics.Add((string)entry.Key, (Bitmap)entry.Value);
                }
            }
            using (ResourceSet resourceSet = Properties.Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true))
            {
                foreach (DictionaryEntry entry in resourceSet)
                {
                    if (entry.Value is Bitmap)
                        bitmaps.Add((string)entry.Key, (Bitmap)entry.Value);
                }
            }

            //Extract individual sprites from the explosionBits spritesheet
            bitmaps.Add("explosionCenter", bitmaps["explosionBits"].Clone(new Rectangle(0, 64, 32, 32), bitmaps["explosionBits"].PixelFormat));
            bitmaps.Add("explosionMiddle0", bitmaps["explosionBits"].Clone(new Rectangle(32, 64, 32, 32), bitmaps["explosionBits"].PixelFormat));
            bitmaps.Add("explosionMiddle1", bitmaps["explosionBits"].Clone(new Rectangle(64, 64, 32, 32), bitmaps["explosionBits"].PixelFormat));
            bitmaps.Add("explosionMiddle2", bitmaps["explosionBits"].Clone(new Rectangle(32, 64, 32, 32), bitmaps["explosionBits"].PixelFormat));
            bitmaps.Add("explosionMiddle3", bitmaps["explosionBits"].Clone(new Rectangle(64, 64, 32, 32), bitmaps["explosionBits"].PixelFormat));
            bitmaps.Add("explosionEnd0", bitmaps["explosionBits"].Clone(new Rectangle(96, 64, 32, 32), bitmaps["explosionBits"].PixelFormat));
            bitmaps.Add("explosionEnd1", bitmaps["explosionBits"].Clone(new Rectangle(160, 64, 32, 32), bitmaps["explosionBits"].PixelFormat));
            bitmaps.Add("explosionEnd2", bitmaps["explosionBits"].Clone(new Rectangle(128, 64, 32, 32), bitmaps["explosionBits"].PixelFormat));
            bitmaps.Add("explosionEnd3", bitmaps["explosionBits"].Clone(new Rectangle(192, 64, 32, 32), bitmaps["explosionBits"].PixelFormat));

            //Extract walk sprites into walkCycles array ?rewrite like explosionBits for simplicity?
            foreach (Player player in p)
            {
                Bitmap playerCycleMap = bitmaps["walkCycle" + player.number];
                for (int walkType = 0; walkType < player.walkCycles.Count; walkType++) //Cycle through the 5 walkTypes
                {
                    int b;
                    int Index = 0;

                    for (b = 0; Index < player.walkCycles.ElementAt(walkType).Value.Length && b < 5; b++, Index++) //Put the first 5 frames in, by going all the way to the right of the sheet
                        player.walkCycles.ElementAt(walkType).Value[Index] = playerCycleMap.Clone(new Rectangle(b * 32, walkType * 32, 32, 32), playerCycleMap.PixelFormat);
                    for (b = 3; Index < player.walkCycles.ElementAt(walkType).Value.Length; b--, Index++) //Set b from 4 to 3 and decrement down, but the index has to keep going up till it reaches 7
                        player.walkCycles.ElementAt(walkType).Value[Index] = playerCycleMap.Clone(new Rectangle(b * 32, walkType * 32, 32, 32), playerCycleMap.PixelFormat);
                }
            }
        }

        private void Engine_Tick(object sender, EventArgs e)
        {
            //Controls/Movement
            for (int i = 0; i < keyInfo["key"].Length; i++)
            {
                Player player = p[(int)keyInfo["player"][i]];
                Key key = (Key)keyInfo["key"][i];
                if (Keyboard.IsKeyDown(key)) //Check if key is pressed
                {
                    if ((string)keyInfo["action"][i] != "bomb")
                    {
                        player.action = (string)keyInfo["action"][i];
                        player.lastPos = player.rec.Location; //Save last player position
                        switch (player.action)
                        {
                            case "up":
                                player.rec.Y -= player.speed;
                                break;
                            case "right":
                                player.rec.X += player.speed;
                                break;
                            case "down":
                                player.rec.Y += player.speed;
                                break;
                            case "left":
                                player.rec.X -= player.speed;
                                break;
                        }
                        Collision(player); //Check collision and reverse move if necessary

                        //snapY
                        int nearestSnap50Y = (50 * (int)Math.Round(player.rec.Y / 50.0)); //Determine the nearest row line
                        if (Math.Abs(player.rec.Y - nearestSnap50Y) <= player.speed / 2) //If the distance to that row line is half the speed
                            player.rec.Y = nearestSnap50Y; //Then snap to it
                        //snapX
                        int nearestSnap50X = (50 * (int)Math.Round(player.rec.X / 50.0)); //Determine the neares column line
                        if (Math.Abs(player.rec.X - nearestSnap50X) <= player.speed / 2) //if the distance to that column line is half the speed
                            player.rec.X = nearestSnap50X; //Then snap to it

                        if (player == p[0]) //Disable multiple key presses, except bombs
                            i = 4;
                        else
                            break;
                    }
                    else //if it's not a movement, lay bomb
                        LayBomb(player);
                }
                else //pressing a movement key breaks the loop, so this can only be the last statement if no move key has been pressed at all
                    player.action = "idle";
            }

            //Keep walk animation going
            foreach (Player player in p)
            {
                if (frame % 4 == 0)
                    player.walkFrame++;
                if (player.walkFrame >= player.walkCycles[player.action].Length) //roll-over
                    player.walkFrame = 0;
            }

            //Keep item animation going
            if(frame % 4 == 0)
            {
                itemPos += itemDirection;
                if (itemPos <= 10 || itemPos >= 40 - 26)
                    itemDirection *= -1;

                for (int i = 0; i < itemRecs.Count; i++)
                {
                    Rectangle rec = itemRecs[i];
                    itemRecs[i] = new Rectangle(rec.X, rec.Y / ts * ts + itemPos, rec.Width, rec.Height);
                }
            }

            //Check all bombs for passThrough
            foreach (Player player in p)
                foreach (Bomb bomb in player.bombs)
                {
                    if (bomb.passThrough)
                    {
                        if (!player.rec.IntersectsWith(bomb.rec)) //If no intersection, then deactivate passThrough
                            bomb.passThrough = false; //Works only between bomb and its player paosidvhpoa7896666666666666666666666666666666666666666666666666666666666666666666
                    }
                }

            //Tick counting
            foreach (Player player in p)
            {
                //Bomb/explosion timing
                for (int b = 0; b < player.bombs.Count; b++) //For every bomb
                {
                    player.bombs[b].tick += 1; //Increment current bomb's tick
                    if (player.bombs[b].tick >= 1.5 * framerate) //Explode after 3 seconds
                    {
                        Explode(player, player.bombs[b].rec);

                        player.bombs.RemoveAt(b); //Remove that bomb
                        b--;
                    }
                }

                for (int ex = 0; ex < player.explosions.Count; ex++) //For every explosion
                {
                    player.explosions[ex].tick += 1; //Increment tick
                    if (player.explosions[ex].tick == 0.75 * framerate) //Remove explosion after 1 second
                    {
                        player.explosions.RemoveAt(ex);
                        ex--;
                    }
                }

                //invincible timing
                if (player.invincibleTicks != -1)
                    player.invincibleTicks++;
                if (player.invincibleTicks >= 5 * framerate)
                    player.invincibleTicks = -1;

                //blockPass timing
                if (player.blockPassTicks != -1)
                    player.blockPassTicks++;
                if (player.blockPassTicks >= 5 * framerate)
                    player.blockPassTicks = -1;

                //superBomb timing
                if (player.superBombTicks != -1)
                    player.superBombTicks++;
                if (player.superBombTicks >= 5 * framerate)
                    player.superBombTicks = -1;
            }

            //Check player - explosion collision
            foreach (Player player in p)
                foreach (Player otherPlayer in p)
                    foreach (Explosion ex in otherPlayer.explosions)
                        foreach (Rectangle rec in ex.recs)
                            if (rec.IntersectsWith(player.rec) && player.invincibleTicks == -1)
                            {
                                player.hp--;
                                if (player.hp <= 0)
                                {
                                    GameOver();
                                }
                            }

            Refresh();
            frame++;
            if (frame == framerate)
                frame = 0;
        }

        private void GameOver()
        {
            engine.Stop();
            Controls.Remove(gameBox);
            Controls.Remove(statusBar);
            Controls.Add(startScreen);
            BodySize = new Size(400, 400);
        }

        private void StatusBar_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(p[0].walkCycles["down"][2], new Rectangle(0, 0, ts, ts) );
            foreach (Player player in p)
                for (int h = 0; h < player.hp; h++) //Hearts
                    e.Graphics.DrawImage(bitmaps["heart"], player.number* (gameBox.Width - 175) + h*5, 0, 30, 30 );
        }

        private void Game_Paint(object sender, PaintEventArgs e)
        {
            for (int y = 0; y < wallData.GetLength(0); y++) //Walls
            {
                for (int x = 0; x < wallData.GetLength(1); x++)
                {
                    switch (wallData[y, x])
                    {
                        case 1: e.Graphics.FillRectangle(Brushes.Black, x * ts, y * ts, ts, ts);
                            break;
                        case 2: e.Graphics.FillRectangle(Brushes.Red, x * ts, y * ts, ts, ts);
                            break;
                    }
                    
                }
            }

            for (int i = 0; i < itemRecs.Count; i++) //Items
                e.Graphics.DrawImage(itemPics[itemType[i]], itemRecs[i]);

            foreach (Player player in p)
            {
                foreach (Bomb bomb in player.bombs) //Bombs
                    e.Graphics.DrawImage(bitmaps["bomb"], bomb.rec);

                foreach (Explosion ex in player.explosions) //ExplosionBits
                    for (int r = 0; r < ex.recs.Count; r++)
                        e.Graphics.DrawImage(bitmaps[ex.recType[r]], ex.recs[r]);
            }

            foreach (Player player in p) //Players
            {
                e.Graphics.DrawImage(player.walkCycles[player.action][player.walkFrame], player.rec);
                if(player.invincibleTicks != -1)
                    e.Graphics.DrawImage(bitmaps["saiyanHair"], player.rec.X + 11, player.rec.Y, 25, 25);
            }
        }

        private void LayBomb(Player player)
        {
            if (player.bombs.Count < player.bombLimit)
            {
                foreach (Bomb bomb in player.bombs) //Check if tile is free
                    if (bomb.rec.Contains((int)Math.Round((float)player.rec.X / ts) * ts, (int)Math.Round((float)player.rec.Y / ts) * ts)) //Is point of new bomb within any old bomb?
                        return; //then dont place bomb

                player.bombs.Add(new Bomb(new Rectangle((int)Math.Round((float)player.rec.X / ts) * ts, (int)Math.Round((float)player.rec.Y / ts) * ts, ts, ts)));
                wallData[player.bombs.Last().rec.Y/ts, player.bombs.Last().rec.X/ts] = 4; //For Explode() collision check
            }
        }
        
        private void Explode(Player player, Rectangle bomb)
        {
            Explosion ex = new Explosion(); //Add an explosion instance
            player.explosions.Add(ex);

            ex.recs.Add(new Rectangle(bomb.X, bomb.Y, ts, ts)); //Add center of explosion
            ex.recType.Add("explosionCenter"); //Set type for that explosion rectangle

            Point bombTile = new Point(bomb.X / ts, bomb.Y / ts); //bombTile from 0 to 9
            Point[] dir = new Point[] { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0),  }; //Implement explosion into all 4 directions
            for (int d = 0; d < dir.Length; d++)
            {
                //Move away as far as possible from center, until wall is hit (break;), or bomb range/game bounds are hit (while condition becomes false)
                int y = bombTile.Y + dir[d].Y;
                int x = bombTile.X + dir[d].X;
                while (y >= 0 && y < wallData.GetLength(0) && y >= bombTile.Y - player.bombRadius && y <= bombTile.Y + player.bombRadius &&
                       x >= 0 && x < wallData.GetLength(1) && x >= bombTile.X - player.bombRadius && x <= bombTile.X + player.bombRadius)
                {
                    bool breakDirection = false;
                    if (wallData[y, x] == 0) //no wall
                    {
                        ex.recs.Add(new Rectangle(x * ts, y * ts, ts, ts)); //Add middle explosion
                        ex.recType.Add("explosionMiddle" + d); //middle
                    }
                    else if (wallData[y, x] == 1) //non-breakable wall, so break the loop
                    {
                        if(player.superBombTicks != -1) //Add explosion tile anyway
                        {
                            ex.recs.Add(new Rectangle(x * ts, y * ts, ts, ts)); //Add middle explosion
                            ex.recType.Add("explosionMiddle" + d); //middle
                        }
                        breakDirection = true;
                    }
                    else if (wallData[y, x] == 2) //breakable wall
                    {
                        //Find which wall belongs to this coordinate
                        for (int w = 0; 0 < walls.Count; w++)
                            if (walls[w].X == x * ts && walls[w].Y == y * ts)
                            {
                                walls.RemoveAt(w); //Remove that wall rectangle
                                wallType.RemoveAt(w); //Remove that wall rectangle
                                wallData[y, x] = 0;
                                breakDirection = true;
                                break;
                            }

                        ex.recs.Add(new Rectangle(x * ts, y * ts, ts, ts)); //Add middle explosion
                        ex.recType.Add("explosionMiddle" + d); //middle

                        //make new item
                        int newItemNumber = random.Next((int)(itemPics.Count*1.5));
                        if (newItemNumber < itemPics.Count)
                        {
                            itemType.Add(itemPics.Keys.ElementAt(newItemNumber));
                            int factor = 4; //item size decrease factor
                            itemRecs.Add(new Rectangle(x *ts + ts/factor, y* ts + ts/factor, ts - ts/factor*2, ts - ts/factor*2));
                            wallData[y, x] = 3;
                        }
                    }
                    else if (wallData[y, x] == 3) //item
                    {
                        //Find which item belongs to this coordinate
                        for (int i = 0; i < itemRecs.Count; i++)
                            if (itemRecs[i].X/ts == x && itemRecs[i].Y/ts == y)
                            {
                                itemRecs.RemoveAt(i); //Remove that item rectangle
                                itemType.RemoveAt(i); //Remove that item type
                                wallData[y, x] = 0; //Remove 3 from wallData grid
                                break;
                            }

                        ex.recs.Add(new Rectangle(x * ts, y * ts, ts, ts)); //Add middle explosion
                        ex.recType.Add("explosionMiddle" + d); //middle
                    }
                    else if (wallData[y, x] == 4) //bomb chain reaction
                    {
                        //Find which bomb belongs to this coordinate
                        foreach(Player pl in p)
                            for (int b = 0; b < pl.bombs.Count; b++)
                                if (pl.bombs[b].rec.X == x *ts && pl.bombs[b].rec.Y == y *ts)
                                {
                                    pl.bombs[b].tick = (int)(1.5 * framerate) - 5; //Remove that item rectangle
                                    wallData[y, x] = 0; //Remove 4 from wallData grid
                                    breakDirection = true;
                                    break;
                                }

                        ex.recs.Add(new Rectangle(x * ts, y * ts, ts, ts)); //Add middle explosion
                        ex.recType.Add("explosionMiddle" + d); //middle
                    }

                    if (breakDirection && player.superBombTicks == -1)
                        break;

                    y += dir[d].Y;
                    x += dir[d].X;
                }
                if (ex.recType.Last() != "explosionCenter") //Change last tile into an end tile, unless it was the center
                    ex.recType[ex.recType.Count - 1] = ex.recType.Last().Replace("explosionMiddle", "explosionEnd"); //middle tile to end tile, keep original direction
            }
        }

        private void Collision(Player player)
        {
            //Check player - gamearea bounds collision
            if (player.rec.Top < 0 || player.rec.Bottom > gameBox.Height || player.rec.Left < 0 || player.rec.Right > gameBox.Width)
                player.rec.Location = player.lastPos;
            //Check player - walls collision
            int w = 0; //track number of wall collisions
            Rectangle colRec = new Rectangle();
            for (int i = 0; i < walls.Count; i++)
            {
                if (player.rec.IntersectsWith(walls[i]) && (player.blockPassTicks == -1 || wallType[i] == 1)) //Also check if player can move through breakable walls
                {
                    w++;
                    colRec = walls[i];
                }
            }
            if (w > 0)
                player.rec.Location = player.lastPos;
            //Translate movement into walls to perpendicular movement
            if (w == 1) //If single wall collision occured
            {
                if (player.action == "right" || player.action == "left") //Is the player moving horizontally
                {
                    if (player.rec.Y < colRec.Y)
                    {
                        player.rec.Y -= player.speed;
                        player.action = "up";
                    }
                    if (player.rec.Y > colRec.Y)
                    {
                        player.rec.Y += player.speed;
                        player.action = "down";
                    }
                }
                else //the player is moving vertically
                {
                    if (player.rec.X < colRec.X)
                    {
                        player.rec.X -= player.speed;
                        player.action = "left";
                    }
                    if (player.rec.X > colRec.X)
                    {
                        player.rec.X += player.speed;
                        player.action = "right";
                    }
                }
            }

            //Check player - other player collision
            Player otherPlayer = (player == p[0] ? p[1] : p[0]);
            //if (player.rec.IntersectsWith(otherPlayer.rec))
            //    player.rec.Location = player.lastPos;

            //Check player - playerBombs collision
            for (int i = 0; i < player.bombs.Count; i++)
            {
                if (player.rec.IntersectsWith(player.bombs[i].rec) && !player.bombs[i].passThrough)
                    player.rec.Location = player.lastPos;
            }

            //Check player - otherPlayerbombs collision
            for (int i = 0; i < otherPlayer.bombs.Count; i++)
            {
                if (player.rec.IntersectsWith(otherPlayer.bombs[i].rec))
                    player.rec.Location = player.lastPos;
            }

            //Check player - item collision
            for (int i = 0; i < itemRecs.Count; i++)
            {
                if (player.rec.IntersectsWith(itemRecs[i]))
                {
                    ApplyItemEffect(itemType[i], player);

                    wallData[itemRecs[i].Y /ts, itemRecs[i].X /ts] = 0; //Remove 3 for item from wallData grid
                    itemRecs.RemoveAt(i);
                    itemType.RemoveAt(i);
                    break;
                }
            }
        }

        private void ApplyItemEffect(string itemName, Player player)
        {
            switch (itemName)
            {
            case "accelerator":
                    player.speed++;
                    break;
            case "blockPasser":
                    player.blockPassTicks = 0;
                    break;
            case "bomberman":
                    player.SetHp(player.hp + 1);
                    break;
            case "bombPasser":
                    break;
            case "boxingGlove":
                    break;
            case "explosionExpander":
                    player.bombRadius++;
                    break;
            case "extraBomb":
                    player.bombLimit++;
                    if (player.bombLimit > Player.maxBombLimit)
                        player.bombLimit = Player.maxBombLimit;
                    break;
            case "fireExtinguisher":
                    player.SetHp(1000);
                    break;
            case "heart":
                    player.SetHp(player.hp *= 2);
                    break;
            case "indestructibleArmor":
                    player.invincibleTicks = 0;
                    break;
            case "kick":
                    break;
            case "maximumExplosion":
                    player.bombRadius = Player.maxBombRadius;
                    break;
            case "mysteryItem": //call this function again but with a random itemNumber
                    int newItemNumber = random.Next(itemPics.Count);
                    ApplyItemEffect(itemPics.Keys.ElementAt(newItemNumber), player);
                    break;
            case "remoteControl":
                    break;
            case "skull":
                    break;
            case "superBomb":
                    player.superBombTicks = 0;
                    break;
            case "time":
                    break;
            }
            Debug.WriteLine(itemName);
        }

        //private void BitmapConverter()
        //{
        //    Bitmap bit = new Bitmap("images/map.png");
        //    for (int y = 0; y < bit.Height; y++)
        //    {
        //        for (int x = 0; x < bit.Width; x++)
        //        {
        //            switch (bit.GetPixel(x, y).Name.Substring(2))
        //            {
        //                case "ffffff":
        //                    Debug.Write("0,");
        //                    wallData[y, x] = 0;
        //                    break;
        //                case "ff0000":
        //                    Debug.Write("2,");
        //                    wallData[y, x] = 2;
        //                    break;
        //                case "000000":
        //                    Debug.Write("1,");
        //                    wallData[y, x] = 1;
        //                    break;
        //            }
        //        }
        //        Debug.WriteLine("");
        //    }
        //}
    }
}