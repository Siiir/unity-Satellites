using UnityEngine;

using static ScienceConstants;
using static ProjectConstants;

/// <summary>
/// Contains scientific constants.
/// </summary>
public static class ScienceConstants
{
    public const float G = 6.67E-11F; // 1/kg²m²
}

/// <summary>Constains linear function coefficients.</summary>
public static class ProjectConstants
{
    /// <summary> <see cref="toRealDist"/> is a transition coefficient
    /// for vectors in the model space to get them into real space. </summary>
    public const float toRealDist = 1E+10F;
    /// <summary> <see cref="toRealDistSq"/> is a data-redundant square of <see cref="toRealDist"/>. </summary>
    public const float toRealDistSq = toRealDist * toRealDist;

    /// <summary>
    /// <see cref="toModelDist"/> is a data-redundant multiplicative inverse of <see cref="toRealDist"/>.
    /// </summary>
    public const float toModelDist = 1 / toRealDist;
    /// <summary>
    /// <see cref="toVisualDist"/> is a obsolete clone of <see cref="toModelDist"/>.
    /// </summary>
    [ObsoleteAttribute("Use toModelDist instead.")]
    public const float toVisualDist = toModelDist;


    public const float toRealMass = 1E+25F;
    public const float toModelMass = 1 / toRealMass;

    [ObsoleteAttribute("Use toModelMass instead.")]
    public const float toGameMass = toModelMass;
}

public static class ProjectFunctions
{
    /// <summary>
    /// Scales the real physical body size to one applicable for the model.
    /// </summary>
    /// <param name="orgSize"> Orginal size of the physical body. </param>
    /// <returns> Size that will be visible on the model. </returns>
    public static float scale_bodySize(float orgSize) => Mathf.Pow(orgSize, 0.3F)*0.01F;

    //Mathematical functions
    /// <param name="r1"> Vector3 representing first point localization. </param>
    /// <param name="r2"> Vector3 representing second point localization. </param>
    /// <returns> Squared distance between to 3-dim points. </returns>
    public static float getDistSq(Vector3 r1, Vector3 r2)
    {
        var x = r1.x - r2.x;
        var y = r1.y - r2.y;
        var z = r1.z - r2.z;
        return x * x + y * y + z * z;
    }

    /// <param name="ModelCords1"> Vector3 representing point localization in the model. </param>
    /// <param name="ModelCords2"> Vector3 representing point localization in the model. </param>
    /// <returns> Squared distance between to 3-dim points in the real space. </returns>
    public static float getRealDistSq(Vector3 ModelCords1, Vector3 ModelCords2) {
        return toRealDistSq * getDistSq(ModelCords1, ModelCords2);
    }

    /// <summary>Wrapper</summary>
    /// <param name="body1">
    /// Any <code>GameObject</code> especially physical,
    /// which for sake of performance constains (generally merely) information about model cordinates
    /// that are stored in transform <code>Component</code>
    /// </param>
    /// <param name="body2"> The second body.</param>
    /// <returns> Squared distance between to 3-dim points in the real space. </returns>
    public static float getRealDistSq(GameObject body1, GameObject body2) {
        Vector3 GameCords1 = body1.transform.position;
        Vector3 GameCords2= body2.transform.position;

        return getRealDistSq(GameCords1, GameCords2);
    }

    //Phisical Functions

    public static Vector3 F_g21(float m1, Vector3 r1, float m2, Vector3 r2)
    {
        float distSq = getDistSq(r1, r2);
        float dist = Mathf.Sqrt(distSq);
        return -(G * m1 * m2) / distSq * (r2 - r1) / dist;
    }

    /// <summary>
    /// <see cref="F_g21(GameObject, GameObject)"/> is a wrapper
    /// over low-level overloads of F_g21 that computes vectorical gravitational force
    /// with witch the second body is ...
    /// </summary>
    /// <param name="body1"></param>

    /// <returns>Gravitational force to be aplied on ?...<param name="body2"></param></returns>
    public static Vector3 F_g21(GameObject body1, GameObject body2)
    {
        Vector3 r1 = toRealDist * body1.transform.position;
        Vector3 r2 = toRealDist * body2.transform.position;
        float m1 = body1.GetComponent<BodyProperties>().mass;
        float m2 = body2.GetComponent<BodyProperties>().mass;

        return F_g21(m1, r1, m2, r2);
    }
    

    public static float F_g(float m1, float m2, float distSq) => G*(m1 * m2) / distSq;
    public static float a_g(float F_g, float m) => F_g / m;
    public static Vector3 a_g(Vector3 F_g, float m) => F_g / m;



    //Other functions
    public static T[] afterRemovalOf<T>(T[] Array, int atInd)
    {
        int l = Array.Length;
        T[] newArray = new T[l-1];

        for (int i=0, j=0; i<l; i++)
        {
            if (i != atInd)
            {
                newArray[j++] = Array[i];
            }
        }

        return newArray;
    }

    public static void remove_body(int atInd, bool safe = true)
    {
        CameraMovement CM= null;
        if (safe)
        {
            safe = false;
            CM = Object.FindObjectOfType<CameraMovement>();
            if (CM.targetInd > atInd) CM._targetInd--;
            else if (CM.targetInd == atInd) safe= true;
        }

        GameObject body = Gravitation.allBodies[atInd];
        Gravitation.allBodies = afterRemovalOf(Gravitation.allBodies, atInd);
        Object.Destroy(body);

        if (safe) CM.targetInd = 0;
    }
    //public static void remove_body(int atInd)
}
