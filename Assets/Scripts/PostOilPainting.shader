// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/PostOilPainting"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Texture", 2D) = "white" {}
        _NoiseStrength ("Noise Strength", float) = 0.3
        _NoiseSize ("Noise Size", float) = 0.3
		_KernelSize("Kernel Size (N)", Int) = 5
    }
    SubShader
    {
        Tags { "LightMode" = "ForwardBase" "RenderType" = "TransparentCutout"}
        
        Pass
        {
            ZTest Always
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _MainTex;
			float2 _MainTex_TexelSize;
			float4 _MainTex_ST;
			
			sampler2D _NoiseTex;
			float2 _NoiseTex_TexelSize;
			float4 _NoiseTex_ST;
			float _NoiseStrength;
			float _NoiseSize;
            
			int _KernelSize;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
 
			struct vertexOutput
			{
				half2 texcoord : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			

			struct region
			{
				float3 mean;
				float variance;
			};
			
			fixed3 random (float2 uv)
            {
                return fixed3(frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123), 
                frac(sin(dot(uv,float2(16.998,7.93)))*3457.953123), 
                frac(sin(dot(uv,float2(45.998,143.253)))*358.5483));
            }
			
			region EvaluateR(int2 lower, int2 upper, int samples, float2 uv)
			{
				region r;
				float3 sum = 0.0;
				float3 squareSum = 0.0;

				for (int x = lower.x; x <= upper.x; ++x)
				{
					for (int y = lower.y; y <= upper.y; ++y)
					{
						float2 offset = float2(_MainTex_TexelSize.x * x, _MainTex_TexelSize.y * y);
						//float3 tex = tex2D(_MainTex, uv + offset);
						
		
						float3 tex = tex2D(_MainTex, uv + offset) + _NoiseStrength * tex2D(_MainTex, uv + offset)*(tex2D(_NoiseTex, _NoiseSize * uv + offset).r-0.5);
						sum += tex;
						squareSum += tex * tex;
					}
				}

				r.mean = sum / samples;
				float3 variance = abs((squareSum / samples) - (r.mean * r.mean));
				r.variance = length(variance);

				return r;
			}
			
			vertexOutput vert(vertexInput Input)
			{
				vertexOutput Output;
 
				Output.vertex = UnityObjectToClipPos(Input.vertex);
				Output.texcoord = Input.texcoord;
				Output.color = Input.color;
 
				return Output;
			}

            
            fixed4 frag (vertexOutput Input) : COLOR
            {
                float2 uv = Input.texcoord.xy;
				
				// Kuwahara-----------------------------------------------------------------
				int upper = (_KernelSize - 1) / 2;
				int lower = -upper;

				int regionNum = (upper + 1) * (upper + 1);

				// Calculate the four regional parameters as discussed.
				region r1 = EvaluateR(int2(lower, lower), int2(0, 0), regionNum, uv);
				region r2 = EvaluateR(int2(0, lower), int2(upper, 0), regionNum, uv);
				region r3 = EvaluateR(int2(lower, 0), int2(0, upper), regionNum, uv);
				region r4 = EvaluateR(int2(0, 0), int2(upper, upper), regionNum, uv);

				fixed3 oColor = r1.mean;
				fixed minVariance = r1.variance;

				// Compare and get the smallest varience
				if (minVariance > r2.variance) {
				    oColor = r2.mean;
				    minVariance = r2.variance;
				}
				if (minVariance > r3.variance) {
				    oColor = r3.mean;
				    minVariance = r3.variance;
				}
				if (minVariance > r4.variance) {
				    oColor = r4.mean;
				    minVariance = r4.variance;
				}
				
                
                // Kuwahara-----------------------------------------------------------------
                //oColor = tex2D(_MainTex, i.uv).rgb;
                // diffuse term
                /*fixed3 emission = oColor * _Emission;
                fixed3 diffuse = _Diffuse * _LightColor0.rgb * oColor * saturate(dot(worldNormal, worldLight));
				
				fixed4 color = fixed4(ambient + emission + diffuse, alpha);
				clip( alpha - 0.5 );*/
				return fixed4(oColor, 1);
            }
            ENDCG
        }
    }
}
