using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Nullspace
{
    public class MannulDraw : MonoBehaviour
    {
        // DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer);
        private Mesh mesh;
        private Material material;
        private bool isOpaque;
        private Renderer render;
        private void Awake()
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;
            render = GetComponent<Renderer>();
            material = render.material;
            isOpaque = material.shader.renderQueue < 3000;
        }

        public void DrawMesh()
        {
            Graphics.DrawMesh(mesh, transform.localToWorldMatrix, material, gameObject.layer);
        }

        private void OnEnable()
        {
            if (!MannulDrawManager.IsDestroy())
            {
                MannulDrawManager.Instance.AddObject(this);
            }
        }

        private void OnDisable()
        {
            if (!MannulDrawManager.IsDestroy())
            {
                MannulDrawManager.Instance.RemoveObject(this);
            }
        }
    }

}
