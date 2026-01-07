using UnityEngine;
using UnityEngine.InputSystem;

namespace ITMoscowRun.Scripts.Player
{
    [RequireComponent(typeof(Movement))]
    public class SwipeDetector : MonoBehaviour
    {
        [SerializeField] private float minDistance = 0.2f;
        [SerializeField] private float maxTime = 1f;

        private PlayerInput input;
        private Movement movement;
        private Vector2 startPosition;
        private float startTime;

        private void Start()
        {
            input = GetComponent<PlayerInput>();
            movement = GetComponent<Movement>();

            input.actions["PrimaryContact"].started += StartTouch;
            input.actions["PrimaryContact"].canceled += EndTouch;
        }

        private void StartTouch(InputAction.CallbackContext context)
        {
            startPosition = input.actions["PrimaryPosition"].ReadValue<Vector2>();
            startTime = (float)context.startTime;
        }

        private void EndTouch(InputAction.CallbackContext context)
        {
            Vector2 endPosition = input.actions["PrimaryPosition"].ReadValue<Vector2>();
            float endTime = (float)context.time;

            DetectSwipe(startPosition, endPosition, startTime, endTime);
        }

        private void DetectSwipe(Vector2 start, Vector2 end, float sTime, float eTime)
        {
            print("df" + Vector3.Distance(start, end) + "    " + (eTime - sTime) + "     " + maxTime + "    " + eTime + "    " + sTime);
            if (Vector3.Distance(start, end) > minDistance && (eTime - sTime) <= maxTime)
            {
                Vector2 direction = (end - start).normalized;
                print("df2");
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    if (direction.x > 0)
                    {
                        print("right");
                        movement.CheckLines(Vector2.right);
                    }
                    else
                    {
                        movement.CheckLines(-Vector2.right);
                    }
                }
                else
                {
                    if (direction.y > 0)
                    {
                        movement.CheckLines(Vector2.up);
                    }
                    else
                    {
                        movement.CheckLines(-Vector2.up);
                    }
                }
            }
        }
    }
}
