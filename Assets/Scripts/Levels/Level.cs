using UnityEngine;

namespace ITMoscowRun.Scripts.Levels
{
    public class Level : MonoBehaviour
    {
        public Transform EndPoint;

        private int id;
        private LevelManager levelManager;

        public void InitLevel(int ID, LevelManager manager)
        {
            id = ID;
            levelManager = manager;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (levelManager == null)
            {
                Debug.LogError("NO MANAGER FOUND IN " + gameObject.name);
                return;
            }
            
            if (other.CompareTag("Player"))
            {
                levelManager.MoveForward();
            }
        }
    }
}
