using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PerspectiveProjection : MonoBehaviour
{
    [SerializeField]
    private GameObject cameraOrigin;
    [SerializeField]
    private Camera trackedCamera;
    [SerializeField]
    private Camera displayCamera;
    [SerializeField]
    private Vector2Int resolution;
    [SerializeField, Tooltip("screen width in meters")]
    private float screenHeight;
    [SerializeField]
    private float scaleFactor;

    private List<Vector2> srcPts = new List<Vector2>();
    private List<Vector2> dstPts = new List<Vector2>();
    private Material material;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().sharedMaterial;
    }

    private void Start()
    {
        if (Application.isPlaying && trackedCamera != null && material != null && displayCamera != null && cameraOrigin != null)
        {
            InitializePerspectiveTransform();
        }
    }

    private void OnValidate()
    {
        InitializePerspectiveTransform();
    }

    private void Update()
    {
        UpdateDestinationPoints();
        SetPerspectiveTransformMatrix(PerspectiveTransform(srcPts, dstPts));
    }

    private void SetPerspectiveTransformMatrix(double[] M)
    {
        material.SetVector("_M0", new Vector3((float)M[0], (float)M[1], (float)M[2]));
        material.SetVector("_M1", new Vector3((float)M[3], (float)M[4], (float)M[5]));
        material.SetVector("_M2", new Vector3((float)M[6], (float)M[7], 1f));
    }

    private void InitializePerspectiveTransform()
    {
        CreateSourcePoints();
        SetScaleAndOrthographicSize();
    }

    private void CreateSourcePoints()
    {
        // Clear the list
        srcPts.Clear();
        // Create a list of 4 vectors with normalized coordinates of the screen's corners.
        // NOTE: the order of the values are provided "vertically flipped" to match UV coordinate system (origin at bottom left)
        srcPts.Add(new Vector2(0, 1));
        srcPts.Add(new Vector2(1, 1));
        srcPts.Add(new Vector2(0, 0));
        srcPts.Add(new Vector2(1, 0));
    }

    private void UpdateDestinationPoints()
    {
        // Clear the list to only hold current values
        dstPts.Clear();

        // Create a list of normalized screen corners position in world space
        var points = new List<Vector3>
        {
            new Vector3(-0.5f, 0.5f, 0),
            new Vector3(0.5f, 0.5f, 0),
            new Vector3(-0.5f, -0.5f, 0),
            new Vector3(0.5f, -0.5f, 0)
        };

        // Fill a list of destination points with the screen corners coordinates in camera space
        foreach (var point in points)
        {
            dstPts.Add(ScreenPointToNormalized(trackedCamera.WorldToScreenPoint(transform.TransformPoint(point))));
        }
    }

    private Vector2 ScreenPointToNormalized(Vector3 screenPoint)
    {
        return new Vector2(screenPoint.x / resolution.x, screenPoint.y / resolution.y);
    }

    private void SetScaleAndOrthographicSize()
    {
        // Perform rescaling of local transform and camera origin based on inspector properties
        transform.localScale = new Vector3(scaleFactor * screenHeight * (float)resolution.x / (float)resolution.y, screenHeight * scaleFactor, screenHeight * scaleFactor);
        displayCamera.orthographicSize = screenHeight * scaleFactor / 2;
        if (cameraOrigin != null)
        {
            cameraOrigin.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
    }

    // Perspective Transform
    private double[] PerspectiveTransform(List<Vector2> src, List<Vector2> dst)
    {
        double[,] A = new double[8, 8];
        double[] B = new double[8];
        double[] X;

        // Algorithm to populate the A and B arrays is based on the OpenCV library getPerspectiveTransform code from (https://github.com/opencv/opencv/blob/4.x/modules/imgproc/src/imgwarp.cpp)
        for (int i = 0; i < 4; i++)
        {
            A[i, 0] = A[i + 4, 3] = src[i].x;
            A[i, 1] = A[i + 4, 4] = src[i].y;
            A[i, 2] = A[i + 4, 5] = 1;
            A[i, 3] = A[i, 4] = A[i, 5] = A[i + 4, 0] = A[i + 4, 1] = A[i + 4, 2] = 0;
            A[i, 6] = -src[i].x * dst[i].x;
            A[i, 7] = -src[i].y * dst[i].x;
            A[i + 4, 6] = -src[i].x * dst[i].y;
            A[i + 4, 7] = -src[i].y * dst[i].y;
            B[i] = dst[i].x;
            B[i + 4] = dst[i].y;
        }

        // Perform Gaussian Elimination
        X = GaussianElimination(A, B);

        return X;
    }

    // Gaussian Elimination
    private double[] GaussianElimination(double[,] A, double[] B)
    {
       var N = B.Length;

        for (int p = 0; p < N; p++)
        {
            // find pivot row and swap
            int max = p;
            for (int i = p + 1; i < N; i++)
            {
                if (Math.Abs(A[i, p]) > Math.Abs(A[max, p]))
                {
                    max = i;
                }
            }
            double[] temp = new double[N];
            for (int i = 0; i < N; i++)
            {
                temp[i] = A[p, i];
                A[p, i] = A[max, i];
                A[max, i] = temp[i];
            }
            double t = B[p];
            B[p] = B[max];
            B[max] = t;

            // singular or nearly singular
            if (Math.Abs(A[p, p]) <= double.Epsilon)
            {
                throw new InvalidOperationException("Matrix is singular or nearly singular");
            }

            // pivot within A and B
            for (int i = p + 1; i < N; i++)
            {
                double alpha = A[i, p] / A[p, p];
                B[i] -= alpha * B[p];
                for (int j = p; j < N; j++)
                {
                    A[i, j] -= alpha * A[p, j];
                }
            }
        }

        // back substitution
        double[] X = new double[N];
        for (int i = N - 1; i >= 0; i--)
        {
            double sum = 0.0;
            for (int j = i + 1; j < N; j++)
            {
                sum += A[i, j] * X[j];
            }
            X[i] = (B[i] - sum) / A[i, i];
        }
        return X;
    }
}
