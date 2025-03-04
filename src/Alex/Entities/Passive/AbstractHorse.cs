using Alex.Graphics.Models.Entity;
using Alex.Networking.Java.Packets.Play;
using Alex.Worlds;
using MiNET.Entities;

namespace Alex.Entities.Passive
{
	public abstract class AbstractHorse : PassiveMob
	{
		protected static readonly string[] PartOfSaddle = new string[]
		{
			"Saddle",
			"SaddleMouthL",
			"SaddleMouthR",
			"SaddleMouthLine",
			"SaddleMouthLineR",
			"HeadSaddle"
		};
		public MiNET.Utils.UUID Owner { get; set; }
		
		public bool HasBred { get; set; }
		public bool IsEating { get; set; }
		public bool IsRearing { get; set; }
		public bool IsMouthOpen { get; set; }

		private bool _isSaddled = false;
		public bool IsSaddled
		{
			get
			{
				return _isSaddled;
			}
			set
			{
				_isSaddled = value;
				
				var modelRenderer = ModelRenderer;

				if (modelRenderer != null)
				{
					foreach (var bone in PartOfSaddle)
					{
						ModelRenderer.SetVisibility(bone, !value);
					}
				}
			}
		}

		/// <inheritdoc />
		protected AbstractHorse(EntityType type, World level) : base(level)
		{
			IsSaddled = false;
		}

		/// <inheritdoc />
		protected override void HandleJavaMeta(MetaDataEntry entry)
		{
			base.HandleJavaMeta(entry);

			if (entry.Index == 16 && entry is MetadataByte data)
			{
				IsTamed = (data.Value & 0x02) != 0;
				IsSaddled = (data.Value & 0x04) != 0;
				HasBred = (data.Value & 0x08) != 0;
				IsEating = (data.Value & 0x10) != 0;
				IsRearing = (data.Value & 0x20) != 0;
				IsMouthOpen = (data.Value & 0x40) != 0;
			}

			if (entry.Index == 17 && entry is MetadataOptUUID uuid)
			{
				Owner = uuid.HasValue ? uuid.Value : null;
			}
		}
	}
}