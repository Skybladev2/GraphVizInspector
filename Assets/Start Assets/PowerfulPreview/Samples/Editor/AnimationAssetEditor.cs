using UnityEngine;
using UnityEditor;
using StartAssets.PowerfulPreview.Controls;
using StartAssets.PowerfulPreview.Utils;

namespace StartAssets.PowerfulPreview.Samples
{
    /// <summary>
    /// It's an example of how you can play animation on the character 
    /// using PreviewAnimator, but also it shows you how you can add your custom preview
    /// drawers like BonesDrawer. 
    /// </summary>
    [CustomEditor(typeof( AnimationAsset ) ) ]
    public class AnimationAssetEditor : PreviewEditor< AnimationAsset >
    {
        [MenuItem( "Assets/Create/Powerful Preview Samples/Animation Preview Asset" )]
        public static void CreateAnimationPreviewAsset()
        {
            AssetCreator.CreateAsset<AnimationAsset>( "Animation Preview Asset" );
        }
        
        public override void OnPreviewSettings()
        {
            if( mTimeline != null )
            {
                var style = new GUIStyle( EditorStyles.boldLabel );
                style.normal.textColor = Color.white;
                style.fontSize = 11;
                EditorGUILayout.LabelField( "Skeleton visible: ", style, GUILayout.Width( 100 ) );
                if( mSkeletonDrawer == null )
                {
                    EditorGUILayout.Toggle( false, GUILayout.Width( 25 ) );
                }
                else
                {
                    mSkeletonDrawer.Visible = EditorGUILayout.Toggle( mSkeletonDrawer.Visible, GUILayout.Width( 25 ) );
                }
                float playSpeed = mTimeline.Speed;
                EditorGUILayout.LabelField( "Speed: ", style, GUILayout.Width( 50 ) );
                playSpeed = GUILayout.HorizontalSlider( playSpeed, 0.0f, 2.0f, GUILayout.Width( 100 ) );
                EditorGUILayout.LabelField( playSpeed.ToString( "F2" ), style, GUILayout.Width( 30 ) );
                mTimeline.Speed = playSpeed; 
            }
        }

        protected override void OnCreate()
        {
            if( EditorPrefs.HasKey( PreviewObjectKey ) )
            {
                var path = EditorPrefs.GetString( PreviewObjectKey );
                mCustomPreviewGameObject = AssetDatabase.LoadAssetAtPath<GameObject>( path );
            }

            mPreviewAnimator = new PreviewAnimator( preview );
            mPreviewAnimator.Animation = asset.animationClip;
            CreateGameObjectInstance();
            preview.CameraController.Target = Vector3.up * 0.9f;

            mTimeline = new Timeline();
            mTimeline.Visible = asset.animationClip != null;
            preview.AddControl( mTimeline );
            if( asset.animationClip != null )
            {
                mTimeline.EndTime = asset.animationClip.length;
                mTimeline.Framerate = asset.animationClip.frameRate;
            }
            preview.SetButtonsEnabled( true ); 
            preview.CameraController.SetStatesEnabled( true );
        }
        protected override void OnGUIUpdate()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            mCustomPreviewGameObject = EditorGUILayout.ObjectField("Preview: ", mCustomPreviewGameObject, typeof(GameObject), false) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                CreateGameObjectInstance();
            }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("animationClip"), new GUIContent("Animation Clip: "), true);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                mPreviewAnimator.Character.transform.position = Vector3.zero;
                mPreviewAnimator.Character.transform.eulerAngles = Vector3.zero;
                mPreviewAnimator.Animation = asset.animationClip;
                if (mTimeline.Visible)
                {
                    mTimeline.CurFrame = 0;
                    mTimeline.EndTime = asset.animationClip.length;
                    mTimeline.Framerate = asset.animationClip.frameRate;
                }
            }
            mTimeline.Visible = asset.animationClip != null;
            serializedObject.ApplyModifiedProperties();
        } 

        protected void CreateGameObjectInstance()
        {
            if( !preview.CanInstantiate )
            {
                return;
            }

            mPreviewAnimator.Character = mCustomPreviewGameObject;
            mSkeletonDrawer = mPreviewAnimator.Character.AddComponent<BonesDrawer>();
            mSkeletonDrawer.Root = mPreviewAnimator.Animator.GetBoneTransform( HumanBodyBones.Hips );
            if( mCustomPreviewGameObject != null )
            {
                EditorPrefs.SetString( PreviewObjectKey, AssetDatabase.GetAssetPath( mCustomPreviewGameObject ) );
            }
            else
            {
                EditorPrefs.DeleteKey( PreviewObjectKey );
            }
        }
        protected override void OnPreviewUpdate()
        {
            if( mPreviewAnimator == null || mTimeline == null )
            {
                return; 
            }
            mPreviewAnimator.SampleAnimation( mTimeline.CurTime );
        }

        protected const string PreviewObjectKey = "__AnimationAsset__PreviewObjectKey__";

        private BonesDrawer mSkeletonDrawer;
        private PreviewAnimator mPreviewAnimator; 
        private Timeline mTimeline;
        private GameObject mCustomPreviewGameObject;
    }
}