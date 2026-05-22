using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private Button _startGame;
    
    private void Start()
    {
        this._startGame = GetComponentInChildren<Button>();
        this._startGame.onClick.AddListener(this.StartGame);
    }

    private void StartGame()
    {
        SceneManager.LoadScene(Statics.Meta.RunScene);
    }
}
