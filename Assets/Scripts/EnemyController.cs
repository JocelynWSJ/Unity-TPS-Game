using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 20f;

    private Transform target;

    private NavMeshAgent agent;
    private Animator animator;

    private PlayerHealth playerHealth;
    private EnemyHealth enemyHealth;

    private float attackCoolDown;

    public float attackPeriod = 1f;
    public int attackDamage = 1;

    public ParticleSystem muzzleflash;

    [Header("Audios")]
    private AudioSource enemyAS;

    public AudioClip flameAC;
    
    // Start is called before the first frame update
    void Start()
    {
        muzzleflash.Stop();
        
        animator = GetComponent<Animator> ();
        target = GameManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        playerHealth = GameManager.instance.player.GetComponent<PlayerHealth>();
        enemyHealth = GetComponent<EnemyHealth>();
        attackCoolDown = 0f;
        enemyAS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, gameObject.transform.position);
        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);
            //start run to player
            animator.SetFloat("Blend", 1f);

            if (distance <= agent.stoppingDistance)
            {
                //attack
                attackCoolDown -= Time.deltaTime;
                AttackPlayer();
                //face the player
                FacePlayer();
            }
        }
        if (distance >= lookRadius*1.3f)
        {
            animator.SetFloat("Blend", 0f);
        }
        
    }

    void FacePlayer()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookDir = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, Time.deltaTime*5f);
        
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    void AttackPlayer()
    {
        if (attackCoolDown <= 0f && enemyHealth.currentHealth > 0)
        {
            playerHealth.TakeDamage(attackDamage);
            attackCoolDown = attackPeriod;
            muzzleflash.Play();
            enemyAS.PlayOneShot(flameAC);
            animator.SetTrigger("Attack");
            
            print("attack player");
        }
    }
}
