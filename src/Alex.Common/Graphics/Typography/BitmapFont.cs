﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Alex.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiNET.Worlds;
using RocketUI;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Alex.Common.Graphics.Typography
{
    public class BitmapFont : IFont
    {
	    #region Properties
	    
	    public IReadOnlyCollection<char> Characters { get; }

        public int GridWidth  { get; }
        public int GridHeight { get; }

        public int LineSpacing { get; set; } = 10;
        public int CharacterSpacing { get; set; } = 1;

        public Texture2D AsciiTexture { get; }

        public Glyph DefaultGlyph { get; private set; }
        //public Glyph[] Glyphs { get; private set; }
        private IFontGlyph[] Glyphs { get; } // Dictionary<char, Glyph> Glyphs { get; }
        private Vector2 Scale  { get; set; } = Vector2.One;

        private bool _isInitialised = false;

        #endregion
        
        #region Constructors
        
        public BitmapFont(GraphicsDevice graphics, Image<Rgba32> bitmap, int gridSize, List<char> characters) :
            this(graphics, bitmap, gridSize, gridSize, characters)
        { }
        public BitmapFont(GraphicsDevice graphics, Image<Rgba32> bitmap, int gridWidth, int gridHeight, List<char> characters) :
            this(null, gridWidth, gridHeight, characters)
        {
	        AsciiTexture = TextureUtils.BitmapToTexture2D(this, graphics, bitmap);
	        Scale = new Vector2(128f / bitmap.Width, 128f / bitmap.Height);
            LoadGlyphs(bitmap, characters);
        }

        public BitmapFont(Texture2D asciiTexture, int gridSize, List<char> characters) : this(asciiTexture, gridSize, gridSize, characters) { }

        public BitmapFont(Texture2D asciiTexture, int gridWidth, int gridHeight, List<char> characters) : this(gridWidth, gridHeight, characters)
        {
	        if (asciiTexture != null)
				AsciiTexture = asciiTexture;
        }

        private BitmapFont(int gridWidth, int gridHeight, List<char> characters)
        {
	        GridWidth   = gridWidth;
            GridHeight  = gridHeight;
            Characters = characters.ToArray();

            DefaultGlyph = new Glyph('\x0000', null, 0, 8);

            Glyphs = ArrayOf<Glyph>.Create((int) characters.Max(x => (int) x), DefaultGlyph).Cast<IFontGlyph>()
	           .ToArray();

            //Glyphs = new Dictionary<char, Glyph>(characters.Count); //new Glyph[characters.Count];
        }
        
        #endregion

        public Vector2 MeasureString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Vector2.Zero;
            }
            
            float width = 0.0f, finalLineHeight = LineSpacing;
            Vector2 offset = Vector2.Zero;
            var firstGlyphOfLine = true;
            
            bool styleRandom = false, styleBold = false, styleItalic = false, styleUnderline = false, styleStrikethrough = false;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if(c == '\r') continue;

                if (c == '\n')
                {
                    offset.X = 0.0f;
                    offset.Y += LineSpacing;

                    finalLineHeight = LineSpacing;
                    firstGlyphOfLine = true;

                    styleRandom        = false;
                    styleBold          = false;
                    styleStrikethrough = false;
                    styleUnderline     = false;
                    styleItalic        = false;
                }
                else if (c == '\x00A7')
                {
                    // Formatting
                    
                    // Get next character
                    if(i + 1 >= text.Length) continue;

                    i++;
                    var formatChar = text[i];
                    if (formatChar == 'k')
                    {
                        styleRandom = true;
                    }
                    else if (formatChar == 'l')
                    {
                        styleBold = true;
                    }
                    else if (formatChar == 'm')
                    {
                        styleStrikethrough = true;
                    }
                    else if (formatChar == 'n')
                    {
                        styleUnderline = true;
                    }
                    else if (formatChar == 'o')
                    {
                        styleItalic = true;
                    }
                    else if (formatChar == 'r')
                    {
                        styleRandom        = false;
                        styleBold          = false;
                        styleStrikethrough = false;
                        styleUnderline     = false;
                        styleItalic        = false;
                    }
                }
                else
                {
                    var glyph = GetGlyphOrDefault(c);

                    if (firstGlyphOfLine)
                    {
                       // offset.X += CharacterSpacing;
	                    firstGlyphOfLine = false;
					}
 
                    offset.X += (glyph.Width) + (styleBold ? 1 : 0) + CharacterSpacing;

                    finalLineHeight = MathF.Max(finalLineHeight, glyph.Height);
                    width           = MathF.Max(width, offset.X);
                }
            }

            return new Vector2(width, offset.Y + finalLineHeight);
        }


        public void DrawString(SpriteBatch sb, string text, Vector2 position,
	        Color                          color,
	        FontStyle                      style      = FontStyle.None,
	        Vector2?                       scale      = null,
	        float                          opacity    = 1f,
	        float                          rotation   = 0f,
	        Vector2?                       origin     = null,
	        SpriteEffects                  effects    = SpriteEffects.None,
	        float                          layerDepth = 0f)
	        => DrawString(sb, text, position, (TextColor) color, style, scale, opacity, rotation, origin, effects, layerDepth);
        
        public void DrawString(SpriteBatch   sb, string text, Vector2 position,
                               TextColor     color,
                               FontStyle     style      = FontStyle.None, 
                               Vector2? scale = null,
                               float         opacity    = 1f,
                               float         rotation   = 0f, 
                               Vector2? origin = null,
                               SpriteEffects effects    = SpriteEffects.None,
                               float         layerDepth = 0f)
		{
			if (string.IsNullOrEmpty(text)) return;

			var originVal = origin ?? Vector2.Zero;
			var scaleVal = scale ?? Vector2.One;
			//scaleVal *= Scale;
			
			originVal *= scaleVal;

			var flipAdjustmentX = 0f;
			var flipAdjustmentY = 0f;
			
			var flippedVert     = (effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically;
			var flippedHorz     = (effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;
			
			if (flippedVert || flippedHorz)
			{
				Vector2 size = MeasureString(text);
				
				if (flippedHorz)
				{
					originVal.X         *= -1;
					flipAdjustmentX =  -size.X;
				}

				if (flippedVert)
				{
					originVal.Y         *= -1;
					flipAdjustmentY =  LineSpacing - size.Y;
				}
			}

			var    adjustedOriginX = flipAdjustmentX - originVal.X;
			var    adjustedOriginY = flipAdjustmentY - originVal.Y;
			
			Matrix transformation  = Matrix.Identity;
			float  cos             = 0, sin = 0;
			if (rotation == 0)
			{
				transformation.M11 = (flippedHorz ? -scaleVal.X : scaleVal.X);
				transformation.M22 = (flippedVert ? -scaleVal.Y : scaleVal.Y);
				transformation.M41 = (adjustedOriginX * transformation.M11) + position.X;
				transformation.M42 = (adjustedOriginY * transformation.M22) + position.Y;
			}
			else
			{
				cos                = MathF.Cos(rotation);
				sin                = MathF.Sin(rotation);
				transformation.M11 = (flippedHorz ? -scaleVal.X : scaleVal.X) * cos;
				transformation.M12 = (flippedHorz ? -scaleVal.X : scaleVal.X) * sin;
				transformation.M21 = (flippedVert ? -scaleVal.Y : scaleVal.Y) * (-sin);
				transformation.M22 = (flippedVert ? -scaleVal.Y : scaleVal.Y) * cos;
				transformation.M41 = ((adjustedOriginX * transformation.M11) + adjustedOriginY * transformation.M21) + position.X;
				transformation.M42 = ((adjustedOriginX * transformation.M12) + adjustedOriginY * transformation.M22) + position.Y; 
			}

			var offsetX          = 0f;
			var offsetY          = 0f;
			var firstGlyphOfLine = true;

			TextColor styleColor = color;

			bool styleRandom = false, 
			     styleBold = style.HasFlag(FontStyle.Bold), 
			     styleItalic = style.HasFlag(FontStyle.Italic), 
			     styleUnderline = style.HasFlag(FontStyle.Underline), 
			     styleStrikethrough = style.HasFlag(FontStyle.StrikeThrough), 
			     dropShadow = style.HasFlag(FontStyle.DropShadow);

			var blendFactor = sb.GraphicsDevice.BlendFactor;
			sb.GraphicsDevice.BlendFactor = Color.White * opacity;
			
			var p = new Vector2(offsetX, offsetY);
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];

				if(c == '\r') continue;
				
				if (c == '\n')
				{
					offsetX =  0.0f;
					offsetY += LineSpacing;

					firstGlyphOfLine = true;

					styleRandom        = false;
					styleBold          = false;
					styleStrikethrough = false;
					styleUnderline     = false;
					styleItalic        = false;
					styleColor         = color;
				}
				else if (c == '\x00A7')
				{
					// Formatting

					// Get next character
					if(i + 1 >= text.Length) continue;
					
					i++;
					var formatChar = text[i];
					
					if (formatChar == 'k')
					{
						styleRandom = true;
					}
					else if (formatChar == 'l')
					{
						styleBold = true;
					}
					else if (formatChar == 'm')
					{
						styleStrikethrough = true;
					}
					else if (formatChar == 'n')
					{
						styleUnderline = true;
					}
					else if (formatChar == 'o')
					{
						styleItalic = true;
					}
					else if (formatChar == 'r')
					{
						styleRandom = false;
						styleBold = false;
						styleStrikethrough = false;
						styleUnderline = false;
						styleItalic = false;
						styleColor = color;
					}
					else if ("0123456789abcdef".IndexOf(formatChar, StringComparison.Ordinal) > 0)
					{
						styleColor = TextColor.GetColor(formatChar);
					}
				}
				else
				{
					var glyph = GetGlyphOrDefault(c);

					if (firstGlyphOfLine)
					{
					//	offset.X += CharacterSpacing;
						firstGlyphOfLine = false;
					}

					//if (styleRandom)
					//{
					//	c = 
					//}

					p.X = offsetX;
					p.Y = offsetY;
					//var p = new Vector2(offsetX, offsetY);
					//var px    = offsetX;
					//var py    = offsetY;
					var width = glyph.Width + (styleBold ? 1f : 0f) + CharacterSpacing;

					if (glyph.Texture != null)
					{
						if (dropShadow)
						{
							var shadowP  = p + Vector2.One;

							if (styleBold)
							{
								var boldShadowP = Vector2.Transform(shadowP + Vector2.UnitX, transformation);

								sb.Draw(
									glyph.Texture, boldShadowP, styleColor.BackgroundColor * opacity, rotation,
									originVal, scaleVal * Scale, effects, layerDepth);
							}

							shadowP = Vector2.Transform(shadowP, transformation);

							sb.Draw(
								glyph.Texture, shadowP, styleColor.BackgroundColor * opacity, rotation, originVal,
								scaleVal * Scale, effects, layerDepth);
						}

						if (styleBold)
						{
							var boldP = Vector2.Transform(p + Vector2.UnitX, transformation);

							sb.Draw(
								glyph.Texture, boldP, styleColor.ForegroundColor * opacity, rotation, originVal,
								scaleVal * Scale, effects, layerDepth);
						}

						/*	if (styleUnderline)
							{
								var lineStart = Vector2.Transform(p + new Vector2(0, 8), transformation);
								
								sb.DrawLine(2, lineStart, new Vector2(lineStart.X + width, lineStart.Y), styleColor.ForegroundColor * opacity, scaleVal * Scale, layerDepth);
							}*/

						p = Vector2.Transform(p, transformation);

						sb.Draw(
							glyph.Texture, p, styleColor.ForegroundColor * opacity, rotation, originVal,
							scaleVal * Scale, effects, layerDepth);
					}

					offsetX += width;
				}
			}

			sb.GraphicsDevice.BlendFactor = blendFactor;
		}

        public IFontGlyph GetGlyphOrDefault(char character)
        {
	        var index = character;

	        if (index > Glyphs.Length - 1)
		        return DefaultGlyph;
	        
	        return Glyphs[index];
        }

        private void LoadGlyphs(Image<Rgba32> bitmap, List<char> characters)
        {
            if (_isInitialised) return;
          //  Glyphs = new Glyph[(int) characters.Max(x => (int) x)];

           /* var lockedBitmap = new LockBitmap(bitmap);
            lockedBitmap.LockBits();
            Color[] textureData = lockedBitmap.GetColorArray();
            lockedBitmap.UnlockBits();*/

           //var rgba = bitmap.TryGetSinglePixelSpan();
			bitmap.TryGetSinglePixelSpan(out var rgba);
            var textureWidth = bitmap.Width;
            var textureHeight = bitmap.Height;

            var cellWidth = (textureWidth / GridWidth);
            var cellHeight = (textureHeight / GridHeight);

            var charactersCount = characters.Count;

            //var glyphs = new Glyph[charactersCount];

            for (int i = 0; i < charactersCount; i++)
            {
	            var character = characters[i];
	            if (character > Glyphs.Length -1) continue;
	            
                int row = i / GridWidth;
                int col = i % GridWidth;
                
                // Scan the grid cell by pixel column, to determine the
                // width of the characters.
                int width = 0;
                int height = 0;
                
                bool columnIsEmpty = true;
                for (var x = cellWidth-1; x >= 0; x--)
                {
                    columnIsEmpty = true;

                    for (var y = cellHeight-1; y >= 0; y--)
                    {
                        // width * y + x
                        //if (textureData[(textureWidth * (row * cellHeight + y)) + (col * cellWidth + x)].A != 0)
                   //     if (rgba[(col * cellWidth) + x, (row * cellHeight) + y].A != 0)
						if (rgba[(textureWidth * (row * cellHeight + y)) + (col * cellWidth + x)].A != 0)
                        {
                            columnIsEmpty = false;
                            if (y > height)
	                            height = y;
                        }
                    }

                    width = x;

                    if (!columnIsEmpty)
                    {
                        break;
                    }
                }
                
                
                var charWidth =  (0.5f + (width * (8.0f / cellWidth)) + 1f);
                var charHeight = (0.5f + (height * (8.0f / cellHeight)) + 1f);
                
                ++width;
                ++height;
                
                var bounds = new Rectangle(col * cellWidth, row * cellHeight, width, cellHeight);
                var textureSlice = AsciiTexture.Slice(bounds);

                if (character == ' ')
	            {
		            charWidth = 4;
	            }

				var glyph = new Glyph(character, textureSlice, charWidth, charHeight);

                Debug.WriteLine($"BitmapFont Glyph Loaded: {glyph}");
                
                Glyphs[(int)character] = glyph;
                //glyphs[i] = glyph;
            }

            //Glyphs = glyphs;
            DefaultGlyph = new Glyph('\x0000',AsciiTexture.Slice(0, 0, 0, 0), 8, 0);

            _isInitialised = true;
        }

        public struct Glyph : IFontGlyph
        {
            public char Character { get; }
            public ITexture2D Texture { get; }
            public float Width { get; }
            public float Height { get; }

            internal Glyph(char character, ITexture2D texture, float width, float height)
            {
                Character = character;
                Texture = texture;
                Width = width;
                Height = height;
            }

            public override string ToString()
            {
                return $"CharacterIndex={Character}, Glyph={Texture}, Width={Width}, Height={Height}";
            }
        }
    }
}
