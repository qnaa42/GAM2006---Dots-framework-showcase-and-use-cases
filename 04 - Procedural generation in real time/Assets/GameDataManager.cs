using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public static class GameDataManager
{
    public static Vector3 playerPosition;

    public static float scale1;
    public static float strenght1;
    public static bool active1;

    public static float scale2;
    public static float strenght2;
    public static bool active2;

    public static float scale3;
    public static float strenght3;
    public static bool active3;

    public static Entity sand;
    public static Entity dirt;
    public static Entity grass;
    public static Entity rock;
    public static Entity snow;

    public static float sandLevel;
    public static float dirtLevel;
    public static float grassLevel;
    public static float rockLevel;
    public static float snowLevel;

    public static bool changedData = false;


}
