using UnityEngine;
using System.Collections;

public static class Extensions {
    public static bool IsInRange(this int value, int min, int max) {
        return value <= max && value >= min;
    }
    public static bool IsInRange(this float value, float min, float max)
    {
        return value <= max && value >= min;
    }
}
