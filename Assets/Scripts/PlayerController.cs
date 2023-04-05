using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float gravityScale = -9.8f;
    [Range(0f, 100f)]
    public float moveSpeed = 5f;
    [SerializeField] private AudioSource footstepAudio;
    [SerializeField] private AudioClip[] footstepClips;
    [Range(0f, 1f)]
    [SerializeField] private float footstepSpeed = .5f;
    private float footstepTime;

    [Header("Gun Settings")]
    [Range(0, 100)]
    public int maxAmmo = 2;
    [Range(0f, 10f)]
    public float fireRate = 0.5f;
    [SerializeField] private AudioSource gunAudio;
    [SerializeField] private AudioClip[] gunShotClips;
    [SerializeField] private ParticleSystem gunParticles;
    private int ammo;
    private float gunCD;

    [SerializeField] new private Camera camera;
    [SerializeField] private LayerMask enemyMask;


    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        footstepTime = 0f;
        ammo = maxAmmo;
        gunCD = 0f;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsInPlay)
        {
            return;
        }

        LookControls();
        MoveControls();

        if (Input.GetMouseButton(0))
        {
            if (gunCD <= 0f && ammo > 0)
            {
                FireGun();
                gunCD = fireRate;
                ammo--;
            }
        }

        if (gunCD > 0f)
        {
            gunCD -= Time.deltaTime;
        }
    }

    private void LookControls()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.Rotate(Vector3.up, mouseX);
        camera.transform.Rotate(Vector3.right, -mouseY);
    }

    private void MoveControls()
    {
        // move controls
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(moveSpeed * Time.deltaTime * move);

        // gravity
        Vector3 gravity = gravityScale * Time.deltaTime * Vector3.up;
        controller.Move(gravity);

        // footstep sfx
        if (controller.velocity.magnitude > 0 && footstepTime <= 0f)
        {
            footstepAudio.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)], 
                footstepAudio.volume);
            footstepTime = footstepSpeed;
        }
        if (footstepTime > 0f)
        {
            footstepTime -= Time.deltaTime;
        }
    }

    private void FireGun()
    {
        Transform cam = camera.transform;

        Debug.Log("Firing Gun");
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 50f, enemyMask))
        {
            GameManager.Instance.OnEnemyHit.Invoke();
            ammo = maxAmmo;
            Debug.Log("Enemy Hit");
            Debug.DrawRay(cam.position, hit.distance * cam.forward, Color.yellow, 2f);
        }
        else
        {
            Debug.DrawRay(cam.position, 50f * cam.forward, Color.yellow, 2f);
        }

        // gun sfx and vfx
        gunAudio.PlayOneShot(gunShotClips[Random.Range(0, gunShotClips.Length)], gunAudio.volume);
        gunParticles.Play();
    }
}
