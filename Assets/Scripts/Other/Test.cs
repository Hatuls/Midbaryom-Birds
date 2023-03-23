using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private float _angle;

    [SerializeField]
    private Vector3 point;
    public float GetAngle(Vector3 point1, Vector3 point2)
    {
        float xDis = point2.x - point1.x;
        float zDis = point2.z - point1.z;

        return Mathf.Atan2(zDis,xDis ) * Mathf.Rad2Deg;
    }

    [SerializeField]
    private float _angleTest;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 rotatedVector = Quaternion.AngleAxis(_angle+270, Vector3.up) * transform.right;
        Gizmos.DrawLine(transform.position, transform.position + rotatedVector);

        rotatedVector = Quaternion.AngleAxis(270-_angle, Vector3.up) * transform.right;
        Gizmos.DrawLine(transform.position, transform.position + rotatedVector);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(point, 1);

        var x = GetAngle(transform.position, point) * -1;
         rotatedVector = Quaternion.AngleAxis(-_angle , Vector3.up) * transform.right;
        Gizmos.DrawLine(transform.position, transform.position + rotatedVector);
        print(x);
        Gizmos.color = AngleToTimeToLerp(x) == -1 ? Color.yellow : Color.red;
        rotatedVector = Quaternion.AngleAxis(x, Vector3.up) * transform.right;
        Gizmos.DrawLine(transform.position, transform.position + rotatedVector);



    }


    public float AngleToTimeToLerp(float Angle)
    {
        float leftToCat = -90 - _angle;
        float rightToCat = -90 + _angle;

        if (Angle <= rightToCat && Angle >= leftToCat)
        {
            return -1;
        }
        else
        {
            return 1;
        }

    }
}
