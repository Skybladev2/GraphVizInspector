using UnityEngine;

namespace StartAssets.PowerfulPreview.Samples
{
    public class CanvasPreviewAsset : ScriptableObject
    {
        public Canvas Canvas
        {
            get
            {
                return m_Canvas; 
            }
        }

        [SerializeField]
        private Canvas m_Canvas;
    }
}