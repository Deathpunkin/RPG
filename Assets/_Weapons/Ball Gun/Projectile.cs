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
        float damageCaused;
        const float DESTROYDELAY = 0.01f;

        public void SetShooter(GameObject shooter)
        {
            this.shooter = shooter;
        }
        public void SetDamage(float damage)
        {
            damageCaused = damage;
        }



        void OnCollisionEnter(Collision collision)
        {
            if (shooter && collision.gameObject.layer != shooter.layer)
            {
                Component damagableComponent = collision.gameObject.GetComponent(typeof(IDamageable));
                if (damagableComponent)
                {
                    (damagableComponent as IDamageable).TakeDamage(damageCaused);
                    print(damagableComponent + " took " + damageCaused + " damage");
                }
            }

            Destroy(gameObject, DESTROYDELAY);
        }
    }
}