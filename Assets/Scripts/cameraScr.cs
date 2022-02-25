using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScr : MonoBehaviour
{
    [SerializeField] private GameObject hero;

    void Update()
    {
        transform.position = hero.transform.position + new Vector3(0, 1, -10);
    }
}
