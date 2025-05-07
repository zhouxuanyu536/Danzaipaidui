void ColorBasedOnUV_float(float2 uv, float progress, float density, out float4 Out)
{
    // 归一化，uv 的值在 [0,1] 之间
    if (density < 2.0f)
    {
        density = 2.0f;
    }
    float divideDensity = 1.0f / density;
    float2 centerPoint = float2(0.0f, 0.0f);
    float distanceX = abs(centerPoint.x - uv.x);
    float distanceY = abs(centerPoint.y - uv.y);
    
    float distanceFromCenter = fmod(distanceX + distanceY + progress, 2.0f * divideDensity);

    if (distanceFromCenter < divideDensity)
    {
        Out = float4(0.0f, 0.0f, 0.0f, 1.0f);
    }
    else
    {
        Out = float4(1.0f, 1.0f, 1.0f, 1.0f);
    }
}

void RoundToTwoDecimals_float(float x, out float Out)
{
    Out = round(x * 100.0f) / 100.0f;
}

void ChangeColor_float(float In, float4 Dark, float4 Light, out float4 Out)
{
    if (In < 0.1f)
    {
        Out = Dark;
    }
    else
    {
        Out = Light;
    }
}

void SpeedTextureScaleRight_float(float2 uv, float offset, float scaleX, float scaleY,float4 darkColor, float4 lightColor, out float4 Out)
{
    float uv_Y = 0;
    if (uv.y < 0) 
    {
        uv_Y = -1.0f * uv.y * scaleY;
    }
    else
    {
        uv_Y = uv.y * scaleY; 
    }

    float modX = fmod(uv.x * scaleX + offset, 0.25f);
    if (modX < 0)
    {
        modX = modX + 0.25f;
    }
    if (uv_Y <= 0.3f)
    {
        if (modX < 0.125f && (4.0f * modX > 0.5f - uv_Y))
        {
            Out = darkColor;
        }
        else if ((modX < 0.125f && 4.0f * modX <= 0.5f - uv_Y) || (4.0f * (modX - 0.125f) > 0.5f - uv_Y))
        {
            Out = lightColor;
        }
        else
        {
            Out = darkColor;
        }
    }
    else
    {
        Out = lightColor;
    }
   
}

void SpeedTextureScaleLeft_float(float2 uv, float offset, float scaleX, float scaleY,float4 darkColor, float4 lightColor, out float4 Out)
{
    float uv_Y = 0;
    if (uv.y < 0)
    {
        uv_Y = -1.0f * uv.y * scaleY;
    }
    else
    {
        uv_Y = uv.y * scaleY;
    }

    float modX = fmod(uv.x * scaleX + offset, 0.25f);
    if (modX < 0)
    {
        modX = modX + 0.25f;
    }
    if (uv_Y <= 0.3f)
    {
        if (modX < 0.125f && (4.0f * modX > uv_Y))
        {
            Out = darkColor;
        }
        else if ((modX < 0.125f && 4.0f * modX <= uv_Y) || (4.0f * (modX - 0.125f) > uv_Y))
        {
            Out = lightColor;
        }
        else
        {
            Out = darkColor;
        }
    }
    else
    {
        Out = lightColor;
    }
   
}

void SpeedTextureScaleUp_float(float2 uv, float offset,float scaleX, float scaleY,float4 darkColor, float4 lightColor, out float4 Out)
{
    float uv_X = 0;
    if (uv.x < 0)
    {
        uv_X = -1.0f * uv.x * scaleX;
    }
    else
    {
        uv_X = uv.x * scaleX;
    }

    float modY = fmod(uv.y * scaleY + offset, 0.25f);
    if (modY < 0)
    {
        modY = modY + 0.25f;
    }
    if (uv_X <= 0.3f)
    {
        if (modY < 0.125f && (4.0f * modY > 0.5f - uv_X))
        {
            Out = darkColor;
        }
        else if ((modY < 0.125f && 4.0f * modY <= 0.5f - uv_X) || (4.0f * (modY - 0.125f) > 0.5f - uv_X))
        {
            Out = lightColor;
        }
        else
        {
            Out = darkColor;
        }
    }
    else
    {
        Out = lightColor;
    }
   
}

void SpeedTextureScaleDown_float(float2 uv, float offset, float scaleX, float scaleY,float4 darkColor, float4 lightColor, out float4 Out)
{
    float uv_X = 0;
    if (uv.x < 0)
    {
        uv_X = -1.0f * uv.x * scaleX;
    }
    else
    {
        uv_X = uv.x * scaleX;
    }

    float modY = fmod(uv.y * scaleY + offset, 0.25f);
    if (modY < 0)
    {
        modY = modY + 0.25f;
    }
    if (uv_X <= 0.3f)
    {
        if (modY < 0.125f && (4.0f * modY > uv_X))
        {
            Out = darkColor;
        }
        else if ((modY < 0.125f && 4.0f * modY <=  uv_X) || (4.0f * (modY - 0.125f) >  uv_X))
        {
            Out = lightColor;
        }
        else
        {
            Out = darkColor;
        }
    }
    else
    {
        Out = lightColor;
    }
   
}

void GroundColor_float(float2 uv,float MinValue,float MaxValue,float4 GroundColor,out float4 Out)
{
    //MinValue = max(MinValue, 1.0f);
    //MaxValue = min(MaxValue, 2.0f);
    //MinValue = min(MinValue, MaxValue); // Ensure MinValue is not greater than MaxValue
    
    if (((abs(uv.x) >= MinValue && abs(uv.x) <= MaxValue)
        || (abs(uv.y) >= MinValue && abs(uv.y) <= MaxValue)) && (abs(uv.x) <= MaxValue && abs(uv.y) <= MaxValue))
    {
        Out = GroundColor * 0.67f;

    }
    else
    {
        Out = GroundColor;
    }

}

void WallColor_float(float2 uv, float MinValue, float MaxValue, float4 GroundColor, out float4 Out)
{
    //MinValue = max(MinValue, 1.0f);
    //MaxValue = min(MaxValue, 2.0f);
    //MinValue = min(MinValue, MaxValue); // Ensure MinValue is not greater than MaxValue
    
    if (uv.x * uv.x + uv.y * uv.y >= MinValue * MinValue 
        && uv.x * uv.x + uv.y * uv.y <= MaxValue * MaxValue)   
    {
        Out = GroundColor * 0.67f;
    }
    else
    {
        Out = GroundColor;
    }
}
bool DoorColor_JudgeInside(float2 uv, float value)
{
    float centerY = (0.5 - (sqrt(3) / 6));
    
    float2 topT = float2(0, value + centerY * 75);
    float2 uvT = float2(uv.x, uv.y + centerY * 75);
    
    float2 topTN = normalize(topT);
    float2 uvTN = normalize(uvT);
    
    float angle = acos(dot(topTN, uvTN));
    float angleToDeg = degrees(angle) % 120;
    
    if (angleToDeg > 60)
    {
        angleToDeg = 120 - angleToDeg;
    }
    float supposedDist = value * cos(radians(60)) / cos(radians(60 - angleToDeg));
    if (uv.x * uv.x + (uv.y + centerY * 75) * (uv.y + centerY * 75) < supposedDist * supposedDist)
    {
        return true;
    }
    else
    {
        return false;
    }
}

void DoorColor_float(float2 uv, float MinValue, float MaxValue, float4 GroundColor, out float4 Out)
{
    if (!DoorColor_JudgeInside(uv, MinValue) && DoorColor_JudgeInside(uv, MaxValue))
    {
        Out = GroundColor * 0.67f;
    }
    else
    {
        Out = GroundColor;
    }
    
}

void TrailElevatorShaderGenerate_float(float MinValue, float MaxValue, float2 scale, float2 uv, float CubeColor, float CircleColor,out float4 Out)
{
    uv.x += 0.5f;
    uv.y += 0.5f;
    for (int i = 1; i < scale.x * 2.0f; i += 2)
    {
        for (int j = 1; j < scale.y * 2.0f; j += 2)
        {
            //生成椭圆圆心
            float xCenter = (float) i / (floor(scale.x) * 2.0f);
            float yCenter = (float) j / (floor(scale.y) * 2.0f);
            //对齐圆心（处于中间位置）
            //xCenter += (scale.x - floor(scale.x)) / (2.0f * scale.x);
            //yCenter += (scale.y - floor(scale.y)) / (2.0f * scale.y);
            //生成半长轴，半短轴
            float xRadius = 1.0f / (floor(scale.x) * 2.0f);
            float yRadius = 1.0f / (floor(scale.y) * 2.0f);
            float ValueOfEllipse = pow(uv.x - xCenter, 2) / pow(xRadius, 2) + pow(uv.y - yCenter, 2) / pow(yRadius, 2);
            if (ValueOfEllipse >= pow(MinValue, 2) && ValueOfEllipse <= pow(MaxValue, 2))  // 修正缩放
            {
                Out = CircleColor;
                return;
            }
        }
    }
    Out = CubeColor;
    
}

float inverseLerp(float a, float b, float value)
{
    return saturate((value - a) / (b - a));
}
void AddColorToTerrain_float(in float4 color0, in float4 color1, in float4 color2,
in float4 color3, in float4 color4, in float4 color5, in float4 color6, in float4 color7,
in float4 color8, in float4 color9,

in float baseHeight0, in float baseHeight1, in float baseHeight2,
in float baseHeight3, in float baseHeight4, in float baseHeight5, in float baseHeight6,
in float baseHeight7, in float baseHeight8, in float baseHeight9,

in float baseBlend0, in float baseBlend1, in float baseBlend2, in float baseBlend3,
in float baseBlend4, in float baseBlend5, in float baseBlend6, in float baseBlend7,
in float baseBlend8, in float baseBlend9,

in float baseColorStrength0, in float baseColorStrength1, in float baseColorStrength2, in float baseColorStrength3,
in float baseColorStrength4, in float baseColorStrength5, in float baseColorStrength6, in float baseColorStrength7,
in float baseColorStrength8, in float baseColorStrength9,

in float baseTextureScale0, in float baseTextureScale1, in float baseTextureScale2, in float baseTextureScale3,
in float baseTextureScale4, in float baseTextureScale5, in float baseTextureScale6, in float baseTextureScale7,
in float baseTextureScale8, in float baseTextureScale9,

in float heightPercent,

float testScale,

out float4 outColor)
{
    
    const static float epsilon = 1E-4; 
    float4 baseColors[10] =
    {
        color0, color1, color2, color3, color4,
        color5, color6, color7, color8, color9
    };

    float baseStartHeights[10] =
    {
        baseHeight0, baseHeight1, baseHeight2, baseHeight3, baseHeight4,
        baseHeight5, baseHeight6, baseHeight7, baseHeight8, baseHeight9
    };
    
    float baseBlends[10] =
    {
        baseBlend0, baseBlend1, baseBlend2, baseBlend3, baseBlend4,
        baseBlend5, baseBlend6, baseBlend7, baseBlend8, baseBlend9
    };
    outColor = float4(0, 0, 0, 0);
    for (int i = 0; i < 10; i++)
    {
        if (baseStartHeights[i] < -epsilon)
            continue;
        float drawStrength = inverseLerp(-baseBlends[i] / 2 - epsilon, baseBlends[i] / 2, heightPercent - baseStartHeights[i]);
        outColor *= (1 - drawStrength);
        outColor += baseColors[i] * drawStrength;
    }
}

