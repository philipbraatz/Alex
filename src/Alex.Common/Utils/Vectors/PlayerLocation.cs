﻿using System;
using Alex.Common.Blocks;
using Microsoft.Xna.Framework;

namespace Alex.Common.Utils.Vectors
{
	public class PlayerLocation : ICloneable
	{
		private float _headYaw;
		private float _yaw;
		private float _pitch;
		
		public  float X { get; set; }
		public  float Y { get; set; }
		public  float Z { get; set; }

		public float Yaw
		{
			get => _yaw;
			set
			{
				_yaw = value;// FixValue(value);
			}
		}

		public float Pitch
		{
			get => _pitch;
			set
			{
				var pitch = value;
				_pitch = pitch;
			}
		}

		public float HeadYaw
		{
			get
			{
				return _headYaw;
			}
			set
			{
				_headYaw = value; //FixValue(value);
			}
		}

		float FixYaw(float value)
		{
			if (value < 0f)
				value = 360 + value;

			return value;
		}

		float FixValue(float value)
		{
			var val = value;

			if (val < 0f)
				val = 360f - (MathF.Abs(val) % 360f);
			else if (val > 360f)
				val = val % 360f;

			return val;
		}
		
		public   bool OnGround { get; set; }

		public PlayerLocation()
		{
		}

		public PlayerLocation(float x, float y, float z, float headYaw = 0f, float yaw = 0f, float pitch = 0f)
		{
			X = x;
			Y = y;
			Z = z;
			HeadYaw = headYaw;
			Yaw = yaw;
			Pitch = pitch;
		}

		public PlayerLocation(double x, double y, double z, float headYaw = 0f, float yaw = 0f, float pitch = 0f) : this((float)x, (float)y, (float)z, headYaw, yaw, pitch)
		{
		}

		public PlayerLocation(Vector3 vector, float headYaw = 0f, float yaw = 0f, float pitch = 0f) : this(vector.X, vector.Y, vector.Z, headYaw, yaw, pitch)
		{
		}

		public void SetPitchBounded(float pitch)
		{
			/*pitch = FixValue(pitch);

			if (pitch < 269.99f && pitch > 89.99f)
			{
				var max = MathF.Abs(270f - pitch);

				var min = MathF.Abs(90f - pitch);

				if (max < min)
				{
					pitch = 270.99f;
				}
				else if (min < max)
				{
					pitch = 89.99f;
				}
			}*/

			_pitch = Math.Clamp(pitch, -89.99f, 89.99f);
		}
		
		/*public PlayerLocation(MiNET.Utils.PlayerLocation p)
		{
			if (p == null) return;
			X = p.X;
			Y = p.Y;
			Z = p.Z;

			Yaw = p.Yaw;
			HeadYaw = p.HeadYaw;
			Pitch = p.Pitch;
		}*/

		public BlockCoordinates GetCoordinates3D()
		{
			return new BlockCoordinates((int)X, (int)Y, (int)Z);
		}

		public double DistanceTo(PlayerLocation other)
		{
			return Math.Sqrt(Square(other.X - X) +
							 Square(other.Y - Y) +
							 Square(other.Z - Z));
		}

		public double Distance(PlayerLocation other)
		{
			return Square(other.X - X) + Square(other.Y - Y) + Square(other.Z - Z);
		}

		private double Square(double num)
		{
			return num * num;
		}

		public Vector3 ToVector3()
		{
			return new Vector3(X, Y, Z);
		}

		public Vector3 GetDirection(bool includePitch = false, bool useHeadYaw = false)
		{
			Vector3 vector = Vector3.Backward;
			vector = Vector3.Transform(vector, GetDirectionMatrix(includePitch, useHeadYaw));
			
			return vector;
			
		//	vector.X = (-MathF.Sin(yaw) * MathF.Cos(pitch));
			//vector.Y = -MathF.Sin(pitch);
		//	vector.Z = (MathF.Cos(yaw) * MathF.Cos(pitch));

		//	return vector;
		}

		public Matrix GetDirectionMatrix(bool includePitch = false, bool useHeadYaw = false)
		{
			float pitch = (includePitch ? Pitch : 0f).ToRadians();
			float yaw   = ((useHeadYaw ? HeadYaw : Yaw)).ToRadians();

			return Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(yaw);
		}
		
		public static PlayerLocation operator *(PlayerLocation a, float b)
		{
			return new PlayerLocation(
				a.X * b,
				a.Y * b,
				a.Z * b,
				a.HeadYaw * b,
				a.Yaw * b,
				a.Pitch * b);
		}
		
		public static PlayerLocation operator +(PlayerLocation a, Vector3 b)
		{
			var (x, y, z) = b;

			return new PlayerLocation(
				a.X + x,
				a.Y + y,
				a.Z + z,
				a.HeadYaw,
				a.Yaw,
				a.Pitch)
			{
				OnGround = a.OnGround
			};
		}

		public static implicit operator Vector2(PlayerLocation a)
		{
			return new Vector2(a.X, a.Z);
		}

		public static implicit operator Vector3(PlayerLocation a)
		{
			return new Vector3(a.X, a.Y, a.Z);
		}

		public static implicit operator PlayerLocation(BlockCoordinates v)
		{
			return new PlayerLocation(v.X, v.Y, v.Z);
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public Matrix CalculateWorldMatrix(bool includePitch = false)
		{
			var dir = GetDirection(includePitch);
			return Matrix.CreateWorld(ToVector3(), dir, Vector3.Up);
		}

		public BlockFace GetFacing()
		{
			byte direction = (byte)((int)Math.Floor((HeadYaw * 4F) / 360F + 0.5D) & 0x03);

			BlockFace facing = BlockFace.North;
			switch (direction)
			{
				case 0: //East
					facing = BlockFace.North;

					break;

				case 1: //South
					facing = BlockFace.East;

					break;

				case 2: //West
					facing = BlockFace.South;

					break;

				case 3: //North
					facing = BlockFace.West;

					break;
			}

			return facing;
		}
		
		public string GetCardinalDirection()
		{
			double rotation = (HeadYaw) % 360;
			if (rotation < 0)
			{
				rotation += 360.0;
			}

			return GetDirection(rotation);
		}
		
		private static string GetDirection(double rotation)
		{
			if (0 <= rotation && rotation < 22.5)
			{
				return "South";
			}
			else if (22.5 <= rotation && rotation < 67.5)
			{
				return "South West";
			}
			else if (67.5 <= rotation && rotation < 112.5)
			{
				return "West";
			}
			else if (112.5 <= rotation && rotation < 157.5)
			{
				return "North West"; //
			}
			else if (157.5 <= rotation && rotation < 202.5)
			{
				return "North"; // 
			}
			else if (202.5 <= rotation && rotation < 247.5)
			{
				return "North East"; //
			}
			else if (247.5 <= rotation && rotation < 292.5)
			{
				return "East";
			}
			else if (292.5 <= rotation && rotation < 337.5)
			{
				return "South East";
			}
			else if (337.5 <= rotation && rotation < 360.0)
			{
				return "South";
			}
			else
			{
				return "N/A";
			}
		}
		
		public override string ToString()
		{
			return $"X={X}, Y={Y}, Z={Z}, HeadYaw={HeadYaw}, Yaw={Yaw}, Pitch={Pitch}";
		}
	}
}
