using StartAssets.PowerfulPreview.Controls;
using StartAssets.PowerfulPreview.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StartAssets.PowerfulPreview.Samples
{
    /// <summary>
    /// It's an example of how you can draw inside the preview 
    /// from different cameras at the same time. 
    /// But also it's an example of what you can do without using the PreviewEditor 
    /// as the base class for you custom editor. 
    /// </summary>
    [CustomEditor( typeof( MultiCameraPreviewAsset ) )]
    public class MultiCameraPreviewAssetEditor : Editor
    {
        [MenuItem( "Assets/Create/Powerful Preview Samples/Multi Camera Preview Asset" )]
        public static void CreateAnimationMove()
        {
            AssetCreator.CreateAsset<MultiCameraPreviewAsset>( "Multi Camera Preview Asset" );
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }
        public override bool HasPreviewGUI()
        {
            return true;
        }
        public override void OnInteractivePreviewGUI( Rect r, GUIStyle background )
        {
            if( asset.data == null )
            {
                var label = string.Format( "There is no asset assigned to the preview." );
                var skin = new GUIStyle( GUI.skin.label );
                skin.fontStyle = FontStyle.Bold;
                skin.fontSize = 15;
                var size = skin.CalcSize( new GUIContent( label ) );
                GUI.Label( new Rect( r.center - size / 2, size ), label, skin );
                mRequiresInit = true;
                return;
            }

            if( mRequiresInit )
            {
                InstantiatePlayer( asset.data.firstPlayer, mFirstPlayerPreviewAnimator, ref mFirstPlayerInstance );
                InstantiatePlayer( asset.data.secondPlayer, mSecondPlayerPreviewAnimator, ref mSecondPlayerInstance );
                mRequiresInit = false; 
            }
            HandleAnimations();
            
            
            mTimeline.PositionY = r.yMin;
            mTimeline.PositionX = r.xMin;
            mTimeline.Width = r.width;
            mTimeline.Update();
            mTimeline.Draw();
            if (Event.current.type != EventType.Layout)
            {
                mTimeline.HandleEvents();
            }
            r.yMin += mTimeline.Height + 4;

            var width = r.width / 2;
            var height = r.height / 2;

            var topViewRect = new Rect( r.xMin, r.yMin, width, height );
            var sideViewRect = new Rect( r.xMin + width, r.yMin, width, height );
            var frontViewRect = new Rect( r.xMin, r.yMin + height, width, height );
            var actionViewRect = new Rect( r.xMin + width, r.yMin + height, width, height );

            var rects = new Rect[]
            {
                topViewRect, frontViewRect, sideViewRect, actionViewRect
            };

            var animationTime = Mathf.Min( mTimeline.CurTime, asset.data.cameraAnimation.length );
    
            for( int iRect = 0; iRect < rects.Length; iRect++ )
            {
                var defaultPosition = preview.Camera.transform.position;
                var defaultRotation = preview.Camera.transform.rotation;
                var defaultTarget = preview.CameraController.Target;

                mCameras[iRect].Apply();
                
                if( iRect == ActionCamera )
                {
                    if( asset.data.cameraAnimation != null )
                    {
                        preview.Camera.transform.position = asset.data.cameraPosition;
                        preview.Camera.transform.eulerAngles = Vector3.zero;
                        preview.Camera.orthographic = asset.data.viewType == CutsceneAsset.ViewType.Ortho;
                        preview.Camera.orthographicSize = asset.data.cameraSize;
                        preview.Camera.fieldOfView = asset.data.cameraFov;
                        preview.Camera.SampleAnimation(asset.data.cameraAnimation, animationTime);
                        mCameraFrustumObject.Visible = false;
                    }
                }
                else
                {
                    if( asset.data.cameraAnimation != null )
                    {
                        mCameraFrustumObject.transform.position = asset.data.cameraPosition;
                        mCameraFrustumObject.Orthographic = asset.data.viewType == CutsceneAsset.ViewType.Ortho;
                        mCameraFrustumObject.OrthographicSize = asset.data.cameraSize;
                        mCameraFrustumObject.FieldOfView = asset.data.cameraFov;
                        mCameraFrustumObject.SampleAnimation( asset.data.cameraAnimation, animationTime );
                        preview.Camera.SampleAnimation(null, 0.0f); //To reset the animation for other cameras 
                        mCameraFrustumObject.Visible = true;
                    }
                }

                var surfaceRect = rects[ iRect ];
                preview.SetSurfaceRect( surfaceRect );
                preview.Update();

                preview.Camera.transform.position = defaultPosition;
                preview.Camera.transform.rotation = defaultRotation;
                preview.CameraController.Target = defaultTarget;
            }
        }

        private void OnEnable()
        {
            mRequiresInit = false;
            asset = target as MultiCameraPreviewAsset;
            preview = Preview.Create(this);
            if (preview == null)
            {
                return;
            }
            mCameraFrustumObject = new PreviewCameraFrustumObject(preview);
            mCameraFrustumObject.Visible = true;

            mFirstPlayerPreviewAnimator = new PreviewAnimator(preview);
            mSecondPlayerPreviewAnimator = new PreviewAnimator(preview);

            if (asset.data != null)
            {
                InstantiatePlayer(asset.data.firstPlayer, mFirstPlayerPreviewAnimator, ref mFirstPlayerInstance);
                InstantiatePlayer(asset.data.secondPlayer, mSecondPlayerPreviewAnimator, ref mSecondPlayerInstance);
            }

            mTimeline = new Timeline();
            mTimeline.Playing = false;
            mTimeline.Visible = true;
            preview.CameraController.SetStatesEnabled(true);
            preview.SetButtonsEnabled(true);

            mCameras = new PreviewCameraSetup[4]
            {
                new PreviewCameraSetup( preview ),
                new PreviewCameraSetup( preview ),
                new PreviewCameraSetup( preview ),
                new PreviewCameraSetup( preview )
            };

            mCameras[TopCamera].Orthographic = true;
            mCameras[TopCamera].OrthographicSize = 2;
            mCameras[TopCamera].Position = new Vector3(0, 2, 0);
            mCameras[TopCamera].Rotation = Quaternion.LookRotation(Vector3.down);

            mCameras[FrontCamera].Orthographic = false;
            mCameras[FrontCamera].OrthographicSize = 2;
            mCameras[FrontCamera].Position = new Vector3(0, 1, -4);
            mCameras[FrontCamera].Rotation = Quaternion.LookRotation(Vector3.forward);
            mCameras[FrontCamera].FieldOfView = 65;

            mCameras[SideCamera].Orthographic = false;
            mCameras[SideCamera].OrthographicSize = 2;
            mCameras[SideCamera].Position = new Vector3(-4, 1, 0);
            mCameras[SideCamera].Rotation = Quaternion.LookRotation(Vector3.right);
            mCameras[SideCamera].FieldOfView = 65;

            mCameras[ActionCamera].Orthographic = false;
            mCameras[ActionCamera].Position = Vector3.zero;
            mCameras[ActionCamera].EulerAngles = Vector3.zero;
            mCameras[ActionCamera].FieldOfView = 65;
        }
        private void OnDisable()
        {
            if (preview != null)
            {
                preview.Dispose();
            }
            preview = null;
        }

        private void HandleAnimations()
        {
            var maxAnimationLength = 0.0f;
            if (asset.data.cameraAnimation != null)
            {
                maxAnimationLength = asset.data.cameraAnimation.length;
            }

            if (mTimeline == null)
            {
                return;
            }
            mTimeline.EndTime = maxAnimationLength;
            mTimeline.Visible = asset.data.firstPlayerAnimation != null || asset.data.secondPlayerAnimation != null
                || asset.data.cameraAnimation != null;

            if (mFirstPlayerInstance != null && asset.data.firstPlayerAnimation != null)
            {
                mFirstPlayerInstance.transform.position = asset.data.firstPlayerPosition;
                mFirstPlayerInstance.transform.eulerAngles = asset.data.firstPlayerRotation;
                mFirstPlayerPreviewAnimator.Animation = asset.data.firstPlayerAnimation;
                mFirstPlayerPreviewAnimator.SampleAnimation(
                    Mathf.Min(mTimeline.CurTime, asset.data.firstPlayerAnimation.length));
            }
            if (mSecondPlayerInstance != null && asset.data.secondPlayerAnimation != null)
            {
                mSecondPlayerInstance.transform.position = asset.data.secondPlayerPosition;
                mSecondPlayerInstance.transform.eulerAngles = asset.data.secondPlayerRotation;
                mSecondPlayerPreviewAnimator.Animation = asset.data.secondPlayerAnimation;
                mSecondPlayerPreviewAnimator.SampleAnimation(
                    Mathf.Min(mTimeline.CurTime, asset.data.secondPlayerAnimation.length));
            }
        }
        private void InstantiatePlayer(GameObject prefab, PreviewAnimator animator, ref GameObject instance)
        {
            if (preview == null || !preview.CanInstantiate)
            {
                return;
            }
            //This can't be simplified wtih prefab ?? PreviewAnimator.DefaultUnityCharacter
            //for some reason it doesn't work in Unity 2017.3.0f3
            animator.Character = (prefab == null) ? PreviewAnimator.DefaultUnityCharacter : prefab;
            instance = animator.Character;
        }
        
        private MultiCameraPreviewAsset asset; 

        private PreviewCameraSetup[] mCameras;
        private PreviewCameraFrustumObject mCameraFrustumObject;
        private Preview preview;

        private Timeline mTimeline;
        private GameObject mFirstPlayerInstance;
        private GameObject mSecondPlayerInstance;

        private PreviewAnimator mFirstPlayerPreviewAnimator;
        private PreviewAnimator mSecondPlayerPreviewAnimator;

        private GUIStyle mTitleStyle;

        private const int TopCamera = 0;
        private const int FrontCamera = 1;
        private const int SideCamera = 2;
        private const int ActionCamera = 3;

        private bool mRequiresInit; 
    }
}