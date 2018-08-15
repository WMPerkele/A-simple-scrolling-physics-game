using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public Vector3 MaxVelocity;
    public Vector3 DragAmount;
    public Vector3 WeaponPosOffset;
    public float BoostPower;
    public float HorizontalMultiplier;
    public float MaxHealth;
    public  List<Weapon> Weapons;
    public LayerMask HurtLayer;
    public LayerMask PickUpLayer;
    public Vector3 SpawnSpot;

    private SpriteRenderer m_spriteRenderer;
    private bool m_isHurt;
    private float m_health;
    private float m_fireInterval;
    private Coroutine m_hurtEffectRoutine;
    private Weapon m_currentWeapon;
    private Vector3 m_mouseDirection;
    private Vector3 m_acceleration;
    private Vector3 m_velocity;
    private Vector3 m_gravityDirection = new Vector2(0,-1);

    const float GRAVITY = 9.81F; //Modify gravity a bit to make it easier to respond to

    void Start()
    {
        for (int i = 0; i < Weapons.Count; i++)
            Weapons[i].gameObject.SetActive(false);
        equipWeapon(Weapons[0]);
        m_health = MaxHealth;
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        checkInput();
        if(!checkIfWithinBound())
        {
            transform.position = SpawnSpot;
            TakeDamage(1.0f);
        }
        m_acceleration += m_gravityDirection * GRAVITY;

        //Clamp our velocity so it wont get too out of control
        m_velocity.Set(Mathf.Clamp(m_velocity.x, -MaxVelocity.x, MaxVelocity.x),
            Mathf.Clamp(m_velocity.y, -MaxVelocity.y, MaxVelocity.y),  0);

        //Add acceleration and apply velocity
        m_velocity += m_acceleration * Time.deltaTime;
        transform.position += m_velocity * Time.deltaTime;

        //Apply some drag to our movement so it's better to control
        m_velocity += Vector3.Scale(-m_velocity.normalized, DragAmount) * Time.deltaTime;
        m_acceleration = Vector3.zero;

        //Reduce cooldown on weapons
        for (int i = 0; i < Weapons.Count; i++)
        {
            if(Weapons[i].Interval >= 0.0f)
                Weapons[i].Interval -= Time.deltaTime;
        }
        //Move our weapon accordingly
        m_currentWeapon.transform.position = transform.position + WeaponPosOffset;
        float _rot = Mathf.Atan2(m_mouseDirection.y, m_mouseDirection.x) * Mathf.Rad2Deg;
        m_currentWeapon.transform.rotation = Quaternion.Euler(0.0f, 0.0f, _rot);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & HurtLayer) != 0)
        {
            TakeDamage(1.0f);
        }
        if(((1 << other.gameObject.layer) & PickUpLayer) != 0)
        {
            for(int i = 0; i < Weapons.Count; i++)
            {
                Weapons[0].AddAmmo = (int)(Weapons[0].MaxAmmo / 10);
            }
            LevelManager.Instance.PickUpObjectManager.ReturnObject(other.gameObject);
        }
    }

    public float GetHealth()
    {
        return m_health;
    }

    public Weapon GetWeapon()
    {
        return m_currentWeapon;
    }

    //Take damage and begin coroutine to do visual effects on hurting
    public void TakeDamage(float damage)
    {
        if (m_isHurt)
            return;

        m_health -= damage;
        if(m_health <= 0.0f)
        {
            GameManager.EndGame();
            return;
        }
        if(m_hurtEffectRoutine != null)
        {
            StopCoroutine(m_hurtEffectRoutine);
            m_hurtEffectRoutine = null;
        }
        m_hurtEffectRoutine = StartCoroutine(hurtRoutine());
    }

    private bool checkIfWithinBound()
    {
        Vector3 _viewPortPos = Camera.main.WorldToViewportPoint(transform.position);
        return _viewPortPos.x < 1.2f && _viewPortPos.x > -0.2f && _viewPortPos.y < 1.2f && _viewPortPos.y > -0.2f;
    }

    private void checkInput()
    {
        m_mouseDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if (Input.GetButton("Boost"))
        {
            m_acceleration += Vector3.up * BoostPower;
        }
        m_acceleration += new Vector3(Input.GetAxis("Horizontal") * HorizontalMultiplier, 0, 0);
        if (Input.GetButton("Fire"))
        {
            fireWeapon();
        }
        if (Input.GetButtonDown("SwitchWeaponLeft"))
            switchWeapon(-1);
        else if (Input.GetButtonDown("SwitchWeaponRight"))
            switchWeapon(1);

    }

    //Get the normalized 2D direction to our mouse position and give weapon fire command
    private void fireWeapon()
    {
        Vector3 _shootingDirection = new Vector2(m_mouseDirection.x, m_mouseDirection.y).normalized;
        if(m_currentWeapon.Fire(_shootingDirection))
            addForce(m_currentWeapon.KnockBack, -_shootingDirection);
    }

    //Circle around the weapons wheel
    private void switchWeapon(int direction)
    {
        int _weaponNumber = 0;
        for (int i = 0; i < Weapons.Count; i++)
        {
            if (m_currentWeapon.Name == Weapons[i].Name)
            {
                _weaponNumber = i;
                break;
            }
        }
        if (direction > 0)
        {
            equipWeapon(_weaponNumber == Weapons.Count - 1 ? Weapons[0] : Weapons[_weaponNumber + 1]);
        }
        else if (direction < 0)
        {
            equipWeapon(_weaponNumber == 0 ? Weapons[Weapons.Count - 1] : Weapons[_weaponNumber - 1]);
        }
    }

    //A function to switch to a weapon and set the previous one unactive
    private void equipWeapon(Weapon equippedWeapon)
    {
        if (m_currentWeapon != null)
            m_currentWeapon.gameObject.SetActive(false);

        m_currentWeapon = equippedWeapon;
        m_currentWeapon.gameObject.SetActive(true);
    }

    //Add impact force to our velocity
   private  Vector3 addForce(float amount, Vector3 direction)
    {
        return m_velocity += direction.normalized * amount;
    }  

    //Handle invis frame
    private IEnumerator hurtRoutine()
    {
        m_isHurt = true;
        float _timer = 0.0f;
        float _timeBetweenToggle = 0.1f;
        bool _toggle = false;
        while(_timer < 1.0f)
        {
            _timer += _timeBetweenToggle;
            _toggle = !_toggle;
            m_spriteRenderer.color = _toggle ? Color.white : Color.red;
            yield return new WaitForSeconds(_timeBetweenToggle);
        }
        m_spriteRenderer.color = Color.white;
        m_isHurt = false;
    }
}
