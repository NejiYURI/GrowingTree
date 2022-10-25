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

[System.Serializable]
public class GeneratePoint
{
    public GeneratePoint(Vector3 _pos, bool _IsGenerate)
    {
        this.Pos = _pos;
        this.GenerateBranch = _IsGenerate;
    }
    public Vector3 Pos;
    public bool GenerateBranch;
    public MeshGenerator branchControl;
}

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    public GameObject Branch;

    [SerializeField]
    private List<GameObject> BranchList;

    [SerializeField]
    Vector3[] vertices;
    int[] triangles;

    public float Radius;

    public int RowSize = 4;
    public int ColumnSize = 4;

    public int BranchWeight;

    public int PointNum = 10;
    [SerializeField]
    private List<GeneratePoint> PointPos;

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
        foreach (var item in BranchList)
        {
            Destroy(item.gameObject);
        }
        BranchList = new List<GameObject>();
        if (GenerateCoroutine != null)
        {
            StopCoroutine(GenerateCoroutine);
        }
        GeneratePoint();
        DrawCurve();
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
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

        bool BranchSpawn = false;
        for (int i = 0, Row = 0; Row <= RowSize; Row++)
        {

            for (int Col = 0; Col <= ColumnSize; Col++)
            {
                float _CurRadius = Radius * (1 - ((float)Row / RowSize));

                var x = PointPos[Row].Pos.x + (Row == RowSize ? 0 : _CurRadius) * Mathf.Cos(rad * Col);
                var z = PointPos[Row].Pos.z + (Row == RowSize ? 0 : _CurRadius) * Mathf.Sin(rad * Col);
                Vector3 tmpVector = new Vector3(x, PointPos[Row].Pos.y, z);
                if (Row != RowSize)
                {
                    Vector3 targetDir = PointPos[Row + 1].Pos - PointPos[Row].Pos;

                    Plane p = new Plane(Vector3.forward, Vector3.zero);
                    Vector3 xAxis = Vector3.up;
                    Vector3 yAxis = Vector3.right;
                    if (p.GetSide(targetDir))
                    {
                        yAxis = Vector3.left;
                    }
                    Vector3.OrthoNormalize(ref targetDir, ref xAxis, ref yAxis);

                    tmpVector = PointPos[Row].Pos +
                ((Row == RowSize ? 0 : _CurRadius) * Mathf.Cos(rad * Col) * xAxis) +
                ((Row == RowSize ? 0 : _CurRadius) * Mathf.Sin(rad * Col) * yAxis);
                }
                vertices[i] = tmpVector;
                i++;


            }

            if (PointPos[Row].GenerateBranch && BranchWeight>0)
            {
                
                GameObject _branch = Instantiate(Branch, PointPos[Row].Pos, Quaternion.LookRotation(Random.insideUnitSphere.normalized));
                _branch.transform.SetParent(this.transform);
                _branch.transform.localPosition = PointPos[Row].Pos;
                if (_branch.GetComponent<MeshGenerator>() != null)
                {
                    PointPos[Row].branchControl = _branch.GetComponent<MeshGenerator>();
                    PointPos[Row].branchControl.Radius= Radius * (1 - ((float)Row / RowSize));
                    PointPos[Row].branchControl.BranchWeight = BranchWeight - 1;
                    PointPos[Row].branchControl.PointNum = Random.Range(1, 5);
                }
                BranchList.Add(_branch);
               
                BranchSpawn = true;
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
                
            }
            vert++;

            if (PointPos[y].GenerateBranch && PointPos[y].branchControl != null && BranchWeight > 0)
            {
                PointPos[y].branchControl.StartGenerate();
            }
            yield return new WaitForSeconds(0.001f);
        }


    }

    void GeneratePoint()
    {
        curvePoints = new List<CurvePoint>();
        PointPos = new List<GeneratePoint>();
        for (int i = 0; i < PointNum; i++)
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
            NewPoint.EndPoint = NewPoint.StartPoint + new Vector3(Random.Range(-5, 5), Random.Range(10, 20), Random.Range(-5, 5));
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
                NewPoint.MiddlePoint = NewPoint.StartPoint + targetDir * Random.Range(0.5f, 2f);
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



            //Plane p = new Plane(Vector3.forward, Vector3.zero);
            //Vector3 xAxis = Vector3.up;
            //Vector3 yAxis = Vector3.right;
            //if (p.GetSide(targetDir))
            //{
            //    yAxis = Vector3.left;
            //}
            //Vector3.OrthoNormalize(ref targetDir, ref xAxis, ref yAxis);

            //float _NewRad = Vector3.Distance(NewPoint.StartPoint, NewPoint.MiddlePoint);
            //float _RandRadius = Random.Range(60, 100);
            //NewPoint.Angle = _RandRadius;
            //NewPoint.EndPoint = NewPoint.MiddlePoint +
            //    (_NewRad * Mathf.Cos(_RandRadius) * xAxis) +
            //    (_NewRad * Mathf.Sin(_RandRadius) * yAxis);

            NewPoint.PointCount = Random.Range(25, 40);
            curvePoints.Add(NewPoint);

        }
        //DrawCurve();
    }

    void DrawCurve()
    {
        //return;
        PointPos = new List<GeneratePoint>();
        for (int i = 0; i < curvePoints.Count; i++)
        {

            for (int j = 0; j < curvePoints[i].PointCount; j++)
            {
                if (j == 0 && i != 0) continue;
                float t = j / (float)curvePoints[i].PointCount;

                GeneratePoint newPos = new GeneratePoint(CalculateBezierPoint_Q(t, curvePoints[i].StartPoint, curvePoints[i].MiddlePoint, curvePoints[i].EndPoint), j == curvePoints[i].PointCount - 1);
                //float angle = (j / (float)curvePoints[i].PointCount);
                //Vector3 newPos = CalculateBiarc(angle, curvePoints[i].StartPoint, curvePoints[i].EndPoint, curvePoints[i].MiddlePoint);
                PointPos.Add(newPos);
                //if (j == curvePoints[i].PointCount - 1)
                //{
                //    Debug.Log(PointPos.Count - 1);
                //}
            }
        }
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
                Gizmos.DrawSphere(PointPos[i].Pos, .1f);
            }


        if (DebugRandPoint)
        {
            for (int i = 0; i < curvePoints.Count; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(curvePoints[i].StartPoint, .5f);
                Gizmos.DrawSphere(curvePoints[i].EndPoint, .5f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(curvePoints[i].MiddlePoint, .5f);
            }

        }
    }
}
