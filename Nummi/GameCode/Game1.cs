using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Code_For_Nummi;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        public int _headsLevel;
        public int _tailsLevel;
        public int _pendingHeadsLevel = -1;
        public int _prepForNextLevel = -1;
        protected float _stateTimer;
        protected bool _coinLvl = false;
        public bool _coinSide = true; // true for heads and false for tails
        public float _health;
        public bool canSeePlayer = false;
        private float _spawnProtectionTimer = 0f;
        private const float SpawnProtectionDuration = 0.1f;
        public bool _showTailsIntro = false;
        public bool _isNextLevelTails = false;

        // Global boss variables needed
        public bool _slimeOffHead = false;

        public float _trapRoomDoorTimer = 0f;
        public float _trapDoorTimer = 1f;
        public Point? _pendingTrapDoor = null;
        public Point _pendingTrapEntryTile;
        public Point _lastPlayerTile;
        public Stack<Point> _sealedTrapDoors = new Stack<Point>();
        public bool _isTrapLevel = false;
        public Tilemap map => _tilemap.Layers[0];

        public bool _bossDead = true;
        public bool _isBossLevel = false;
        public SpriteEnemy _currentBoss;
        public string _bossName = "";
        public int _bossesDeadNum = 0;

        // Tails Variables

        public GridSystem _grid;
        GridRenderer gridRenderer;
        public BuildingSystem buildingSystem;

        // Lighting Variables

        public Vector2[] _torchPositions = Array.Empty<Vector2>();
        public LightingRenderer _lighting;
        public bool _useLighting = false;
        Texture2D _shopButtonTexture,
            _shieldCrystalTex,
            _hayCrystalTex,
            _smithCrystalTex,
            _currentCrystalTex;

        public SpriteFont font;
        public SpriteFont _menuFont;
        public SpriteFont _titleFont;
        public SpriteFont _smallMenuFont;

        public float _scaleText = 1.0f;

        // Class Variables

        public CharacterStats savedStats;
        public LevelSystem savedLevelSystem;
        public int savedWeapon;

        public SpritePlayer _player;
        TextButton playButton, guideButton, settingsButton, exitButton, resumeButton, tailsFlipButton;
        TextButton shopButton;
        Background _MenuBackground, _GuideBackground, _SettingsBackground, _PauseBackground;
        public SpriteNPC _npc;
        public DialogBox _box;
        public Camera2D _tailsCamera;
        public HUD _hud;
        public CurrencySystem _currency;
        public Shop _shop;
        public ShopUI _shopUI;

        // Boss-drop unlocks survive shop rebuilds when changing levels.
        public List<ShopItem> _unlockedBossItems = new List<ShopItem>();

        // Keys the player has picked up from the tilemap. Each entry represents
        // one key; the count is what the Big Dealer uses for its dialogue.
        public List<int> _keys = new List<int>();

        public SoundEffect _pickUpSound, _gettingHitSound, _slamSound;
        public Song _titleMusic, _gameplayMusic;
        public bool _musicOn = true;
        public string _musicState = "ON";
        public bool _soundeffectsOn = true;
        public string _soundeffectsState = "ON";

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
            "Maps/Dungeon2-Section2.xml",
            "Maps/Dungeon3-Section1.xml",
            "Maps/BossFight3.xml",
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
            GBL.GDM.PreferredBackBufferWidth = 1280;
            GBL.GDM.PreferredBackBufferHeight =  720;
            GBL.GDM.ApplyChanges();

            // Initializes the cameras
            GBL._camera = new FollowCamera(GBL.GDM.GraphicsDevice.Viewport);

            _tailsCamera = new Camera2D(GraphicsDevice.Viewport, 74, 64, 32);
        }

        protected override void Initialize()
        {

            GBL.Content = Content;

            font = Content.Load<SpriteFont>("MyFont");
            _menuFont = Content.Load<SpriteFont>("MenuFont");
            _titleFont = Content.Load<SpriteFont>("TitleFont");
            _smallMenuFont = Content.Load<SpriteFont>("SmallMenuFont");

            _tilemap = Tilemap.FromFile(levelFiles[0]);


            _screenBounds = GBL.GD.PresentationParameters.Bounds;
            shopButton = new TextButton(font, "Shop", ScreenRelative(0.92f, 0.95f), Color.White);
            // for all buttons in the menus initialize them here and then only update and draw in the respective states
            playButton = new TextButton(_menuFont, "Play Game", ScreenRelative(0.82f, 0.4f), Color.White);
            guideButton = new TextButton(_menuFont, "Guide / Controls", ScreenRelative(0.82f, 0.5f), Color.White);
            settingsButton = new TextButton(_menuFont, "Settings", ScreenRelative(0.82f, 0.6f), Color.White);
            exitButton = new TextButton(_menuFont, "Exit", ScreenRelative(0.97f, 0.03f), Color.Red);
            resumeButton = new TextButton(_menuFont, "Resume", ScreenRelative(0.16f, 0.5f), Color.White);
            tailsFlipButton = new TextButton(_menuFont, "Flip to Heads", ScreenRelative(0.09f, 0.5f), Color.White);

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

            _shieldCrystalTex = Content.Load<Texture2D>("Textures\\Animations\\ShieldCrystal");
            _hayCrystalTex = Content.Load<Texture2D>("Textures\\Animations\\hayCrystal");
            _smithCrystalTex = Content.Load<Texture2D>("Textures\\Animations\\SmithCrystal");
            _gettingHitSound = Content.Load<SoundEffect>("Sounds\\Getting hit");
            _pickUpSound = Content.Load<SoundEffect>("Sounds\\Item_Pickup");
            _slamSound = Content.Load<SoundEffect>("Sounds\\Slam");
            _titleMusic = Content.Load<Song>("Sounds\\Title Music");
            _gameplayMusic = Content.Load<Song>("Sounds\\Game Level Music");


            _defaultTxr = new Texture2D(GraphicsDevice, 1, 1);
            _defaultTxr.SetData(new[] { Color.White });

            // Making music Repeat and 50% volume
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5f;
        }

        protected override void Update(GameTime gameTime)
        {

            //Makes sure the crystal texture is right
            if (_headsLevel == 3)
                _currentCrystalTex = _shieldCrystalTex;
            else if (_headsLevel == 5)
                _currentCrystalTex = _hayCrystalTex;
            else if (_headsLevel == 7)
                _currentCrystalTex = _smithCrystalTex;

            if(exitButton != null)
                if(exitButton.IsClicked)
                {
                    if (_gameState == GameState.Title) Exit();
                    else if (_gameState == GameState.MainMenu) StartTitle();
                    else if (_gameState == GameState.Paused) StartMainMenu();
                    else if (_gameState == GameState.Guide) StartMainMenu();
                    else if(_gameState == GameState.Settings) StartMainMenu();
                }

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
                case GameState.GameFinished: UpdateGameFinished(gameTime); break;
                default: StartTitle(); break;
            }



            base.Update(gameTime);
        }

        public void UpdateTitle(GameTime gameTime)
        {
            if (GBL.KeyPress(Keys.Enter)) StartMainMenu();
            _MenuBackground.Update(gameTime);

            exitButton.Update();
        }

        public void UpdateMainMenu(GameTime gameTime)
        {
            playButton.Update();

            if(playButton.IsClicked)
            {
                StartNewGame();
            }

            guideButton.Update();

            if (guideButton.IsClicked) StartGuide();

            _MenuBackground.Update(gameTime);

            settingsButton.Update();

            if(settingsButton.IsClicked) StartSettings();

            exitButton.Update();
        }

        public void UpdateHeadsLevel(GameTime gameTime)
        {
            if (_player == null) return;

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
            foreach (Sprite eachSprite in _spriteList.ToList())
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

            if (_headsLevel == 0
                || _headsLevel == 1
                || _headsLevel == 2) _coinLvl = true;

            if (_spawnProtectionTimer > 0f)
                _spawnProtectionTimer -= GBL.DeltaTime;
            // Player spawn protection, if the timer is above 0 the player cannot die and it will only start counting down after the player spawns in so they dont die immediately from something they cant see
            if (_spawnProtectionTimer <= 0f && !_spriteList.OfType<SpritePlayer>().Any())
            {
                PlayerDied();
            }

            if(_bossesDeadNum >= 2)
            {
                var groundLayer = _tilemap.Layers[1];
                int tx = (int)(_player._position.X / 32f);
                int ty = (int)(_player._position.Y / 32f);

                groundLayer.SetTile(tx, ty, 24);    // chest
                groundLayer.SetTile(tx - 2, ty, 47);    // mirror exit
                groundLayer.SetTile(tx - 2, ty - 1, 39);
            }

            // used for going to next level
            if (_tilemap.IsExitAtWorld((int)_player._position.X, (int)_player._position.Y) && _coinLvl)
            {
                NextLevel();
            }

            if (_player != null)
            {
                int tileSize = (int)map.TileWidth;
                Point curTile = new Point((int)_player._position.X / tileSize,
                                          (int)_player._position.Y / tileSize);

                bool onTrapDoor = _tilemap.TryGetTrapDoorTileAtWorld(
                    (int)_player._position.X, (int)_player._position.Y,
                    out Point trapDoorTile);

                if (onTrapDoor)
                {

                    if (_pendingTrapDoor != trapDoorTile)
                    {
                        _pendingTrapDoor = trapDoorTile;
                        _pendingTrapEntryTile = (_lastPlayerTile == trapDoorTile)
                            ? _pendingTrapEntryTile
                            : _lastPlayerTile;
                        _trapRoomDoorTimer = 0f;
                    }
                }
                else if (_pendingTrapDoor.HasValue)
                {

                    _trapRoomDoorTimer += GBL.DeltaTime;
                    if (_trapRoomDoorTimer >= _trapDoorTimer)
                    {
                        Point door  = _pendingTrapDoor.Value;
                        Point entry = _pendingTrapEntryTile;

                        int dot = (door.X - entry.X) * (curTile.X - door.X)
                                + (door.Y - entry.Y) * (curTile.Y - door.Y);

                        if (dot > 0)
                        {
                            int trapped = 0;
                            switch (_headsLevel)
                            {
                                case 0: trapped = 17; break;
                                case 1: trapped = 17; break;
                                case 2: trapped = 17; break;
                                case 3: trapped = 17; break;
                                case 4: trapped = 0;  break;
                                case 5: trapped = 0;  break;
                                case 6: trapped = 0;  break;
                                case 7: trapped = 0;  break;
                            }

                            map.SetTile(door.X, door.Y, trapped);
                            _sealedTrapDoors.Push(door);
                            _player._isInTrapRoom = true;
                        }

                        _pendingTrapDoor = null;
                        _trapRoomDoorTimer = 0f;
                    }
                }

                _lastPlayerTile = curTile;

                // the backroom on _keys.Count.
                if (_tilemap.TryGetKeyTileAtWorld((int)_player._position.X, (int)_player._position.Y, out Point keyTile))
                {
                    _keys.Add(1);
                    // Clear it from whichever layer holds it.
                    foreach (var layer in _tilemap.Layers)
                    {
                        if (layer.TryGetKeyTileAtWorld((int)_player._position.X, (int)_player._position.Y, out Point kt))
                        {
                            layer.SetTile(kt.X, kt.Y, -1);
                            break;
                        }
                    }

                    // Two keys unlocks every locked-door tile on the map.
                    if (_keys.Count >= 2) _tilemap.UnlockAllDoors();
                }


                if (_tilemap.TryGetChestTileAtWorld((int)_player._position.X, (int)_player._position.Y, out Point chestTile))
                {
                    if(!_isBossLevel) _player.ChestOpened(_player._position, new DroppedWeapon(this, _player._position, new Random().Next(0, 5)));

                    if (_player._isInTrapRoom)
                    {
                        if(_isBossLevel)_player.ChestOpened(_player._position, new DroppedWeapon(this, _player._position, new Random().Next(0, 5)));

                        if (_sealedTrapDoors.Count > 0)
                        {
                            var openedTrapDoor = 0;
                            switch (_headsLevel)
                            {
                                case 0: openedTrapDoor = 0; break;
                                case 1: openedTrapDoor = 0; break;
                                case 2: openedTrapDoor = 0; break;
                                case 3: openedTrapDoor = 0; break;
                                case 4: openedTrapDoor = 2; break;
                                case 5: openedTrapDoor = 2; break;
                                case 6: openedTrapDoor = 1; break;
                                case 7: openedTrapDoor = 1; break;
                            }


                            Point door = _sealedTrapDoors.Pop();
                            map.SetTile(door.X, door.Y, openedTrapDoor);
                        }

                        // Only "in" a trap room while at least one is sealed.
                        if (_sealedTrapDoors.Count == 0) _player._isInTrapRoom = false;
                    }
                    else if (_isBossLevel)
                    {
                        _player.ChestOpened(_player._position, new PossessedOakDrop(this, _currentCrystalTex, _player._position + new Vector2(0, -50))); //Creates drop using the right texture
                    }

                    var openedChest = 0;

                    switch (_headsLevel)
                    {
                        case 0: openedChest = 0; break;
                        case 1: openedChest = 0; break;
                        case 2: openedChest = 0; break;
                        case 3: openedChest = 0; break;
                        case 4: openedChest = 2; break;
                        case 5: openedChest = 2; break;
                        case 6: openedChest = 1; break;
                        case 7: openedChest = 1; break;
                    }

                    var map1 = _tilemap.Layers[1];
                    map1.SetTile(chestTile.X, chestTile.Y, openedChest);
                }
            }
        }

        public void UpdateTailsLevel(GameTime gameTime)
        {
            tailsFlipButton.Update();
            if (tailsFlipButton.IsClicked) NextLevel();
            // updates the camera 
            _tailsCamera.Update(gameTime);

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

            var mouse = GBL.mscurr;

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
        public void UpdateSettings(GameTime gameTime)
        {
            _SettingsBackground.Update(gameTime);

            exitButton.Update();

            // controlling music volume
            if (GBL.KeyPress(Keys.Up))
            {
                MediaPlayer.Volume = MathHelper.Clamp(MediaPlayer.Volume + 0.1f, 0f, 1f);
            }
            if (GBL.KeyPress(Keys.Down))
            {
                MediaPlayer.Volume = MathHelper.Clamp(MediaPlayer.Volume - 0.1f, 0f, 1f);
            }
            // turn on and off music and sound effects
            if (GBL.KeyPress(Keys.M))
            {
                _musicOn = !_musicOn;
                if (_musicOn)
                {
                    _musicState = "ON";
                }
                else
                {
                    _musicState = "OFF";
                }
            }
            if (GBL.KeyPress(Keys.N))
            {
                _soundeffectsOn = !_soundeffectsOn;
                if (_soundeffectsOn)
                {
                    _soundeffectsState = "ON";
                }
                else
                {
                    _soundeffectsState = "OFF";
                }
            }
        }

        public void UpdateGuide(GameTime gameTime)
        {
            _GuideBackground.Update(gameTime);

            exitButton.Update();
        }

        public void UpdatePaused(GameTime gameTime)
        {
            exitButton.Update();
            _PauseBackground.Update(gameTime);

            resumeButton.Update();
            if (resumeButton.IsClicked) StartHeadsLevel(_headsLevel);
        }

        public void UpdateDeathScreen(GameTime gameTime)
        {
            // timer ticks down
            _stateTimer -= GBL.DeltaTime;
            // makes game over only last a couple seconds
            if (_stateTimer <= 0f || GBL.KeyPress(Keys.Escape)) StartTitle();
        }
        public void UpdateGameFinished(GameTime gameTime)
        {
            // timer ticks down
            _stateTimer -= GBL.DeltaTime;
            // makes game over only last a couple seconds
            if (_stateTimer <= 0f || GBL.KeyPress(Keys.Escape)) StartTitle();
        }
        // used to start title and clear sprites for when going back to title from main menu or from beating the game or whatever else might send you back to the title
        public void StartTitle()
        {
            // makes any other music stop and plays title music
            MediaPlayer.Stop();
            if (_musicOn)
                MediaPlayer.Play(_titleMusic);

            _gameState = GameState.Title;
            // clears all sprites
            _spriteList.Clear();

            _MenuBackground = new Background(this, Content.Load<Texture2D>("Textures\\Backgrounds\\Main Menu"), 0);


        }
        //Starts main menu
        public void StartMainMenu()
        {
            // makes any other music stop and plays title music
            MediaPlayer.Stop();
            if (_musicOn)
                MediaPlayer.Play(_titleMusic);

            _gameState = GameState.MainMenu;
            // clears all sprites
            _spriteList.Clear();
        }
        public void PrepNextLevel()
        {
            // used to reset values between levels

            _pendingTrapDoor = null;
            _sealedTrapDoors.Clear();
            _trapRoomDoorTimer = 0f;
            if (_player != null) _player._isInTrapRoom = false;
        }
        // Starts Heads level
        public void StartHeadsLevel(int level)
        {
            //stops any other music and plays gameplay music
            MediaPlayer.Stop();
            if (_musicOn)
                MediaPlayer.Play(_gameplayMusic);

            _gameState = GameState.HeadsLevel;
            _coinSide = true;
            _headsLevel = level;
            // clears old sprites
            _spriteList.Clear();
            _newSpriteList.Clear();
            _spawnProtectionTimer = SpawnProtectionDuration;
            // spawns the sprites that appear at start of game
            LevelData.SpawnLevel(_headsLevel, this);

            if (_player != null)
            {
                _health = _player.Stats.MaxHP;
                _player._posture = _player.Stats.Posture;
            }
        }
        // Start Tails level
        public void StartTailsLevel(int level)
        {
            //stops any other music and plays gameplay music
            MediaPlayer.Stop();
            if (_musicOn)
                MediaPlayer.Play(_gameplayMusic);

            _gameState = GameState.TailsLevel;
            _coinSide = false;
            _tailsLevel = level;

            // clears old sprites
            _spriteList.Clear();
            _newSpriteList.Clear();
            // spawns the sprites that appear at start of game
            LevelData.SpawnLevel(_tailsLevel, this);

            _currency.AddCoins(_currency.Population * 50);
            _box = new DialogBox(this, new List<string>() { "Huh What the where did this gold just appear from...", "welp who cares its mine now hehehhe" });
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

            _PauseBackground = new Background(this, Content.Load<Texture2D>("Textures\\Backgrounds\\Main Menu"), 3);
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

            _GuideBackground = new Background(this, Content.Load<Texture2D>("Textures\\Backgrounds\\Main Menu"), 1);

        }
        // Starts settings
        public void StartSettings()
        {
            // stops music
            MediaPlayer.Stop();

            _gameState = GameState.Settings;

            _SettingsBackground = new Background(this, Content.Load<Texture2D>("Textures\\Backgrounds\\Main Menu"), 2);
        }
        public void StartGameFinished()
        {
            // stops music
            MediaPlayer.Stop();

            _gameState = GameState.GameFinished;
            _stateTimer = 5f;
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
                    foreach (Sprite eachSprite in _spriteList)
                    {
                        if (eachSprite is DialogBox || eachSprite is WeaponSelection) continue;
                        eachSprite.Draw(GBL.spriteBatch);
                    }
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
                    {
                        if (s is DialogBox db) db.Draw(GBL.spriteBatch);
                        if (s is WeaponSelection ws) ws.Draw(GBL.spriteBatch);
                    }
                    break;
                case GameState.TailsLevel:
                    tailsFlipButton.Draw();
                    _shopUI.DrawCurrency(10, 10, _currency.Coins, GBL.Content.Load<Texture2D>("Textures\\UI\\Coin Icon"), "g");
                    _shopUI.DrawCurrency(10, 50, _currency.Population, GBL.Content.Load<Texture2D>("Textures\\UI\\Population Icon"), "p");
                    _shopUI.DrawCurrency(10, 90, _currency.Food, GBL.Content.Load<Texture2D>("Textures\\UI\\Food Icon"), "f");
                    _shopUI.DrawCurrency(10, 130, _currency.Energy, GBL.Content.Load<Texture2D>("Textures\\UI\\Energy Icon"), "e");
                    _shopUI.Draw();
                    shopButton.Draw();
                    Vector2 RectPos = ScreenRelative(0.88f, 0.93f);
                    GBL.spriteBatch.Draw(_shopButtonTexture, new Rectangle((int)RectPos.X, (int)RectPos.Y, 96, 32), _shopButtonTexture.Bounds, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.09f);
                    if (_showTailsIntro && _box != null)
                        _box.Draw(GBL.spriteBatch);
                    break;
                case GameState.Guide: DrawGuide(); break;
                case GameState.Settings: DrawSettings(); break;
                case GameState.GameFinished: DrawGameFinished(); break;
            }
            
            GBL.spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawTitle()
        {
            _MenuBackground.Draw(GBL.spriteBatch);
            exitButton.Draw();

            FancyText(_titleFont, "Nummi", ScreenRelative(0.5f, 0.1f), Color.Gold, Color.White);
            FancyText(_menuFont, "Press [ENTER] to Begin", ScreenRelative(0.5f, 0.9f), Color.Gold, Color.White, 1f, 1.5f);

        }
        public void DrawMainMenu()
        {
            playButton.Draw();
            guideButton.Draw();
            settingsButton.Draw();
            exitButton.Draw();

            _MenuBackground.Draw(GBL.spriteBatch);

            Vector2 Rectpos = ScreenRelative(0.65f, 0f);

            Rectangle rect = new Rectangle((int)Rectpos.X, (int)Rectpos.Y, GBL.GDM.PreferredBackBufferWidth / 2, GBL.GDM.PreferredBackBufferHeight);

            GBL.spriteBatch.Draw(
                _defaultTxr,
                rect,
                null,
                Color.White * 0.5f,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.95f
            );

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

            gridRenderer.Draw();
        }
        public void DrawSettings()
        {
            _SettingsBackground.Draw(GBL.spriteBatch);
            exitButton.Draw();

            FancyText(_menuFont, "Settings", ScreenRelative(0.16f, 0.1f), Color.White, Color.Black);
            FancyText(_smallMenuFont, "UP/DOWN to Control Music Volume ", ScreenRelative(0.16f, 0.15f), Color.White, Color.Black, 1.5f);
            FancyText(_smallMenuFont, "Current Volume: " + (int)(MediaPlayer.Volume * 100) + "%", ScreenRelative(0.16f, 0.2f), Color.White, Color.Black, 1.5f);
            FancyText(_smallMenuFont, "M Toggle Music On/Off: " + _musicState, ScreenRelative(0.16f, 0.3f), Color.White, Color.Black, 1.5f);
            FancyText(_smallMenuFont, "N Toggle Sound Effects On/Off: " + _soundeffectsState, ScreenRelative(0.16f, 0.35f), Color.White, Color.Black, 1.5f);

            Vector2 Rectpos = ScreenRelative(-0.15f, 0f);

            Rectangle rect = new Rectangle((int)Rectpos.X, (int)Rectpos.Y, GBL.GDM.PreferredBackBufferWidth / 2, GBL.GDM.PreferredBackBufferHeight);

            GBL.spriteBatch.Draw(
                _defaultTxr,
                rect,
                null,
                Color.White * 0.5f,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.95f
            );
        }
        public void DrawGuide()
        {
            _GuideBackground.Draw(GBL.spriteBatch);
            exitButton.Draw();

            Vector2 Rectpos = ScreenRelative(-0.15f, 0f);

            Rectangle rect = new Rectangle((int)Rectpos.X, (int)Rectpos.Y, GBL.GDM.PreferredBackBufferWidth / 2, GBL.GDM.PreferredBackBufferHeight);

            GBL.spriteBatch.Draw(
                _defaultTxr,
                rect,
                null,
                Color.White * 0.5f,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.95f
            );

            FancyText(_menuFont, "Guide", ScreenRelative(0.16f, 0.1f), Color.White, Color.Black);
            FancyText(_smallMenuFont, "Your Goal is to traverse through\nthe dungeons and build your town up\n(some town buildings \nwill affect your heads stats)", ScreenRelative(0.15f, 0.15f), Color.White, Color.Black);
            
            FancyText(_menuFont, "Controls", ScreenRelative(0.16f, 0.25f), Color.White, Color.Black);
            FancyText(_smallMenuFont, "WASD for movement", ScreenRelative(0.16f, 0.3f), Color.White, Color.Black);
            FancyText(_smallMenuFont, "LMB to attack", ScreenRelative(0.16f, 0.35f), Color.White, Color.Black);
            FancyText(_smallMenuFont, "Q to Dash", ScreenRelative(0.16f, 0.4f), Color.White, Color.Black);
            FancyText(_smallMenuFont, "F to Block", ScreenRelative(0.16f, 0.45f), Color.White, Color.Black);
            FancyText(_smallMenuFont, "(Be Aware of your posture)", ScreenRelative(0.16f, 0.5f), Color.White, Color.Black);

        }
        public void DrawPaused()
        {
            exitButton.Draw();
            _PauseBackground.Draw(GBL.spriteBatch);
            resumeButton.Draw();

            Vector2 Rectpos = ScreenRelative(-0.15f, 0f);

            Rectangle rect = new Rectangle((int)Rectpos.X, (int)Rectpos.Y, GBL.GDM.PreferredBackBufferWidth / 2, GBL.GDM.PreferredBackBufferHeight);

            GBL.spriteBatch.Draw(
                _defaultTxr,
                rect,
                null,
                Color.White * 0.5f,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.95f
            );
        }
        public void DrawDeathScreen()
        {
            Rectangle rect = new Rectangle(0,0, GBL.GDM.PreferredBackBufferWidth, GBL.GDM.PreferredBackBufferHeight);

            GBL.spriteBatch.Draw(
                _defaultTxr,
                rect,
                null,
                Color.Black,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.95f
            );

            FancyText(_titleFont, "GAME OVER", ScreenRelative(0.5f, 0.5f), Color.Red, Color.White);
            FancyText(_menuFont, "Mr Mirrors Reign Continues", ScreenRelative(0.5f, 0.6f), Color.Red, Color.White);
        }
        public void DrawGameFinished()
        {
            Rectangle rect = new Rectangle(0, 0, GBL.GDM.PreferredBackBufferWidth, GBL.GDM.PreferredBackBufferHeight);

            GBL.spriteBatch.Draw(
                _defaultTxr,
                rect,
                null,
                Color.Gold,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.95f
            );
            FancyText(_titleFont, "GAME OVER", ScreenRelative(0.5f, 0.5f), Color.Red, Color.White);
            FancyText(_menuFont, "Mr Mirror has Consumed your mind he was never supposed to be beaten", ScreenRelative(0.5f, 0.6f), Color.Red, Color.White);
        }

        // start new game
        public void StartNewGame()
        {
            _player = null;
            _bossDead = true;
            _isBossLevel = false;

            _headsLevel = 0;
            _tailsLevel = 0;

            _spriteList.Clear();
            _newSpriteList.Clear();

            StartHeadsLevel(_headsLevel);
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
                StartHeadsLevel(_headsLevel);
            }
        }
        // going to next level and if you are on the last level it goes back to the title screen
        public void NextLevel()
        {
            PrepNextLevel();

            // =========================================
            // HEADS LEVEL
            // =========================================
            if (_gameState == GameState.HeadsLevel)
            {
                // Going to tails first
                if (_isNextLevelTails)
                {
                    // Save the NEXT heads level
                    _pendingHeadsLevel = _headsLevel + 1;

                    // Go to next tails level
                    _tailsLevel++;

                    StartTailsLevel(_tailsLevel);

                    _isNextLevelTails = false;

                    return;
                }

                // Normal heads progression
                _headsLevel++;

                if (_headsLevel > LevelData.LastHeadsLevelIndex)
                {
                    StartGameFinished();
                    return;
                }

                StartHeadsLevel(_headsLevel);
            }

            // =========================================
            // TAILS LEVEL
            // =========================================
            else if (_gameState == GameState.TailsLevel)
            {
                // Resume stored heads level
                if (_pendingHeadsLevel >= 0)
                {
                    _headsLevel = _pendingHeadsLevel;

                    _pendingHeadsLevel = -1;

                    StartHeadsLevel(_headsLevel);

                    return;
                }

                _headsLevel++;

                if (_headsLevel > LevelData.LastHeadsLevelIndex)
                {
                    StartGameFinished();
                    return;
                }

                StartHeadsLevel(_headsLevel);
            }
        }


        #region ***** Utility Functions *****
        // For fancy out lined shadowed text
        public void FancyText(
            SpriteFont font,
            string text,
            Vector2 position,
            Color foreground,
            Color shadow,
            float scale = 1f,
            float shadowSize = 2f)
        {
            Vector2 size = font.MeasureString(text) * scale;
            Vector2 textPos = position - (size / 2f);

            GBL.spriteBatch.DrawString(
                font,
                text,
                textPos - new Vector2(shadowSize),
                shadow,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0.9f);

            GBL.spriteBatch.DrawString(
                font,
                text,
                textPos + new Vector2(shadowSize),
                shadow,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0.9f);

            GBL.spriteBatch.DrawString(
                font,
                text,
                textPos,
                foreground,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0.01f);
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
        GameFinished,

    }
}
