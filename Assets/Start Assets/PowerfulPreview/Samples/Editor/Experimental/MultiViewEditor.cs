using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using StartAssets.PowerfulPreview.Controls;
using StartAssets.PowerfulPreview.Utils;

namespace StartAssets.PowerfulPreview.Samples
{
    /// <summary>
    /// It's an example of how you can use previews of other assets 
    /// to draw them at the same time in the preview of this editor. 
    /// </summary>
    [CustomEditor( typeof( MultiViewAsset ) )]
    public class MultiViewEditor : Editor
    {
        [MenuItem( "Assets/Create/Powerful Preview Samples/Multi View Preview Asset" )]
        public static void CreateMultiViewAsset()
        {
            AssetCreator.CreateAsset<MultiViewAsset>( "Multi View Preview Asset" );
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if( mTitleStyle == null )
            {
                mTitleStyle = new GUIStyle( EditorGUIUtility.isProSkin ? EditorStyles.whiteLabel : EditorStyles.label );
                mTitleStyle.fontStyle = FontStyle.Bold;
                mTitleStyle.fontSize = 15;
            }

            var assetsProperty = serializedObject.FindProperty( "assets" );
            var viewNames = new string[]
            {
                "Top Left View", "Top Right View", "Bottom Left View", "Bottom Right View"
            };
            for (int i = 0; i < 4; i++)
            {
                var prop = serializedObject.FindProperty(string.Format("assets.Array.data[{0}]", i));
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(prop, new GUIContent(viewNames[i]));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    CreateEditors();
                    return;
                }
            }
            EditorGUILayout.Space();
            
            var lastRect = GUILayoutUtility.GetLastRect();
            var contentHeight = ( Screen.height - lastRect.yMax - lastRect.height - 100);
            GUILayout.Space(contentHeight);

            var lineWidth = 6;
            var width = lastRect.width / 2;
            var viewHeight = contentHeight / 2;
            DrawGrid(lastRect, width, lineWidth, viewHeight);

            var rects = GetRectsForEditors(lastRect, width, lineWidth, viewHeight);
            for (int i = 0; i < mAssetEditors.Length; i++)
            {
                var r = rects[i];
                r.yMin += lineWidth * 2 + 2;
                r.height += lineWidth - 2;
                if (mAssetEditors[i] != null)
                {
                    mAssetEditors[i].OnInteractivePreviewGUI(r, GUI.skin.box);
                }
                else
                {
                    var label = string.Format("There is no asset assigned to the {0}", viewNames[i].ToLower());
                    var skin = new GUIStyle(GUI.skin.label);
                    skin.fontStyle = FontStyle.Bold;
                    skin.fontSize = 15;
                    var size = skin.CalcSize(new GUIContent(label));
                    GUI.Label(new Rect(r.center - size / 2, size), label, skin);
                }
            }

            serializedObject.ApplyModifiedProperties();
            Repaint();
        }

        private void OnEnable()
        {
            mData = target as MultiViewAsset;
            CreateEditors();
        }
        private void OnDisable()
        {
            ReleaseEditors();
        }

        private void CreateEditors()
        {
            ReleaseEditors();
            mAssetEditors = new Editor[ mData.assets.Length ]; 
            for( int i = 0; i < mAssetEditors.Length; i++ )
            {
                var asset = mData.assets[ i ];
                if( asset != null && asset.GetType() == typeof( MultiViewAsset ) )
                {
                    mData.assets[ i ] = null; 
                }
                mAssetEditors[ i ] = asset != null ? CreateEditor( asset ) : null;
            }
        }
        private void ReleaseEditors()
        {
            if( mAssetEditors == null )
            {
                return; 
            }
            foreach( var editor in mAssetEditors )
            {
                DestroyImmediate( editor );
            }
            mAssetEditors = null;
        }

        private void DrawGrid( Rect lastRect, float width, float lineWidth, float viewHeight )
        {
            EditorGUI.LabelField(new Rect(lastRect.xMin + width - 3, lastRect.yMin + 10, lineWidth, viewHeight * 2), "", 
                GUI.skin.verticalSlider);
            EditorGUI.LabelField(new Rect(lastRect.xMin, lastRect.yMin + viewHeight, lastRect.width, lineWidth), "",
                GUI.skin.horizontalSlider);
            EditorGUI.LabelField(new Rect(lastRect.xMin, lastRect.yMin + viewHeight * 2, lastRect.width, lineWidth), "", 
                GUI.skin.horizontalSlider);

        }

        private Rect[] GetRectsForEditors(Rect lastRect, float width, float lineWidth, float viewHeight)
        {
            var topViewRect = new Rect(lastRect.xMin, lastRect.yMin, width - lineWidth / 2, viewHeight);
            var sideViewRect = new Rect(lastRect.xMin + width + lineWidth + lineWidth / 2, lastRect.yMin, width - 8, viewHeight);
            var frontViewRect = new Rect(lastRect.xMin, lastRect.yMin + viewHeight, width - lineWidth / 2, viewHeight);
            var actionViewRect = new Rect(lastRect.xMin + width + lineWidth, lastRect.yMin + viewHeight, width - 8, viewHeight - lineWidth - 2);
            return new Rect[]
            {
                topViewRect,
                frontViewRect,
                sideViewRect,
                actionViewRect,
            };
        }

        private GUIStyle mTitleStyle;

        private MultiViewAsset mData;
        private Editor[] mAssetEditors; 
    }
}