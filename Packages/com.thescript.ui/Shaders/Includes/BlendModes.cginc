#ifndef BLEND_MODES_CGINC
#define BLEND_MODES_CGINC

fixed4 Add(fixed4 src, fixed4 dst)
{
    return src + dst;
}

fixed4 Darken(fixed4 src, fixed4 dst)
{
    fixed4 r = min(src, dst);
    r.a = dst.a;
    return r;
}

fixed4 Lighten(fixed4 src, fixed4 dst)
{
    fixed4 r = max(src, dst);
    r.a = dst.a;
    return r;
}

fixed4 Overlay(fixed4 src, fixed4 dst)
{
    fixed4 r = src < 0.5 ? 2.0 * src * dst : 1.0 - 2.0 * (1.0 - src) * (1.0 - dst);
    r.a = dst.a;
    return r;
}

fixed4 Screen(fixed4 src, fixed4 dst)
{
    fixed4 r = 1.0 - (1.0 - src) * (1.0 - dst);
    r.a = dst.a;
    return r;
}

fixed4 ColorDodge(fixed4 src, fixed4 dst)
{
    fixed4 r = src / (1.0 - dst);
    r.a = dst.a;
    return r;
}

fixed4 Multiply(fixed4 src, fixed4 dst)
{
    fixed4 r = src * dst;
    r.a = dst.a;
    return r;
}

fixed4 ColorBurn(fixed4 src, fixed4 dst)
{
    fixed4 r = 1.0 - (1.0 - dst) / src;
    r.a = dst.a;
    return r;
}

fixed4 LinearBurn(fixed4 src, fixed4 dst)
{
    fixed4 r = src + dst - 1.0;
    r.a = dst.a;
    return r;
}

fixed4 Exclusion(fixed4 src, fixed4 dst)
{
    fixed4 r = 0.5 - 2.0 * (src - 0.5) * (dst - 0.5);
    r.a = dst.a;
    return r;
}

fixed4 AlphaBlend(fixed4 src, fixed4 dst)
{
    fixed a = src.a + dst.a * (1.0 - src.a);
    fixed4 r = (src * src.a + dst * dst.a * (1.0 - src.a)) / a;
    r.a = a;
    return r; 
}

fixed4 ColorBlend(fixed4 src, fixed4 dst)
{
    const float3x3 toYUVMatrix = float3x3(0.299, 0.587, 0.114, -0.14713, -0.28886, 0.436, 0.615, -0.51499, -0.10001);
    const float3x3 toRGBMatrix = float3x3(1, 0, 1.28033, 1, -0.21482, -0.38059, 1, 2.12798, 0);

    fixed4 r = dst;
    r.rgb = mul(toYUVMatrix, dst.rgb);
    r.r = mul(toYUVMatrix, src.rgb).r;
    r.rgb = mul(toRGBMatrix, r.rgb);
    return r;
}

#endif