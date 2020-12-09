using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float viewRadius;
    [Range(0, 360), SerializeField] private float viewAngle;

    [SerializeField] private float surroundViewRadius;
    private float surroundViewAngle = 360;

    [SerializeField] private AnimationCurve fovHeightCurve;
    [SerializeField] private float curveMultiplier;
    [SerializeField] private float maxHeightClamp;

    [SerializeField] private LayerMask obstacleMask;
    //private MeshFilter viewMeshFilter;
    private Mesh viewMesh;
    private bool insideOther = false;

    [SerializeField] private float viewVisualisationHeight;
    [SerializeField] private float maskCutawayDistance;

    public float meshResolution;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "ViewMesh";
        GetComponent<MeshFilter>().mesh = viewMesh;
    }

    public void UpdateVariables(float viewRadius, float viewAngle, float surroundViewRadius)
    {
        this.viewRadius = viewRadius;
        this.viewAngle = viewAngle;
        this.surroundViewRadius = surroundViewRadius;
    }

    //private void Update()
    //{
        //transform.position = playerHeadTip.position - Vector3.forward * viewVisualisationHeight;


    //}

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    private void OnTriggerEnter(Collider other)
    {
        insideOther = true;
    }

    private void OnTriggerExit(Collider other)
    {
        insideOther = false;
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(surroundViewAngle * meshResolution);
        float stepAngleSize = surroundViewAngle / stepCount;

        float maxViewAngleRange = transform.eulerAngles.y + viewAngle / 2;
        float minViewAngleRange = transform.eulerAngles.y - viewAngle / 2;
        
        List<Vector3> viewpoints = new List<Vector3>();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - surroundViewAngle / 2 + stepAngleSize * i;

            float currentAngle01 = (angle - minViewAngleRange) / (maxViewAngleRange - minViewAngleRange);
            float fovHeightAtAngle = fovHeightCurve.Evaluate(currentAngle01) * curveMultiplier;

            ViewCastInfo newViewCast;
            if (angle < maxViewAngleRange && angle > minViewAngleRange)
                newViewCast = ViewCast(angle, obstacleMask, viewRadius, fovHeightAtAngle);
            else
                newViewCast = ViewCast(angle, obstacleMask, surroundViewRadius, fovHeightAtAngle);

            viewpoints.Add(ClampFOVPointHeight(newViewCast.point));
        }

        int vertexCount = viewpoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewpoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast (float globalAngle, LayerMask rayMask, float _viewRadius, float yHeight)
    {
        Vector3 dir = DirFromAngle(globalAngle, true, yHeight);
        //Vector3 dir = DirFromAngle(globalAngle, true, 0);
        RaycastHit hit;

        //if (insideOther)
        //return new ViewCastInfo(true, transform.position, 0, globalAngle);

        if (Physics.Raycast(transform.position, dir, out hit, _viewRadius, rayMask))
        {
            if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Blocker"))
            {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle, yHeight, false);
            } else
            {
                float minYBound = hit.transform.position.y - (hit.transform.localScale.y / 2);
                //Debug.Log(minBounds);
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle, minYBound, true);
            }
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * _viewRadius, _viewRadius, globalAngle, yHeight, false);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal, float testYAngle)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), testYAngle, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle, float _yHeight, bool hitBlocker)
        {
            hit = _hit;
            if (hitBlocker)
                point = new Vector3(_point.x, _yHeight, _point.z);
            else
                point = _point;
            distance = _distance;
            angle = _angle;
        }
    }

    public Vector3 ClampFOVPointHeight(Vector3 _point)
    {
        float yHeight = Mathf.Clamp(_point.y, 0, maxHeightClamp);
        return new Vector3(_point.x, yHeight, _point.z);
    }
}
