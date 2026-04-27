using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Nummi;
using Code_For_Nummi;

namespace Nummi
{
    public class ShopSystem : Sprite
    {
        TextButton house;
        TextButton blacksmith;
        TextButton nuculearReactor;
        TextButton barracks;
        TextButton farmBuilding;

        public ShopSystem(Game1 gameRoot)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\UI\\Dialog Box"),
                   Vector2.Zero, false, false)
        {

        }


    }
}
