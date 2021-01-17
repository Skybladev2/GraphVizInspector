using StartAssets.PowerfulPreview.Utils;
using System;
using UnityEditor;
using UnityEngine;

namespace StartAssets.PowerfulPreview.Samples
{
    /// <summary>
    /// It's an example of how you can draw an embedded 
    /// preview inside the inspector of your assets. 
    /// </summary>
    [CustomEditor(typeof( EmbeddedPreviewAsset ))]
    public class EmbeddedPreviewAssetEditor : Editor 
    {
        [MenuItem( "Assets/Create/Powerful Preview Samples/Embedded Preview Asset" )]
        public static void CreateEmbeddedPreviewAsset()
        {
            AssetCreator.CreateAsset<EmbeddedPreviewAsset>( "Embedded Preview Asset" );
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Rect lastRect = new Rect();
            float yPreviewPos = 0.0f;
            float gap = 4;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("data"), new GUIContent("Previewable asset"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                CreateEditor();
                return;
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            lastRect = GUILayoutUtility.GetLastRect();
            yPreviewPos = lastRect.yMin + lastRect.height + gap;
            GUILayout.Space(256 + yPreviewPos + gap / 2.0f);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            var rect = new Rect( Screen.width / 2 - 128 + lastRect.xMin, yPreviewPos, 256, 256 );
            if ( mAssetEditor != null )
            {
                mAssetEditor.OnInteractivePreviewGUI( rect, GUI.skin.box );
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            mAsset = target as EmbeddedPreviewAsset;
            mPreview = Preview.Create(this);
            CreateEditor();
        }
        private void OnDisable()
        {
            mPreview?.Dispose();
            mPreview = null;
            ReleaseEditor();
        }

        private void CreateEditor()
        {
            ReleaseEditor();
            mAssetEditor = mAsset.data != null ? CreateEditor(mAsset.data) : null;
        }
        private void ReleaseEditor()
        {
            if (mAssetEditor == null)
            {
                return;
            }
            DestroyImmediate(mAssetEditor);
            mAssetEditor = null;
        }

        private EmbeddedPreviewAsset mAsset;
        private Editor mAssetEditor; 
        private Preview mPreview; 
    }
}
