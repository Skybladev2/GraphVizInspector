using UnityEngine;
using UnityEditor;
using StartAssets.PowerfulPreview.Controls;
using StartAssets.PowerfulPreview.Utils;
using UnityEngine.UI;
using StartAssets.PowerfulPreview.Drawers;

namespace StartAssets.PowerfulPreview.Samples
{
    /// <summary>
    /// It's an example of how you can draw the canvases inside the preview. 
    /// </summary>
    [CustomEditor(typeof( CanvasPreviewAsset ) ) ]
    public class CanvasPreviewAssetEditor : PreviewEditor< CanvasPreviewAsset >
    {
        [MenuItem( "Assets/Create/Powerful Preview Samples/Canvas Preview Asset" )]
        public static void CreateCanvasPreviewAsset()
        {
            AssetCreator.CreateAsset<CanvasPreviewAsset>("Canvas Preview Asset");
        }
        
        public override void OnPreviewSettings()
        {
            var previewSize = preview.SurfaceRect.size;
            var aspectRatio = previewSize.x / previewSize.y;
            var labelString = string.Format("Preview size: {0}x{1} Aspect ratio: {2}",
                (int)previewSize.x, (int)previewSize.y, aspectRatio.ToString("F2"));
            var style = EditorStyles.boldLabel;
            style.fontSize = 11;
            style.normal.textColor = Color.white;
            EditorGUILayout.LabelField(labelString, style, GUILayout.Width(270));
        }

        protected override void OnCreate()
        {
            CreateCanvasInstance();

            preview.Camera.orthographic = true;
            preview.Camera.transform.localEulerAngles = Vector3.zero;
            preview.Camera.transform.localPosition = Vector3.zero;
            preview.CameraController.SetStatesEnabled(false);
        }
        protected override void OnGUIUpdate()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Canvas"));
            if( EditorGUI.EndChangeCheck() )
            {
                serializedObject.ApplyModifiedProperties();
                CreateCanvasInstance();
            }
        } 
        protected override void OnPreviewUpdate()
        {
            if( asset.Canvas == null )
            {
                return;
            }

            var lastActiveSceneView = SceneView.lastActiveSceneView;
            if (lastActiveSceneView != null)
            {
                var lastCamera = lastActiveSceneView.camera;
                if (lastCamera != null)
                {
                    var offset = lastCamera.transform.TransformDirection(Vector3.back);
                    var distance = mCanvas.planeDistance * 10.0f;
                    var origin = lastCamera.transform.position;

                    preview.Camera.transform.position = origin + offset * distance;
                    preview.Camera.transform.eulerAngles = lastCamera.transform.eulerAngles;
                }
            }

            preview.Camera.fieldOfView = 60.0f;

            if (mOriginalCanvasScaler != null && mPreviewCanvasScaler != null )
            {
                mPreviewCanvasScaler.screenSize = preview.SurfaceRect.size;
                mPreviewCanvasScaler.referenceResolution = mOriginalCanvasScaler.referenceResolution;
                mPreviewCanvasScaler.screenMatchMode = mOriginalCanvasScaler.screenMatchMode;
                mPreviewCanvasScaler.uiScaleMode = mOriginalCanvasScaler.uiScaleMode;
                mPreviewCanvasScaler.dynamicPixelsPerUnit = mOriginalCanvasScaler.dynamicPixelsPerUnit;
                mPreviewCanvasScaler.fallbackScreenDPI = mOriginalCanvasScaler.fallbackScreenDPI;
                mPreviewCanvasScaler.physicalUnit = mOriginalCanvasScaler.physicalUnit;
                mPreviewCanvasScaler.referencePixelsPerUnit = mOriginalCanvasScaler.referencePixelsPerUnit;
                mPreviewCanvasScaler.scaleFactor = mOriginalCanvasScaler.scaleFactor;
                mPreviewCanvasScaler.matchWidthOrHeight = mOriginalCanvasScaler.matchWidthOrHeight;
            }
        }

        private void CreateCanvasInstance()
        {
            if( mCanvas != null )
            {
                preview.Scene.DestroyInstance(mCanvas.gameObject);
                mCanvas = null;
            }
            if( asset.Canvas == null )
            {
                return;
            }

            var instance = preview.Scene.Instantiate(asset.Canvas.gameObject);
            PrepareCanvas(instance);
            mCanvas = instance.GetComponent<Canvas>();
            mOriginalCanvasScaler = asset.Canvas.GetComponent<CanvasScaler>();
            if (mOriginalCanvasScaler != null)
            {
                DestroyImmediate(instance.GetComponent<CanvasScaler>());
                mPreviewCanvasScaler = instance.AddComponent<PreviewCanvasScaler>();
            }
        }
        private void PrepareCanvas( GameObject gameObject )
        {
            var canvas = gameObject.GetComponent<Canvas>();
            if( canvas != null )
            {
                canvas.worldCamera = preview.Camera;
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.planeDistance = 1.0f;
            }

            foreach (Transform child in gameObject.transform)
            {
                PrepareCanvas(child.gameObject);
            }
        }

        private PreviewCanvasScaler mPreviewCanvasScaler;
        private Canvas mCanvas;
        private CanvasScaler mOriginalCanvasScaler;
    }
}