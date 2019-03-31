using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] float spawnTime = 3;
    [SerializeField] Enemy[] enemiesToSpawn;
    [SerializeField] Enemy[] enemy;
    [SerializeField] GameObject[] spawnPoints;
    [SerializeField] float[] timeOfDeath;
    [SerializeField] bool[] isDead;
    [SerializeField] float[] timer;

	// Use this for initialization
	void Start () {
        timeOfDeath = new float[enemiesToSpawn.Length];
        isDead = new bool[enemiesToSpawn.Length];
        timer = new float[enemiesToSpawn.Length];
        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            SpawnEnemies(i);
        }
    }

    // Update is called once per frame
    void Update () {
        enemy = GetComponentsInChildren<Enemy>();

        for (int i = 0; i < enemy.Length; i++)
        {
            CheckIfDead(i);
        }
        for (int i = 0; i < isDead.Length; i++)
        {
            if(isDead[i])
            {
                timer[i] += Time.deltaTime;
                if(timer[i] >= spawnTime)
                {
                    SpawnEnemies(i);
                    timer[i] = 0;
                    isDead[i] = false;
                }
            }
        }
    }

    void CheckIfDead(int i)
    {
        if (enemy[i].IsDead())
        {
            if (isDead[i])
            {
                return;
            }
            else
            {
                isDead[i] = true;
            }
        }
    }

    void SpawnEnemies(int i)
    {
        Instantiate(enemiesToSpawn[i], spawnPoints[i].transform.position, spawnPoints[i].transform.rotation, this.transform);
    }

}
