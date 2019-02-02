using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Choose_Your_Path
{
    class Algorithm
    {
        private List<GeoCoordinate> Coordinates;
        private List<List<double>> Distance;
        private List<List<double>> M;
        private List<Tuple<int, int>> Result;
        private List<Tuple<int, int>> C;
        private List<int> Rows;
        private List<int> Cols;
        private double min_sol;

        public Algorithm(List<GeoCoordinate> myCoordinates)
        {
            Coordinates = myCoordinates;
            Distance = new List<List<double>>();
            Result = new List<Tuple<int, int>>();
            C = new List<Tuple<int, int>>();
            Rows = new List<int>();
            Cols = new List<int>();
            min_sol = Double.PositiveInfinity;
            GetDistances();

            for (int i = 0; i < Distance.Count; i++)
            {
                Rows.Add(i);
                Cols.Add(i);
            }
        }

        public List<int> Start()
        {
            M = new List<List<double>>();
            M = DeepCopy(Distance);

            C = new List<Tuple<int, int>>();
            C = DeepCopy(Result);

            TraverseTree(M, C, 0, Rows, Cols);
            SortResult();

            List<int> Output = new List<int>();
            for (int i = 0; i < Result.Count; i++)
            {
                if (Result[0].Item1 == 0)
                {
                    Output.Add(Result[i].Item1);
                }
                else
                {
                    Output.Add(Result[i].Item2);
                }
            }
            if (Result[0].Item1 == 0)
            {
                Output.Add(Result[Result.Count - 1].Item2);
            }
            else
            {
                Output.Add(Result[Result.Count - 1].Item1);
            }
            return Output;
        }

        private List<int> DeepCopy(List<int> list)
        {
            List<int> tmp = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                tmp.Add(list[i]);
            }
            return tmp;
        }

        private List<double> DeepCopy(List<double> list)
        {
            List<double> tmp = new List<double>();
            for (int i = 0; i < list.Count; i++)
            {
                tmp.Add(list[i]);
            }
            return tmp;
        }

        private List<Tuple<int, int>> DeepCopy(List<Tuple<int, int>> list)
        {
            List<Tuple<int, int>> tmp = new List<Tuple<int, int>>();
            for (int i = 0; i < list.Count; i++)
            {
                tmp.Add(list[i]);
            }
            return tmp;
        }

        private List<List<double>> DeepCopy(List<List<double>> list)
        {
            List<List<double>> tmp = new List<List<double>>();
            for (int i = 0; i < list.Count; i++)
            {
                tmp.Add(DeepCopy(list[i]));
            }
            return tmp;
        }

        private void SortResult()
        {
            for (int i = 0; i < Result.Count; i++)
            {
                if (Result[i].Item1 == 0)
                {
                    Tuple<int, int> tmp = Result[0];
                    Result[0] = Result[i];
                    Result[i] = tmp;
                    i = Result.Count;
                }
            }
            for (int i = 0; i < Result.Count; i++)
            {
                for (int j = i + 1; j < Result.Count; j++)
                {
                    if (Result[i].Item2 == Result[j].Item1)
                    {
                        Tuple<int, int> tmp = Result[i + 1];
                        Result[i + 1] = Result[j];
                        Result[j] = tmp;
                    }
                }
            }
            Result.RemoveAt(Result.Count-1);
        }

        private double Reduce(List<List<double>> Matrix)
        {
            double r = 0;
            for (int i = 0; i < Matrix.Count; i++)
            {
                double min_row = 0;
                min_row = Matrix[i].Min();
                if (min_row > 0)
                {
                    for (int j = 0; j < Matrix[i].Count; j++)
                    {
                        if (Double.IsPositiveInfinity(Matrix[i][j]))
                        {
                            continue;
                        }
                        Matrix[i][j] -= min_row;
                    }
                    r += min_row;
                }
            }
            for (int i = 0; i < Matrix.Count; i++)
            {
                double min_col = 0;
                List<double> tmp = new List<double>();
                for (int j = 0; j < Matrix[i].Count; j++)
                {
                    tmp.Add(Matrix[j][i]);
                }
                min_col = tmp.Min();
                if (min_col > 0)
                {
                    for (int j = 0; j < Matrix[i].Count; j++)
                    {
                        if (Double.IsPositiveInfinity(Matrix[i][j]))
                        {
                            continue;
                        }
                        Matrix[j][i] -= min_col;
                    }
                    r += min_col;
                }
            }
            return r;
        }

        private double FindEdge(List<List<double>> Matrix, out int r, out int c)
        {
            r = -1;
            c = -1;
            double max = Double.NegativeInfinity;
            for (int i = 0; i < Matrix.Count; i++)
            {
                for (int j = 0; j < Matrix[i].Count; j++)
                {
                    if (Matrix[i][j] == 0)
                    {
                        List<double> tmp_rows = DeepCopy(Matrix[i]);
                        tmp_rows.RemoveAt(j);
                        double min_r = tmp_rows.Min();

                        List<double> tmp_cols = new List<double>();
                        for (int k = 0; k < Matrix.Count; k++)
                        {
                            if (k == i)
                            {
                                continue;
                            }
                            tmp_cols.Add(Matrix[k][j]);
                        }
                        double min_c = tmp_cols.Min();

                        if (min_r + min_c > max)
                        {
                            max = min_r + min_c;
                            r = i;
                            c = j;
                        }
                    }
                }
            }
            return max;
        }

        private void TraverseTree(List<List<double>> Matrix, List<Tuple<int, int>> Couple, double LB, List<int> rows, List<int> cols)
        {
            double r = Reduce(Matrix);
            if (LB + r < min_sol)
            {
                if (Couple.Count == Distance.Count - 2)
                {
                    min_sol = LB + r;

                    if (Matrix[0][1] == Double.PositiveInfinity && Matrix[1][0] == Double.PositiveInfinity || Matrix[0][0] == 0 && Matrix[1][1] == 0)
                    {
                        Couple.Add(new Tuple<int, int>(rows[0], cols[0]));
                        Couple.Add(new Tuple<int, int>(rows[1], cols[1]));
                    }
                    else
                    {
                        if (Matrix[0][0] == Double.PositiveInfinity && Matrix[1][1] == Double.PositiveInfinity || Matrix[0][1] == 0 && Matrix[1][0] == 0)
                        {
                            Couple.Add(new Tuple<int, int>(rows[0], cols[1]));
                            Couple.Add(new Tuple<int, int>(rows[1], cols[0]));
                        }
                    }
                    Result = Couple;
                }
                else
                {
                    int x, y;
                    double max = FindEdge(Matrix, out x, out y);

                    List<Tuple<int, int>> tmp_C = DeepCopy(Couple);
                    tmp_C.Add(new Tuple<int, int>(rows[x], cols[y]));
                    List<int> tmp_rows = DeepCopy(rows);
                    tmp_rows.RemoveAt(x);
                    List<int> tmp_cols = DeepCopy(cols);
                    tmp_cols.RemoveAt(y);
                    List<List<double>> tmp_M = DeepCopy(Matrix);
                    tmp_M[y][x] = Double.PositiveInfinity;
                    for (int i = 0; i < tmp_M.Count; i++)
                    {
                        tmp_M[i].RemoveAt(y);
                    }
                    tmp_M.RemoveAt(x);
                    tmp_M = RemoveCycles(tmp_M, tmp_C, tmp_rows, tmp_cols);

                    TraverseTree(tmp_M, tmp_C, LB + r, tmp_rows, tmp_cols);
                    if (LB + r + max < min_sol)
                    {
                        Matrix[x][y] = Double.PositiveInfinity;
                        TraverseTree(Matrix, Couple, LB + r, rows, cols);
                        Matrix[x][y] = 0;
                    }
                }
            }
        }

        private List<List<double>> RemoveCycles(List<List<double>> Matrix, List<Tuple<int, int>> ActResult, List<int> rows, List<int> cols)
        {
            if (ActResult.Count > 1 && Matrix.Count > 2)
            {
                List<Tuple<int, int>> list = DeepCopy(ActResult);
                for (int j = 1; j < list.Count; j++)
                {
                    j--;
                    Tuple<int, int> test = list[j];
                    list.RemoveAt(j);
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (test.Item1 == list[i].Item2)
                        {
                            test = new Tuple<int, int>(list[i].Item1, test.Item2);
                            list.RemoveAt(i);
                            i = 0;
                            continue;
                        }
                        if (test.Item2 == list[i].Item1)
                        {
                            test = new Tuple<int, int>(test.Item1, list[i].Item2);
                            list.RemoveAt(i);
                            i = 0;
                            continue;
                        }
                    }
                    int x = -1;
                    int y = -1;
                    for (int k = 0; k < rows.Count; k++)
                    {
                        if (rows[k] == test.Item2)
                        {
                            x = k;
                            break;
                        }
                    }
                    if (x != -1)
                    {
                        for (int k = 0; k < cols.Count; k++)
                        {
                            if (cols[k] == test.Item1)
                            {
                                y = k;
                                break;
                            }
                        }
                    }
                    if (x != -1 && y != -1)
                    {
                        Matrix[x][y] = Double.PositiveInfinity;
                    }
                }
            }
            return Matrix;
        }

        private void PrintMatrix(List<List<double>> Matrix)
        {
            string text;
            for (int i = 0; i < Matrix.Count; i++)
            {
                text = "";
                for (int j = 0; j < Matrix[i].Count; j++)
                    text += Matrix[i][j] + " ";
                Debug.WriteLine(text);
            }
        }

        private void GetDistances()
        {
            Distance = new List<List<double>>();

            for (int i = 0; i < Coordinates.Count; i++)
            {
                List<double> tmp = new List<double>();
                for (int j = 0; j < Coordinates.Count; j++)
                {
                    if (i != j)
                    {
                        tmp.Add(GetDistance(Coordinates[i], Coordinates[j]));
                    }
                    else
                    {
                        tmp.Add(Double.PositiveInfinity);
                    }
                }
                Distance.Add(tmp);
            }
        }

        private double GetDistance(GeoCoordinate p1, GeoCoordinate p2)
        {
            return Math.Sqrt((p1.Latitude - p2.Latitude) * (p1.Latitude - p2.Latitude) + (p1.Longitude - p2.Longitude) * (p1.Longitude - p2.Longitude));
        }

    }
}
