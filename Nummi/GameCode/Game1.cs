using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Code_For_Nummi;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Nummi.GameCode;
using Nummi.GameCode.Sprites;

namespace Nummi
{
    public class Game1 : Game
    {

        public Texture2D _defaultTxr;

        public Rectangle _screenBounds;

        public GameState _gameState;
        public int _currentLevel;
        public int _prepForNextLevel = -1;
        protected float _stateTimer;
        protected bool _coinLvl = false;
        public bool _coinSide = true; // true for heads and false for tails
        public float _health = 100f;

        SpriteFont font;

        public float _scaleText = 1.0f;

        public SpritePlayer _player;
        TextButton playButton;
        public Background _levelBackground;
        private FollowCamera _camera;

        public List<Sprite> _spriteList = new List<Sprite>();
        public List<Sprite> _newSpriteList = new List<Sprite>();

        public Game1()
        {
            GBL.GDM = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            GBL.GDM.PreferredBackBufferWidth = 800;
            GBL.GDM.PreferredBackBufferHeight =  480;
            GBL.GDM.ApplyChanges();

            _camera = new FollowCamera(GBL.GDM.GraphicsDevice.Viewport);
        }

        protected override void Initialize()
        {
            GBL.Content = Content;

            SpriteFont font = Content.Load<SpriteFont>("MyFont");

            playButton = new TextButton(font, "Play Game", new Vector2(300, 200));

            base.Initialize();

            _screenBounds = GBL.GD.PresentationParameters.Bounds;
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
                case GameState.Title: UpdateTitle(gameTime); break;
                case GameState.MainMenu: UpdateMainMenu(gameTime); break;
                case GameState.HeadsLevel: UpdateHeadsLevel(gameTime); break;
                case GameState.TailsLevel: UpdateTailsLevel(gameTime); break;
                case GameState.Settings: UpdateSettings(gameTime); break;
                case GameState.Guide: UpdateGuide(gameTime); break;
                case GameState.Paused: UpdatePaused(gameTime); break;
                case GameState.DeathScreen: UpdateDeathScreen(gameTime); break;
                default: StartTitle(); break;
            }



            base.Update(gameTime);
        }

        public void UpdateTitle(GameTime gameTime)
        {
            if (GBL.KeyPress(Keys.Enter)) StartMainMenu();
        }

        public void UpdateMainMenu(GameTime gameTime)
        {
            playButton.Update();

            if(playButton.IsClicked)
            {
                StartNewGame();
            }
        }

        public void UpdateHeadsLevel(GameTime gameTime)
        {
            if(GBL.KeyPress(Keys.Escape))
            {
                SetPaused(true);
            }
            foreach (Sprite eachSprite in _spriteList)
            {
                eachSprite.Update(gameTime);
                if (_prepForNextLevel >= 0)
                {
                    break;
                }
            }

            _spriteList.AddRange(_newSpriteList);
            _newSpriteList.Clear();
            _spriteList.RemoveAll(deadSprite => deadSprite.Dead);
            if (_prepForNextLevel >= 0)
            {
                StartHeadsLevel(_prepForNextLevel);
                _prepForNextLevel = -1;
            }

            if (_currentLevel == 0) _coinLvl = true;

            if (!_spriteList.OfType<SpritePlayer>().Any())
            {
                PlayerDied();
            }

            Vector2 playerCentre = new Vector2(_player._collisionBounds.X + _player._collisionBounds.Width / 2f,
                    _player._collisionBounds.Y + _player._collisionBounds.Height / 2f);

            _camera.Follow(playerCentre);
            _camera.Update();
        }

        public void UpdateTailsLevel(GameTime gameTime)
        {

        }

        public void UpdateSettings(GameTime gameTime)
        {

        }

        public void UpdateGuide(GameTime gameTime)
        {

        }

        public void UpdatePaused(GameTime gameTime)
        {

        }

        public void UpdateDeathScreen(GameTime gameTime)
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
            Vector2 playerCentre = new Vector2(_player._collisionBounds.X + _player._collisionBounds.Width / 2f,
                    _player._collisionBounds.Y + _player._collisionBounds.Height / 2f);
            _camera.Follow(playerCentre);
            _camera.Update();
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
            GBL.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, transformMatrix: _camera._transform);

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
            GBL.spriteBatch.Draw(Content.Load<Texture2D>("MainMenuBackgroundPlaceHolder"),
                new Rectangle(0, 0, _screenBounds.Width, _screenBounds.Height),
                null,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.04f);
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

        public void StartNewGame()
        {
            _player = null;

            _health = 100;
            StartHeadsLevel(0);

            Vector2 playerCentre = new Vector2(_player._collisionBounds.X + _player._collisionBounds.Width / 2f,
                    _player._collisionBounds.Y + _player._collisionBounds.Height / 2f);

            _spriteList.Clear();
            _newSpriteList.Clear();

            _camera.Follow(playerCentre);
            _camera.Update();
        }

        public void PlayerDied()
        {
            _health--;

            if(_health <= 0)
            {
                StartDeathScreen();
            }
            else
            {
                StartHeadsLevel(_currentLevel);
            }
        }

        public void NextLevel()
        {
            if (_currentLevel >= LevelData.LastLevelIndex)
            {
                StartTitle();
            }
            // otherwise, start the next level.
            else
            {
                _currentLevel++;
                StartHeadsLevel(_currentLevel);
            }
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
