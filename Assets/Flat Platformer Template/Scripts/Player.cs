using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [SerializeField] InputActionReference inputActionMove;
    [SerializeField] InputActionReference inputActionShoot;
    [SerializeField] InputActionReference inputActionJump;
    [SerializeField] InputActionReference inputActionQuit;
    [SerializeField] CharacterController2D controller2D;
    [SerializeField] GameObject FirstWinButton;
    [SerializeField] float runSpeed = 40f;
    public Camera cam;
    public bool mirror;
    public Text _scoreTxt = null;
    public GameObject _WinPanel = null;
    public AudioManager _AudioManager = null;
    public GameObject _singObject = null;
    public GameObject _noteObject = null;

    private bool _canShoot;
    private Rigidbody2D rig;
    private float _inputAxis;
    private Animator _animator;
    private Vector3 StartPos;
    private int _score;
    private bool _isDead;
    private string prevPointName = string.Empty;
    private bool jump = false;
    void Start()
    {
        _noteObject.SetActive(false);
        _WinPanel.SetActive(false);
        _isDead = false;
        _canShoot = true;
        _score = -1;
        SetScore();
        StartPos = transform.position;
        rig = gameObject.GetComponent<Rigidbody2D>();
        _animator = gameObject.GetComponent<Animator>();
    }

    private void SetScore()
    {
        _score++;
        _scoreTxt.text = _score.ToString();
        Debug.Log($"scorre: {_score}");
    }

    private void OnEnable()
    {
        inputActionShoot.action.started += ShootKey;
        inputActionJump.action.started += JumpKey;
        inputActionQuit.action.started += QuitKey;
    }

    private void OnDisable()
    {
        inputActionShoot.action.started -= ShootKey;
        inputActionJump.action.started -= JumpKey;
        inputActionQuit.action.started -= QuitKey;
    }

    private void QuitKey(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(0);
    }

    private void JumpKey(InputAction.CallbackContext context)
    {
        jump = true;
    }

    private void ShootKey(InputAction.CallbackContext context)
    {
        if (_canShoot)
        {
            StartCoroutine(ShootTimer());
        }
    }

    void Update()
    {
        if (_isDead)
        {
            return;
        }
        _inputAxis = inputActionMove.action.ReadValue<Vector2>().x * runSpeed;
    }

    IEnumerator ShootTimer()
    {
        _AudioManager.PlaySong();
        _canShoot = false;
        _singObject.SetActive(false);
        _noteObject.SetActive(true);
        Vector2 face = mirror ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + 3, transform.position.y), face, 5f);
        //EnemyLayer is the layer having enemys
        if (hit)
        {
            print(hit.collider.name + "-" + hit.collider.tag);
            var enem = hit.collider.transform.root.GetComponent<Enemy>();
            if (enem != null)
            {
                enem.Sleep();
            }
        }
        yield return new WaitForSeconds(1);
        _noteObject.SetActive(false);
        yield return new WaitForSeconds(4);
        _canShoot = true;
        _singObject.SetActive(true);
    }

    void FixedUpdate()
    {
        if (_isDead)
        {
            float step = 2 * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, -41f, transform.position.z), step);
            return;
        }
        if (_inputAxis != 0)
        {
            _AudioManager.PlayStep(true);
            _animator.Play("Walk");
        }
        else
        {
            _AudioManager.PlayStep(false);
            _animator.Play("Idle");
        }
        controller2D.Move(_inputAxis * Time.deltaTime, false, jump, () => 
        {
            _AudioManager.PlayJump();
            _animator.Play("Jump");
        });
        jump = false;
    }

    public bool IsMirror()
    {
        return mirror;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isDead)
        {
            return;
        }
        if (collision.transform.CompareTag("Enemy"))
        {
            StartCoroutine(DeadCoroutine());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isDead)
        {
            return;
        }
        if (collision.transform.CompareTag("Point"))
        {
            if (collision.transform.name == prevPointName)
            {
                return;
            }
            Debug.Log("SCORE !");
            prevPointName = collision.transform.name;
            _AudioManager.PlayScore();
            SetScore();
            Destroy(collision.gameObject);
        }
        if (_score > 15)
        {
            _AudioManager.PlayWin();
            Win();
        }
    }

    private void Win()
    {
        _animator.Play("Idle");
        _AudioManager.PlayStep(false);
        rig.velocity = Vector2.zero;
        _isDead = true;
        TunrOffColliders();
        _WinPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(FirstWinButton);
    }

    IEnumerator DeadCoroutine()
    {
        Debug.Log("DEAD !");
        _AudioManager.PlayDead();
        _isDead = true;
        rig.velocity = Vector2.zero;
        TunrOffColliders();
        _animator.StopPlayback();
        _animator.Play("Dead");
        yield return new WaitForSeconds(2.5f);
        transform.position = StartPos;
        TunrOnColliders();
        _isDead = false;
    }

    public void OnWinClick()
    {
        SceneManager.LoadScene(0);
    }

    private void TunrOnColliders()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        SetLayerRecursively(gameObject, playerLayer);
    }
    
    private void TunrOffColliders()
    {
        int deadLayer = LayerMask.NameToLayer("Dead");
        SetLayerRecursively(gameObject, deadLayer);
    }
    
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

}
