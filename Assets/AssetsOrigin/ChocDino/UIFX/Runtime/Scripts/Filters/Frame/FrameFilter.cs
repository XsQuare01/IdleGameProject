//--------------------------------------------------------------------------//
// Copyright 2023-2024 Chocolate Dinosaur Ltd. All rights reserved.         //
// For full documentation visit https://www.chocolatedinosaur.com           //
//--------------------------------------------------------------------------//

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using UnityInternal = UnityEngine.Internal;

namespace ChocDino.UIFX
{
	public enum FrameShape
	{
		Rectangle,
		Square,
		Circle,
	}

	[System.Serializable]
	public struct RectPadToEdge
	{
		public bool left, right;
		public bool top, bottom;
	}

	[System.Serializable]
	public struct RectEdge
	{
		public RectEdge(float value)
		{
			left = right = top = bottom = value;
		}

		public float left;
		public float right;
		public float top;
		public float bottom;

		public Vector4 ToVector()
		{
			return new Vector4(left, right, top, bottom);
		}
	}

	[System.Serializable]
	public struct RectCorners
	{
		public RectCorners(float value)
		{
			topLeft = topRight = bottomLeft = bottomRight = value;
		}

		[Range(0f, 1f)] public float topLeft;
		[Range(0f, 1f)] public float topRight;
		[Range(0f, 1f)] public float bottomLeft;
		[Range(0f, 1f)] public float bottomRight;

		public bool IsZero()
		{
			return (topLeft <= 0f && topRight <= 0f && bottomLeft <= 0f && bottomRight <= 0f);
		}

		public Vector4 ToVector()
		{
			return new Vector4(topLeft, topRight, bottomLeft, bottomRight);
		}
	}

	public enum FrameRoundCornerMode
	{
		None,
		Small,
		Medium,
		Large,
		Circular,
		Percent,
		Custom,
	}

	/// <summary>
	/// </summary>
	[AddComponentMenu("UI/Chocolate Dinosaur UIFX/Filters/UIFX - Frame Filter")]
	public class FrameFilter : FilterBase
	{
		[SerializeField] FrameShape _shape = FrameShape.Rectangle;
		[SerializeField] Color _color = Color.black;
		[SerializeField] Sprite _sprite = null;
		[SerializeField] float _radiusPadding = 16f;
		[SerializeField] RectEdge _rectPadding = new RectEdge(16f);
		[SerializeField] RectPadToEdge _rectToEdge;
		[SerializeField] FrameRoundCornerMode _rectRoundCornerMode = FrameRoundCornerMode.None;
		[SerializeField, Range(0f, 1f)] float _rectRoundCornersPercent = 0.25f;
		[SerializeField] RectCorners _rectRoundCorners = new RectCorners(0.25f);
		[SerializeField] float _softness = 2f;
		[SerializeField] bool _cutoutSource = false;
		[SerializeField] Color _borderColor = Color.white;
		[SerializeField] float _borderSize = 0f;
		[SerializeField] float _borderSoftness = 2f;

		/// <summary></summary>
		public FrameShape Shape { get { return _shape; } set { ChangeProperty(ref _shape, value); } }

		/// <summary></summary>
		public Color Color { get { return _color; } set { ChangeProperty(ref _color, value); } }

		/// <summary></summary>
		public float RadiusPadding { get { return _radiusPadding; } set { ChangeProperty(ref _radiusPadding, value); } }

		/// <summary></summary>
		public RectEdge RectPadding { get { return _rectPadding; } set { ChangeProperty(ref _rectPadding, value); } }

		/// <summary></summary>
		public RectPadToEdge RectToEdge { get { return _rectToEdge; } set { ChangeProperty(ref _rectToEdge, value); } }

		/// <summary></summary>
		public FrameRoundCornerMode RectRoundCornerMode { get { return _rectRoundCornerMode; } set { ChangeProperty(ref _rectRoundCornerMode, value); } }

		/// <summary></summary>
		public float RectRoundCornersPercent { get { return _rectRoundCornersPercent; } set { ChangeProperty(ref _rectRoundCornersPercent, value); } }

		/// <summary></summary>
		public RectCorners RectRoundCorners { get { return _rectRoundCorners; } set { ChangeProperty(ref _rectRoundCorners, value); } }

		/// <summary></summary>
		public bool CutoutSource { get { return _cutoutSource; } set { ChangeProperty(ref _cutoutSource, value); } }

		/// <summary></summary>
		public float BorderSize { get { return _borderSize; } set { ChangeProperty(ref _borderSize, Mathf.Max(0f, value)); } }

		/// <summary></summary>
		public Color BorderColor { get { return _borderColor; } set { ChangeProperty(ref _borderColor, value); } }

		internal class FrameShader
		{
			internal const string Id = "Hidden/ChocDino/UIFX/Blend-Frame";

			internal static class Prop
			{
				internal static readonly int CutoutAlpha = Shader.PropertyToID("_CutoutAlpha");
				internal static readonly int FillColor = Shader.PropertyToID("_FillColor");
				//internal static readonly int FillTex = Shader.PropertyToID("_FillTex");
				internal static readonly int Rect_ST = Shader.PropertyToID("_Rect_ST");
				internal static readonly int EdgeRounding = Shader.PropertyToID("_EdgeRounding");
				internal static readonly int FillSoft = Shader.PropertyToID("_FillSoft");
				internal static readonly int BorderColor = Shader.PropertyToID("_BorderColor");
				internal static readonly int BorderSizeSoft = Shader.PropertyToID("_BorderSizeSoft");
			}
			internal static class Keyword
			{
				internal const string Cutout = "CUTOUT";
				internal const string Border = "BORDER";
				internal const string ShapeCircle = "SHAPE_CIRCLE";
				internal const string ShapeRoundRect = "SHAPE_ROUNDRECT";
			}
		}

		protected override string GetDisplayShaderPath()
		{
			return FrameShader.Id;
		}

		#if UNITY_EDITOR
		protected override void OnValidate()
		{
			/*_rectPadding.left = Mathf.Max(0f, _rectPadding.left);
			_rectPadding.right = Mathf.Max(0f, _rectPadding.right);
			_rectPadding.top = Mathf.Max(0f, _rectPadding.top);
			_rectPadding.bottom = Mathf.Max(0f, _rectPadding.bottom);
			_radiusPadding = Mathf.Max(0f, _radiusPadding);*/
			_rectRoundCorners.topLeft = Mathf.Clamp01(_rectRoundCorners.topLeft);
			_rectRoundCorners.topRight = Mathf.Clamp01(_rectRoundCorners.topRight);
			_rectRoundCorners.bottomLeft = Mathf.Clamp01(_rectRoundCorners.bottomLeft);
			_rectRoundCorners.bottomRight = Mathf.Clamp01(_rectRoundCorners.bottomRight);
			_softness = Mathf.Max(1f, _softness);
			_borderSize = Mathf.Max(0f, _borderSize);
			_borderSoftness = Mathf.Max(1f, _borderSoftness);
			base.OnValidate();

		}
		#endif

		protected override bool DoParametersModifySource()
		{
			if (!base.DoParametersModifySource())
			{
				return false;
			}

			if (_color.a <= 0f && !this.IsBorderVisible()) return false;

			return true;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			/*RenderTextureHelper.ReleaseTemporary(ref _rt);
			ObjectHelper.Destroy(ref _slicedMesh);
			if (_cb != null)
			{
				_cb.Release();
				_cb = null;
			}*/

			base.OnDisable();
		}

		protected override void GetFilterAdjustSize(ref Vector2Int leftDown, ref Vector2Int rightUp)
		{
			Rect sourceRect = _screenRect.GetRect();
			if (sourceRect.width <= 0f || sourceRect.height <= 0f) { return; }

			float scaledSoftness = _softness * ResolutionScalingFactor;
			switch (_shape)
			{
				case FrameShape.Rectangle:
				{
					bool hasBorder = (_sprite != null && _sprite.border.sqrMagnitude > 0f);

					var monitorSize = Filters.GetMonitorResolution();

					// Left
					if (_rectToEdge.left)
					{
						leftDown.x += Mathf.CeilToInt(Mathf.Max(0f, _screenRect.GetRect().x + scaledSoftness - 0f * ResolutionScalingFactor));
					}
					else
					{
						float border = -4096f;
						if (hasBorder)
						{
							border = _sprite.border.x;
						}
						leftDown.x += (int)(Mathf.Max(_rectPadding.left, border) * ResolutionScalingFactor);
					}

					// Right
					if (_rectToEdge.right)
					{
						rightUp.x += Mathf.CeilToInt(Mathf.Max(0f, monitorSize.x - _screenRect.GetRect().xMax + scaledSoftness));
					}
					else
					{
						float border = -4096f;
						if (hasBorder)
						{
							border = _sprite.border.z;
						}
						rightUp.x += (int)(Mathf.Max(_rectPadding.right, border) * ResolutionScalingFactor);
					}

					// Top
					if (_rectToEdge.top)
					{
						rightUp.y += Mathf.CeilToInt(Mathf.Max(0f, monitorSize.y - _screenRect.GetRect().yMax + scaledSoftness));
					}
					else
					{
						float border = -4096f;
						if (hasBorder)
						{
							border = _sprite.border.w;
						}
						rightUp.y += (int)(Mathf.Max(_rectPadding.top, border) * ResolutionScalingFactor);
					}

					// Bottom
					if (_rectToEdge.bottom)
					{
						leftDown.y += Mathf.CeilToInt(Mathf.Max(0f, _screenRect.GetRect().y + scaledSoftness));
					}
					else
					{
						float border = -4096f;
						if (hasBorder)
						{
							border = _sprite.border.y;
						}
						leftDown.y += (int)(Mathf.Max(_rectPadding.bottom, border) * ResolutionScalingFactor);
					}
				}
				break;
				case FrameShape.Circle:
				{
					float hw = sourceRect.width * 0.5f;
					float hh = sourceRect.height * 0.5f;
					float radius = Mathf.Sqrt(hw*hw+hh*hh);
					// NOTE: Currently we can't make leftDown and rightUp negative, so just make it as small as possible while keeping the circle shape
					float radiusPadding = _radiusPadding;//Mathf.Max(-Mathf.Min(radius - hw, radius - hh), _radiusPadding);
					int paddingX = Mathf.CeilToInt(radius - hw + radiusPadding * ResolutionScalingFactor);
					int paddingY = Mathf.CeilToInt(radius - hh + radiusPadding * ResolutionScalingFactor);
					leftDown += new Vector2Int(paddingX, paddingY);
					rightUp += new Vector2Int(paddingX, paddingY);
					break;
				}
				case FrameShape.Square:
				{
					float hw = sourceRect.width * 0.5f;
					float hh = sourceRect.height * 0.5f;
					float radius = Mathf.Max(hw, hh);
					// NOTE: Currently we can't make leftDown and rightUp negative, so just make it as small as possible while keeping the square shape
					float radiusPadding = Mathf.Max(-Mathf.Min(radius - hw, radius - hh), _radiusPadding);
					int paddingX = Mathf.CeilToInt(radius - hw + radiusPadding * ResolutionScalingFactor);
					int paddingY = Mathf.CeilToInt(radius - hh + radiusPadding * ResolutionScalingFactor);
					leftDown += new Vector2Int(paddingX, paddingY);
					rightUp += new Vector2Int(paddingX, paddingY);
					break;
				}
			}

			if (IsBorderVisible())
			{
				int border = Mathf.CeilToInt(_borderSize * ResolutionScalingFactor);
				leftDown += new Vector2Int(border, border);
				rightUp += new Vector2Int(border, border);
			}
		}

		private bool IsBorderVisible()
		{
			return (_borderSize > 0f && _borderColor.a > 0f);
		}

		private bool HasRoundCorners()
		{
			return (_shape != FrameShape.Circle && _rectRoundCornerMode != FrameRoundCornerMode.None && !_rectRoundCorners.IsZero());
		}

		protected override void SetupDisplayMaterial(Texture source, Texture result)
		{
			if (!_displayMaterial) { return; }
			
			// Calculate scale and offset values for our texture to fit it within the geometry rectangle with various layout controls
			/*Rect textureAspectAdjust = Rect.zero;
			if (_texture)
			{
				Rect geometryRect = _screenRect.GetRect();
				float textureAspect = (float)_texture.width / (float)_texture.height;
				Rect aspectGeometryRect = MathUtils.ResizeRectToAspectRatio(geometryRect, _textureScaleMode, textureAspect);
				textureAspectAdjust = MathUtils.GetRelativeRect(geometryRect, aspectGeometryRect);
			
				// Scale centrally
				{
					textureAspectAdjust.x -= 0.5f;
					textureAspectAdjust.y -= 0.5f;
					textureAspectAdjust.xMin *= _textureScale;
					textureAspectAdjust.yMin *= _textureScale;
					textureAspectAdjust.xMax *= _textureScale;
					textureAspectAdjust.yMax *= _textureScale;
					textureAspectAdjust.x += 0.5f;
					textureAspectAdjust.y += 0.5f;
				}
			}
			_displayMaterial.SetTexture(FrameShader.Prop.FillTex, _texture);
			_displayMaterial.SetTextureScale(FrameShader.Prop.FillTex, new Vector2(textureAspectAdjust.width, textureAspectAdjust.height));
			_displayMaterial.SetTextureOffset(FrameShader.Prop.FillTex, new Vector2(textureAspectAdjust.x, textureAspectAdjust.y));*/

			_displayMaterial.SetVector(FrameShader.Prop.Rect_ST, new Vector4(1f / _rectRatio.width, 1f / _rectRatio.height, -_rectRatio.xMin / _rectRatio.width, -_rectRatio.yMin / _rectRatio.height));
			_displayMaterial.SetColor(FrameShader.Prop.FillColor, Color.Lerp(Color.clear, _color, this.Strength));
			_displayMaterial.SetFloat(FrameShader.Prop.FillSoft, _softness * ResolutionScalingFactor);
			
			if (_cutoutSource)
			{
				_displayMaterial.EnableKeyword(FrameShader.Keyword.Cutout);
				_displayMaterial.SetFloat(FrameShader.Prop.CutoutAlpha, this.Strength);
			}
			else
			{
				_displayMaterial.DisableKeyword(FrameShader.Keyword.Cutout);
			}

			switch (_shape)
			{
				case FrameShape.Circle:
				_displayMaterial.EnableKeyword(FrameShader.Keyword.ShapeCircle);
				_displayMaterial.DisableKeyword(FrameShader.Keyword.ShapeRoundRect);
				break;
				case FrameShape.Rectangle:
				case FrameShape.Square:
				if (_rectRoundCornerMode != FrameRoundCornerMode.None)
				{
					if (_rectRoundCornerMode != FrameRoundCornerMode.Custom)
					{
						switch (_rectRoundCornerMode)
						{
							case FrameRoundCornerMode.Small:
							_rectRoundCornersPercent = 0.125f;
							break;
							case FrameRoundCornerMode.Medium:
							_rectRoundCornersPercent = 0.25f;
							break;
							case FrameRoundCornerMode.Large:
							_rectRoundCornersPercent = 0.5f;
							break;
							case FrameRoundCornerMode.Circular:
							_rectRoundCornersPercent = 1f;
							break;
							case FrameRoundCornerMode.Percent:
							break;
						}
						_rectRoundCorners.topLeft = _rectRoundCornersPercent;
						_rectRoundCorners.topRight = _rectRoundCornersPercent;
						_rectRoundCorners.bottomLeft = _rectRoundCornersPercent;
						_rectRoundCorners.bottomRight = _rectRoundCornersPercent;
					}
				}

				if (HasRoundCorners())
				{
					Rect geometryRect = _screenRect.GetRect();
					float size = Mathf.Min(geometryRect.width - _borderSize * 2f, geometryRect.height - _borderSize * 2f) / ResolutionScalingFactor;

					_displayMaterial.SetVector(FrameShader.Prop.EdgeRounding, _rectRoundCorners.ToVector() * size * 0.5f * ResolutionScalingFactor);
					_displayMaterial.DisableKeyword(FrameShader.Keyword.ShapeCircle);
					_displayMaterial.EnableKeyword(FrameShader.Keyword.ShapeRoundRect);
				}
				else
				{
					_displayMaterial.DisableKeyword(FrameShader.Keyword.ShapeCircle);
					_displayMaterial.DisableKeyword(FrameShader.Keyword.ShapeRoundRect);
				}
				break;
			}

			if (IsBorderVisible())
			{
				_displayMaterial.EnableKeyword(FrameShader.Keyword.Border);
				_displayMaterial.SetColor(FrameShader.Prop.BorderColor, Color.Lerp(Color.clear, _borderColor, this.Strength));
				_displayMaterial.SetVector(FrameShader.Prop.BorderSizeSoft, new Vector4(_borderSize * ResolutionScalingFactor, _borderSoftness * ResolutionScalingFactor, 0f, 0f));
			}
			else
			{
				_displayMaterial.DisableKeyword(FrameShader.Keyword.Border);
				_displayMaterial.SetVector(FrameShader.Prop.BorderSizeSoft, Vector4.zero);
			}

			base.SetupDisplayMaterial(source, result);
		}

#if false
		private Mesh _slicedMesh;
		private CommandBuffer _cb;
		private VertexHelper _vh;
		private RenderTexture _rt;

		protected override RenderTexture RenderFilters(RenderTexture source)
		{
			/*RenderTextureHelper.ReleaseTemporary(ref _rt);
			_rt = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

			if (_slicedMesh == null)
			{
				_slicedMesh = new Mesh();
			}

			RectInt textureRect = _screenRect.GetTextureRect();

			_vh = new VertexHelper();
			_vh.Clear();
			SlicedSprite.Generate9SliceGeometry_Tile(_sprite, new Rect(0f, 0f, textureRect.width, textureRect.height), true, Color.white, 1f, _vh);
			_vh.FillMesh(_slicedMesh);
			_vh.Dispose(); _vh = null;

			Graphic.defaultGraphicMaterial.mainTexture = _sprite.texture;

			if (_cb == null)
			{
				_cb = new CommandBuffer();
			}
			_cb.Clear();
			_cb.SetRenderTarget(new RenderTargetIdentifier(_rt));
			_cb.ClearRenderTarget(false, true, Color.clear, 0f);
			_cb.SetViewMatrix(Matrix4x4.identity);
			var projectionMatrix = Matrix4x4.Ortho(0f, textureRect.width, 0f, textureRect.height, -1000f, 1000f);
			projectionMatrix = GL.GetGPUProjectionMatrix(projectionMatrix, false);
			_cb.SetProjectionMatrix(projectionMatrix);
			_cb.DrawMesh(_slicedMesh, Matrix4x4.identity, Graphic.defaultGraphicMaterial);
			Graphics.ExecuteCommandBuffer(_cb);

			return _rt;*/

			return source;
		}
#endif

	}
}