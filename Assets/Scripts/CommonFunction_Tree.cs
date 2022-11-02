using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonFunction_Tree
{
    public static List<CurvePoint> GeneratePoint(BasicParameter _parameter)
    {
        List<CurvePoint> curvePoints = new List<CurvePoint>();
        //PointPos = new List<GeneratePoint>();
        for (int i = 0; i < _parameter.SegmentNum; i++)
        {
            CurvePoint NewPoint = new CurvePoint();

            //set a start point
            if (i == 0)
            {
                NewPoint.StartPoint = Vector3.zero;

            }
            else
            {
                NewPoint.StartPoint = curvePoints[i - 1].EndPoint;
            }

            #region-BezierMode
            //set an end point
            NewPoint.EndPoint = NewPoint.StartPoint + new Vector3(_parameter.BoundX.GetRandResult(), _parameter.BoundY.GetRandResult(), _parameter.BoundZ.GetRandResult());
            Vector3 tmpPnt = NewPoint.StartPoint + ((NewPoint.EndPoint - NewPoint.StartPoint) * (Random.Range(10, 90) / 100));

            float rad = (360 / Random.Range(4, 10)) * Mathf.PI / 180;


            if (i == 0)
            {
                Vector3 targetDir = NewPoint.EndPoint - NewPoint.StartPoint;
                NewPoint.MiddlePoint = targetDir * (Random.Range(40, 60) / 100f) + NewPoint.StartPoint;
            }
            else
            {
                Vector3 targetDir = NewPoint.StartPoint - curvePoints[i - 1].MiddlePoint;
                NewPoint.MiddlePoint = NewPoint.StartPoint + targetDir * Random.Range(0.5f, 1.5f);
            }
            //Plane p = new Plane(Vector3.forward, Vector3.zero);
            //Vector3 xAxis = Vector3.up;
            //Vector3 yAxis = Vector3.right;
            //if (p.GetSide(targetDir))
            //{
            //    yAxis = Vector3.left;
            //}
            //Vector3.OrthoNormalize(ref targetDir, ref xAxis, ref yAxis);

            //NewPoint.MiddlePoint = targetDir * (Random.Range(40, 60) / 100f) + NewPoint.StartPoint;
            //float _radius = Random.Range(1, 5);
            //NewPoint.MiddlePoint = NewPoint.MiddlePoint +
            //    (_radius * Mathf.Cos(rad) * xAxis) +
            //    (_radius * Mathf.Sin(rad) * yAxis);
            #endregion
            #region-Biarcs
            //if (i == 0)
            //{
            //    NewPoint.MiddlePoint = NewPoint.StartPoint + new Vector3(Random.Range(-5, 5), Random.Range(5, 10), Random.Range(-5, 5));
            //}
            //else
            //{
            //    Vector3 dir = NewPoint.StartPoint - curvePoints[i - 1].MiddlePoint;
            //    NewPoint.MiddlePoint = NewPoint.StartPoint + dir * Random.Range(1, 2);
            //}

            //Vector3 targetDir = NewPoint.StartPoint - NewPoint.MiddlePoint;
            //Vector3 angleVector = new Vector3(Random.Range(-270, 270), Random.Range(-270, 270), Random.Range(-270, 270));
            //Quaternion _RandAngle = Quaternion.Euler(angleVector);

            //NewPoint.EndPoint = _RandAngle * targetDir + NewPoint.MiddlePoint;

            //NewPoint.PointCount = Random.Range(40, 50);
            //for (int j = 0; j < NewPoint.PointCount; j++)
            //{
            //    if (j == 0 && i != 0) continue;
            //    Vector3 newPos = Quaternion.Euler(angleVector * ((float)j / NewPoint.PointCount)) * targetDir + NewPoint.MiddlePoint;
            //    PointPos.Add(newPos);
            //}
            #endregion
            NewPoint.PointCount = 10;
            curvePoints.Add(NewPoint);

        }
        return curvePoints;
    }

    public static List<GeneratePoint> DrawCurve(List<CurvePoint> curvePoints)
    {
        //return;
        List<GeneratePoint>PointPos = new List<GeneratePoint>();
        for (int i = 0; i < curvePoints.Count; i++)
        {
            PointPos.Add(new GeneratePoint(curvePoints[i].StartPoint, false));
            for (int j = 0; j < curvePoints[i].PointCount; j++)
            {
                if (j == 0 && i != 0) continue;
                float t = j / (float)curvePoints[i].PointCount;

                GeneratePoint newPos = new GeneratePoint(CalculateBezierPoint_Q(t, curvePoints[i].StartPoint, curvePoints[i].MiddlePoint, curvePoints[i].EndPoint), j == curvePoints[i].PointCount - 1);

                PointPos.Add(newPos);
            }
        }
        return PointPos;
    }



    Vector3 CalculateBiarc(float angle, Vector3 StartP, Vector3 EndP, Vector3 MidP)
    {
        float _ang = getAngle(StartP, EndP, MidP);
        Debug.Log(_ang);
        float Rad = Vector3.Distance(MidP, StartP);
        Vector3 dir = GerCross(StartP, EndP, MidP);
        //Debug.Log(angle);
        Plane p = new Plane(Vector3.forward, Vector3.zero);
        Vector3 xAxis = Vector3.up;
        Vector3 yAxis = Vector3.right;
        if (p.GetSide(dir * -1))
        {
            yAxis = Vector3.left;
        }
        Vector3.OrthoNormalize(ref dir, ref xAxis, ref yAxis);

        return MidP + (Rad * Mathf.Cos(angle - 180) * xAxis) + (Rad * Mathf.Sin(angle - 180) * yAxis);

    }

    float getAngle(Vector3 StartP, Vector3 EndP, Vector3 MidP)
    {
        return Vector3.Angle(StartP - MidP, EndP - MidP);
    }

    Vector3 GerCross(Vector3 StartP, Vector3 EndP, Vector3 MidP)
    {
        Vector3 side1 = StartP - MidP;
        Vector3 side2 = EndP - MidP;

        return Vector3.Cross(side1, side2).normalized;
    }

    private static Vector3 CalculateBezierPoint_Q(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {

        //B(t) = P1(1 - t)^2 + 2P2t(1 - t) + P3t^2
        float u = 1 - t;
        float t_Mul = t * t;
        float u_Mul = u * u;

        Vector3 p = u_Mul * p0;
        p += 2 * u * t * p1;
        p += t_Mul * p2;
        return p;
    }
}
