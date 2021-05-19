using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UI;
using Random = System.Random;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 5;

    [SerializeField] public int currentHealth;

    public GameObject HealthUI;
    public Slider healthSlider;
    public ParticleSystem BloodEffect;
    [Header("Animation")]
    private Animator _animator;
    public Animator GetAnim { get { return _animator; } }
    
    private NavMeshAgent agent;
    
    private IEnumerator coroutine;

    public GameObject[] Pickups;
    [Header("Audio")]
    private AudioSource enemyAS;
    public AudioClip dieAC;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator> ();
        healthSlider.value = CaculateHealthBar();
        agent = GetComponent<NavMeshAgent>();
        enemyAS = GetComponent<AudioSource>();
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
            agent.speed = 0f;
            Die();
        }
        if ((float)currentHealth/(float)startingHealth < 0.4)
        {
            _animator.SetTrigger("Dying");
            agent.speed = 0.5f;
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
        
        enemyAS.PlayOneShot(dieAC);
    
        coroutine = SelfDestroy();
        StartCoroutine(coroutine);

        

    }
    private IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(2);
        //生成道具
        int idx = UnityEngine.Random.Range(0, Pickups.Length);
        Instantiate(Pickups[idx], gameObject.transform.position, Quaternion.identity);
        
        Destroy(gameObject);
        print(gameObject.name + " destroyed");
        
    }


    private float CaculateHealthBar()
    {
        return ((float)currentHealth/(float)startingHealth);
    }
}
