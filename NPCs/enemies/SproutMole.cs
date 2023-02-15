using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using static IL.Terraria.WorldBuilding.Searches;

namespace Up2SpeedModding.NPCs.enemies
{
    // Here we are using an NPC from my Omori Mod (check it out)
    // to help describe an enemy
    internal class SproutMole : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // here we can add their name (similar to item)
            DisplayName.SetDefault("Sprout Mole");

            // This right here is how many frames are in the animation
            // AKA this tells the program how many times to split your
            // png!
            Main.npcFrameCount[NPC.type] = 9;
        }

        public override void SetDefaults()
        {
            // I don't know / can't find stuff about this one
            // I assume this means that the NPC will always have netUpdate
            // called on it for easy syncing, but I really don't know!
            //NPC.netAlways = true;

            // If this NPC flys or something (so that it doesn't have gravity!)
            //NPC.noGravity = true;

            // Allows this NPC to go through blocks!
            //NPC.noTileCollide = true;

            // Whether this NPC is townie or not
            //NPC.townNPC = false;

            // The current life of the enemy
            //NPC.life = 0;

            // The enemy's life regen if they have it
            //NPC.lifeRegen = 0;

            // I believe this makes it more likely for an NPC to despawn
            // although I am not entirely sure, so make sure to test and 
            // find our for yourself!
            //NPC.despawnEncouraged = true;

            // Where the frame's y position is
            // Same for other things such as X
            //NPC.frame.Y = 0;

            // A useful variable for keeping track of
            // animations, you usually don't want to set it here
            //NPC.frameCounter = 0;

            // width and height are the dimensions!
            NPC.width = 17;
            NPC.height = 30;

            // This is the MAX life of the enemy
            NPC.lifeMax = 40;

            // If you want an NPC to act like terraria's
            // predefined NPCs (such as a zombie) then set
            // the AI style to the style you wish.
            // If you want to make a custom AI, as we will here
            // set this value to -1
            // this should always be here!!!
            NPC.aiStyle = -1;

            // If an NPC is friendly or not, our enemy is obviously not!
            NPC.friendly = false;

            // Determines how much damage this NPC does
            NPC.damage = 10;

            // Determines how much defense this NPC has
            NPC.defense = 4;

            // This sound plays when the NPC is hit or takes damage
            NPC.HitSound = SoundID.NPCHit7;

            // This sound plays when the NPC dies
            NPC.DeathSound = SoundID.NPCDeath9;

            // This determines how much money the NPC will drop
            NPC.value = 10f;

            // This determines how much knockback the NPC takes
            NPC.knockBackResist = 0.5f;
            
            // This makes it so the NPC is synced, it only happens at
            // the next update, not all future updates, so if your
            // NPC has randomization at all, make sure to set netUpdate
            // to true in AI!
            NPC.netUpdate = true;
        }

        // This is where we can make enemies drop sweet sweet loot!
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // The first value in Common is the item to be dropped
            // The second is the denominator of the chance
            // So this enemy has a 1/3 chance of dropping a dirt block!
            npcLoot.Add(ItemDropRule.Common(ItemID.DirtBlock, 3));
        }


        // This is where we can dtermine our spawn chance of our NPC
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {

            // I make a variable here as the default spawn chance
            float spawnChance = .0f;

            // good chance of spawning if day on surface and underground
            if(Main.dayTime && spawnInfo.SpawnTileY == Main.worldSurface)
            {
                // if the time is day and we are at the surface, add some spawn chance
                spawnChance += .4f;
            }
            else if(spawnInfo.SpawnTileY == Main.rockLayer)
            {
                // if we are in the underground, but not too deep, add some spawn chance
                spawnChance += .4f;
            }

            // Finally, we have our end spawn chance which we return.
            // .4f is a decent spawn chance, in my testing they will spawn
            // around as often as slimes (If I remember correctly)
            return spawnChance;   
        }

        // I am defining some variables here in order to improve readabilty for the AI code
        private const int State_Docile = 0;
        private const int State_Surprise = 1;
        private const int State_Attack = 2;

        // This is what is going to be keeping track of our different AI states. States can
        // be anything like attacking, moving, or something else entirely
        // For this enemy, we have 3 states, Docile, Surprised, and Attack as seen above!
        public float AI_State
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        // This holds the total amount of ticks so we can appropriately time our state switching and
        // other ai needs!
        public float AI_Timer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        // These are 2 more variables we need in order to make this AI function correctly.
        public float AIPreviousXPosition;
        public bool jumping = false;

        // Here is the MEAT of the NPC's class. This defines how they move, attack and everything
        // in between. Writing AI's is tough work, but creating something new at the end is always
        // rewarding.

        // For this AI, we will make it so that our enemy wanders aimlessly until the player gets
        // close enough for the enemy to notice. When this happens, the enemy will face the player,
        // get surprised and jump. Then, when it lands, it will start charging at the player.
        // If our enemy gets stuck, it will also attempt to jump up to get out of any hole or crevice it is in
        public override void AI()
        {

            // For AI's that include randomness (this one does) you should always
            // include NPC.netUpdate = true to prevent desync bugs!!!
            // AKA this enemy might turn right for me, but left for you
            NPC.netUpdate = true;

            // If we have't noticed the player yet and the player hasn't attacked
            // If the player attacks from a range, they'll probably feel it!
            if (AI_State == State_Docile && !NPC.justHit)
            {
                // This makes it so the enemy faces the direction they are moving
                // instead of the player
                NPC.TargetClosest(false);
                if (AI_Timer == 0)
                {
                    // We set the direction of the enemy to a random direction to make them
                    // walk around aimlessly
                    bool leftOrRight = Main.rand.NextBool(2) ? true : false;

                    if(leftOrRight)
                    {
                        NPC.direction = 1;
                    }
                    else
                    {
                        NPC.direction = -1;
                    }
                }
                // increment the timer so we can make it so they turn around at points
                AI_Timer++;

                // 120 means 2 seconds here as terraria runs at 60 fps, which means we proceed to NOT do this
                // once every 2 seconds
                if(AI_Timer < 120)
                {
                    // we make the NPC's velocity equal to the direction they are facing, so they will
                    // walk slowly in that direction. The 8f is to keep pushing them into the ground so
                    // they don't start to fly if they go off a ledge, AKA it's gravity
                    NPC.velocity = new Vector2(NPC.direction, 8f);

                    // Now, if the NPC "sees" the player, and they are close enough, then it gets surprised
                    if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 500f)
                    {
                        // make the enemy surprised and set the timer to 0
                        AI_State = State_Surprise;
                        AI_Timer = 0;
                    }

                }
                else
                {
                    // reset the ai's timer so the enemy has the chance to pick a new direction
                    // to walk in
                    AI_Timer = 0;
                }
            }
            // The enemy is surprised now!
            else if (AI_State == State_Surprise || NPC.justHit)
            {
                // make them face the player
                NPC.TargetClosest(true);
                
                // Give them an intial upwards velocity boost (like they are jumping)
                if(AI_Timer == 0)
                {
                    NPC.velocity = new Vector2(0, -3f);
                }

                AI_Timer++;

                // add gravity over time
                if (NPC.velocity.Y <= 0)
                {
                    // we add a small amount of y gravity to simulate acceleration
                    NPC.velocity += new Vector2(0, 0.05f);
                }
                if(NPC.velocity.Y > 0)
                {
                    // Now we set the previous position (which we use later), get ready to attack and 
                    // set the timer to 0 again
                    AIPreviousXPosition = NPC.position.X;
                    AI_State = State_Attack;
                    AI_Timer = 0;
                }


            }
            else if (AI_State == State_Attack)
            {
                // Now we make the NPC target the player
                AI_Timer++;
                NPC.TargetClosest(true);
                NPC.GetTargetData(true);
                
                // if the player moves too far away from the enemy (around offscreen)
                // then they will stop chasing the player and become docile again,
                // effectively restarting the proccess
                if (Main.player[NPC.target].Distance(NPC.Center) > 800f)
                {
                    AI_State = State_Docile;
                }
                else
                {
                    // We add some stronger gravity to keep the enemy on the ground
                    NPC.velocity += new Vector2(0, .25f);

                    // this happens if the enemy decided to jump
                    if (jumping)
                    {
                        // use this to reduce the amount of code, instead of using multiple
                        // if statements
                        int multiplier = 1;
                        if(NPC.direction < 0)
                        {
                            multiplier = -1;
                        }
                        
                        // Accelerate towards the target
                        NPC.velocity += new Vector2(multiplier * .25f, 0);

                        // After 1/4th of a second, the jump is over
                        if(AI_Timer % 15 == 0)
                        {
                            jumping = false;
                        }
                        
                    }
                    // Ok! now this is how we can solve our "if stuck" problem
                    // basically, we wait a small bit of time, and if the enemy is in the same
                    // position vertically as it was around 1/3 of a second ago, it decides to jump
                    // to try to get unstuck
                    else if (AI_Timer > 20 && AI_Timer % 20 == 0)
                    {
                        // If it is "stuck"
                        if (NPC.position.X == AIPreviousXPosition)
                        {
                            // We set the velocity to be a large jump
                            NPC.velocity += new Vector2(NPC.direction * 4f, -8f);
                            // Make it so that we go to the jump portion next time
                            jumping = true;
                            // and reset our ai timer 
                            AI_Timer = 0;
                        }

                        // this is where we update our previous position (it updates every 1/3 of a second)
                        AIPreviousXPosition = NPC.position.X;

                    }
                    else
                    {
                        // same as in jumping
                        int multiplier = 1;

                        if(NPC.direction < 0)
                        {
                            multiplier = -1;
                        }

                        
                        // if the enemy hasn't reached the max horizontal speed of 4, but is faster than
                        // the walking speed (so it doesn't slide around like on ice when it turns)
                        if (Math.Abs(NPC.velocity.X) < 4 && Math.Abs(NPC.velocity.X) > 1)
                        {
                            // accelerate the enemy towards you
                            NPC.velocity += new Vector2(multiplier * 0.05f, 0);
                        }
                        else
                        {
                            // cap the speed at 4 to prevent this enemy from zooming around
                            if(multiplier > 0 && NPC.velocity.X >= 4)
                            {
                                NPC.velocity = new Vector2(4, NPC.velocity.Y);
                            }
                            // same as above but for the other direction
                            else if(multiplier < 0 && NPC.velocity.X <= -4)
                            {
                                NPC.velocity = new Vector2(-4, NPC.velocity.Y);
                            }
                            // Here this is where our Math.Abs(NPC.velocity.X) <= 1, so we
                            // accelerate the enemy a tad bit faster
                            else
                            {
                                NPC.velocity += new Vector2(multiplier * .08f, 0);
                            }
                            
                        }

                    }
                }
            }
        }

        // Wow! Did you make it through that AI function? Yeah it is quite long
        // and can get quite confusing, but I hope I provided enough context to
        // understand how to do the things that our enemy does and provide reasonable
        // explanation as to how we implement those

        // Now, time to move on to animation, like the states above, these names below are
        // just for clarity

        private const int frame1 = 0;
        private const int frame2 = 1;
        private const int frame3 = 2;
        private const int frame4 = 3;
        private const int frame5 = 4;
        private const int frame6 = 5;
        private const int frame7 = 6;
        private const int frame8 = 7;

        // This function is the heart of the animations
        public override void FindFrame(int frameHeight)
        {
            // We can set the direction of the sprite to face the correct direction
            NPC.spriteDirection = NPC.direction;

            // We start using frame counter here in order to change the frames at
            // set intervals, I personally used 1/6th of a second, you can do more
            // or less depending on your animation style!
            NPC.frameCounter++;

            // Here in the surprised state, the enemy doesn't need to be walking because it is
            // jumping, so we use the same frame. We also set the frameCounter to 30, to smoothly
            // continue the animation loop after the jump
            if(AI_State == State_Surprise)
            {
                NPC.frame.Y = frame3 * frameHeight;
                NPC.frameCounter = 30;
            }

            // This is the overall animation loop. It works as such. For 10 ticks we are on frame1
            // then we go to frame 2 for 10 ticks, then frame 3 for 10 ticks, and so on until we
            // get to frame 8. Here frame counter is 69 so we go until 79 (AKA < 80) where when
            // we go to 80, we set the frameCounter back and proceed to go through starting from
            // frame 1 again

            if(NPC.frameCounter < 10)
            {
                NPC.frame.Y = frame1 * frameHeight;
            }
            else if(NPC.frameCounter < 20)
            {
                NPC.frame.Y = frame2 * frameHeight;
            }
            else if (NPC.frameCounter < 30)
            {
                NPC.frame.Y = frame3 * frameHeight;
            }
            else if (NPC.frameCounter < 40)
            {
                NPC.frame.Y = frame4 * frameHeight;
            }
            else if (NPC.frameCounter < 50)
            {
                NPC.frame.Y = frame5 * frameHeight;
            }
            else if (NPC.frameCounter < 60)
            {
                NPC.frame.Y = frame6 * frameHeight;
            }
            else if (NPC.frameCounter < 70)
            {
                NPC.frame.Y = frame7 * frameHeight;
            }
            else if (NPC.frameCounter < 80)
            {
                NPC.frame.Y = frame8 * frameHeight;
            }
            else
            {
                NPC.frame.Y = frame1 * frameHeight;
                NPC.frameCounter = 0;
            }

        }
    }
}

// That is how you create a basic NPC!!!