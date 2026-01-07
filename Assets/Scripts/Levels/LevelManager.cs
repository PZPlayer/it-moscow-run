using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace ITMoscowRun.Scripts.Levels
{
    [Serializable]
    public struct Bioms
    {
        public List<GameObject> levels; 
    }

    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<Bioms> _bioms;
        [SerializeField] private List<Level> _spawnedLevels;
        [SerializeField] private int _levelsAhead = 5;
        [SerializeField] private int _levelsSeeDistance = 3;

        private int currentLevel = 1;

        private void Start()
        {
            if (currentLevel >= _spawnedLevels.Count)
            {
                StartCoroutine(SpawnLevels(currentLevel + _levelsAhead));
            }
        }

        private IEnumerator SpawnLevels(int spawnUntil)
        {
            while (_spawnedLevels.Count < spawnUntil)
            {
                int chooseBiom = UnityEngine.Random.Range(0, Mathf.Max(_bioms.Count, 0));
                
                foreach (GameObject level in _bioms[chooseBiom].levels)
                {
                    _spawnedLevels.Add(Instantiate(level, _spawnedLevels[_spawnedLevels.Count - 1].EndPoint.position, Quaternion.identity).GetComponent<Level>());
                    _spawnedLevels[^1].InitLevel(_spawnedLevels.Count - 1, this);
                }
                yield return null;
            }
        }

        public void MoveForward()
        {
            int targetIndex = Mathf.Max(0, currentLevel - 2);

            if (targetIndex < _spawnedLevels.Count && _spawnedLevels[targetIndex] != null)
            {
                _spawnedLevels[targetIndex].gameObject.SetActive(false);
            }

            currentLevel++;
            StartCoroutine(SpawnLevels(currentLevel + _levelsAhead));
        }
    }
}