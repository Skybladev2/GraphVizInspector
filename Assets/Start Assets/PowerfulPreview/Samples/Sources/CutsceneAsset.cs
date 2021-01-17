using UnityEngine;

namespace StartAssets.PowerfulPreview.Samples
{
    public class CutsceneAsset : ScriptableObject
    {
        public GameObject firstPlayer;
        public AnimationClip firstPlayerAnimation;
        public Vector3 firstPlayerPosition = Vector3.zero;
        public Vector3 firstPlayerRotation = Vector3.zero;
        public GameObject secondPlayer;
        public AnimationClip secondPlayerAnimation;
        public Vector3 secondPlayerPosition = Vector3.zero;
        public Vector3 secondPlayerRotation = Vector3.zero;

        public enum ViewType
        {
            Perspective,
            Ortho
        }

        public AnimationClip cameraAnimation;
        public ViewType viewType = ViewType.Perspective;
        public float cameraFov = 60;
        public float cameraSize = 1;
        public Vector3 cameraPosition = Vector3.zero;
    }
}