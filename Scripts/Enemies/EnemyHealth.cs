using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyHealth : HealthSystem
{
    //EnemySpawner spawner;
    public List<Transform> dropItems = new List<Transform>();
    public EnemySpawner spawner;
    public GameManager manager;


    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    public override void TakeDamage(float dmgAmount)
    {
        health -= dmgAmount;

        if (health <= 0)
        {
            RandomSpawnItem();
            Die();
        }
    }

    void RandomSpawnItem()
    {
        Vector2 pos = transform.position;
        Instantiate(dropItems[Random.Range(0, dropItems.Count-1)], pos, Quaternion.identity);
    }

    protected override void Die()
    {
        spawner.Spawn();
        Destroy(gameObject);
        manager.UpdateEnemyInfo();
    }

}
