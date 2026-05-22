using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasController : MonoBehaviour
{
    private ActionManager _actions;
    private PlayerController _player;
    private PlayerLeveler _leveler;

    private Button _exit;
    private Slider _attackCooldown;
    private Slider _health;
    private Slider _xp;
    private TMP_Text _level;
    private TMP_Text _status;
    private TMP_Text _killed;

    private Button[] _perkButtons;

    private void Start()
    {
        this._perkButtons = new Button[3];
        this._perkButtons[0] = this._perkButtons[0] ?? this.FindChildByName<Button>("Perk1");
        this._perkButtons[1] = this._perkButtons[1] ?? this.FindChildByName<Button>("Perk2");
        this._perkButtons[2] = this._perkButtons[2] ?? this.FindChildByName<Button>("Perk3");

        this._exit = this._exit ?? this.FindChildByName<Button>("Exit");
        this._attackCooldown = this._attackCooldown ?? this.FindChildByName<Slider>("AttackCooldown");
        this._health = this._health ?? this.FindChildByName<Slider>("Health");
        this._xp = this._xp ?? this.FindChildByName<Slider>("XP");
        this._level = this._level ?? this.FindChildByName<TMP_Text>("Level");

        this._status = this._status ?? this.FindChildByName<TMP_Text>("Status");
        this._killed = this._killed ?? this.FindChildByName<TMP_Text>("Killed");

        this._player = this._player ?? FindAnyObjectByType<PlayerController>();
        this._leveler = this._leveler ?? FindAnyObjectByType<PlayerLeveler>();

        this._actions = this._actions ?? FindAnyObjectByType<ActionManager>();
        this._actions.Subscribe(this.HandleActions);
    }

    private T FindChildByName<T>(string name)
    {
        foreach (Transform child in transform.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name)
            {
                return child.GetComponent<T>();
            }
        }

        return default(T);
    }

    private void HandleActions(string message)
    {
        switch (message)
        {
            case Statics.Meta.LevelUp:
                this.ToggleLevelUp();
                break;
            case Statics.Meta.TogglePause:
                this.ToggleStatus("PAUSE");
                this._exit.onClick.AddListener(this.TogglePause);
                break;
            case Statics.Meta.GameOver:
                this._health.value = 0f;
                this.ToggleStatus("GAME OVER");
                this._exit.onClick.AddListener(this.ToMainMenu);
                break;
            default:
                break;
        }
    }

    private void TogglePause()
    {
        this._actions.Notify(Statics.Meta.TogglePause);
    }

    private void ToMainMenu()
    {
        this.ToggleStatus("GAME OVER");
        this._actions.Notify(Statics.Meta.MainMenuRequest);
    }

    private void ToggleLevelUp()
    {
        this._status.text = "LEVEL UP";
        this._status.gameObject.SetActive(!this._status.gameObject.activeSelf);
        string[] perks = this._leveler.GetThreePerks();

        UnityEngine.Events.UnityAction[] listeners = new UnityEngine.Events.UnityAction[3];

        for (int i = 0; i < perks.Length; i++)
        {
            string perk = perks[i];
            listeners[i] = () => this.Increase(perk, listeners);

            this._perkButtons[i].GetComponentInChildren<TMP_Text>().text = perk;
            this._perkButtons[i].onClick.AddListener(listeners[i]);
            this._perkButtons[i].gameObject.SetActive(true);
        }
    }

    private void Increase(string message, UnityEngine.Events.UnityAction[] listeners)
    {
        for (int i = 0; i < listeners.Length; i++)
        {
            this._perkButtons[i].onClick.RemoveListener(listeners[i]);
            this._perkButtons[i].gameObject.SetActive(false);
        }

        this._status.gameObject.SetActive(!this._status.gameObject.activeSelf);
        this._actions.Notify($"{Statics.Meta.Increase} {message}");
    }

    private void ToggleStatus(string label)
    {
        this._status.gameObject.SetActive(!this._status.gameObject.activeSelf);
        this._killed.gameObject.SetActive(!this._killed.gameObject.activeSelf);
        this._exit.gameObject.SetActive(!this._exit.gameObject.activeSelf);

        this._status.text = label;
    }

    private void FixedUpdate()
    {
        this._level.text = $"{this._leveler.Level}";
        this._killed.text= $"{this._leveler.XP} pushed off";
        this._xp.value = this._leveler.CalculateLevelProgress();
        this._attackCooldown.value = (float)this._player.AttackCooldown / this._leveler.AttackCooldown;
        this._health.value = (float)this._player.CurrentHealth / this._leveler.MaxHealth;
    }
}
