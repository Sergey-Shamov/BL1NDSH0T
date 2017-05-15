using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintProjectileBehavior : MonoBehaviour
{
    public Color paintColor = Color.white;
    public float paintDiameter = 1.5f;
    public int damage = 40;
    public bool isInvisible;
    
    private bool isActive = false;

    private void Start()
    {
        if (paintDiameter > 0)
        {
            if (!isInvisible)
                GetComponent<Renderer>().material.color = paintColor;
            isActive = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive)
            return;

        Destroy(gameObject);
        ParticleSystem cloudParticle = Instantiate(GameManager.GetInstance().GetProjectileManager().cloudParticlePrefab);
        ParticleSystem burstParticle = Instantiate(GameManager.GetInstance().GetProjectileManager().burstParticlePrefab);
        cloudParticle.transform.position = transform.position;
        burstParticle.transform.position = transform.position;
        var cloudSettings = cloudParticle.main;
        cloudSettings.startColor = paintColor;
        var burstSettings = burstParticle.main;
        burstSettings.startColor = paintColor;
        cloudParticle.Play();
        burstParticle.Play();

        PaintProjectileManager manager = GameManager.GetInstance().GetProjectileManager();

        for (int i = 0; i < 14; ++i)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, manager.GetRandomSphereRay()/*GetSphereRay(i)*/, out hit, paintDiameter))
            {
                if (hit.collider is MeshCollider)
                {
                    MyShaderBehavior script = hit.collider.gameObject.GetComponent<MyShaderBehavior>();
                    if (null != script)
                    {
                        script.PaintOnColored(hit.textureCoord2, manager.GetRandomProjectileSplash(), paintColor);
                    }
                }
            }
        }

        INpcBehavior npcScript = other.GetComponent<INpcBehavior>();
        if (null != npcScript)
        {
            npcScript.DealDamage(damage);
        }
    }
}
