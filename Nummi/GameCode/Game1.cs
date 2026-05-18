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
        // Used for is visible to make sprite invisible
        public Texture2D _defaultTxr;

        public Rectangle _screenBounds;
        public Rectangle _roomBounds;

        // Heads Variables

        public GameState _gameState;
        public int _currentLevel;
        public int _prepForNextLevel = -1;
        protected float _stateTimer;
        protected bool _coinLvl = false;
        public bool _coinSide = true; // true for heads and false for tails
        public float _health;
        public bool canSeePlayer = false;
        private float _spawnProtectionTimer = 0f;
        private const float SpawnProtectionDuration = 0.1f;
        public bool _showTailsIntro = false;

        public float _trapRoomDoorTimer = 0f;
        public float _trapDoorTimer = 1f;
        public bool _justGoneOverTrapDoor = false;
        public bool _alreadyGoneIntoTrapDoor = false;
        public Point _trapDoorTile;
        public bool _isTrapLevel = false;
        public Tilemap map => _tilemap.Layers[0];

        public bool _bossDead = true;
        public SpriteEnemy _currentBoss;

        // Tails Variables

        public GridSystem _grid;
        GridRenderer gridRenderer;
        public BuildingSystem buildingSystem;

        // Lighting Variables

        public Vector2[] _torchPositions = Array.Empty<Vector2>();
        public LightingRenderer _lighting;
        public bool _useLighting = false;

        Texture2D _barracksTexture, _nuclearReactorTxr, _houseTexture;
        Texture2D _shopButtonTexture;

        public SpriteFont font;

        public float _scaleText = 1.0f;

        // Class Variables

        public SpritePlayer _player;
        TextButton playButton;
        TextButton shopButton;
        Background _MenuBackground;
        public SpriteNPC _npc;
        public DialogBox _box;
        public Camera2D _tailsCamera;
        public HUD _hud;
        public CurrencySystem _currency;
        public Shop _shop;
        public ShopUI _shopUI;

        public TilemapGroup _tilemap;

        // Xml files for tilemaps
        public readonly string[] levelFiles =
        {
            "Maps/HeadsTown.xml",
            "Maps/TailsTown1.xml",
            "Maps/Dungeon1-Section1.xml",
            "Maps/Dungeon1-Section2.xml",
            "Maps/Dungeon1-BossRoom.xml",
            "Maps/Dungeon2-Section1.xml",
        };

        // Sprite lists
        public List<Sprite> _spriteList = new List<Sprite>();
        public List<Sprite> _newSpriteList = new List<Sprite>();

        public Game1()
        {
            GBL.GDM = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Sets the resolution to 800x480 and applies the changes. This is a common resolution for 16:9 aspect ratio games, and it ensures that the game will have a consistent window size when it starts.
            GBL.GDM.PreferredBackBufferWidth = 800;
            GBL.GDM.PreferredBackBufferHeight =  480;
            GBL.GDM.ApplyChanges();

            // Initializes the cameras
            GBL._camera = new FollowCamera(GBL.GDM.GraphicsDevice.Viewport);

            _tailsCamera = new Camera2D(GraphicsDevice.Viewport, 74, 64, 32);
        }

        protected override void Initialize()
        {

            GBL.Content = Content;

            font = Content.Load<SpriteFont>("MyFont");

            _tilemap = Tilemap.FromFile(levelFiles[0]);

            // for all buttons in the menus initialize them here and then only update and draw in the respective states
            playButton = new TextButton(font, "Play Game", new Vector2(300, 200));

            shopButton = new TextButton(font, "Shop", new Vector2(740, 450));
            _screenBounds = GBL.GD.PresentationParameters.Bounds;

            _MenuBackground = new Background(this, Content.Load<Texture2D>("Textures\\Backgrounds\\Main Menu"), 1);

            // makes the grid for building
            _grid = new GridSystem(64, 64, 32);
            _grid.Origin = new Vector2(1, 1);

            base.Initialize();

            var map = _tilemap.Layers[0];
            // The room bounds are the area in which the player can move, which is the screen bounds minus the size of the tiles on each side. This is because the tiles on the edges are solid and the player cannot move into them.
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

            // for loading the grid and building system(the grid loading isnt really necessary but it is used for the grid renderer and building system so its here)
            gridRenderer = new GridRenderer(GraphicsDevice, _grid);
            buildingSystem = new BuildingSystem(_grid);

            _shopButtonTexture = Content.Load<Texture2D>("Textures\\UI\\Dialog Box");

            // for loading the UI and shop system and lighting system
            _currency = new CurrencySystem(startingCoins: 200);
            _shop = new Shop(_currency, buildingSystem);
            _shopUI = new ShopUI(_shop, _currency, font);

            _hud = new HUD(this, font);

            _lighting = new LightingRenderer();
        }

        protected override void Update(GameTime gameTime)
        {

            GBL.Update(gameTime, this);

            // For separating updates into scenes

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
            _MenuBackground.Update(gameTime);
        }

        public void UpdateMainMenu(GameTime gameTime)
        {
            playButton.Update();

            if(playButton.IsClicked)
            {
                StartNewGame();
            }

            _MenuBackground.Update(gameTime);
        }

        public void UpdateHeadsLevel(GameTime gameTime)
        {
            // Adds flickering to the torches
            if (_useLighting)
                _lighting.TorchLightRadius = 80 + (int)(Math.Sin(gameTime.TotalGameTime.TotalSeconds * 10) * 5);

            Vector2 playerCentre = new Vector2(_player._collisionBounds.X + _player._collisionBounds.Width / 2f,
                    _player._collisionBounds.Y + _player._collisionBounds.Height / 2f);

            // Pausing game
            if (GBL.KeyPress(Keys.Escape))
            {
                SetPaused(true);
            }
            // Goes through every sprite in sprite list
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
                    if (distance <= enemy._aggrorange)
                    {
                        // One of these two can see player variables is used in enemy for animation so it doesnt just stop updating and is stuck on a weird frame

                        canSeePlayer = CanSeePlayer(enemyCentre, playerCentre);

                        bool _playerSeen = CanSeePlayer(enemyCentre, playerCentre);

                        // Handles the patrolling of enemies if they can patrol
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
                // updates every sprite in spritelist
                eachSprite.Update(gameTime);
                if (_prepForNextLevel >= 0)
                {
                    break;
                }
            }

            // adds new sprites to the sprite list and removes dead sprites from the sprite list
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

            if (_spawnProtectionTimer > 0f)
                _spawnProtectionTimer -= GBL.DeltaTime;
            // Player spawn protection, if the timer is above 0 the player cannot die and it will only start counting down after the player spawns in so they dont die immediately from something they cant see
            if (_spawnProtectionTimer <= 0f && !_spriteList.OfType<SpritePlayer>().Any())
            {
                PlayerDied();
            }
            // used for going to next level
            if (_tilemap.IsExitAtWorld((int)_player._position.X, (int)_player._position.Y) && _coinLvl)
            {
                NextLevel();
            }
            // used to check if the player just went into a trap room
            // TODO this only can handle one trap room per level right now and is a bit janky so maybe fix that
            if(_tilemap.TryGetTrapDoorTileAtWorld((int)_player._position.X, (int)_player._position.Y, out Point trapDoorTile))
            {
                _justGoneOverTrapDoor = true;
                _trapDoorTile = trapDoorTile;
            }
            if(_justGoneOverTrapDoor && _isTrapLevel)
            {
                _trapRoomDoorTimer += GBL.DeltaTime;
                if(_trapRoomDoorTimer >= _trapDoorTimer)
                {
                    _justGoneOverTrapDoor = false;
                    _trapRoomDoorTimer = 0f;

                    if(!_alreadyGoneIntoTrapDoor)
                    {
                        map.SetTile(_trapDoorTile.X, _trapDoorTile.Y, 17);
                        _alreadyGoneIntoTrapDoor = true;
                    }
                    
                }
            }
            // when getting the chest in the trap room it opens the chest and then resets the trap door so you can go back out
            if (_tilemap.TryGetChestTileAtWorld((int)_player._position.X, (int)_player._position.Y, out Point chestTile))
            {
                _player.ChestOpened(_player._position);

                var map1 = _tilemap.Layers[1];

                map1.SetTile(chestTile.X, chestTile.Y, 0);

                if(_alreadyGoneIntoTrapDoor) map.SetTile(_trapDoorTile.X, _trapDoorTile.Y, 3);
            }
            // for testing purposes to skip to tails level
            if (GBL.KeyPress(Keys.Tab))
            {
                StartTailsLevel(0);
            }
            // for testing boss fight
            if (GBL.KeyPress(Keys.G))
            {
                _bossDead = true;
            }
        }

        public void UpdateTailsLevel(GameTime gameTime)
        {
            // plays dialog at start of tails level and stops player from moving until its done
            if (_showTailsIntro && _box != null)
            {
                _box.Update(gameTime);
                if (_box.Dead)
                {
                    _showTailsIntro = false;
                    _box = null;
                }
                return; 
            }

            if (_prepForNextLevel >= 0)
            {
                StartTailsLevel(_prepForNextLevel);
                _prepForNextLevel = -1;
            }
            // updates the camera 
            _tailsCamera.Update(gameTime);
            // for testing purposes to skip to heads level
            if (GBL.KeyPress(Keys.LeftControl))
            {
                StartHeadsLevel(0);
            }
            // updates all sprites inside the spritelist not really useful as the tails level doesnt use sprite for the houses
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
            // building system update which handles the building of structures and the preview of where they will be built and if they can be built there or not
            buildingSystem.Update(mouseWorld, _shop);

            // Show grid only in build mode
            gridRenderer.Visible = buildingSystem.IsBuildMode;
            // shop updates
            shopButton.Update();
            if (shopButton.IsClicked)
            {
                _shop.Toggle();
            }
            _shopUI.Update();
        }
        // TODO make the other menus
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
        // used to start title and clear sprites for when going back to title from main menu or from beating the game or whatever else might send you back to the title
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
        public void PrepNextLevel()
        {
            // used to reset values between levels

            _alreadyGoneIntoTrapDoor = false;
            _justGoneOverTrapDoor = false;
            _trapRoomDoorTimer = 0f;
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
            _spawnProtectionTimer = SpawnProtectionDuration;
            // spawns the sprites that appear at start of game
            LevelData.SpawnLevel(_currentLevel, this);
            Vector2 playerCentre = new Vector2(_player._collisionBounds.X + _player._collisionBounds.Width / 2f,
                    _player._collisionBounds.Y + _player._collisionBounds.Height / 2f);

            _health = _player.Stats.MaxHP;
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

            _currency.AddCoins(_currency.Population * 50);

            _showTailsIntro = true;
            _box = new DialogBox(this, "Huh What the where did this gold just appear from...", "welp who cares its mine now hehehhe");
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
            // applies lighting if its enabled and we are in the heads level since tails level is bright and cheery and doesnt need lighting and it also just looks weird with the lighting system since it was designed for the darker heads levels
            if (_useLighting && _gameState == GameState.HeadsLevel)
            {
                _lighting.DrawLighting(
                    _player._position + new Vector2(-10, -10), 
                    _torchPositions,
                    GBL._camera._transform
                );
            }

            GBL.GD.Clear(Color.DarkGreen);

            // this is very messy but its to allow stuff to be drawn without being affected by the cameras and then stuff that needs to be drawn with the cameras and then stuff that needs to be drawn with the tails camera in the tails level since it has a different camera from the heads level and then the UI on top of everything else

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

            if (_useLighting && _gameState == GameState.HeadsLevel)
                _lighting.ApplyLighting();

            // controls what to draw on each state
            switch (_gameState)
            {
                case GameState.Title: DrawTitle(); break;
                case GameState.MainMenu: DrawMainMenu(); break;
                case GameState.Paused: DrawPaused(); break;
                case GameState.DeathScreen: DrawDeathScreen(); break;
                case GameState.HeadsLevel:
                    _hud.Draw(_player);
                    foreach (Sprite s in _spriteList)
                        if (s is DialogBox db) db.Draw(GBL.spriteBatch);
                    break;
                case GameState.TailsLevel:
                    _shopUI.DrawCurrency(10, 10, _currency.Coins, GBL.Content.Load<Texture2D>("Textures\\UI\\Coin Icon"), "g");
                    _shopUI.DrawCurrency(10, 50, _currency.Population, GBL.Content.Load<Texture2D>("Textures\\UI\\Population Icon"), "p");
                    _shopUI.DrawCurrency(10, 90, _currency.Food, GBL.Content.Load<Texture2D>("Textures\\UI\\Food Icon"), "f");
                    _shopUI.DrawCurrency(10, 130, _currency.Energy, GBL.Content.Load<Texture2D>("Textures\\UI\\Energy Icon"), "e");
                    _shopUI.Draw();
                    shopButton.Draw();
                    GBL.spriteBatch.Draw(_shopButtonTexture, new Rectangle(691, 434, 96, 32), _shopButtonTexture.Bounds, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.09f);
                    if (_showTailsIntro && _box != null)
                        _box.Draw(GBL.spriteBatch);
                    break;
                case GameState.Guide: DrawGuide(); break;
                case GameState.Settings: DrawSettings(); break;
            }
            
            GBL.spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawTitle()
        {
            _MenuBackground.Draw(GBL.spriteBatch);

        }
        public void DrawMainMenu()
        {
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
        // start new game
        public void StartNewGame()
        {
            _player = null;

            StartHeadsLevel(0);

            Vector2 playerCentre = new Vector2(_player._collisionBounds.X + _player._collisionBounds.Width / 2f,
                    _player._collisionBounds.Y + _player._collisionBounds.Height / 2f);

            _spriteList.Clear();
            _newSpriteList.Clear();
        }
        // player dies and if they have no health left it goes to death screen otherwise it restarts the level they are on(I laid this out like with lives but idk if we will have lives)
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
        // going to next level and if you are on the last level it goes back to the title screen
        public void NextLevel()
        {
            PrepNextLevel();
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
        // checks if the enemy can see the player by sampling points along the line between the enemy and the player and checking if any of those points are solid blocks in the tilemap. If it finds a solid block, it returns false, meaning the enemy cannot see the player. If it reaches the end of the line without finding any solid blocks, it returns true, meaning the enemy can see the player.
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
    // Game states for controlling what update and draw functions to use and what to update and draw in those functions
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
