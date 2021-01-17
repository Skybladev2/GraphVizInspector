using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Inspector for .SVG assets
/// </summary>
[CustomEditor(typeof(DefaultAsset))]
public class GraphVizEditor : Editor
{
    public override bool HasPreviewGUI()
    {
        return true;
    }
    
    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        var path = AssetDatabase.GetAssetPath(target);
        if (path.EndsWith(".svg"))
        {
            Debug.Log(path);            
            ReadXml(path);
            
            //var texture = new Texture2D(100, 100);
            //texture.SetPixel(1, 1, Color.red);
            //texture.Apply();
            //GUI.DrawTexture(r, texture, ScaleMode.StretchToFill, false);
        }
    }

    private void ReadXml(string path)
    {
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.DtdProcessing = DtdProcessing.Parse;
        XmlReader reader = XmlReader.Create(path, settings);

        reader.MoveToContent();
        var gId = "";
        var ellipseId = "";
        var pathId = "";
        var polygonId = "";
        GameObject ellipseGameObject = null;
        GameObject pathGameObject = null;
        GameObject polygonGameObject = null;
        LineRenderer lr = null;
        MeshRenderer mr = null;
        MeshFilter mf = null;

        // Parse the file and display each of the nodes.
        while (reader.Read())
        {
            
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    //Debug.Log($"<{reader.Name}>");
                    
                    if(reader.Name == "g")
                    {
                        var @class = reader.GetAttribute("class");
                        if(@class == "graph")
                        {
                            continue;
                        }
                        gId = reader.GetAttribute("id");
                        ellipseId = gId + ".ellipse";
                        ellipseGameObject = GameObject.Find(ellipseId);
                        if(ellipseGameObject == null)
                        {
                            ellipseGameObject = new GameObject(ellipseId);
                        }
                        pathId = gId + ".path";
                        pathGameObject = GameObject.Find(pathId);
                        if (pathGameObject == null)
                        {
                            pathGameObject = new GameObject(pathId);
                        }
                        polygonId = gId + ".polygon";
                        polygonGameObject = GameObject.Find(polygonId);
                        if (polygonGameObject == null)
                        {
                            polygonGameObject = new GameObject(polygonId);
                        }
                        continue;
                    }
                    if (reader.Name == "ellipse")
                    {
                        if(ellipseGameObject == null)
                        {
                            continue;
                        }
                        var cx = float.Parse(reader.GetAttribute("cx"), CultureInfo.InvariantCulture);
                        var cy = -float.Parse(reader.GetAttribute("cy"), CultureInfo.InvariantCulture);
                        var rx = float.Parse(reader.GetAttribute("rx"), CultureInfo.InvariantCulture);
                        var ry = float.Parse(reader.GetAttribute("ry"), CultureInfo.InvariantCulture);
                        lr = ellipseGameObject.GetComponent<LineRenderer>();
                        if (lr == null)
                        {
                            lr = ellipseGameObject.AddComponent<LineRenderer>();
                        }
                        SetEllipseRadius(lr, cx, cy, rx, ry);
                        continue;
                    }
                    if (reader.Name == "path")
                    {
                        if(pathGameObject == null)
                        {
                            continue;
                        }
                        var d = reader.GetAttribute("d");
                        var dParts = d.Split('C');
                        var movePart = dParts[0].Replace("M", "");
                        var curvePart = dParts[1];
                        var firstPair = movePart.Split(',');
                        var restPairs = curvePart.Split(' ', ',');
                        var coordStrings = firstPair.Concat(restPairs).ToArray();
                        var coords = new Vector3[coordStrings.Count() / 2];
                        lr = pathGameObject.GetComponent<LineRenderer>();
                        if (lr == null)
                        {
                            lr = pathGameObject.AddComponent<LineRenderer>();
                        }
                        for (int i = 0; i < coords.Length; i++)
                        {
                            var x = float.Parse(coordStrings[i * 2], CultureInfo.InvariantCulture);
                            var y = -float.Parse(coordStrings[i * 2 + 1], CultureInfo.InvariantCulture);
                            coords[i] = new Vector3(x, y, 0);
                        }
                        lr.positionCount = coords.Length;
                        lr.SetPositions(coords);
                        continue;
                    }
                    if (reader.Name == "polygon")
                    {
                        if(polygonGameObject == null)
                        {
                            continue;
                        }
                        var points = reader.GetAttribute("points");
                        var coordStrings = points.Split(' ', ',').Take(6).ToArray();
                        mr = polygonGameObject.GetComponent<MeshRenderer>();
                        if (mr == null)
                        {
                            mr = polygonGameObject.AddComponent<MeshRenderer>();
                        }
                        mf = polygonGameObject.GetComponent<MeshFilter>();
                        if(mf == null)
                        {
                            mf = polygonGameObject.AddComponent<MeshFilter>();
                        }
                        if(mf.sharedMesh == null)
                        {
                            var coords = new Vector3[coordStrings.Length / 2];
                            for (int i = 0; i < coords.Length; i++)
                            {
                                var x = float.Parse(coordStrings[i * 2], CultureInfo.InvariantCulture);
                                var y = -float.Parse(coordStrings[i * 2 + 1], CultureInfo.InvariantCulture);
                                coords[i] = new Vector3(x, y, 0);
                            }
                            var mesh = new Mesh();
                            mesh.vertices = coords;
                            mesh.triangles = new int[] { 0, 1, 2 };
                            mf.sharedMesh = mesh;
                        }
                        continue;
                    }
                    break;
                case XmlNodeType.EndElement:
                    //Debug.Log(reader.Name + " end");
                    break;

            }
        }
    }

       private void SetEllipseRadius(LineRenderer lr, float cx, float cy, float rx, float ry)
    {
        var segments = 500;
        var points = new Vector3[segments + 2];
        lr.positionCount = segments + 1;
        for (int i = 0; i < segments + 1; i++)
        {
            var angle = (float)i / (float)segments * 2.0f * Mathf.PI;
            var x = Mathf.Cos(angle) * rx + cx;
            var y = Mathf.Sin(angle) * ry + cy;
            points[i] = new Vector3(x, y, 0f);
        }
        lr.SetPositions(points);
        lr.loop = true;
    }
}

