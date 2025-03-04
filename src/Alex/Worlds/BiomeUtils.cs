﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Alex.Common.Utils;
using Microsoft.Xna.Framework;
using MathF = System.MathF;

namespace Alex.Worlds
{
	public class Biome
	{
		private static readonly Color DefaultWaterColor    = ColorHelper.HexToColor("#44AFF5");
		private static readonly Color DefaultWaterFogColor = ColorHelper.HexToColor("#44AFF5");
		private static readonly Color DefaultFogColor = ColorHelper.HexToColor("#ABD2FF");

		public                  int    Id;
		public                  string Name;
		public                  float  Temperature;
		public                  float  Downfall;
		
		public float  MinHeight = 0.1f;
		public float  MaxHeight = 0.3f;

		public Color FogColor                 { get; set; } = DefaultFogColor;
		public Color Water                    { get; set; } = DefaultWaterColor;
		
		private bool _waterFogColorSet = false;
		private Color _waterFogColor = DefaultWaterFogColor;

		public Color WaterFogColor
		{
			get
			{
				return _waterFogColorSet ? _waterFogColor : Water;
			}
			set
			{
				_waterFogColorSet = true;
				_waterFogColor = value;
			}
		}

		public float WaterFogDistance         { get; set; } = 15f;
		public float WaterSurfaceTransparency { get; set; } = 0.65f;

		//public float HeightScale = 100;
	}

	public class BiomeUtils
	{
		public static Biome[] Biomes =
		{
			new Biome
			{
				Id = 0,
				Name = "Ocean",
				Temperature = 0.5f,
				Downfall = 0.5f,
				MinHeight = -1f,
				MaxHeight = 0.4f,
				
				Water = ColorHelper.HexToColor("#1787D4"),
				WaterFogDistance = 60
				//	SurfaceBlock = 12,
				//	SoilBlock = 24
			}, // default values of temp and rain
			new Biome
			{
				Id = 1,
				Name = "Plains",
				Temperature = 0.8f,
				Downfall = 0.4f,
				MinHeight = 0.0125f,
				MaxHeight = 0.5f, //TODO
				Water = ColorHelper.HexToColor("#44AFF5")
			},
			new Biome
			{
				Id = 2,
				Name = "Desert",
				Temperature = 2.0f,
				Downfall = 0.0f,
				MinHeight = 0.1f,
				MaxHeight = 0.2f,
				Water = ColorHelper.HexToColor("#32A598")
			},
			new Biome
			{
				Id = 3,
				Name = "Extreme Hills",
				Temperature = 0.2f,
				Downfall = 0.3f,
				MinHeight = 0.2f,
				MaxHeight = 1f,
				Water = ColorHelper.HexToColor("#007BF7")
			},
			new Biome
			{
				Id = 4,
				Name = "Forest",
				Temperature = 0.7f,
				Downfall = 0.8f,
				MinHeight = 0.1f, //TODO
				MaxHeight = 0.2f,
				Water = ColorHelper.HexToColor("#1E97F2")
			},
			new Biome
			{
				Id = 5,
				Name = "Taiga",
				Temperature = 0.05f,
				Downfall = 0.8f,
				MinHeight = 0.1f,
				MaxHeight = 0.4f,
				Water = ColorHelper.HexToColor("#287082")
			},
			new Biome
			{
				Id = 6,
				Name = "Swampland",
				Temperature = 0.8f,
				Downfall = 0.9f,
				MinHeight = -0.2f,
				MaxHeight = 0.1f,
				Water = ColorHelper.HexToColor("#4c6559"),
				WaterFogDistance = 8
			},
			new Biome
			{
				Id = 7,
				Name = "River",
				Temperature = 0.5f,
				Downfall = 0.5f,
				MinHeight = -0.5f,
				MaxHeight = 0f,
				Water = ColorHelper.HexToColor("#0084FF"),
				WaterFogDistance = 30
			}, // default values of temp and rain
			new Biome
			{
				Id = 8,
				Name = "Nether",
				Temperature = 2.0f,
				Downfall = 0.0f,
				MinHeight = 0.1f,
				MaxHeight = 0.2f, //TODO!
				Water = ColorHelper.HexToColor("#905957")
			},
			new Biome
			{
				Id = 9,
				Name = "End",
				Temperature = 0.5f,
				Downfall = 0.5f,
				MinHeight = 0.1f,
				MaxHeight = 0.2f, //TODO!
				Water = ColorHelper.HexToColor("#62529e")
			}, // default values of temp and rain
			new Biome
			{
				Id = 10,
				Name = "Frozen Ocean",
				Temperature = 0.0f,
				Downfall = 0.5f,
				MinHeight = -1f,
				MaxHeight = 0.5f,
				Water = ColorHelper.HexToColor("#2570B5"),
				WaterFogDistance = 20
			},
			new Biome
			{
				Id = 11,
				Name = "Frozen River",
				Temperature = 0.0f,
				Downfall = 0.5f,
				MinHeight = -0.5f,
				MaxHeight = 0f,
				Water = ColorHelper.HexToColor("#185390")
			},
			new Biome
			{
				Id = 12,
				Name = "Ice Plains",
				Temperature = 0.0f,
				Downfall = 0.5f,
				MinHeight = 0.125f,
				MaxHeight = 0.5f, //TODO
				Water = ColorHelper.HexToColor("#14559b")
			},
			new Biome
			{
				Id = 13,
				Name = "Ice Mountains",
				Temperature = 0.0f,
				Downfall = 0.5f,
				MinHeight = 0.2f,
				MaxHeight = 1.2f,
				Water = ColorHelper.HexToColor("#1156a7")
			},
			new Biome
			{
				Id = 14,
				Name = "Mushroom Island",
				Temperature = 0.9f,
				Downfall = 1.0f,
				MinHeight = 0.2f,
				MaxHeight = 1f,
				Water = ColorHelper.HexToColor("#8a8997")
			},
			new Biome
			{
				Id = 15,
				Name = "Mushroom Island Shore",
				Temperature = 0.9f,
				Downfall = 1.0f,
				MinHeight = -1f,
				MaxHeight = 0.1f,
				Water = ColorHelper.HexToColor("#818193")
			},
			new Biome
			{
				Id = 16,
				Name = "Beach",
				Temperature = 0.8f,
				Downfall = 0.4f,
				MinHeight = 0f,
				MaxHeight = 0.1f,
				Water = ColorHelper.HexToColor("#157cab"),
				WaterFogDistance = 60
			},
			new Biome
			{
				Id = 17,
				Name = "Desert Hills",
				Temperature = 2.0f,
				Downfall = 0.0f,
				MinHeight = 0.2f,
				MaxHeight = 0.7f,
				Water = ColorHelper.HexToColor("#1a7aa1")
			},
			new Biome
			{
				Id = 18,
				Name = "Forest Hills",
				Temperature = 0.7f,
				Downfall = 0.8f,
				MinHeight = 0.2f,
				MaxHeight = 0.6f,
				Water = ColorHelper.HexToColor("#056bd1")
			},
			new Biome
			{
				Id = 19,
				Name = "Taiga Hills",
				Temperature = 0.2f,
				Downfall = 0.7f,
				MinHeight = 0.2f,
				MaxHeight = 0.7f,
				Water = ColorHelper.HexToColor("#236583")
			},
			new Biome
			{
				Id = 20,
				Name = "Extreme Hills Edge",
				Temperature = 0.2f,
				Downfall = 0.3f,
				MinHeight = 0.2f,
				MaxHeight = 0.8f,
				Water = ColorHelper.HexToColor("#045cd5")
			},
			new Biome
			{
				Id = 21,
				Name = "Jungle",
				Temperature = 1.2f,
				Downfall = 0.9f,
				MinHeight = 0.1f,
				MaxHeight = 0.4f,
				Water = ColorHelper.HexToColor("#14A2C5")
			},
			new Biome
			{
				Id = 22,
				Name = "Jungle Hills",
				Temperature = 1.2f,
				Downfall = 0.9f,
				MinHeight = 0.2f,
				MaxHeight = 1.8f,
				Water = ColorHelper.HexToColor("#1B9ED8")
			},
			
			//TODO: The rest of min/max
			new Biome
			{
				Id = 23,
				Name = "Jungle Edge",
				Temperature = 0.95f,
				Downfall = 0.8f,
				MinHeight = 0.1f,
				MaxHeight = 0.2f,
				Water = ColorHelper.HexToColor("#0D8AE3")
			},
			new Biome
			{
				Id = 24,
				Name = "Deep Ocean",
				Temperature = 0.5f,
				Downfall = 0.5f,
				MinHeight = -1.8F,
				MaxHeight = 0.1f,
				Water = ColorHelper.HexToColor("#1787D4"),
				WaterFogDistance = 60
			},
			new Biome
			{
				Id = 25,
				Name = "Stone Beach",
				Temperature = 0.2f,
				Downfall = 0.3f,
				MinHeight = 0.1f,
				MaxHeight = 0.8f,
				Water = ColorHelper.HexToColor("#0d67bb")
			},
			new Biome
			{
				Id = 26,
				Name = "Cold Beach",
				Temperature = 0.05f,
				Downfall = 0.3f,
				MinHeight = 0f,
				MaxHeight = 0.025f,
				Water = ColorHelper.HexToColor("#1463a5"),
				WaterFogDistance = 50
			},
			new Biome
			{
				Id = 27,
				Name = "Birch Forest",
				Temperature = 0.6f,
				Downfall = 0.6f,
				MinHeight = 0.1f,
				MaxHeight = 0.2f,
				Water = ColorHelper.HexToColor("#0677ce")
			},
			new Biome
			{
				Id = 28,
				Name = "Birch Forest Hills",
				Temperature = 0.6f,
				Downfall = 0.6f,
				MinHeight = 0.35f,
				MaxHeight = 0.45f,
				Water = ColorHelper.HexToColor("#0677ce")
			},
			new Biome
			{
				Id = 29,
				Name = "Roofed Forest",
				Temperature = 0.7f,
				Downfall = 0.8f,
				MinHeight = 0.1f,
				MaxHeight = 0.2f,
				Water = ColorHelper.HexToColor("#3B6CD1")
			},
			new Biome
			{
				Id = 30,
				Name = "Cold Taiga",
				Temperature = -0.5f,
				Downfall = 0.4f,
				MinHeight = 0.2f,
				MaxHeight = 0.2f,
				Water = ColorHelper.HexToColor("#205e83")
			},
			new Biome
			{
				Id = 31,
				Name = "Cold Taiga Hills",
				Temperature = -0.5f,
				Downfall = 0.4f,
				MinHeight = 0.3f,
				MaxHeight = 0.45f,
				Water = ColorHelper.HexToColor("#245b78")
			},
			new Biome
			{
				Id = 32,
				Name = "Mega Taiga",
				Temperature = 0.3f,
				Downfall = 0.8f,
				MinHeight = 0.2f,
				MaxHeight = 0.2f,
				Water = ColorHelper.HexToColor("#2d6d77")
			},
			new Biome
			{
				Id = 33,
				Name = "Mega Taiga Hills",
				Temperature = 0.3f,
				Downfall = 0.8f,
				MinHeight = 0.3f,
				MaxHeight = 0.45f,
				Water = ColorHelper.HexToColor("#286378")
			},
			new Biome
			{
				Id = 34,
				Name = "Extreme Hills+",
				Temperature = 0.2f,
				Downfall = 0.3f,
				MinHeight = 0.5f,
				MaxHeight = 1f,
				Water = ColorHelper.HexToColor("#0E63AB")
			},
			new Biome
			{
				Id = 35,
				Name = "Savanna",
				Temperature = 1.2f,
				Downfall = 0.0f,
				MinHeight = 0.005f,
				MaxHeight = 0.125f,
				Water = ColorHelper.HexToColor("#2C8B9C")
			},
			new Biome
			{
				Id = 36,
				Name = "Savanna Plateau",
				Temperature = 1.0f,
				Downfall = 0.0f,
				MinHeight = 0.025f,
				MaxHeight = 1.5f,
				Water = ColorHelper.HexToColor("#2590A8")
			},
			new Biome
			{
				Id = 37,
				Name = "Mesa",
				Temperature = 2.0f,
				Downfall = 0.0f,
				MinHeight = 0.1f,
				MaxHeight = 0.2f,
				Water = ColorHelper.HexToColor("#4E7F81")
			},
			new Biome
			{
				Id = 38,
				Name = "Mesa Plateau F",
				Temperature = 2.0f,
				Downfall = 0.0f,
				MinHeight = 1.5f,
				MaxHeight = 0.25f,
				Water = ColorHelper.HexToColor("#55809E")
			},
			new Biome
			{
				Id = 39,
				Name = "Mesa Plateau",
				Temperature = 2.0f,
				Downfall = 0.0f,
				MinHeight = 1.5f,
				MaxHeight = 0.025f,
				Water = ColorHelper.HexToColor("#55809E")
			},
			new Biome()
			{
				Id = 46,
				Name = "Cold Ocean",
				Temperature = 0.5f,
				Water = ColorHelper.HexToColor("#2080C9"),
				WaterFogDistance = 60
			}, 
			new Biome()
			{
				Id = 49,
				Name = "Cold Deep Ocean",
				Temperature = 0.5f,
				Water = ColorHelper.HexToColor("#2080C9"),
				WaterFogDistance = 60
			}, 
			new Biome {Id = 127, Name = "The Void", Temperature = 0.8f, Downfall = 0.4f},
			new Biome {Id = 128, Name = "Unknown Biome", Temperature = 0.8f, Downfall = 0.4f},
			new Biome {Id = 129, Name = "Sunflower Plains", Temperature = 0.8f, Downfall = 0.4f},
			new Biome
			{
				Id = 130,
				Name = "Desert M",
				Temperature = 2.0f,
				Downfall = 0.0f
			},
			new Biome
			{
				Id = 131,
				Name = "Extreme Hills M",
				Temperature = 0.2f,
				Downfall = 0.3f,
				MinHeight = 0.2f,
				MaxHeight = 0.8f
			},
			new Biome {Id = 132, Name = "Flower Forest", Temperature = 0.7f, Downfall = 0.8f, Water = ColorHelper.HexToColor("#20A3CC")},
			new Biome {Id = 133, Name = "Taiga M", Temperature = 0.05f, Downfall = 0.8f},
			new Biome {Id = 134, Name = "Swampland M", Temperature = 0.8f, Downfall = 0.9f, WaterFogDistance = 8},
			new Biome {Id = 140, Name = "Ice Plains Spikes", Temperature = 0.0f, Downfall = 0.5f},
			new Biome {Id = 149, Name = "Jungle M", Temperature = 1.2f, Downfall = 0.9f},
			new Biome {Id = 150, Name = "Unknown Biome", Temperature = 0.8f, Downfall = 0.4f},
			new Biome {Id = 151, Name = "JungleEdge M", Temperature = 0.95f, Downfall = 0.8f},
			new Biome {Id = 155, Name = "Birch Forest M", Temperature = 0.6f, Downfall = 0.6f},
			new Biome
			{
				Id = 156,
				Name = "Birch Forest Hills M",
				Temperature = 0.6f,
				Downfall = 0.6f,
				MinHeight = 0.2f,
				MaxHeight = 0.8f
			},
			new Biome {Id = 157, Name = "Roofed Forest M", Temperature = 0.7f, Downfall = 0.8f},
			new Biome {Id = 158, Name = "Cold Taiga M", Temperature = -0.5f, Downfall = 0.4f},
			new Biome {Id = 160, Name = "Mega Spruce Taiga", Temperature = 0.25f, Downfall = 0.8f},
			// special exception, temperature not 0.3
			new Biome
			{
				Id = 161,
				Name = "Mega Spruce Taiga Hills",
				Temperature = 0.3f,
				Downfall = 0.8f,
				MinHeight = 0.2f,
				MaxHeight = 0.8f
			},
			new Biome {Id = 162, Name = "Extreme Hills+ M", Temperature = 0.2f, Downfall = 0.3f},
			new Biome {Id = 163, Name = "Savanna M", Temperature = 1.2f, Downfall = 0.0f, Water = ColorHelper.HexToColor("#2C8B9C")},
			new Biome {Id = 164, Name = "Savanna Plateau M", Temperature = 1.0f, Downfall = 0.0f, Water = ColorHelper.HexToColor("#2590A8")},
			new Biome {Id = 165, Name = "Mesa (Bryce)", Temperature = 2.0f, Downfall = 0.0f},
			new Biome {Id = 166, Name = "Mesa Plateau F M", Temperature = 2.0f, Downfall = 0.0f},
			new Biome {Id = 167, Name = "Mesa Plateau M", Temperature = 2.0f, Downfall = 0.0f},
		};

		private struct BiomeCorner
		{
			public int Red;
			public int Green;
			public int Blue;
		}

		internal static void FixMinMaxHeight()
		{
			float minTemp = float.MaxValue;
			float maxTemp = float.MinValue;
			float minRain = float.MaxValue;
			float maxRain = float.MinValue;
			for (int i = 0; i < Biomes.Length; i++)
			{
				var biome = Biomes[i];
				var min = MathF.Min(biome.MinHeight, biome.MaxHeight);
				var max = MathF.Max(biome.MinHeight, biome.MaxHeight);

				biome.MinHeight = min;
				biome.MaxHeight = max;

				if (biome.Temperature < minTemp)
				{
					minTemp = biome.Temperature;
				}
				if (biome.Temperature > maxTemp)
				{
					maxTemp = biome.Temperature;
				}
				if (biome.Downfall < minRain)
				{
					minRain = biome.Downfall;
				}
				if (biome.Downfall > maxRain)
				{
					maxRain = biome.Downfall;
				}

				Biomes[i] = biome;
			}
			Debug.WriteLine($"Temperature (min: {minTemp} max: {maxTemp}) Downfall (min:{minRain} max: {maxRain})");
		}

		//$c = self::interpolateColor(256, $x, $z, [0x47, 0xd0, 0x33], [0x6c, 0xb4, 0x93], [0xbf, 0xb6, 0x55], [0x80, 0xb4, 0x97]);

		//private static BiomeCorner[] PEgrassCorners = new BiomeCorner[3]
		//{
		//	new BiomeCorner {red = 0xbf, green = 0xb6, blue = 0x55}, // lower left, temperature starts at 1.0 on left
		//	new BiomeCorner {red = 0x80, green = 0xb4, blue = 0x97}, // lower right
		//	new BiomeCorner {red = 0x47, green = 0xd0, blue = 0x33} // upper left
		//};

		private static BiomeCorner[] grassCorners = new BiomeCorner[3]
		{
			new BiomeCorner {Red = 191, Green = 183, Blue = 85}, // lower left, temperature starts at 1.0 on left
			new BiomeCorner {Red = 128, Green = 180, Blue = 151}, // lower right
			new BiomeCorner {Red = 71, Green = 205, Blue = 51} // upper left
		};

		private static BiomeCorner[] foliageCorners = new BiomeCorner[3]
		{
			new BiomeCorner {Red = 174, Green = 164, Blue = 42}, // lower left, temperature starts at 1.0 on left
			new BiomeCorner {Red = 96, Green = 161, Blue = 123}, // lower right
			new BiomeCorner {Red = 26, Green = 191, Blue = 0} // upper left
		};

		public static float Clamp(float value, float min, float max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}

		// NOTE: elevation is number of meters above a height of 64. If elevation is < 64, pass in 0.
		private int BiomeColor(float temperature, float rainfall, int elevation, BiomeCorner[] corners)
		{
			// get UVs
			temperature = Clamp(temperature - elevation * 0.00166667f, 0.0f, 1.0f);
			// crank it up: temperature = clamp(temperature - (float)elevation*0.166667f,0.0f,1.0f);
			rainfall = Clamp(rainfall, 0.0f, 1.0f);
			rainfall *= temperature;

			// UV is essentially temperature, rainfall

			// lambda values for barycentric coordinates
			float[] lambda = new float[3];
			lambda[0] = temperature - rainfall;
			lambda[1] = 1.0f - temperature;
			lambda[2] = rainfall;

			float red = 0.0f, green = 0.0f, blue = 0.0f;
			for (int i = 0; i < 3; i++)
			{
				red += lambda[i] * corners[i].Red;
				green += lambda[i] * corners[i].Green;
				blue += lambda[i] * corners[i].Blue;
			}

			int r = (int)Clamp(red, 0.0f, 255.0f);
			int g = (int)Clamp(green, 0.0f, 255.0f);
			int b = (int)Clamp(blue, 0.0f, 255.0f);

			return (r << 16) | (g << 8) | b;
		}

		private int BiomeGrassColor(float temperature, float rainfall, int elevation)
		{
			return BiomeColor(temperature, rainfall, elevation, grassCorners);
		}

		private int BiomeFoliageColor(float temperature, float rainfall, int elevation)
		{
			return BiomeColor(temperature, rainfall, elevation, foliageCorners);
		}

		public void PrecomputeBiomeColors()
		{
			for (int biome = 0; biome < Biomes.Length; biome++)
			{
				//Biomes[biome].Grass = ComputeBiomeColor(biome, 0, true);
				//Biomes[biome].Foliage = ComputeBiomeColor(biome, 0, false);
			}

			//var mesaGrass = GetBiome(37).grass;
			//var desertGrass = GetBiome(2).grass;
			//if(mesaGrass != desertGrass) throw new Exception("Mesa: " + mesaGrass + " Desert: " + desertGrass);
		}

		private const int FOREST_BIOME = 4;
		private const int SWAMPLAND_BIOME = 6;
		private const int FOREST_HILLS_BIOME = 18;
		private const int BIRCH_FOREST_BIOME = 27;
		private const int BIRCH_FOREST_HILLS_BIOME = 28;
		private const int ROOFED_FOREST_BIOME = 29;

		private const int MESA_BIOME = 37;
		private const int MESA_PLATEAU_F_BIOME = 38;
		private const int MESA_PLATEAU_BIOME = 39;


		// elevation == 0 means for precomputed colors and for elevation off
		// or 64 high or below. 
		public int ComputeBiomeColor(int biome, int elevation, bool isGrass)
		{
			int color;

			switch (biome)
			{
				case SWAMPLAND_BIOME:
					// the fefefe makes it so that carries are copied to the low bit,
					// then their magic "go to green" color offset is added in, then
					// divide by two gives a carry that will nicely go away.
					// old method:
					//color = BiomeGrassColor( gBiomes[biome].temperature, gBiomes[biome].rainfall );
					//gBiomes[biome].grass = ((color & 0xfefefe) + 0x4e0e4e) / 2;
					//color = BiomeFoliageColor( gBiomes[biome].temperature, gBiomes[biome].rainfall );
					//gBiomes[biome].foliage = ((color & 0xfefefe) + 0x4e0e4e) / 2;

					// new method:
					// yes, it's hard-wired in. It actually varies with temperature:
					//         return temperature < -0.1D ? 0x4c763c : 0x6a7039;
					// where temperature is varied by PerlinNoise, but I haven't recreated the
					// PerlinNoise function yet. Rich green vs. sickly swamp brown. I'm going with brown.
					return 0x6a7039;

				// These are actually perfectly normal. Only sub-type 3, roofed forest, is different.
				//case FOREST_BIOME:	// forestType 0
				//case FOREST_HILLS_BIOME:	// forestType 0
				//case BIRCH_FOREST_BIOME:	// forestType 2
				//case BIRCH_FOREST_HILLS_BIOME:	// forestType 2
				//	break;

				case ROOFED_FOREST_BIOME: // forestType 3
					if (isGrass)
					{
						color = BiomeGrassColor(GetBiome(biome).Temperature, GetBiome(biome).Downfall, elevation);
						// the fefefe makes it so that carries are copied to the low bit,
						// then their magic "go to green" color offset is added in, then
						// divide by two gives a carry that will nicely go away.
						return ((color & 0xfefefe) + 0x28340a) / 2;
					}
					else
					{
						return BiomeFoliageColor(GetBiome(biome).Temperature, GetBiome(biome).Downfall, elevation);
					}

				case MESA_BIOME:
				case MESA_PLATEAU_F_BIOME:
				case MESA_PLATEAU_BIOME:
					// yes, it's hard-wired
					return isGrass ? 0x90814d : 0x9e814d;

				default:
					return isGrass ? BiomeGrassColor(GetBiome(biome).Temperature, GetBiome(biome).Downfall, elevation) :
						BiomeFoliageColor(GetBiome(biome).Temperature, GetBiome(biome).Downfall, elevation);
			}
		}

		public static Biome GetBiome(int biomeId)
		{
			Biome first = null;
			foreach (var biome in Biomes)
			{
				if (biome.Id == biomeId)
				{
					first = biome;
					break;
				}
			}

			return first ?? new Biome { Id = biomeId };
		}
		public int BiomeSwampRiverColor(int color)
		{
			int r = (int)((color >> 16) & 0xff);
			int g = (int)((color >> 8) & 0xff);
			int b = (int)color & 0xff;

			// swamp color modifier is 0xE0FFAE
			r = (r * 0xE0) / 255;
			// does nothing: g=(g*0xFF)/255;
			b = (b * 0xAE) / 255;
			color = (r << 16) | (g << 8) | b;

			return color;
		}

		public static Biome GetEdgeBiome(Biome biome)
		{
			if (biome.Id == 21 || biome.Id == 22) //Jungle or Jungle Hills
			{
				return GetBiome(23); //Return Jungle Edge
			}
			else if (biome.MaxHeight >= 0.8f)
			{
				return GetBiome(20); //Extreme hills edge.
			}

			return biome;
		}

		public static Dictionary<Biome, double> GetBiomes(float temp, float rain)
		{
			if (temp < -1f || temp > 2f || rain < 0f || rain > 1f)
				Debug.WriteLine($"Temp: {temp} Rain: {rain}");

			//return Biomes.Where(x => x.Id != 8 && x.Id != 9 && x.Id <= 39).OrderBy(x => GetSquaredDistance(x, temp, rain)).Take(3).ToArray();

			//	Debug.WriteLine($"Temp: {temp} Rain: {rain}");
			double threshold = 1000.0;
			Dictionary<Biome, double> biomes = new Dictionary<Biome, double>(3);

			Biome closestBiome = null, secondClosestBiome = null, thirdClosestBiome = null;
			double closestDist = 10000000, secondClosestDist = 10000000, thirdClosestDist = 10000000;

			foreach (Biome biome in Biomes.Where(x => x.Id != 8 && x.Id != 9).OrderBy(x => GetSquaredDistance(x, temp, rain)).Take(3))
			{
				double dist = GetSquaredDistance(biome, temp, rain);

				if (dist < closestDist)
				{
					thirdClosestDist = secondClosestDist; thirdClosestBiome = secondClosestBiome;
					secondClosestDist = closestDist; secondClosestBiome = closestBiome;
					closestDist = dist; closestBiome = biome;
				}

				else if (dist < secondClosestDist)
				{
					if (dist >= threshold) continue; //We don't want to calculate the noise values for biomes that have almost no influence
					thirdClosestDist = secondClosestDist; thirdClosestBiome = secondClosestBiome;
					secondClosestDist = dist; secondClosestBiome = biome;
				}

				else if (dist < thirdClosestDist)
				{
					if (dist >= threshold) continue;
					thirdClosestDist = dist; thirdClosestBiome = biome;
				}
			}

			biomes.Add(closestBiome, closestDist);
			if (secondClosestBiome != null) biomes.Add(secondClosestBiome, secondClosestDist);
			if (thirdClosestBiome != null) biomes.Add(thirdClosestBiome, thirdClosestDist);

			return biomes;

		}

		public static Biome GetBiome(float temp, float rain)
		{
			if (temp < -1f || temp > 2f || rain < 0f || rain > 1f)
				Debug.WriteLine($"Temp: {temp} Rain: {rain}");

			return ClosestTo(Biomes, temp, rain);
		}

		private static Biome ClosestTo(IEnumerable<Biome> collection, float targetTemp, float targetRain)
		{
			Biome closest = null;
			float closestDistance = float.MaxValue;
			foreach (var element in collection)
			{
				var distance = GetSquaredDistance(element, targetTemp, targetRain);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					closest = element;
				}
			}

			//Debug.WriteLine($"Closest biome to temp: {targetTemp} rain: {targetRain} is {closest.Name}, distance: {closestDistance}");
			return closest;
		}

		private static float GetSquaredDistance(Biome biome, float temp, float rain)
		{
			return MathF.Abs((biome.Temperature - temp) * (biome.Temperature - temp) + (biome.Downfall - rain) * (biome.Downfall - rain));
		}
	}
}
