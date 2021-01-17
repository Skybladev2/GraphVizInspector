using UnityEditor;
using UnityEngine;
using StartAssets.PowerfulPreview.Controls;
using StartAssets.PowerfulPreview.Utils;

namespace StartAssets.PowerfulPreview.Samples
{
    /// <summary>
    /// It's an example of how you can play animation on the camera, 
    /// characters, but also how you can animate the frustum of the camera,
    /// and show it in the preview. 
    /// </summary>
    [CustomEditor( typeof( CutsceneAsset ) )]
    public class CutsceneAssetEditor : PreviewEditor<CutsceneAsset>
    {
        [MenuItem( "Assets/Create/Powerful Preview Samples/Cutscene Preview Asset" )]
        public static void CreateCutscenePreviewAsset()
        {
            AssetCreator.CreateAsset<CutsceneAsset>( "Cutscene Preview Asset" );
        }
 
        public override void OnPreviewSettings()
        {
            if( asset.cameraAnimation != null )
            {
                var boldLabelStyle = new GUIStyle( EditorStyles.boldLabel );
                boldLabelStyle.normal.textColor = Color.white; 
                boldLabelStyle.fontSize = 11;
                EditorGUILayout.LabelField( "Camera View: ", boldLabelStyle, GUILayout.Width( 85 ) );
                EditorGUI.BeginChangeCheck();
                mPreviewCameraView = EditorGUILayout.Toggle( mPreviewCameraView, EditorStyles.toggle, GUILayout.Width( 15 ) );
                if( EditorGUI.EndChangeCheck() )
                {
                    if( !mPreviewCameraView )
                    {
                        preview.Camera.orthographic = false;
                    }
                    else
                    {
                        preview.Camera.transform.position = asset.cameraPosition;
                        preview.Camera.transform.eulerAngles = Vector3.zero; 
                        preview.Camera.orthographic = asset.viewType == CutsceneAsset.ViewType.Ortho;
                        preview.Camera.orthographicSize = asset.cameraSize;
                        preview.Camera.fieldOfView = asset.cameraFov;
                    }
                    SetupPreviewCamera();
                    mCameraFrustumObject.Visible = !mPreviewCameraView;
                }
               
            }
        }
        public override string GetInfoString()
        {
            if( mTimeline != null )
            {
                return string.Format( "Current frame: {0} ({1}%)", mTimeline.CurFrame, mTimeline.CurNormalTime * 100 );
            }
            return base.GetInfoString();
        }
        public override GUIContent GetPreviewTitle()
        {
            return new GUIContent( "It's a cool cutscene asset." );
        }

        protected override void OnCreate()
        {
            mFirstPlayerPreviewAnimator = new PreviewAnimator( preview );
            mSecondPlayerPreviewAnimator = new PreviewAnimator( preview );

            InstantiatePlayer( asset.firstPlayer, mFirstPlayerPreviewAnimator, ref mFirstPlayerInstance );
            InstantiatePlayer( asset.secondPlayer, mSecondPlayerPreviewAnimator, ref mSecondPlayerInstance );

            mTimeline = new Timeline();
            preview.AddControl( mTimeline );
            preview.CameraController.SetStatesEnabled( true );
            preview.SetButtonsEnabled( true );

            mCameraFrustumObject = new PreviewCameraFrustumObject(preview);
            mCameraFrustumObject.Visible = true;
        }
        protected override void OnGUIUpdate()
        {
            if( mTitleStyle == null )
            {
                mTitleStyle = new GUIStyle( EditorGUIUtility.isProSkin ? EditorStyles.whiteLabel : EditorStyles.label );
                mTitleStyle.fontStyle = FontStyle.Bold;
                mTitleStyle.fontSize = 15;
            }
            DrawFirstPlayerArea();
            DrawSecondPlayerArea();
            DrawCameraArea();
            
        }
        protected override void OnPreviewUpdate()
        {
            HandleAnimations();
        }

        protected void DrawFirstPlayerArea()
        {
            EditorGUILayout.LabelField( "First Player", mTitleStyle, GUILayout.Height( 25 ) );
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField( serializedObject.FindProperty( "firstPlayer" ), true );
            if( EditorGUI.EndChangeCheck() )
            {
                asset.firstPlayer = serializedObject.FindProperty( "firstPlayer" ).objectReferenceValue as GameObject;
                InstantiatePlayer( asset.firstPlayer, mFirstPlayerPreviewAnimator, ref mFirstPlayerInstance );
            }

            EditorGUILayout.PropertyField( serializedObject.FindProperty( "firstPlayerAnimation" ), true );
            EditorGUILayout.PropertyField( serializedObject.FindProperty( "firstPlayerPosition" ), true );
            EditorGUILayout.PropertyField( serializedObject.FindProperty( "firstPlayerRotation" ), true );
        }
        protected void DrawSecondPlayerArea()
        {
            EditorGUILayout.LabelField( "Second Player", mTitleStyle, GUILayout.Height( 25 ) );
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField( serializedObject.FindProperty( "secondPlayer" ), true );
            if( EditorGUI.EndChangeCheck() )
            {
                asset.secondPlayer = serializedObject.FindProperty( "secondPlayer" ).objectReferenceValue as GameObject;
                InstantiatePlayer( asset.secondPlayer, mSecondPlayerPreviewAnimator, ref mSecondPlayerInstance );
            }
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField( serializedObject.FindProperty( "secondPlayerAnimation" ), true );
            }
            EditorGUILayout.PropertyField( serializedObject.FindProperty( "secondPlayerPosition" ), true );
            EditorGUILayout.PropertyField( serializedObject.FindProperty( "secondPlayerRotation" ), true );
        }
        protected void DrawCameraArea()
        {
            EditorGUILayout.LabelField( "Camera", mTitleStyle, GUILayout.Height( 25 ) );
            EditorGUILayout.PropertyField( serializedObject.FindProperty( "viewType" ), true );
            if( asset.viewType == CutsceneAsset.ViewType.Perspective )
            {
                EditorGUILayout.PropertyField( serializedObject.FindProperty( "cameraFov" ), true );
            }
            else
            {
                EditorGUILayout.PropertyField( serializedObject.FindProperty( "cameraSize" ), true );
            }
            EditorGUILayout.PropertyField( serializedObject.FindProperty( "cameraAnimation" ), true );
            asset.cameraAnimation = serializedObject.FindProperty( "cameraAnimation" ).objectReferenceValue as AnimationClip;

            EditorGUILayout.PropertyField( serializedObject.FindProperty( "cameraPosition" ), true );
        }
        
        protected void HandleAnimations()
        {
            var maxAnimationLength = 0.0f;
            if( asset.firstPlayerAnimation != null )
            {
                maxAnimationLength = Mathf.Max( asset.firstPlayerAnimation.length, maxAnimationLength );
            }
            if( asset.secondPlayerAnimation != null )
            {
                maxAnimationLength = Mathf.Max( asset.secondPlayerAnimation.length, maxAnimationLength );
            }
            if( asset.cameraAnimation != null && mPreviewCameraView )
            {
                maxAnimationLength = Mathf.Max( asset.cameraAnimation.length, maxAnimationLength );
            }

            if( mTimeline == null )
            {
                return; 
            }
            mTimeline.EndTime = maxAnimationLength;
            mTimeline.Visible = asset.firstPlayerAnimation != null || asset.secondPlayerAnimation != null
                || asset.cameraAnimation != null;

            if( mFirstPlayerInstance != null )
            {
                mFirstPlayerInstance.transform.position = asset.firstPlayerPosition;
                mFirstPlayerInstance.transform.eulerAngles = asset.firstPlayerRotation;
                mFirstPlayerPreviewAnimator.Animation = asset.firstPlayerAnimation;
                if( asset.firstPlayerAnimation != null)
                {
                    mFirstPlayerPreviewAnimator.SampleAnimation(
                        Mathf.Min( mTimeline.CurTime, asset.firstPlayerAnimation.length ) );
                }
            }
            if( mSecondPlayerInstance != null )
            {
                mSecondPlayerInstance.transform.position = asset.secondPlayerPosition;
                mSecondPlayerInstance.transform.eulerAngles = asset.secondPlayerRotation;
                mSecondPlayerPreviewAnimator.Animation = asset.secondPlayerAnimation;
                if( asset.secondPlayerAnimation != null )
                {
                    mSecondPlayerPreviewAnimator.SampleAnimation(
                        Mathf.Min( mTimeline.CurTime, asset.secondPlayerAnimation.length ) );
                }
            }
            if( asset.cameraAnimation != null )
            {
                var animationTime = Mathf.Min( mTimeline.CurTime, asset.cameraAnimation.length );

                if( mPreviewCameraView )
                {
                    preview.Camera.transform.position = asset.cameraPosition;
                    preview.Camera.transform.eulerAngles = Vector3.zero;
                    preview.Camera.orthographic = asset.viewType == CutsceneAsset.ViewType.Ortho;
                    preview.Camera.orthographicSize = asset.cameraSize;
                    preview.Camera.fieldOfView = asset.cameraFov;
                    preview.Camera.SampleAnimation( asset.cameraAnimation, animationTime );
                }
                else
                {
                    mCameraFrustumObject.transform.position = asset.cameraPosition;
                    mCameraFrustumObject.Orthographic = asset.viewType == CutsceneAsset.ViewType.Ortho;
                    mCameraFrustumObject.OrthographicSize = asset.cameraSize;
                    mCameraFrustumObject.FieldOfView = asset.cameraFov;
                    mCameraFrustumObject.SampleAnimation( asset.cameraAnimation, animationTime );
                    preview.Camera.SampleAnimation(null, 0.0f);
                }
            }

        }
  
        protected void InstantiatePlayer( GameObject prefab, PreviewAnimator animator, ref GameObject instance )
        {
            if( !preview.CanInstantiate )
            {
                return;
            }
            //This can't be simplified wtih prefab ?? PreviewAnimator.DefaultUnityCharacter
            //for some reason it doesn't work in Unity 2017.3.0f3
            animator.Character = ( prefab == null ) ? PreviewAnimator.DefaultUnityCharacter : prefab;
            instance = animator.Character; 
        }

        private PreviewCameraFrustumObject mCameraFrustumObject; 

        private PreviewAnimator mFirstPlayerPreviewAnimator;
        private PreviewAnimator mSecondPlayerPreviewAnimator; 
        private Timeline mTimeline;
        private GameObject mFirstPlayerInstance;
        private GameObject mSecondPlayerInstance;
        private GUIStyle mTitleStyle; 
        private bool mPreviewCameraView = false; 
    }
}