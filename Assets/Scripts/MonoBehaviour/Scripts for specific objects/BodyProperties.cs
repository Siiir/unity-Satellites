using System;
using UnityEngine;
//using static UnityEngine.Mathf;
using static ProjectConstants;
using static ProjectFunctions;
//using static UnityEngine.Debug;

public class BodyProperties : MonoBehaviour
{
    public float mass= 5.9722E+24F, diameter= 12_742_000; //kg, m; real values

    public float radius { get => this.diameter/2; set { this.diameter = value * 2; } }
    //internal float visualD; //is this data redundancy useful?
    public float volume { get { float r = this.radius; return (4.0F / 3) * Mathf.PI * (r * r * r); } set { float rCb = value*3.0F / (Mathf.PI * 4); float r = Mathf.Pow(rCb, 1.0F/3); diameter = 2 * r; } }

    protected void visualInit()
    {
        float visualD = scale_bodySize(this.diameter);
        this.transform.localScale = new Vector3(visualD, visualD, visualD);
    }
    protected void ModelInit()
    {
        visualInit();
        this.GetComponent<Rigidbody>().mass = this.mass * toGameMass;
    }

    protected void Awake() //Init
    {
        this.ModelInit();
    }

    public bool has_inside(Transform anotherTransform)
    {
        float dist= Vector3.Distance(transform.position, anotherTransform.position);
        float R = transform.lossyScale.x / 2;
        float r = anotherTransform.lossyScale.x / 2;

        return dist < R - r;
    }

    public bool is_anyContaining(Transform anotherTransform)
    {
        float distSq = getDistSq(transform.position, anotherTransform.position);
        float R = transform.lossyScale.x / 2;
        float r = anotherTransform.lossyScale.x / 2;

        float delta = R - r;
        return distSq < delta*delta;
    }

    public void Eat(float mass, float volume)
    {
        this.mass += mass;
        this.volume += volume;
    }
    public void Eat_body(int atInd)
    {
        GameObject[] allBodies = Gravitation.allBodies;
        CameraMovement CM = UnityEngine.Object.FindObjectOfType<CameraMovement>();
        bool gainFocus = false;
        if (CM.targetInd > atInd) CM._targetInd--;
        else if (CM.targetInd == atInd) gainFocus = true;

        GameObject anotherBody = allBodies[atInd];
        BodyProperties aBP = anotherBody.GetComponent<BodyProperties>();
        this.Eat(aBP.mass, aBP.volume);
        this.ModelInit();

        remove_body(atInd, false);
        if (gainFocus) { for (int i = 0, stop = allBodies.Length; i < stop; i++) if (allBodies[i] == this.gameObject) CM.targetInd= i; }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
