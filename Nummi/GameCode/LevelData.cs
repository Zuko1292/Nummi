using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nummi;
using System.Diagnostics;

namespace Nummi
{
    public static class LevelData
    {

        public static int LastLevelIndex = 1;

        public static void SpawnLevel(int level, Game1 gameRoot)
        {
            
            switch (gameRoot._gameState)
            {

                case GameState.HeadsLevel:
                    gameRoot._currentLevel = level;
                    gameRoot._spriteList.Clear();
                    gameRoot._newSpriteList.Clear();

                    gameRoot._player = null;

                    switch (level)
                    {
                        case 0:

                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[0]);

                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(45, 45), true);
                            gameRoot._spriteList.Add(gameRoot._player);

                            gameRoot._levelBackground = new Background(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Backgrounds\\HeadsLevelBackgroundPlaceholder"));
                            gameRoot._spriteList.Add(gameRoot._levelBackground);

                            // gameRoot._spriteList.Add(new SpriteNPC(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Player_SpriteSheet"), TilePos(12, 5), true, 3f));

                            gameRoot._spriteList.Add(new Slime(gameRoot, TilePos(12, 5)));
                            break;
                        case 1:
                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[2]);

                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(3, 6), true);
                            gameRoot._spriteList.Add(gameRoot._player);

                            
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


    }
}
