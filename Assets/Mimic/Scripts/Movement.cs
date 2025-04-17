using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MimicSpace
{
    /// <summary>
    /// This is a very basic movement script, if you want to replace it
    /// Just don't forget to update the Mimic's velocity vector with a Vector3(x, 0, z)
    /// </summary>
    public class Movement : MonoBehaviour
    {
        [Header("Controls")]
        [Tooltip("Body Height from ground")]
        [Range(0.5f, 5f)]
        public float height = 0.8f;
        public float speed = 5f;
        [Tooltip("Multiplier for the applied force.")]
        public float forceMultiplier = 10f;
        Vector3 velocity = Vector3.zero;
        public float velocityLerpCoef = 4f;
        Mimic myMimic;
        Rigidbody rb;

        public LayerMask validTerrain;

        private void Start()
        {
            myMimic = GetComponent<Mimic>();
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
    {
        // Rotate player based on mouse input
        float mouseX = Input.GetAxis("Mouse X");
        Vector3 rotation = new Vector3(0f, mouseX * 2, 0f);
        transform.Rotate(rotation);
    }

        void FixedUpdate()
        {
            // Horizontal movement: get input and determine the desired horizontal velocity.
            Vector3 inputVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            Vector3 targetVelocity = transform.TransformDirection(inputVector) * speed;
            
            // Current horizontal velocity (ignoring vertical component)
            Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            
            // Calculate the difference between desired and current velocity.
            Vector3 velocityChange = targetVelocity - currentHorizontalVelocity;
            
            // Apply force to achieve the target horizontal velocity.
            rb.AddForce(velocityChange * forceMultiplier, ForceMode.Acceleration);

            // Update the Mimic's velocity for leg placement.
            myMimic.velocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            // Vertical adjustment: raycast to determine ground height.
            RaycastHit hit;
            // if (Physics.Raycast(transform.position + Vector3.up * 5f, -Vector3.up, out hit, Mathf.Infinity, validTerrain))
            if (Physics.Raycast(transform.position + Vector3.up * 1f + transform.forward * 0.5f, -Vector3.up, out hit, Mathf.Infinity, validTerrain))
            {
                // Calculate the target vertical position.
                float targetY = hit.point.y + height;
                float yDifference = targetY - transform.position.y;
                
                // Apply an upward force proportional to the vertical difference.
                rb.AddForce(Vector3.up * Math.Min(10, yDifference) * forceMultiplier, ForceMode.Acceleration);
            }

            if (rb.linearVelocity.magnitude > 100){  rb.linearVelocity = rb.linearVelocity.normalized * 100; }
        }

    }

}