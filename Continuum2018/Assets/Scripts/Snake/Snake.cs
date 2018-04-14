using UnityEngine;

public class Snake : MonoBehaviour 
{
	public Animator SnakeAnim;
	public string[] SnakeAnimations;

	public void Start ()
	{
		int SnakeAnimIndex = Random.Range (0, SnakeAnimations.Length);
		SnakeAnim.Play (SnakeAnimations [SnakeAnimIndex]);
	}

	public void DestroySnake ()
	{
		Destroy (gameObject);
	}
}
