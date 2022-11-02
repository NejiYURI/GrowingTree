using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




[RequireComponent(typeof(MeshFilter))]
public class BranchGenerator : MonoBehaviour
{
    Mesh mesh;
    MeshRenderer M_Renderer;

    public GameObject Branch;

    [SerializeField]
    private List<GameObject> BranchList;

    [SerializeField]
    private List<LeafScript> LeafList;

    [SerializeField]
    List<Vector3> vertices;
    List<int> triangles;

    public int RowSize = 4;
    public int ColumnSize = 4;

    public GameObject LeafObject;
    public Material LeafMaterial;

    public BasicParameter TreeParameter;

    private float GrowSpeed = 0.0001f;

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
        InitialSetup();
        // InitialSetup();
        // StartGenerate();
        //UpdateMesh();
    }

    private void Update()
    {
        //UpdateMesh();
    }

    void InitialSetup()
    {
        mesh = new Mesh();
        LeafList = new List<LeafScript>();
        M_Renderer = GetComponent<MeshRenderer>();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void StartGenerate()
    {
        if (mesh == null) InitialSetup();

        foreach (var item in BranchList)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in LeafList)
        {
            Destroy(item.gameObject);
        }
        BranchList = new List<GameObject>();
        LeafList = new List<LeafScript>();
        if (GenerateCoroutine != null)
        {
            StopCoroutine(GenerateCoroutine);
        }
        this.curvePoints = new List<CurvePoint>();
        this.curvePoints.AddRange(CommonFunction_Tree.GeneratePoint(TreeParameter));
        this.PointPos = new List<GeneratePoint>();
        this.PointPos.AddRange(CommonFunction_Tree.DrawCurve(this.curvePoints));
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
            GenerateCoroutine = StartCoroutine(CreateShape());


    }

    public void ClearFunc()
    {
        vertices = new List<Vector3>();
        curvePoints = new List<CurvePoint>();
        triangles = new List<int>();
        //mesh.Clear();
    }

    void UpdateMesh()
    {
        //mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.triangles = mesh.triangles.Reverse().ToArray();
        mesh.RecalculateNormals();
    }

    IEnumerator CreateShape()
    {
        mesh.Clear();
        RowSize = PointPos.Count - 1;
        // vertices = new Vector3[(RowSize + 1) * (ColumnSize + 1)];
        vertices = new List<Vector3>();
        float rad = (360 / ColumnSize) * Mathf.PI / 180;
        Plane p = new Plane(Vector3.forward, Vector3.zero);
        Vector3 xAxis = Vector3.up;
        Vector3 yAxis = Vector3.right;
        Vector3 targetDir = Vector3.zero;
        Vector3 tmpVector = Vector3.zero;
        for (int i = 0, Row = 0; Row <= RowSize; Row++)
        {

            float _CurRadius = TreeParameter.Radius * (1 - ((float)Row / RowSize));


            for (int Col = 0; Col <= ColumnSize; Col++)
            {
                if (Row != RowSize)
                {
                    targetDir = PointPos[Row + 1].Pos - PointPos[Row].Pos;
                    if (p.GetSide(targetDir))
                    {
                        yAxis = Vector3.left;
                    }
                    Vector3.OrthoNormalize(ref targetDir, ref xAxis, ref yAxis);

                    tmpVector = PointPos[Row].Pos +
                ((Row == RowSize ? 0 : _CurRadius) * Mathf.Cos(rad * Col) * xAxis) +
                ((Row == RowSize ? 0 : _CurRadius) * Mathf.Sin(rad * Col) * yAxis);
                }
                else
                {
                    var x = PointPos[Row].Pos.x + (Row == RowSize ? 0 : _CurRadius) * Mathf.Cos(rad * Col);
                    var z = PointPos[Row].Pos.z + (Row == RowSize ? 0 : _CurRadius) * Mathf.Sin(rad * Col);
                    tmpVector = new Vector3(x, PointPos[Row].Pos.y, z);
                }
                //vertices[i] = tmpVector;
                //i++;
                vertices.Add(tmpVector);


            }

            if (PointPos[Row].GenerateBranch && TreeParameter.BranchWeight > 0)
            {
                int RnadBNum = ((1 - (Row / PointPos.Count)) * TreeParameter.BranchWeight);
                for (int b_i = 0; b_i < RnadBNum; b_i++)
                {
                    GameObject _branch = Instantiate(Branch, PointPos[Row].Pos, Quaternion.LookRotation(Random.insideUnitSphere.normalized));
                    _branch.transform.localScale = new Vector3(1, 1, 1);
                    _branch.transform.SetParent(this.transform);
                    _branch.transform.localPosition = PointPos[Row].Pos;
                    if (_branch.GetComponent<BranchGenerator>() != null)
                    {
                        BranchGenerator newControl = _branch.GetComponent<BranchGenerator>();
                        newControl.TreeParameter.Radius = TreeParameter.Radius * (1 - ((float)Row / RowSize));
                        newControl.TreeParameter.BranchWeight = RnadBNum / 2;
                        newControl.TreeParameter.SegmentNum = (TreeParameter.SegmentNum - 1 > 0) ? Random.Range(1, TreeParameter.SegmentNum - 1) : 1;
                        newControl.LeafMaterial = LeafMaterial;
                        newControl.TreeParameter.GrowSpeedMul = this.TreeParameter.GrowSpeedMul;
                        newControl.TreeParameter.BoundX = this.TreeParameter.BoundX;
                        newControl.TreeParameter.BoundY = this.TreeParameter.BoundY;
                        newControl.TreeParameter.BoundZ = this.TreeParameter.BoundZ;
                        newControl.ChangeMaterial(this.M_Renderer.materials);
                        PointPos[Row].branchControl.Add(newControl);
                    }
                    BranchList.Add(_branch);
                }

            }
            if (Row == RowSize)
            {
                int LeafNum = Random.Range(1, 3);
                for (int l_i = 0; l_i < LeafNum; l_i++)
                {
                    float RandRad = Random.Range(0, 360);


                    if (Row != RowSize)
                    {
                        targetDir = PointPos[Row + 1].Pos - PointPos[Row].Pos;

                        //Plane p = new Plane(Vector3.forward, Vector3.zero);
                        //Vector3 xAxis = Vector3.up;
                        //Vector3 yAxis = Vector3.left;
                        if (p.GetSide(targetDir))
                        {
                            yAxis = Vector3.left;
                        }
                        Vector3.OrthoNormalize(ref targetDir, ref xAxis, ref yAxis);

                        tmpVector = PointPos[Row].Pos +
                    ((Row == RowSize ? 0 : _CurRadius) * Mathf.Cos(rad) * xAxis) +
                    ((Row == RowSize ? 0 : _CurRadius) * Mathf.Sin(rad) * yAxis);
                    }
                    else
                    {
                        var x = PointPos[Row].Pos.x + (Row == RowSize ? 0 : _CurRadius) * Mathf.Cos(RandRad);
                        var z = PointPos[Row].Pos.z + (Row == RowSize ? 0 : _CurRadius) * Mathf.Sin(RandRad);
                        tmpVector = new Vector3(x, PointPos[Row].Pos.y, z);
                    }

                    GameObject _Leaf = Instantiate(LeafObject, tmpVector, Quaternion.LookRotation(Random.insideUnitSphere.normalized));
                    _Leaf.transform.SetParent(this.transform);
                    _Leaf.transform.localPosition = tmpVector;
                    if (_Leaf.GetComponent<LeafScript>() != null)
                    {
                        _Leaf.GetComponent<LeafScript>().MaterialSetup(LeafMaterial);
                        LeafList.Add(_Leaf.GetComponent<LeafScript>());
                    }
                }

            }
        }

        // triangles = new int[RowSize * ColumnSize * 6];
        triangles = new List<int>();
        int vert = 0;
        int tris = 0;

        for (int y = 0; y < RowSize; y++)
        {
            for (int x = 0; x < ColumnSize; x++)
            {
                //triangles[tris + 0] = vert + 0;
                //triangles[tris + 1] = vert + ColumnSize + 1;
                //triangles[tris + 2] = vert + 1;
                //triangles[tris + 3] = vert + 1;
                //triangles[tris + 4] = vert + ColumnSize + 1;
                //triangles[tris + 5] = vert + ColumnSize + 2;
                triangles.Add(vert + 0);
                triangles.Add(vert + ColumnSize + 1);
                triangles.Add(vert + 1);
                triangles.Add(vert + 1);
                triangles.Add(vert + ColumnSize + 1);
                triangles.Add(vert + ColumnSize + 2);

                vert++;
                tris += 6;

            }
            vert++;

            if (PointPos[y].GenerateBranch && PointPos[y].branchControl != null && TreeParameter.BranchWeight > 0)
            {
                foreach (var item in PointPos[y].branchControl)
                {
                    item.StartGenerate();
                }
            }
            UpdateMesh();
            yield return new WaitForSeconds(GrowSpeed * TreeParameter.GrowSpeedMul);

        }

        foreach (var item in LeafList)
        {
            item.ShowLeaf();
            yield return new WaitForSeconds(0.1f);
        }

    }

    public void ChangeMaterial(Material[] _mats)
    {
        if (this.M_Renderer == null)
            this.M_Renderer = GetComponent<MeshRenderer>();
        this.M_Renderer.materials = _mats;
    }



    private void OnDrawGizmos()
    {
        if (vertices != null && DebugVertices)
        {
            for (int i = 0; i < vertices.Count; i++)
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
