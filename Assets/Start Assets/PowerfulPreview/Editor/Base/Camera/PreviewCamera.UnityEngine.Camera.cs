using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// This part of the PreviewCamera class contains only copy of the UnityEngine.Camera interface. 
    /// The documentation for those methods and properties you can find in the unity scrip reference
    /// Url: https://docs.unity3d.com/ScriptReference/Camera.html
    /// 
    /// This specific file might be extended later to fit all the new UnityEngine.Camera features, 
    /// but for now it's sufficient to fit all the preview needs. 
    /// </summary>
    public partial class PreviewCamera
    {
        public Color backgroundColor
        {
            set
            {
                mMainCamera.backgroundColor = value;
            }
            get
            {
                return mMainCamera.backgroundColor;
            }
        }
        public Matrix4x4 cameraToWorldMatrix
        {
            get
            {
                return mMainCamera.cameraToWorldMatrix;
            }
        }
        public CameraType cameraType
        {
            get
            {
                return CameraType.Preview;
            }
        }
        public float farClipPlane
        {
            set
            {
                mMainCamera.farClipPlane = value;
                mGizmoCamera.farClipPlane = value;
                mHandlesCamera.farClipPlane = value;
            }
            get
            {
                return mMainCamera.farClipPlane;
            }
        }
        public float nearClipPlane
        {
            set
            {
                mMainCamera.nearClipPlane = value;
                mGizmoCamera.nearClipPlane = value;
                mHandlesCamera.nearClipPlane = value;
            }
            get
            {
                return mMainCamera.nearClipPlane;
            }
        }

        public int pixelHeight
        {
            get
            {
                return mMainCamera.pixelHeight;
            }
        }
        public int pixelWidth
        {
            get
            {
                return mMainCamera.pixelWidth;
            }
        }
        public Rect pixelRect
        {
            get
            {
                return mMainCamera.pixelRect;
            }
        }
        public Rect rect
        {
            get
            {
                return mMainCamera.rect;
            }
        }
        public Vector3 velocity
        {
            get
            {
                return mMainCamera.velocity;
            }
        }

        public float fieldOfView
        {
            set
            {
                mMainCamera.fieldOfView = value;
                mGizmoCamera.fieldOfView = value;
                mHandlesCamera.fieldOfView = value;
            }
            get
            {
                return mMainCamera.fieldOfView;
            }
        }
        public bool orthographic
        {
            set
            {
                mMainCamera.orthographic = value;
                mGizmoCamera.orthographic = value;
                mHandlesCamera.orthographic = value;
            }
            get
            {
                return mMainCamera.orthographic;
            }
        }
        public float orthographicSize
        {
            set
            {
                mMainCamera.orthographicSize = value;
                mGizmoCamera.orthographicSize = value;
                mHandlesCamera.orthographicSize = value;
            }
            get
            {
                return mMainCamera.orthographicSize;
            }
        }
        public float maxOrthographicSize
        {
            set
            {
                maxOrthographicSize = value;
            }
            get
            {
                return maxOrthographicSize;
            }
        }
        public float minOrthographicSize
        {
            set
            {
                minOrthographicSize = value;
            }
            get
            {
                return minOrthographicSize;
            }
        }
        public Transform transform
        {
            get
            {
                return mTransformCameraContainer.transform;
            }
        }

        public Vector3 WorldToViewportPoint(Vector3 position)
        {
            return mMainCamera.WorldToViewportPoint(position);
        }
        public Vector3 WorldToScreenPoint(Vector3 position)
        {
            return mMainCamera.WorldToScreenPoint(position);
        }
        public Vector3 ViewportToWorldPoint(Vector3 position)
        {
            return mMainCamera.ViewportToWorldPoint(position);
        }
        public Vector3 ViewportToScreenPoint(Vector3 position)
        {
            return mMainCamera.ViewportToScreenPoint(position);
        }
        public Ray ViewportPointToRay(Vector3 position)
        {
            return mMainCamera.ViewportPointToRay(position);
        }
        public Vector3 ScreenToWorldPoint(Vector3 position)
        {
            return mMainCamera.ScreenToWorldPoint(position);
        }
        public Vector3 ScreenToViewportPoint(Vector3 position)
        {
            return mMainCamera.ScreenToViewportPoint(position);
        }
        public Ray ScreenPointToRay(Vector3 position)
        {
            return mMainCamera.ScreenPointToRay(position);
        }

        /// <summary>
        /// Casts PreviewCamera to UnityEngine.Camera. 
        /// </summary>
        /// <returns>The main camera.</returns>
        public static implicit operator UnityEngine.Camera(PreviewCamera previewCamera)
        {
            return previewCamera.mMainCamera;
        }
    }
}