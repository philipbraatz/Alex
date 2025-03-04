using System.Runtime.Serialization;

namespace Alex.Common.Data.Options
{
	[DataContract]
	public class ControllerOptions : OptionsBase
	{
		[DataMember]
		public OptionsProperty<int> LeftJoystickSensitivity { get; set; }
		
		[DataMember]
		public OptionsProperty<int> RightJoystickSensitivity { get; set; }
		
		[DataMember]
		public OptionsProperty<bool> InvertX { get; set; }
		
		[DataMember]
		public OptionsProperty<bool> InvertY { get; set; }

		public ControllerOptions()
		{
			LeftJoystickSensitivity = DefineRangedProperty(200, 1, 400);
			RightJoystickSensitivity = DefineRangedProperty(200, 1, 400);

			InvertX = DefineProperty(false);
			InvertY = DefineProperty(true);
		}
	}
}