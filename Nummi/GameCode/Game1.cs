using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Code_For_Nummi;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Nummi;

namespace Nummi
{
    public class Game1 : Game
    {

        public Texture2D _defaultTxr;

        public Rectangle _screenBounds;
        public Rectangle _roomBounds;

        public GameState _gameState;
        public int _currentLevel;
        public int _prepForNextLevel = -1;
        protected float _stateTimer;
        protected bool _coinLvl = false;
        public bool _coinSide = true; // true for heads and false for tails
        public float _health = 100f;
        float _aggrorange = 400f;
        public bool canSeePlayer = false;

        public bool _bossDead = true;

        // Tails Variables

        GridSystem grid;
        GridRenderer gridRenderer;
        BuildingSystem buildingSystem;

        Texture2D houseTexture;
        Texture2D factoryTexture;

        public SpriteFont font;

        public float _scaleText = 1.0f;

        public SpritePlayer _player;
        TextButton playButton;
        public Background _levelBackground;
        public SpriteNPC _npc;
        public DialogBox _box;
        public Camera2D _tailsCamera;

        public TilemapGroup _tilemap;

        public readonly string[] levelFiles =
        {
            "Maps/HeadsTown.xml",
            "Maps/TailsTown1.xml",
            "Maps/Dungeon1-Section1.xml",
            "Maps/Dungeon1-Section2.xml"
        };

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

            GBL._camera = new FollowCamera(GBL.GDM.GraphicsDevice.Viewport);

            _tailsCamera = new Camera2D(GraphicsDevice.Viewport, 74, 64, 32);
        }

        protected override void Initialize()
        {

            GBL.Content = Content;

            font = Content.Load<SpriteFont>("MyFont");

            playButton = new TextButton(font, "Play Game", new Vector2(300, 200));

            _screenBounds = GBL.GD.PresentationParameters.Bounds;

            grid = new GridSystem(64, 64, 32);

            grid.Origin = new Vector2(1, 1);   

            base.Initialize();

            var map = _tilemap.Layers[0];

            _roomBounds = new Rectangle(
                (int)map.TileWidth,
                (int)map.TileHeight,
                _screenBounds.Width - (int)map.TileWidth * 2,
                _screenBounds.Width - (int)map.TileWidth * 2
                );
        }

        protected override void LoadContent()
        {
            GBL.spriteBatch = new SpriteBatch(GraphicsDevice);

            _tilemap = Tilemap.FromFile(levelFiles[0]);

            _tilemap = Tilemap.FromFile(levelFiles[1]);

            gridRenderer = new GridRenderer(GraphicsDevice, grid);
            buildingSystem = new BuildingSystem(grid);

            houseTexture = Content.Load<Texture2D>("Textures\\Houses\\House1");
            factoryTexture = Content.Load<Texture2D>("Textures\\SpecialBuildings\\Barracks");

            buildingSystem.hotkeys[Keys.D1] =
                new BuildingType("House", houseTexture, new Point(1, 1));

            buildingSystem.hotkeys[Keys.D2] =
                new BuildingType("Factory", factoryTexture, new Point(2, 2));
        }

        protected override void Update(GameTime gameTime)
        {

            GBL.Update(gameTime, this);

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

            Vector2 playerCentre = new Vector2(_player._collisionBounds.X + _player._collisionBounds.Width / 2f,
                    _player._collisionBounds.Y + _player._collisionBounds.Height / 2f);

            if (GBL.KeyPress(Keys.Escape))
            {
                SetPaused(true);
            }
            foreach (Sprite eachSprite in _spriteList)
            {
                if (eachSprite is SpriteEnemy enemy)
                {
                    Vector2 enemyCentre = new Vector2(enemy._collisionBounds.X + enemy._collisionBounds.Width / 2,
                    enemy._collisionBounds.Y + enemy._collisionBounds.Height / 2);

                    //Adds an aggro range to enemies
                    //This checks the distance from the player to the enemy
                    //and if they are in range and not behind a solid block they can move
                    float distance = Vector2.Distance(playerCentre, enemyCentre);

                    if(enemy._canPatrol) enemy.Update(gameTime);

                    if (distance <= _aggrorange)
                    {
                        // One of these two can see player variables is used in enemy for animation so it doesnt just stop updating and is stuck on a weird frame

                        canSeePlayer = CanSeePlayer(enemyCentre, playerCentre);

                        bool _playerSeen = CanSeePlayer(enemyCentre, playerCentre);

                        if (_playerSeen)
                        {
                            if(!enemy._canPatrol) enemy.Update(gameTime);
                            enemy._isPatrolling = false;
                            enemy._lastSeenTimer = 2f;
                        }
                        else if (enemy._lastSeenTimer > 0f)
                        {
                            if (!enemy._canPatrol) enemy.Update(gameTime);
                            enemy._isPatrolling = false;
                            enemy._lastSeenTimer -= GBL.DeltaTime;
                        }
                    }
                    continue;
                }
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

            if (_currentLevel == 0
                || _currentLevel == 1
                || _currentLevel == 2) _coinLvl = true;

            if (!_spriteList.OfType<SpritePlayer>().Any())
            {
                PlayerDied();
            }

            if (_tilemap.IsExitAtWorld((int)_player._position.X, (int)_player._position.Y) && _coinLvl)
            {
                NextLevel();
            }

            Point chestTile;

            if (_tilemap.TryGetChestTileAtWorld((int)_player._position.X, (int)_player._position.Y, out chestTile))
            {
                _player.ChestOpened(_player._position);

                var map = _tilemap.Layers[1];

                map.SetTile(chestTile.X, chestTile.Y, 0);
            }

            if (GBL.KeyPress(Keys.Tab))
            {
                StartTailsLevel(0);
            }

            if(GBL.KeyPress(Keys.G))
            {
                _bossDead = true;
            }
        }

        public void UpdateTailsLevel(GameTime gameTime)
        {
            if (_prepForNextLevel >= 0)
            {
                StartTailsLevel(_prepForNextLevel);
                _prepForNextLevel = -1;
            }

            _tailsCamera.Update(gameTime);

            if (GBL.KeyPress(Keys.LeftShift))
            {
                StartHeadsLevel(0);
            }

            foreach (Sprite eachSprite in _spriteList)
            {
                eachSprite.Update(gameTime);
                if (_prepForNextLevel >= 0)
                {
                    break;
                }
            }

            var mouse = Mouse.GetState();

            Vector2 mouseWorld = _tailsCamera.ScreenToWorld(mouse.Position.ToVector2());

            buildingSystem.Update(mouseWorld);

            // Show grid only in build mode
            gridRenderer.Visible = buildingSystem.IsBuildMode;
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
            _coinSide = true;
            _currentLevel = level;
            // clears old sprites
            _spriteList.Clear();
            _newSpriteList.Clear();
            // spawns the sprites that appear at start of game
            LevelData.SpawnLevel(_currentLevel, this);
            Vector2 playerCentre = new Vector2(_player._collisionBounds.X + _player._collisionBounds.Width / 2f,
                    _player._collisionBounds.Y + _player._collisionBounds.Height / 2f);
        }
        // Start Tails level
        public void StartTailsLevel(int level)
        {

            _gameState = GameState.TailsLevel;
            _coinSide = false;
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
            GBL.GD.Clear(Color.DarkGreen);

            GBL.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, transformMatrix: GBL._camera._transform);
            

            switch(_gameState)
            {
                case GameState.HeadsLevel: DrawHeadsLevel();
                    foreach (Sprite eachSprite in _spriteList) eachSprite.Draw(GBL.spriteBatch);
                    break;
            }
            GBL.spriteBatch.End();

            GBL.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, transformMatrix: _tailsCamera.Transform);

            switch (_gameState)
            {
                case GameState.TailsLevel:
                    DrawTailsLevel();
                    foreach (Sprite eachSprite in _spriteList) eachSprite.Draw(GBL.spriteBatch);
                    break;
            }
            GBL.spriteBatch.End();

            GBL.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            // controls what to draw on each state
            switch (_gameState)
            {
                case GameState.Title: DrawTitle(); break;
                case GameState.MainMenu: DrawMainMenu(); break;
                
            }

            // draws each sprite from spritelist

            

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
            GBL.spriteBatch.Draw(Content.Load<Texture2D>("Textures\\Backgrounds\\MainMenuBackgroundPlaceHolder"),
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
            _tilemap.Draw();
        }
        public void DrawTailsLevel()
        {
            _tilemap.Draw();

            Vector2 mouseWorld = _tailsCamera.ScreenToWorld(Mouse.GetState().Position.ToVector2());

            buildingSystem.Draw(mouseWorld);
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

        public bool CanSeePlayer(Vector2 enemyCentre, Vector2 playerCentre)
        {
            Vector2 arrow = playerCentre - enemyCentre;

            float distance = arrow.Length();

            if (distance < 0.001f)
            {
                return true;
            }

            Vector2 direction = arrow / distance;

            float stepSize = 4f;

            for (float travelled = 0f; travelled <= distance; travelled += stepSize)
            {
                Vector2 samplePoint = enemyCentre + direction * travelled;

                if (_tilemap.IsSolidAtWorld((int)samplePoint.X, (int)samplePoint.Y))
                {
                    return false;
                }
            }
            return true;
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
