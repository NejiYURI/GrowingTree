using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CurvePoint
{
    public int PointCount;
    public Vector3 StartPoint;
    public Vector3 MiddlePoint;
    public Vector3 EndPoint;
    public float Angle;
}

[System.Serializable]
public class BasicParameter
{
    [Tooltip("How many segment the branch will be")]
    [Range(1, 8)]
    public int SegmentNum = 5;
    [Tooltip("Control whether the branch will create new branch")]
    [Range(1, 3)]
    public int BranchWeight = 2;
    [Tooltip("Growing Speed")]
    public float GrowSpeedMul = 1;
    [Tooltip("The radius of branch")]
    public float Radius = 1;

    [Tooltip("The range of X")]
    [SerializeField]
    public minMaxData BoundX = new minMaxData(-5, 5);
    [Tooltip("The range of Y")]
    [SerializeField]
    public minMaxData BoundY = new minMaxData(5, 10);
    [Tooltip("The range of Z")]
    [SerializeField]
    public minMaxData BoundZ = new minMaxData(-5, 5);
}

[System.Serializable]
public class GeneratePoint
{
    public GeneratePoint(Vector3 _pos, bool _IsGenerate)
    {
        this.Pos = _pos;
        this.GenerateBranch = _IsGenerate;
        this.branchControl = new List<BranchGenerator>();
    }
    public Vector3 Pos;
    public bool GenerateBranch;
    public List<BranchGenerator> branchControl;
}

[System.Serializable]
public class EnvToleranceSetting
{
    public minMaxData Temperature;
    public minMaxData WaterAmount;
    public float WindDust;
    public float Sunlight;
}

[System.Serializable]
public class ToleranceChanges
{
    public float Temperature;
    public float WaterAmount;
    public float WindDust;
    public float Sunlight;
}

public class BranchSetting
{
    [Range(1, 8)]
    public int SegmentNum = 10;

    [SerializeField]
    public minMaxData RandBoundX;
    [SerializeField]
    public minMaxData RandBoundY;
    [SerializeField]
    public minMaxData RandBoundZ;
}

public class BranchData
{
    public BranchData()
    {
        this.PointPos = new List<GeneratePoint>();
        this.curvePoints = new List<CurvePoint>();
    }

    [SerializeField]
    Vector3[] vertices;
    int[] triangles;

    [SerializeField]
    private List<GeneratePoint> PointPos;

    [SerializeField]
    private List<CurvePoint> curvePoints;

    public void GeneratePoint(BranchData i_Branch)
    {
        curvePoints = new List<CurvePoint>();
        PointPos = new List<GeneratePoint>();
        //for (int i = 0; i < SegmentNum; i++)
        //{
        //    CurvePoint NewPoint = new CurvePoint();

        //    //set a start point
        //    if (i == 0)
        //    {
        //        NewPoint.StartPoint = Vector3.zero;

        //    }
        //    else
        //    {
        //        NewPoint.StartPoint = curvePoints[i - 1].EndPoint;
        //    }

        //    #region-BezierMode
        //    //set an end point
        //    //NewPoint.EndPoint = NewPoint.StartPoint + new Vector3(RandBoundX.GetResult(), RandBoundY.GetResult(), RandBoundZ.GetResult());
        //    Vector3 tmpPnt = NewPoint.StartPoint + ((NewPoint.EndPoint - NewPoint.StartPoint) * (Random.Range(10, 90) / 100));

        //    float rad = (360 / Random.Range(4, 10)) * Mathf.PI / 180;


        //    if (i == 0)
        //    {
        //        Vector3 targetDir = NewPoint.EndPoint - NewPoint.StartPoint;
        //        NewPoint.MiddlePoint = targetDir * (Random.Range(40, 60) / 100f) + NewPoint.StartPoint;
        //    }
        //    else
        //    {
        //        Vector3 targetDir = NewPoint.StartPoint - curvePoints[i - 1].MiddlePoint;
        //        NewPoint.MiddlePoint = NewPoint.StartPoint + targetDir * Random.Range(0.5f, 1.5f);
        //    }
        //    //Plane p = new Plane(Vector3.forward, Vector3.zero);
        //    //Vector3 xAxis = Vector3.up;
        //    //Vector3 yAxis = Vector3.right;
        //    //if (p.GetSide(targetDir))
        //    //{
        //    //    yAxis = Vector3.left;
        //    //}
        //    //Vector3.OrthoNormalize(ref targetDir, ref xAxis, ref yAxis);

        //    //NewPoint.MiddlePoint = targetDir * (Random.Range(40, 60) / 100f) + NewPoint.StartPoint;
        //    //float _radius = Random.Range(1, 5);
        //    //NewPoint.MiddlePoint = NewPoint.MiddlePoint +
        //    //    (_radius * Mathf.Cos(rad) * xAxis) +
        //    //    (_radius * Mathf.Sin(rad) * yAxis);
        //    #endregion
        //    #region-Biarcs
        //    //if (i == 0)
        //    //{
        //    //    NewPoint.MiddlePoint = NewPoint.StartPoint + new Vector3(Random.Range(-5, 5), Random.Range(5, 10), Random.Range(-5, 5));
        //    //}
        //    //else
        //    //{
        //    //    Vector3 dir = NewPoint.StartPoint - curvePoints[i - 1].MiddlePoint;
        //    //    NewPoint.MiddlePoint = NewPoint.StartPoint + dir * Random.Range(1, 2);
        //    //}

        //    //Vector3 targetDir = NewPoint.StartPoint - NewPoint.MiddlePoint;
        //    //Vector3 angleVector = new Vector3(Random.Range(-270, 270), Random.Range(-270, 270), Random.Range(-270, 270));
        //    //Quaternion _RandAngle = Quaternion.Euler(angleVector);

        //    //NewPoint.EndPoint = _RandAngle * targetDir + NewPoint.MiddlePoint;

        //    //NewPoint.PointCount = Random.Range(40, 50);
        //    //for (int j = 0; j < NewPoint.PointCount; j++)
        //    //{
        //    //    if (j == 0 && i != 0) continue;
        //    //    Vector3 newPos = Quaternion.Euler(angleVector * ((float)j / NewPoint.PointCount)) * targetDir + NewPoint.MiddlePoint;
        //    //    PointPos.Add(newPos);
        //    //}
        //    #endregion

        //    NewPoint.PointCount = 30;
        //    curvePoints.Add(NewPoint);

        //}
    }
}
