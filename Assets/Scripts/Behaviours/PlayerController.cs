using Sevakon.Input;
using Sevakon.Managers;
using Sevakon.Systems;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.Input;

namespace Sevakon.Behaviours
{
    /// <summary>
    /// Controls player movement and shooting
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        public GameManager gameManager;

        public float moveSpeed;
        public float rotateSpeed;
        public float jumpForce = 2.0f;

        private Vector2 m_Move;
        private Vector2 m_Look;
        private bool isGrounded = true;
        private Vector2 m_Rotation;
        private Rigidbody m_Rigidbody;
        private Transform cameraTransform;
        private Controls controls;

        private void Awake()
        {
            controls = gameManager.controls;

            controls.Gameplay.FirePerformed.AddListener(FireBullet);

            controls.Gameplay.MovePerformed.AddListener(context => m_Move = context.ReadValue<Vector2>());
            controls.Gameplay.LookPerformed.AddListener(context => m_Look += context.ReadValue<Vector2>());
            controls.Gameplay.JumpPerformed.AddListener(context =>
            {
                if (isGrounded)
                {
                    var jumpVector = Vector3.up * jumpForce;
                    m_Rigidbody.AddForce(jumpVector, ForceMode.Impulse);
                    isGrounded = false;
                }
            });
        }

        private void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            cameraTransform = GetComponentInChildren<Camera>().transform;
        }

        private void OnEnable()
        {
            controls.Enable();
        }

        private void OnDisable()
        {
            controls.Disable();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Terrain"))
            {
                isGrounded = true;
            }
        }

        private void FireBullet(InputAction.CallbackContext context)
        {
            World.Active.GetOrCreateManager<BulletSystem>().Spawn();
        }

        private void LateUpdate()
        {
            Move();
            Look();
        }

        private void Move()
        {
            var direction = m_Move;
            var scaledMoveSpeed = moveSpeed * Time.deltaTime;
            var move = transform.TransformDirection(direction.x, 0, direction.y);
            m_Rigidbody.position += move * scaledMoveSpeed;
        }

        private void Look()
        {
            const float kClampAngle = 80.0f;

            var rotate = m_Look;
            m_Rotation.y += rotate.x * rotateSpeed * Time.deltaTime;
            m_Rotation.x -= rotate.y * rotateSpeed * Time.deltaTime;

            m_Rotation.x = Mathf.Clamp(m_Rotation.x, -kClampAngle, kClampAngle);

            var playerRotation = Quaternion.Euler(0f, m_Rotation.y, 0f);
            var cameraRotation = Quaternion.Euler(m_Rotation.x, 0f, 0f);

            m_Rigidbody.rotation = playerRotation;
            cameraTransform.localRotation = cameraRotation;

            m_Look = Vector2.zero;
        }
    }
}