using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] InputActionReference inputActionStart;
    [SerializeField] InputActionReference inputActionInfo;
    [SerializeField] InputActionReference inputActionQuit;
    [SerializeField] private GameObject InfoScreen;
        
    private void StartGame()
    {
        if (CheckInfo())
        {
            return;
        }
        SceneManager.LoadScene(1);
    }

    private void QuitGame()
    {
        if (CheckInfo())
        {
            return;
        }
        Application.Quit();
    }

    private void ShowInfo()
    {
        InfoScreen.SetActive(!InfoScreen.activeSelf);
    }

    private bool CheckInfo()
    {
        return InfoScreen.activeSelf;
    }
    
    private void OnEnable()
    {
        inputActionStart.action.started += OnStart;
        inputActionInfo.action.started += OnInfo;
        inputActionQuit.action.started += OnQuit;
    }

    private void OnDisable()
    {
        inputActionStart.action.started -= OnStart;
        inputActionInfo.action.started -= OnInfo;
        inputActionQuit.action.started -= OnQuit;
    }

    private void OnStart(InputAction.CallbackContext context)
    {
        StartGame();
    }

    private void OnInfo(InputAction.CallbackContext context)
    {
        ShowInfo();
    }

    private void OnQuit(InputAction.CallbackContext context)
    {
        QuitGame();
    }
}
