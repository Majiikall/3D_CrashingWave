using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFloat : MonoBehaviour
{
    //public properties
    public float AirDrag = 1;
    public float WaterDrag = 10;
    public bool AffectDirection = true;
    public bool AttachToSurface = false;
    public Transform[] FloatPoints;

    //components
    protected Rigidbody Rigidbody;
    protected WAVES Waves;

    //water line
    protected float WaterLine;
    protected Vector3[] WaterLinePoints;

    //help vectors
    protected Vector3 centerOffset;
    protected Vector3 smoothVectorRotaion;
    protected Vector3 TargetUp;

    public Vector3 Center { get { return transform.position + centerOffset; } }

    // Start is called before the first frame update
    void Awake()
    {
        Waves = FindObjectOfType<WAVES>();
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.useGravity = false;

        // compute center
        WaterLinePoints = new Vector3[FloatPoints.Length];
        for (int i = 0; i < FloatPoints.Length; i++) {
            WaterLinePoints[i] = FloatPoints[i].position;
        }
        centerOffset = GetCenter(WaterLinePoints) - transform.position; // not using physicshelper
    }

    // Update is called once per frame
    void Update()
    {
        //default water surface
        var newWaterLine = 0f;
        var pointsUnderWater = false;

        //set WaterLinePoints and WaterLine
        for (int i = 0; i <FloatPoints.Length; i++)
        {
            //height
            WaterLinePoints[i] = FloatPoints[i].position;
            WaterLinePoints[i].y = Waves.GetHeight(FloatPoints[i].position);
            newWaterLine += WaterLinePoints[i].y / FloatPoints.Length;
            if (WaterLinePoints[i].y > FloatPoints[i].position.y)
            {
                pointsUnderWater = true;
            }
        }

        var waterLineDelta = newWaterLine - WaterLine;
        WaterLine = newWaterLine;

        //Gravity
        var gravity = Physics.gravity;
        Rigidbody.drag = AirDrag;
        if (WaterLine > Center.y)
        {
            Rigidbody.drag = WaterDrag;

            //under water
            if (AttachToSurface)
            {
                // attach to water Attach
                Rigidbody.position = new Vector3(Rigidbody.position.x, WaterLine - centerOffset.y, Rigidbody.position.z);
            }
            else
            {
                gravity = -Physics.gravity;
                transform.Translate(Vector3.up * waterLineDelta * 0.9f);
            }
        }
        Rigidbody.AddForce(gravity * Mathf.Clamp(Mathf.Abs(WaterLine - Center.y), 0, 1));

        //compute up vector
        TargetUp = GetNormal(WaterLinePoints);

        //rotation
        if (pointsUnderWater)
        {
            //attach to water surface
            TargetUp = Vector3.SmoothDamp(transform.up, TargetUp, ref smoothVectorRotaion, 0.2f);
            Rigidbody.rotation = Quaternion.FromToRotation(transform.up, TargetUp) * Rigidbody.rotation;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (FloatPoints == null) {
            return;
        }

        for (int i = 0; i < FloatPoints.Length; i++)
        {
            if (FloatPoints[i] == null) {
                continue;
            }

            if (Waves != null)
            {
                //draw cube
                Gizmos.color = Color.red;
                Gizmos.DrawCube(WaterLinePoints[i], Vector3.one * 0.3f);
            }

            //Draw Sphere
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(FloatPoints[i].position, 0.1f);
        }

        // draw center
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(Center.x, WaterLine, Center.z), Vector3.one * 1f);
        }
    }

    public static Vector3 GetCenter(Vector3[] points)
    {
        var center = Vector3.zero;
        for (int i = 0; i < points.Length; i++)
        {
            center += points[i] / points.Length;
        }
        return center;
    }

    public static Vector3 GetNormal(Vector3[] points)
    {
        if (points.Length < 3)
        {
            return Vector3.up;
        }

        var center = GetCenter(points);

        float xx = 0f, xy = 0f, xz = 0f, yy = 0f, yz = 0f, zz = 0f;

        for (int i = 0; i < points.Length; i++)
        {
            var r = points[i] - center;
            xx += r.x * r.x;
            xy += r.x * r.y;
            xz += r.x * r.z;
            yy += r.y * r.y;
            yz += r.y * r.z;
            zz += r.z * r.z;
        }

        var det_x = yy * zz - yz * yz;
        var det_y = xx * zz - xz * xz;
        var det_z = xx * yy - xy * xy;

        if (det_x > det_y && det_x > det_z)
        {
            return new Vector3(det_x, xz * yz - xy * zz, xy * yz - xz * yy).normalized;
        }
        if (det_y > det_z)
        {
            return new Vector3(xz * yz - xy * zz, det_y, xy * xz - yz * xx).normalized;
        }
        else
        {
            return new Vector3(xy * yz - xz * yy, xy * xz - yz * xx, det_z).normalized;
        }
    }
}
