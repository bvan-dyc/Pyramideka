using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {
    public GameObject   enemy;
    public float        timer;
    public float        spawnDelay;

	void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Spawn();
            timer = spawnDelay;
        }
    }

    void Spawn()
    {
        GameObject enemyInstance = Instantiate(enemy, transform.position, transform.rotation);
        enemyInstance.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
