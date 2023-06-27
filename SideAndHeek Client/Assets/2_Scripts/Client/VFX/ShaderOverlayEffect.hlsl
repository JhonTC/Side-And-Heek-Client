// PositionTexture
// EffectRadius
// WorldPosition

#ifndef ShaderOverlayEffect_INCLUDED
#define ShaderOverlayEffect_INCLUDED

float2 PixelToUV_float(float PixelX, float PixelY, float2 TextureSize)
{
    float uvX = (PixelX + 0.5) / TextureSize.x;
    float uvY = (PixelY + 0.5) / TextureSize.y;
	
	return float2(uvX, uvY);
}

float4 Sample_float(UnityTexture2D PositionTexture, UnitySamplerState samplerState, float2 UV, float multiplier = 10)
{
    float4 sampledValue = PositionTexture.Sample(samplerState, UV);
    sampledValue.x = floor(sampledValue.x * multiplier);
    sampledValue.y = floor(sampledValue.y * multiplier);
    sampledValue.z = floor(sampledValue.z * multiplier);
    
    return sampledValue;
}

void EffectOverlayLoop_float(UnityTexture2D PositionTexture, float2 TextureSize, float EffectRadius, float3 WorldPosition, UnitySamplerState samplerState, out float3 ClosestVector, out float Distance)
{
    float closestDistance = 100000;
    float3 closestPosition = float3(0, 0, 0);
	
    for (int i = 0; i < TextureSize.y; i++) 
	{
        float4 position = Sample_float(PositionTexture, samplerState, float2(PixelToUV_float(0, i, TextureSize)));
        //PositionTexture.Sample(samplerState, float2(PixelToUV_float(0, i, TextureSize)));
        float4 decimalIndex = Sample_float(PositionTexture, samplerState, float2(PixelToUV_float(TextureSize.x - 1, i, TextureSize)));
        //PositionTexture.Sample(samplerState, float2(PixelToUV_float(TextureSize.x - 1, i, TextureSize)));
        float4 sign = Sample_float(PositionTexture, samplerState, float2(PixelToUV_float(TextureSize.x - 2, i, TextureSize)), 1);
        //PositionTexture.Sample(samplerState, float2(PixelToUV_float(TextureSize.x - 2, i, TextureSize)));
        
        //position *= 10;
        //decimalIndex *= 10;
        
        for (int j = 1; j < TextureSize.x - 2; j++)
        {
            float4 positionDigit = Sample_float(PositionTexture, samplerState, float2(PixelToUV_float(j, i, TextureSize)));
            //PositionTexture.Sample(samplerState, float2(PixelToUV_float(j, i, TextureSize)));
            //positionDigit *= 10;

            position *= 10;
            position += positionDigit;
        }
        position.x /= pow(10, TextureSize.x - 2 - decimalIndex.x);
        position.y /= pow(10, TextureSize.x - 2 - decimalIndex.y);
        position.z /= pow(10, TextureSize.x - 2 - decimalIndex.z);
        
        sign = (sign - float4(0.5f, 0.5f, 0.5f, 0)) * 2;
        position.x *= sign.x;
        position.y *= sign.y;
        position.z *= sign.z;
		
        float dist = distance(position.xyz, WorldPosition);
        if (dist < closestDistance)
        {
            closestDistance = dist;
            closestPosition = position;
        }
    }

    Distance = closestDistance;
    ClosestVector = closestPosition - WorldPosition;
}



#endif //ShaderOverlayEffect_INCLUDED


