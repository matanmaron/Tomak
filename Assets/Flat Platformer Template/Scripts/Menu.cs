using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] InputActionReference inputActionStart;
    [SerializeField] InputActionReference inputActionQuit;

    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        inputActionStart.action.started += OnStart;
        inputActionQuit.action.started += OnQuit;
    }

    private void OnDisable()
    {
        inputActionStart.action.started -= OnStart;
        inputActionQuit.action.started -= OnQuit;
    }

    private void OnStart(InputAction.CallbackContext context)
    {
        StartGame();
    }

    private void OnQuit(InputAction.CallbackContext context)
    {
        QuitGame();
    }
}
