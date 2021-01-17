using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class Ellipse : MonoBehaviour
{
    private LineRenderer lr;

    [Range(10, 500)]
    public int segments;
    public float xAxis = 5f;
    public float yAxis = 3f;
    public bool loop = true;

    private void Awake()
    {
        if (lr == null)
            lr = GetComponent<LineRenderer>();
    }

    void OnValidate()
    {
        CalculateEllipse();
    }

    private void CalculateEllipse()
    {
        var points = new Vector3[segments + 2];
        lr.positionCount = segments + 1;
        for (int i = 0; i < segments + 1; i++)
        {
            var angle = (float)i / (float)segments * 2.0f * Mathf.PI;
            var x = Mathf.Cos(angle) * xAxis;
            var y = Mathf.Sin(angle) * yAxis;
            points[i] = new Vector3(x, y, 0f);
        }
        lr.SetPositions(points);
        lr.loop = loop;
    }

    //public float width = 1f;
    //public float rotationAngle = 45;
    //public int resolution = 500;

    //private Vector3[] positions;

    //void OnValidate()
    //{
    //    UpdateEllipse();
    //}

    //public void UpdateEllipse()
    //{
    //    if (lr == null)
    //        lr = GetComponent<LineRenderer>();

    //    lr.positionCount = resolution + 3;

    //    lr.startWidth = width;
    //    lr.endWidth = width;
    //    lr.loop = true;


    //    AddPointToLineRenderer(0f, 0);
    //    for (int i = 1; i <= resolution + 1; i++)
    //    {
    //        AddPointToLineRenderer((float)i / (float)(resolution) * 2.0f * Mathf.PI, i);
    //    }
    //    AddPointToLineRenderer(0f, resolution + 2);
    //}

    //void AddPointToLineRenderer(float angle, int index)
    //{
    //    Quaternion pointQuaternion = Quaternion.AngleAxis(rotationAngle, Vector3.forward);
    //    Vector3 pointPosition;

    //    pointPosition = new Vector3(xAxis * Mathf.Cos(angle), yAxis * Mathf.Sin(angle), 0.0f);
    //    pointPosition = pointQuaternion * pointPosition;

    //    lr.SetPosition(index, pointPosition);
    //}
}