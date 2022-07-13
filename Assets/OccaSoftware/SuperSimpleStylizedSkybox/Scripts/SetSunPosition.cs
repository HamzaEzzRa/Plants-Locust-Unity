using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OccaSoftware
{
    public class SetSunPosition : MonoBehaviour
    {
        [SerializeField] float rotationSpeed = 10f;

        void Update()
        {
            transform.Rotate(transform.right * rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}