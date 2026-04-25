using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObjectPool : MonoBehaviour, IFactory<GameObject>
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject _prefab;

    private List<GameObject> _objectPool = new List<GameObject>();
    private DiContainer _diContainer;

    public GameObject GetFromPool()
    {
        for (int i = 0; i < _objectPool.Count; i++)
        {
            if (_objectPool[i].activeSelf)
                continue;

            _objectPool[i].SetActive(true);
            return _objectPool[i];
        }

        GameObject newObject = Create();
        newObject.SetActive(true);
        return newObject;
    }

    public GameObject Create()
    {
        // Объект создаётся как дочерний для этого пула
        GameObject newObject = _diContainer.InstantiatePrefab(_prefab, transform);

        newObject.SetActive(false);
        _objectPool.Add(newObject);

        return newObject;
    }

    [Inject]
    private void Construct(DiContainer diContainer)
    {
        _diContainer = diContainer;
    }
}