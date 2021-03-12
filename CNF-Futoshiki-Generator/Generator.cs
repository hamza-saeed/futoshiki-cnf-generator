using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIQuestion6
{
    class Program
    {
        static int N;
        static int N2;
        static int VARS;
        static int clauses = 0;
        static List<String> DIMACS_CNF;
        static int[] solution = { -1, -2, 3, -4, -5, -6, -7, 8, 9, -10, -11, -12, -13, 14, -15, -16, 17, -18, -19, -20, -21, -22, 23, -24, -25, 26, -27, -28, -29, -30, -31, 32, -33, -34, -35, 36, -37, 38, -39, -40, -41, -42, 43, -44, 45, -46, -47, -48, -49, 50, -51, -52, 53, -54, -55, -56, -57, -58, -59, 60, -61, -62, 63, -64 };

        static void Main(string[] args)
        {
            DIMACS_CNF = new List<String>();
            //User inputs N
            Console.WriteLine("Enter N (Number of Rows/Columns): ");
            if ((Int32.TryParse(Console.ReadLine(), out int nInput)) && (nInput > 1))
            {
                N = nInput;
            }
            else
            {
                Console.WriteLine("Incorrect input. Setting N to 4...");
                N = 4;
            }
            //setting n2 and vars
            N2 = N * N;
            VARS = N * N * N;

            addFacts();
            atLeastOneDigitInCell();
            eachDigitAtMostOnesInRow();
            eachDigitAtMostOnesInColumn();
            addContraints();
            print_DIMACS_CNF_format();

            //print sat solution
            printSATSolutionBoard(solution);
            //keep console open
            Console.ReadLine();
        }


        private static void addFacts()
        {
            //Facts
            DIMACS_CNF.Add("c Pre-Assigned entries");
            //Update the number of facts according to the number of added DIMACS CNF clauses
            int facts = 1;
            DIMACS_CNF.Add(toVariable(2, 1, 1) + " 0");
            clauses += facts;
        }


        private static void atLeastOneDigitInCell()
        {
            // Every cell contains at least one digit
            DIMACS_CNF.Add("c Every cell contains at least one digit:");
            String str;
            for (int row = 1; row <= N; row++)
            {
                for (int column = 1; column <= N; column++)
                {
                    str = "";
                    for (int digit = 1; digit <= N; digit++)
                    {
                        str += toVariable(digit, row, column) + " ";
                    }
                    DIMACS_CNF.Add(str + "0");
                    clauses++;
                }
            }
        }

        private static void eachDigitAtMostOnesInRow()
        {
            // Each digit appears at most once in each row
            DIMACS_CNF.Add("c Each digit appears at most once in each row:");
            for (int digit = 1; digit <= N; digit++)
            {
                for (int row = 1; row < N; row++)
                {
                    for (int columnLow = 1; columnLow <= N - 1; columnLow++)
                    {
                        for (int columnHigh = columnLow + 1; columnHigh <= N; columnHigh++)
                        {
                            DIMACS_CNF.Add("-" + toVariable(digit, row, columnLow) + " -" + toVariable(digit, row, columnHigh) + " 0");
                            clauses++;
                        }
                    }
                }
            }
        }

        private static void eachDigitAtMostOnesInColumn()
        {
            // Each number appears at most once in each column
            DIMACS_CNF.Add("c Each number appears at most once in each column:");
            for (int digit = 1; digit <= N; digit++)
            {
                for (int column = 1; column <= N; column++)
                {
                    for (int rowLow = 1; rowLow <= N - 1; rowLow++)
                    {
                        for (int rowHigh = rowLow + 1; rowHigh <= N; rowHigh++)
                        {
                            DIMACS_CNF.Add("-" + toVariable(digit, rowLow, column) + " -" + toVariable(digit, rowHigh, column) + " 0");
                            clauses++;
                        }
                    }
                }
            }
        }

        private static void addContraints()
        {
            //List of constraints. More can be added
            List<constraint> constraints = new List<constraint>()
            {
                new constraint() { lessThanRow = 1, lessThanColumn = 2, greaterThanRow = 2, greaterThanColumn = 2 },
                new constraint() { lessThanRow = 2, lessThanColumn = 2, greaterThanRow = 3, greaterThanColumn = 2 },
                new constraint() { lessThanRow = 3, lessThanColumn = 2, greaterThanRow = 4, greaterThanColumn = 2 },
                new constraint() { lessThanRow = 1, lessThanColumn = 4, greaterThanRow = 2, greaterThanColumn = 4 },
                new constraint() { lessThanRow = 4, lessThanColumn = 1, greaterThanRow = 3, greaterThanColumn = 1 },
               // CONTRADICTORY CONSTRAINT -> new constraint() { lessThanRow = 3, lessThanColumn = 1, greaterThanRow = 4, greaterThanColumn = 1 }
            };
            DIMACS_CNF.Add("c Adding constraints:");
            //Loop through every saved constraint
            foreach (var constraint in constraints)
            {
                //loop through all possibel numebrs
                for (int digitLow = 1; digitLow <= N - 1; digitLow++)
                {
                    //loop through all numbers higher than current digit and add close to ensure state is not met
                    for (int digitHigh = digitLow + 1; digitHigh <= N; digitHigh++)
                    {
                        //add not clause
                        DIMACS_CNF.Add("-" + toVariable(digitLow, constraint.lessThanRow, constraint.lessThanColumn) + " -" + 
                            toVariable(digitHigh, constraint.greaterThanRow, constraint.greaterThanColumn) + " 0");
                        clauses++;
                    }
                }
            }
        }

        private static void print_DIMACS_CNF_format()
        {
            StringBuilder cnfFile = new StringBuilder("");
            //build string with CNF format
            cnfFile.AppendLine("c digit range [1..." + N + "]");
            cnfFile.AppendLine("c row range: [1..." + N + "]");
            cnfFile.AppendLine("c column range: [1..." + N + "]");
            cnfFile.AppendLine("c board[digit][row][column]: variable");
            for (int digit = 1; digit <= N; digit++)
            {
                for (int row = 1; row <= N; row++)
                {
                    for (int column = 1; column <= N; column++)
                    {
                        cnfFile.AppendLine("c board[" + digit + "][" + row + "][" + column + "]: " + toVariable(digit, row, column));
                    }
                }
            }
            cnfFile.AppendLine("c #vars: " + VARS);
            cnfFile.AppendLine("c #clauses: " + clauses);
            cnfFile.AppendLine("p cnf " + VARS + " " + clauses);
            for (int i = 0; i < DIMACS_CNF.Count; i++)
            {
                cnfFile.AppendLine(DIMACS_CNF[i]);
            }
            string fileName = "Futoshiki-" + N + "x" + N + ".cnf";
            //Export CNF file
            System.IO.File.WriteAllText(fileName, cnfFile.ToString());
            Console.WriteLine("The CNF file has successfully been written to " + fileName);
        }

        private static void printSATSolutionBoard(int[] variables)
        {
            int digit;
            int tmp;
            int row;
            int column;

            int[,] tmpBoard = new int[N, N];
            for (row = 0; row < N; row++)
            {
                for (column = 0; column < N; column++)
                {
                    tmpBoard[row, column] = -1;
                }
            }
            for (int i = 0; i < variables.Length; i++)
            {
                if (variables[i] > 0)
                {
                    digit = (variables[i] - 1) / N2; //n2?
                    tmp = (variables[i] - 1) % N2; //n2?
                    row = tmp / N;
                    column = tmp % N;
                    tmpBoard[row, column] = digit;
                }
            }

            Console.WriteLine("=======================");
            Console.WriteLine("===== Given board =====");
            Console.WriteLine("=======================");
            for (row = 0; row < N; row++)
            {
                Console.Write("   ");
                for (column = 0; column < N; column++)
                {
                    Console.Write(((tmpBoard[row, column]) >= 0 ? (tmpBoard[row, column] + 1) + "" : "-") + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("=======================");
        }


        private static int toVariable(int digit, int row, int column)
        {
            return (N2 * (digit - 1) + N * (row - 1) + (column - 1) + 1);
        }

        struct constraint
        {
            public int lessThanRow;
            public int lessThanColumn;
            public int greaterThanRow;
            public int greaterThanColumn;
        }
    }
}
