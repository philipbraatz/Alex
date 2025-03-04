﻿using Alex.Common.Items;
using Alex.Common.Utils;
using Microsoft.Xna.Framework;

namespace Alex.Common.Blocks
{
	public interface IMaterial
	{
		string SoundCategory { get; }
		IMapColor MapColor { get; }
		TintType TintType { get; }
		Color TintColor { get; }

		double Slipperiness { get; }
		float Hardness { get; }
		bool BlocksLight { get; }
		bool BlocksMovement { get; }
		bool IsFlammable { get; }
		bool IsLiquid { get; }
		bool IsOpaque { get; }
		bool IsReplaceable { get; }
		bool IsSolid { get; }
		bool IsToolRequired { get; }
		bool IsWatterLoggable { get; }
		
		IMaterial WithSoundCategory(string soundCategory);
		IMaterial WithTintType(TintType type, Color color);
		IMaterial WithMapColor(IMapColor color, int index = 2);
		IMaterial WithSlipperiness(double value);
		IMaterial WithHardness(float hardness);

		IMaterial SetCollisionBehavior(BlockCollisionBehavior collisionBehavior);
		IMaterial SetReplaceable();
		IMaterial SetWaterLoggable();
		IMaterial SetTranslucent();
		IMaterial SetRequiresTool();
		IMaterial SetFlammable();
		
		bool CanUseTool(ItemType type, ItemMaterial material);
		IMaterial SetRequiredTool(ItemType type, ItemMaterial material);
		
		IMaterial Clone();
	}

	public interface IMapColor
	{
		int Index { get; }
		Color BaseColor { get; }
		Color GetMapColor(int index);

		IMapColor WithAlpha(byte alpha);
	}

	public enum BlockCollisionBehavior
	{
		None,
		Blocking
	}
}
