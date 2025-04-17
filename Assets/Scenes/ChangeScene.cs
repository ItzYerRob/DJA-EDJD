using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {
	public Button newGameButton, continueButton, optionsButton, creditsButton, exitButton;

	void Start () {
		Button ngbtn = newGameButton.GetComponent<Button>();
		ngbtn.onClick.AddListener(onClickNewGameButton);

        Button contbtn = continueButton.GetComponent<Button>();
		contbtn.onClick.AddListener(onClickContinueButton);

        Button optionsbtn = optionsButton.GetComponent<Button>();
		optionsbtn.onClick.AddListener(onClickOptionsButton);

        Button creditsbtn = creditsButton.GetComponent<Button>();
		creditsbtn.onClick.AddListener(onClickCreditsButton);

        Button exitbtn = exitButton.GetComponent<Button>();
		exitbtn.onClick.AddListener(onClickExitButton);
	}

	public void onClickNewGameButton(){
		SceneManager.LoadSceneAsync("GameScene");
	}
    public void onClickContinueButton(){
		SceneManager.LoadSceneAsync("GameScene");
	}
    public void onClickOptionsButton(){
		//
	}
    public void onClickCreditsButton(){
		//
	}

    public void onClickExitButton(){
		//
	}
}