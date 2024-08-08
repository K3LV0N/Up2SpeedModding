using Terraria;
using Terraria.ModLoader;

namespace Up2SpeedModding.Buffs
{
    public class BurnyStuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            // set the dubuff status here to true
            // our debuff status thing here is the burnyStuffDebuff bool in the player class don below
            player.GetModPlayer<BurnyStuffPlayer>().burnyStuffDebuff = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            // THIS PART IS OPTIONAL FOR NPCS

            // Firt we must get rid of any current regeneration (if we want to fully remove any regen
            // However this can be avoided if we just want a flat decrease to regen, which could result
            // in negative regen
            if (npc.lifeRegen > 0)
            {
                npc.lifeRegen = 0;
            }

            // THIS PART IS OPTIONAL FOR NPCS


            // lifeRegen is 1/2 per second (for some reason) so this -= 16 results in -8 health per second
            npc.lifeRegen -= 8;
        }
    }

    public class BurnyStuffPlayer : ModPlayer
    {
        // We use this bool to keep track of whether the debuff is currently applied or not
        public bool burnyStuffDebuff;

        public override void ResetEffects()
        {
            // Whenever the player is reset (AKA dies or something), the debuff should be removed
            burnyStuffDebuff = false;
        }

        public override void UpdateBadLifeRegen()
        {

            // Here we check to see if the debuff is currently active
            if (burnyStuffDebuff)
            {
                // Firt we must get rid of any current regeneration (if we want to fully remove any regen
                // However this can be avoided if we just want a flat decrease to regen, which could result
                // in negative regen
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                   

                // Player.lifeRegenTime is used to increase the speed in which you reach maximum natural life regeneration.
                // So setting it to 0 resets the timer which disallows natural regen while this debuff is active
                Player.lifeRegenTime = 0;

                // lifeRegen is 1/2 per second (for some reason) so this -= 16 results in -8 health per second
                Player.lifeRegen -= 8;
            }
        }
    }

}
