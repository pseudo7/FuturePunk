using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject enemyPrefab;

    public EnemyTexture[] enemyTextures;

    public int enemyCount = 10;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
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
        Instantiate(enemyPrefab, new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50)), Quaternion.identity);
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
