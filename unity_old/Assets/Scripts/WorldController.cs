using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldController : MonoBehaviour
{
    public static bool IsGameOver { get; private set; }
    public static bool IsPaused { get; private set; }

    private ActionManager _actions;
    private PlayerLeveler _leveler;
    private bool _hasNotified;
    private AudioSource _audio;
    private AudioSource _background;
    private AudioSource _wilhelm;

    private void Start()
    {
        this._audio = this._audio ?? gameObject.GetComponents<AudioSource>()[0];
        this._background = this._background ?? gameObject.GetComponents<AudioSource>()[1];
        this._wilhelm = this._background ?? gameObject.GetComponents<AudioSource>()[2];
        this._actions = this._actions ?? FindAnyObjectByType<ActionManager>();
        this._actions.Subscribe(this.HandleActions);
        this._leveler = this._leveler ?? FindAnyObjectByType<PlayerLeveler>();
        this._hasNotified = false;
    }

    private void HandleActions(string message)
    {
        if (message.StartsWith(Statics.Meta.Increase))
        {
            this.TogglePause();
            return;
        }

        switch (message)
        {
            case Statics.Enemy.Expire:
                this.PlaySound("Pop");
                this.PlayWilhelm();
                break;
            case Statics.Meta.LevelUp:
                this.PlaySound("DingDong");
                this.TogglePause();
                break;
            case Statics.Meta.MainMenuRequest:
                InputController.EnableCursor();
                this.UnsetPause();
                this._hasNotified = false;
                WorldController.IsGameOver = false;
                SceneManager.LoadScene(Statics.Meta.MenuScene);
                break;
            case Statics.Meta.GameOver:
                this._background.Stop();
                this.PlaySound("GameOver");
                InputController.EnableCursor();
                this.SetPause();
                WorldController.IsGameOver = true;
                this.SetPause();
                break;
            case Statics.Meta.TogglePause:
                this.TogglePause();
                break;
            case Statics.Meta.RunSceneLoaded:
                this._background.Stop();
                this._background.PlayOneShot(Resources.Load<AudioClip>($"Sounds/Background"));
                WorldController.IsPaused = false;
                InputController.DisableCursor();
                this.UnsetPause();
                break;
        }
    }

    private void PlaySound(string name)
    {
        this._audio.pitch = Random.Range(0.8f, 1.8f);
        this._audio.PlayOneShot(Resources.Load<AudioClip>($"Sounds/{name}"));
    }

    private void PlayWilhelm()
    {
        if (Random.Range(0, 5) == 1 && this._leveler.Level > 8)
        {
            this._wilhelm.PlayOneShot(Resources.Load<AudioClip>($"Sounds/Wilhelm"));
        }
    }

    private void SetPause()
    {
        Time.timeScale = 0f;
    }

    private void UnsetPause()
    {
        Time.timeScale = 1f;
    }

    private void TogglePause()
    {
        bool isPaused = Time.timeScale == 0f;

        if (isPaused)
        {
            this.UnsetPause();
            WorldController.IsPaused = false;
            InputController.DisableCursor();
        }
        else
        {
            this.SetPause();
            WorldController.IsPaused = true;
            InputController.EnableCursor();
        }
    }

    private void SetRunSceneLoaded()
    {
        if (this._hasNotified)
        {
            return;
        }

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(Statics.Meta.RunScene) && !this._hasNotified)
        {
            this._actions.Notify(Statics.Meta.RunSceneLoaded);
            this._hasNotified = true;
        }
        else
        {
            this._hasNotified = false;
        }
    }

    private void FixedUpdate()
    {
        this.SetRunSceneLoaded();
    }
}
