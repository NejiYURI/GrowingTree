using System.Collections;
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

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    [SerializeField]
    Vector3[] vertices;
    int[] triangles;

    public float Radius;

    public int RowSize = 4;
    public int ColumnSize = 4;

    public int PointNum = 10;
    [SerializeField]
    private List<Vector3> PointPos;

    [SerializeField]
    private List<CurvePoint> curvePoints;

    private Coroutine GenerateCoroutine;

    public bool DebugVertices;
    public bool DebugCurvePoints;
    public bool DebugRandPoint;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        // StartGenerate();
        //UpdateMesh();
    }

    private void Update()
    {
        UpdateMesh();
    }

    public void StartGenerate()
    {
        if (GenerateCoroutine != null)
        {
            StopCoroutine(GenerateCoroutine);
        }
        GeneratePoint();
        DrawCurve();
        GenerateCoroutine = StartCoroutine(CreateShape());
    }

    public void ClearFunc()
    {
        vertices = new Vector3[0];
        curvePoints = new List<CurvePoint>();
        triangles = new int[0];
        //mesh.Clear();
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    IEnumerator CreateShape()
    {
        RowSize = PointPos.Count - 1;
        vertices = new Vector3[(RowSize + 1) * (ColumnSize + 1)];
        float rad = (360 / ColumnSize) * Mathf.PI / 180;


        for (int i = 0, Row = 0; Row <= RowSize; Row++)
        {
            for (int Col = 0; Col <= ColumnSize; Col++)
            {
                float _CurRadius = Radius * (1 - ((float)Row / RowSize));

                var x = PointPos[Row].x + (Row == RowSize ? 0 : _CurRadius) * Mathf.Cos(rad * Col);
                var z = PointPos[Row].z + (Row == RowSize ? 0 : _CurRadius) * Mathf.Sin(rad * Col);
                Vector3 tmpVector = new Vector3(x, PointPos[Row].y, z);
                if (Row != RowSize)
                {
                    Vector3 targetDir = PointPos[Row + 1] - PointPos[Row];

                    Plane p = new Plane(Vector3.forward, Vector3.zero);
                    Vector3 xAxis = Vector3.up;
                    Vector3 yAxis = Vector3.right;
                    if (p.GetSide(targetDir))
                    {
                        yAxis = Vector3.left;
                    }
                    Vector3.OrthoNormalize(ref targetDir, ref xAxis, ref yAxis);

                    tmpVector = PointPos[Row] +
                ((Row == RowSize ? 0 : _CurRadius) * Mathf.Cos(rad * Col) * xAxis) +
                ((Row == RowSize ? 0 : _CurRadius) * Mathf.Sin(rad * Col) * yAxis);
                }
                vertices[i] = tmpVector;
                i++;
            }
        }

        triangles = new int[RowSize * ColumnSize * 6];

        int vert = 0;
        int tris = 0;

        for (int y = 0; y < RowSize; y++)
        {
            for (int x = 0; x < ColumnSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + ColumnSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + ColumnSize + 1;
                triangles[tris + 5] = vert + ColumnSize + 2;

                vert++;
                tris += 6;
                yield return new WaitForSeconds(0.001f);
            }
            vert++;
        }


    }

    void GeneratePoint()
    {
        curvePoints = new List<CurvePoint>();
        for (int i = 0; i < PointNum; i++)
        {
            CurvePoint NewPoint = new CurvePoint();

            //set a start point
            if (i == 0)
            {
                NewPoint.StartPoint = this.transform.position;
            }
            else
            {
                NewPoint.StartPoint = curvePoints[i - 1].EndPoint;
            }

            #region-BezierMode
            ////set an end point
            //NewPoint.EndPoint = NewPoint.StartPoint + new Vector3(Random.Range(-5, 5), Random.Range(5, 10), Random.Range(-5, 5));
            //Vector3 tmpPnt = NewPoint.StartPoint + ((NewPoint.EndPoint - NewPoint.StartPoint) * (Random.Range(10, 90) / 100));

            //float rad = (360 / Random.Range(4, 10)) * Mathf.PI / 180;

            //Vector3 targetDir = NewPoint.EndPoint - NewPoint.StartPoint;
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
            if (i == 0)
            {
                NewPoint.MiddlePoint = NewPoint.StartPoint + new Vector3(Random.Range(-5, 5), Random.Range(5, 10), Random.Range(-5, 5));
            }
            else
            {
                Vector3 dir = NewPoint.StartPoint - curvePoints[i - 1].MiddlePoint;
                NewPoint.MiddlePoint = NewPoint.StartPoint + dir * Random.Range(1, 2);
            }

            Vector3 targetDir = NewPoint.MiddlePoint - NewPoint.StartPoint;

            Plane p = new Plane(Vector3.forward, Vector3.zero);
            Vector3 xAxis = Vector3.up;
            Vector3 yAxis = Vector3.right;
            if (p.GetSide(targetDir))
            {
                yAxis = Vector3.left;
            }
            Vector3.OrthoNormalize(ref targetDir, ref xAxis, ref yAxis);

            float _NewRad = Vector3.Distance(NewPoint.StartPoint, NewPoint.MiddlePoint);
            float _RandRadius = Random.Range(90, 270);
            NewPoint.Angle = _RandRadius;
            NewPoint.EndPoint = NewPoint.MiddlePoint +
                (_NewRad * Mathf.Cos(_RandRadius) * xAxis) +
                (_NewRad * Mathf.Sin(_RandRadius) * yAxis);

            NewPoint.PointCount = Random.Range(40, 50);
            curvePoints.Add(NewPoint);

        }
        //DrawCurve();
    }

    void DrawCurve()
    {
        //PointPos = new Vector3[PointNum];
        //for (int i = 1; i < PointNum + 1; i++)
        //{
        //    float t = i / (float)PointNum;
        //    PointPos[i - 1] = CalculateBezierPoint_Q(t, StartPoint.position, MiddlePoint.position, EndPoint.position);
        //}
        PointPos = new List<Vector3>();
        for (int i = 0; i < curvePoints.Count; i++)
        {
            for (int j = 0; j < curvePoints[i].PointCount; j++)
            {
                if (j == 0 && i != 0) continue;
                //float t = j / (float)curvePoints[i].PointCount;
                //Vector3 newPos = CalculateBezierPoint_Q(t, curvePoints[i].StartPoint, curvePoints[i].MiddlePoint, curvePoints[i].EndPoint);
                float angle = (j / (float)curvePoints[i].PointCount) * curvePoints[i].Angle;
                Vector3 newPos = CalculateBiarc(angle, curvePoints[i].StartPoint, curvePoints[i].EndPoint, curvePoints[i].MiddlePoint);
                PointPos.Add(newPos);
            }
        }
    }

    Vector3 CalculateBiarc(float angle, Vector3 StartP, Vector3 EndP, Vector3 MidP)
    {
        float Rad = Vector3.Distance(MidP, StartP);
        Vector3 dir = GerCross(StartP, EndP, MidP);
        Debug.Log(dir);
        Plane p = new Plane(Vector3.forward, Vector3.zero);
        Vector3 xAxis = Vector3.up;
        Vector3 yAxis = Vector3.right;
        if (p.GetSide(dir))
        {
            yAxis = Vector3.left;
        }
        Vector3.OrthoNormalize(ref dir, ref xAxis, ref yAxis);
        return MidP + (Rad * Mathf.Cos(angle) * xAxis) + (Rad * Mathf.Sin(angle) * yAxis);

    }

    Vector3 GerCross(Vector3 StartP, Vector3 EndP, Vector3 MidP)
    {
        Vector3 side1 = StartP - MidP;
        Vector3 side2 = EndP - MidP;

        return Vector3.Cross(side1, side2).normalized;
    }

    private Vector3 CalculateBezierPoint_Q(float t, Vector3 p0, Vector3 p1, Vector3 p2)
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

    private void OnDrawGizmos()
    {
        if (vertices != null && DebugVertices)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], .1f);
            }
        }

        if (DebugCurvePoints)
            for (int i = 0; i < PointPos.Count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(PointPos[i], .1f);
            }


        if (DebugRandPoint)
        {
            for (int i = 0; i < curvePoints.Count; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(curvePoints[i].StartPoint, .1f);
                Gizmos.DrawSphere(curvePoints[i].EndPoint, .1f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(curvePoints[i].MiddlePoint, .1f);
            }

        }
    }
}
