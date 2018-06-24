using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable {

    [SerializeField] int enemyLayer = 9;
    public float maxHealthPoints = 100f;
    public float damagePerHit = 10f;
    public float attackSpeed = .5f;
    public float maxAttackRange = 2f;

    GameObject currentTarget;
    public float currentHealthPoints;
    CameraRaycaster cameraRaycaster;
    float lastHitTime = 0f;
    public float timeSinceLastDamaged;
    float regenHealthDelay = 5f;
    float regenHealthspeed = 1f;
    float lastDamaged;

    public float healthAsPercentage
    {
        get
        {
            return currentHealthPoints / maxHealthPoints;
        }
    }

    void Start()
    {
        cameraRaycaster = FindObjectOfType<CameraRaycaster>();
        cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
        currentHealthPoints = maxHealthPoints;
    }

    void Update()
    {
        if ((Time.time - timeSinceLastDamaged) >= regenHealthDelay && currentHealthPoints != maxHealthPoints)
        {
            print("regenerating!");
            StartCoroutine("regenHealth");
        }
        if (currentHealthPoints == maxHealthPoints & (Time.time - timeSinceLastDamaged) <= regenHealthDelay)
        {
            //CancelInvoke();
            StopCoroutine("regenHealth");
            print("Done Regen!");
        }
    }

    IEnumerator regenHealth()
    {
        currentHealthPoints = currentHealthPoints + regenHealthspeed;
        //        regenHealthSpeed = regenHealthSpeed +1 ;
        print("Still Regen!");
        yield return new WaitForSeconds(1);
    }

    void OnMouseClick(RaycastHit raycastHit, int layerHit)
    {
        if (layerHit == enemyLayer)
        {
            var enemy = raycastHit.collider.gameObject;
             
            // Check enemy is in range
            if ((enemy.transform.position - transform.position).magnitude > maxAttackRange)
            {
                return;
            }

            currentTarget = enemy;

            var enemyComponent = enemy.GetComponent<Enemy>();
            if (Time.time - lastHitTime > attackSpeed)
            {
                enemyComponent.TakeDamage(damagePerHit);
                lastHitTime = Time.time;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        timeSinceLastDamaged = Time.time;
    }

    //TODO Uncomment this after fixing exp system.
    public void OnGUI()
    {
        string currentHealth = currentHealthPoints.ToString();
        string maxHealth = maxHealthPoints.ToString();
        //    string lvl = level.ToString();
        //    string exp = experiencePoints.ToString();
        //    string expToLevel = experienceToNextLevel.ToString();
        //    string lastdamaged = lastDamaged.ToString();
        GUI.Label(new Rect(Screen.width / 2, Screen.height - 55, 100, 20), currentHealth + "/" + maxHealth);
    //    GUI.Label(new Rect(Screen.width / 3, Screen.height - 30, 100, 20), lvl);
    //    GUI.Label(new Rect(Screen.width / 2, Screen.height - 30, 100, 20), exp + "/" + expToLevel);
    //    GUI.Label(new Rect(Screen.width - 80, Screen.height - 20, 100, 20), "Last hit " + (Time.time - timeSinceLastDamaged) + "s ago");

    }
}