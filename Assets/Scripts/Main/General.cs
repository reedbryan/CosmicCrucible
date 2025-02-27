using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General : MonoBehaviour
{

    /// <summary>
    /// Applies a force in the direction the subject is facing. 
    /// </summary>
    /// <param name="force"></param>
    /// <param name="tsf"></param>
    /// <param name="Rb"></param>
    public static void ForwardForce(float force, Transform tsf, Rigidbody2D Rb)
    {
        // Physics
        float radAngle = tsf.eulerAngles.z * Mathf.Deg2Rad;

        float x = Mathf.Sin(radAngle) * -1f;
        float y = Mathf.Cos(radAngle);

        Rb.AddForce(new Vector2(x, y) * force * Time.deltaTime);
    }

    /// <summary>
    /// Rotation speed should be ~1 for slow movement and ~10 for fast movement.
    /// </summary>
    /// <param name="tsf"></param>
    /// <param name="target"></param>
    /// <param name="rotationSpeed"></param>
    public static void RotateTowards(Transform tsf, Vector2 target, float rotationSpeed)
    {
        float angle = getAngle(tsf.position, target);
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        tsf.rotation = Quaternion.Slerp(tsf.rotation, q, Time.deltaTime * rotationSpeed);
    }

    /// <summary>
    /// Returns the distance between pos1 and pos2.
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <returns></returns>
    public static float Distance(Vector2 pos1, Vector2 pos2)
    {
        Vector2 pos = pos1 - pos2;

        float distance = Mathf.Sqrt(Mathf.Pow(pos.x, 2) + Mathf.Pow(pos.y, 2));

        return Mathf.Abs(distance);
    }

    /// <summary>
    /// Returns the angle on pos1's z-axis so it is facing pos2.
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <returns></returns>
    public static float getAngle(Vector2 pos1, Vector2 pos2)
    {
        Vector2 xy = pos2 - new Vector2(pos1.x, pos1.y);
        float angle = (Mathf.Atan2(xy.y, xy.x) * Mathf.Rad2Deg) - 90;
        return angle;
    }

    /// <summary>
    /// Returns the time it will take for a object to reach a position with an initial velocity and drag.
    /// </summary>
    /// <param name="currpos"></param>
    /// <param name="newpos"></param>
    /// <param name="initialVelocity"></param>
    /// <param name="drag"></param>
    /// <returns></returns>
    public static float timeToReach(Vector2 currpos, Vector2 newpos, Vector2 initialVelocity, float drag)
    {
        float floatVelocity = Mathf.Sqrt(Mathf.Pow(initialVelocity.x, 2) + Mathf.Pow(initialVelocity.y, 2));
        Vector2 netpos = currpos - newpos;
        float desiredDistance = Mathf.Sqrt(Mathf.Pow(netpos.x, 2) + Mathf.Pow(netpos.y, 2));
        float possibleDistance = floatVelocity / drag;
        if (desiredDistance > possibleDistance)
            return 0;
        float percentDistance = desiredDistance / possibleDistance;

        float percentTime = (-1 / drag) * Mathf.Log(1 - percentDistance);
        return percentTime;
    }


    /// <summary>
    /// Returns the distance an object can traval in a timeframe with decreasing velocity.
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="initialVelocity"></param>
    /// <param name="drag"></param>
    /// <returns></returns>
    public static Vector2 distanceInTime(Vector2 currpos, float time, Vector2 initialVelocity, float drag)
    {
        if (drag == 0) drag = 1;
        Vector2 newpos = currpos + (initialVelocity / drag) * (1 - Mathf.Exp(-drag * time));
        return newpos;
    }
}
