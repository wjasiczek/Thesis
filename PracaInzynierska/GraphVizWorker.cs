using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;


namespace PracaInzynierska
{
    class GraphVizWorker
    {
        private const string LIB_GVC = "gvc.dll";
        private const string LIB_GRAPH = "graph.dll";

        [DllImport(LIB_GVC, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr gvContext();

        [DllImport(LIB_GVC, CallingConvention = CallingConvention.Cdecl)]
        private static extern int gvFreeContext(IntPtr gvc);

        [DllImport(LIB_GRAPH, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agmemread(string data);

        [DllImport(LIB_GRAPH, CallingConvention = CallingConvention.Cdecl)]
        private static extern void agclose(IntPtr g);

        [DllImport(LIB_GVC, CallingConvention = CallingConvention.Cdecl)]
        private static extern int gvLayout(IntPtr gvc, IntPtr g, string engine);

        [DllImport(LIB_GVC, CallingConvention = CallingConvention.Cdecl)]
        private static extern int gvFreeLayout(IntPtr gvc, IntPtr g);

        [DllImport(LIB_GVC, CallingConvention = CallingConvention.Cdecl)]
        private static extern int gvRenderFilename(IntPtr gvc, IntPtr g,
            string format, string fileName);

        [DllImport(LIB_GVC, CallingConvention = CallingConvention.Cdecl)]
        private static extern int gvRenderData(IntPtr gvc, IntPtr g,
            string format, out IntPtr result, out int length);

        [DllImport(LIB_GRAPH, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agopen(string name, int kind);

        [DllImport(LIB_GRAPH, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agsubg(IntPtr g, string name);

        [DllImport(LIB_GRAPH, CallingConvention = CallingConvention.Cdecl)]
        private static extern int agsafeset(IntPtr obj, string name, string value, string def);

        [DllImport(LIB_GRAPH, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agnode(IntPtr g, string name);

        [DllImport(LIB_GRAPH, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agedge(IntPtr g, IntPtr tail, IntPtr head);


        public static void GenerateGraph(int outerIterationCount, int innerIterationCount, String fileName,
            List<KeyValuePair<String, String>> dependencies, String outerArgumentName,
            String innerArgumentName, List<KeyValuePair<String, String>> leftDependencies, bool isCutHorizontal, 
            bool isCutVertical, bool overlap, bool isSmall)
        {
            List<IntPtr> nodeList = new List<IntPtr>();
            IntPtr gvc = gvContext();
            IntPtr graph = agopen("DependencyGraph", 1);
            agsafeset(graph, "outputorder", "edgesfirst", "");
            agsafeset(graph, "nodesep", "0.5", "");
           
            if (overlap)
                agsafeset(graph, "splines", "false", "");
            else
                agsafeset(graph, "splines", "ortho", "");

            agsafeset(graph, "rankdir", "BT", "");

            AddInvisibleEdges(ref graph, ref nodeList, outerIterationCount, innerIterationCount, isCutHorizontal, isCutVertical, isSmall);

            for (var i = 0; i < dependencies.Count; i++)
            {
                AddEdges(ref graph, ref nodeList, dependencies[i].Key, dependencies[i].Value, innerIterationCount, outerIterationCount,
                    outerArgumentName, innerArgumentName, leftDependencies[i].Key, leftDependencies[i].Value);
            }


            gvLayout(gvc, graph, "dot");

            gvRenderFilename(gvc, graph, "png", @"Dependency graphs\" + fileName + ".png");

            gvFreeLayout(gvc, graph);

            agclose(graph);
        }

        private static void AddInvisibleEdges(ref IntPtr graph, ref List<IntPtr> nodeList, int outerIterationCount, 
            int innerIterationCount, bool isCutHorizontal, bool isCutVertical, bool isSmall)
        {
            List<IntPtr> horizontalNodeList = new List<IntPtr>();
            List<IntPtr> verticalNodeList = new List<IntPtr>();
            List<IntPtr> horizontalNumberList = new List<IntPtr>();
            List<IntPtr> verticalNumberList = new List<IntPtr>();
            IntPtr subgraphVertical = new IntPtr();

            for (var i = 0; i < outerIterationCount; i++)
            {
                IntPtr subgraph = agsubg(graph, "subgraph" + i.ToString());
                agsafeset(subgraph, "rank", "same", "");

                if (i == outerIterationCount - 1)
                {
                    subgraphVertical = agsubg(graph, "subgraph" + (i + 1).ToString());
                    agsafeset(subgraphVertical, "rank", "same", "");
                }

                for (var j = 0; j < innerIterationCount; j++)
                {
                    IntPtr node = agnode(subgraph, i.ToString() + ", " + j.ToString());
                    nodeList.Add(node);
                    agsafeset(node, "style", "filled", "");
                    if (isSmall)
                    {
                        agsafeset(node, "shape", "point", "");
                    }
                    if (i > 0)
                    {
                        IntPtr edge = agedge(graph, nodeList[i * innerIterationCount + j], nodeList[(i - 1) * innerIterationCount + j]);
                        agsafeset(edge, "style", "invis", "");
                    }
                    if (j > 0)
                    {
                        IntPtr edge = agedge(graph, nodeList[i * innerIterationCount + j - 1], nodeList[i * innerIterationCount + j]);
                        agsafeset(edge, "style", "invis", "");
                    }
                    if (isCutHorizontal)
                    {
                        if (j == innerIterationCount - 1)
                        {
                            node = agnode(subgraph, i.ToString() +  ", ...");
                            horizontalNodeList.Add(node);
                            agsafeset(node, "style", "filled", "");
                            if (isSmall)
                            {
                                agsafeset(node, "shape", "point", "");
                            }
                            if (i > 0)
                            {
                                IntPtr edge = agedge(graph, horizontalNodeList[i], horizontalNodeList[i - 1]);
                                agsafeset(edge, "style", "invis", "");
                            }
                            if (j > 0)
                            {
                                IntPtr edge = agedge(graph, nodeList[i * innerIterationCount + j], horizontalNodeList[i]);
                                agsafeset(edge, "style", "invis", "");
                            }
                        }
                    }
                    if (isCutVertical)
                    {
                        if (i == outerIterationCount - 1)
                        {
                            node = agnode(subgraphVertical, "... ," + j.ToString());
                            verticalNodeList.Add(node);
                            agsafeset(node, "style", "filled", "");
                            if (isSmall)
                            {
                                agsafeset(node, "shape", "point", "");
                            }
                            if (i > 0)
                            {
                                IntPtr edge = agedge(graph, verticalNodeList[j], nodeList[i * innerIterationCount + j]);
                                agsafeset(edge, "style", "invis", "");
                            }
                            if (j > 0)
                            {
                                IntPtr edge = agedge(graph, verticalNodeList[j - 1], verticalNodeList[j]);
                                agsafeset(edge, "style", "invis", "");
                            }
                        }
                    }
                    if (isCutHorizontal && isCutVertical)
                    {
                        if (i == outerIterationCount - 1 && j == innerIterationCount - 1)
                        {
                            node = agnode(subgraphVertical, "... , ...");
                            if (isSmall)
                            {
                                agsafeset(node, "shape", "point", "");
                            }
                            agsafeset(node, "style", "filled", "");
                            IntPtr edge = agedge(graph, node, horizontalNodeList[i]);
                            agsafeset(edge, "style", "invis", "");
                            IntPtr edge2 = agedge(graph, verticalNodeList[j], node);
                            agsafeset(edge2, "style", "invis", "");
                        }
                    }
                }


                if (isSmall)
                {
                    IntPtr node = agnode(subgraph, (i + 20).ToString());
                    verticalNumberList.Add(node);
                    agsafeset(node, "shape", "none", "");
                    agsafeset(node, "style", "filled", "");
                    agsafeset(node, "label", i.ToString(), "");
                    IntPtr edge = agedge(graph, node, nodeList[i * innerIterationCount]);
                    agsafeset(edge, "style", "invis", "");
                    if (i > 0)
                    {
                        IntPtr edge1 = agedge(graph, node, verticalNumberList[i - 1]);
                        agsafeset(edge1, "style", "invis", "");
                    }

                    if (i == outerIterationCount - 1 && isCutVertical)
                    {
                        IntPtr node1 = agnode(subgraphVertical, (outerIterationCount + 20).ToString());
                        agsafeset(node1, "shape", "none", "");
                        agsafeset(node1, "style", "filled", "");
                        agsafeset(node1, "label", "...", "");
                        IntPtr edge1 = agedge(graph, node1, verticalNodeList[0]);
                        agsafeset(edge1, "style", "invis", "");
                        IntPtr edge2 = agedge(graph, node1, verticalNumberList[i]);
                        agsafeset(edge2, "style", "invis", "");
                    }
                }
            }

            if (isSmall)
            {
                IntPtr subgraph = agsubg(graph, "subgraph horizontalNumbers");
                agsafeset(subgraph, "rank", "same", "");

                for (var i = 0; i < innerIterationCount; i++)
                {
                    IntPtr node = agnode(subgraph, i.ToString());
                    horizontalNumberList.Add(node);
                    agsafeset(node, "shape", "none", "");
                    agsafeset(node, "style", "filled", "");
                    IntPtr edge = agedge(graph, nodeList[i], node);
                    agsafeset(edge, "style", "invis", "");
                    if (i > 0)
                    {
                        IntPtr edge1 = agedge(graph, horizontalNumberList[i - 1], node);
                        agsafeset(edge1, "style", "invis", "");
                    }
                }

                if (isCutHorizontal)
                {
                    IntPtr node = agnode(subgraph, "...");
                    agsafeset(node, "shape", "none", "");
                    agsafeset(node, "style", "filled", "");
                    IntPtr edge = agedge(graph, horizontalNodeList[0], node);
                    agsafeset(edge, "style", "invis", "");
                    IntPtr edge1 = agedge(graph, horizontalNumberList[innerIterationCount - 1], node);
                    agsafeset(edge1, "style", "invis", "");
                }
            }
        }

        public static void AddEdges(ref IntPtr graph, ref List<IntPtr> nodeList, String dependencyOuter, String dependencyInner, 
            int innerIterationCount, int outerIterationCount, String outerArgumentName, String innerArgumentName,
            string leftOuterDependency, string leftInnerDependency)
        {
            List<String> argumentNames = new List<String>();
            argumentNames.Add(outerArgumentName);
            argumentNames.Add(innerArgumentName);
            string[] colorsArray = new string[] { "black", "blue", "forestgreen", "red", "dodgerblue", "deeppink", "darkviolet", "darkgoldenrod4", "magenta", "brown"};

            for (var i = 0; i < outerIterationCount; i++)
            {
                for (var j = 0; j < innerIterationCount; j++)
                {
                    List<int> argumentValues = new List<int>();
                    argumentValues.Add(i);
                    argumentValues.Add(j);

                    var leftVertical = Evaluate(leftOuterDependency, argumentNames, argumentValues);
                    var vertical = Evaluate(dependencyOuter, argumentNames, argumentValues); 
                    
                    var leftHorizontal = Evaluate(leftInnerDependency, argumentNames, argumentValues);
                    var horizontal = Evaluate(dependencyInner, argumentNames, argumentValues);

                  
                    if (vertical < 0 || leftVertical < 0 || horizontal < 0 || leftHorizontal < 0)
                            continue;
                    
                    if (outerArgumentName != "" && dependencyOuter == "0")
                    {
                        if (vertical >= nodeList.Count || leftVertical >= nodeList.Count ||
                            horizontal >= nodeList.Count || leftHorizontal >= nodeList.Count)
                            break;

                        IntPtr edge = agedge(graph, nodeList[leftVertical + leftHorizontal], nodeList[vertical + horizontal]);
                        agsafeset(edge, "constraint", "false", "");
                        agsafeset(edge, "penwidth", "1.5", "");
                        if (outerIterationCount == 1)
                            agsafeset(edge, "color", colorsArray[j], "");
                        else
                            agsafeset(edge, "color", colorsArray[i], "");
                    }
                    else
                    {
                        if (vertical >= outerIterationCount || leftVertical >= outerIterationCount ||
                            horizontal >= innerIterationCount || leftHorizontal >= innerIterationCount)
                            break;

                        IntPtr edge = agedge(graph, nodeList[leftVertical * innerIterationCount + leftHorizontal], nodeList[vertical * innerIterationCount + horizontal]);
                        agsafeset(edge, "constraint", "false", "");
                        agsafeset(edge, "penwidth", "1.5", "");
                        if (outerIterationCount == 1)
                            agsafeset(edge, "color", colorsArray[j], "");
                        else
                            agsafeset(edge, "color", colorsArray[i], "");
                    }                
                }
            }
        }

        public static int Evaluate(string expression, List<String> argumentName, List<int> argumentValues)
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
    }
}
