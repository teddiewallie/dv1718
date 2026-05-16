using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private CharacterController _character;
    private InputController _input;
    private ActionManager _actions;
    private GameObject _attackEffect;
    private PlayerLeveler _leveler;
    private AudioSource _audio;

    private float _yVelocity;

    private int _currentHealth;
    public int CurrentHealth => this._currentHealth;

    private int _attackCooldown;
    public int AttackCooldown => this._attackCooldown;

    private FloorController _floor;

    private void Start()
    {
        this._character = this._character ?? gameObject.GetComponent<CharacterController>();
        this._input = this._input ?? FindAnyObjectByType<InputController>();
        this._floor = this._floor ?? FindAnyObjectByType<FloorController>();
        this._leveler = this._leveler ?? gameObject.GetComponent<PlayerLeveler>();
        this._audio = this._audio ?? gameObject.GetComponent<AudioSource>();
        this._actions = this._actions ?? FindAnyObjectByType<ActionManager>();
        this._actions.Subscribe(this.HandleActions);

        this._attackEffect = Resources.Load<GameObject>("PlayerAttackEffect");

        this._currentHealth = this._leveler.MaxHealth;
        this._attackCooldown = 0;

        this._input.Player.Enable();
    }

    private void HandleActions(string message)
    {
        switch (message)
        {
            case Statics.Enemy.Attack: this.HandleEnemyAttack(); break; 
            case Statics.Player.Attack: this._attackCooldown = this._leveler.AttackCooldown; break;
            case Statics.Player.Hit: this.PlaySound("Punch"); break;
            default: break;
        }
    }

    private void PlaySound(string name)
    {
        this._audio.pitch = Random.Range(0.8f, 1.8f);
        this._audio.PlayOneShot(Resources.Load<AudioClip>($"Sounds/{name}"));
    }

    private void HandleEnemyAttack()
    {
        if (this._currentHealth == 0)
        {
            return;
        }

        this.PlaySound("Grunt");

        if (--this._currentHealth <= 0)
        {
            this._actions.Notify(Statics.Meta.GameOver);
        }
    }

    public bool IsInRange(GameObject thing)
    {
        float distance = Vector3.Distance(transform.position, thing.transform.position);
        return distance < this._leveler.PushRadius;
    }

    public bool IsInAttackRange(GameObject thing, bool isDead)
    {
        float distance = Vector3.Distance(transform.position, thing.transform.position);
        float angle = Vector3.Angle(transform.forward, (thing.transform.position - transform.position).normalized);
        float ourAngle = isDead ? 180f : this._leveler.PushAngle;

        return distance < this._leveler.PushRadius && angle <= ourAngle;
    }

    private float HandleJump()
    {
        if (this._character.isGrounded && this._yVelocity < 0)
        {
            this._yVelocity = -2f;

            if (this._input.Jump.WasPressedThisFrame())
            {
                this._yVelocity = Statics.JumpForce;
            }
        }

        this._yVelocity += Statics.Gravity * Time.deltaTime * 2;

        return this._yVelocity;
    }

    private Vector3 HandleMoving()
    {
        transform.Rotate(Vector3.up * this._input.MouseX);

        Vector2 move2 = this._input.MoveButtons.ReadValue<Vector2>();
        Vector3 move3 = new Vector3(move2.x, 0, move2.y);

        if (move3.magnitude > 1f)
        {
            move3 = move3.normalized;
        }

        move3 = transform.TransformDirection(move3);
        move3 *= this._leveler.Speed;
        move3.y = this.HandleJump();

        return move3 * Time.deltaTime;
    }

    private void HandleCooldowns()
    {
        this._attackCooldown = this._attackCooldown == 0 ? 0 : --this._attackCooldown;
    }

    private void FixedUpdate()
    {
        this.HandleCooldowns();

    }

    private void Update()
    {
        if (WorldController.IsGameOver)
        {
            return;
        }

        this._character.Move(this.HandleMoving());
        transform.position = this._floor.Clamp(transform.position, Statics.Player.Height);

        if (this._input.Attack.triggered && this._attackCooldown == 0 && !WorldController.IsPaused)
        {

            this._actions.Notify(Statics.Player.Attack);
        }
    }
}

