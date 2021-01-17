using UnityEngine;
using UnityEditor;
using StartAssets.PowerfulPreview.Controls;
using StartAssets.PowerfulPreview.Utils;

namespace StartAssets.PowerfulPreview.Samples
{
    /// <summary>
    /// An example of the preview editor which draws preview for some specific 
    /// prefab which contains a particle system inside. 
    /// </summary>
    [CustomEditor(typeof(ParticleSystemPreviewAsset))]
    public class ParticleSystemPreviewAssetEditor : TimelinePreviewEditor<ParticleSystemPreviewAsset>
    {
        [MenuItem("Assets/Create/Powerful Preview Samples/Particle System Preview Asset")]
        public static void CreateParticleSystemPreviewAsset()
        {
            AssetCreator.CreateAsset<ParticleSystemPreviewAsset>("Particle System Preview Asset");
        }

        protected override void OnCreate()
        {
            CreateParticleSystemProvider();

            preview.Camera.orthographic = true;
            preview.Camera.transform.eulerAngles = Vector3.zero;
            preview.Camera.transform.position = Vector3.zero;
            SetupPreviewCamera();
            preview.CameraController.SetStatesEnabled(false);
        }
        protected override void OnGUIUpdate()
        {
            var particleSystemProperty = serializedObject.FindProperty("m_ParticleSystemPrefab");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(particleSystemProperty);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                CreateParticleSystemProvider();
            }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Duration"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                Timeline.EndTime = asset.Duration;
            }

            mParticleSystemProvider?.Simulate(Timeline.CurNormalTime);
        }

        private void CreateParticleSystemProvider()
        {
            if (mParticleSystemProvider != null && mParticleSystemProvider.GameObject != null)
            {
                preview.Scene.DestroyInstance(mParticleSystemProvider.GameObject);
                mParticleSystemProvider = null;
            }
            if (asset.ParticleSystemPrefab != null)
            {
                var instance = preview.Scene.Instantiate(asset.ParticleSystemPrefab);
                instance.transform.localPosition += Vector3.up;
                mParticleSystemProvider = new ParticleSystemAnimator(instance);
                Timeline.EndTime = asset.Duration;
            }
            Timeline.Visible = asset.ParticleSystemPrefab != null;
        }

        private ParticleSystemAnimator mParticleSystemProvider;
    }
}