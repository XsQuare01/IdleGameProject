//--------------------------------------------------------------------------//
// Copyright 2023-2024 Chocolate Dinosaur Ltd. All rights reserved.         //
// For full documentation visit https://www.chocolatedinosaur.com           //
//--------------------------------------------------------------------------//

using UnityEngine;
using UnityEditor;

namespace ChocDino.UIFX.Editor
{
	[CustomEditor(typeof(FrameFilter), true)]
	[CanEditMultipleObjects]
	internal class FrameFilterEditor : FilterBaseEditor
	{
		private static GUIContent Content_Shape = new GUIContent("Shape");
		private static GUIContent Content_Padding = new GUIContent("Padding");
		private static GUIContent Content_PadToEdge = new GUIContent("Pad To Edge");
		private static GUIContent Content_RoundCorners = new GUIContent("Round Corners");
		private static GUIContent Content_Percent = new GUIContent("Percent");
		private static GUIContent Content_Custom = new GUIContent("Custom");

		private SerializedProperty _propColor;
		//private SerializedProperty _propTexture;
		//private SerializedProperty _propTextureScaleMode;
		//private SerializedProperty _propTextureScale;
		//private SerializedProperty _propSprite;
		private SerializedProperty _propShape;
		private SerializedProperty _propRectPadding;
		private SerializedProperty _propRadiusPadding;
		private SerializedProperty _propRectToEdge;
		private SerializedProperty _propRectRoundCornerMode;
		private SerializedProperty _rectRoundCornersCustomPercent;
		private SerializedProperty _propRectRoundCorners;
		private SerializedProperty _propCutoutSource;
		private SerializedProperty _propBorderColor;
		private SerializedProperty _propBorderSize;
		protected SerializedProperty _propStrength;

		private readonly AboutInfo aboutInfo = 
				new AboutInfo("UIFX - Frame Filter\n© Chocolate Dinosaur Ltd", "uifx-icon")
				{
					sections = new AboutSection[]
					{
						new AboutSection("Asset Guides")
						{
							buttons = new AboutButton[]
							{
								new AboutButton("Components Reference", "https://www.chocdino.com/products/uifx/bundle/components/frame-filter/"),
							}
						},
						new AboutSection("Unity Asset Store Review\r\n<color=#ffd700>★★★★☆</color>")
						{
							buttons = new AboutButton[]
							{
								new AboutButton("Review <b>UIFX Bundle</b>", AssetStoreBundleReviewUrl),
							}
						},
						new AboutSection("UIFX Support")
						{
							buttons = new AboutButton[]
							{
								new AboutButton("Discord Community", DiscordUrl),
								new AboutButton("Post to Unity Discussions", ForumBundleUrl),
								new AboutButton("Post Issues to GitHub", GithubUrl),
								new AboutButton("Email Us", SupportEmailUrl),
							}
						}
					}
				};

		protected virtual void OnEnable()
		{
			_propColor = VerifyFindProperty("_color");
			//_propTexture = VerifyFindProperty("_texture");
			//_propTextureScaleMode = VerifyFindProperty("_textureScaleMode");
			//_propTextureScale = VerifyFindProperty("_textureScale");
			//_propSprite = VerifyFindProperty("_sprite");
			_propShape = VerifyFindProperty("_shape");
			_propRectPadding = VerifyFindProperty("_rectPadding");
			_propRadiusPadding = VerifyFindProperty("_radiusPadding");
			_propRectToEdge = VerifyFindProperty("_rectToEdge");
			_propRectRoundCornerMode = VerifyFindProperty("_rectRoundCornerMode");
			_rectRoundCornersCustomPercent = VerifyFindProperty("_rectRoundCornersPercent");
			_propRectRoundCorners = VerifyFindProperty("_rectRoundCorners");
			_propCutoutSource = VerifyFindProperty("_cutoutSource");
			_propBorderColor = VerifyFindProperty("_borderColor");
			_propBorderSize = VerifyFindProperty("_borderSize");
			_propStrength = VerifyFindProperty("_strength");
		}

		public override void OnInspectorGUI()
		{
			aboutInfo.OnGUI();

			serializedObject.Update();

			var filter = this.target as FilterBase;

			if (OnInspectorGUI_Check(filter))
			{
				return;
			}

			GUILayout.Label(Content_Shape, EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EnumAsToolbar(_propShape);
			if (_propShape.enumValueIndex == (int)FrameShape.Rectangle)
			{
				EditorGUILayout.PropertyField(_propRectPadding, Content_Padding);
				EditorGUILayout.PropertyField(_propRectToEdge, Content_PadToEdge);
			}
			else
			{
				EditorGUILayout.PropertyField(_propRadiusPadding, Content_Padding);
			}

			if (_propShape.enumValueIndex != (int)FrameShape.Circle)
			{
				EditorGUILayout.PropertyField(_propRectRoundCornerMode, Content_RoundCorners);

				switch (_propRectRoundCornerMode.enumValueIndex)
				{
					case (int)FrameRoundCornerMode.Percent:
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(_rectRoundCornersCustomPercent, Content_Percent);
					EditorGUI.indentLevel--;
					break;
					case (int)FrameRoundCornerMode.Custom:
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(_propRectRoundCorners, Content_Custom);
					EditorGUI.indentLevel--;
					break;
				}
			}

			EditorGUI.indentLevel--;

			GUILayout.Label(Content_Fill, EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_propColor);
			//EditorGUILayout.PropertyField(_propTexture);
			//EditorGUILayout.PropertyField(_propTextureScaleMode);
			//EditorGUILayout.PropertyField(_propTextureScale);
			//EditorGUILayout.PropertyField(_propSprite);
			EditorGUI.indentLevel--;

			GUILayout.Label(Content_Border, EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_propBorderSize, Content_Size);
			EditorGUILayout.PropertyField(_propBorderColor, Content_Color);
			EditorGUI.indentLevel--;

			GUILayout.Label(Content_Apply, EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_propCutoutSource);
			DrawStrengthProperty(_propStrength);
			EditorGUI.indentLevel--;

			if (OnInspectorGUI_Baking(filter))
			{
				return;
			}

			FilterBaseEditor.OnInspectorGUI_Debug(filter);

			serializedObject.ApplyModifiedProperties();
		}
	}
}