using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class PossessedOakDrop : SpriteCollectable
    {
        public override bool Dead
        {
            set
            {
                _dead = value;
                OnPickup();
            }

            get
            {
                return _dead;
            }
        }

        public PossessedOakDrop(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\ShieldCrystal"), position)
        {

        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            //Crystal
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 32, 32));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        public void OnPickup()
        {
            _gameRoot._shop.AddItem(new ShopItem(
                        "Barracks",
                        "Increases Defense",
                        GBL.Content.Load<Texture2D>("Textures\\SpecialBuildings\\Barracks"),
                        cost: 150,
                        building: new BuildingType("Barracks", GBL.Content.Load<Texture2D>("Textures\\SpecialBuildings\\Barracks"), new Point(3, 3))
                    ));
        }
    }
}
