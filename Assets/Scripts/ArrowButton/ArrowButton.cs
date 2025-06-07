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
        private Vector3 _startScale;
        private Vector3 _shrinkScale;

        private void Awake()
        {
            _startScale = components[0].transform.localScale;
            _shrinkScale = _startScale * 2/3;
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
            switch (GameManager.currentGameState)
            {
                case GameState.MainMenu:
                    if (destination == GameState.Options)
                    {
                        StartCoroutine(RotateCamera(mainCamera.transform, 90.0f));
                        GameManager.currentGameState = GameState.Options;
                    }
                    else if (destination == GameState.Credits)
                    {
                        StartCoroutine(RotateCamera(mainCamera.transform, -90.0f));
                        GameManager.currentGameState = GameState.Credits;
                    }
                    destination = GameState.MainMenu;
                    break;
                case GameState.Credits:
                    if (destination == GameState.MainMenu)
                    {
                        StartCoroutine(RotateCamera(mainCamera.transform, 0));
                        GameManager.currentGameState = GameState.MainMenu;
                    }
                    destination = GameState.Credits;
                    break;
                case GameState.Options:
                    if (destination == GameState.MainMenu)
                    {
                        StartCoroutine(RotateCamera(mainCamera.transform, 0));
                        GameManager.currentGameState = GameState.MainMenu;
                    }
                    destination = GameState.Options;
                    break;
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
            else if (angle < 0)
            {
                
                while (target.rotation.eulerAngles.y > angle)
                {
                    var angles = target.rotation.eulerAngles;
                    angles.y = target.rotation.eulerAngles.y + angle * Time.deltaTime * 2;
                    var rotation = target.rotation;
                    rotation.eulerAngles = angles;
                    target.rotation = rotation;

                    if (target.rotation.eulerAngles.y < 360f + angle)
                    {
                        var vector3 = target.rotation.eulerAngles;
                        vector3.y = angle;
                        var quaternion = target.rotation;
                        quaternion.eulerAngles = vector3;
                        target.rotation = quaternion;
                        break;
                    }

                    yield return null;
                }
            }
            if (angle == 0)
            {
                if (target.rotation.eulerAngles.y < 180)
                {
                    while (target.rotation.eulerAngles.y > 0)
                    {
                        if (target.rotation.eulerAngles.y < 340f)
                        {
                            var angles = target.rotation.eulerAngles;
                            angles.y = target.rotation.eulerAngles.y - Time.deltaTime * 200;
                            var rotation = target.rotation;
                            rotation.eulerAngles = angles;
                            target.rotation = rotation;

                            if (target.rotation.eulerAngles.y is > 0 and < 1)
                            {
                                var vector3 = target.rotation.eulerAngles;
                                vector3.y = 0;
                                var quaternion = target.rotation;
                                quaternion.eulerAngles = vector3;
                                target.rotation = quaternion;
                            }
                        }
                        else
                        {
                            var vector3 = target.rotation.eulerAngles;
                            vector3.y = 0;
                            var quaternion = target.rotation;
                            quaternion.eulerAngles = vector3;
                            target.rotation = quaternion;
                            break;
                        };

                        yield return null;

                    }
                }
                else
                {
                    while (target.rotation.eulerAngles.y > 180)
                    {
                        var angles = target.rotation.eulerAngles;
                        angles.y = target.rotation.eulerAngles.y + Time.deltaTime * 200;
                        var rotation = target.rotation;
                        rotation.eulerAngles = angles;
                        target.rotation = rotation;

                        if (target.rotation.eulerAngles.y is < 180f and > 0)
                        {
                            var vector3 = target.rotation.eulerAngles;
                            vector3.y = 0;
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
}
