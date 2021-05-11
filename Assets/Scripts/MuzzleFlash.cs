using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 2f)] public float flashtime = 0.8f;

    public GameObject muzzleFlash;
    // Start is called before the first frame update
    void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        muzzleFlash.SetActive(true);
        
        Invoke("Deactivate", flashtime);
    }

    void Deactivate()
    {
        muzzleFlash.SetActive(false);
    }
}
