using System.Linq;
using UnityEngine;

public class PCA_MidLine : MonoBehaviour
{ 
    void OnDrawGizmos()
    {

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            // 提取物体的所有顶点
            Vector3[] vertices = meshFilter.sharedMesh.vertices;
            Vector3 center = Vector3.zero;

            // 计算几何中心（质心）
            foreach (Vector3 vertex in vertices)
            {
                center += vertex;
            }
            center /= vertices.Length;

            // 转换到世界空间位置
            center = transform.TransformPoint(center);

            // 获取PCA计算的主轴方向
            Vector3 mainDirection = PerformPCA(vertices);

            // 绘制中轴线
            Gizmos.color = Color.green;
            Gizmos.DrawLine(center, center + mainDirection * 5f); // 生成中轴线
        }
    }

    // 执行PCA算法，返回最重要的主轴方向
    Vector3 PerformPCA(Vector3[] vertices)
    {
        // 计算几何中心（物体的质心）
        Vector3 center = Vector3.zero;
        foreach (Vector3 vertex in vertices)
        {
            center += vertex;
        }
        center /= vertices.Length;

        // 计算每个顶点相对于中心的偏差
        var deviations = vertices.Select(v => v - center).ToArray();

        // 构造协方差矩阵
        Matrix3x3 covarianceMatrix = CalculateCovarianceMatrix(deviations);

        // 计算协方差矩阵的特征值和特征向量
        var eigen = EigenDecomposition(covarianceMatrix);

        // 特征向量对应着主轴，返回具有最大特征值的特征向量
        Vector3 mainAxis = eigen.Vectors.OrderByDescending(v => v.sqrMagnitude).First();
        return mainAxis;
    }

    // 计算协方差矩阵
    Matrix3x3 CalculateCovarianceMatrix(Vector3[] deviations)
    {
        Matrix3x3 matrix = new Matrix3x3();

        // 计算协方差矩阵（3x3）
        for (int i = 0; i < deviations.Length; i++)
        {
            Vector3 deviation = deviations[i];
            matrix[0, 0] += deviation.x * deviation.x;
            matrix[0, 1] += deviation.x * deviation.y;
            matrix[0, 2] += deviation.x * deviation.z;

            matrix[1, 0] += deviation.y * deviation.x;
            matrix[1, 1] += deviation.y * deviation.y;
            matrix[1, 2] += deviation.y * deviation.z;

            matrix[2, 0] += deviation.z * deviation.x;
            matrix[2, 1] += deviation.z * deviation.y;
            matrix[2, 2] += deviation.z * deviation.z;
        }

        // 协方差矩阵归一化
        float len = deviations.Length;
        matrix[0, 0] /= len;
        matrix[0, 1] /= len;
        matrix[0, 2] /= len;

        matrix[1, 0] /= len;
        matrix[1, 1] /= len;
        matrix[1, 2] /= len;

        matrix[2, 0] /= len;
        matrix[2, 1] /= len;
        matrix[2, 2] /= len;

        return matrix;
    }

    // 计算矩阵的特征值和特征向量
    // 这里我们简化为一种简单的特征向量解法
    (Vector3[] Vectors, float[] EigenValues) EigenDecomposition(Matrix3x3 m)
    {
        // Eigen decomposition 需要更复杂的数学操作，通常我们会使用第三方库来计算特征值/特征向量
        // 例如 MathNet.Numerics 或自己实现 Jacobi 算法等。为了简化起见，先返回假设的特征向量和特征值。
        // 你可以用现成的数学库（如 MathNet）来精确计算。

        Vector3[] vectors = { Vector3.right, Vector3.up, Vector3.forward }; // 假设的主轴
        float[] eigenValues = { 1f, 0.5f, 0.3f }; // 假设的特征值

        return (vectors, eigenValues);
    }
}

// 简单的 3x3 矩阵类
public class Matrix3x3
{
    private float[,] values = new float[3, 3];

    public float this[int row, int col]
    {
        get => values[row, col];
        set => values[row, col] = value;
    }
}