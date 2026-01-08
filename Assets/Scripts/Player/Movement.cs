using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ITMoscowRun.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private Vector2[] _avaibleLinesForPlayer;
        [SerializeField] private Vector3 _directionMove = Vector3.forward;
        [SerializeField] private float _speed = 5;
        [SerializeField] private float _jumpForce = 50;
        [SerializeField] private float _crouchTime = 1;

        private Vector3 startSize;
        private Vector3 crouchSize;
        private bool isGround = true;
        private bool ifAlive = true;
        private Rigidbody rb;
        private Coroutine jumpToLines;
        private Coroutine crouch;
        private enum Lines { Left = 0, Middle = 1, Right = 2 };
        private Lines currentLine = Lines.Middle;

        private void Start()
        {
            startSize = transform.localScale;
            crouchSize = new Vector3(startSize.x, startSize.y / 2f, startSize.z);
             rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (ifAlive) rb.linearVelocity = new Vector3(_directionMove.x * _speed, rb.linearVelocity.y, _directionMove.z * _speed);
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
            if (!isGround) return;

            transform.localScale = startSize;
            rb.AddForce(Vector3.up * _jumpForce);
        }

        private void TryCrouch()
        {
            if (crouch != null)
            {
                StopCoroutine(crouch);
                crouch = null;
            }

            crouch = StartCoroutine(PlayerCrouch());
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
            transform.localScale = crouchSize;
            rb.AddForce(Vector3.down * _jumpForce);

            yield return new WaitForSeconds(_crouchTime);

            transform.localScale = startSize;
            crouch = null;
        }

        private IEnumerator JumpToLine()
        {
            float timer = 0;
            Vector3 newPos = new Vector3(_avaibleLinesForPlayer[(int)currentLine].x, transform.position.y, transform.position.z + _speed * 0.3f);
            Vector3 oldPos = transform.position;

            while (timer < 0.3f)
            {
                timer += Time.deltaTime;
                rb.MovePosition(Vector3.Lerp(oldPos, newPos, timer / 0.3f));
                yield return null;
            }

            rb.MovePosition(newPos);
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
