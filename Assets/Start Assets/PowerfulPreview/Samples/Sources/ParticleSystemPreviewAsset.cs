using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemPreviewAsset : ScriptableObject
{
    public GameObject ParticleSystemPrefab
    {
        get
        {
            return m_ParticleSystemPrefab;
        }
    }
    public float Duration
    {
        get
        {
            return m_Duration;
        }
    }

    [SerializeField]
    private GameObject m_ParticleSystemPrefab;
    [SerializeField]
    private float m_Duration;
}
