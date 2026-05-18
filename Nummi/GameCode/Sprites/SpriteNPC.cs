using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    // Class for the NPCs
    public class SpriteNPC : SpriteCharacter
    {
        protected bool _isTalking = false;
        protected float _talkingDuration = 2f;
        protected float _talkingCooldown = 0f;
        protected float _speechTimer = 0f;
        protected float _walkingArea = 50f;
        protected bool _canInteract = false;
        protected float _spawnPlace;
        protected float _walkingTime;

        public SpriteNPC(Game1 gameRoot, Texture2D texture, Vector2 position, bool canMove, float speechTimer, float walkingArea, float walkingTime)
            : base(gameRoot, texture, position, canMove)
        {
            _canFlip = true;
            _layerDepth = 0.31f;
            _spawnPlace = position.X;
            _texture = texture;
            _talkingDuration = speechTimer;
            _walkingArea = walkingArea;
            _walkingTime = walkingTime;
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
            // If the speech timer is above 0, the NPC is talking and can't move. Once the speech timer runs out, the NPC can move again but there is a cooldown before they can talk again.
            // probably could be optimized by just using the talking cooldown and not having a separate speech timer, but this works fine for now
            if (_speechTimer > 0f)
            {
                _isTalking = true;
                _velocity.X = 0f;
                SetAnimation(0);
            }
            else
            {
                _isTalking = false;
                _talkingCooldown -= GBL.DeltaTime;
            }
            // If the NPC can move and isn't talking, they will walk back and forth in a sine wave pattern. The walking area and time can be adjusted in the constructor.
            if (_canMove && !_isTalking)
            {
                _velocity.X = _walkingArea * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds / _walkingTime) * 0.5f;
                if (_position.X < _spawnPlace - _walkingArea) _position.X = _spawnPlace - _walkingArea;
                if (_position.X > _spawnPlace + _walkingArea) _position.X = _spawnPlace + _walkingArea;
                SetAnimation(1);
            }

            if (_collisionBounds.Intersects(_gameRoot._player._collisionBounds) && _talkingCooldown <= 0f)
            {
                _canInteract = true;

                // Only open dialog if one isn't already open
                if (GBL.KeyPress(Keys.E) && (_gameRoot._box == null || _gameRoot._box.Dead))
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
        // When the player interacts with the NPC, the speech timer is set to the talking duration, the talking cooldown is set to 2 seconds, and a dialog box is created with a message. The player can't move while the dialog box is open.
        public void DialogueTrigger()
        {
            _speechTimer = _talkingDuration;
            _talkingCooldown = 2f;
            _gameRoot._player._canMove = false;
            _gameRoot._box = new DialogBox(
                _gameRoot,
                "Hello there!\nWelcome to the world of Nummi!",
                "Feel free to explore and talk to other characters!");
            _gameRoot._newSpriteList.Add(_gameRoot._box);
        }
    }
}
