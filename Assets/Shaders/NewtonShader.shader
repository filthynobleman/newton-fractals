Shader "Custom/NewtonShader"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _NormalStrength ("Bump Intensity", Range(0, 1)) = 0.1

        _NumIters ("Number of Iterations", Integer) = 32
        _Power ("Number of Roots", Integer) = 3
        _Scale ("Scale", Float) = 1.0
        _Seed ("Color Set", Integer) = 0

        _XOff ("X Axis Offset", Float) = 0.0
        _YOff ("Y Axis Offset", Float) = 0.0
        _ZOff ("Z Axis Offset", Float) = 0.0
        _W ("Fourth Dimension", Float) = 0.0

        _Theta ("Convergence Gradient Speed", Float) = 3.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float4 z;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        int _NumIters;
        int _Power;
        float _Scale;
        float _W;

        float _XOff;
        float _YOff;
        float _ZOff;

        float _Theta;

        // Conversion from a coordinate system based on 4 real values to
        // an orthogonal basis with complex coefficients
        float4 r4_to_c2(float4 r4)
        {
            return float4(r4.x + r4.w,
                          r4.y - r4.z,
                          r4.x - r4.w,
                          r4.y + r4.z);
        }

        // Vertex shader passes the 3D position of the vertices, modified
        // according to the input proprties, and its conversion to the
        // orthogonal basis.
        // Notice passing the orthogonal basis in vertex shader is perfectly
        // fine, since the transformation is linear and preserves interpolation.
        // In this way, we save some computation.
        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.worldPos = _Scale * v.vertex.xyz;
            float4 Off = _Scale * float4(_XOff, _YOff, _ZOff, _W);
            o.z = r4_to_c2(float4(o.worldPos, 0.0f) + Off);
        }






        // Absolute value of a bicomplex number. When expressed in
        // the orthogonal basis, the absolute value is half the sum
        // of the square of the coordinates.
        float bcabs(float4 z)
        {
            return 0.5f * dot(z, z);
        }

        // Multiplication between complex numbers.
        float2 cmul(float2 c1, float2 c2)
        {
            return float2(c1.x * c2.x - c1.y * c2.y,
                c1.x * c2.y + c1.y * c2.x);
        }

        // Integer power of a complex number
        float2 cpow(float2 c, int k)
        {
            float rho2 = dot(c, c);
            float theta = atan2(c.y, c.x);
            rho2 = pow(rho2, 0.5 * k);
            return rho2 * float2(cos(k * theta), sin(k * theta));
        }

        // Integer power of a bicomplex number
        float4 bcpow(float4 z, int k)
        {
            float2 ep = float2(z.x, z.y);
            float2 em = float2(z.z, z.w);
            ep = cpow(ep, k);
            em = cpow(em, k);
            return float4(ep.x, ep.y, em.x, em.y);
        }

        // Single step of the Newton-Raphson method for solving the
        // polinomial z^n - 1 = 0
        float4 newton_step(float4 z, int power)
        {
            return 1.0f / power * ((power - 1) * z + bcpow(z, 1 - power));
        }

        // Data structure for returning a 4D point and the convergence time
        struct newton_res
        {
            float4 sol;
            float time;
        };

        // Newton-Raphson method for solving the polynomial z^n - 1 = 0.
        // The iteration runs to convergence and at the same time computes
        // the convergence time (i.e. how fast the point converged to a solution)
        newton_res newton(float4 z, int niters, int power)
        {
            newton_res res;
            res.sol = z;
            for (int i = 0; i < niters; ++i)
            {
                res.sol = newton_step(z, power);
                float delta = bcabs(res.sol - z);
                z = res.sol;
                res.time += 1.0f / (1 + exp(delta + _Theta) - exp(_Theta));
            }
            res.time = (niters - res.time) / niters;
            return res;
        }

        // Find the solution of the polynomial z^n - 1 = 0 which is
        // the closest to the given point.
        float closest_solution(float4 z, int power)
        {
            int sol = 0;
            float diff = 1.0e8;
            for (int i = 0; i < power; ++i)
            {
                float theta = 2 * 3.14159265 * i / power;
                float2 alpha = float2(cos(theta), sin(theta));

                for (int j = 0; j < power; ++j)
                {
                    float phi = 2 * 3.14159265 * j / power;
                    float2 beta = float2(cos(phi), sin(phi));

                    float4 root = float4(alpha.x, alpha.y, beta.x, beta.y);
                    float d = dot(z - root, z - root);
                    if (d < diff)
                    {
                        sol = i * power + j;
                        diff = d;
                    }
                }
            }
            return float(sol) / float(power * power - 1);
        }


        // Random utilities for producing random colors from floats.
        int _Seed;
        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
        {
            float randomno = frac(sin(dot(Seed, float2(12.9898, 78.233))) * 43758.5453);
            Out = lerp(Min, Max, randomno);
        }
        float3 hash_float_to_color(float x)
        {
            float3 res;
            float2 seed = (x + _Seed) * float2(1.0f, 1.0f);
            Unity_RandomRange_float(seed, 0.0f, 1.0f, res.x);
            Unity_RandomRange_float(seed + 100, 0.0f, 1.0f, res.y);
            Unity_RandomRange_float(seed + 200, 0.0f, 1.0f, res.z);
            return res;
        }


        // Pixel shader computing the Newton-Raphson method for each pixel and
        // assigning a color depending on which solution the point converged to.
        // The convergence speed is used as a bump map.
        float _NormalStrength;
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Newton
            newton_res z = newton(IN.z, _NumIters, _Power);
            float sol = closest_solution(z.sol, _Power);

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = hash_float_to_color(sol);// *(1 - z.time) + float3(1.0f, 1.0f, 1.0f) * z.time;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            float bump = -2 * z.time - 1.0f;
            bump *= _NormalStrength;
            bump *= 0.5f;
            o.Normal = UnpackNormal(.5 + bump);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
