using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldText : MonoBehaviour
{

    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }
}
