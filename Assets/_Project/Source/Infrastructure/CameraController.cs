using System;
using UnityEngine;

namespace ConwaysGameOfLife.Source.Infrastructure
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private void Update()
        {
            Vector3 direction = GetBaseInput();
            if (direction.sqrMagnitude > 0.01f)
                transform.Translate(_speed * Time.deltaTime * direction);
        }

        private Vector3 GetBaseInput()
        {
            Vector3 direction = new Vector3();

            if (Input.GetKey(KeyCode.W))
            {
                direction += new Vector3(0, 0, 1);
            }

            if (Input.GetKey(KeyCode.S))
            {
                direction += new Vector3(0, 0, -1);
            }

            if (Input.GetKey(KeyCode.A))
            {
                direction += new Vector3(-1, 0, 0);
            }

            if (Input.GetKey(KeyCode.D))
            {
                direction += new Vector3(1, 0, 0);
            }

            return direction;
        }
    }
}