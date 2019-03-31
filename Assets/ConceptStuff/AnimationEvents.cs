using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Weapons;
using UMA;
using UMA.CharacterSystem;

public class AnimationEvents : MonoBehaviour {

    [SerializeField] DynamicCharacterAvatar _player;
    [SerializeField] Character player;

    public void CastProjectile()
    {
        Transform leftHand = _player.umaData.skeleton.GetBoneGameObject(UMASkeleton.StringToHash("LeftHand")).transform;
        Vector3 aimoffset = new Vector3(0, 1f, 0);

        float damage = Mathf.Round(UnityEngine.Random.Range(player.GetMinDamage(), player.GetMaxDamage()));
        GameObject _castProjectile = Instantiate(player.GetCastProjectile(), leftHand.position, Quaternion.identity);
        Projectile projectileComponent = _castProjectile.GetComponent<Projectile>();
        projectileComponent.SetDamage(damage);
        projectileComponent.SetShooter(gameObject);

        Vector3 unitVectorToTarget = (player.GetTarget().transform.position + aimoffset - leftHand.position).normalized;
        float projectileSpeed = projectileComponent.projectileSpeed;
        _castProjectile.GetComponent<Rigidbody>().velocity = unitVectorToTarget * projectileSpeed;
        projectileComponent.PlayCastSound();
    }
}
