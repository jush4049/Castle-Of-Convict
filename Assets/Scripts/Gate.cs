using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public Transform[] gate;
    public int gateList;

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" && GameManager.isKey)
        {
            for (int i = 0; i < gateList; i++)
            {
                gate[i].Rotate(new Vector3(0, 90, 0));
            }
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player" && GameManager.isKey)
        {
            for (int i = 0; i < gateList; i++)
            {
                gate[i].Rotate(new Vector3(0, -90, 0));
            }
        }
    }

}
