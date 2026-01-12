using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ITMoscowRun.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Vector2[] _avaibleLinesForPlayer;
        [SerializeField] private Vector3 _directionMove = Vector3.forward;
        [SerializeField] private GameObject _mesh;
        [SerializeField] private float _speed = 5;
        [SerializeField] private float _jumpForce = 50;
        [SerializeField] private float _crouchTime = 1;
        [SerializeField] private float _jumpCoolDown = 0.5f;
        [SerializeField] private float _crochCoolDown = 0.5f;
        [SerializeField] private float _rotationAngle = 30f;

        private CapsuleCollider colider;
        private float lastJumpTime;
        private float lastCrouchTime;
        private float startSize;
        private float crouchSize;
        private float normalOffest;
        private float jumpingOffest;
        private bool jumped = false;
        private bool isGround = true;
        private bool ifAlive = true;
        private Rigidbody rb;
        private Coroutine jumpToLines;
        private Coroutine crouch;
        private Coroutine playerRotate;
        private enum Lines { Left = 0, Middle = 1, Right = 2 };
        private Lines currentLine = Lines.Middle;

        private void Start()
        {
            colider = transform.GetComponent<CapsuleCollider>();
            startSize = colider.height;
            crouchSize = colider.height / 2;
            normalOffest = colider.center.y;
            jumpingOffest = colider.center.y + 0.2f;
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (ifAlive) rb.linearVelocity = new Vector3(_directionMove.x * _speed, rb.linearVelocity.y, _directionMove.z * _speed);
            if (jumped && isGround)
            {
                jumped = false;
                colider.center = new Vector3(0, normalOffest, 0);

            }

            _animator.SetBool("IsGround", !isGround);
        }

        public void OnMove(InputValue action)
        {
            Vector2 input = action.Get<Vector2>();

            CheckLines(input);
        }

        public void CheckLines(Vector2 input)
        {
            if (input.x > 0)
            {
                if (currentLine == Lines.Left)
                {
                    currentLine = Lines.Middle;
                }
                else if (currentLine == Lines.Middle)
                {
                    currentLine = Lines.Right;
                }
                else
                {
                    return;
                }

                if (playerRotate != null)
                {
                    StopCoroutine(playerRotate);
                }

                playerRotate = StartCoroutine(RotateSlowly(-180 + _rotationAngle, 0.2f));

                SwitchLine();
            }
            else if (input.x < 0)
            {
                if (currentLine == Lines.Right)
                {
                    currentLine = Lines.Middle;
                }
                else if (currentLine == Lines.Middle)
                {
                    currentLine = Lines.Left;
                }
                else
                {
                    return;
                }

                if (playerRotate != null)
                {
                    StopCoroutine(playerRotate);
                }

                playerRotate = StartCoroutine(RotateSlowly(-180 - _rotationAngle, 0.2f));

                SwitchLine();
            }

            if (input.y > 0)
            {
                TryJump();
            }
            else if (input.y < 0)
            {
                TryCrouch();
            }
        }

        private void TryJump()
        {
            if (!isGround || Time.time - lastJumpTime < _jumpCoolDown) return;

            transform.GetComponent<CapsuleCollider>().height = startSize;
            colider.center = new Vector3(0, jumpingOffest, 0);
            rb.AddForce(Vector3.up * _jumpForce);
            lastJumpTime = Time.time;
        }

        private void TryCrouch()
        {
            if (Time.time - lastCrouchTime < _crochCoolDown) return;

                if (crouch != null)
            {
                StopCoroutine(crouch);
                crouch = null;
            }

            crouch = StartCoroutine(PlayerCrouch());
            lastCrouchTime = Time.time;
        }

        private void SwitchLine()
        {
            if (jumpToLines != null)
            {
                StopCoroutine(jumpToLines);
            }

            jumpToLines = StartCoroutine(JumpToLine());
        }

        private IEnumerator PlayerCrouch()
        {
            transform.GetComponent<CapsuleCollider>().height = crouchSize;
            rb.AddForce(Vector3.down * _jumpForce);

            yield return new WaitForSeconds(_crouchTime);

            transform.GetComponent<CapsuleCollider>().height = startSize;
            crouch = null;
        }

        private IEnumerator RotateSlowly(float angle, float time)
        {
            float timer = 0;
            Quaternion newRotation = Quaternion.Euler(0, angle, 0);
            Quaternion oldRotation = _mesh.transform.rotation;

            while (timer < time)
            {
                timer += Time.deltaTime;
                float t = timer / time;

                _mesh.transform.rotation = Quaternion.Slerp(oldRotation, newRotation, t);
                yield return null;
            }

            _mesh.transform.rotation = newRotation;
        }

        private IEnumerator JumpToLine()
        {
            float timer = 0;
            Vector3 newPos = new Vector3(_avaibleLinesForPlayer[(int)currentLine].x, transform.position.y, transform.position.z + _speed * 0.3f);
            Vector3 oldPos = transform.position;

            while (timer < 0.3f)
            {
                timer += Time.deltaTime;
                rb.MovePosition(Vector3.Lerp(new Vector3(oldPos.x, transform.position.y, oldPos.z), new Vector3(newPos.x, transform.position.y, newPos.z), timer / 0.3f));
                yield return null;
            }

            if (playerRotate != null)
            {
                StopCoroutine(playerRotate);
            }

            playerRotate = StartCoroutine(RotateSlowly(-180, 0.1f));

            rb.MovePosition(new Vector3(newPos.x, transform.position.y, newPos.z));
            jumpToLines = null;
        }

        private void OnCollisionStay(Collision other)
        {
            isGround = true;
        }

        private void OnCollisionExit(Collision other)
        {
            isGround = false;
        }
    }
}
