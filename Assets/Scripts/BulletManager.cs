using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private List<Bullet> m_bulletList = new List<Bullet>();
    public Bullet BulletPrefab;

    private static BulletManager m_instance;
    public static BulletManager Instance
    { get { return m_instance; } }

    void Awake()
    {
        if (m_instance == null)
            m_instance = this;
        else Destroy(gameObject);
    }

    public static Bullet GetBullet()
    {
        Bullet _bullet;
        int _availableObjectIndex = m_instance.m_bulletList.Count - 1;
        if (_availableObjectIndex >= 0)
        {
            _bullet = m_instance.m_bulletList[_availableObjectIndex];
            m_instance.m_bulletList.RemoveAt(_availableObjectIndex);
            _bullet.gameObject.SetActive(true);
        }
        else
        {
            _bullet = Instantiate<Bullet>(m_instance.BulletPrefab);
            _bullet.transform.SetParent(m_instance.transform, false);
        }
        return _bullet;
    }

    public static void ReturnBullet(Bullet bullet)
    {
        m_instance.m_bulletList.Add(bullet);
        bullet.gameObject.SetActive(false);
    }
}
