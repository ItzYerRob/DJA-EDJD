using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace ArrowButton
{
    public class ArrowButton : MonoBehaviour
    {
        [SerializeField] private GameObject button;
        [SerializeField] private List<GameObject> components;
        [SerializeField] private GameObject mainCamera;
        [SerializeField] private GameState destination;
        private GameState _gameState; 
        private Vector3 _startScale;
        private Vector3 _shrinkScale;

        private void Awake()
        {
            _startScale = components[0].transform.localScale;
            _shrinkScale = _startScale * 2/3;
            _gameState = GameManager.currentGameState;
            Shrink();
        }
        
        private void OnMouseEnter()
        {
            Grow();
        }

        private void OnMouseExit()
        {
            Shrink();
        }

        private void Shrink()
        {
            button.GetComponent<Animator>().enabled = false;
            foreach (var component in components)
            {
                component.transform.localScale = _shrinkScale;
                var color = component.GetComponent<Renderer>().material.color;
                color.a = 0.5f;
                component.GetComponent<Renderer>().material.color = color;
            }
        }

        private void Grow()
        {
            button.GetComponent<Animator>().enabled = true;
            foreach (var component in components)
            {
                component.transform.localScale = _startScale;
                var color = component.GetComponent<Renderer>().material.color;
                color.a = 1.0f;
                component.GetComponent<Renderer>().material.color = color;
            }
        }

        private void OnMouseDown()
        {
            
            if (destination == GameState.Options)
            {
                if (_gameState == GameState.MainMenu)
                {
                    GameManager.currentGameState = GameState.Options;
                    destination = GameState.MainMenu;
                    StartCoroutine(RotateCamera(mainCamera.transform, 90f));
                }
                else
                {
                    GameManager.currentGameState = GameState.MainMenu;
                    destination = GameState.Options;
                    StartCoroutine(RotateCamera(mainCamera.transform, 0f));
                }
            } else if (destination == GameState.Credits)
            {
                if (_gameState == GameState.MainMenu)
                {
                    GameManager.currentGameState = GameState.Credits;
                    destination = GameState.MainMenu;
                    StartCoroutine(RotateCamera(mainCamera.transform, -90f));
                }
                else
                {
                    GameManager.currentGameState = GameState.MainMenu;
                    destination = GameState.Credits;
                    StartCoroutine(RotateCamera(mainCamera.transform, 0f));
                }
            }
        }

        private static IEnumerator RotateCamera(Transform target, float angle)
        {
            if (angle > 0)
            {
                while (target.rotation.eulerAngles.y < angle)
                {
                    var angles = target.rotation.eulerAngles;
                    angles.y = target.rotation.eulerAngles.y + angle * Time.deltaTime * 2;
                    var rotation = target.rotation;
                    rotation.eulerAngles = angles;
                    target.rotation = rotation;

                    if (target.rotation.eulerAngles.y > angle)
                    {
                        var vector3 = target.rotation.eulerAngles;
                        vector3.y = angle;
                        var quaternion = target.rotation;
                        quaternion.eulerAngles = vector3;
                        target.rotation = quaternion;
                    }

                    yield return null;
                }
            }
            else
            {
                while (target.rotation.eulerAngles.y > angle)
                {
                    var angles = target.rotation.eulerAngles;
                    angles.y = target.rotation.eulerAngles.y + angle * Time.deltaTime * 2;
                    var rotation = target.rotation;
                    rotation.eulerAngles = angles;
                    target.rotation = rotation;
                    print(target.rotation.eulerAngles.y);

                    if (target.rotation.eulerAngles.y < 360f + angle)
                    {
                        var vector3 = target.rotation.eulerAngles;
                        vector3.y = angle;
                        var quaternion = target.rotation;
                        quaternion.eulerAngles = vector3;
                        target.rotation = quaternion;
                    }

                    yield return null;
                }
            }
        }
    }
}
