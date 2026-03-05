using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 dir = Vector3.zero;

        if (Keyboard.current.wKey.isPressed) dir += Vector3.forward;
        if (Keyboard.current.sKey.isPressed) dir += Vector3.back;
        if (Keyboard.current.aKey.isPressed) dir += Vector3.left;
        if (Keyboard.current.dKey.isPressed) dir += Vector3.right;

        dir = dir.normalized; // vector math (no diagonal más rápida)

        // Move respeta colisiones con paredes
        controller.Move(dir * speed * Time.deltaTime);
    }
}