using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject enemyPrefab;
    public EnemyTexture[] enemyTextures;
    public Transform[] enemySpawns;

    public int enemyCount = 10;

    SpriteRenderer hitFlash;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        hitFlash = Camera.main.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        for (int i = 0; i < enemyCount; i++)
            SpawnEnemy();
        AudioManager.Instance.Play(Constants.BACKGROUND_AUDIO);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Pseudo/Capture")]
    public static void Capture()
    {
        ScreenCapture.CaptureScreenshot(string.Format("{0}.png", System.DateTime.Now.Ticks.ToString()));
    }
#endif

    void SpawnEnemy()
    {
        var enemyRenderer = enemyPrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        var gunRenderer = enemyPrefab.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();

        var enemyTexture = enemyTextures[Random.Range(0, enemyTextures.Length)];
        var mat = new Material(Shader.Find(Constants.SELF_ILLUMIN_SHADER_PATH));

        mat.SetTexture("_MainTex", enemyTexture.main);
        mat.SetTexture("_Illum", enemyTexture.emission);
        enemyRenderer.material = mat;
        mat.SetTexture("_MainTex", enemyTexture.mainGun);
        mat.SetTexture("_Illum", null);
        gunRenderer.material = mat;
        var spawnPoint = enemySpawns[Random.Range(0, enemySpawns.Length)];
        var spawnPosition = spawnPoint.position + Vector3.right * Random.Range(-5, 5) * 5;
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    public void RestartLevel(float delay)
    {
        StartCoroutine(LoadCurrentLevel(delay));
    }

    IEnumerator LoadCurrentLevel(float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public IEnumerator ShowHitFlash(int count)
    {
        while (count-- > 0)
        {
            hitFlash.color = Color.red;
            yield return new WaitForEndOfFrame();
            hitFlash.color = Color.clear;
        }
    }

    [System.Serializable]
    public struct EnemyTexture
    {
        public string name;
        public Texture main;
        public Texture emission;
        public Texture mainGun;

        public EnemyTexture(string name, Texture main, Texture emission, Texture mainGun)
        {
            this.name = name;
            this.main = main;
            this.emission = emission;
            this.mainGun = mainGun;
        }
    }
}
