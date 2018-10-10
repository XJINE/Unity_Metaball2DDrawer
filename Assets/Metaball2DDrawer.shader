Shader "Hidden/Metaball2DDrawer"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            
            #include "UnityCG.cginc"

            #pragma target 5.0
            #pragma vertex vert_img
            #pragma fragment frag

            struct Metaball
            {
                float  radius;
                float2 position;
                float2 direction;
                float4 color;
            };

            StructuredBuffer<Metaball> _MetaballBuffer;

            static const fixed4 transparent = fixed4(0.0, 0.0, 0.0, 0.0);

            void drawMetaball(float2 inputPos, float2 centerPos, float radius, float power,
                              fixed4 color, float aspectRatio, inout fixed4 destination)
            {
                inputPos.x  *= aspectRatio;
                centerPos.x *= aspectRatio;

                float  lng      = length(inputPos - centerPos);
                float  strength = power * radius / lng;
                
                destination += color * strength;

                if (1 < destination.w)
                {
                    destination = color;
                }
            }

            fixed4 frag (v2f_img input) : SV_Target
            {
                fixed4 destination = transparent;
                float  aspectRatio = _ScreenParams.x / _ScreenParams.y;

                Metaball metaball;

                for (int i = 0; i < (int)_MetaballBuffer.Length; i++)
                {
                    metaball = _MetaballBuffer[i];

                    drawMetaball(input.uv,
                                 metaball.position,
                                 metaball.radius,
                                 1,
                                 metaball.color,
                                 aspectRatio,
                                 destination);
                }

                if (destination.w < 1)
                {
                    destination = transparent;
                    //discard;
                }

                return destination;
            }

            ENDCG
        }
    }
}