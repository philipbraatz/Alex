using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alex.Graphics.Effect
{
	public class EntityEffect : Microsoft.Xna.Framework.Graphics.Effect, IEffectMatrices, IEffectFog
    {
        #region Effect Parameters

        private EffectParameter _textureParam;
       // EffectParameter diffuseColorParam;
        private EffectParameter _alphaTestParam;
        private EffectParameter _fogColorParam;
        private EffectParameter _fogStartParam;
        private EffectParameter _fogEndParam;
        private EffectParameter _fogEnabledParam;
        
        private EffectParameter _fogVectorParam;

        private EffectParameter _worldParam;

        private EffectParameter _projParam;

        private EffectParameter _viewParam;
        
        private EffectParameter _textureScaleParam;
         //EffectParameter worldViewProjParam;
        private EffectParameter _bonesParam;
        #endregion

        #region Fields

        bool _fogEnabled;
        bool _vertexColorEnabled;

        Matrix _world      = Matrix.Identity;
        Matrix _view       = Matrix.Identity;
        Matrix _projection = Matrix.Identity;

        Matrix _worldView;

        Vector3 _diffuseColor = Vector3.One;

        float _alpha = 1;

        float _fogStart = 0;
        float _fogEnd   = 1;

        CompareFunction _alphaFunction = CompareFunction.Greater;
        int             _referenceAlpha;
        bool            _isEqNe;

        EffectDirtyFlags _dirtyFlags = EffectDirtyFlags.All;

        Vector2 _textureScale = Vector2.One;
        #endregion

        #region Public Properties
        
        /// <summary>
        /// Gets or sets the world matrix.
        /// </summary>
        public Matrix World
        {
            get { return _world; }

            set
            {
                _world = value;
                _dirtyFlags |= EffectDirtyFlags.WorldViewProj | EffectDirtyFlags.Fog;
            }
        }


        /// <summary>
        /// Gets or sets the view matrix.
        /// </summary>
        public Matrix View
        {
            get { return _view; }

            set
            {
                _view = value;
                _dirtyFlags |= EffectDirtyFlags.WorldViewProj | EffectDirtyFlags.Fog;
            }
        }


        /// <summary>
        /// Gets or sets the projection matrix.
        /// </summary>
        public Matrix Projection
        {
            get { return _projection; }

            set
            {
                _projection = value;
                _dirtyFlags |= EffectDirtyFlags.WorldViewProj;
            }
        }


        /// <summary>
        /// Gets or sets the material diffuse color (range 0 to 1).
        /// </summary>
        public Vector3 DiffuseColor
        {
            get { return _diffuseColor; }

            set
            {
                _diffuseColor = value;
                _dirtyFlags |= EffectDirtyFlags.MaterialColor;
            }
        }


        /// <summary>
        /// Gets or sets the material alpha.
        /// </summary>
        public float Alpha
        {
            get { return _alpha; }

            set
            {
                _alpha = value;
                _dirtyFlags |= EffectDirtyFlags.MaterialColor;
            }
        }


        /// <summary>
        /// Gets or sets the fog enable flag.
        /// </summary>
        public bool FogEnabled
        {
            get { return _fogEnabled; }

            set
            {
                if (_fogEnabled != value)
                {
                    _fogEnabled = value;
                    _dirtyFlags |= EffectDirtyFlags.ShaderIndex | EffectDirtyFlags.FogEnable;
                }
            }
        }


        /// <summary>
        /// Gets or sets the fog start distance.
        /// </summary>
        public float FogStart
        {
            get { return _fogStart; }

            set
            {
                _fogStart = value;
                _dirtyFlags |= EffectDirtyFlags.Fog;
            }
        }


        /// <summary>
        /// Gets or sets the fog end distance.
        /// </summary>
        public float FogEnd
        {
            get { return _fogEnd; }

            set
            {
                _fogEnd = value;
                _dirtyFlags |= EffectDirtyFlags.Fog;
            }
        }

        private float _lightOffset = 0;
        public float LightOffset
        {
            get
            {
                return _lightOffset;
            }
            set
            {
                _lightOffset = value;
                _dirtyFlags |= EffectDirtyFlags.LightOffset;
            }
        }


        /// <summary>
        /// Gets or sets the fog color.
        /// </summary>
      /*  public Vector3 FogColor
        {
            get { return fogColorParam.GetValueVector3(); }
            set { fogColorParam.SetValue(value); }
        }*/
      
        public Vector3 FogColor { get; set; }


        /// <summary>
        /// Gets or sets the current texture.
        /// </summary>
        public Texture2D Texture
        {
            get { return _textureParam.GetValueTexture2D(); }
            set
            {
                _textureParam.SetValue(value);

              //  if (value != null)
              //  {
             //       _textureScaleParam.SetValue(Vector2.One / value.Bounds.Size.ToVector2());
               // }
            }
        }

        public Vector2 TextureScale
        {
            get
            {
                return _textureScale;
                return _textureScaleParam.GetValueVector2();
            }
            set
            {
                _textureScale = value;
                _textureScaleParam.SetValue(value);
            }
        }

        /// <summary>
        /// Gets or sets whether vertex color is enabled.
        /// </summary>
        public bool VertexColorEnabled
        {
            get { return _vertexColorEnabled; }

            set
            {
                if (_vertexColorEnabled != value)
                {
                    _vertexColorEnabled = value;
                    _dirtyFlags |= EffectDirtyFlags.ShaderIndex;
                }
            }
        }


        /// <summary>
        /// Gets or sets the alpha compare function (default Greater).
        /// </summary>
        public CompareFunction AlphaFunction
        {
            get { return _alphaFunction; }

            set
            {
                _alphaFunction = value;
                _dirtyFlags |= EffectDirtyFlags.AlphaTest;
            }
        }


        /// <summary>
        /// Gets or sets the reference alpha value (default 0).
        /// </summary>
        public int ReferenceAlpha
        {
            get { return _referenceAlpha; }

            set
            {
                _referenceAlpha = value;
                _dirtyFlags |= EffectDirtyFlags.AlphaTest;
            }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Creates a new AlphaTestEffect with default parameter settings.
        /// </summary>
        public EntityEffect(GraphicsDevice device, byte[] byteCode) : base(device, byteCode)
        {
            CacheEffectParameters();
        }
        
        public EntityEffect() : base(ResourceManager.EntityEffect)
        {
            CacheEffectParameters();
        }
        
        /// <summary>
        /// Creates a new AlphaTestEffect by cloning parameter settings from an existing instance.
        /// </summary>
        public EntityEffect(EntityEffect cloneSource) : base(cloneSource)
        {
            CacheEffectParameters();

            _fogEnabled = cloneSource._fogEnabled;
            _vertexColorEnabled = cloneSource._vertexColorEnabled;

            _world = cloneSource._world;
            _view = cloneSource._view;
            _projection = cloneSource._projection;

            _diffuseColor = cloneSource._diffuseColor;

            _alpha = cloneSource._alpha;

            _fogStart = cloneSource._fogStart;
            _fogEnd = cloneSource._fogEnd;

            _alphaFunction = cloneSource._alphaFunction;
            _referenceAlpha = cloneSource._referenceAlpha;
            _textureScale = cloneSource._textureScale;
            
            Matrix[] identityBones = new Matrix[MaxBones];
            
            for (int i = 0; i < MaxBones; i++)
            {
                identityBones[i] = Matrix.Identity;
            }
            
            SetBoneTransforms(identityBones);
        }

        /// <summary>
        /// Creates a clone of the current AlphaTestEffect instance.
        /// </summary>
        public override Microsoft.Xna.Framework.Graphics.Effect Clone()
        {
            return new EntityEffect(this);
        }

        /// <summary>
        /// Looks up shortcut references to our effect parameters.
        /// </summary>
        void CacheEffectParameters()
        {
            _textureParam = Parameters["Texture"];
           // diffuseColorParam = Parameters["DiffuseColor"];
            _alphaTestParam = Parameters["AlphaTest"];
            _fogColorParam = Parameters["FogColor"];
            _fogStartParam = Parameters["FogStart"];
            _fogEndParam = Parameters["FogEnd"];
            _fogEnabledParam = Parameters["FogEnabled"];
          //  fogVectorParam = Parameters["FogVector"];
        
            _worldParam = Parameters["World"];

            _projParam = Parameters["Projection"];

            _viewParam = Parameters["View"];
            _textureScaleParam = Parameters["UvScale"];
            _bonesParam                  = Parameters["Bones"];
            //  worldViewProjParam = Parameters["WorldViewProj"];
        }
        
        private static void SetFogVector(
            ref Matrix worldView,
            float fogStart,
            float fogEnd,
            EffectParameter fogVectorParam)
        {
            if ((double) fogStart == (double) fogEnd)
            {
                fogVectorParam.SetValue(new Vector4(0.0f, 0.0f, 0.0f, 1f));
            }
            else
            {
                float num = (float) (1.0 / ((double) fogStart - (double) fogEnd));
                fogVectorParam.SetValue(new Vector4()
                {
                    X = worldView.M13 * num,
                    Y = worldView.M23 * num,
                    Z = worldView.M33 * num,
                    W = (worldView.M43 + fogStart) * num
                });
            }
        }
        
        internal static EffectDirtyFlags SetWorldViewProjAndFog(
            EffectDirtyFlags dirtyFlags,
            ref Matrix world,
            ref Matrix view,
            ref Matrix projection,
            ref Matrix worldView,
            bool fogEnabled,
            float fogStart,
            float fogEnd,
            EffectParameter worldParam,
            EffectParameter viewParam,
            EffectParameter projectionParam,
            EffectParameter fogStartParam,
            EffectParameter fogEndParam,
            EffectParameter fogEnabledParam,
            EffectParameter fogColorParam,
            Vector3 fogColor)
        {
            if ((dirtyFlags & EffectDirtyFlags.WorldViewProj) != ~EffectDirtyFlags.All)
            {
             //   Matrix.Multiply(ref world, ref view, out worldView);
            //    Matrix result;
             //   Matrix.Multiply(ref worldView, ref projection, out result);
               // worldViewProjParam.SetValue(result);
                worldParam.SetValue(world);
                viewParam.SetValue(view);
                projectionParam.SetValue(projection);
                dirtyFlags &= ~EffectDirtyFlags.WorldViewProj;
            }
            if (fogEnabled)
            {
                if ((dirtyFlags & (EffectDirtyFlags.Fog | EffectDirtyFlags.FogEnable)) != ~EffectDirtyFlags.All)
                {
                   // SetFogVector(ref worldView, fogStart, fogEnd, fogVectorParam);
                   fogStartParam.SetValue(fogStart);
                   fogEndParam.SetValue(fogEnd);
                   fogEnabledParam.SetValue(1f);
                   fogColorParam.SetValue(fogColor);
                    dirtyFlags &= ~(EffectDirtyFlags.Fog | EffectDirtyFlags.FogEnable);
                }
            }
            else if ((dirtyFlags & EffectDirtyFlags.FogEnable) != ~EffectDirtyFlags.All)
            {
                fogEnabledParam.SetValue(0f);
                //fogVectorParam.SetValue(Vector4.Zero);
                dirtyFlags &= ~EffectDirtyFlags.FogEnable;
            }
            return dirtyFlags;
        }

        /// <summary>
        /// Lazily computes derived parameter values immediately before applying the effect.
        /// </summary>
        protected override void OnApply()
        {
            // Recompute the world+view+projection matrix or fog vector?
            _dirtyFlags = SetWorldViewProjAndFog(
                _dirtyFlags, ref _world, ref _view, ref _projection, ref _worldView, _fogEnabled, _fogStart, _fogEnd,
                _worldParam, _viewParam, _projParam, _fogStartParam, _fogEndParam, _fogEnabledParam, _fogColorParam, FogColor);

            // Recompute the diffuse/alpha material color parameter?
            if ((_dirtyFlags & EffectDirtyFlags.MaterialColor) != 0)
            {
              //  diffuseColorParam.SetValue(new Vector4(diffuseColor * alpha, alpha));

                _dirtyFlags &= ~EffectDirtyFlags.MaterialColor;
            }

            // Recompute the alpha test settings?
            if ((_dirtyFlags & EffectDirtyFlags.AlphaTest) != 0)
            {
                Vector4 alphaTest = new Vector4();
                bool    eqNe      = false;

                // Convert reference alpha from 8 bit integer to 0-1 float format.
                float reference = (float) _referenceAlpha / 255f;

                // Comparison tolerance of half the 8 bit integer precision.
                const float threshold = 0.5f / 255f;

                switch (_alphaFunction)
                {
                    case CompareFunction.Less:
                        // Shader will evaluate: clip((a < x) ? z : w)
                        alphaTest.X = reference - threshold;
                        alphaTest.Z = 1;
                        alphaTest.W = -1;

                        break;

                    case CompareFunction.LessEqual:
                        // Shader will evaluate: clip((a < x) ? z : w)
                        alphaTest.X = reference + threshold;
                        alphaTest.Z = 1;
                        alphaTest.W = -1;

                        break;

                    case CompareFunction.GreaterEqual:
                        // Shader will evaluate: clip((a < x) ? z : w)
                        alphaTest.X = reference - threshold;
                        alphaTest.Z = -1;
                        alphaTest.W = 1;

                        break;

                    case CompareFunction.Greater:
                        // Shader will evaluate: clip((a < x) ? z : w)
                        alphaTest.X = reference + threshold;
                        alphaTest.Z = -1;
                        alphaTest.W = 1;

                        break;

                    case CompareFunction.Equal:
                        // Shader will evaluate: clip((abs(a - x) < Y) ? z : w)
                        alphaTest.X = reference;
                        alphaTest.Y = threshold;
                        alphaTest.Z = 1;
                        alphaTest.W = -1;
                        eqNe = true;

                        break;

                    case CompareFunction.NotEqual:
                        // Shader will evaluate: clip((abs(a - x) < Y) ? z : w)
                        alphaTest.X = reference;
                        alphaTest.Y = threshold;
                        alphaTest.Z = -1;
                        alphaTest.W = 1;
                        eqNe = true;

                        break;

                    case CompareFunction.Never:
                        // Shader will evaluate: clip((a < x) ? z : w)
                        alphaTest.Z = -1;
                        alphaTest.W = -1;

                        break;

                    case CompareFunction.Always:
                    default:
                        // Shader will evaluate: clip((a < x) ? z : w)
                        alphaTest.Z = 1;
                        alphaTest.W = 1;

                        break;
                }

                _alphaTestParam.SetValue(alphaTest);

                _dirtyFlags &= ~EffectDirtyFlags.AlphaTest;

                // If we changed between less/greater vs. equal/notequal
                // compare modes, we must also update the shader index.
                if (_isEqNe != eqNe)
                {
                    _isEqNe = eqNe;
                    _dirtyFlags |= EffectDirtyFlags.ShaderIndex;
                }
            }

            // Recompute the shader index?
            if ((_dirtyFlags & EffectDirtyFlags.ShaderIndex) != 0)
            {
                CurrentTechnique = Techniques[0];
            }
        }


        #endregion

        public const int MaxBones = 72;
        /// <summary>
        /// Sets an array of skinning bone transform matrices.
        /// </summary>
        public void SetBoneTransforms(Matrix[] boneTransforms)
        {
            if ((boneTransforms == null) || (boneTransforms.Length == 0))
                throw new ArgumentNullException("boneTransforms");

            if (boneTransforms.Length > MaxBones)
                throw new ArgumentException();

            _bonesParam.SetValue(boneTransforms);
        }


        /// <summary>
        /// Gets a copy of the current skinning bone transform matrices.
        /// </summary>
        public Matrix[] GetBoneTransforms(int count)
        {
            if (count <= 0 || count > MaxBones)
                throw new ArgumentOutOfRangeException("count");

            Matrix[] bones = _bonesParam.GetValueMatrixArray(count);
            
            // Convert matrices from 43 to 44 format.
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].M44 = 1;
            }
            
            return bones;
        }
    }
}