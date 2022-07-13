using System.Collections.Generic;
using UnityEngine;

public class GrassPhysics : MonoBehaviour
{
    [SerializeField] private List<Transform> tramplers;

    private Transform[] trampleTransforms;

    [SerializeField, Range(0.001f, 1f)] private float trampleCutoff = 0.1f;

    [SerializeField, Range(0.001f, 1f)] private float elasticity = 0.01f;

    [SerializeField] private Material grassMat;
    [SerializeField] private ComputeShader shader;
    //[SerializeField] private Material debugMat;

    private readonly int textureWidth = 256;
    private readonly int textureHeight = 256;

    private int updateKernel;
    private RenderTexture renderTex;
    private ComputeBuffer trampleBuffer;
    private ComputeBuffer tramplerObjectsBuffer;

    private Vector4[] tramplerData;
    private Vector2[] previousPos;

    private void Start()
    {
        trampleTransforms = tramplers.ToArray();
        tramplerData = new Vector4[trampleTransforms.Length];
        previousPos = new Vector2[trampleTransforms.Length];

        // Create a rendertexture, for reading results.
        renderTex = new RenderTexture(textureHeight, textureHeight, 24);
        renderTex.enableRandomWrite = true;
        renderTex.wrapMode = TextureWrapMode.Clamp;
        renderTex.Create();

        // Create the compute buffer, for storing previous values.
        Vector4[] bufferData = new Vector4[textureWidth * textureHeight];
        trampleBuffer = new ComputeBuffer(bufferData.Length, 16); // 16 is 4 bytes for 4 floats

        // Sets the texture and buffer for the update kernel.
        updateKernel = shader.FindKernel("UpdateTrample");

        // Set the texture and buffer in the compute shader.
        shader.SetTexture(updateKernel, "Result", renderTex);
        shader.SetBuffer(updateKernel, "trampleBuffer", trampleBuffer);

        // Set some grass/trample settings in the compute shader.
        shader.SetFloat("width", textureWidth);
        shader.SetFloat("height", textureHeight);
        shader.SetFloat("trampleCutoff", trampleCutoff);
        shader.SetFloat("elasticity", elasticity);

        // Set the result texture in the grass material.
        grassMat.SetTexture("_TrampleMap", renderTex);

        // Set the debug texture
        //if (debugMat != null)
        //    debugMat.SetTexture("_BaseMap", renderTex);
    }

    private void Update()
    {
        for (int i = 0; i < trampleTransforms.Length; i++)
        {
            Player player = trampleTransforms[i].GetComponent<Player>();
            if (player != null && !player.IsGrounded)
                continue;

            // These calculations are used to get the correct UV values from the world positions. 
            // Should be fixed later on.
            Vector2 tramplePos = new Vector2(trampleTransforms[i].position.x, trampleTransforms[i].position.z);
            tramplePos /= 100f;
            tramplePos.x += 0.5f;
            tramplePos.y += 0.5f;
            Vector2 moveDir = tramplePos - previousPos[i];

            tramplerData[i] = new Vector4(1f - tramplePos.x, 1f - tramplePos.y, moveDir.x, moveDir.y);
            previousPos[i] = tramplePos;
        }

        tramplerObjectsBuffer = new ComputeBuffer(tramplerData.Length, 16); // 16 is 4 bytes for 4 floats
        tramplerObjectsBuffer.SetData(tramplerData);
        shader.SetBuffer(updateKernel, "tramplerObjects", tramplerObjectsBuffer);

        shader.Dispatch(updateKernel, textureWidth / 8, textureHeight / 8, 1);

        tramplerObjectsBuffer.Dispose();
    }

    private void OnDestroy() { trampleBuffer?.Dispose(); }
}
