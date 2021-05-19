using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickUpCategory {Heal, Clip}
public class PickUp : MonoBehaviour
{
    public enum PickUpCategory {Heal, Clip}
    [SerializeField] private PickUpCategory pickUpCategory = PickUpCategory.Heal;
    public AudioClip pickUpSound;
    [Header("Movement")]
    public float Speed = 1.5f;

    private Vector3 startPos;

    private AudioSource pickupAS;
    // Start is called before the first frame update
    void Start()
    {
        startPos = gameObject.transform.position;
        pickupAS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = startPos + 0.2f * new Vector3(0, Mathf.Sin(Speed * (Time.time)), 0);
        gameObject.transform.Rotate(Vector3.up,Speed * 10f * Time.deltaTime );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //buff
            if (pickUpCategory == PickUpCategory.Heal)
            {
                //加血
                GameManager.instance.player.GetComponent<PlayerHealth>().Heal(2);
            } 
            else if (pickUpCategory == PickUpCategory.Clip)
            {
                //加弹夹
                GameManager.instance.player.GetComponent<Attack>().addClip(1);
            }
            
            //
            GameManager.instance.player.GetComponent<PlayerController>().PickUp();
            //destroy itself
            Destroy(gameObject);
        }
    }
}
