
using UnityEngine;
using System;
using System.Collections.Generic;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// This class implements a preview object. Preview object is drawn inside the preview, 
    /// and it's not visible for other cameras. 
    /// </summary>
	[ExecuteInEditMode()]
    [AddComponentMenu( "" )]
    public class PreviewObject : MonoBehaviour
	{
        /// <summary>
        /// Is the object visible. 
        /// </summary>
        public bool Visible
        {
            set;
            get;
        }

        /// <summary>
        ///Set's up the renderer material. It's useful, when you want create object 
        ///from scratch with some custom material. 
        /// </summary>
        public Material RendererMaterial
        {
            set
            {
                if( mMaterials == null )
                {
                    mMaterials = new Material[ 1 ];
                }
                mMaterials[ 0 ] = value; 
            }
        }
        
        /// <summary>
        /// Initializes all the need data, prepares 
        /// materials and then makes all children 
        /// game object to be preview objects too.
        /// </summary>
        /// <param name="gizmo">Is the object should be drawn on the gizmo layer?</param>
        /// <param name="root">Is this object is a root object in the hierarchy?</param>
        public void Run( bool gizmo = false, bool root = true )
        {
            Visible = true;
            if( root )
            {
                if (gizmo)
                {
                    mCameraToDraw = "PREVIEW__GizmoCamera";
                }
                else
                {
                    mCameraToDraw = "PREVIEW__MainCamera";
                }
            }
            mInvisibleMaterial = new Material( Shader.Find( "Transparent/Diffuse" ) );
            mInvisibleMaterial.color = new Color( 1.0f, 1.0f, 1.0f, 0.0f );

            mRenderer = gameObject.GetComponent< SkinnedMeshRenderer >();
            if( mRenderer == null )
            {
                mRenderer = gameObject.GetComponent<Renderer>();
            }
            if( mRenderer != null )
            {
                mMaterials = new Material[ mRenderer.sharedMaterials.Length ];
                for( int i = 0; i < mRenderer.sharedMaterials.Length; i++ )
                {
                    if( mRenderer.sharedMaterials[ i ] != null )
                    {
                        mMaterials[ i ] = new Material( mRenderer.sharedMaterials[ i ] );
                        mMaterials[ i ].shader = mRenderer.sharedMaterials[ i ].shader;
                    }
                    else
                    {
                        mMaterials[ i ] = new Material( Shader.Find( "Transparent/Diffuse" ) );
                    }
                }
            }

            if( root )
            {
                mCameraUUID = gameObject.name.Substring( gameObject.name.LastIndexOf( '_' ) );
                mCameraToDraw += mCameraUUID;
            }
            foreach( Transform child in transform )
            {
                PreviewObject previewObject = child.gameObject.GetComponent<PreviewObject>();
                if( previewObject == null )
                {
                    previewObject = child.gameObject.AddComponent<PreviewObject>();
                }
                previewObject.mCameraToDraw = mCameraToDraw; 
                previewObject.Run( gizmo, false );
            }
        }

        protected void Update()
        {
            gameObject.layer = 1;
        }

        protected void OnWillRenderObject()
		{
            if( Camera.current == null )
            {
                return;
            }
            bool shouldDraw = Camera.current.name.Equals( mCameraToDraw ) && Visible;
            if ( mRenderer != null )
            {
                var m = mRenderer.sharedMaterials;
                for( int i = 0; i < mMaterials.Length; i++ )
                {
                    if( shouldDraw )
                    {
                        m[ i ] = mMaterials[ i ];
                    }
                    else
                    {
                        m[ i ] = mInvisibleMaterial;
                    }
                }
                mRenderer.sharedMaterials = m;
            }
		}

        private Renderer mRenderer;
		private Material[] mMaterials;
		private Material mInvisibleMaterial;
        private string mCameraToDraw;
        private string mCameraUUID = ""; 
    }
}