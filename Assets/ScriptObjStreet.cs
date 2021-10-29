using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StreetData", menuName = "My Game/Street Data")]

public class ScriptObjStreet : ScriptableObject
{
    public int Length;
    public int Width;
    public bool xOriented;
    public Material Color;

    public Intersection[] intersections;

    public streetObj[] objects;
}

[System.Serializable] 
public class Intersection
{
    public Vector3 position;
    public Vector3 otherPosition;
    public ScriptObjStreet other;
}

[System.Serializable] 
public class streetObj
{
    public GameObject myPrefab;
    public Vector3 streetPos;
}