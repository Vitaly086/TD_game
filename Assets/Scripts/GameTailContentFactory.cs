using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GameTailContentFactory : ScriptableObject
{
    [SerializeField] private GameTileContent _destinationPrefab;
    [SerializeField] private GameTileContent _emptyPrefab;

    private Scene _contentScene;

    public void Reclaim(GameTileContent content)
    {
        Destroy(content.gameObject);
    }

    public GameTileContent Get(GameTileContentType type)
    {
        switch (type)
        {
            case GameTileContentType.Destination:
                return Get(_destinationPrefab);
            case GameTileContentType.Empty:
                return Get(_emptyPrefab);
        }

        return null;
    }
    private GameTileContent Get(GameTileContent prefab)
    {
        GameTileContent instance = Instantiate(prefab);
        instance.TailContentFactory = this;
        MoveToFactoryScene(instance.gameObject);
        return instance;
    }


    private void MoveToFactoryScene(GameObject obj)
    {
        if (!_contentScene.isLoaded)
        {
            if (Application.isEditor)
            {
                _contentScene = SceneManager.GetSceneByName(name);
                if (!_contentScene.isLoaded)
                {
                    _contentScene = SceneManager.CreateScene(name);
                }
            }
            else
            {
                _contentScene = SceneManager.CreateScene(name);
            }
            SceneManager.MoveGameObjectToScene(obj, _contentScene);
        }
    }
}