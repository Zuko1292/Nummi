using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    // Class for dropped weapons, which are collectable items that the player can pick up to gain new weapons. The class inherits from SpriteCollectable, which means it has all the properties and methods of a collectable sprite, such as collision detection and animation handling. The DroppedWeapon class adds a weapon type property to determine which weapon it represents, and a pickup cooldown to prevent the player from picking it up immediately after it spawns.
    // When changing weapons literally just change weapon type variable and that changes the animation
    // TODO make the other weapons animations
    public class DroppedWeapon : SpriteCollectable
    {
        int _weaponType;
        public float _pickupCD = 1.5f;

        public DroppedWeapon(Game1 gameRoot, Vector2 position, int WeaponType)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Weapons-SpriteSheet"), position)
        {
            _weaponType = WeaponType;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {        
            _frameDuration = 1f / 8f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sword
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 16, 16));

            // Great Sword
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 16, 24, 24));

            // Mace
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(16, 0, 16, 16));

            // Great Hammer 
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(24, 16, 24, 24));

            // Bow
            animations.Add(new List<Rectangle>());
            animations[4].Add(new Rectangle(32, 0, 16, 16));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            SetAnimation(_weaponType);

            _pickupCD -= GBL.DeltaTime;
        }
    }
}
