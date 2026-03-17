using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi.GameCode.Sprites;

namespace Nummi
{
    public class SpriteNPC : SpriteCharacter
    {
        protected bool _isTalking = false;
        protected float _talkingDuration = 2f;
        protected float _talkingCooldown = 2f;
        protected float _speechTimer = 0f;
        protected float _walkingArea = 50f;
        protected bool _canInteract = false;
        protected float _spawnPlace;

        public SpriteNPC(Game1 gameRoot, Texture2D texture, Vector2 position, bool canMove, float speechTimer)
            : base(gameRoot, texture, position, canMove)
        {
            _canFlip = true;
            _layerDepth = 0.31f;
            _spawnPlace = position.X;
            _texture = texture;
            _talkingDuration = speechTimer;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Idle animation
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 32, 32));
            animations[0].Add(new Rectangle(128, 0, 32, 32));

            // Walk animation
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 64, 32, 32));
            animations[1].Add(new Rectangle(32, 64, 32, 32));
            animations[1].Add(new Rectangle(64, 64, 32, 32));
            animations[1].Add(new Rectangle(96, 64, 32, 32));
            animations[1].Add(new Rectangle(128, 64, 32, 32));
            animations[1].Add(new Rectangle(160, 64, 32, 32));
            animations[1].Add(new Rectangle(192, 64, 32, 32));
            animations[1].Add(new Rectangle(224, 64, 32, 32));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            _speechTimer -= GBL.DeltaTime;

            if (_speechTimer > 0f)
            {
                _isTalking = true;
                _velocity.X = 0f;
                SetAnimation(0);
            }
            else
            {
                _isTalking = false; // Re-enable player movement after talking
                _talkingCooldown -= GBL.DeltaTime;
            }

            if(_canMove && !_isTalking)
            {
                // Simple back-and-forth movement within the walking area
                _velocity.X = _walkingArea * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) * 0.5f; // Oscillate between -walkingArea and +walkingArea

                if(_position.X < _spawnPlace - _walkingArea) _position.X = _spawnPlace - _walkingArea;
                if(_position.X > _spawnPlace + _walkingArea) _position.X = _spawnPlace + _walkingArea;
                SetAnimation(1);
            }

            if (_collisionBounds.Intersects(_gameRoot._player._collisionBounds) && !_canInteract && _talkingCooldown <= 0f)
            {
                _canInteract = true;
                if (GBL.KeyPress(Keys.E))
                {
                    DialogueTrigger();
                }
            }
            else
            {
                _canInteract = false;
            }

            base.Update(gameTime);
        }

        public void DialogueTrigger()
        {
            _speechTimer = _talkingDuration; // Reset speech timer to 3 seconds
            _talkingCooldown = 2f; // Set cooldown to prevent immediate re-triggering
            _gameRoot._player._canMove = false; // Disable player movement while talking
            _gameRoot._box = new DialogBox(_gameRoot, _position + new Vector2(0, -40), "Hello there!\nWelcome to the world of Nummi!", "Feel free to explore and talk to other characters!");
            _gameRoot._newSpriteList.Add(_gameRoot._box);
        }
    }
}
