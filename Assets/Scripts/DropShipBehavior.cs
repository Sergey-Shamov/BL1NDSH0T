using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShipBehavior : MonoBehaviour, INpcBehavior
{
    public int speed = 10;
    public int HP = 400;
    public int bombRate = 4;

    private bool m_isCrashing;
    private float m_bombDelay;
    private float m_remainingDelay;
    private Color m_bombColor = Color.white;

    void Start()
    {
        m_bombDelay = (float)1 / bombRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GetInstance().GetNpcManager().IsGameOver())
            Destroy(gameObject);
        else
        {
            if (m_isCrashing)
            {
                if (0 >= m_remainingDelay)
                    m_bombColor = UnityEngine.Random.ColorHSV();
                transform.position += transform.forward * speed * Time.deltaTime;
            }
            else
            {
                transform.position += transform.forward * speed * Time.deltaTime;
            }

            if (0 < m_remainingDelay)
                m_remainingDelay -= Time.deltaTime;
            else
            {
                // right bomb
                GameObject projectile = Instantiate(GameManager.GetInstance().GetProjectileManager().paintBombPrefab, transform.position + transform.right * 3, transform.rotation);
                projectile.GetComponent<PaintProjectileBehavior>().paintColor = m_bombColor;
                projectile.GetComponent<Rigidbody>().velocity = (transform.forward * speed) + Vector3.down * 5 + transform.right * 4;
                // left bomb
                projectile = Instantiate(GameManager.GetInstance().GetProjectileManager().paintBombPrefab, transform.position - transform.right * 3, transform.rotation);
                projectile.GetComponent<PaintProjectileBehavior>().paintColor = m_bombColor;
                projectile.GetComponent<Rigidbody>().velocity = (transform.forward * speed) + Vector3.down * 5 - transform.right * 4;

                m_remainingDelay = m_bombDelay;
            }
        }
    }

    public void DealDamage(int amount)
    {
        HP -= amount;
        if (0 >= HP)
        {
            m_isCrashing = true;
            m_bombDelay = .2f;
        }
    }
}
