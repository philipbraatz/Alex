﻿using Alex.Common.Utils;
using Microsoft.Xna.Framework;

namespace Alex.ResourcePackLib.Json.Models
{
    public class DisplayElement
    {
	    public Vector3 Rotation    { get; set; } = Vector3.Zero;
	    public Vector3 Translation { get; set; } = Vector3.Zero;
	    public Vector3 Scale       { get; set; } = Vector3.One;

	    public DisplayElement()
	    {
		    
	    }

	    public DisplayElement(Vector3 rotation, Vector3 translation, Vector3 scale)
	    {
		    Rotation = rotation;
		    Translation = translation;
		    Scale = scale;
	    }
	    
	    public static readonly DisplayElement Default = new DisplayElement(Vector3.Zero, Vector3.Zero, Vector3.One);

	    public DisplayElement Clone()
	    {
		    return new DisplayElement(
			    new Vector3(Rotation.X, Rotation.Y, Rotation.Z),
			    new Vector3(Translation.X, Translation.Y, Translation.Z), new Vector3(Scale.X, Scale.Y, Scale.Z));
	    }
    }
}
