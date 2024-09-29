namespace ProLinked.Infrastructure.Recommendation;

public static class Algorithms
{
    public static double[,] CalculateMatrixFactorization(
        double[,] R,
        int K,
        int maxIterations,
        double a,
        double b)
    {
        var randomNumberGenerator = new Random();
        // Set up userCount and itemCount
        var userCount = R.GetLength(0);
        var itemCount = R.GetLength(1);


        var V = new double[userCount, K];
        for (int  u = 0; u < userCount; u++){
            for (int k = 0; k < K; k++) {
                V[u, k] = randomNumberGenerator.NextDouble();
            }
        }

        var F = new double[K, itemCount];
        for (int k = 0; k < K; k++)
        {
            for (int i = 0; i < itemCount; i++)
            {
                F[k, i] = randomNumberGenerator.NextDouble();
            }
        }

        var columns =
            Enumerable.
                Range(0, K).
                Select(i => 0.0).
                ToArray();
        var rows =
            Enumerable.
                Range(0, K).
                Select(i => 0.0).
                ToArray();

        var err = 0.0;
        var errSquare = 0.0;
        for (int step = 0; step < maxIterations; step++)
        {
            for (int u = 0; u < userCount; u++)
            {
                for (int i = 0; i < itemCount; i++)
                {
                    if (R[u, i] + 1.0 == 0)
                    {
                        continue;
                    }

                    for (int k = 0; k < K; k++)
                    {
                        rows[k] = V[u, k];
                        columns[k] = F[k, i];
                    }

                    err = R[u, i] - Dot(rows, columns, K);
                    for (int k = 0; k < K; k++)
                    {
                        V[u, k] += a * (2 * err * F[k, i] - b * V[u, k]); //Gradient
                        F[k, i] += a * (2 * err * V[u, k] - b * F[k, i]); //Gradient
                    }
                }
            }

            errSquare = 0;
            for (int u = 0; u < userCount; u++)
            {
                for (int i = 0; i < itemCount; i++)
                {
                    if (R[u, i] + 1 == 0)
                    {
                        continue;
                    }
                    for (int k = 0; k < K; k++)
                    {
                        rows[k] = V[u, k];
                        columns[k] = F[k, i];
                    }
                    err = R[u, i] - Dot(rows, columns, K);
                    errSquare += err * err;
                    for (int k = 0; k < K; k++)
                        errSquare += (b / 2) * (V[u, k] * V[u, k] + F[k, i] * F[k, i]);
                }
            }
            if (errSquare <= 0.001) //Precision 10^-3
                break;
        }

        var newR = new double[userCount, itemCount];

        for (int u = 0; u < userCount; u++)
        {
            for (int i = 0; i < itemCount; i++) {
                for (int k = 0; k < K; k++) {
                    rows[k] = V[u,k];
                    columns[k] = F[k,i];
                }
                newR[u,i] = Dot(rows, columns, K);
            }
        }
        return newR;
    }

    private static double Dot(double[] row, double[] col , int K){
        var product = 0.0;
        for (int i = 0; i < K; i++)
        {
            product += row[i] * col[i];
        }
        return product;
    }
}