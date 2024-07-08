using UnityEngine;
using static UnityEngine.Mathf;

public class CameraMovement : MonoBehaviour
{
    internal int _targetInd = 0;
    public int targetInd { get => this._targetInd; set { this._targetInd = value; OutputManagement.targetBody.text = this.target.name; } }
    public GameObject target { get => Gravitation.allBodies[this.targetInd]; }
    public Transform targetTransform { get => this.target.transform; }

    [SerializeField]
    float movementSensitiveness, rotationSensitiveness;
    [SerializeField]
    float moveEnforcement, rotationEnforcement;

    public void aim_target()
    {
        transform.LookAt(Gravitation.allBodies[targetInd].transform);
    }
    System.Collections.IEnumerator delayedRotationCorrection()
    {
        yield return new WaitForSeconds(1);
        aim_target();
    }

    private void Start()
    {
        //this.StartCoroutine(delayedRotationCorrection());
        OutputManagement.targetBody.text = this.target.name;
    }


    // Update is called once per frame
    void Update()
    {
        //transformComponent.LookAt(target);
        float t= Time.deltaTime/Time.timeScale;

        //Rotation
        Vector3 rot;
        {
            if (Input.GetKey(KeyCode.Mouse2) || Input.GetKey(KeyCode.Keypad5))
            {
                aim_target();
                rot = transform.localEulerAngles;
            }
            else
            {
                float baseRotation = rotationSensitiveness * t;

                rot = transform.localEulerAngles;
                rot.y += Input.GetAxis("Y Rotation") * baseRotation;
                rot.x -= Input.GetAxis("X Rotation") * baseRotation;
                rot.z -= Input.GetAxis("Z Rotation") * baseRotation;

                transform.localEulerAngles = rot;
            }
        }

        //Location
        {
            float baseMove = movementSensitiveness * t;
            if (Input.GetKey(KeyCode.LeftShift)) baseMove *= moveEnforcement;
            Vector3 loc = transform.localPosition;

            //x,z  move
            {
                float rotYInRad = Deg2Rad * rot.y;
                float Vx = Sin(rotYInRad)*baseMove, Vz= Cos(rotYInRad)*baseMove;

                //Zooming effect
                //..to be coded

                //keys w,s
                {
                    if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
                    {
                        loc.x -= Vx;
                        loc.z -= Vz;
                    }
                    else if (!Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W))
                    {
                        loc.x += Vx;
                        loc.z += Vz;
                    }
                }

                //keys a,d
                {
                    if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) //Counter clockwise
                    {
                        loc.x -= Vz;
                        loc.z += Vx;
                    }
                    else if (!Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) //Clockwise
                    {
                        loc.x += Vz;
                        loc.z -= Vx;
                    }
                }
            }

            //y move;  keys lAlt,space;
            {
                if (Input.GetKey(KeyCode.LeftAlt)) loc.y -= baseMove;
                if (Input.GetKey(KeyCode.Space)) loc.y += baseMove;
            }

            transform.localPosition = loc;
        }
        
    }
}
