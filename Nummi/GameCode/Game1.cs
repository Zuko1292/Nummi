using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Nummi.GameCode;
using Nummi.GameCode.Sprites;
using static System.Net.Mime.MediaTypeNames;

namespace Nummi
{
    public class Game1 : Game
    {

        public Texture2D _defaultTxr;

        public Rectangle _screenBounds = new Rectangle(0, 0, 800, 480);

        public GameState _gameState;
        public int _currentLevel;
        public int _currentLives;
        public int _prepForNextLevel = -1;
        protected float _stateTimer;
        protected bool _coinLvl = false;
        public bool _coinSide = true; // true for heads and false for tails
        public float _health = 100f;

        SpriteFont font;

        public float _scaleText = 1.0f;

        public SpritePlayer _player;
        TextButton playButton;
        Background _mainMenuBackground;

        public List<Sprite> _spriteList = new List<Sprite>();
        public List<Sprite> _newSpriteList = new List<Sprite>();

        public Game1()
        {
            GBL.GDM = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            SpriteFont font = Content.Load<SpriteFont>("MyFont");

            playButton = new TextButton(font, "Play Game", new Vector2(300, 200));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            GBL.spriteBatch = new SpriteBatch(GraphicsDevice);



            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            GBL.Update(gameTime);

            switch (_gameState)
            {
                case GameState.Title: UpdateTitle(); break;
                case GameState.MainMenu: UpdateMainMenu(); break;
                case GameState.HeadsLevel: UpdateHeadsLevel(); break;
                case GameState.TailsLevel: UpdateTailsLevel(); break;
                case GameState.Settings: UpdateSettings(); break;
                case GameState.Guide: UpdateGuide(); break;
                case GameState.Paused: UpdatePaused(); break;
                case GameState.DeathScreen: UpdateDeathScreen(); break;
                default: StartTitle(); break;
            }



            base.Update(gameTime);
        }

        public void UpdateTitle()
        {
            if (GBL.KeyPress(Keys.Enter)) StartMainMenu();
        }

        public void UpdateMainMenu()
        {
            playButton.Update();

            if(playButton.IsClicked)
            {
                StartHeadsLevel(1);
            }

            if (_mainMenuBackground == null)
            {
                _mainMenuBackground = new Background(this, Content.Load<Texture2D>("MainMenuBackgroundPlaceholder"));
                _spriteList.Add(_mainMenuBackground);
            }
        }

        public void UpdateHeadsLevel()
        {
            Debug.WriteLine("Updating Heads Level");
        }

        public void UpdateTailsLevel()
        {

        }

        public void UpdateSettings()
        {

        }

        public void UpdateGuide()
        {

        }

        public void UpdatePaused()
        {

        }

        public void UpdateDeathScreen()
        {

        }

        public void StartTitle()
        {
            _gameState = GameState.Title;
            // clears all sprites
            _spriteList.Clear();
        }
        //Starts main menu
        public void StartMainMenu()
        {
            _gameState = GameState.MainMenu;
            // clears all sprites
            _spriteList.Clear();
        }
        public void PrepNextLevel(int level)
        {
            // if more levels are added this will be used to reset values between levels
            _prepForNextLevel = level;
        }
        // Starts Heads level
        public void StartHeadsLevel(int level)
        {
            _gameState = GameState.HeadsLevel;
            _currentLevel = level;
            // clears old sprites
            _spriteList.Clear();
            _newSpriteList.Clear();
            // spawns the sprites that appear at start of game
            LevelData.SpawnLevel(_currentLevel, this);
        }
        // Start Tails level
        public void StartTailsLevel(int level)
        {
            _gameState = GameState.TailsLevel;
            _currentLevel = level;
            // clears old sprites
            _spriteList.Clear();
            _newSpriteList.Clear();
            // spawns the sprites that appear at start of game
            LevelData.SpawnLevel(_currentLevel, this);
        }
        // starts pause when true and unpauses when false
        public void SetPaused(bool paused)
        {
            // stops music
            MediaPlayer.Stop();

            // controls pausing and unpausing
            if (paused && _gameState == GameState.HeadsLevel || paused && _gameState == GameState.Guide || paused && _gameState == GameState.Settings)
            {
                _gameState = GameState.Paused;
            }
        }
        // starts DeathScreen
        public void StartDeathScreen()
        {
            // stops music
            MediaPlayer.Stop();

            _gameState = GameState.DeathScreen;
            _stateTimer = 3f;
        }
        // starts guide
        public void StartGuide()
        {
            // stops music
            MediaPlayer.Stop();

            _gameState = GameState.Guide;

        }
        // Starts settings
        public void StartSettings()
        {
            // stops music
            MediaPlayer.Stop();

            _gameState = GameState.Settings;
        }

        protected override void Draw(GameTime gameTime)
        {
            // starts draw states and add layer depth
            GBL.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            // controls what to draw on each state
            switch (_gameState)
            {
                case GameState.Title: DrawTitle(); break;
                case GameState.MainMenu: DrawMainMenu(); break;
                case GameState.HeadsLevel: DrawHeadsLevel(); break;
                case GameState.TailsLevel: DrawTailsLevel(); break;
            }

            // draws each sprite from spritelist
            foreach (Sprite eachSprite in _spriteList) eachSprite.Draw(GBL.spriteBatch);

            switch (_gameState)
            {
                case GameState.Paused: DrawPaused(); break;
                case GameState.DeathScreen: DrawDeathScreen(); break;
                case GameState.Guide: DrawGuide(); break;
                case GameState.Settings: DrawSettings(); break;
            }
            
            GBL.spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawTitle()
        {

        }
        public void DrawMainMenu()
        {
            playButton.Draw();
            
        }
        public void DrawHeadsLevel()
        {

        }
        public void DrawTailsLevel()
        {

        }
        public void DrawSettings()
        {

        }
        public void DrawGuide()
        {

        }
        public void DrawPaused()
        {

        }
        public void DrawDeathScreen()
        {

        }

        


        #region ***** Utility Functions *****
        // For fancy out lined shadowed text
        public void FancyText(SpriteFont font, string text, Vector2 position, Color foreground, Color shadow, float shadowSize = 2f)
        {
            Vector2 textPos = position - (font.MeasureString(text) / 2f);
            GBL.spriteBatch.DrawString(font, text, textPos - new Vector2(shadowSize), shadow, 0f, Vector2.Zero, _scaleText, SpriteEffects.None, 0.02f);
            GBL.spriteBatch.DrawString(font, text, textPos + new Vector2(shadowSize), shadow, 0f, Vector2.Zero, _scaleText, SpriteEffects.None, 0.02f);
            GBL.spriteBatch.DrawString(font, text, textPos, foreground, 0f, Vector2.Zero, _scaleText, SpriteEffects.None, 0.01f);

            Rectangle bounds = new Rectangle(
                (int)Math.Round(position.X),
                (int)Math.Round(position.Y),
                (int)Math.Round(position.X - font.MeasureString(text).X),
                (int)Math.Round(position.Y - font.MeasureString(text).Y));
        }
        // for position of text
        public Vector2 ScreenRelative(Vector2 relative)
        {
            return _screenBounds.Size.ToVector2() * relative;
        }

        public Vector2 ScreenRelative(float relativeX, float relativeY)
        {
            return ScreenRelative(new Vector2(relativeX, relativeY));
        }
        // Clamps Vector 
        public static Vector2 ClampVec2(Vector2 vector, float min, float max)
        {
            if (vector == Vector2.Zero)
                return Vector2.Zero;

            Vector2 norm = Vector2.Normalize(vector);
            float len = vector.Length();

            if (len < min) return norm * min;
            if (len > max) return norm * max;
            return vector;
        }

        public static Vector2 ClampVec2(Vector2 vector, float max)
        {
            return ClampVec2(vector, 0, max);
        }

        #endregion ***** Utility Functions *****

    }

    public enum GameState
    {
        None,
        Title,
        MainMenu,
        HeadsLevel,
        TailsLevel,
        Settings,
        Guide,
        Paused,
        DeathScreen,

    }
}
