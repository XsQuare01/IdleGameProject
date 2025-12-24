//--------------------------------------------------------------------------//
// Copyright 2023-2024 Chocolate Dinosaur Ltd. All rights reserved.         //
// For full documentation visit https://www.chocolatedinosaur.com           //
//--------------------------------------------------------------------------//
#if UIFX_TMPRO

// In Unity 2020.2 reorderable lists became default for arrays and lists
#if UNITY_2020_2_OR_NEWER
	#define UNITY_REORDERABLE_LISTS
#endif

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using TMPro;
using ChocDino.UIFX;

namespace ChocDino.UIFX.Editor
{
	[CustomEditor(typeof(FilterStackTextMeshPro), true)]
	[CanEditMultipleObjects]
	internal class FilterStackTextMeshProEditor : BaseEditor
	{
		private static readonly GUIContent Content_Filters = new GUIContent("Filters");
		private static readonly GUIContent Content_FilterList = new GUIContent("Filter List");
		private static readonly string Pref_Prefix = "UIFX.FilterStackTextMeshPro.";
		private static readonly string Pref_SelectedTypeIndex = Pref_Prefix + "SelectedTypeIndex";

		#if !UNITY_REORDERABLE_LISTS
		private ReorderableList _reorderableList;
		#endif

		private SerializedProperty _propApplyToSprites;
		private SerializedProperty _propRelativeToTransformScale;
		private SerializedProperty _propRelativeFontSize;
		private SerializedProperty _propUpdateOnTransform;

		static GUIContent Content_Add = new GUIContent("Add");

		private SerializedProperty _propFilters;

		private List<System.Type> _filterTypes;
		private List<FilterBase> _unusedFilterComponents = new List<FilterBase>(4);
		private bool _hasNullComponents = false;
		private GUIContent[] _filterTypesNames;
		private int _selectedTypeIndex;

		void OnEnable()
		{
			// Note: Had a case where in a prefab if the component is removed it can still call OnEnable()
			if (this.target == null) return;

			_filterTypes = new List<System.Type>(FindSubClassesOf<FilterBase>());
			if (_filterTypes != null && _filterTypes.Count > 0)
			{
				_filterTypesNames = new GUIContent[_filterTypes.Count];
				for (int i = 0; i < _filterTypes.Count; i++)
				{
					_filterTypesNames[i] = new GUIContent(GetDisplayNameForComponentType(_filterTypes[i]));
				}
			}

			_selectedTypeIndex = EditorPrefs.GetInt(Pref_SelectedTypeIndex, 0);
			_selectedTypeIndex = Mathf.Clamp(_selectedTypeIndex, 0, _filterTypesNames.Length - 1);

			_propFilters = VerifyFindProperty("_filters");
			_propApplyToSprites = VerifyFindProperty("_applyToSprites");
			_propRelativeToTransformScale = VerifyFindProperty("_relativeToTransformScale");
			_propRelativeFontSize = VerifyFindProperty("_relativeFontSize");
			_propUpdateOnTransform = VerifyFindProperty("_updateOnTransform");
			#if !UNITY_REORDERABLE_LISTS
			_reorderableList = new ReorderableList(serializedObject, _propFilters, true, false, true, true)
			{
				drawHeaderCallback = DrawListHeader,
				drawElementCallback = DrawListElement
			};
			#endif

			UpdateUnusedFilterList();
		}

		private void UpdateUnusedFilterList()
		{
			var filterStack = this.target as FilterStackTextMeshPro;
			var filterComponents = filterStack.gameObject.GetComponents<FilterBase>();

			_hasNullComponents = false;
			_unusedFilterComponents = new List<FilterBase>(filterComponents);
			for (int i = 0; i < _propFilters.arraySize; i++)
			{
				FilterBase filterComponent = _propFilters.GetArrayElementAtIndex(i).objectReferenceValue as FilterBase;
				if (filterComponent == null)
				{
					_hasNullComponents = true;
				}
				if (filterComponents.Contains(filterComponent))
				{
					_unusedFilterComponents.Remove(filterComponent);
				}
			}
		}

		void OnDisable()
		{
			_filterTypes = null;
			_filterTypesNames = null;
			EditorPrefs.SetInt(Pref_SelectedTypeIndex, _selectedTypeIndex);
		}

		static void InsertIntoList(SerializedProperty listProp, Component component)
		{
			// Insert into the filter list, find the last null child, or append
			int indexToInsert = -1;
			if (listProp.arraySize > 0)
			{
				var lastChildProp = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
				if (lastChildProp.objectReferenceValue == null)
				{
					indexToInsert = listProp.arraySize - 1;
				}
			}
			if (indexToInsert < 0)
			{
				listProp.InsertArrayElementAtIndex(listProp.arraySize);
				indexToInsert = listProp.arraySize - 1;
			}
			listProp.GetArrayElementAtIndex(indexToInsert).objectReferenceValue = component;
		}

		#if !UNITY_REORDERABLE_LISTS
		private void DrawListHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, Content_FilterList);
		}

		private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty prop = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
	
			// By default, the array dropdown is offset to the left which intersects with
			// the drag handle, so we can either indent the array property or inset the rect.
			//EditorGUI.indentLevel++;
	
			// Take note of the last argument, since this is an array,
			// we want to draw it with all of its children.
			
			EditorGUI.PrefixLabel(rect, new GUIContent("Filter " + index));
			Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent("Filter 88  "));
			rect.x += labelSize.x; rect.width -= labelSize.x;

			FilterBase filterComponent = _propFilters.GetArrayElementAtIndex(index).objectReferenceValue as FilterBase;
			filterComponent.enabled = EditorGUI.Toggle(rect, filterComponent.enabled);
			Vector2 toggleSize = GUI.skin.toggle.CalcSize(GUIContent.none);
			rect.x += toggleSize.x; rect.width -= toggleSize.x;

			EditorGUI.PropertyField(rect, prop, GUIContent.none, includeChildren: false);
			//EditorGUI.indentLevel--;

			//EditorGUI.PropertyField(rect, prop, new GUIContent("Filter " + index), includeChildren: false);
		}
		#endif

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(_propApplyToSprites);
			EditorGUILayout.PropertyField(_propRelativeToTransformScale);
			GUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(_propRelativeFontSize);
			if (_propRelativeFontSize.floatValue > 0f)
			{
				if (GUILayout.Button("Remove", GUILayout.ExpandWidth(false)))
				{
					_propRelativeFontSize.floatValue = 0f;
				}
			}
			else
			{
				var textMeshPro = (target as Behaviour).GetComponent<TextMeshProUGUI>();
				if (textMeshPro.fontSize != _propRelativeFontSize.floatValue)
				{
					if (GUILayout.Button("Set From Text", GUILayout.ExpandWidth(false)))
					{
						_propRelativeFontSize.floatValue = textMeshPro.fontSize;
					}
				}
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(_propUpdateOnTransform);

			EditorGUILayout.Space();
			GUILayout.Label(Content_Filters, EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			if (_propFilters.arraySize == 0)
			{
				EditorGUILayout.HelpBox("No filters currently added", MessageType.Error, true);
			}

			if (_unusedFilterComponents.Count > 0)
			{
				{
					var unusedList = string.Empty;
					foreach (var component in _unusedFilterComponents)
					{
						unusedList += "\n - " + component.GetType().Name;
					}
					EditorGUILayout.HelpBox("There are " + _unusedFilterComponents.Count + " unused filter components:" + unusedList, MessageType.Warning, true);
				}
				GUILayout.BeginHorizontal();
				if (GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), "Add " + _unusedFilterComponents.Count + " Unused Filters"))
				{
					foreach (var component in _unusedFilterComponents)
					{
						InsertIntoList(_propFilters, component);
					}
					_unusedFilterComponents.Clear();
				}
				if (GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), "Remove " + _unusedFilterComponents.Count + " Unused Filters"))
				{
					foreach (var component in _unusedFilterComponents)
					{
						ObjectHelper.Destroy(component);
					}
					_unusedFilterComponents.Clear();
				}
				GUILayout.EndHorizontal();
			}

			EditorGUI.BeginChangeCheck();
			#if UNITY_REORDERABLE_LISTS
			{
				EditorGUILayout.PropertyField(_propFilters, Content_FilterList);
			}
			#else
			{
				_reorderableList.DoLayoutList();
			}
			#endif
			if (EditorGUI.EndChangeCheck())
			{
				UpdateUnusedFilterList();
			}

			if (_hasNullComponents)
			{
				EditorGUILayout.HelpBox("There are empty slots in the filter list", MessageType.Info, true);
				if (GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), "Remove Empty Slots"))
				{
					int arraySize = _propFilters.arraySize;
					for (int index = 0; index < arraySize;)
					{
						var arrayElement = _propFilters.GetArrayElementAtIndex(index);
						if (arrayElement.objectReferenceValue == null)
						{
							_propFilters.DeleteArrayElementAtIndex(index);
							arraySize--;
						}
						else
						{
							index++;
						}
					}
					UpdateUnusedFilterList();
				}
				EditorGUILayout.Space();
			}

			serializedObject.ApplyModifiedProperties();

			if (_filterTypesNames != null && _filterTypesNames.Length > 0)
			{
				EditorGUILayout.PrefixLabel("Add Filter:");
				EditorGUILayout.BeginHorizontal();
				// Show dropdown list of filters that can be added
				_selectedTypeIndex = EditorGUILayout.Popup(_selectedTypeIndex, _filterTypesNames);

				// Show button to add selected filter
				if (GUILayout.Button(Content_Add))
				{
					serializedObject.Update();
					foreach (var obj in this.targets)
					{
						var filterStack = obj as FilterStackTextMeshPro;
						var component = ObjectFactory.AddComponent(filterStack.gameObject, _filterTypes[_selectedTypeIndex]);
						InsertIntoList(_propFilters, component);
					}
					serializedObject.ApplyModifiedProperties();
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUI.indentLevel--;
		}

		private static IEnumerable<System.Type> FindSubClassesOf<TBaseType>()
		{
			var baseType = typeof(TBaseType);
			var assembly = baseType.Assembly;

			return assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
		}

		private static string GetDisplayNameForComponentType(System.Type type)
		{
			string title = null;
			
			// Use the AddComponentMenu attribute to get the nice name for the component
			var attr = type.GetCustomAttributes(typeof(AddComponentMenu), false).FirstOrDefault() as AddComponentMenu;
			if (attr != null)
			{
				title = attr.componentMenu?.Trim();
				if (!string.IsNullOrEmpty(title))
				{
					var lastPathCharIndex = title.LastIndexOf('/');
					if (lastPathCharIndex >= 0)
					{
						if (lastPathCharIndex < title.Length - 1)
						{
							title = title.Substring(lastPathCharIndex + 1);
						}
					}
				}
			}
			// Fallback to just use the type name
			if (string.IsNullOrEmpty(title))
			{
				title = type.Name;
			}
			return title;
		}
	}
}
#endif