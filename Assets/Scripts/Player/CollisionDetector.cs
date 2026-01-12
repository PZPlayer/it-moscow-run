using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ITMoscowRun.Scripts.Player
{
    [RequireComponent(typeof(Movement))]
    public class CollisionDetector : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onVictory;
        [SerializeField] private UnityEvent _onDeafeat;
        [SerializeField] private float _defeatAnimationWait = 1;
        [SerializeField] private Animator _animator;

        private bool dead = false;

        private void OnCollisionEnter(Collision other)
        {
            if(dead) return;

            if (other.gameObject.layer == 6) //layer 6 = "Obstacle"
            {
                _animator.SetTrigger(other.transform.position.y >= transform.position.y ? "DeathFromWall" : "DeathFromBelow");
                StartCoroutine(WaitUntilCalling(_onDeafeat, _defeatAnimationWait));
            }
            else if (other.gameObject.layer == 7) //layer 6 = "Victory"
            {
                Victory();
            }
        }

        private IEnumerator WaitUntilCalling(UnityEvent @event, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);  
            @event.Invoke();
        }

        private void Victory()
        {
            _onVictory?.Invoke();
            transform.GetComponent<Movement>().enabled = false;
        }

        public void Death()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
