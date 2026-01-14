using UnityEngine;

namespace ITMoscowRun.Scripts.Levels.NPC
{
    public class NPCMovement : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;
        [SerializeField] private Vector3 _direction = Vector3.forward;
        [SerializeField] private float _cubeSize = 1f;
        [SerializeField] private float _jumpPower = 5f;

        private Rigidbody rb;
        private Collider lastJumpColider;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            _direction.Normalize();
        }

        private void FixedUpdate()
        {
            Vector3 movement = _direction * (_speed * Time.fixedDeltaTime);
            rb.MovePosition(rb.position + movement);

            CheckCubeAhead();
        }

        private void CheckCubeAhead()
        {
            Vector3 cubeCenter = transform.position + _direction * (_cubeSize * 0.5f);
            Collider[] hits = Physics.OverlapBox(cubeCenter, Vector3.one * (_cubeSize * 0.5f), Quaternion.LookRotation(_direction));

            foreach (Collider hit in hits)
            {
                if (hit.gameObject.layer == 6)
                {
                    if (hit.transform.position.y < transform.position.y)
                    {
                        if (hit != lastJumpColider)
                        {
                            Vector3 jumpVelocity = rb.linearVelocity;
                            jumpVelocity.y = _jumpPower;
                            rb.linearVelocity = jumpVelocity;

                            lastJumpColider = hit;
                            return;
                        }
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 6)
            {
                if (collision.collider == lastJumpColider)
                {
                    lastJumpColider = null;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 cubeCenter = transform.position + _direction.normalized * (_cubeSize * 0.5f);
            Gizmos.DrawWireCube(cubeCenter, Vector3.one * _cubeSize);
        }
    }
}
