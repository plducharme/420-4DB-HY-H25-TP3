using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Media;
using System.IO;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
//using System.Windows.Media;
using System.Windows;
using System.Windows.Interop;
//using System.Windows.Media;

namespace HugoWorld
{
    public class GameState
    {
        public World World;
        public int Attack;
        public int Armour;
        public int Level;
        public int Health;
        public int Treasure;
        public int Potions;
        public bool HasBrownKey;
        public bool HasGreenKey;
        public bool HasRedKey;
        public bool GameIsWon;

        private int _experience;
        private int _nextUpgrade;
        private Sprite _experienceSprite;
        private Sprite _attackSprite;
        private Sprite _armourSprite;
        private Sprite _healthSprite;
        private Sprite _treasureSprite;
        private Sprite _potionSprite;
        private Sprite _brownKeySprite;
        private Sprite _greenKeySprite;
        private Sprite _redKeySprite;
        private Dictionary<string, Tile> _tiles = new Dictionary<string, Tile>();

        private static Font _font = new Font("Arial", 24);
        private static Brush _brush = new SolidBrush(Color.White);
        private static Random _random = new Random();



        public GameState()
        {

            //Load in all the tile definitions
            readTileDefinitions(@"GameData\tilelookups.csv");

            //Create the sprites for the UI
            int y = 50;
            _experienceSprite = new Sprite(this, 580, y, _tiles["her"].Bitmap, _tiles["her"].Rectangle, _tiles["her"].NumberOfFrames);
            _experienceSprite.ColorKey = Color.FromArgb(75, 75, 75);
            _healthSprite = new Sprite(this, 580, y += 74, _tiles["fd1"].Bitmap, _tiles["fd1"].Rectangle, _tiles["fd1"].NumberOfFrames);
            _healthSprite.ColorKey = Color.FromArgb(75, 75, 75);
            _attackSprite = new Sprite(this, 580, y += 74, _tiles["att"].Bitmap, _tiles["att"].Rectangle, _tiles["att"].NumberOfFrames);
            _attackSprite.ColorKey = Color.FromArgb(75, 75, 75);
            _armourSprite = new Sprite(this, 580, y += 74, _tiles["arm"].Bitmap, _tiles["arm"].Rectangle, _tiles["arm"].NumberOfFrames);
            _armourSprite.ColorKey = Color.FromArgb(75, 75, 75);
            _treasureSprite = new Sprite(this, 580, y += 74, _tiles["tr2"].Bitmap, _tiles["tr2"].Rectangle, _tiles["tr2"].NumberOfFrames);
            _treasureSprite.ColorKey = Color.FromArgb(75, 75, 75);
            _potionSprite = new Sprite(this, 580, y += 74, _tiles["pot"].Bitmap, _tiles["pot"].Rectangle, _tiles["pot"].NumberOfFrames);
            _potionSprite.ColorKey = Color.FromArgb(75, 75, 75);
            _brownKeySprite = new Sprite(this, 580, y += 74, _tiles["kbr"].Bitmap, _tiles["kbr"].Rectangle, _tiles["kbr"].NumberOfFrames);
            _brownKeySprite.ColorKey = Color.FromArgb(75, 75, 75);
            _greenKeySprite = new Sprite(this, 654, y, _tiles["kgr"].Bitmap, _tiles["kgr"].Rectangle, _tiles["kgr"].NumberOfFrames);
            _greenKeySprite.ColorKey = Color.FromArgb(75, 75, 75);
            _redKeySprite = new Sprite(this, 728, y, _tiles["kre"].Bitmap, _tiles["kre"].Rectangle, _tiles["kre"].NumberOfFrames);
            _redKeySprite.ColorKey = Color.FromArgb(75, 75, 75);
        }

        internal void Draw(object imgJeu)
        {
            throw new NotImplementedException();
        }

        //Experience property automatically upgrades your skill as the 'set' passes
        //the level threshold
        public int Experience
        {
            get
            {
                return _experience;
            }
            set
            {
                _experience = value;
                //If we hit the upgrade threshold then increase our abilities
                if (_experience > _nextUpgrade)
                {
                    Attack++;
                    Armour++;
                    //Each upgrade is a little harder to get
                    _nextUpgrade = _nextUpgrade + 20 * Level;
                    Level++;
                }
            }
        }

        public void Draw(System.Windows.Controls.Image img)
        {

            //Bitmap bUpdateImg = new Bitmap(Convert.ToInt32(GameArea.Width), Convert.ToInt32(GameArea.Height));
            Bitmap bUpdateImg = new Bitmap(800,600);

            using (Graphics graphics = Graphics.FromImage(bUpdateImg))
            {
                World.Draw(graphics);

                //Draw the HUD
                _experienceSprite.Draw(graphics);
                _healthSprite.Draw(graphics);
                _attackSprite.Draw(graphics);
                _armourSprite.Draw(graphics);
                _treasureSprite.Draw(graphics);
                _potionSprite.Draw(graphics);
                if (HasBrownKey)
                    _brownKeySprite.Draw(graphics);
                if (HasGreenKey)
                    _greenKeySprite.Draw(graphics);
                if (HasRedKey)
                    _redKeySprite.Draw(graphics);
                int y = 65;
                graphics.DrawString(Experience.ToString(), _font, _brush, 650, y);
                graphics.DrawString(Health.ToString(), _font, _brush, 650, y += 74);
                graphics.DrawString(Attack.ToString(), _font, _brush, 650, y += 74);
                graphics.DrawString(Armour.ToString(), _font, _brush, 650, y += 74);
                graphics.DrawString(Treasure.ToString(), _font, _brush, 650, y += 74);
                graphics.DrawString(Potions.ToString(), _font, _brush, 650, y += 74);

                //If the game is over then display the end game message
                if (Health == 0)
                {
                    graphics.DrawString("You died!", _font, _brush, 200, 250);
                    graphics.DrawString("Press 's' to play again", _font, _brush, 100, 300);
                }

                //If the game is won then show congratulations
                if (GameIsWon)
                {
                    graphics.DrawString("You Won!", _font, _brush, 200, 250);
                    graphics.DrawString("Press 's' to play again", _font, _brush, 100, 300);
                }
                graphics.Dispose();
            }
            using (MemoryStream memory = new MemoryStream())
            {
                bUpdateImg.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                System.Windows.Media.ImageSource imageSource = bitmapImage;
                img.Source = imageSource;
            }          
        }

        /// <summary>
        /// http://stackoverflow.com/questions/26260654/wpf-converting-bitmap-to-imagesource
        /// Allow us to Render a Bitmap into an Image, Works nice and i have not much to say against that
        /// I could change the name of the variable to act like i created this, but i prefer being honest, was needed for the wpf intergration
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    System.Windows.Media.PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }


        public void Update(double gameTime, double elapsedTime)
        {
            World.Update(gameTime, elapsedTime);
        }


        public void Initialize()
        {
            Sounds.Start();

            //Create all the main gameobjects
            World = new World(this, _tiles, @"GameData\map.txt");

            //Reset the game state
            Attack = 1;
            Potions = 10;
            Armour = 1;
            Experience = 0;
            Level = 1;
            _nextUpgrade = 20;
            Health = 100;
            Treasure = 0;
            GameIsWon = false;
        }

        //Each line contains a comma delimited tile definition that the tile constructor understands
        private void readTileDefinitions(string tileDescriptionFile)
        {
            using (StreamReader stream = new StreamReader(tileDescriptionFile))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    //separate out the elements of the 
                    string[] elements = line.Split(',');

                    //And make the tile.
                    Tile tile = new Tile(elements);
                    _tiles.Add(tile.Shortcut, tile);
                }
            }
        }


        public void KeyDown(Key key)
        {
            //If the game is not over then allow the user to play
            if (Health > 0 && !GameIsWon)
                World.KeyDown(key);
            else
            {
                //If game is over then allow S to restart 
                if (key == Key.S)
                    Initialize();
            }
        }
    }
}
