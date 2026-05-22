using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    // Dungeon 2 "Big Dealer" - an anaconda in a nice suit.
    //
    // Frozen: always asleep. Player attacks bounce off harmlessly.
    // Thawed: not hostile. Player attacks bounce off and the dealer chuckles.
    //         Player can talk to it (E). Dialogue depends on how many keys
    //         the player is carrying (Game1._keys.Count). After collecting
    //         both keys, repeat conversations escalate in pedantry.
    public class Big_Dealer : SpriteNPC
    {
        public enum TempState
        {
            Frozen,
            Thawed
        }

        public TempState _tempState;

        // Counts how many times the player has spoken to the dealer while
        // holding 2+ keys, so dialogue can escalate.
        private int _postTwoKeysTalkCount = 0;

        // Cooldown so chuckles don't spam every collision frame.
        private float _chuckleCooldown = 0f;
        private const float ChuckleCooldownDuration = 1f;

        public Big_Dealer(Game1 gameRoot, Vector2 pos, TempState tempState)
            : base(gameRoot,
                   GBL.Content.Load<Texture2D>("Textures\\Animations\\Snake Dealer"),
                   pos,
                   canMove: false,
                   speechTimer: 3f,
                   walkingArea: 0f,
                   walkingTime: 1f,
                   dialogue: new List<string>() { "..." })
        {
            _drawScale = new Vector2(1.5f, 1.5f);
            _tempState = tempState;
            // SpriteNPC doesn't take damage from Attacks by default, so no
            // additional invincibility flag is needed here - attacks just
            // bounce off via the chuckle/growl reaction in OnCollideEvent.
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;
            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // 0 - Idle / Asleep (placeholder rectangles, swap for the real sheet)
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 32, 64));

            // 1 - Chuckle reaction (thawed) / sleepy growl (frozen)
            animations.Add(new List<Rectangle>());
               animations[1].Add(new Rectangle(0, 0, 32, 64));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);
            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            _chuckleCooldown -= GBL.DeltaTime;

            // The big dealer never self-propels and never moves.
            _velocity = Vector2.Zero;

            // Frozen: stay on the sleep animation. No interaction allowed.
            if (_tempState == TempState.Frozen)
            {
                if (_animIndex != 1) SetAnimation(0);
                base.Update(gameTime);
                return;
            }

            // Thawed: defer to SpriteNPC's interaction logic for the talk
            // prompt + E to open dialog. DialogueTrigger is overridden below
            // so the dialogue we hand the box depends on the key count.
            base.Update(gameTime);
        }

        // Build the dialogue lazily based on key count and prior talks.
        public override void DialogueTrigger()
        {
            _dialogue = BuildDialogueForCurrentState();
            base.DialogueTrigger();
        }

        private List<string> BuildDialogueForCurrentState()
        {
            int keys = _gameRoot._keys.Count;

            if (keys == 0)
            {
                return new List<string>
                {
                    "Hssss... lost, are we?",
                    "You'll need TWO keys to get through to the backroom.",
                    "Off you go."
                };
            }

            if (keys == 1)
            {
                return new List<string>
                {
                    "One key, eh? Halfway there.",
                    "Find ONE more and the backroom is yours."
                };
            }

            // keys >= 2 — escalating tiers of condescension on repeat talks.
            _postTwoKeysTalkCount++;
            switch (_postTwoKeysTalkCount)
            {
                case 1:
                    return new List<string>
                    {
                        "You have both keys. Why are you still standing here?",
                        "The door is RIGHT THERE."
                    };
                case 2:
                    return new List<string>
                    {
                        "Alright, let me explain this slowly.",
                        "You see that big rectangular shape in the wall?",
                        "That is called a 'door'. It has a 'handle'.",
                        "You take the keys, put them in the keyhole, and turn.",
                        "Then you walk through it. Got it?"
                    };
                case 3:
                    return new List<string>
                    {
                        "Step one: locate the door.",
                        "Step two: locate the keyhole (it's the small slot, NOT the handle).",
                        "Step three: insert key one. Rotate clockwise.",
                        "Step four: insert key two. Rotate clockwise again.",
                        "Step five: GRASP the handle. Apply downward pressure.",
                        "Step six: PUSH or PULL. Don't ask me which, figure it out.",
                        "Step seven: physically translate your body through the doorway.",
                        "Step eight: close the door behind you. Manners."
                    };
                default:
                    // Fourth+ time: the "Wikipedia article" treatment.
                    return new List<string>
                    {
                        "A door is a hinged or otherwise movable barrier",
                        "that allows ingress into and egress from an enclosure.",
                        "The created opening in the wall is a doorway or portal.",
                        "A door's essential and primary purpose is",
                        "to provide security by controlling access to the doorway.",
                        "Conventionally, it is a panel that fits into the doorway",
                        "of a building, room, or vehicle.",
                        "Doors are generally made of a material suited to",
                        "the door's task. They are commonly attached by hinges,",
                        "but can move by other means, such as slides or counterbalancing.",
                        "...are you still here? Just go through the door."
                    };
            }
        }

        // Bounce attacks off, chuckle (thawed) or growl (frozen).
        protected override void OnCollideEvent(Sprite otherSprite)
        {
            if (otherSprite is Attack && _chuckleCooldown <= 0f)
            {
                _chuckleCooldown = ChuckleCooldownDuration;
                SetAnimation(1);
            }

            // Skip base.OnCollideEvent so the SpriteNPC doesn't try to react.
        }

        protected override void OnAnimationFinished()
        {
            base.OnAnimationFinished();
            if (_animIndex == 1) SetAnimation(0); // back to idle/sleep
        }
    }
}
