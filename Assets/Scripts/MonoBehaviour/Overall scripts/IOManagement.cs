using UnityEngine;
using UnityEngine.UI;

using static ProjectFunctions;
using static InputManagement;
using static OutputManagement;

/*public static class UserMethods
{
    public static void remove_targetBody()
    {
        remove_body(Object.FindObjectOfType<CameraMovement>().targetInd);
    }
}*/

public static class InputManagement
{
    
}

public static class OutputManagement
{
    static public Text targetBody;
    static public Text timeScale;
    static public Text toVisualDist;
}

//Change names to IOManager
public class IOManagement : MonoBehaviour
{
    static protected CameraMovement cameraMovement;
    public void remove_targetBody()
    {
        remove_body(cameraMovement.targetInd);
    }
    protected void Awake()
    {
        cameraMovement = Object.FindObjectOfType<CameraMovement>();

        targetBody = GameObject.Find("Text – targetBody").GetComponent<Text>();
        timeScale = GameObject.Find("Text – timeScale value").GetComponent<Text>();
        toVisualDist = GameObject.Find("Text – toVisualDist value").GetComponent<Text>();
    }
    protected void Start()
    {
        timeScale.text = System.Convert.ToString(Time.timeScale);
        toVisualDist.text = System.Convert.ToString(ProjectConstants.toVisualDist);
    }
    void Update()
    {
        float tScaleInc = Input.GetAxis("Time Speed");
        if (tScaleInc != 0)
        {
            Time.timeScale += 0.02F * tScaleInc * Time.timeScale;
            timeScale.text = System.Convert.ToString( Time.timeScale );
        }

        {
            int l = Gravitation.allBodies.Length;
            for (int i=0; i<l; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i)) { cameraMovement.targetInd = i; }
            }
        }
    }
}