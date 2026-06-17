using UnityEngine;
using UnityEngine.SceneManagement;

namespace MagicalTower.Runtime
{
    public sealed class SceneLoader : MonoBehaviour
    {
        public void ReloadActiveScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.buildIndex < 0)
            {
                Debug.LogError(
                    $"Cannot reload scene '{activeScene.path}' because it is not enabled in Build Settings.",
                    this);
                return;
            }

            SceneManager.LoadScene(activeScene.buildIndex);
        }
    }
}
