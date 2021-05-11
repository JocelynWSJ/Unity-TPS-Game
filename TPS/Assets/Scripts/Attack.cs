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

    //private MuzzleFlash muzzleFlash;
    public ParticleSystem muzzleFlash_single;
    public ParticleSystem muzzleFlash_burst;

    [SerializeField] private FireMode _fireMode = FireMode.Semi;
    [SerializeField] private int numBullet_burst = 3;
    private float timer; // Record the time since last shoot
    [SerializeField] [Range(0.1f, 2f)] public float firetime = 1f;
    [SerializeField] private int damageAmount = 3;
    
    [SerializeField] [Range(5f, 80f)] public int bulletNumPerClip = 30;
    [SerializeField] private int currentBulletNum = 30;
    private bool reload = false;

    public GameObject AimImg;

    public GameObject curBulletGO;
    public GameObject maxBulletGO;
    private TextMeshProUGUI curBulletTmp;
    private TextMeshProUGUI maxBulletTmp;
    

    // 0: 单射  1：连射
    private int shootMode = 0;
    private bool _shootInput = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator> ();
        currentBulletNum = bulletNumPerClip;
        //muzzleFlash = GetComponent<ParticleSystem>();
        curBulletTmp = curBulletGO.GetComponent<TextMeshProUGUI>();
        maxBulletTmp = maxBulletGO.GetComponent<TextMeshProUGUI>();

        UpdateUIText();
    }

    // Update is called once per frame
    void Update()
    {
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

            if (stateinfo.IsName("Base Layer.ReloadGun"))
            {
                AimImg.SetActive(false);
            }
        }
        //
        timer += Time.deltaTime;
        if (timer >= firetime)
        {
            // shoot
            if (!reload)
            {
                if (_fireMode == FireMode.Semi)
                {
                    if (Input.GetButtonDown(GameConstants.k_ButtonNameFire))
                    {
                        SemiShoot();
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
                
                /*
                if (shootMode == 0) //单射
                {
                    Shoot();
                    muzzleFlash_single.Play();
                    _animator.Play("Shoot");
                    timer = 0f;
                }
                else
                {
                    muzzleFlash_burst.Play();
                    _animator.Play("Shoot_burst");
                    Shoot();
                    Shoot();
                    Shoot();
                    timer = 0f;
                }
                */
            }
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
            }
            else
            {
                _fireMode = FireMode.Burst;
            }
            print("Change shoot mode to " + _fireMode);
        }
    }

    private void SemiShoot()
    {
        muzzleFlash_single.Play();
        _animator.Play("Shoot");
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
        if (currentBulletNum <= 0)
        {
            ReloadGun();
        }
    }
    
    private void BurstShoot()
    {
        muzzleFlash_burst.Play();
        _animator.Play("Shoot_burst");
        for (int i = 0; i < numBullet_burst; i++)
        {
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
            if (currentBulletNum <= 0)
            {
                print("break");
                break;
            }
        }
        if (currentBulletNum <= 0)
        {
            ReloadGun();
        }
        
    }

    void ReloadGun()
    {
        //_animator.SetTrigger("Reload");
        _animator.SetBool("Reload", true);
        
        // Stop shooting
        reload = true;
        print("reload");
        //GameObject.Find("AimTarget").SetActive(false);
    }

    private void UpdateUIText()
    {
        maxBulletTmp.SetText("/ "+ bulletNumPerClip.ToString());
        curBulletTmp.SetText(currentBulletNum.ToString());
    }
}
