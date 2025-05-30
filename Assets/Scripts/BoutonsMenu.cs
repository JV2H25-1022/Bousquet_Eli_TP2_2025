using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BoutonsMenu : MonoBehaviour
{

    [SerializeField] private GameObject _PanneauOptions;

    void Start()
    {

    }

    void Update()
    {

    }

    public void CommencerJeu(string Scene)
    {

        Debug.Log("Scene à charger: " + Scene);
        SceneManager.LoadScene(Scene);

    }


    public void VoirOptions()
    {

        _PanneauOptions.SetActive(true);

    }
    
        public void FermerOptions()
    {

        _PanneauOptions.SetActive(false);

    }
    
    public void QuitterJeu()
    {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

    }

}
