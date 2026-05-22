using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour
{
    private ActionManager _actions;
    private CharacterController _character;
    private FloorController _floor;
    private PlayerController _player;
    private PlayerLeveler _leveler;
    private Rigidbody _rigidbody;

    public bool IsDead { get; private set; }

    private int _attackCooldown;
    private bool _shouldDestroy;

    private void Start()
    {
        this._character = this._character ?? gameObject.GetComponent<CharacterController>();
        this._floor = this._floor ?? FindAnyObjectByType<FloorController>();
        this._player = this._player ?? FindAnyObjectByType<PlayerController>();
        this._leveler = this._leveler ?? FindAnyObjectByType<PlayerLeveler>();
        this._actions = this._actions ?? FindAnyObjectByType<ActionManager>();
        this._actions.Subscribe(this.HandleActions);

        this.IsDead = false;

        this._attackCooldown = this._leveler.EnemyAttackCooldown;
        this._shouldDestroy = false;
    }

    public void HandleActions(string message)
    {
        switch (message)
        {
            case Statics.Player.Attack: this.HandleAttack(); break;
        }
    }

    public void HandleAttack()
    {
        if (!this._player.IsInAttackRange(gameObject, this.IsDead))
        {
            this._actions.Notify(Statics.Player.Miss);
            return;
        }

        this._actions.Notify(Statics.Player.Hit);

        float distance = Vector3.Distance(this._player.transform.position, transform.position);

        if (distance < Statics.Player.Radius)
        {
            if (!this.IsDead)
            {
                this.IsDead = true;

                this._rigidbody = gameObject.AddComponent<Rigidbody>();
                this._rigidbody.mass = 0.1f;
                this._rigidbody.linearDamping = 0.2f;
                this._rigidbody.angularDamping = 0.05f;
                this._rigidbody.useGravity = true;
                this._rigidbody.isKinematic = false;
            }

            Vector3 direction = (transform.position - this._player.transform.position).normalized;
            direction.y = 0.3f;
            this._rigidbody.AddForce(direction * this._leveler.PushStrength / 10, ForceMode.Impulse);

            Vector3 randomTorque = Random.insideUnitSphere * 5f;
            this._rigidbody.AddTorque(randomTorque, ForceMode.Impulse);
        }
    }

    private void LookAtPlayer()
    {
        Vector3 playerPos = new Vector3(
            this._player.transform.position.x,
            transform.position.y,
            this._player.transform.position.z
        );

        transform.LookAt(playerPos);
    }

    private void Move()
    {
        float distance = Vector3.Distance(this._player.transform.position, transform.position);

        Vector3 enemyToPlayer = this._player.transform.position - transform.position;
        enemyToPlayer.y = 0f;
        enemyToPlayer = enemyToPlayer.normalized * this._leveler.EnemySpeed;

        Vector3 playerToEnemy = transform.position - this._player.transform.position;
        playerToEnemy.y = 0f;

        if (distance > Statics.Player.Radius)
        {
            enemyToPlayer.y = 0f;
            this._character.Move(enemyToPlayer * Time.deltaTime);
        }
    }

    private void Expire()
    {
        this._actions.Notify(Statics.Enemy.Expire);
        this.Destroy();
    }

    public void MarkForDestruction()
    {
        this._shouldDestroy = true;
    }

    private void Destroy()
    {
        this._actions.Unsubscribe(this.HandleActions);
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (WorldController.IsGameOver)
        {
            return;
        }

        if (this._player.IsInRange(this.gameObject) && !this.IsDead)
        {
            if (--this._attackCooldown <= 0)
            {
                this._actions.Notify(Statics.Enemy.Attack);
                this._attackCooldown = this._leveler.EnemyAttackCooldown;
            }

            return;
        }

        this._attackCooldown = (this._leveler.AttackCooldown * 2);
    }

    private void Update()
    {
        if (WorldController.IsGameOver)
        {
            return;
        }

        if (this._shouldDestroy)
        {
            this.Destroy();
            return;
        }

        if (transform.position.y < -1f)
        {
            this.Expire();
            return;
        }

        this._character.enabled = !this.IsDead;

        if (this.IsDead)
        {
            return;
        }

        this.LookAtPlayer();
        this.Move();

        transform.position = this._floor.Clamp(transform.position, Statics.Enemy.Height);
    }
}

