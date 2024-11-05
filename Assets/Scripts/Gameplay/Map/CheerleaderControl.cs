using System;
using UnityEngine;
using Random = System.Random;

namespace Gameplay.Map
{
    public class CheerleaderControl : MonoBehaviour
    {
        [SerializeField] private Animation anim;
        [SerializeField] private MeshRenderer mesh;

        private void Awake()
        {
            anim[anim.clip.name].normalizedTime = UnityEngine.Random.Range(0f, 1f);
            mesh.material.color = UnityEngine.Random.ColorHSV();
        }
    }
}
