using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	private Rigidbody2D rbody;
	[SerializeField] private GameObject throwCollider = null;
	[SerializeField] private GameObject platformCollider = null;

	// Start is called before the first frame update
	void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
	}

	void Paralize()
	{

	}

	public void Retrieve()
	{
		//fade animation
		//fade sound
		Destroy(gameObject);
	}

	void Damage()
	{
		Destroy(gameObject);
	}

	void Attach()
	{

	}
    // Update is called once per frame
    void Update()
    {
        
    }

	void OnCollisionEnter2D(Collision2D other)
	{
		rbody.constraints = RigidbodyConstraints2D.FreezeAll;
		throwCollider.SetActive(false);
		platformCollider.SetActive(true);
	}
}
