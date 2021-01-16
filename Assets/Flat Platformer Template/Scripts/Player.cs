using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour {
    public float WalkSpeed;
    public float JumpForce;
    public Transform _Blade, _GroundCast;
    public Camera cam;
    public bool mirror;
    public Text _scoreTxt = null;
    public GameObject _WinPanel = null;
    public AudioManager _AudioManager = null;

    private bool _canJump, _canWalk;
    private bool _isWalk, _isJump;
    private float rot, _startScale;
    private Rigidbody2D rig;
    private Vector2 _inputAxis;
    private RaycastHit2D _hit;
    private Animator _animator;
    private Vector3 StartPos;
    private int _score;
    private bool _isDead;
    private bool _afterJump = false;
    private string prevPointName = string.Empty;

    void Start ()
    {
        _afterJump = false;
        _WinPanel.SetActive(false);
        _isDead = false;
        _score = -1;
        SetScore();
         StartPos = transform.position;
        rig = gameObject.GetComponent<Rigidbody2D>();
        _animator = gameObject.GetComponent<Animator>();
        _startScale = transform.localScale.x;
	}

    private void SetScore()
    {
        _score++;
        _scoreTxt.text = _score.ToString();
        Debug.Log($"scorre: {_score}");
    }

    void Update()
    {
        if (_isDead)
        {
            return;
        }
        if (_hit = Physics2D.Linecast(new Vector2(_GroundCast.position.x, _GroundCast.position.y + 0.2f), _GroundCast.position))
        {
            if (_hit.transform.CompareTag("Ground"))
            {
                if (_afterJump)
                {
                    Debug.Log("Hit GROUND AFTER JUMP");
                    _AudioManager.PlayStep(true);
                    _afterJump = false;
                }
                _canJump = true;
                _canWalk = true;
            }
        }
        else _canJump = false;

        _inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (_inputAxis.y > 0 && _canJump)
        {
            _canWalk = false;
            _isJump = true;
        }
    }

    void FixedUpdate()
    {
        if (_isDead)
        {
            float step = 2 * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, -41f, transform.position.z), step);
            return;
        }
        Vector3 dir = _inputAxis;
        dir.Normalize();

        if (dir.x > 0)
            mirror = false;
        if (dir.x < 0)
            mirror = true;

        if (!mirror)
        {
            rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.localScale = new Vector3(_startScale, _startScale, 1);
            //_Blade.transform.rotation = Quaternion.AngleAxis(rot, Vector3.forward);
        }
        if (mirror)
        {
            rot = Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg;
            transform.localScale = new Vector3(-_startScale, _startScale, 1);
            //_Blade.transform.rotation = Quaternion.AngleAxis(rot, Vector3.forward);
        }
        if (_inputAxis.x != 0)
        {
            rig.velocity = new Vector2(_inputAxis.x * WalkSpeed * Time.deltaTime, rig.velocity.y);

            if (_canWalk)
            {
                if (!_isWalk)
                {
                    _AudioManager.PlayStep(true);
                    _animator.Play("Walk");
                    _isWalk = true;
                }
            }
        }

        else
        {
            _AudioManager.PlayStep(false);
            rig.velocity = new Vector2(0, rig.velocity.y);
            _animator.Play("Idle");
            _isWalk = false;
        }

        if (_isJump)
        {
            if (_canJump)
            {
                _AudioManager.PlayJump();
                _afterJump = true;
            }
            rig.AddForce(new Vector2(0, JumpForce));
            _animator.Play("Jump");
            _canJump = false;
            _isJump = false;
        }
    }

    public bool IsMirror()
    {
        return mirror;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, _GroundCast.position);
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
        _isWalk = false;
        _isJump = false;
        rig.velocity = Vector2.zero;
        _isDead = true;
        _WinPanel.SetActive(true);
    }

    IEnumerator DeadCoroutine()
    {
        Debug.Log("DEAD !");
        _AudioManager.PlayDead();
        _isDead = true;
        _animator.StopPlayback();
        _animator.Play("Dead");
        yield return new WaitForSeconds(2.5f);
        transform.position = StartPos;
        _isWalk = false;
        _isJump = false;
        _isDead = false;
    }

    public void OnWinClick()
    {
        SceneManager.LoadScene(0);
    }

}
