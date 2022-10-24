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

    void Start()
    {
        if (Application.isPlaying)
        {
            // Variable needed later to reference vectors in the WarpPerspective shader and assign matrix transformation coefficients
            material = GetComponent<MeshRenderer>().sharedMaterial;

            // Crate a list of 4 vectors with normalized coordinates of the screen's corners.
            // NOTE: the order of the values are provided "vertically flipped" to match UV coordinate system (origin at bottom left)
            srcPts.Add(new Vector2(0, 1));
            srcPts.Add(new Vector2(1, 1));
            srcPts.Add(new Vector2(0, 0));
            srcPts.Add(new Vector2(1, 0));

            // Perform rescaling of local transform and camera origine based on inspector properties
            transform.localScale = new Vector3(scaleFactor * screenHeight * (float)resolution.x / (float)resolution.y, screenHeight * scaleFactor, screenHeight * scaleFactor);
            displayCamera.orthographicSize = screenHeight * scaleFactor / 2;
            cameraOrigin.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
    }

    private void OnValidate()
    {
        // Variable needed later to reference vectors in the WarpPerspective shader and assign matrix transformation coefficients
        material = GetComponent<MeshRenderer>().sharedMaterial;

        // Crate a list of 4 vectors with normalized coordinates of the screen's corners.
        // NOTE: the order of the values are provided "vertically flipped" to match UV coordinate system (origin at bottom left)
        srcPts.Add(new Vector2(0, 1));
        srcPts.Add(new Vector2(1, 1));
        srcPts.Add(new Vector2(0, 0));
        srcPts.Add(new Vector2(1, 0));

        // Perform rescaling of local transform and camera origine based on inspector properties
        transform.localScale = new Vector3(scaleFactor * screenHeight * (float)resolution.x / (float)resolution.y, screenHeight * scaleFactor, screenHeight * scaleFactor);
        displayCamera.orthographicSize = screenHeight * scaleFactor / 2;
        cameraOrigin.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    void Update()
    {
        // Check captureCamera is different from null
        if (trackedCamera != null)
        {
            // Clear the list to only hold current values
            dstPts.Clear();
            // Create a list of destination points
            dstPts.Add(new Vector2(trackedCamera.WorldToScreenPoint(transform.TransformPoint(new Vector3((float)-0.5,(float)0.5,0))).x / resolution.x, trackedCamera.WorldToScreenPoint(transform.TransformPoint(new Vector3((float)-0.5, (float)0.5, 0))).y / resolution.y));
            dstPts.Add(new Vector2(trackedCamera.WorldToScreenPoint(transform.TransformPoint(new Vector3((float)0.5, (float)0.5, 0))).x / resolution.x, trackedCamera.WorldToScreenPoint(transform.TransformPoint(new Vector3((float)0.5, (float)0.5, 0))).y / resolution.y));
            dstPts.Add(new Vector2(trackedCamera.WorldToScreenPoint(transform.TransformPoint(new Vector3((float)-0.5, (float)-0.5, 0))).x / resolution.x, trackedCamera.WorldToScreenPoint(transform.TransformPoint(new Vector3((float)-0.5, (float)-0.5, 0))).y / resolution.y));
            dstPts.Add(new Vector2(trackedCamera.WorldToScreenPoint(transform.TransformPoint(new Vector3((float)0.5, (float)-0.5, 0))).x / resolution.x, trackedCamera.WorldToScreenPoint(transform.TransformPoint(new Vector3((float)0.5, (float)-0.5, 0))).y / resolution.y));
            WarpPerspective(srcPts, dstPts);
        }
    }

    private void WarpPerspective(List<Vector2> src, List<Vector2> dst)
    {
        Vector B = new Vector(8);
        Matrix A = new Matrix(8, 8);

        // Below algorithm to populate the A and B is based on the OpenCV library getPerspectiveTransform code from (https://github.com/opencv/opencv/blob/4.x/modules/imgproc/src/imgwarp.cpp)
        for (int i = 0; i < 4; i++)
        {
            A[i, 0] = A[i+4, 3] = src[i].x;
            A[i, 1] = A[i+4, 4] = src[i].y;
            A[i, 2] = A[i+4, 5] = 1;
            A[i, 3] = A[i, 4] = A[i, 5] = A[i+4, 0] = A[i+4, 1] = A[i+4, 2] = 0;
            A[i, 6] = -src[i].x*dst[i].x;
            A[i, 7] = -src[i].y*dst[i].x;
            A[i+4, 6] = -src[i].x*dst[i].y;
            A[i+4, 7] = -src[i].y*dst[i].y;
            B[i] = dst[i].x;
            B[i+4] = dst[i].y;
        }

        // Perform Gaussian Elimination
        A.ElimPartial(B);
        
        // Create three vectors to hold the rows of generated transformation matrix
        Vector3 M0 = new Vector3((float)B[0], (float)B[1], (float)B[2]);
        Vector3 M1 = new Vector3((float)B[3], (float)B[4], (float)B[5]);
        Vector3 M2 = new Vector3((float)B[6], (float)B[7], 1);

        // Set the output coefficients in the WarpPespective shader to calculate new UV coordinates for sampling the camera's RenderTexture
        material.SetVector("_M0", M0);
        material.SetVector("_M1", M1);
        material.SetVector("_M2", M2);
    }

    // Start of Gaussian Elimination code from https://rosettacode.org/wiki/Gaussian_elimination available under Creative Commons Attribution-ShareAlike 4.0 International (CC BY-SA 4.0, https://creativecommons.org/licenses/by-sa/4.0/)
    internal class Vector
    {
        private double[] b;
        internal readonly int rows;

        internal Vector(int rows)
        {
            this.rows = rows;
            b = new double[rows];
        }

        internal Vector(double[] initArray)
        {
            b = (double[])initArray.Clone();
            rows = b.Length;
        }

        internal Vector Clone()
        {
            Vector v = new Vector(b);
            return v;
        }

        internal double this[int row]
        {
            get { return b[row]; }
            set { b[row] = value; }
        }

        internal void SwapRows(int r1, int r2)
        {
            if (r1 == r2) return;
            double tmp = b[r1];
            b[r1] = b[r2];
            b[r2] = tmp;
        }

        internal double norm(double[] weights)
        {
            double sum = 0;
            for (int i = 0; i < rows; i++)
            {
                double d = b[i] * weights[i];
                sum += d * d;
            }
            return Math.Sqrt(sum);
        }

        internal void print()
        {
            for (int i = 0; i < rows; i++)
                Debug.Log(b[i]);
        }

        public static Vector operator -(Vector lhs, Vector rhs)
        {
            Vector v = new Vector(lhs.rows);
            for (int i = 0; i < lhs.rows; i++)
                v[i] = lhs[i] - rhs[i];
            return v;
        }
    }

    class Matrix
    {
        private double[] b;
        internal readonly int rows, cols;

        internal Matrix(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            b = new double[rows * cols];
        }

        internal Matrix(int size)
        {
            this.rows = size;
            this.cols = size;
            b = new double[rows * cols];
            for (int i = 0; i < size; i++)
                this[i, i] = 1;
        }

        internal Matrix(int rows, int cols, double[] initArray)
        {
            this.rows = rows;
            this.cols = cols;
            b = (double[])initArray.Clone();
            if (b.Length != rows * cols) throw new Exception("bad init array");
        }

        internal double this[int row, int col]
        {
            get { return b[row * cols + col]; }
            set { b[row * cols + col] = value; }
        }

        public static Vector operator *(Matrix lhs, Vector rhs)
        {
            if (lhs.cols != rhs.rows) throw new Exception("I can't multiply matrix by vector");
            Vector v = new Vector(lhs.rows);
            for (int i = 0; i < lhs.rows; i++)
            {
                double sum = 0;
                for (int j = 0; j < rhs.rows; j++)
                    sum += lhs[i, j] * rhs[j];
                v[i] = sum;
            }
            return v;
        }

        internal void SwapRows(int r1, int r2)
        {
            if (r1 == r2) return;
            int firstR1 = r1 * cols;
            int firstR2 = r2 * cols;
            for (int i = 0; i < cols; i++)
            {
                double tmp = b[firstR1 + i];
                b[firstR1 + i] = b[firstR2 + i];
                b[firstR2 + i] = tmp;
            }
        }

        internal void ElimPartial(Vector B)
        {
            for (int diag = 0; diag < rows; diag++)
            {
                int max_row = diag;
                double max_val = Math.Abs(this[diag, diag]);
                double d;
                for (int row = diag + 1; row < rows; row++)
                    if ((d = Math.Abs(this[row, diag])) > max_val)
                    {
                        max_row = row;
                        max_val = d;
                    }
                SwapRows(diag, max_row);
                B.SwapRows(diag, max_row);
                double invd = 1 / this[diag, diag];
                for (int col = diag; col < cols; col++)
                    this[diag, col] *= invd;
                B[diag] *= invd;
                for (int row = 0; row < rows; row++)
                {
                    d = this[row, diag];
                    if (row != diag)
                    {
                        for (int col = diag; col < cols; col++)
                            this[row, col] -= d * this[diag, col];
                        B[row] -= d * B[diag];
                    }
                }
            }
        }

        internal void print()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    Debug.Log(this[i, j].ToString() + "  ");
            }
        }
    }
}