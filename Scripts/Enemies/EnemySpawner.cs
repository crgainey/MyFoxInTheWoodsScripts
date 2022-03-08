using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] float RespawnTime;
    public GameObject enemyObject;

    public void Spawn()
    {
        StartCoroutine(SetEnemyActive());
    }

    IEnumerator SetEnemyActive()
    {
        yield return new WaitForSeconds(RespawnTime);
        Vector2 pos = transform.position;
        Instantiate(enemyObject, pos, Quaternion.identity);
    }

}
