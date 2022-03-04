using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //public List<Transform> enemies = new List<Transform>();
    //public int enemiesSpawned;

    [SerializeField] float RespawnTime;
    public GameObject enemyObject;

    public void DoTHING()
    {
        StartCoroutine(SetEnemyActive());
    }

    IEnumerator SetEnemyActive()
    {
        //Debug.Log("SetEnemyActive");
        yield return new WaitForSeconds(RespawnTime);
        //enemyObject.SetActive(true);
        //enemyObject.GetComponent<AgressiveEnemeyAI>().Patrol();
        //enemyObject.GetComponent<EnemyPathMovementt>().ReStartMain();
        Vector2 pos = transform.position;
        Instantiate(enemyObject, pos, Quaternion.identity);
    }

    //public void SpawnEnemy()
    //{
    //    RandomEnemySpawn();
    //}
  
    //public void RandomEnemySpawn()
    //{
    //    Vector2 pos = transform.position;
        
    //    Instantiate(enemies[Random.Range(0, enemies.Count)], pos, Quaternion.identity);
    //   // enemiesSpawned++;
    //}
    
}
