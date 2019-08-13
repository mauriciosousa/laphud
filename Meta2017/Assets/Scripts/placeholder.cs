using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MyEnum
{
    V = 1,
    W = 10,
    X = 20,
    Y = 30,
    Z = 60
}


public class placeholder : MonoBehaviour {

    [HideInInspector]
    public MyEnum fieldA = MyEnum.X;

    [HideInInspector]
    public MyEnum fieldB = MyEnum.Y;

    // Not Hidden
    public MyEnum fieldC = MyEnum.Z;

    [Range(1, 10)]
    public int testInt = 1;

    List<Vector3> listOfCoordinates = new List<Vector3>(3);
}
