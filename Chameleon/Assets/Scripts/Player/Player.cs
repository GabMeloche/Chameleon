using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [SerializeField]
    public float m_movementSpeed = 0.01f;

    private Rigidbody m_rigidbody;
    public UnityEvent m_playerCaught;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        m_rigidbody.velocity = Vector3.zero;

        float movementSpeed = m_movementSpeed;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Translate(Vector3.right * movementSpeed * horizontal * Time.deltaTime);
        transform.Translate(Vector3.forward * movementSpeed * vertical * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Guard guard;

        if (collision.gameObject.TryGetComponent(out guard))
        {
            m_playerCaught.Invoke();
        }
    }
}
