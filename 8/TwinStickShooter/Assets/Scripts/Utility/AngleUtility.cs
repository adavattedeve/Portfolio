using UnityEngine;
using System.Collections;

//Provides utility methods for calculating angles
public static class AngleUtility {

    //Returns angle between v1 clockwise to v2
    public static float AngleTo(Vector3 v1, Vector3 v2) {
        float angle = Vector3.Angle(v1, v2);
        return Vector3.Cross(v1, v2).y > 0 ? angle : 360 - angle;
    }

    //Returns angle between v1 to v2
    public static float AngleBetween(Vector3 v1, Vector3 v2)
    {
        float angle = Vector3.Angle(v1, v2);
        return Vector3.Cross(v1, v2).y > 0 ? angle : -angle;
    }

    //returns true if given angle is in given range. Angle and range are given in 0-360 range. No negatives.
    public static bool AngleInRange(Vector2 range, float angle) {
        float maxRange = range.y;
        if (range.x > range.y)
            maxRange += 360;
        if (angle >= range.x && angle <= maxRange) {
            return true;
        }
        else if (angle + 360 >= range.x && angle + 360 <= maxRange) {
            return true;
        }
        return false;
    }

    //Returns delta angle between angle range's centre and angle parameter
    public static float DeltaFromAngleRangeCentre(Vector2 range, float angle)
    {
        float maxRange = range.y;
        if (range.x > range.y)
            maxRange += 360;

        float centre = (range.x + maxRange) / 2;
        if (centre >= 360) {
            centre -= 360;
        }

        return Mathf.Abs(centre - Mathf.Abs(TransformAngleToNegativeScale(angle)));
    }

    //Transforms angle from 0-360 scale to -180-180 scale
    public static float TransformAngleToNegativeScale(float angle) {
        while (angle > 360) {
            angle -= 360;
        }
        return angle > 180 ? -(360 - angle) : angle;
    }
}
