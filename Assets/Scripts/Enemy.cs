using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy : MonoBehaviour
{
    public enum EAIPath
    {
        Straight = 1,
        Circle = 2,
        Still = 3,
    }

    public EAIPath Path;
    public float Damage;
    public float FireInterval;

    private float m_health;
    private float m_interval;
    private float m_angle;
    private Vector3 m_origin;
    private Player m_player;
    private EnemyObjectManager m_objectManager;
    public EnemyObjectManager ObjectManager
    { set { m_objectManager = value; } }


    void Start()
    {
        m_origin = transform.position;
    }

    public void Initialize(int type, float health)
    {
        if (m_player == null)
            m_player = GameManager.Player;

        m_angle = 0;
        Path = type == 0 ? (EAIPath)Random.Range(1, 4) : (EAIPath)type;
        m_origin = transform.position;
        m_health = health;
    }

	void Update ()
    {
        m_interval -= Time.deltaTime;
        if (m_interval < 0.0f)
        {
            m_interval = FireInterval;
            shootPlayer();
        }
	}

    //Move depending on your enum path, still, up and down or in circles
    public void Move(Vector3 offset)
    {
        m_origin += offset;
        Vector3 _posOffset = new Vector3(0.0f, 0.0f);
        float _angle = 360 * (m_angle % 1.0f) * Mathf.Deg2Rad;
        m_angle += Time.deltaTime * 0.5f;
        switch(Path)
        {
            case EAIPath.Circle:
                _posOffset.Set(Mathf.Cos(_angle), Mathf.Sin(_angle), 0.0f);
                break;
            case EAIPath.Straight:
                _posOffset.Set(0.0f, Mathf.Sin(Time.time), 0.0f);
                break;
        }
        transform.position = m_origin + _posOffset;
    }

    //Take damage and do a death check
    public void TakeDamage(float damage)
    {
        m_health -= damage;
        if (m_health <= 0.0f)
        {
            LevelManager.Instance.PickUpObjectManager.SpawnObject(transform.position);
            GameManager.Instance.AddKill();
            m_objectManager.ReturnObject(gameObject);
        }
    }   

    //Aim and shoot at player
    private void shootPlayer()
    {
        Bullet _bullet = BulletManager.GetBullet();
        Vector3 _direction = (m_player.transform.position - transform.position);
        _direction = new Vector2(_direction.x, _direction.y).normalized;
        _bullet.transform.position = transform.position + _direction;
        _bullet.Fire(_direction, 7.5f, Damage, false);
    }
}
