using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Up2SpeedModding.Items
{
	public class MultiTool : ModItem
	{
		// This is where we define the name and decription of the item!
		public override void SetStaticDefaults()
		{

			// DisplayName is the name of the item in the game
			// If you choose to not include this here, the name of 
			// the item will default to the class name (MultiTool)
			// with spaces put where capitals are. ex: "Multi Tool"
			DisplayName.SetDefault("Excaxaper");


			// This adds a translation for your name if you need it
			// This is currently being translated into spanish
			// The 5 in AddTranslation tells the function that
			// You can do the same for Tooltip
			// This translates to epic sword
			DisplayName.AddTranslation(5, "espada épica");


			// This is the desription of the item in the game
			// If not included there will be no desrciption
			Tooltip.SetDefault("This is an axe, pickaxe, hammer and sword. That's a lot!");

			Tooltip.AddTranslation(5, "Esto es un hacha, un pico, un martillo y una espada. ¡Eso es mucho!");
			
		}

		// This is where we set the stats of the item and such!
		// There are a LOT of values we can set here so I am not
		// going to use or mention all of them, however, the I picked
		// a bunch of commonly used ones so you can see what they do!
		public override void SetDefaults()
		{

            // If the item is an accessory or not
            //Item.accessory = true;

            // If the item is ammo and what type of ammo it is
            //Item.ammo = AmmoID.Bullet;

            // What this item shoots if it shoots things
            //Item.shoot = AmmoID.Bullet;

            // How fast this item shoots things
            //Item.shootSpeed = 10f;

            // How much life this heals you per use
            //Item.healLife = 5;

            // How much mana this restores per use
            //Item.healMana = 5;

            // How much mana it takes to use this item
            //Item.mana = 5;

            // If this item can autoswing
            Item.autoReuse = true;

			// How much axe power this item has
			Item.axe = 75;

            // How much pickaxe power this item has
            Item.pick = 75;

            // How much hammer power this item has
            Item.hammer = 75;

			// How this item is held when used
			Item.holdStyle = 1;

			// How many of this item you can hold in a stack
			Item.maxStack = 1;

			// If the item can do damage on contact or not
			Item.noMelee = false;

			// If this item is ammo or not
			Item.notAmmo = true;

			// How much damage this item does
			Item.damage = 50;

			// What type of damage this item does
			Item.DamageType = DamageClass.Melee;

			// How wide the item is
			Item.width = 40;

			// How tall the item is
			Item.height = 40;

			// How much time it takes to use this item
			Item.useTime = 20;

			// The animation of the item being used
			Item.useAnimation = 20;

			// How the item is used
			Item.useStyle = 1;

			// How much knockback the item has
			Item.knockBack = 6;

			// How much this item is worth in shops
			Item.value = 10000;

			// What color the item's name is
			Item.rare = 2;

			// What sound is used when the item is used
			Item.UseSound = SoundID.Item1;
		}

		// This is where we can add recipes for our item if we want!
		public override void AddRecipes()
		{
			// To start we need to make a new recipe object, 
			// I'll call this one r1 
			Recipe r1 = CreateRecipe();

			// next let's add an ingredient, this right here adds 2 dirt blocks
			// as an inredient
			r1.AddIngredient(ItemID.DirtBlock, 2);

			// next we'll add another ingredient, which is 1 wood
			r1.AddIngredient(ItemID.Wood, 1);

			// in total we need 2 dirt and 1 wood to make our item now!

			// next, let's make it so we need to make it at a work bench,
			// which we add with the AddTile function
			r1.AddTile(TileID.WorkBenches);

			// finally, let's finish our recipe by calling the Register function!
			r1.Register();

			// And just like that, our item has a recipe of 2 dirt, 1 wood that must
			// be made at a work bench!
		}
	}
}