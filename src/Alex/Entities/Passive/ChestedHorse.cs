using Alex.Networking.Java.Packets.Play;
using Alex.Worlds;
using MiNET.Entities;

namespace Alex.Entities.Passive
{
	public abstract class ChestedHorse : AbstractHorse
	{
		/// <inheritdoc />
		public override bool IsChested
		{
			get
			{
				return base.IsChested;
			}
			set
			{
				base.IsChested = value;

				var modelRenderer = ModelRenderer;

				if (modelRenderer != null)
				{
					modelRenderer.SetVisibility("Bag1", value);
					modelRenderer.SetVisibility("Bag2", value);
				}
			}
		}

		/// <inheritdoc />
		protected ChestedHorse(EntityType type, World level) : base(type, level) { }

		/// <inheritdoc />
		protected override void HandleJavaMeta(MetaDataEntry entry)
		{
			base.HandleJavaMeta(entry);

			if (entry.Index == 18 && entry is MetadataBool val)
			{
				IsChested = val.Value;
			}
		}
	}
}