using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Roslyn.Scripting.CSharp;

namespace PracaInzynierska
{
    class RoslynWorker
    {
        private ISolution solution;
        public List<KeyValuePair<ForStatementSyntax, CommonSyntaxTree>> ForLoops;
        public List<KeyValuePair<WhileStatementSyntax, CommonSyntaxTree>> WhileLoops;
        public IEnumerable<BinaryExpressionSyntax> arrayExpressions;
        public List<CommonSyntaxTree> trees;

        public RoslynWorker(String solutionPath)
        {
            IWorkspace workspace = Workspace.LoadSolution(solutionPath);
            solution = workspace.CurrentSolution;
            trees = new List<CommonSyntaxTree>();
            ForLoops = new List<KeyValuePair<ForStatementSyntax, CommonSyntaxTree>>();
            WhileLoops = new List<KeyValuePair<WhileStatementSyntax, CommonSyntaxTree>>();

            foreach (IProject project in solution.Projects)
            {
                foreach (IDocument document in project.Documents)
                {
                    CommonSyntaxTree tree = document.GetSyntaxTree();
                    trees.Add(tree);
                    CommonSyntaxNode root = tree.GetRoot();
                    IEnumerable<ForStatementSyntax> fors = root.DescendantNodes().OfType<ForStatementSyntax>().
                        Where(e => e.Parent.DescendantNodes().ElementAt(0).Kind != SyntaxKind.ForStatement);

                    IEnumerable<WhileStatementSyntax> whiles = root.DescendantNodes().OfType<WhileStatementSyntax>().
                        Where(e => e.Parent.DescendantNodes().ElementAt(0).Kind != SyntaxKind.WhileStatement);

                    foreach (ForStatementSyntax forStatement in fors)
                    {
                        ForLoops.Add(new KeyValuePair<ForStatementSyntax, CommonSyntaxTree>(forStatement, tree));
                    }

                    foreach (WhileStatementSyntax whileStatement in whiles)
                    {
                        WhileLoops.Add(new KeyValuePair<WhileStatementSyntax, CommonSyntaxTree>(whileStatement, tree));
                    }
                }
            }
        }

        public int GetInnerIterationCount(int index, string loopType)
        {
            var engine = new ScriptEngine();
            var session = engine.CreateSession();
            var innerIterationCount = 0;

            if (loopType == "For")
            {
                var outerFor = ForLoops.ElementAt(index).Key;
                var innerFor = ForLoops.ElementAt(index).Key.DescendantNodes().OfType<ForStatementSyntax>();

                if (innerFor.Count() == 0)
                {
                    innerIterationCount = EvaluateLoop(outerFor, ForLoops.ElementAt(index).Value);
                }
                else
                {
                    var firstInnerFor = innerFor.ElementAt(0);
                    innerIterationCount = EvaluateLoop(firstInnerFor, ForLoops.ElementAt(index).Value);
                }
            }

            if (loopType == "While")
            {
                var outerWhile = WhileLoops.ElementAt(index).Key;
                var innerWhile = WhileLoops.ElementAt(index).Key.DescendantNodes().OfType<WhileStatementSyntax>();

                if (innerWhile.Count() == 0)
                {
                    innerIterationCount = EvaluateLoop(outerWhile, WhileLoops.ElementAt(index).Value);
                }
                else
                {
                    var firstInnerWhile = innerWhile.ElementAt(0);
                    innerIterationCount = EvaluateLoop(firstInnerWhile, WhileLoops.ElementAt(index).Value);
                }
            }
            return innerIterationCount;
        }

        public int GetOuterIterationCount(int index, string loopType)
        {
            var engine = new ScriptEngine();
            var session = engine.CreateSession();
            var outerIterationCount = 0;

            if (loopType == "For")
            {
                var outerFor = ForLoops.ElementAt(index).Key;
                var innerFor = ForLoops.ElementAt(index).Key.DescendantNodes().OfType<ForStatementSyntax>();

                if (innerFor.Count() == 0)
                {
                    outerIterationCount = 1;
                }
                else
                {
                    outerIterationCount = EvaluateLoop(outerFor, ForLoops.ElementAt(index).Value);
                }
            }

            if (loopType == "While")
            {
                var outerWhile = WhileLoops.ElementAt(index).Key;
                var innerWhile = WhileLoops.ElementAt(index).Key.DescendantNodes().OfType<WhileStatementSyntax>();

                if (innerWhile.Count() == 0)
                {
                    outerIterationCount = 1;
                }
                else
                {
                    outerIterationCount = EvaluateLoop(outerWhile, WhileLoops.ElementAt(index).Value);
                }
            }
            return outerIterationCount;
        }

        public String GetInnerIterationDependency(int expressionNumber)
        {
            var dependency = arrayExpressions.ElementAt(expressionNumber).Right.DescendantNodes().OfType<ArgumentSyntax>();
            String dependencyValue = "";

            if (dependency.Count() == 1)
            {
                dependencyValue = dependency.ElementAt(0).Expression.ToString();
            }
            else
            {
                dependencyValue = dependency.ElementAt(1).Expression.ToString();
            }

            return dependencyValue;
        }

        public String GetOuterIterationDependency(int expressionNumber)
        {
            var dependency = arrayExpressions.ElementAt(expressionNumber).Right.DescendantNodes().OfType<ArgumentSyntax>();
            String dependencyValue = "";

            if (dependency.Count() == 1)
            {
                dependencyValue = "0";
            }
            else
            {
                dependencyValue = dependency.ElementAt(0).Expression.ToString();
            }

            return dependencyValue;
        }

        public String GetLeftOuterDependency(int expressionNumber)
        {
            var dependency = arrayExpressions.ElementAt(expressionNumber).Left.DescendantNodes().OfType<ArgumentSyntax>();
            String dependencyValue = "";

            if (dependency.Count() == 1)
            {
                dependencyValue = "0";
            }
            else
            {
                dependencyValue = dependency.ElementAt(0).Expression.ToString();
            }

            return dependencyValue;
        }

        public String GetLeftInnerDependency(int expressionNumber)
        {
            var dependency = arrayExpressions.ElementAt(expressionNumber).Left.DescendantNodes().OfType<ArgumentSyntax>();
            String dependencyValue = "";

            if (dependency.Count() == 1)
            {
                dependencyValue = dependency.ElementAt(0).Expression.ToString();
            }
            else
            {
                dependencyValue = dependency.ElementAt(1).Expression.ToString();
            }

            return dependencyValue;
        }

        public String GetOuterArgumentName(int index, string loopType)
        {
            string returnValue = "";

            if (loopType == "For")
            {
                var outerFor = ForLoops.ElementAt(index).Key;
                var innerFor = ForLoops.ElementAt(index).Key.DescendantNodes().OfType<ForStatementSyntax>();

                if (innerFor.Count() != 0)
                {
                    try
                    {
                        returnValue = outerFor.Declaration.Variables.ElementAt(0).Identifier.ToString();
                    }
                    catch
                    {
                        var value = 11;
                        returnValue = value.ToString();
                    }
                }
            }

            if (loopType == "While")
            {
                var outerWhile = WhileLoops.ElementAt(index).Key;
                var innerWhile = WhileLoops.ElementAt(index).Key.DescendantNodes().OfType<WhileStatementSyntax>();

                if (innerWhile.Count() != 0)
                {
                    try
                    {
                        returnValue = outerWhile.Condition.DescendantNodes().OfType<IdentifierNameSyntax>().ElementAt(0).ToString();
                    }
                    catch
                    {
                        var value = 11;
                        returnValue = value.ToString();
                    }
                }
            }

            return returnValue;
        }

        public String GetInnerArgumentName(int index, string loopType)
        {
            string returnValue = "";

            if (loopType == "For")
            {
                var outerFor = ForLoops.ElementAt(index).Key;
                var innerFor = ForLoops.ElementAt(index).Key.DescendantNodes().OfType<ForStatementSyntax>();

                if (innerFor.Count() == 0)
                {
                    returnValue = outerFor.Declaration.Variables.ElementAt(0).Identifier.ToString();
                }
                else
                {
                    returnValue = innerFor.ElementAt(0).Declaration.Variables.ElementAt(0).Identifier.ToString();
                }
            }

            if (loopType == "While")
            {
                var outerWhile = WhileLoops.ElementAt(index).Key;
                var innerWhile = WhileLoops.ElementAt(index).Key.DescendantNodes().OfType<WhileStatementSyntax>();

                if (innerWhile.Count() == 0)
                {
                    try
                    {
                        returnValue = outerWhile.Condition.DescendantNodes().OfType<IdentifierNameSyntax>().ElementAt(0).ToString();
                    }
                    catch
                    {
                        var value = 11;
                        returnValue = value.ToString();
                    }
                }
                else
                {
                    try
                    {
                        returnValue = innerWhile.ElementAt(0).Condition.DescendantNodes().OfType<IdentifierNameSyntax>().ElementAt(0).ToString();
                    }
                    catch
                    {
                        var value = 11;
                        returnValue = value.ToString();
                    }
                }
            }

            return returnValue;
        }

        public int SetArrayExpressions(int index, string loopType)
        {
            if (loopType == "For")
            {
                var innerFor = ForLoops.ElementAt(index).Key.DescendantNodes().OfType<ForStatementSyntax>();

                if (innerFor.Count() == 0)
                {
                    arrayExpressions = ForLoops.ElementAt(index).Key.Statement.DescendantNodes().OfType<BinaryExpressionSyntax>().
                                        Where(e => (e.Right.Kind == SyntaxKind.ElementAccessExpression) &&
                                        (e.Left.Kind == SyntaxKind.ElementAccessExpression && 
                                        ((ElementAccessExpressionSyntax)e.Right).Expression.ToString() == ((ElementAccessExpressionSyntax)e.Left).Expression.ToString()));
                }
                else
                {
                    arrayExpressions = innerFor.ElementAt(0).Statement.DescendantNodes().OfType<BinaryExpressionSyntax>().
                                        Where(e => (e.Right.Kind == SyntaxKind.ElementAccessExpression) &&
                                        (e.Left.Kind == SyntaxKind.ElementAccessExpression && 
                                        ((ElementAccessExpressionSyntax)e.Right).Expression.ToString() == ((ElementAccessExpressionSyntax)e.Left).Expression.ToString()));
                }
            }

            if (loopType == "While")
            {
                var innerWhile = WhileLoops.ElementAt(index).Key.DescendantNodes().OfType<WhileStatementSyntax>();

                if (innerWhile.Count() == 0)
                {
                    arrayExpressions = WhileLoops.ElementAt(index).Key.Statement.DescendantNodes().OfType<BinaryExpressionSyntax>().
                                        Where(e => (e.Right.Kind == SyntaxKind.ElementAccessExpression) &&
                                        (e.Left.Kind == SyntaxKind.ElementAccessExpression && 
                                        ((ElementAccessExpressionSyntax)e.Right).Expression.ToString() == ((ElementAccessExpressionSyntax)e.Left).Expression.ToString()));
                }
                else
                {
                    arrayExpressions = innerWhile.ElementAt(0).Statement.DescendantNodes().OfType<BinaryExpressionSyntax>().
                                        Where(e => (e.Right.Kind == SyntaxKind.ElementAccessExpression) &&
                                        (e.Left.Kind == SyntaxKind.ElementAccessExpression && 
                                        ((ElementAccessExpressionSyntax)e.Right).Expression.ToString() == ((ElementAccessExpressionSyntax)e.Left).Expression.ToString()));
                }
            }

            return arrayExpressions.Count();
        }

        public void SaveTextRepresentation(int index, string loopType, string fileName)
        {
            List<KeyValuePair<String, String>> dependencies = new List<KeyValuePair<String, String>>();
            List<KeyValuePair<String, String>> leftDependencies = new List<KeyValuePair<String, String>>();

            var innerIterationCount = GetInnerIterationCount(index, loopType);
            var outerIterationCount = GetOuterIterationCount(index, loopType);
            var outerArgumentName = GetOuterArgumentName(index, loopType);
            var innerArgumentName = GetInnerArgumentName(index, loopType);
            SetArrayExpressions(index, loopType);

            for (var i = 0; i < arrayExpressions.Count(); i++)
            {
                dependencies.Add(new KeyValuePair<String, String>(GetOuterIterationDependency(i),
                    GetInnerIterationDependency(i)));
                leftDependencies.Add(new KeyValuePair<String, String>(GetLeftOuterDependency(i),
                    GetLeftInnerDependency(i)));
            }

            List<String> argumentNames = new List<String>();
            argumentNames.Add(outerArgumentName);
            argumentNames.Add(innerArgumentName);

            using (StreamWriter file = new StreamWriter(@"Dependency text\" + fileName + ".txt", false))
            {
                int counter = 0;
                for (var i = 0; i < outerIterationCount; i++)
                {
                    for (var j = 0; j < innerIterationCount; j++)
                    {
                        counter++;
                        file.WriteLine("Iteration: {0},{1} ({2})", i, j, counter);
                        List<int> argumentValues = new List<int>();
                        argumentValues.Add(i);
                        argumentValues.Add(j);

                        for (var k = 0; k < dependencies.Count; k++)
                        {
                            var leftOuter = Evaluate(leftDependencies[k].Key, argumentNames, argumentValues);
                            var outer = Evaluate(dependencies[k].Key, argumentNames, argumentValues);

                            var leftInner = Evaluate(leftDependencies[k].Value, argumentNames, argumentValues);
                            var inner = Evaluate(dependencies[k].Value, argumentNames, argumentValues);

                            argumentValues.Clear();
                            argumentValues.Add(leftOuter);
                            argumentValues.Add(leftInner);
                            argumentValues.Add(outer);
                            argumentValues.Add(inner);
                            string expressionLeft = arrayExpressions.ElementAt(k).Left.ToString();
                            string expressionRight = arrayExpressions.ElementAt(k).Right.ToString();

                            if (leftOuter >= 0 && leftInner >= 0 && outer >= 0 && inner >= 0 &&
                                leftOuter < outerIterationCount && outer < outerIterationCount && leftInner < innerIterationCount && inner < innerIterationCount)
                            {
                                if (outerArgumentName == "")
                                {
                                    expressionLeft = expressionLeft.Replace(arrayExpressions.ElementAt(k).Left.DescendantNodes().
                                    OfType<BracketedArgumentListSyntax>().ElementAt(0).ToString(), "[" + leftInner + "]");
                                    expressionRight = expressionRight.Replace(arrayExpressions.ElementAt(k).Right.DescendantNodes().
                                        OfType<BracketedArgumentListSyntax>().ElementAt(0).ToString(), "[" + inner + "]");
                                }
                                else
                                {
                                    expressionLeft = expressionLeft.Replace(arrayExpressions.ElementAt(k).Left.DescendantNodes().
                                        OfType<BracketedArgumentListSyntax>().ElementAt(0).ToString(), "[" + leftOuter + ", " + leftInner + "]");
                                    expressionRight = expressionRight.Replace(arrayExpressions.ElementAt(k).Right.DescendantNodes().
                                        OfType<BracketedArgumentListSyntax>().ElementAt(0).ToString(), "[" + outer + ", " + inner + "]");
                                }

                                file.WriteLine(expressionLeft + " = " + expressionRight);
                            }

                        }
                        file.WriteLine();
                    }
                }
            }
        }

        private int Evaluate(string expression, List<String> argumentName, List<int> argumentValues)
        {
            for (var i = 0; i < argumentName.Count; i++)
            {
                if (argumentName[i] != "")
                    expression = Regex.Replace(expression, @"[" + argumentName[i] + "]", argumentValues[i].ToString());
            }

            DataTable table = new DataTable();
            table.Columns.Add("expression", typeof(string), expression);
            DataRow row = table.NewRow();
            table.Rows.Add(row);
            return Int32.Parse((string)row["expression"]);
        }

        private string GetReferencedVariables(StatementSyntax loop, CommonSyntaxTree tree)
        {
            string referencedVariablesString = "";
            var identifiers = loop.DescendantNodes().OfType<IdentifierNameSyntax>().GroupBy(e => e.ToString()).Select(g => g.First());

            foreach (var elem in identifiers)
            {
                var declaratorExpressions = tree.GetRoot().DescendantNodes().OfType<LocalDeclarationStatementSyntax>().Where(
                    e => e.Declaration.Variables.First().Identifier.ToString() == elem.ToString() &&
                    e.Span.End < loop.Span.Start);

                if (declaratorExpressions.Count() != 0)
                {
                    referencedVariablesString += declaratorExpressions.ElementAt(0).ToString() + "\n";
                }

                var assignExpressions = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().Where(
                        e => e.Left.ToString() == elem.ToString() && e.Parent.Kind == SyntaxKind.ExpressionStatement &&
                        e.Span.End < loop.Span.Start);

                if (assignExpressions.Count() != 0)
                {
                    var helpfulVariable = assignExpressions.OrderByDescending(e => e.Span.End).Take(1).ElementAt(0);
                    referencedVariablesString += helpfulVariable.ToString() + ";\n";
                }
            }

            return referencedVariablesString;
        }

        private int EvaluateLoop(StatementSyntax loop, CommonSyntaxTree tree)
        {
            var engine = new ScriptEngine();
            var session = engine.CreateSession();
            var referencedVars = GetReferencedVariables(loop, tree);
            var iterationCount = 0;

            try
            {
                session.Execute(referencedVars);
                var body = "int GraphVizIterationCounter = 0;\n";
                if (loop.Kind == SyntaxKind.ForStatement)
                {
                    body += "for (" + ((ForStatementSyntax)loop).Declaration + "; "
                        + ((ForStatementSyntax)loop).Condition + "; "
                        + ((ForStatementSyntax)loop).Incrementors + ")\n";
                    body += "{\nGraphVizIterationCounter++;\n}\n";
                }
                if (loop.Kind == SyntaxKind.WhileStatement)
                {
                    body += "while (" + ((WhileStatementSyntax)loop).Condition + ")\n";
                    body += ((WhileStatementSyntax)loop).Statement;
                    body = body.Remove(body.Length - 1);
                    body += "GraphVizIterationCounter++;\n}\n";
                }
                iterationCount = Int32.Parse(session.Execute(body + "GraphVizIterationCounter").ToString());
            }
            catch
            {
                iterationCount = 10;
            }

            return iterationCount;
        }
    }
}
