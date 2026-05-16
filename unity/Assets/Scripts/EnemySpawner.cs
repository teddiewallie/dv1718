using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private GameObject _enemy;

    private PlayerController _player;
    private PlayerLeveler _leveler;
    private FloorController _floor;
    private ActionManager _actions;

    private float _nextSpawnTime;

    private void Awake()
    {
        this._player = this._player ?? FindAnyObjectByType<PlayerController>();
        this._leveler = this._leveler ?? FindAnyObjectByType<PlayerLeveler>();
        this._floor = this._floor ?? FindAnyObjectByType<FloorController>();
        this._actions = this._actions ?? FindAnyObjectByType<ActionManager>();
        this._actions.Subscribe(this.HandleActions);

        this._enemy = Resources.Load<GameObject>("Enemy");
    }

    private void HandleActions(string message)
    {
        switch (message)
        {
            case Statics.Meta.LevelUp: this.NukeAllEnemies(); break;
        }
    }

    private void NukeAllEnemies()
    {
        foreach (EnemyController enemy in FindObjectsByType<EnemyController>())
        {
            enemy.MarkForDestruction();
        }
    }

    private void Update()
    {
        if (WorldController.IsGameOver)
        {
            return;
        }

        if (Time.time > this._nextSpawnTime)
        {
            SpawnEnemy();
            this._nextSpawnTime = Time.time + this._leveler.SpawnDelay;
        }
    }

    private void SpawnEnemy()
    {
        for (int i = 0; i < this._leveler.GetSpawnAmount(); i++)
        {
            GameObject gameObject = Instantiate(this._enemy, this._floor.RandomPos(), this._player.transform.rotation);
        }
    }
}

