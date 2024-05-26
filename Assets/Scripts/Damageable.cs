using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    Animator animator;

    public UnityEvent<float, Vector2> damageableHit;
    public UnityEvent<float, float> healthChanged;
    
    [SerializeField]
    private float _maxHealth = 100;
    public float MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }

    [SerializeField]
    private float _health = 100;
    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth); // Clamp the health value between 0 and MaxHealth
            healthChanged?.Invoke(_health, MaxHealth);

            // If health is 0 or less, set IsAlive to false
            if(_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    private bool isInvincible = false;
    private float timeSinceHit = 0;
    public float invincibilityTime = 1f;

    [SerializeField]
    private bool _isAlive = true;
    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive set " + value);
        }
    }

    // Kecepatan Tidak Boleh diubah Selama ini Benar, Tetapi Perlu Dihormati oleh Komponen Fisika Lainnya Seperti Kontrol Player.
    public bool LockVelocity { get 
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        } 
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        if(isInvincible)
        {
            if(timeSinceHit > invincibilityTime)
            {
                // Menghilangkan invincibility
                isInvincible = false;
                timeSinceHit = 0;
            }
            timeSinceHit += Time.deltaTime;
        } 
    }

    private void FixedUpdate()
    {

    }

    // Mengembalikan Nilai Jika Terkena Damage atau Tidak
    public bool Hit(float damage, Vector2 knockback)
    {
        if(IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;

            // Memberi Tahu Komponen Lainnya Bahwa Objek Menerima Serangan Telah Terkena Untuk Mengatasi Efek Lanjutan Seperti Knockback dll
            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            damageableHit?.Invoke(damage, knockback);
            CharacterEvents.characterDamaged.Invoke(gameObject, damage);

            return true;
        }
        // Tidak Bisa Di Hit
        return false;
    }

    public bool Heal(float healthRestore)
    {
        if(IsAlive && Health < MaxHealth)
        {
            float maxHeal = Mathf.Max(MaxHealth - Health, 0);
            float actualHeal = Mathf.Min(maxHeal, healthRestore);
            Health += actualHeal;
            CharacterEvents.characterHealed.Invoke(gameObject, actualHeal);
            return true;
        }
        return false;
    }
}
