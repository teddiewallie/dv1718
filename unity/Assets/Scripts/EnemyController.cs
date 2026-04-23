using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour
{
    private CharacterController character;
    private FloorController floor;
    private PlayerController player;

    private void Start()
    {
        character = character ?? gameObject.GetComponent<CharacterController>();
        floor = floor ?? gameObject.GetComponent<FloorController>();
        player = player ?? FindAnyObjectByType<PlayerController>();
    }

    private void Update()
    {
        Vector3 playerPos = new Vector3(
            player.transform.position.x,
            transform.position.y,
            player.transform.position.z
        );

        transform.LookAt(playerPos);

        Vector3 enemyToPlayer = player.transform.position - transform.position;
        enemyToPlayer.y = 0f;
        enemyToPlayer = enemyToPlayer.normalized * Statics.EnemySpeed;

        Vector3 playerToEnemy = transform.position - player.transform.position;
        playerToEnemy.y = 0f;

        float playerRadius = 3f;
        float pushRadius = 3f;
        float pushStrength = 15f;

        if (playerToEnemy.magnitude < pushRadius)
        {
            Vector3 push = playerToEnemy.normalized * pushStrength;
            enemyToPlayer += push;
        }

        if (enemyToPlayer.magnitude > playerRadius)
        {
            character.Move(enemyToPlayer * Time.deltaTime);
        }
        

        transform.position = floor.Clamp(transform.position);
    }
}

