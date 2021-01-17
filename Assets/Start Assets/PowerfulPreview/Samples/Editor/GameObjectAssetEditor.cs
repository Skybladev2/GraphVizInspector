using StartAssets.PowerfulPreview.Utils;
using UnityEditor;
using UnityEngine;

namespace StartAssets.PowerfulPreview.Samples
{
    /// <summary>
    /// An example preview editor that shows how to set up the camera, the preview 
    /// and handle the preview events. 
    /// </summary>
    [CustomEditor(typeof( GameObjectAsset ) ) ]
    public class GameObjectAssetEditor : PreviewEditor< GameObjectAsset >
    {
        [MenuItem( "Assets/Create/Powerful Preview Samples/Game Object Preview Asset" )]
        public static void CreateGameObjectPreviewAsset()
        {
            AssetCreator.CreateAsset<GameObjectAsset>( "Game Object Preview Asset" );
        }

        public override void OnPreviewSettings()
        {
            var style = new GUIStyle( EditorStyles.boldLabel );
            style.normal.textColor = Color.white; 
            style.fontSize = 11;

            //Here we add a view type popup, so you can change it. 
            mViewType = ( ViewType )EditorGUILayout.EnumPopup( mViewType, GUILayout.Width( 90 ) );
            if( mViewType == ViewType.Perspective )
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField( "Fov: ", style, GUILayout.Width( 30 ) );
                mFov = EditorGUILayout.FloatField( mFov, GUILayout.Width( 30 ) );
                if( EditorGUI.EndChangeCheck() )
                {
                    preview.Camera.fieldOfView = mFov;
                }
            }
            else
            {
                EditorGUILayout.LabelField( "Size: ", style, GUILayout.Width( 30 ) );
                EditorGUI.BeginChangeCheck();
                mSize = EditorGUILayout.FloatField( mSize, GUILayout.Width( 30 ) );
                if( EditorGUI.EndChangeCheck() )
                {
                    preview.Camera.orthographicSize = mSize;
                }
            }
            if( preview.Camera != null )
            {
                preview.Camera.orthographic = mViewType == ViewType.Ortho;
            }
            var buttonStyle = EditorStyles.miniButton;
            if( GUILayout.Button( "Reset", buttonStyle ) )
            {
                SetupPreviewCamera();
            }
        }

        protected override void OnCreate()
        {
            CreateGameObjectInstance();
            preview.OnBeforeDraw += OnBeforeDrawEvent;
            preview.OnDraw += OnDrawEvent;
            preview.OnAfterDraw += OnAfterDrawEvent;

            preview.OnMouseDown += OnMouseDownEvent;
            preview.OnMouseUp += OnMouseUpEvent;
            preview.OnMouseDrag += OnMouseDragEvent;
            preview.OnScrollWheel += OnScrollWheelEvent;
            preview.AddHandler(new CameraOrientationGizmo());

            if( preview.CanInstantiate )
            {
                mGizmoInstance = GameObject.CreatePrimitive( PrimitiveType.Cube );
                mGizmoInstance.transform.localScale = Vector3.one * 0.5f;
                mGizmoInstance.transform.position = new Vector3( 0, 0, -2 );
                preview.Scene.AddObject( mGizmoInstance, new Material( Shader.Find( "Legacy Shaders/VertexLit" ) ), true );
            }
        }
        protected override void OnDisable()
        {
            if( preview != null )
            {
                preview.OnBeforeDraw -= OnBeforeDrawEvent;
                preview.OnDraw -= OnDrawEvent;
                preview.OnAfterDraw -= OnAfterDrawEvent;

                preview.OnMouseDown -= OnMouseDownEvent;
                preview.OnMouseUp -= OnMouseUpEvent;
                preview.OnMouseDrag -= OnMouseDragEvent;
                preview.OnScrollWheel -= OnScrollWheelEvent;

            }
            base.OnDisable();
        }
        protected override void OnGUIUpdate()
        {
            DrawPreviewSettingsArea();
            GUILayout.Space( 5 );
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField( serializedObject.FindProperty( "gameObject" ) );
            asset.gameObject = serializedObject.FindProperty( "gameObject" ).objectReferenceValue as GameObject;
            if( EditorGUI.EndChangeCheck() )
            {
                CreateGameObjectInstance();
            }
        }

        protected override void SetupPreviewCamera()
        {
            base.SetupPreviewCamera();
            if( preview.Camera == null )
            {
                return; 
            }
            mSize = preview.Camera.orthographicSize;
            mFov = preview.Camera.fieldOfView;
            mViewType = preview.Camera.orthographic ? ViewType.Ortho : ViewType.Perspective;
        }
        
        protected void DrawPreviewSettingsArea()
        {
            if( mTitleStyle == null )
            {
                mTitleStyle = EditorStyles.boldLabel; 
                mTitleStyle.fontSize = 15;
            }

            EditorGUILayout.LabelField( "Navigation Settings", mTitleStyle, GUILayout.Height( 25 ) );
            mEnableDragging = EditorGUILayout.Toggle( "Enable dragging", mEnableDragging );
            mEnableRotating = EditorGUILayout.Toggle( "Enable rotating", mEnableRotating );
            mEnableZooming = EditorGUILayout.Toggle( "Enable zooming", mEnableZooming );

            EditorGUILayout.LabelField( "Input Settings", mTitleStyle, GUILayout.Height( 25 ) );
            mEnableLeftMouse = EditorGUILayout.Toggle( "Enable LMB", mEnableLeftMouse );
            mEnableMiddleMouse = EditorGUILayout.Toggle( "Enable MMB", mEnableMiddleMouse );
            mEnableRightMouse = EditorGUILayout.Toggle( "Enable RMB", mEnableRightMouse );
            mEnableScrollWheel = EditorGUILayout.Toggle( "Enable scroll wheel", mEnableScrollWheel );

            preview.SetButtonEnabled( Preview.Buttons.Left, mEnableLeftMouse );
            preview.SetButtonEnabled( Preview.Buttons.Right, mEnableRightMouse );
            preview.SetButtonEnabled( Preview.Buttons.Middle, mEnableMiddleMouse );
            preview.ScrollWheelEnabled = mEnableScrollWheel;

            preview.CameraController.SetStateEnabled( PreviewCameraStates.Dragging, mEnableDragging );
            preview.CameraController.SetStateEnabled( PreviewCameraStates.Rotating, mEnableRotating );
            preview.CameraController.SetStateEnabled( PreviewCameraStates.Zooming, mEnableZooming );
        }

        protected void CreateGameObjectInstance()
        {
            if( mGameObjectInstance != null )
            {
                preview.Scene.DestroyInstance( mGameObjectInstance );
            }
            if( !preview.CanInstantiate )
            {
                return;
            }
            if( asset.gameObject != null )
            {
                mGameObjectInstance = preview.Scene.Instantiate( asset.gameObject );
                var animator = mGameObjectInstance.GetComponent< Animator >();
                if( animator != null )
                {
                    DestroyImmediate( animator );
                }
            }
        }

        protected void OnBeforeDrawEvent( Rect r )
        {
            r.yMin += r.height / 2 - 20;
            r.xMin += r.width / 4;
            GUI.Label( r, "On before draw label. Almost invisible, cause it is drawn before everything.", EditorStyles.boldLabel );
        }
        protected void OnDrawEvent( Rect r )
        {
            r.yMin += r.height / 2;
            r.xMin += r.width / 4;
            GUI.Label( r, "On draw label. It is drawn with all main object. It is covered by gizmo layer.", EditorStyles.boldLabel );
        }
        protected void OnAfterDrawEvent( Rect r )
        {
            r.yMin += r.height / 2 + 20;
            r.xMin += r.width / 4;
            GUI.Label( r, "On after draw label.", EditorStyles.boldLabel );
        }
        
        protected void OnMouseDownEvent( UnityEngine.Event e )
        {
            Debug.Log( "On mouse down event(pos): " + e.mousePosition );
        }
        protected void OnMouseUpEvent( UnityEngine.Event e )
        {
            Debug.Log( "On mouse up event(button)" );
        }
        protected void OnMouseDragEvent( UnityEngine.Event e )
        {
            Debug.Log( "On mouse drag event(delta): " + e.delta );
        }
        protected void OnScrollWheelEvent( UnityEngine.Event e )
        {
            Debug.Log( "On scroll wheel event: " + e.delta );
        }

        private enum ViewType
        {
            Perspective,
            Ortho
        }

        private GUIStyle mTitleStyle;
        private ViewType mViewType = ViewType.Perspective;

        private GameObject mGameObjectInstance;
        private GameObject mGizmoInstance; 

        private float mFov = 60.0f;
        private float mSize = 1.0f;

        private bool mEnableDragging = true;
        private bool mEnableRotating = true;
        private bool mEnableZooming = true;

        private bool mEnableLeftMouse = true;
        private bool mEnableMiddleMouse = true;
        private bool mEnableRightMouse = true;
        private bool mEnableScrollWheel = true;
    }
}