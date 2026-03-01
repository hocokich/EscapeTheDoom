using UnityEngine;

public class DamageScreen : MonoBehaviour
{
    Animator Animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Animator = GetComponent<Animator>();
	}

    public void damageScreenOn()
    {
        Animator.SetTrigger("damage");
	}
}
