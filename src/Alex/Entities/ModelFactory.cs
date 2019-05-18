using Alex.ResourcePackLib.Json.Models.Entities;

namespace Alex.Entities
{
	public static class ModelFactory
	{
		public static bool TryGetModel(string geometry, out EntityModel model)
		{
			if (geometry.Equals("geometry.npc"))
			{
				model = new Models.NpcModel();
				return true;
			}

			if (geometry.Equals("geometry.bed"))
			{
				model = new Models.BedModel();
				return true;
			}

			if (geometry.Equals("geometry.cape"))
			{
				model = new Models.CapeModel();
				return true;
			}

			if (geometry.Equals("geometry.wolf"))
			{
				model = new Models.WolfModel();
				return true;
			}

			if (geometry.Equals("geometry.agent"))
			{
				model = new Models.AgentModel();
				return true;
			}

			if (geometry.Equals("geometry.dragon"))
			{
				model = new Models.DragonModel();
				return true;
			}

			if (geometry.Equals("geometry.trident"))
			{
				model = new Models.TridentModel();
				return true;
			}

			if (geometry.Equals("geometry.humanoid"))
			{
				model = new Models.HumanoidModel();
				return true;
			}

			if (geometry.Equals("geometry.guardian"))
			{
				model = new Models.GuardianModel();
				return true;
			}

			if (geometry.Equals("geometry.mob_head"))
			{
				model = new Models.MobHeadModel();
				return true;
			}

			if (geometry.Equals("geometry.quadruped"))
			{
				model = new Models.QuadrupedModel();
				return true;
			}

			if (geometry.Equals("geometry.irongolem"))
			{
				model = new Models.IrongolemModel();
				return true;
			}

			if (geometry.Equals("geometry.player_head"))
			{
				model = new Models.PlayerHeadModel();
				return true;
			}

			if (geometry.Equals("geometry.dragon_head"))
			{
				model = new Models.DragonHeadModel();
				return true;
			}

			if (geometry.Equals("geometry.zombie.drowned"))
			{
				model = new Models.ZombieDrownedModel();
				return true;
			}

			if (geometry.Equals("geometry.chalkboard_small"))
			{
				model = new Models.ChalkboardSmallModel();
				return true;
			}

			if (geometry.Equals("geometry.chalkboard_large"))
			{
				model = new Models.ChalkboardLargeModel();
				return true;
			}

			if (geometry.Equals("geometry.chalkboard_medium"))
			{
				model = new Models.ChalkboardMediumModel();
				return true;
			}

			if (geometry.Equals("geometry.humanoid.customSlim"))
			{
				model = new Models.HumanoidCustomslimModel();
				return true;
			}

			if (geometry.Equals("geometry.humanoid.custom"))
			{
				model = new Models.HumanoidCustomGeometryHumanoidModel();
				return true;
			}

			if (geometry.Equals("geometry.bat"))
			{
				model = new Models.BatModel();
				return true;
			}

			if (geometry.Equals("geometry.cat"))
			{
				model = new Models.CatModel();
				return true;
			}

			if (geometry.Equals("geometry.cod"))
			{
				model = new Models.CodModel();
				return true;
			}

			if (geometry.Equals("geometry.cow"))
			{
				model = new Models.CowModel();
				return true;
			}

			if (geometry.Equals("geometry.pig"))
			{
				model = new Models.PigModel();
				return true;
			}

			if (geometry.Equals("definition.vex"))
			{
				model = new Models.VexModel();
				return true;
			}

			if (geometry.Equals("geometry.blaze"))
			{
				model = new Models.BlazeModel();
				return true;
			}

			if (geometry.Equals("geometry.ghast"))
			{
				model = new Models.GhastModel();
				return true;
			}

			if (geometry.Equals("geometry.horse"))
			{
				model = new Models.HorseModel();
				return true;
			}

			if (geometry.Equals("geometry.llama"))
			{
				model = new Models.LlamaModel();
				return true;
			}

			if (geometry.Equals("geometry.panda"))
			{
				model = new Models.PandaModel();
				return true;
			}

			if (geometry.Equals("geometry.slime"))
			{
				model = new Models.SlimeModel();
				return true;
			}

			if (geometry.Equals("geometry.squid"))
			{
				model = new Models.SquidModel();
				return true;
			}

			if (geometry.Equals("definition.boat"))
			{
				model = new Models.BoatModel();
				return true;
			}

			if (geometry.Equals("definition.husk"))
			{
				model = new Models.HuskModel();
				return true;
			}

			if (geometry.Equals("geometry.ocelot"))
			{
				model = new Models.OcelotModel();
				return true;
			}

			if (geometry.Equals("geometry.parrot"))
			{
				model = new Models.ParrotModel();
				return true;
			}

			if (geometry.Equals("geometry.rabbit"))
			{
				model = new Models.RabbitModel();
				return true;
			}

			if (geometry.Equals("geometry.salmon"))
			{
				model = new Models.SalmonModel();
				return true;
			}

			if (geometry.Equals("geometry.shield"))
			{
				model = new Models.ShieldModel();
				return true;
			}

			if (geometry.Equals("geometry.spider"))
			{
				model = new Models.SpiderModel();
				return true;
			}

			if (geometry.Equals("geometry.turtle"))
			{
				model = new Models.TurtleModel();
				return true;
			}

			if (geometry.Equals("definition.sheep"))
			{
				model = new Models.SheepModel();
				return true;
			}

			if (geometry.Equals("definition.skull"))
			{
				model = new Models.SkullModel();
				return true;
			}

			if (geometry.Equals("definition.stray"))
			{
				model = new Models.StrayModel();
				return true;
			}

			if (geometry.Equals("definition.witch"))
			{
				model = new Models.WitchModel();
				return true;
			}

			if (geometry.Equals("geometry.chicken"))
			{
				model = new Models.ChickenModel();
				return true;
			}

			if (geometry.Equals("geometry.creeper"))
			{
				model = new Models.CreeperModel();
				return true;
			}

			if (geometry.Equals("geometry.dolphin"))
			{
				model = new Models.DolphinModel();
				return true;
			}

			if (geometry.Equals("geometry.phantom"))
			{
				model = new Models.PhantomModel();
				return true;
			}

			if (geometry.Equals("geometry.shulker"))
			{
				model = new Models.ShulkerModel();
				return true;
			}

			if (geometry.Equals("definition.evoker"))
			{
				model = new Models.EvokerModel();
				return true;
			}

			if (geometry.Equals("definition.wither"))
			{
				model = new Models.WitherModel();
				return true;
			}

			if (geometry.Equals("definition.zombie"))
			{
				model = new Models.ZombieModel();
				return true;
			}

			if (geometry.Equals("geometry.cow.v1.8"))
			{
				model = new Models.CowV18Model();
				return true;
			}

			if (geometry.Equals("geometry.enderman"))
			{
				model = new Models.EndermanModel();
				return true;
			}

			if (geometry.Equals("geometry.horse.v2"))
			{
				model = new Models.HorseV2Model();
				return true;
			}

			if (geometry.Equals("geometry.pig.v1.8"))
			{
				model = new Models.PigV18Model();
				return true;
			}

			if (geometry.Equals("geometry.pillager"))
			{
				model = new Models.PillagerModel();
				return true;
			}

			if (geometry.Equals("geometry.skeleton"))
			{
				model = new Models.SkeletonModel();
				return true;
			}

			if (geometry.Equals("geometry.vex.v1.8"))
			{
				model = new Models.VexV18Model();
				return true;
			}

			if (geometry.Equals("geometry.villager"))
			{
				model = new Models.VillagerModel();
				return true;
			}

			if (geometry.Equals("definition.drowned"))
			{
				model = new Models.DrownedModel();
				return true;
			}

			if (geometry.Equals("geometry.endermite"))
			{
				model = new Models.EndermiteModel();
				return true;
			}

			if (geometry.Equals("geometry.llamaspit"))
			{
				model = new Models.LlamaspitModel();
				return true;
			}

			if (geometry.Equals("geometry.lavaslime"))
			{
				model = new Models.LavaslimeModel();
				return true;
			}

			if (geometry.Equals("geometry.mooshroom"))
			{
				model = new Models.MooshroomModel();
				return true;
			}

			if (geometry.Equals("geometry.polarbear"))
			{
				model = new Models.PolarbearModel();
				return true;
			}

			if (geometry.Equals("geometry.snowgolem"))
			{
				model = new Models.SnowgolemModel();
				return true;
			}

			if (geometry.Equals("definition.minecart"))
			{
				model = new Models.MinecartModel();
				return true;
			}

			if (geometry.Equals("geometry.llama.v1.8"))
			{
				model = new Models.LlamaV18Model();
				return true;
			}

			if (geometry.Equals("geometry.silverfish"))
			{
				model = new Models.SilverfishModel();
				return true;
			}

			if (geometry.Equals("geometry.vindicator"))
			{
				model = new Models.VindicatorModel();
				return true;
			}

			if (geometry.Equals("geometry.witherBoss"))
			{
				model = new Models.WitherbossModel();
				return true;
			}

			if (geometry.Equals("geometry.armor_stand"))
			{
				model = new Models.ArmorStandModel();
				return true;
			}

			if (geometry.Equals("geometry.bow_standby"))
			{
				model = new Models.BowStandbyModel();
				return true;
			}

			if (geometry.Equals("geometry.evoker.v1.8"))
			{
				model = new Models.EvokerV18Model();
				return true;
			}

			if (geometry.Equals("geometry.ocelot.v1.8"))
			{
				model = new Models.OcelotV18Model();
				return true;
			}

			if (geometry.Equals("geometry.rabbit.v1.8"))
			{
				model = new Models.RabbitV18Model();
				return true;
			}

			if (geometry.Equals("geometry.slime.armor"))
			{
				model = new Models.SlimeArmorModel();
				return true;
			}

			if (geometry.Equals("geometry.spider.v1.8"))
			{
				model = new Models.SpiderV18Model();
				return true;
			}

			if (geometry.Equals("geometry.stray.armor"))
			{
				model = new Models.StrayArmorModel();
				return true;
			}

			if (geometry.Equals("geometry.villager_v2"))
			{
				model = new Models.VillagerV2Model();
				return true;
			}

			if (geometry.Equals("geometry.zombie.v1.8"))
			{
				model = new Models.ZombieV18Model();
				return true;
			}

			if (geometry.Equals("definition.magma_cube"))
			{
				model = new Models.MagmaCubeModel();
				return true;
			}

			if (geometry.Equals("definition.pufferfish"))
			{
				model = new Models.PufferfishModel();
				return true;
			}

			if (geometry.Equals("geometry.shulker.v1.8"))
			{
				model = new Models.ShulkerV18Model();
				return true;
			}

			if (geometry.Equals("definition.cave_spider"))
			{
				model = new Models.CaveSpiderModel();
				return true;
			}

			if (geometry.Equals("geometry.bow_pulling_0"))
			{
				model = new Models.BowPulling0Model();
				return true;
			}

			if (geometry.Equals("geometry.bow_pulling_1"))
			{
				model = new Models.BowPulling1Model();
				return true;
			}

			if (geometry.Equals("geometry.bow_pulling_2"))
			{
				model = new Models.BowPulling2Model();
				return true;
			}

			if (geometry.Equals("geometry.illager_beast"))
			{
				model = new Models.IllagerBeastModel();
				return true;
			}

			if (geometry.Equals("geometry.sheep.sheared"))
			{
				model = new Models.SheepShearedModel();
				return true;
			}

			if (geometry.Equals("geometry.skeleton.v1.8"))
			{
				model = new Models.SkeletonV18Model();
				return true;
			}

			if (geometry.Equals("geometry.tripod_camera"))
			{
				model = new Models.TripodCameraModel();
				return true;
			}

			if (geometry.Equals("geometry.villager.v1.8"))
			{
				model = new Models.VillagerV18Model();
				return true;
			}

			if (geometry.Equals("definition.ender_dragon"))
			{
				model = new Models.EnderDragonModel();
				return true;
			}

			if (geometry.Equals("definition.tropicalfish"))
			{
				model = new Models.TropicalfishModel();
				return true;
			}

			if (geometry.Equals("geometry.mooshroom.v1.8"))
			{
				model = new Models.MooshroomV18Model();
				return true;
			}

			if (geometry.Equals("geometry.pufferfish.mid"))
			{
				model = new Models.PufferfishMidModel();
				return true;
			}

			if (geometry.Equals("geometry.snowgolem.v1.8"))
			{
				model = new Models.SnowgolemV18Model();
				return true;
			}

			if (geometry.Equals("geometry.skeleton.stray"))
			{
				model = new Models.SkeletonStrayModel();
				return true;
			}

			if (geometry.Equals("geometry.tropicalfish_a"))
			{
				model = new Models.TropicalfishAModel();
				return true;
			}

			if (geometry.Equals("geometry.tropicalfish_b"))
			{
				model = new Models.TropicalfishBModel();
				return true;
			}

			if (geometry.Equals("geometry.pigzombie.v1.8"))
			{
				model = new Models.PigzombieV18Model();
				return true;
			}

			if (geometry.Equals("definition.zombie_pigman"))
			{
				model = new Models.ZombiePigmanModel();
				return true;
			}

			if (geometry.Equals("geometry.creeper.charged"))
			{
				model = new Models.CreeperChargedModel();
				return true;
			}

			if (geometry.Equals("geometry.vindicator.v1.8"))
			{
				model = new Models.VindicatorV18Model();
				return true;
			}

			if (geometry.Equals("geometry.skeleton.wither"))
			{
				model = new Models.SkeletonWitherModel();
				return true;
			}

			if (geometry.Equals("geometry.zombie.husk.v1.8"))
			{
				model = new Models.ZombieHuskV18Model();
				return true;
			}

			if (geometry.Equals("geometry.pufferfish.small"))
			{
				model = new Models.PufferfishSmallModel();
				return true;
			}

			if (geometry.Equals("geometry.pufferfish.large"))
			{
				model = new Models.PufferfishLargeModel();
				return true;
			}

			if (geometry.Equals("geometry.stray.armor.v1.8"))
			{
				model = new Models.StrayArmorV18Model();
				return true;
			}

			if (geometry.Equals("geometry.witherBoss.armor"))
			{
				model = new Models.WitherbossArmorModel();
				return true;
			}

			if (geometry.Equals("definition.wither_skeleton"))
			{
				model = new Models.WitherSkeletonModel();
				return true;
			}

			if (geometry.Equals("definition.zombie_villager"))
			{
				model = new Models.ZombieVillagerModel();
				return true;
			}

			if (geometry.Equals("geometry.sheep.sheared.v1.8"))
			{
				model = new Models.SheepShearedV18Model();
				return true;
			}

			if (geometry.Equals("geometry.zombie.villager_v2"))
			{
				model = new Models.ZombieVillagerV2Model();
				return true;
			}

			if (geometry.Equals("geometry.pufferfish.mid.v1.8"))
			{
				model = new Models.PufferfishMidV18Model();
				return true;
			}

			if (geometry.Equals("geometry.skeleton.stray.v1.8"))
			{
				model = new Models.SkeletonStrayV18Model();
				return true;
			}

			if (geometry.Equals("geometry.pigzombie.baby.v1.8"))
			{
				model = new Models.PigzombieBabyV18Model();
				return true;
			}

			if (geometry.Equals("geometry.skeleton.wither.v1.8"))
			{
				model = new Models.SkeletonWitherV18Model();
				return true;
			}

			if (geometry.Equals("geometry.zombie.villager.v1.8"))
			{
				model = new Models.ZombieVillagerV18Model();
				return true;
			}

			if (geometry.Equals("geometry.pufferfish.small.v1.8"))
			{
				model = new Models.PufferfishSmallV18Model();
				return true;
			}

			if (geometry.Equals("geometry.pufferfish.large.v1.8"))
			{
				model = new Models.PufferfishLargeV18Model();
				return true;
			}

			if (geometry.Equals("geometry.evoker"))
			{
				model = new Models.EvokerGeometryVillagerModel();
				return true;
			}

			if (geometry.Equals("geometry.sheep.v1.8"))
			{
				model = new Models.SheepV18GeometrySheepShearedV18Model();
				return true;
			}

			if (geometry.Equals("geometry.sheep"))
			{
				model = new Models.SheepGeometrySheepShearedModel();
				return true;
			}

			if (geometry.Equals("geometry.villager.witch.v1.8"))
			{
				model = new Models.VillagerWitchV18GeometryVillagerV18Model();
				return true;
			}

			if (geometry.Equals("geometry.villager.witch"))
			{
				model = new Models.VillagerWitchGeometryVillagerModel();
				return true;
			}

			model = null;
			return false;
		}
	}
}