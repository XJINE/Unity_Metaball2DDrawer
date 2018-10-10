using System.Runtime.InteropServices;
using UnityEngine;

public class Metaball2DDrawer : MonoBehaviour
{
    #region Struct

    public struct Metaball
    {
        public float   radius;
        public Vector2 position;
        public Vector2 direction;
        public Vector4 color;
    }

    #endregion Struct

    #region Field

    public int     metaballCount = 30;
    public Vector2 metaballRadiusRange    = new Vector2(0.001f, 0.002f);
    public Vector2 metaballDirectionRange = new Vector2(0.001f, 0.005f);
    public Color   metaballColor = Color.blue;

    public  Shader   metaballShader;
    private Material metaballMaterial;

    private ComputeBuffer metaballBuffer;
    private Metaball[]    metaballs;

    #endregion Field

    #region Method

    void Start()
    {
        Initialize();
    }
    
    void Update()
    {
        Metaball metaball;

        for (int i = 0; i < this.metaballs.Length; i++)
        {
            metaball = this.metaballs[i];
            metaball.position += metaball.direction;

            if (metaball.position.x < 0 || 1 < metaball.position.x)
            {
                metaball.direction.x *= -1;
            }
            if (metaball.position.y < 0 || 1 < metaball.position.y)
            {
                metaball.direction.y *= -1;
            }

            this.metaballs[i] = metaball;
        }

        this.metaballBuffer.SetData(this.metaballs);
        this.metaballMaterial.SetBuffer("_MetaballBuffer", this.metaballBuffer);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, this.metaballMaterial);
    }

    void OnDestroy()
    {
        GameObject.Destroy(this.metaballMaterial);
        this.metaballBuffer.Dispose();
    }

    void Initialize()
    {
        this.metaballMaterial = new Material(this.metaballShader);
        this.metaballBuffer   = new ComputeBuffer(this.metaballCount, Marshal.SizeOf(typeof(Metaball)));
        this.metaballs        = new Metaball[this.metaballBuffer.count];

        for (int i = 0; i < this.metaballs.Length; i++)
        {
            this.metaballs[i] = new Metaball()
            {
                radius    = Random.Range(this.metaballRadiusRange.x, this.metaballRadiusRange.y),
                position  = new Vector2(Random.value, Random.value),
                direction = new Vector2(Random.Range(this.metaballDirectionRange.x,
                                                     this.metaballDirectionRange.y) * (Random.value < 0.5 ? -1 : 1),
                                        Random.Range(this.metaballDirectionRange.x,
                                                     this.metaballDirectionRange.y) * (Random.value < 0.5 ? -1 : 1)),
                color     = this.metaballColor,
            };
        }

        this.metaballBuffer.SetData(this.metaballs);
        this.metaballMaterial.SetBuffer("_MetaballBuffer", this.metaballBuffer);
    }

    #endregion Method
}