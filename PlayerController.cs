using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player parameters
    private bool grounded;
    public Transform groundCheck;
    private float groundRadius = 0.2f;
    public LayerMask groundLayer;

    private bool climbing;
    private bool canClimb;
    private bool jumping;
    private bool rolling;
    private bool attacking;
    private bool idling;

    // Player Weapon
    public bool unarmed;
    public bool sword;
    public bool spear;
    public bool bow;
    string lastUsed;
    const string UNARMED = "Unarmed";
    const string SWORD = "Sword";
    const string SPEAR = "Spear";
    const string BOW = "Bow";
    
    Animator animator;
    PlayerAttack playerAttack;
    private float moveX;
    private float moveY;
    private bool facingRight = true;
    private bool dying = false;
    
    // Player animation base states
    string currentState;
    const string IDLE = "Idle";
    const string RUN = "Run";
    const string CLIMB = "Climb";
    const string DIE = "Die";
    const string JUMP = "Jump";
    const string ROLL = "Roll";
    const string FALL = "Fall";

    //Player animation sword states
    const string IDLE_SWORD = "IdleSword";
    const string ATTACKSWORD1 = "AttackSword1";
    const string ATTACKSWORD2 = "AttackSword2";
    const string ATTACKSWORD3 = "AttackSword3";
    const string RUN_SWORD = "RunSword";
    const string CLIMB_SWORD = "ClimbSword";
    const string DIE_SWORD = "DieSword";
    const string JUMP_SWORD = "JumpSword";
    const string ROLL_SWORD = "RollSword";
    const string FALL_SWORD = "FallSword";

    //Player animation bow states
    const string IDLE_BOW = "IdleBow";
    const string ATTACKBOW = "AttackBow";
    const string RUN_BOW = "RunBow";
    const string CLIMB_BOW = "ClimbBow";
    const string DIE_BOW = "DieBow";
    const string JUMP_BOW = "JumpBow";
    const string ROLL_BOW = "RollBow";
    const string FALL_BOW = "FallBow";

    //Player animation sword states
    const string IDLE_SPEAR = "IdleSpear";
    const string ATTACKSPEAR1 = "AttackSpear1";
    const string ATTACKSPEAR2 = "AttackSpear2";
    const string ATTACKSPEARTHROW = "AttackSpearThrow";
    const string RUN_SPEAR = "RunSpear";
    const string CLIMB_SPEAR = "ClimbSpear";
    const string DIE_SPEAR = "DieSpear";
    const string JUMP_SPEAR = "JumpSpear";
    const string ROLL_SPEAR = "RollSpear";
    const string FALL_SPEAR = "FallSpear";


    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
        lastUsed = UNARMED;
    }

    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // JUMP //
        if ((Input.GetButtonDown("Jump")) && 
            (!jumping) && (grounded))
        {
            jumping = true;
            Jump();
        }

        // RUN //
        if ((moveX != 0) && (grounded) && 
            (!attacking) && (!rolling) && 
            (!climbing))
        { 
            Run();
            animator.speed = 1;
        }

        // SPRITE FLIP //
        if (moveX != 0f)
        {
            Flip();
        }

        // ATTACK //
        if ((Input.GetButtonDown("Attack")) && 
            (!attacking) && (!rolling))
        {
            attacking = true;
            Attack();
        }

        if (Input.GetButtonDown("AttackSpecial"))
        {
            // ADD LATER
            // sword heavy
            // spear throw
            attacking = true;
        }

        if ((Input.GetButtonDown("AttackRanged")) && 
            (!attacking) && (!rolling))
        {
            attacking = true;
            AttackRanged();
        }

        // ROLL //
        if ((Input.GetButtonDown("Roll")) && (!rolling))
        {
            animator.speed = 1;
            Roll();
            rolling = true;
        }

        // CLIMB //
        if ((moveY != 0) && (canClimb))
        {
            climbing = true;
            Climb();
        }

        if ((moveY == 0) && (climbing))
        {
            climbing = true;
            PauseClimb();
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            animator.Play(currentState);
        }
    }

    void LateUpdate()
    {
        // PARAMETER SETS //
        if ((!IsPlaying(animator, ATTACKBOW)) && 
            (!IsPlaying(animator, ATTACKSPEAR1)) &&
            (!IsPlaying(animator, ATTACKSWORD1)))
        {
            attacking = false;
        }

        if ((!IsPlaying(animator, ROLL)) && 
            (!IsPlaying(animator, ROLL_BOW)) && 
            (!IsPlaying(animator, ROLL_SPEAR)) &&
            (!IsPlaying(animator, ROLL_SWORD)))
        {
            rolling = false;
        }

        if ((!IsPlaying(animator, JUMP)) && 
            (!IsPlaying(animator, JUMP_BOW)) &&
            (!IsPlaying(animator, JUMP_SPEAR)) &&
            (!IsPlaying(animator, JUMP_SWORD)))
        {
            jumping = false;
        }

        // IDLE //
        if ((attacking) || (rolling) || 
            (jumping) || (climbing)) 
        {
            idling = false;
        }
        else
        {
            idling = true;
        }

        if ((moveX == 0) && (!climbing) && 
            (grounded) && (!attacking) &&
            (!rolling) && (!jumping) && (idling))
        {
            Idle();
        }

        // FALL //
        if ((grounded) || (climbing) ||
            (jumping) || (rolling) || 
            (attacking))
        {
            return;
        }
        else
        {
            Fall();
        }
    }

    void Idle()
    {
        if (lastUsed == UNARMED)
        {
            ChangeAnimation(IDLE);
        }

        if (lastUsed == SWORD)
        {
            ChangeAnimation(IDLE_SWORD);
        }

        if (lastUsed == SPEAR)
        {
            ChangeAnimation(IDLE_SPEAR);
        }
        
        if (lastUsed == BOW)
        {
            ChangeAnimation(IDLE_BOW);
        }
    }

    void Attack()
    {
        if (sword == true)
        {
            ChangeAnimation(ATTACKSWORD1);
            playerAttack.Attack();
            lastUsed = SWORD;
        }

        if (spear == true)
        {
            ChangeAnimation(ATTACKSPEAR1);
            playerAttack.Attack();
            lastUsed = SPEAR;
        }
    }

    void AttackSpecial()
    {

    }

    void AttackRanged()
    {
        if (bow == true)
        {
            ChangeAnimation(ATTACKBOW);
            playerAttack.AttackRanged();
            lastUsed = BOW;
        }   
    }

    void Run()
    {
        if (lastUsed == UNARMED)
        {
            ChangeAnimation(RUN);
        }

        if (lastUsed == SWORD)
        {
            ChangeAnimation(RUN_SWORD);
        }

        if (lastUsed == SPEAR)
        {
            ChangeAnimation(RUN_SPEAR);
        }
        
        if (lastUsed == BOW)
        {
            ChangeAnimation(RUN_BOW);
        }
    }

    void Jump()
    {
        if (lastUsed == UNARMED)
        {
            ChangeAnimation(JUMP);
        }

        if (lastUsed == SWORD)
        {
            ChangeAnimation(JUMP_SWORD);
        }

        if (lastUsed == SPEAR)
        {
            ChangeAnimation(JUMP_SPEAR);
        }
        
        if (lastUsed == BOW)
        {
            ChangeAnimation(JUMP_BOW);
        }
    }

    void Fall()
    {
        if (lastUsed == UNARMED)
        {
            ChangeAnimation(FALL);
        }

        if (lastUsed == SWORD)
        {
            ChangeAnimation(FALL_SWORD);
        }

        if (lastUsed == SPEAR)
        {
            ChangeAnimation(FALL_SPEAR);
        }
        
        if (lastUsed == BOW)
        {
            ChangeAnimation(FALL_BOW);
        }
    }

    void Roll()
    {
        if (lastUsed == UNARMED)
        {
            ChangeAnimation(ROLL);
        }

        if (lastUsed == SWORD)
        {
            ChangeAnimation(ROLL_SWORD);
        }

        if (lastUsed == SPEAR)
        {
            ChangeAnimation(ROLL_SPEAR);
        }
        
        if (lastUsed == BOW)
        {
            ChangeAnimation(ROLL_BOW);
        }
    }


    void Climb()
    {
        animator.speed = 1;
        
        if (lastUsed == UNARMED)
        {
            ChangeAnimation(CLIMB);
        }

        if (lastUsed == SWORD)
        {
            ChangeAnimation(CLIMB_SWORD);
        }

        if (lastUsed == SPEAR)
        {
            ChangeAnimation(CLIMB_SPEAR);
        }
        
        if (lastUsed == BOW)
        {
            ChangeAnimation(CLIMB_BOW);
        }
    }

    void PauseClimb()
    {
        animator.speed = 0;
    }

    void Flip()
    {
        if ((moveX < 0f) && (facingRight == true))
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            facingRight = false;
        }

        if ((moveX > 0f) && (facingRight == false))
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            facingRight = true;
        }
    }

    public void Die()
    {
        if (lastUsed == UNARMED)
        {
            ChangeAnimation(DIE);
        }

        if (lastUsed == SWORD)
        {
            ChangeAnimation(DIE_SWORD);
        }

        if (lastUsed == SPEAR)
        {
            ChangeAnimation(DIE_SPEAR);
        }
        
        if (lastUsed == BOW)
        {
            ChangeAnimation(DIE_BOW);
        }

        dying = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            canClimb = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Ladder"))
        {
            canClimb = false;
            climbing = false;
        }
    }
    public void AnimatorWeapon(bool unequiped, bool swrd, bool spr, bool bw)
    {
        unarmed = unequiped;
        sword = swrd;
        spear = spr;
        bow = bw;
    }

    private void ChangeAnimation(string newState)
    {
        if (dying == true)
        {
            return;
        }

        if (newState == currentState)
        {
            return;
        }

        animator.Play(newState);
        currentState = newState;
    }

    private bool IsPlaying(Animator anim, string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
        anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}