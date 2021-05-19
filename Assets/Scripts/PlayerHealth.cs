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
    public Image sliderImg;

    [Header("Damage Screen")] 
    private Image DamImage;
    public Color DamColor;
    public float colorSmoothing = 6f;
    private bool isTakingDamage = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
        healthSlider.value = CaculateHealthBar();
        DamImage = GameManager.instance.DamageImg;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTakingDamage)
        {
            DamImage.color = DamColor;
            isTakingDamage = false;
        }
        else
        {
            DamImage.color = Color.Lerp(DamImage.color, Color.clear, colorSmoothing*Time.deltaTime);
        }
    }
    public void TakeDamage(int damageAmount)
    {
        isTakingDamage = true;
        //HealthUI.SetActive(true);
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
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

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > startingHealth)
        {
            currentHealth = startingHealth;
        }
        healthSlider.value = CaculateHealthBar();
    }
    
    private float CaculateHealthBar()
    {
        if (currentHealth <= 1)
        {
            sliderImg.color = Color.red;
        }
        else
        {
            sliderImg.color = Color.green;
        }
        return ((float)currentHealth/(float)startingHealth);
    }

    private void Die()
    {
        GameManager.instance.LoseLevelImg.gameObject.SetActive(true);
        GameManager.instance.GameOn = false;
    }
}
