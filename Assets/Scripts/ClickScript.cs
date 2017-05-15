using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickScript : MonoBehaviour
{
    public enum GunMode
    {
        UIPointer,
        PaintGun
    }

    public Transform launcherTransform;
    public Transform laserEmitterTransform;
    public GameObject laserDotPrefab;
    public Texture2D splashTexture;
    public float fireRate = 2;

    private const int c_colorArrayLength = 6;
    private readonly Color[] c_colorArray = new Color[c_colorArrayLength] { Color.blue, Color.yellow, Color.red, Color.green, Color.white, Color.black };

    private GunMode m_mode = GunMode.UIPointer;

    private LineRenderer m_lineRenderer;
    private GameObject m_laserDot;
    private int m_currColor = 0;

    private float m_projectileDelay;        // delay between projectiles
    private float m_nextProjectileDelay;    // remaining time until next projectile

    private void Start()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        m_laserDot = Instantiate(laserDotPrefab, Vector3.zero, Quaternion.identity);
        m_laserDot.SetActive(false);

        m_projectileDelay = 1 / fireRate;
    }

    void Update ()
    {
        VRInputManager.SetIsControllerButtonPressed(Input.GetMouseButton(0));

        if (m_mode == GunMode.UIPointer)
        {
            RaycastHit hit;
            if (Physics.Raycast(laserEmitterTransform.position, transform.forward, out hit))
            {
                m_laserDot.SetActive(true);
                m_laserDot.transform.position = hit.point;
                m_laserDot.transform.LookAt(laserEmitterTransform.position);
                m_lineRenderer.SetPositions(new Vector3[2] { laserEmitterTransform.position, hit.point });
            }
            else
            {
                m_laserDot.SetActive(false);
                m_lineRenderer.SetPositions(new Vector3[2] { laserEmitterTransform.position, laserEmitterTransform.position + transform.forward * 20 });
            }

            if (VRInputManager.GetIsControllerButtonPressed())
            {
                m_lineRenderer.startColor = Color.green;
                m_lineRenderer.endColor = Color.green;
            }
            else
            {
                m_lineRenderer.startColor = Color.red;
                m_lineRenderer.endColor = Color.red;
            }
        }
        else
        {
            if (m_nextProjectileDelay > 0)
                m_nextProjectileDelay -= Time.deltaTime;
            if (VRInputManager.GetIsControllerButtonPressed())
            {
                if (m_nextProjectileDelay <= 0)
                {
                    GameObject projectile = Instantiate(GameManager.GetInstance().GetProjectileManager().paintBombPrefab, launcherTransform.position + launcherTransform.forward * 3, launcherTransform.rotation);
                    projectile.GetComponent<PaintProjectileBehavior>().paintColor = GameManager.GetInstance().GetProjectileManager().paintBombColor;
                    projectile.GetComponent<Rigidbody>().velocity = launcherTransform.forward * 20;
                    m_nextProjectileDelay = m_projectileDelay;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                GameManager.GetInstance().GetProjectileManager().paintBombColor = c_colorArray[m_currColor];
                m_currColor++;
                if (m_currColor == c_colorArrayLength)
                    m_currColor = 0;
            }

            // TODO: DEBUG ONLY
            if (Input.GetMouseButtonDown(2))
            {
                GameManager.GetInstance().GetNpcManager().SpawnLandNpcAtRandom();
            }
            // DEBUG ONLY
        }
    }

    public void SetGunMode(GunMode mode)
    {
        if (mode == GunMode.UIPointer)
        {
            m_lineRenderer.enabled = true;
        }
        else
        {
            m_lineRenderer.enabled = false;
            m_laserDot.SetActive(false);
        }
        m_mode = mode;
    }
}
