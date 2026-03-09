using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nummi.GameCode.Sprites;
using System.Diagnostics;

namespace Nummi
{
    public static class LevelData
    {

        public static int LastLevelIndex = 1;

        public static void SpawnLevel(int level, Game1 gameRoot)
        {
            gameRoot._spriteList.Clear();
            gameRoot._newSpriteList.Clear();

            gameRoot._player = null;
            switch (gameRoot._gameState)
            {
                case GameState.HeadsLevel:
                    gameRoot._currentLevel = level;

                    switch (level)
                    {
                        case 0:

                            gameRoot._tilemap = Tilemap.FromFile(gameRoot.levelFiles[gameRoot._currentLevel]);

                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(8, 6), true);
                            gameRoot._spriteList.Add(gameRoot._player);

                            gameRoot._levelBackground = new Background(gameRoot, GBL.Content.Load<Texture2D>("HeadsLevelBackgroundPlaceholder"));
                            gameRoot._spriteList.Add(gameRoot._levelBackground);

                            break;
                        case 1:
                            gameRoot._player = new SpritePlayer(gameRoot, TilePos(3, 6), true);
                            gameRoot._spriteList.Add(gameRoot._player);
                            break;
                    }
                    break;
                case GameState.TailsLevel:
                    gameRoot._currentLevel = level;
                    switch(level)
                    {
                        case 0:
                            // Put the Tails level logic here
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
