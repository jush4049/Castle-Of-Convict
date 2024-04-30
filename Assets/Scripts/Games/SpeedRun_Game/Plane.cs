using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Collider boxCollider = gameObject.GetComponent<BoxCollider>();
            //boxCollider.enabled = false;
            gameObject.AddComponent<Rigidbody>();
        }

        if (col.gameObject.tag == "Fall")
        {
            Collider boxCollider = gameObject.GetComponent<BoxCollider>();
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();

            boxCollider.enabled = true;
            Destroy(rb);
            gameObject.transform.localPosition = new Vector3(-0.115f, 0, -0.03f);
        }
    }
}
