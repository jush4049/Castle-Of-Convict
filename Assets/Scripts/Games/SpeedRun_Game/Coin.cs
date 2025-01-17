using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    float rotateSpeed = 150;

    public AudioSource audioSource;

    void Update()
    {
        transform.Rotate(Vector3.right * Time.deltaTime * rotateSpeed);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            audioSource.Play();
            SpeedRunManager.coinCount++;
            GaugeBar.time += 3;
            gameObject.SetActive(false);
        }
    }
}
