using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CosmeticType
{
    CastCosmetic,
}

[CreateAssetMenu]
public class CosmeticItem : Item {

    public CosmeticType CosmeticType;
    [Space]
    [SerializeField] GameObject cosmeticPrefab;
    [SerializeField] AudioClip castSound;
    [SerializeField] AudioClip hitSound;

    public GameObject GetCosmeticPrefab()
    {
        return cosmeticPrefab;
    }

    public AudioClip GetCastSound()
    {
        return castSound;
    }

    public AudioClip GetHitSound()
    {
        return hitSound;
    }

}
