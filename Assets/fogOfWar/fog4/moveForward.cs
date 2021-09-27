using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveForward : MonoBehaviour, Iunit
{
    public int height { get => _height; }
    public Vector3 pos { get => transform.position; }
    public float viewRadius { get => _viewRadius; }
    [SerializeField] float speed = 5;
    public float _viewRadius = 5;
    public int _height = 0;
    void Awake() => _viewRadius = Random.value * 70;
    void Update() => transform.Translate(speed * Time.deltaTime * Vector3.forward);
    //private void OnDrawGizmos() => Gizmos.DrawWireSphere(transform.position, _viewRadius);
}
