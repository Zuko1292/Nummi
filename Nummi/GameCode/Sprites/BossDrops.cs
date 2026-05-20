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

        public PossessedOakDrop(Game1 gameRoot, Texture2D texture, Vector2 position)
            : base(gameRoot, texture, position) //previously done using Textures\\Animations\\Crystal to set the crystal. TODO get texture to be right texture
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
            // unlocks crystal based on the level.
            if(_gameRoot._currentLevel == 0)
                _gameRoot._shop.AddItem(new ShopItem(
                        "Barracks",
                        "Increases Defense",
                        GBL.Content.Load<Texture2D>("Textures\\SpecialBuildings\\Barracks"),
                        cost: 150,
                        building: new BuildingType("Barracks", GBL.Content.Load<Texture2D>("Textures\\SpecialBuildings\\Barracks"), new Point(3, 3))
                    ));
            else if (_gameRoot._currentLevel == 1)
                _gameRoot._shop.AddItem(new ShopItem(
                        "Farm",
                        "Increases Health",
                        GBL.Content.Load<Texture2D>("Textures\\SpecialBuildings\\Farm Building"),
                        cost: 150,
                        building: new BuildingType("Farm", GBL.Content.Load<Texture2D>("Textures\\SpecialBuildings\\Farm Building"), new Point(3, 3))
                    ));
            else if (_gameRoot._currentLevel == 2)
                _gameRoot._shop.AddItem(new ShopItem(
                        "Black Smith",
                        "Increases Attack",
                        GBL.Content.Load<Texture2D>("Textures\\SpecialBuildings\\BlackSmith"),
                        cost: 150,
                        building: new BuildingType("Barracks", GBL.Content.Load<Texture2D>("Textures\\SpecialBuildings\\Barracks"), new Point(3, 3))
                    ));
        }
    }
}
