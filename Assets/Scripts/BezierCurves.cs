using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class BezierCurves : MonoBehaviour
{
    public enum CurveType { Linear, Quadratic, Cubic }
    public CurveType type;
    [SerializeField] public bool CanBeCutted = false;
    [SerializeField] public bool ControlRandomizer = false;
    [Range(0f, 10f)] [SerializeField] private float controlRndPower = 2.5f;
    [Range(0f, 50f)] [SerializeField] private float lerpSpeed = 2.5f;
    [Range(0f, 1f)] [SerializeField] private float accuracyToNext = 0.5f;

    private bool isCollides;
    public bool IsCollides { get => isCollides; }

    private Vector3 p2Rnd, p3Rnd;


    /*
     * // FIX#1 - rotation doest work properly
     * 
    [SerializeField] private bool rotateP2 = false;
    [Range(-360f, 360f)] public float p2Speed = 180f;

    [SerializeField] private bool rotateP3 = false;
    [Range(-360f, 360f)] public float p3Speed = 180f;
    */

    [Range(2, 50)] public int Detail = 25;
    [Header("Line operator points")]
    // GameObjects that operates line points: [0] - Start point, [1] - End point, [2] = Bezier curve operator1, [3] Bezier curve operator2 //  
    public Transform[] p;
    [SerializeField] private bool useNoise = false;
    [Range(0f, 2f)] [SerializeField] private float noisePower = 0.25f;
    [SerializeField] private bool spring = false;
    [Range(0f, 5f)] [SerializeField] private float springRadius = 2.5f;
    
    [Range(0.01f, 1f)] [SerializeField] public float updateFrequency = 0.5f;


    private LineRenderer rend;
    private Vector3[] lineDots;
    public Vector3[] LineDots { get => lineDots; set => lineDots = value; }


    private Vector3[] addNoise(Vector3[] original)
    {
        Vector3 noise = Random.onUnitSphere.normalized * noisePower;
        Vector3 dir;
        for (int i = 1; i < original.Length - 1; i++)
        {
            dir = original[i] - original[i - 1];
            //original[i] = original[i - 1] + dir * (noisePower + 1f);
            original[i] += Random.onUnitSphere.normalized * noisePower;
        }
        return original;
    }
    private Vector3[] makeSpring(Vector3[] original)
    {
        Vector3 dir;
        for (int i = 1; i < original.Length - 1; i++)
        {
            dir = original[i] - original[i - 1];
            original[i] += Vector3.Cross(dir, new Vector3(dir.z, dir.y, dir.x)).normalized * springRadius;
        }
        return original;
    }
    private Vector3[] cutter(Vector3[] original)
    {
        Vector3 dir;
        RaycastHit hit;
        for (int i = 1; i < original.Length; i++)
        {
            dir = original[i] - original[i - 1];
            if (Physics.Raycast(original[i - 1], dir, out hit, dir.magnitude))
            {
                isCollides = true;

                Vector3[] cutted = new Vector3[i + 1];
                cutted[0] = original[0];
                for (int k = 1; k < cutted.Length; k++)
                {
                    cutted[k] = original[k];
                }
                cutted[i] = hit.point;
                // Debug.DrawLine(original[i - 1], hit.point, Color.cyan); /*From last point before collision - to collisio point;
                return cutted;
            }
        }
        isCollides = false;
        return original;
    }
    private float power(float t, int power)
    {
        return Mathf.Pow(t, power);
    }

    // FUNCTIONS FOR EACH DOT ON A CURVE
    private Vector3 LinearBezierPoint(float t, Vector3 start, Vector3 end)
    {
        float _t = 1f - t;
        return _t * start + t * end;
    }
    private Vector3 QuadraticBezierPoint(float t, Vector3 start, Vector3 end, Vector3 control1)
    {
        float _t = 1f - t;
        return power(_t, 2) * start + 2 * t * _t * control1 + power(t, 2) * end;
    }
    private Vector3 CubeBezierPoint(float t, Vector3 start, Vector3 end, Vector3 control1, Vector3 control2)
    {
        float _t = 1f - t;
        Vector3 result;
        result = power(_t, 3) * start;
        result += 3 * t * power(_t, 2) * control1;
        result += 3 * t * t * _t * control2;
        result += power(t, 3) * end;
        return result;
    }

    // FUNCTIONS FOR CALCULATING EACH CURVE
    private Vector3[] calculateCurveDots(CurveType _type, Vector3 start, Vector3 end)
    {
        Vector3[] result = new Vector3[Detail];
        result[0] = start;

        switch (_type)
        {
            case CurveType.Linear:
                for (int i = 1; i < result.Length - 1; i++)
                {
                    float t = i / (float)(result.Length - 1);
                    result[i] = LinearBezierPoint(t, start, end);
                }
                break;
            case CurveType.Quadratic:
                for (int i = 1; i < result.Length - 1; i++)
                {
                    float t = i / (float)(result.Length - 1);
                    result[i] = QuadraticBezierPoint(t, start, end, p[2].position);
                }
                break;
            case CurveType.Cubic:
                for (int i = 1; i < result.Length - 1; i++)
                {
                    float t = i / (float)(result.Length - 1);
                    result[i] = CubeBezierPoint(t, start, end, p[2].position, p[3].position);
                }
                break;
        }
        result[result.Length - 1] = end;
        return result;
    }
    private void _rotateP2(float speed)
    {
        Vector3 dir = Vector3.Project(p[2].position, p[1].position - p[0].position);

        p[2].RotateAround(dir, p[1].position - p[0].position, speed * Time.deltaTime);

    } // NEED TO FIX, doesnt work if P0, P1 not in standard position    // FIX#1
    private void _rotateP3(float speed)
    {
        Vector3 dir = Vector3.Project(p[3].position, p[1].position - p[0].position);
        p[3].RotateAround(dir, p[1].position - p[0].position, speed * Time.deltaTime);
    } // NEED TO FIX, doesnt work if P0, P1 not in standard position    // FIX#1
    // for children
    private void controlRandom(CurveType _curveType)
    {
        Vector3 dist = p[1].localPosition - p[0].localPosition;

        if ((p2Rnd == Vector3.zero) || (p3Rnd == Vector3.zero) || (p[2].localPosition - p2Rnd).magnitude < accuracyToNext || (p[3].localPosition - p3Rnd).magnitude < accuracyToNext)
        {
            switch (_curveType)
            {
                case CurveType.Quadratic:
                    p2Rnd = new Vector3(Random.Range(-controlRndPower, controlRndPower), Random.Range(-controlRndPower, controlRndPower), 0) + p[0].localPosition + dist / 2f;
                    break;
                case CurveType.Cubic:
                    p2Rnd = new Vector3(Random.Range(-controlRndPower, controlRndPower), Random.Range(-controlRndPower, controlRndPower), 0) + p[0].localPosition + dist / 3f;
                    p3Rnd = new Vector3(Random.Range(-controlRndPower, controlRndPower), Random.Range(-controlRndPower, controlRndPower), 0) + p[0].localPosition + dist / 3f * 2f;
                    break;
                default:
                    break;
            }
        }
        switch (_curveType)
        {
            case CurveType.Quadratic:
                p[2].localPosition = Vector3.Lerp(p[2].localPosition, p2Rnd,lerpSpeed*Time.deltaTime);
                break;
            case CurveType.Cubic:
                    p[2].localPosition = Vector3.Lerp(p[2].localPosition, p2Rnd, lerpSpeed * Time.deltaTime);
                    p[3].localPosition = Vector3.Lerp(p[3].localPosition, p3Rnd, lerpSpeed * Time.deltaTime);
                break;

            default:
                break;
        }
    }


    // Start is called before the first frame update
    private IEnumerator execute()
    {
        while (true)
        {

            LineDots = calculateCurveDots(type, p[0].position, p[1].position);
            if (CanBeCutted) { LineDots = cutter(LineDots); } else { isCollides = false; }
            if (spring) { LineDots = makeSpring(LineDots); }
            if (useNoise) { LineDots = addNoise(LineDots); }
            { // DOESNT WORK #FIX#1
                /*
                if (Application.IsPlaying(gameObject))
                {
                    if (rotateP2)
                    {
                        _rotateP2(p2Speed);
                    }
                    if (rotateP3) { _rotateP3(p3Speed); }
                }
               Vector3 dir = Vector3.Project(p[2].position, p[1].position - p[0].position);
                Debug.DrawLine(dir, p[2].position, Color.red);
                dir = Vector3.Project(p[3].position, p[1].position - p[0].position);
                Debug.DrawLine(dir, p[3].position, Color.green);
                p[2].rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                p[3].rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                */
            }

            //Debug.DrawLine(p[0].position, p[1].position, Color.blue);
            rend.positionCount = LineDots.Length;
            rend.SetPositions(LineDots);
            yield return new WaitForSeconds(updateFrequency);
        }

    }
    void Start()
    {
        if (transform.GetComponent<LineRenderer>() == null)
        {
            gameObject.AddComponent<LineRenderer>();
        } // checks if GameObject have LineRenderer component

        rend = transform.GetComponent<LineRenderer>();

        StartCoroutine(execute());
    }
    private void Update()
    {
        if (ControlRandomizer)
        { controlRandom(type); }


    }
    public BezierCurves()
    {
        p = new Transform[4];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(p[0].position, 0.2f);
        Gizmos.DrawSphere(p[1].position, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(p[2].position, 0.2f);
        Gizmos.DrawSphere(p[3].position, 0.2f);
    }
}
