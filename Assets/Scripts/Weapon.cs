using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Weapon : MonoBehaviour
{
    public enum EFireType
    {
        Normal,
        Spread,
        Fast
    }

    public string Name;
    public EFireType FireType;
    public float Power;
    public float FireRate;
    public int MaxAmmo;
    public float KnockBack;

    public float Interval;
    public Sprite WeaponTexture;

    private int m_ammo;
    public int AddAmmo
    { set { m_ammo += value; } }

    void Start()
    {
        m_ammo = MaxAmmo;
    }

    //Get a bullet from the manager and give it direction to go to
    public bool Fire(Vector3 direction)
    {
        if (m_ammo == 0 || Interval > 0.0f)
            return false;


        Vector3 _shootingDirection = direction;
        if (FireType == EFireType.Spread)
        {
            for(int i = 0; i < 5; i++)
            {
                Vector3 _spreadDirection = _shootingDirection;
                int _angle = -6 + i * 3;
                float _angleInRadians = Mathf.Deg2Rad * _angle;
                _spreadDirection = new Vector2(_shootingDirection.x * Mathf.Cos(_angleInRadians) - _shootingDirection.y * Mathf.Sin(_angleInRadians),
                    _shootingDirection.y * Mathf.Cos(_angleInRadians) + _shootingDirection.x * Mathf.Sin(_angleInRadians));
                fire(_spreadDirection);
            }
        }
        else
            fire(_shootingDirection);

        m_ammo--;
        Interval = FireRate;
        return true;
    }


    private void fire(Vector3 direction)
    {
        Bullet _bullet = BulletManager.GetBullet();
        _bullet.transform.position = transform.position + direction;
        _bullet.Fire(direction, FireType == EFireType.Fast ? 50 : 10, Power);
    }

}