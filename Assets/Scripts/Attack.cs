using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public enum FireMode {Semi, Burst}
public class Attack : MonoBehaviour
{
    public Camera _camera;
    public float range = 100f;
    private Animator _animator;
    public Animator GetAnim { get { return _animator; } }

    [Header("Effects")]
    //private MuzzleFlash muzzleFlash;
    public ParticleSystem muzzleFlash_single;
    public ParticleSystem muzzleFlash_burst;

    [Header("Fire settings")]
    [SerializeField] private FireMode _fireMode = FireMode.Semi;
    [SerializeField] private int numBullet_burst = 3;
    private float timer; // Record the time since last shoot
    private float firetime;
    [SerializeField] [Range(0.1f, 2f)] public float firetime_single = 1f;
    [SerializeField] [Range(0.01f, 2f)] public float firetime_loop = 1f;
    [SerializeField] private int damageAmount = 3;
    
    [SerializeField] [Range(5f, 80f)] public int bulletNumPerClip = 30;
    [SerializeField] private int currentBulletNum = 30;
    private bool reload = false;
    public int clipNum = 0;
    

    // 0: 单射  1：连射
    private int shootMode = 0;
    private float attackCoolDown;
    public float attackPeriod = 0.2f;
    private bool _shootInput = false;
    
    [Header("Audios")]
    private AudioSource playerAS;
    public AudioClip shootAC;
    public AudioClip reloadAC;
    
    [Header("GUI")]
    public GameObject AimImg;

    public GameObject curBulletGO;
    public GameObject maxBulletGO;
    public GameObject clipGO;
    public GameObject modeGO;
    
    private TextMeshProUGUI curBulletTmp;
    private TextMeshProUGUI maxBulletTmp;
    private TextMeshProUGUI clipTmp;
    private TextMeshProUGUI modeTmp;
    // Start is called before the first frame update
    void Start()
    {
        muzzleFlash_burst.Stop();
        muzzleFlash_single.Stop();
        attackCoolDown = 0f;

        firetime = firetime_single;
        //
        _animator = GetComponent<Animator> ();
        currentBulletNum = bulletNumPerClip;
        //muzzleFlash = GetComponent<ParticleSystem>();
        curBulletTmp = curBulletGO.GetComponent<TextMeshProUGUI>();
        maxBulletTmp = maxBulletGO.GetComponent<TextMeshProUGUI>();
        clipTmp = clipGO.GetComponent<TextMeshProUGUI>();
        modeTmp = modeGO.GetComponent<TextMeshProUGUI>();
        
        playerAS = GetComponent<AudioSource>();
        

        UpdateUIText();
    }

    // Update is called once per frame
    void Update()
    {
        attackCoolDown -= Time.deltaTime;
        
        switch (_fireMode)
        {
            case FireMode.Semi:
                _shootInput = Input.GetButtonDown(GameConstants.k_ButtonNameFire);
                break;
            case FireMode.Burst:
                _shootInput = Input.GetButton(GameConstants.k_ButtonNameFire);
                break;
            
        }
        //Reload 结束判断
        if (reload)
        {
            //获取动画层 0 指Base Layer.
            AnimatorStateInfo stateinfo = _animator.GetCurrentAnimatorStateInfo(0);
            //
            if(stateinfo.IsName("Base Layer.Idle_Movement"))
            {
                reload = false;
                _animator.SetBool("Reload", false);
                AimImg.SetActive(true);
                currentBulletNum = bulletNumPerClip;
                UpdateUIText();
            }

            if (stateinfo.IsName("Base Layer.ReloadGun") && AimImg.activeSelf)
            {
                AimImg.SetActive(false);
                playerAS.PlayOneShot(reloadAC);
            }
        }
        //
        timer += Time.deltaTime;
        if (timer >= firetime)
        {
            // shoot
            if (!reload && GameManager.instance.GameOn && currentBulletNum > 0)
            {
                if (_fireMode == FireMode.Semi)
                {
                    if (Input.GetButtonDown(GameConstants.k_ButtonNameFire))
                    {
                        Shoot();
                        timer = 0f;
                    }
                } else if (_fireMode == FireMode.Burst)
                {
                    if (Input.GetButton(GameConstants.k_ButtonNameFire))
                    {
                        BurstShoot();
                        timer = 0f;
                    }
                }
            }
        }
        else if (currentBulletNum <= 0 && clipNum > 0)
        {
            ReloadGun();
            clipNum--;
        }

        /*
        if (Input.GetMouseButton(0))
        {
            _animator.Play("Shoot_AutoShot_AR");
            print("AutoShot");
        }
        */

        if (Input.GetButtonDown(GameConstants.k_ButtonNameSwitchWeapon))
        {
            if (_fireMode == FireMode.Burst)
            {
                _fireMode = FireMode.Semi;
                firetime = firetime_single;
                modeTmp.SetText("单 发");
            }
            else
            {
                _fireMode = FireMode.Burst;
                firetime = firetime_loop;
                modeTmp.SetText("连 发");
            }
            print("Change shoot mode to " + _fireMode);
        }
    }

    private void Shoot()
    {
        muzzleFlash_single.Play();
        playerAS.PlayOneShot(shootAC);
        
        //
        _animator.enabled = false;
        _animator.enabled = true;
        if (_fireMode == FireMode.Burst)
        {
            _animator.Play("Shoot_burst");
        }
        else
        {
            _animator.Play("Shoot");
        }
        
        RaycastHit hitInfo;
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hitInfo, range))
        {
            var health = hitInfo.collider.gameObject.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
            
            print("hit " + hitInfo.collider.gameObject.name);
            //Debug.DrawRay(_camera.transform.position, _camera.transform.forward * 100, Color.red, 12f);
        }
        
        
        // Reload gun if bullet used out
        currentBulletNum--;
        UpdateUIText();
        if (currentBulletNum <= 0 && clipNum > 0)
        {
            ReloadGun();
            clipNum--;
        }
    }
    
    private void BurstShoot()
    {
        if (attackCoolDown <= 0f)
        {
            Shoot();
            attackCoolDown = attackPeriod;
            //muzzleflash.Play();
            //print("attack player");
        }
  
        
    }

    void ReloadGun()
    {
        //_animator.SetTrigger("Reload");
        _animator.SetBool("Reload", true);
        //playerAS.PlayOneShot(reloadAC);
        // Stop shooting
        reload = true;
        print("reload");
        //GameObject.Find("AimTarget").SetActive(false);
    }

    private void UpdateUIText()
    {
        maxBulletTmp.SetText("/ "+ bulletNumPerClip.ToString());
        curBulletTmp.SetText(currentBulletNum.ToString());
        clipTmp.SetText(clipNum.ToString());
    }

    public void addClip(int num)
    {
        clipNum += num;
        UpdateUIText();
    }
}
