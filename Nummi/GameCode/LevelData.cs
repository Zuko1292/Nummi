using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nummi;

namespace Nummi
{
    public static class LevelData
    {
        // This class is responsible for spawning levels and contains data related to the levels, such as tilemap rules and level-specific settings. It has a method called SpawnLevel which takes in the level number and the game root object, and based on the current game state (HeadsLevel or TailsLevel), it loads the appropriate tilemap, sets up the player, enemies, and other sprites for that level. It also contains helper methods for converting tile coordinates to world coordinates and generating buildable areas from the tilemap.
        // This varible is used to determine the final level
        public static int LastHeadsLevelIndex = 7;

        // Rules for Heads town and Tails Town and Dungeon 1 section 1 and 2 (which share the same tileSet)
        public static readonly TilemapRules Rules1 = new TilemapRules()
            .AddSolid(4, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 21, 23, 24, 25, 26, 27, 28, 29, 30, 31)
            .AddExit(14, 22)
            .AddChest(7)
            .AddTrapDoor(3)
            .AddKey()
            .SetAffectsLayers(CollisionLayer.Player | CollisionLayer.Enemy);

        // Rules for dungeon 1 section 1, 2 and 3 (which share the same tileSet)
        public static readonly TilemapRules Rules2 = new TilemapRules()
            .AddSolid(0, 8, 9, 10, 11, 12, 13, 14, 16, 17, 18, 19, 20, 21, 22, 25, 26, 27, 28, 29, 30, 35, 36, 37, 43, 44, 45, 51, 52, 53, 56, 57, 58, 60, 61, 62)
            .AddExit(39, 47)
            .AddChest(24)
            .AddTrapDoor(46)
            .AddKey(31)
            .AddLockedDoor(62)
            .SetAffectsLayers(CollisionLayer.Player | CollisionLayer.Enemy);

        public static readonly TilemapRules Rules3 = new TilemapRules()
            .AddSolid(0, 2, 5, 6, 7, 8, 9, 10, 11, 13, 14, 15, 17)
            .AddExit(12, 16)
            .AddChest(3)
            .AddTrapDoor(18)
            .AddKey(4)
            .AddLockedDoor(19)
            .SetAffectsLayers(CollisionLayer.Player | CollisionLayer.Enemy);


        public static void SpawnLevel(int level, Game1 gameRoot)
        {
            
            switch (gameRoot._gameState)
            {

                case GameState.HeadsLevel:
                    gameRoot._headsLevel = level;
                    gameRoot._spriteList.Clear();
                    gameRoot._newSpriteList.Clear();

                    gameRoot.savedStats = gameRoot._player?.Stats ?? new CharacterStats(str: 1, vit: 1);
                    gameRoot.savedLevelSystem = gameRoot._player?.LevelSystem ?? new LevelSystem();
                    if(gameRoot._player != null)
                        gameRoot.savedWeapon = gameRoot._player._currentWeapon;

                    gameRoot._player = null;
                    // When adding enemies or NPCs to the levels, make sure to add them to the _spriteList so they get updated and drawn. Also make sure to set their position using the TilePos helper method to convert tile coordinates to world coordinates. For example, if you want to place an enemy at tile (10, 5), you would use TilePos(10, 5) to get the correct world position for that enemy.
                    // Load stuff into the heads levels here
                    // If you want to draw like UI which is not offset by camera dont do it here follow where I did it in game1(Developer note)
                    switch (level)
                    {
                        case 0:
                            // If is trap level more it true
                            gameRoot._isTrapLevel = false;
                            gameRoot._isBossLevel = false;
                            // If is a lighting level make it true and set torch positions, if not set to false and empty array, set the torch position. light positions like I did in case 3.
                            gameRoot._useLighting = false;
                            gameRoot._torchPositions = Array.Empty<Vector2>();
                            // Load the tilemap for the level and set the rules for it. The rules determine which tiles are solid, which are exits, which are chests, etc. Make sure to set the rules according to the tile IDs used in the tilemap for that level.
                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[0]);
                            gameRoot._tilemap.SetRules(Rules1);
                            // Loads the player always load it like this
                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(45, 45), true);
                            gameRoot._spriteList.Add(gameRoot._player);

                            // always load enemies and Npcs like this and set their position using the TilePos helper method. Make sure to add them to the _spriteList so they get updated and drawn.
                            gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Slime Anim-Sheet"), TilePos(7, 44)));
                            gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Slime Anim-Sheet"), TilePos(7, 4)));
                            gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Slime Anim-Sheet"), TilePos(42, 4)));
                            gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Slime Anim-Sheet"), TilePos(24, 11)));

                            gameRoot._spriteList.Add(new HeadsHouse(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Houses\\House2_v2"), TilePos(14, 18)));
                            gameRoot._spriteList.Add(new HeadsHouse(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Houses\\House1_v2"), TilePos(20, 18)));
                            gameRoot._spriteList.Add(new HeadsHouse(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Houses\\House3_v2"), TilePos(26, 18)));
                            gameRoot._spriteList.Add(new HeadsHouse(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Houses\\BlackSmith_HeadsTown"), TilePos(37, 18)));

                            gameRoot._spriteList.Add(new SpriteNPC(gameRoot,
                                GBL.Content.Load<Texture2D>("Textures\\Animations\\TownPerson1"),
                                TilePos(37, 20),
                                false, 3f, 85f, 0.98f,
                                new List<string>()
                                {
                                    "God These people have gone mad",
                                    "I'm just the weapon maker",
                                    "I've got Swords, Great Swords, Maces, Great Hammers and a Bow",
                                    "Take your pick"
                                },
                                isBlackSmith: true));
                            gameRoot._spriteList.Add(new SpriteNPC(gameRoot,
                                GBL.Content.Load<Texture2D>("Textures\\Animations\\TownPerson3"),
                                TilePos(26, 21),
                                true, 3f, 85f, 0.98f,
                                new List<string>()
                                {
                                    "Are you just Another Delusion",
                                    "Oh, your not well thats a relief",
                                    "But still my mind is trapped in this nightmare",
                                    "It's MR Mirror, He torments my dreams, I get no sleep",
                                    "I see him in the shadows, I see him in the light",
                                    "I dont know whats real anymore",
                                    "*Goes back to screaming*"
                                }));
                            gameRoot._spriteList.Add(new SpriteNPC(gameRoot,
                                GBL.Content.Load<Texture2D>("Textures\\Animations\\TownPerson4"),
                                TilePos(14, 20),
                                true, 3f, 85f, 0.98f,
                                new List<string>()
                                {
                                    "He's all I see",
                                    "He's all I see",
                                    "He's all I see",
                                    "MR MIRROR, MR MIRROR, MR MIRROR"
                                }));
                            gameRoot._spriteList.Add(new SpriteNPC(gameRoot,
                                GBL.Content.Load<Texture2D>("Textures\\Animations\\TownPerson6"),
                                TilePos(20, 23),
                                true, 3f, 85f, 0.98f,
                                new List<string>()
                                {
                                    "Seems I'm one of the few left with my wits about me",
                                    "MR Mirror isn't all that scary just annoying to me\n*cackles cockily*",
                                    "Anyway I guess I still care some what for my fellow\nTownspeople, He resides in the mirrors. Look over there",
                                    "Do me a favour and go into the mirror\n east of the town square, youll find him",
                                    "Well somewhere in the mirror world at least"
                                }));

                            // gameRoot._spriteList.Add(new SpriteNPC(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Player_SpriteSheet"), TilePos(12, 5), true, 3f));
                            break;
                        case 1:
                            gameRoot._isTrapLevel = true;
                            gameRoot._isBossLevel = false;
                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[2]);
                            gameRoot._tilemap.SetRules(Rules1);

                            gameRoot._useLighting = false;
                            gameRoot._torchPositions = Array.Empty<Vector2>();

                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(6, 8), true, gameRoot.savedStats, gameRoot.savedLevelSystem);
                            gameRoot._spriteList.Add(gameRoot._player);

                           // Room 2 Enemies
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Slime Anim-Sheet"),TilePos(32, 8)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(33, 11)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(33, 5)));
                           
                           // Room 3 Enemies
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(49, 11)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Slime Anim-Sheet"), TilePos(48, 4)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(58, 12)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(56, 7)));
                           
                           // Room 4 Enemies
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(43, 18)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(49, 20)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(46, 27)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(43, 24)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(43, 30)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(51, 27)));
                           
                           // Trap Room Enemies
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(53, 35)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Slime Anim-Sheet"), TilePos(53, 37)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(53, 39)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(53, 41)));
                           
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(59, 35)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(59, 37)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Slime Anim-Sheet"), TilePos(59, 39)));
                           gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(59, 41)));


                            break;
                        case 2:

                            gameRoot._isTrapLevel = true;
                            gameRoot._isBossLevel = false;
                            gameRoot._useLighting = true;
                            gameRoot._torchPositions = new Vector2[]
                            {
                                TilePos(41, 10)- new Vector2(10, 10),
                                TilePos(41, 6)- new Vector2(10, 10),
                                TilePos(49, 4)- new Vector2(10, 10),
                                TilePos(55, 12)- new Vector2(10, 10),
                                TilePos(59, 12)- new Vector2(10, 10),
                                TilePos(49, 19)- new Vector2(10, 10),
                                TilePos(59, 20)- new Vector2(10, 10),
                                TilePos(54, 23)- new Vector2(10, 10),
                                TilePos(54, 25)- new Vector2(10, 10),
                                TilePos(59, 24)- new Vector2(10, 10),
                                TilePos(59, 28)- new Vector2(10, 10),
                                TilePos(41, 21)- new Vector2(10, 10),
                                TilePos(41, 27)- new Vector2(10, 10),
                                TilePos(35, 21)- new Vector2(10, 10),
                                TilePos(35, 27)- new Vector2(10, 10),
                                TilePos(49, 29)- new Vector2(10, 10)
                            };

                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[3]);
                            gameRoot._tilemap.SetRules(Rules1);
                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(9, 8), true, gameRoot.savedStats, gameRoot.savedLevelSystem);
                            gameRoot._spriteList.Add(gameRoot._player);

                            // Room 2 Enemies
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(26, 5)));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(26, 11)));
                            
                            // Room 3 Enemies
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(46, 14)));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(52, 7)));
                            
                            // Room 4 Enemies
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(46, 20)));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(51, 28)));
                            
                            // Trap Room Enemies
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(56, 42)));

                            break;
                        case 3:
                            // When its a boss level make sure you do the _bossDead variable to false and set the current boss to the boss you want in the level.
                            gameRoot._bossDead = false;
                            gameRoot._isBossLevel = true;
                            gameRoot._isTrapLevel = false;

                            gameRoot._useLighting = false;
                            gameRoot._torchPositions = Array.Empty<Vector2>();

                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[4]);
                            gameRoot._tilemap.SetRules(Rules1);
                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(9, 3), true, gameRoot.savedStats, gameRoot.savedLevelSystem);
                            gameRoot._spriteList.Add(gameRoot._player);

                            SpriteEnemy boss = new PossessedTree(gameRoot, TilePos(10, 14));
                            gameRoot._currentBoss = boss;
                            gameRoot._bossName = "POSSESSED OAK";
                            gameRoot._spriteList.Add(boss);
                            break;
                        case 4:
                            gameRoot._isTrapLevel = false;
                            gameRoot._isBossLevel = false;
                            gameRoot._useLighting = false;
                            gameRoot._torchPositions = Array.Empty<Vector2>();

                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[5]);
                            gameRoot._tilemap.SetRules(Rules2);
                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(6, 10), true, gameRoot.savedStats, gameRoot.savedLevelSystem);
                            gameRoot._spriteList.Add(gameRoot._player);

                            

                            // Room 1 Enemies
                            gameRoot._spriteList.Add(new Security_Guard(gameRoot, TilePos(25, 7), Security_Guard.TempState.Frozen));
                            gameRoot._spriteList.Add(new Security_Guard(gameRoot, TilePos(25, 13), Security_Guard.TempState.Frozen));

                            // Room 2 Enemies
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(42, 6), Waiter.TempState.Frozen));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(43, 13), Waiter.TempState.Frozen));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(61, 15), Waiter.TempState.Frozen));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(63, 14), Waiter.TempState.Frozen));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(65, 15), Waiter.TempState.Frozen));

                            // Room 3 Enemies
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(62, 23), Waiter.TempState.Frozen, true));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(64, 25), Waiter.TempState.Frozen, true));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(62, 27), Waiter.TempState.Frozen, true));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(63, 30), Waiter.TempState.Frozen, true));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(64, 33), Waiter.TempState.Frozen, true));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(62, 35), Waiter.TempState.Frozen, true));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(64, 38), Waiter.TempState.Frozen, true));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(62, 39), Waiter.TempState.Frozen, true));

                            // Room 4 Enemies
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(71, 45), Waiter.TempState.Frozen));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(95, 45), Waiter.TempState.Frozen));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(119, 45), Waiter.TempState.Frozen));

                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(64, 52), Waiter.TempState.Frozen));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(73, 54), Waiter.TempState.Frozen));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(88, 53), Waiter.TempState.Frozen));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(96, 54), Waiter.TempState.Frozen));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(101, 50), Waiter.TempState.Frozen));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(112, 53), Waiter.TempState.Frozen));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(120, 50), Waiter.TempState.Frozen));
                            gameRoot._spriteList.Add(new Waiter(gameRoot, TilePos(126, 56), Waiter.TempState.Frozen));

                            gameRoot._spriteList.Add(new Dealer(gameRoot, TilePos(67, 60), Dealer.TempState.Frozen));
                            gameRoot._spriteList.Add(new Dealer(gameRoot, TilePos(75, 60), Dealer.TempState.Frozen));
                            gameRoot._spriteList.Add(new Dealer(gameRoot, TilePos(91, 60), Dealer.TempState.Frozen));
                            gameRoot._spriteList.Add(new Dealer(gameRoot, TilePos(99, 60), Dealer.TempState.Frozen));
                            gameRoot._spriteList.Add(new Dealer(gameRoot, TilePos(115, 60), Dealer.TempState.Frozen));
                            gameRoot._spriteList.Add(new Dealer(gameRoot, TilePos(123, 60), Dealer.TempState.Frozen));

                            gameRoot._spriteList.Add(new Security_Guard(gameRoot, TilePos(131, 53), Security_Guard.TempState.Frozen));

                            SpriteEnemy bossFrozen = new Manager_Croc(gameRoot, TilePos(149, 53), Manager_Croc.TempState.Frozen);
                            gameRoot._spriteList.Add(bossFrozen);

                            break;
                        case 5:
                            gameRoot._isTrapLevel = true;
                            gameRoot._isBossLevel = true;
                            gameRoot._bossDead = false;

                            gameRoot._useLighting = false;
                            gameRoot._torchPositions = Array.Empty<Vector2>();

                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[6]);
                            gameRoot._tilemap.SetRules(Rules2);
                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(24, 5), true, gameRoot.savedStats, gameRoot.savedLevelSystem);
                            gameRoot._spriteList.Add(gameRoot._player);

                            //TilePos(150, 47)

                            SpriteEnemy boss2 = new Manager_Croc(gameRoot, TilePos(9, 10), Manager_Croc.TempState.Thawed);
                            gameRoot._currentBoss = boss2;
                            gameRoot._bossName = "Manager Croc";
                            gameRoot._spriteList.Add(boss2);
                            break;
                        case 6:
                            gameRoot._isTrapLevel = true;
                            gameRoot._isBossLevel = false;
                            gameRoot._useLighting = false;
                            gameRoot._torchPositions = Array.Empty<Vector2>();

                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[7]);
                            gameRoot._tilemap.SetRules(Rules3);
                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(7, 27), true, gameRoot.savedStats, gameRoot.savedLevelSystem);
                            gameRoot._spriteList.Add(gameRoot._player);

                            // Room 1 Enemies 

                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(39, 19)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(35, 27)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(39, 23)));

                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(34, 23), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(32, 25), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(41, 28), 500, 300f, 300));

                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(32, 20), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(33, 27), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(21, 22), 300, 300f, 300));

                            // Room 2 Enemies
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(58, 19)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(54, 27)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(58, 23)));

                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(52, 23), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(51, 25), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(60, 28), 500, 300f, 300));

                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(51, 20), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(52, 27), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(40, 22), 300, 300f, 300));

                            // Room 3 Enemies
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(77, 19)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(73, 27)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(77, 23)));

                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(71, 23), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(70, 25), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(79, 28), 500, 300f, 300));

                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(70, 20), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(71, 27), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(59, 22), 300, 300f, 300));

                            // Room 4 Enemies
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(96, 19)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(92, 27)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(96, 23)));

                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(90, 23), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(89, 25), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(98, 28), 500, 300f, 300));

                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(89, 20), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(90, 27), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(78, 22), 300, 300f, 300));

                            var zone = new DetectionZone(gameRoot, TilePos(37, 27), 576, 672, 1);
                            gameRoot._spriteList.Add(zone);
                            var zone2 = new DetectionZone(gameRoot, TilePos(56, 27), 576, 672, 2);
                            gameRoot._spriteList.Add(zone2);
                            var zone3 = new DetectionZone(gameRoot, TilePos(75, 27), 576, 672, 3);
                            gameRoot._spriteList.Add(zone3);
                            var zone4 = new DetectionZone(gameRoot, TilePos(94, 27), 576, 672, 4);
                            gameRoot._spriteList.Add(zone4);

                            gameRoot._spriteList.Add(new Bartender(gameRoot, TilePos(36, 18)));
                            gameRoot._spriteList.Add(new Bartender(gameRoot, TilePos(54, 18)));
                            gameRoot._spriteList.Add(new Bartender(gameRoot, TilePos(72, 18)));
                            gameRoot._spriteList.Add(new Bartender(gameRoot, TilePos(90, 18)));

                            break;
                        case 7:
                            gameRoot._isTrapLevel = true;
                            gameRoot._isBossLevel = false;
                            gameRoot._useLighting = false;
                            gameRoot._torchPositions = Array.Empty<Vector2>();

                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[7]);
                            gameRoot._tilemap.SetRules(Rules3);
                            var map = gameRoot._tilemap.Layers[1];

                            map.SetTile(115, 26, -1);
                            map.SetTile(115, 27, -1);
                            map.SetTile(4, 26, 16);
                            map.SetTile(4, 27, 12);

                            var map2 = gameRoot._tilemap.Layers[0];

                            map.SetTile(103, 27, 1);

                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(113, 27), true, gameRoot.savedStats, gameRoot.savedLevelSystem);
                            gameRoot._spriteList.Add(gameRoot._player);

                            // Room 1 Enemies 

                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(39, 19)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(35, 27)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(39, 23)));

                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(34, 23), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(32, 25), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(41, 28), 500, 300f, 300));

                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(32, 20), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(33, 27), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(21, 22), 300, 300f, 300));

                            // Room 2 Enemies
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(58, 19)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(54, 27)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(58, 23)));

                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(52, 23), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(51, 25), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(60, 28), 500, 300f, 300));

                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(51, 20), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(52, 27), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(40, 22), 300, 300f, 300));

                            // Room 3 Enemies
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(77, 19)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(73, 27)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(77, 23)));

                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(71, 23), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(70, 25), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(79, 28), 500, 300f, 300));

                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(70, 20), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(71, 27), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(59, 22), 300, 300f, 300));

                            // Room 4 Enemies
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(96, 19)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(92, 27)));
                            gameRoot._spriteList.Add(new Drunken_Rat(gameRoot, TilePos(96, 23)));

                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(90, 23), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(89, 25), 500, 300f, 300));
                            gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(98, 28), 500, 300f, 300));

                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(89, 20), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(90, 27), 300, 300f, 300));
                            gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(78, 22), 300, 300f, 300));

                            var zone1 = new DetectionZone(gameRoot, TilePos(94, 27), 576, 672, 1);
                            gameRoot._spriteList.Add(zone1);
                            var zone22 = new DetectionZone(gameRoot, TilePos(75, 27), 576, 672, 2);
                            gameRoot._spriteList.Add(zone22);
                            var zone33 = new DetectionZone(gameRoot, TilePos(56, 27), 576, 672, 3);
                            gameRoot._spriteList.Add(zone33);
                            var zone44 = new DetectionZone(gameRoot, TilePos(37, 27), 576, 672, 4);
                            gameRoot._spriteList.Add(zone44);

                            break;
                    }
                    if (gameRoot._player != null) gameRoot._player._currentWeapon = gameRoot.savedWeapon;
                    break;
                case GameState.TailsLevel:
                    gameRoot._tailsLevel = level;

                    gameRoot._spriteList.Clear();
                    gameRoot._newSpriteList.Clear();

                    gameRoot._player = null;
                    // Load stuff into the tails levels here
                    switch (level)
                    {
                        case 0:
                            break;
                        case 1:
                            //Load the player so the stats save and load properly when going between levels, also make sure to set the position of the player using the TilePos helper method to convert tile coordinates to world coordinates.
                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(1000, 1000), true, gameRoot.savedStats, gameRoot.savedLevelSystem);
                            gameRoot._spriteList.Add(gameRoot._player);

                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[1]);
                            SetBuildingLimits(gameRoot, 1);

                            gameRoot._grid.ResetBuildable(); // Clear old data first
                            GenerateBuildableFromTilemap(gameRoot._grid, gameRoot._tilemap, 0);

                            gameRoot._spriteList.Add(new Grid(gameRoot, new Vector2(1024 + (5 * 32), 1024)));
                            break;
                        case 2:
                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(1000, 1000), true, gameRoot.savedStats, gameRoot.savedLevelSystem);
                            gameRoot._spriteList.Add(gameRoot._player);

                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[1]);
                            SetBuildingLimits(gameRoot, 2);

                            GenerateBuildableFromTilemap(gameRoot._grid, gameRoot._tilemap, 0);

                            gameRoot._spriteList.Add(new Grid(gameRoot, new Vector2(1024 + (5 * 32), 1024)));
                            break;
                    }
                    if (gameRoot._player != null) gameRoot._player._currentWeapon = gameRoot.savedWeapon;
                    break;
            }
        }
        // This method converts tile coordinates to world coordinates. It takes a Point representing the tile position, an optional grid size (defaulting to 32x32), and a boolean indicating whether to return the position of the center of the tile (defaulting to true). If centred is true, it adds half the grid size to the tile position to return the center; otherwise, it returns the top-left corner of the tile.
        public static Vector2 TilePos(Point tile, Point gridSize = default, bool centred = true)
        {
            if (gridSize == default) gridSize = new Point(32, 32);

            if (centred) return new Vector2(tile.X * 32 + gridSize.X / 2, tile.Y * 32 + gridSize.Y / 2);
            return new Vector2(tile.X * gridSize.X, tile.Y * gridSize.Y);
        }
        // Overload for convenience when you want to pass X and Y directly instead of creating a Point
        public static Vector2 TilePos(int X, int Y)
        {
            return TilePos(new Point(X, Y));
        }

        // This method generates the buildable grid based on the tilemap. It checks each tile in the ground layer to see if it's a grass tile and then checks the object layer to see if there's anything on top of it. If it's grass and there's nothing on top, it marks that tile as buildable in the grid system. This allows the game to determine where the player can place buildings based on the tilemap design.
        public static void GenerateBuildableFromTilemap(GridSystem grid, TilemapGroup tilemap, int grassTileId)
        {
            var groundLayer = tilemap.Layers[0];
            var objectLayer = tilemap.Layers[1];

            for (int x = 0; x < groundLayer.Columns; x++)
            {
                for (int y = 0; y < groundLayer.Rows; y++)
                {
                    int groundTileId = groundLayer.GetTileId(x, y);
                    int objectTileId = objectLayer.GetTileId(x, y);

                    // Buildable only if grass AND nothing on top
                    bool isGrass = groundTileId == grassTileId;
                    bool hasObject = objectTileId >= 0; // -1 = empty, anything else = object

                    grid.SetBuildable(new Point(x, y), isGrass && !hasObject);
                }
            }
        }

        private static void SetBuildingLimits(Game1 gameRoot, int tailsLevel)
        {
            gameRoot.buildingSystem.ResetCounts();
            gameRoot._shop.ClearStock();

            // Re-add any items the player has unlocked via boss drops so they
            // survive the shop rebuild when entering a tails level.
            foreach (ShopItem unlocked in gameRoot._unlockedBossItems)
                gameRoot._shop.AddItem(unlocked);

            switch (tailsLevel)
            {
                // Once added Item to case 0 it'll be available in all levels, so we can add all items here and just adjust limits for each level. For now we only have 2 levels so I put everything in case 0 and left case 1 empty for future expansion.
                // TODO probably change it so you only unlock buildings after the dungeons
                case 0:
                    break;
                case 1:
                    gameRoot.buildingSystem.SetLimit("House", 3);
                    gameRoot.buildingSystem.SetLimit("Barracks", 1);

                    gameRoot._shop.AddItem(new ShopItem(
                        "House",
                        "Increases Population",
                        GBL.Content.Load<Texture2D>("Textures\\Houses\\House1"),
                        cost: 50,
                        building: new BuildingType("House", GBL.Content.Load<Texture2D>("Textures\\Houses\\House1"), new Point(2, 2))
                    ));

                    
                    //gameRoot._shop.AddItem(new ShopItem(
                    //    "Farmhouse",
                    //    "Produces food",
                    //    GBL.Content.Load<Texture2D>("Textures\\SpecialBuildings\\Farm Building"),
                    //    cost: 150,
                    //    building: new BuildingType("Farm", GBL.Content.Load<Texture2D>("Textures\\SpecialBuildings\\Farm Building"), new Point(3, 3))
                    //));
                    //gameRoot._shop.AddItem(new ShopItem(
                    //    "Black Smith",
                    //    "Produces weapons",
                    //    GBL.Content.Load<Texture2D>("Textures\\SpecialBuildings\\BlackSmith"),
                    //    cost: 150,
                    //    building: new BuildingType("Black Smith", GBL.Content.Load<Texture2D>("Textures\\SpecialBuildings\\BlackSmith"), new Point(3, 3))
                    //    ));
                    //gameRoot._shop.AddItem(new ShopItem(
                    //    "Nuclear Reactor",
                    //    "Generates energy",
                    //    GBL.Content.Load<Texture2D>("Textures\\SpecialBuildings\\Nuclear Reactor"),
                    //    cost: 150,
                    //    building: new BuildingType("Nuclear Reactor", GBL.Content.Load<Texture2D>("Textures\\SpecialBuildings\\Nuclear Reactor"), new Point(3, 3))
                    //));
                    break;

                case 2:
                    gameRoot.buildingSystem.SetLimit("House", 6);
                    gameRoot.buildingSystem.SetLimit("Barracks", 3);

                    gameRoot._shop.AddItem(new ShopItem(
                        "House",
                        "Increases Population",
                        GBL.Content.Load<Texture2D>("Textures\\Houses\\House1"),
                        cost: 50,
                        building: new BuildingType("House", GBL.Content.Load<Texture2D>("Textures\\Houses\\House1"), new Point(2, 2))
                    ));
                    // Add more items or higher limits for level 1
                    break;
            }
        }

    }
}
