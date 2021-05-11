using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 5;

    [SerializeField] private int currentHealth;

    public GameObject HealthUI;
    public Slider healthSlider;
    public ParticleSystem BloodEffect;
    
    private Animator _animator;
    public Animator GetAnim { get { return _animator; } }
    
    private IEnumerator coroutine;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator> ();
        healthSlider.value = CaculateHealthBar();
    }

    private void Update()
    {
        // bloodbar 始终面向camera
        var mainCamera = FindObjectOfType<Camera>();
        var cameraRot = Quaternion.LookRotation(mainCamera.transform.TransformVector(Vector3.forward), mainCamera.transform.TransformVector(Vector3.up)); 
        cameraRot = new Quaternion(0, cameraRot.y, 0, cameraRot.w);
        HealthUI.transform.SetPositionAndRotation(HealthUI.transform.position, cameraRot);
    }

    private void OnEnable()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        BloodEffect.Play();
        //HealthUI.SetActive(true);
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
        if ((float)currentHealth/(float)startingHealth < 0.4)
        {
            _animator.SetTrigger("Dying");
        }
        else
        {
            _animator.SetTrigger("Shoot");
        }
        healthSlider.value = CaculateHealthBar();
    }

    private void Die()
    {
        // Die animation
        print(gameObject.name + "Die");
        _animator.SetTrigger("Die");
        HealthUI.SetActive(false);
    
        coroutine = SelfDestroy();
        StartCoroutine(coroutine);


    }
    private IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
        print(gameObject.name + " destroyed");
    }


    private float CaculateHealthBar()
    {
        return ((float)currentHealth/(float)startingHealth);
    }
}
