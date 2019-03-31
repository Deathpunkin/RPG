using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Weapons
{
    public class Projectile : MonoBehaviour
    {

        public float projectileSpeed; // Note other classes can set

        [SerializeField] GameObject shooter; //for debug inspection
        [SerializeField] AudioClip castSound;
        [SerializeField] AudioClip hitSound;
        [SerializeField] AudioSource audioSource;
        float damageCaused;
        float DESTROYDELAY;

        private void Awake()
        {
           if (!audioSource)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        public void SetShooter(GameObject shooter)
        {
            this.shooter = shooter;
        }
        public void SetDamage(float damage)
        {
            damageCaused = damage;
        }

        public void PlayCastSound()
        {
            audioSource.clip = castSound;
            audioSource.Play();
        }

        void PlayHitSound()
        {
            audioSource.clip = hitSound;
            audioSource.Play();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (shooter && collision.gameObject.layer != shooter.layer)
            {
                Component damagableComponent = collision.gameObject.GetComponent(typeof(IDamageable));
                if (damagableComponent)
                {
                    (damagableComponent as IDamageable).TakeDamage(damageCaused);
                    DamageTextController.CreateFloatingDamageText(damageCaused.ToString(), collision.gameObject.transform);
                    PlayHitSound();
                    print(damagableComponent + " took " + damageCaused + " damage");
                }
            }

            if (!hitSound)
            {
                Destroy(gameObject, 0.3f);
            }
            else
            {
                Destroy(gameObject, hitSound.length);
            }
        }
    }
}