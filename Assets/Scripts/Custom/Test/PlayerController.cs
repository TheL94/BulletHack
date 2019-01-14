﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : BaseUnit, IShooter
{
    public enum DirectionType { Global, Camera };

    #region Serialized Fields

    //References
    public ParticleSystem walkSmoke;
    [SerializeField] BaseGun equippedGun;
    public Transform gunPoint;

    //Behaviours
    public DirectionType InputDirection;
    public bool parry = true;
    public bool automaticParry = false;
    public bool dash = true;

    //Parameters
    // --BaseUnit.Health
    // --BaseUnit.MoveSpeed
    [SerializeField] KeyCode shootInput = KeyCode.Mouse0;
    [SerializeField] KeyCode parryInput = KeyCode.Mouse1;
    [SerializeField] KeyCode dashInput = KeyCode.Space;
    public LayerMask MaskToIgnore;
    public LayerMask aimLayer;
    [Tooltip("Changes apply at game start")] public float parryRadius = 2f;
    public float parryTime = .5f;
    public float parryCooldown = 2f;
    public float dashDistance = 10f;
    [Tooltip("Measured in meters per second")] public float dashSpeed = 5f;
    public float dashCooldown = 3f;

    //events
    public UnityEvent OnShoot;
    public FloatEvent OnParryStart, OnParryEnd, OnDashStart, OnDashEnd;

    #endregion

    #region Other Vars

    Vector3 moveDirection;
    Vector3 cameraBasedDirection;
    Vector3 playerDirection;

    SphereCollider catchNFireArea;
    Camera cam;
    Rigidbody rb;

    bool isMoving = false;
    bool canParry = true;
    bool isParrying = false;
    bool canDash = true;
    bool canMove = true;

    #endregion

    #region Properties

    public BaseGun EquippedGun
    {
        get
        {
            return equippedGun;
        }

        set
        {
            equippedGun = value;
        }
    }

    public bool IsMoving
    {
        get { return isMoving; }
        private set
        {
            isMoving = value;
            if (!walkSmoke)
                return;
            if (isMoving)
                walkSmoke.Play();
            else
                walkSmoke.Stop();
        }
    }

    #endregion

    #region MonoBehaviour methods

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        catchNFireArea = GetComponentInChildren<SphereCollider>();
        if (!catchNFireArea)
        {
            Debug.LogWarning(name + " did not find a catchAndFire collider!");
        }
        else
        {
            catchNFireArea.radius = parryRadius;
        }
        if (parry && automaticParry)
            isParrying = true;
        cam = Camera.main;
    }

    void Update()
    {
        #region Input

        //Move Input
        if (canMove)
        {
            if (InputDirection == DirectionType.Global)
            {
                moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
            }
            else
            if (InputDirection == DirectionType.Camera)
            {
                moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
                cameraBasedDirection = cam.transform.TransformDirection(moveDirection);
                moveDirection = new Vector3(cameraBasedDirection.x, moveDirection.y, cameraBasedDirection.z);
            }
        }

        //Shoot Input
        if (Input.GetKeyDown(shootInput) && EquippedGun)
        {
            EquippedGun.Shoot();
        }

        //ParryInput
        if (parry && !automaticParry && canParry && Input.GetKeyDown(parryInput))
        {
            StartParry();
        }

        //DashInput
        if (dash && canDash && Input.GetKeyDown(dashInput))
        {
            print(canDash);
            StartDash();
        }

        #endregion

        #region Aim

        //Aim Input
        RaycastHit hitInfo;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, int.MaxValue, aimLayer))
        {
            transform.LookAt(new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z));
        }

        #endregion

        if (moveDirection == Vector3.zero)
            IsMoving = false;
        else
            IsMoving = true;
    }

    private void FixedUpdate()
    {
        if(canMove)
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //check for enemy contact damage
        TestEnemy t = collision.collider.GetComponent<TestEnemy>();
        if (t && t.dealsDamageOnContact)
            TakeDamage(t.damage);
    }

    private void OnDrawGizmos()
    {
        if (parry)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, parryRadius);
        }
    }

    #endregion

    #region PlayeController methods

    public void EquipGun(BaseGun gunToEquip)
    {
        if (EquippedGun)
            Destroy(EquippedGun.gameObject);
        EquippedGun = gunToEquip;
        EquippedGun.transform.parent = gunPoint;
        EquippedGun.transform.position = gunPoint.position;
        EquippedGun.transform.rotation = gunPoint.rotation;
    }

    public override void TakeDamage(int amount)
    {
        Counters.instance.UpdateHits(amount);
        base.TakeDamage(amount);
    }

    public void SetParryDoability(bool f)
    {
        canParry = f;
    }

    public void StartParry()
    {
        isParrying = true;
        canParry = false;
        OnParryStart.Invoke(parryTime);
    }

    public void StopParry()
    {
        isParrying = false;
        OnParryEnd.Invoke(parryCooldown);
    }

    public void StartDash()
    {
        canDash = false;
        canMove = false;
        StartCoroutine(DashRoutine());
    }

    public void EndDash()
    {
        canMove = true;
        OnDashEnd.Invoke(dashCooldown);
    }

    public void SetDashDoability(bool f)
    {
        canDash = f;
    }

    IEnumerator DashRoutine()
    {
        Vector3 targetPos = rb.position + transform.forward * dashDistance;

        //perform the dash
        while (rb.position != targetPos)
        {
            rb.position = Vector3.MoveTowards(rb.position, targetPos, dashSpeed * Time.deltaTime); // --> remember we use fixed bcause we are moving a rigidbody
            yield return null;
        }

        EndDash();
    }

    //BaseUnit.Die();

    #endregion

}

[System.Serializable]
public class FloatEvent : UnityEvent<float>
{

}
