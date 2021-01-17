using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StartAssets.PowerfulPreview.Samples
{
    public class MultiViewAsset : ScriptableObject
    {
        public ScriptableObject[] assets = new ScriptableObject[4];

        public CutsceneAsset cutsceneData;
        public AnimationAsset animationData;
        public GameObjectAsset gameObjectData;
        public MultiCameraPreviewAsset multiCameraData; 
    }
}