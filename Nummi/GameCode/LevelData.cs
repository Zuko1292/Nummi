using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nummi;
using System;
using System.Diagnostics;

namespace Nummi
{
    public static class LevelData
    {

        public static int LastLevelIndex = 3;

        // Rules for Heads town and Tails Town and Dungeon 1 section 1 and 2 (which share the same tileSet)
        public static readonly TilemapRules Rules1 = new TilemapRules()
            .AddSolid(4, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 21, 23, 24, 25, 26, 27, 28, 29, 30, 31)
            .AddExit(14, 22)
            .AddChest(7)
            .AddTrapDoor(3);


        public static void SpawnLevel(int level, Game1 gameRoot)
        {
            
            switch (gameRoot._gameState)
            {

                case GameState.HeadsLevel:
                    gameRoot._currentLevel = level;
                    gameRoot._spriteList.Clear();
                    gameRoot._newSpriteList.Clear();

                    CharacterStats savedStats = gameRoot._player?.Stats ?? new CharacterStats(str: 1, vit: 1);
                    LevelSystem savedLevelSystem = gameRoot._player?.LevelSystem ?? new LevelSystem();

                    gameRoot._player = null;

                    switch (level)
                    {
                        case 0:

                            gameRoot._isTrapLevel = false;

                            gameRoot._useLighting = false;
                            gameRoot._torchPositions = Array.Empty<Vector2>();

                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[0]);
                            gameRoot._tilemap.SetRules(Rules1);

                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(45, 45), true);
                            gameRoot._spriteList.Add(gameRoot._player);

                            gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Slime Anim-Sheet"), TilePos(30, 45)));

                            gameRoot._spriteList.Add(new HeadsHouse(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Houses\\House2_v2"), TilePos(14, 18)));
                            gameRoot._spriteList.Add(new HeadsHouse(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Houses\\House1_v2"), TilePos(20, 18)));
                            gameRoot._spriteList.Add(new HeadsHouse(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Houses\\House3_v2"), TilePos(26, 18)));

                            gameRoot._spriteList.Add(new SpriteNPC(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Player_SpriteSheet"), TilePos(26, 21), true, 3f, 85f, 0.98f));
                            gameRoot._spriteList.Add(new SpriteNPC(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Player_SpriteSheet"), TilePos(14, 20), true, 3f, 75f, 1.02f));
                            gameRoot._spriteList.Add(new SpriteNPC(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Player_SpriteSheet"), TilePos(20, 23), true, 3f, 50f, 1.05f));

                            gameRoot._levelBackground = new Background(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Backgrounds\\HeadsLevelBackgroundPlaceholder"));
                            gameRoot._spriteList.Add(gameRoot._levelBackground);

                            // gameRoot._spriteList.Add(new SpriteNPC(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Player_SpriteSheet"), TilePos(12, 5), true, 3f));
                            break;
                        case 1:
                            gameRoot._isTrapLevel = true;

                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[2]);
                            gameRoot._tilemap.SetRules(Rules1);

                            gameRoot._useLighting = false;
                            gameRoot._torchPositions = Array.Empty<Vector2>();

                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(6, 8), true, savedStats, savedLevelSystem);
                            gameRoot._spriteList.Add(gameRoot._player);

                            //// Room 2 Enemies
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Slime Anim-Sheet"),TilePos(32, 8)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(33, 11)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(33, 5)));
                            //
                            //// Room 3 Enemies
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(49, 11)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Slime Anim-Sheet"), TilePos(48, 4)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(58, 12)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(56, 7)));
                            //
                            //// Room 4 Enemies
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(43, 18)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(49, 20)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(46, 27)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(43, 24)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(43, 30)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(51, 27)));
                            //
                            //// Trap Room Enemies
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(53, 35)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Slime Anim-Sheet"), TilePos(53, 37)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(53, 39)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(53, 41)));
                            //
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(59, 35)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet"), TilePos(59, 37)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Slime Anim-Sheet"), TilePos(59, 39)));
                            //gameRoot._spriteList.Add(new Slime(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet"), TilePos(59, 41)));


                            break;
                        case 2:

                            gameRoot._isTrapLevel = true;

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
                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(9, 8), true, savedStats, savedLevelSystem);
                            gameRoot._spriteList.Add(gameRoot._player);

                            //// Room 2 Enemies
                            //gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(26, 5)));
                            //gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(26, 11)));
                            //
                            //// Room 3 Enemies
                            //gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(46, 14)));
                            //gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(52, 7)));
                            //
                            //// Room 4 Enemies
                            //gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(46, 20)));
                            //gameRoot._spriteList.Add(new BigOrangeSlime(gameRoot, TilePos(51, 28)));
                            //
                            //// Trap Room Enemies
                            //gameRoot._spriteList.Add(new TallPurpleSlime(gameRoot, TilePos(56, 42)));

                            break;
                        case 3:
                            gameRoot._bossDead = false;
                            gameRoot._isTrapLevel = false;

                            gameRoot._useLighting = false;
                            gameRoot._torchPositions = Array.Empty<Vector2>();

                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[4]);
                            gameRoot._tilemap.SetRules(Rules1);
                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(9, 3), true, savedStats, savedLevelSystem);
                            gameRoot._spriteList.Add(gameRoot._player);

                            gameRoot._spriteList.Add(new PossessedTree(gameRoot, TilePos(10, 14)));
                            break;
                    }
                    break;
                case GameState.TailsLevel:
                    gameRoot._currentLevel = level + LastLevelIndex;

                    gameRoot._spriteList.Clear();
                    gameRoot._newSpriteList.Clear();

                    gameRoot._player = null;
                    switch (level)
                    {
                        case 0:
                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[1]);
                            SetBuildingLimits(gameRoot, 0);

                            gameRoot._grid.ResetBuildable(); // Clear old data first
                            GenerateBuildableFromTilemap(gameRoot._grid, gameRoot._tilemap, 0);

                            gameRoot._spriteList.Add(new Grid(gameRoot, new Vector2(1024 + (5 * 32), 1024)));

                            gameRoot._levelBackground = new Background(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Backgrounds\\MainMenuBackgroundPlaceholder"));
                            gameRoot._spriteList.Add(gameRoot._levelBackground);
                            break;
                    }
                    break;
            }
        }

        public static Vector2 TilePos(Point tile, Point gridSize = default, bool centred = true)
        {
            if (gridSize == default) gridSize = new Point(32, 32);

            if (centred) return new Vector2(tile.X * 32 + gridSize.X / 2, tile.Y * 32 + gridSize.Y / 2);
            return new Vector2(tile.X * gridSize.X, tile.Y * gridSize.Y);
        }

        public static Vector2 TilePos(int X, int Y)
        {
            return TilePos(new Point(X, Y));
        }

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

            switch (tailsLevel)
            {
                case 0:
                    gameRoot.buildingSystem.SetLimit("House", 3);
                    gameRoot.buildingSystem.SetLimit("Barracks", 1);
                    break;
                case 1:
                    gameRoot.buildingSystem.SetLimit("House", 6);
                    gameRoot.buildingSystem.SetLimit("Barracks", 3);
                    break;
            }
        }

    }
}
