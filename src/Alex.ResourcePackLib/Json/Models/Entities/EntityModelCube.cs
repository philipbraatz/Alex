﻿using Alex.ResourcePackLib.Json.Converters;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Alex.ResourcePackLib.Json.Models.Entities
{
	using J = Newtonsoft.Json.JsonPropertyAttribute;
	using R = Newtonsoft.Json.Required;
	using N = Newtonsoft.Json.NullValueHandling;

    public sealed class EntityModelCube
    {
	    /// <summary>
	    /// This point declares the unrotated lower corner of cube (smallest x/y/z value in model space units).
	    /// </summary>
	    [J("origin")]
        public Vector3 Origin { get; set; }
        
	    /// <summary>
	    /// If this field is specified, rotation of this cube occurs around this point, otherwise its rotation is around the center of the box.
	    /// Note that in 1.12 this is flipped upside-down, but is fixed in 1.14.
	    /// </summary>
        [J("pivot", NullValueHandling = N.Ignore)]
        public Vector3? Pivot { get; set; }
        
        /// <summary>
        /// The cube is rotated by this amount (in degrees, x-then-y-then-z order) around the pivot.
        /// </summary>
        [J("rotation", NullValueHandling = N.Ignore)]
        public Vector3? Rotation { get; set; }

        /// <summary>
        /// The cube extends this amount relative to its origin (in model space units).
        /// </summary>
	    [J("size")]
        public Vector3 Size { get; set; }

        /// <summary>
        ///		The uv mapping for this model.
        /// </summary>
	    [J("uv")]
        public EntityModelUV Uv { get; set; }

        /// <summary>
        /// Mirrors this cube about the unrotated x axis (effectively flipping the east / west faces), overriding the bone's 'mirror' setting for this cube.
        /// </summary>
	    [J("mirror", NullValueHandling = N.Ignore)]
	    public bool? Mirror { get; set; }

	    /// <summary>
	    /// Grow this box by this additive amount in all directions (in model space units), this field overrides the bone's inflate field for this cube only.
	    /// </summary>
	    [J("inflate", NullValueHandling = N.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
	    public double? Inflate { get; set; }
	    
	//	[JsonIgnore]
	public Vector3 InflatedSize(float amount)
	{
		var inflation = amount;
		var size      = new Vector3(Size.X, Size.Y, Size.Z);
		
		if (amount == 0f)
			return size;
		
		if (size.X < 0) size.X -= inflation;
		else size.X += inflation;

		if (size.Y < 0) size.Y -= inflation;
		else size.Y += inflation;

		if (size.Z < 0) size.Z -= inflation;
		else size.Z += inflation;

		return size;
	}

	//  [JsonIgnore]
	  public Vector3 InflatedOrigin(float amount)
	  {
		  var origin = Origin;
		  if (amount == 0f)
			  return origin;
		  
		//  return Origin;
		 // var origin = Origin;// * new Vector3(1f, 1f, -1f);
		  //  var s         = InflatedSize(amount);
		  var inflation = (amount / 2f);

		  if (amount > 0f)
		  {
			  origin.X -= inflation;

			  origin.Y -= inflation;

			  origin.Z -= inflation;
		  }
		  else
		  {
			  origin.X += inflation;

			  origin.Y += inflation;

			  origin.Z += inflation;
		  }

		  // var origin         = new Vector3(Origin.X + inflation, Origin.Y + inflation, Origin.Z + inflation);
		  
		 // return origin;

		 return origin;
	  }
	  
	  public Vector3 InflatedPivot(float amount)
	  {
		  if (amount == 0f)
			  return Pivot.Value;
		  // var origin = Origin;// * new Vector3(1f, 1f, -1f);
		  //  var s         = InflatedSize(amount);
		  var inflation = (amount / 2f);

		  //  if (amount > 0f)
		  //	  inflation = -inflation;

		  var p     = Pivot.Value;
		  var pivot = new Vector3(p.X - inflation, p.Y - inflation, p.Z - inflation);

		  return pivot;
	  }

	  public EntityModelCube Clone()
	    {
		    return new EntityModelCube()
		    {
			    Inflate = Inflate,
			    Mirror = Mirror,
			    Origin = Origin,
			    Pivot = Pivot,
			    Rotation = Rotation,
			    Size = Size,
			    Uv = Uv
		    };
	    }
    }
}
