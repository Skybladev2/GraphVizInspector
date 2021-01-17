using UnityEngine;
using UnityEditor;
using StartAssets.PowerfulPreview;
using StartAssets.PowerfulPreview.Utils;

#if true
[CustomEditor(typeof(YourCustomAsset))]
public class YourCustomAssetEditor : PreviewEditor<YourCustomAsset>
{
    protected override void OnCreate()
    {
        //Initialize your data here
    }

    protected override void OnGUIUpdate()
    {
        //Draw your GUI here 
        DrawDefaultInspector();

        Debug.Log(asset);
    }
}
#endif 