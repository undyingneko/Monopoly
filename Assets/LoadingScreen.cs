using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;
    public TextMeshProUGUI progressText;

    [SerializeField]
    private GameObject loading_Bar_Holder;

    [SerializeField]
    private Image loading_Bar_Progress;

    private float progress_Value = 1.1f;
    public float progress_Multiplier_1 = 0.5f;
    public float progress_Multiplier_2 = 0.07f;
    public float load_level_Time = 2f;
    
    void Awake() {
        MakeSingleton();
    }

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(LoadingSomeLevel());
    }
    void Update() {
        ShowLoadingScreen();
    }
    void MakeSingleton() {
        if(instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void LoadLevel(string levelName){
        loading_Bar_Holder.SetActive(true);
        progress_Value = 0f;
        //Time.timeScale = 0f;
        SceneManager.LoadScene(levelName);
    }
    
    void ShowLoadingScreen() {
        if(progress_Value < 1f) {
            progress_Value += progress_Multiplier_1 * progress_Multiplier_2;
            loading_Bar_Progress.fillAmount = progress_Value;

            // the loadign bar has finished
            if(progress_Value >= 1f){
               progress_Value = 1.1f;
               loading_Bar_Progress.fillAmount = 0f;
               loading_Bar_Holder.SetActive(false);

               //Time.timeScale = 1f;
            }
        } // if progress value < 1
    }
    IEnumerator LoadingSomeLevel() {
        yield return new WaitForSeconds(load_level_Time);
        // LoadLevel("Gameplay");
        LoadLevelAsync("MainMenu");
    }

    public void LoadLevelAsync( string levelName){
        StartCoroutine(LoadAsynchronously(levelName));
    }

    IEnumerator LoadAsynchronously(string levelName) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);
        loading_Bar_Holder.SetActive(true);

        //while the operation is NOT DONE
        while(!operation.isDone){
            float progress = operation.progress / 0.9f;
            // Debug.Log(progress);
            loading_Bar_Progress.fillAmount = progress;
            progressText.text = progress * 100f + "%";
            if(progress >= 1f) {
                loading_Bar_Holder.SetActive(false);
            }
            yield return null; 
        }
    }
    
}
//---------------
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;
// using TMPro;
// public class LoadingScreen : MonoBehaviour{
//     [SerializeField]
//     public GameObject loadingScreen;
//     public Slider slider;
//     public TextMeshProUGUI progressText;
//     // public float load_level_Time = 2f;
//     private bool loadingComplete = false;
    
//     public void LoadLevel(string levelName)
//     {
//         StartCoroutine(LoadAsynchronously(levelName));
//     }
//     // IEnumerator levelName() {
//     //     yield return new WaitForSeconds(load_level_Time);
//     //     // LoadLevel("MainMenu");
//     //     LoadLevel("MainMenu");
//     // }
//     IEnumerator LoadAsynchronously (string levelName)
//     {
//         AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);

//         loadingScreen.SetActive(true);

//         while (!operation.isDone)
//         {
//             float progress = Mathf.Clamp01(operation.progress / 0.9f);
//             Debug.Log(progress);
//             slider.value = progress;
//             progressText.text = progress * 100f + "%";
//             yield return null;
//         }
//         loadingComplete = true; // Mark loading as complete
//     }
//      private void Update()
//     {
//         if (loadingComplete)
//         {
//             // When loading is complete, transition to the MainMenu scene
//             SceneManager.LoadSceneAsync("MainMenu");
//         }
//     }   

// }
//-------------------------------------------