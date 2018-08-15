using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 m_direction;
    private float m_speed;
    private float m_damage;
    private bool m_isOwnerPlayer;
    private Coroutine m_moveRoutine;


    //Set values and start moving towards a direction
    public void Fire(Vector3 direction, float speed, float damage, bool isPlayer = true)
    {
        m_isOwnerPlayer = isPlayer;
        m_direction = direction;
        m_speed = speed;
        m_damage = damage;

        if (m_moveRoutine != null)
            StopCoroutine(m_moveRoutine);

        float _rot = Mathf.Atan2(m_direction.y, m_direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, _rot);
        m_moveRoutine = StartCoroutine(move());
    }

    //Move and check if outside of bounds
    IEnumerator move()
    {
        while (true)
        {
            transform.position += m_direction * m_speed * Time.deltaTime;
            Vector3 _position = Camera.main.WorldToViewportPoint(transform.position);
            if (_position.x < -0.1 || _position.y > 1.1 || _position.x > 1.1 || _position.y < -0.1)
            {
                StartCoroutine(disappear(1.0f));
                yield break;
            }
            else
                yield return null;
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (m_isOwnerPlayer)
        {
            Enemy _enemy = other.GetComponent<Enemy>();
            if (_enemy != null)
            {
                _enemy.TakeDamage(m_damage);
                StartCoroutine(disappear(0.0f));
            }
        }
        else
        {
            Player _player = other.GetComponent<Player>();
            if (_player != null)
            {
                _player.TakeDamage(m_damage);
                StartCoroutine(disappear(0.0f));
            }
        }
    }

    //Return to pool in set time
    IEnumerator disappear(float waitUntil)
    {
        if (m_moveRoutine != null)
            StopCoroutine(m_moveRoutine);
        m_moveRoutine = null;
        BulletManager.ReturnBullet(this);
        yield return new WaitForSeconds(waitUntil);
    }

}
