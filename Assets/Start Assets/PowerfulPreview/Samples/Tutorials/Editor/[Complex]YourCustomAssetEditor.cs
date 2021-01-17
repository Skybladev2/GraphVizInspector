using UnityEngine;
using UnityEditor;
using StartAssets.PowerfulPreview;

#if false
[CustomEditor(typeof(YourCustomAsset))]
public class YourCustomAssetEditor : Editor
{
    public override bool RequiresConstantRepaint()
    {
        return true;
    }
    public override bool HasPreviewGUI()
    {
        return true; 
    }

    public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
    {
        mPreview?.SetSurfaceRect(r);
        mPreview?.Update();
        
        //Do other things with the preview if you need...
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Draw custom inspector GUI if you need. 
    }

    private void OnEnable()
    {
        mPreview = Preview.Create(this);
    }
    private void OnDisable()
    {
        mPreview?.Dispose();
    }

    private Preview mPreview; 
}
#endif 