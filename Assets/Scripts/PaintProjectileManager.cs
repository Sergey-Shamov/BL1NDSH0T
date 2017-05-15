using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintProjectileManager : MonoBehaviour
{
    public ParticleSystem cloudParticlePrefab;
    public ParticleSystem burstParticlePrefab;
    public GameObject paintBombPrefab;
    public Color paintBombColor = Color.black;
    public Texture2D[] projectileSplashTextures;

    private List<float[,]> m_projSplashTextures;
    private int m_projSplashTexturesCount;

    void Start()
    {
        // Strip unused color information
        // Later we may need to clip a rectangle from the texture, so we'll store it as two-dimensional array for convenience
        m_projSplashTexturesCount = projectileSplashTextures.Length;
        m_projSplashTextures = new List<float[,]>(m_projSplashTexturesCount);
        for (int i = 0; i < m_projSplashTexturesCount; ++i)
        {
            Texture2D texture = projectileSplashTextures[i];
            int textureWidth = texture.width;
            int textureHeight = texture.height;
            Color[] currTexture = texture.GetPixels();
            float[,] textureAlphas = new float[textureWidth, textureHeight];
            int counter = 0;
            for (int x = 0; x < textureWidth; ++x)
            {
                for (int y = 0; y < textureHeight; ++y)
                {
                    textureAlphas[x, y] = currTexture[counter].a;
                    counter++;
                }
            }
            m_projSplashTextures.Add(textureAlphas);
        }
    }

    public float[,] GetRandomProjectileSplash()
    {
        return m_projSplashTextures[Random.Range(0, m_projSplashTexturesCount)];
    }

    public Vector3 GetRandomSphereRay()
    {
        // it is possible we will want to use predefined set of vectors here
        return Random.onUnitSphere;
    }
}
