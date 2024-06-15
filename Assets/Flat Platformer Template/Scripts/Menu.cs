using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] InputActionReference inputActionShoot;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    private void OnEnable()
    {
        inputActionShoot.action.started += OnStart;
    }

    private void OnDisable()
    {
        inputActionShoot.action.started -= OnStart;
    }

    private void OnStart(InputAction.CallbackContext context)
    {
        StartGame();
    }
}
