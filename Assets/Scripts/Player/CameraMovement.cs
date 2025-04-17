using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        public bool followPlayer;

        private void Awake()
        {
            followPlayer = false;
        }
        
        private void Update()
        {
            if (!followPlayer) return;
            
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3((player.transform.position - player.transform.forward * 2).x, 1f, (player.transform.position - player.transform.forward * 2).z), Time.deltaTime*2.5f);
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, player.transform.rotation, Time.deltaTime*3);
        }
    }
}