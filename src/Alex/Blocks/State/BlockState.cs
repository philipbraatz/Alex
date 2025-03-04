﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Alex.Blocks.Minecraft;
using Alex.Blocks.Properties;
using Alex.Common.Blocks.Properties;
using Alex.Common.Resources;
using Alex.ResourcePackLib.Json.BlockStates;
using Microsoft.Xna.Framework;
using NLog;

namespace Alex.Blocks.State
{
	public class BlockState : IEnumerable<IStateProperty>
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger(typeof(BlockState));
		
		protected internal HashSet<IStateProperty> States { get; set; }
		public int Count => States.Count;
		public BlockState()
		{
			States = new HashSet<IStateProperty>(new StatePropertyComparer());
		}

		public string Name { get; set; }
		public uint ID { get; set; }

		public BlockStateVariant ModelData { get; set; } = new BlockStateVariant()
		{
			new BlockStateModel()
		};
		
		public Block Block       { get; set; }
		public bool  Default     { get; set; } = false;

		public BlockStateVariantMapper VariantMapper { get; set; }

		public T GetValue<T>(StateProperty<T> property)
		{
			if (States.TryGetValue(property, out var first))
			{
				if (first is StateProperty<T> t)
				{
					return t.Value;
				}

				return property.ParseValue(first.StringValue);
			}

			return property.DefaultValue;
		}
		
		public BlockState WithProperty<T>(StateProperty<T> property, T value)
		{
			if (VariantMapper.TryResolve(this, property, value, out BlockState result))
			{
				return result;
			}

			if (LoggingConstants.LogInvalidBlockProperties)
				Log.Debug($"Invalid property on state {Name} ({property}={value})");
			
			return this;
		}
		
		public BlockState WithProperty(string property, string value)
		{
			if (VariantMapper.TryResolve(this, property, value, out BlockState result))
			{
				return result;
			}

			if (LoggingConstants.LogInvalidBlockProperties)
				Log.Debug($"Invalid property on state {Name} ({property}={value})");
			
			return this;
		}

		public bool TryGetValue(string property, out string value)
		{
			var hashcode = property.GetHashCode(StringComparison.InvariantCultureIgnoreCase);
			var first = States.FirstOrDefault(x => x.Identifier == hashcode);

			if (first != null)
			{
				value = first.StringValue;
				return true;
			}

			value = null;
			return false;
		}
		
		/*public bool TryGetValue<T>(StateProperty<T> property, out T value)
		{
			if (States.TryGetValue(property, out var first))
			{
				var v = first.Value;

				if (v is T t)
				{
					value = t;
				}
				else
				{
					value = property.ParseValue(first.StringValue);
				}

				return true;
			}

			value = default(T);
			return false;
		}
		
		public T GetTypedValue<T>(StateProperty<T> property)
		{
			if (TryGetValue(property, out var val))
			{
				return val;
			}

			return default(T);
		}*/

		public bool Equals(BlockState other)
		{
			bool result = Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase);
			if (!result) return false;

			//var thisStates = new HashSet<StateProperty>(States);
			var otherStates = new HashSet<IStateProperty>(other.States, new StatePropertyComparer());

			otherStates.IntersectWith(States);
			result = otherStates.Count == States.Count;

			return result;
		}

		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			
			return obj.GetHashCode().Equals(GetHashCode());
		}

		public override int GetHashCode()
		{
			var hash = new HashCode();
			//hash.Add(ID);
			hash.Add(Name);
			foreach (var state in States)
			{
				switch (state)
				{
					case PropertyByte blockStateByte:
						hash.Add(blockStateByte);
						break;
					case PropertyInt blockStateInt:
						hash.Add(blockStateInt);
						break;
					case PropertyString blockStateString:
						hash.Add(blockStateString);
						break;
					default:
						hash.Add(state);
						break;
				}
			}
			
			return hash.ToHashCode();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public IEnumerator<IStateProperty> GetEnumerator()
		{
			foreach (var kv in States)
			{
				yield return kv;
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			var v = States.ToArray();
			for (var index = 0; index < v.Length; index++)
			{
				var kv = v[index];
				sb.Append(kv.ToFormattedString());

				if (index != v.Length - 1)
					sb.Append(',');
			}

			return sb.ToString();
		}

		public string FormattedString => $"{Name}[{ToString()}]";
		
		private static readonly Regex VariantParser = new Regex("(?'property'[^=,]*?)=(?'value'[^,]*)", RegexOptions.Compiled);
		public static Dictionary<string, string> ParseData(string variant)
		{
			return VariantParser.Matches(variant).ToDictionary(
				x => x.Groups["property"].Value, x => x.Groups["value"].Value);
		}
	}
}
