using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 5;

    [SerializeField] private int currentHealth;

    //public GameObject HealthUI;
    public Slider healthSlider;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
        healthSlider.value = CaculateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(int damageAmount)
    {
        //HealthUI.SetActive(true);
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            //Die();
        }
        if ((float)currentHealth/(float)startingHealth < 0.4)
        {
            //_animator.SetTrigger("Dying");
        }
        else
        {
            //_animator.SetTrigger("Shoot");
        }
        healthSlider.value = CaculateHealthBar();
    }
    
    private float CaculateHealthBar()
    {
        return ((float)currentHealth/(float)startingHealth);
    }
}
