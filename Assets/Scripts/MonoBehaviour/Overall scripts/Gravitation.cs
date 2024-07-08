using static ProjectFunctions;
using static ProjectConstants;
using UnityEngine;

public class Gravitation : MonoBehaviour
{
    public static GameObject[] allBodies;

    protected void init_allBodies()
    {
        BodyProperties[] AllBPs = FindObjectsOfType<BodyProperties>();
        int l = AllBPs.Length;
        allBodies = new GameObject[l];

        int i = 0;
        foreach (BodyProperties BP in AllBPs)
        {
            allBodies[i++] = BP.gameObject;
        }
    }

    private void Awake()
    {
        init_allBodies();
    }

    void FixedUpdate()
    {
        //Debug.Log($"Here {allBodies.Length} bodies:");
        //foreach (GameObject GO in allBodies) Debug.Log(GO);
        float deltaT = Time.fixedDeltaTime;

        GameObject body1, body2;
        int lastInd = allBodies.Length-1;
        for (int i= 0; i<lastInd; i++ )
        {
            //Debug.Log("In exter. loop.");
            body1 = allBodies[i];
            BodyProperties BP1 = body1.GetComponent<BodyProperties>();
            float m1 = BP1.mass;
            for (int j= i+1; j<=lastInd;)
            {
                //Debug.Log("In inter. loop.");
                body2 = allBodies[j];

                BodyProperties BP2 = body2.GetComponent<BodyProperties>();

                float m2 = BP2.mass;
                if (BP1.is_anyContaining(body2.transform))
                {
                    //Debug.Log("In If.");
                    lastInd -= 1;
                    if (m1 >= m2) { BP1.Eat_body(j); m1 = BP1.mass; }
                    else { BP2.Eat_body(i); i--; break; }
                }
                else
                {
                    //Debug.Log("In else");
                    Vector3 F2 = F_g21(body1, body2);  //To optimize.
                    Vector3 F1 = -F2;
                    Vector3 a2 = F2 / m2, a1 = F1 / m1;

                    Vector3 v2 = a2 * deltaT, v1 = a1 * deltaT;
                    Vector3 visualV2 = v2 * toVisualDist, visualV1 = v1 * toVisualDist;

                    body1.GetComponent<Rigidbody>().AddForce(v1, ForceMode.VelocityChange);
                    body2.GetComponent<Rigidbody>().AddForce(v2, ForceMode.VelocityChange);

                    j++;
                }
            }
        }
    }
}
