using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

namespace StartButton
{
    public class StartButtonHitbox : MonoBehaviour
    {
    
        [SerializeField] private GameObject[] gameObjects = new GameObject[5];
        [FormerlySerializedAs("_components")] public List<StartBtnComponent> components;
        [SerializeField] private GameObject mainCamera;
        [SerializeField] private GameObject player;

        private void Start()
        {
            foreach (var o in gameObjects)
            {
                components.Add(o.GetComponent<StartBtnComponent>());
                o.GetComponent<StartBtnComponent>().Shrink();
            }
        }

        private void OnMouseOver()
        {
            foreach (var component in components)
            {
                component.Grow();
            }
        }

        private void OnMouseExit()
        {
            foreach (var component in components)
            {
                component.Shrink();
            }
        }

        private void OnMouseDown()
        {
            //mainCamera.transform.parent = player.transform;
            mainCamera.GetComponent<CameraMovement>().followPlayer = true;
            
            foreach (var o in gameObjects)
            {
                o.SetActive(false);
            }

            gameObject.SetActive(false);

            SceneManager.LoadSceneAsync("Level1");
        }
    }
}
