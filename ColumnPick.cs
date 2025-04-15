// Copyright (C) 2025 Akil Woolfolk Sr. 
// All Rights Reserved
// All the changes released under the MIT license as the original code.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Reflection;
// need to revisit the previous using statement

namespace ColumnPick
{
    class CPMain
    {
        struct CMDArguments
        {
            public bool bParseCmdArguments;
            public string strSourceFileName;
            public string strDestinationFileName;
            public int intColumn;
            public bool bInteractiveMode;
            public int intDelimiter; // 1 = tab, 2 = comma
        }

        struct FileDataParameters
        {
            public string strSourceFileName;
            public string strDestinationFileName;
            public int intColumn;
            public char chrDelimiter;
        }

        static void funcPrintParameterSyntax()
        {
            Console.WriteLine("ColumnPick v1.0);
            Console.WriteLine();
            Console.WriteLine("Description: Extract a single colum from a delimited source file");
            Console.WriteLine();
            Console.WriteLine("Parameter syntax:");
            Console.WriteLine();
            Console.WriteLine("Use the following required parameters in the following order:");
            Console.WriteLine("-run                required parameter");
            Console.WriteLine("-src:               to specify the source filename");
            Console.WriteLine("-dest:              to specify the destination filename");
            Console.WriteLine();
            Console.WriteLine("Use one of the following required parameters:");
            Console.WriteLine("-i                  to enter interactive mode to choose the column");
            Console.WriteLine("-col:               to specify the column to pick from the source file");
            Console.WriteLine();
            Console.WriteLine("Use one of the following required parameters:");
            Console.WriteLine("-t                  to specify input file is tab-delimited");
            Console.WriteLine("-c                  to specify input file is comma-delimited");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("ColumnPick -run -src:input.txt -dest:output.txt -col:1 -t");
            Console.WriteLine("ColumnPick -run -src:input.txt -dest:output.txt -i -c");
        }

        static CMDArguments funcParseCmdArguments(string[] cmdargs)
        {
            CMDArguments objCMDArguments = new CMDArguments();

            try
            {
                bool bCmdArg1Complete = false;
                bool bCmdArg2Complete = false;
                bool bCmdArg3Complete = false;

                if (cmdargs[0] == "-run" & cmdargs.Length > 1)
                {
                    if (cmdargs[1].Contains("-src:"))
                    {
                        // [DebugLine] Console.WriteLine(cmdargs[1].Substring());
                        objCMDArguments.strSourceFileName = cmdargs[1].Substring(5);
                        bCmdArg1Complete = true;

                        if (bCmdArg1Complete & cmdargs.Length > 2)
                        {
                            if (cmdargs[2].Contains("-dest:"))
                            {
                                // [DebugLine] Console.WriteLine(cmdargs[2].Substring());
                                objCMDArguments.strDestinationFileName = cmdargs[2].Substring(6);
                                bCmdArg2Complete = true;

                                if (bCmdArg2Complete & cmdargs.Length > 3)
                                {
                                    if (cmdargs[3] == "-i" | cmdargs[3].Contains("-col:"))
                                    {
                                        if (cmdargs[3] == "-i")
                                        {
                                            // [DebugLine] Console.WriteLine(cmdargs[3].Substring());
                                            objCMDArguments.bInteractiveMode = true;
                                            bCmdArg3Complete = true;
                                        }
                                        if (cmdargs[3].Contains("-col:"))
                                        {
                                            // [DebugLine] Console.WriteLine(cmdargs[3].Substring());
                                            objCMDArguments.bInteractiveMode = false;
                                            objCMDArguments.intColumn = Int32.Parse(cmdargs[3].Substring(5));
                                            bCmdArg3Complete = true;
                                        }
                                        if (bCmdArg3Complete & cmdargs.Length > 4)
                                        {
                                            // [DebugLine] Console.WriteLine("cmdarg4: {0}",cmdargs[4]);
                                            if (cmdargs[4] == "-t" | cmdargs[4] == "-c")
                                            {
                                                if (cmdargs[4] == "-t")
                                                {
                                                    objCMDArguments.intDelimiter = 1;
                                                    objCMDArguments.bParseCmdArguments = true;
                                                }
                                                if (cmdargs[4] == "-c")
                                                {
                                                    objCMDArguments.intDelimiter = 2;
                                                    objCMDArguments.bParseCmdArguments = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    objCMDArguments.bParseCmdArguments = false;
                }
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }

            return objCMDArguments;
        }

        static void funcProgramExecution(CMDArguments objCMDArguments2)
        {
            try
            {
                Construct.ProgramRegistryTag(Construct.strProgramName);

                char chrDelimiter;
                int intColumnPick = objCMDArguments2.intColumn;

                FileDataParameters sFileDataParameters;

                // [DebugLine] Console.WriteLine(intColumnPick.ToString());
                // [DebugLine] Console.WriteLine("intDelimiter: {0}",objCMDArguments2.intDelimiter.ToString());

                sFileDataParameters.strSourceFileName = objCMDArguments2.strSourceFileName;
                sFileDataParameters.strDestinationFileName = objCMDArguments2.strDestinationFileName;
                sFileDataParameters.intColumn = objCMDArguments2.intColumn;

                if (objCMDArguments2.intDelimiter == 1)
                {
                    chrDelimiter = '\t';
                    sFileDataParameters.chrDelimiter = '\t';
                }
                else
                {
                    chrDelimiter = ',';
                    sFileDataParameters.chrDelimiter = ',';
                }

                if (Construct.CheckForFile(objCMDArguments2.strSourceFileName))
                {
                    if (funcCheckFileRowsForDelimiter(sFileDataParameters))
                    {
                        if (!objCMDArguments2.bInteractiveMode)
                        {
                            funcProcessFiles(objCMDArguments2.strSourceFileName, objCMDArguments2.strDestinationFileName, false, chrDelimiter, intColumnPick);
                        }
                        else
                        {
                            int intColumnValue = funcGetColumnSelection(sFileDataParameters.strSourceFileName, sFileDataParameters.chrDelimiter);
                            // [DebugLine] Console.WriteLine(intColumnValue.ToString());
                            funcProcessFiles(objCMDArguments2.strSourceFileName, objCMDArguments2.strDestinationFileName, false, chrDelimiter, intColumnValue);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }

        }

        static int funcGetColumnSelection(string strInputFile, char chrHeaderRowDelimiter)
        {
            int intSelectedColumn = 0;

            try
            {
                if (Construct.CheckForFile(strInputFile))
                {
                    TextReader trSourceFile = new StreamReader(strInputFile);

                    string[] strarrHeaderRow = trSourceFile.ReadLine().Split(chrHeaderRowDelimiter);

                    funcPrintColumnSelect(strarrHeaderRow);

                    string strConsoleInput = Console.ReadLine();

                    // [DebugLine] Console.WriteLine("inputlen: {0}",strConsoleInput.Length.ToString());

                    intSelectedColumn = Int32.Parse(strConsoleInput);

                    trSourceFile.Close();
                }

            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }

            return intSelectedColumn;
        }

        static void funcPrintColumnSelect(string[] arrColumnSelect)
        {
            try
            {
                int intColumnSelection = 1;

                Console.WriteLine("Select one of the following columns:");
                Console.WriteLine();

                foreach (string strHeaderElement in arrColumnSelect)
                {
                    Console.WriteLine("Enter {0} for column {1}", intColumnSelection, strHeaderElement);
                    intColumnSelection++;
                }
                Console.WriteLine();

                Console.Write("Select:");
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }

        }

        static void funcProcessFiles(string strInputFile, string strDestinationFile, bool bGetHeaderRow, char chrDelimiter, int intColumnPick)
        {
            try
            {
                TextReader trSourceFile = new StreamReader(strInputFile);

                TextWriter twDestinationFile = new StreamWriter(strDestinationFile);

                using (trSourceFile)
                {
                    string strLine = "";

                    while ((strLine = trSourceFile.ReadLine()) != null)
                    {
                        string[] strNewLine = strLine.Split(chrDelimiter);
                        // [DebugLine] Console.WriteLine("strNewLine length: {0}",strNewLine.Length.ToString());
                        if (intColumnPick <= strNewLine.Length)
                        {
                            twDestinationFile.WriteLine(strNewLine[intColumnPick - 1]);
                        }
                    }

                    twDestinationFile.Close();
                }

                trSourceFile.Close();

            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }

        }

        static bool funcCheckFileRowsForDelimiter(FileDataParameters newFileDataParameters)
        {
            bool bFileProperlyDelimited = true;

            try
            {
                int intFirstRowLength = 0;
                int intNewRowLength = 0;

                // [DebugLine] Console.WriteLine("begin function: {0} ", bFileProperlyDelimited.ToString());

                TextReader trSourceFile = new StreamReader(newFileDataParameters.strSourceFileName);

                using (trSourceFile)
                {
                    string strLine = "";

                    string[] strFirstLine = trSourceFile.ReadLine().Split(newFileDataParameters.chrDelimiter);
                    intFirstRowLength = strFirstLine.Length;

                    while ((strLine = trSourceFile.ReadLine()) != null)
                    {
                        string[] strNewLine = strLine.Split(newFileDataParameters.chrDelimiter);
                        intNewRowLength = strNewLine.Length;
                        if (intNewRowLength != intFirstRowLength)
                        {
                            bFileProperlyDelimited = false;
                        }
                        // [DebugLine] Console.WriteLine("firstrowlen: {0}\t newrowlen: {1}", intFirstRowLength, intNewRowLength);
                    }
                }

                trSourceFile.Close();

                if (!bFileProperlyDelimited)
                {
                    Console.WriteLine("The source file does not appear to be delimited properly.");
                }

                // [DebugLine] Console.WriteLine("end function: {0} ", bFileProperlyDelimited.ToString());
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }

            return bFileProperlyDelimited;
        }

        static void funcGetFuncCatchCode(string strFunctionName, Exception currentex)
        {
            string strCatchCode = "";

            Dictionary<string, string> dCatchTable = new Dictionary<string, string>();
            dCatchTable.Add("funcGetFuncCatchCode", "p:f0");
            dCatchTable.Add("funcPrintParameterSyntax", "p:f1");
            dCatchTable.Add("funcParseCmdArguments", "p:f2");
            dCatchTable.Add("funcProgramExecution", "p:f3");
            dCatchTable.Add("funcGetColumnSelection", "p:f4");
            dCatchTable.Add("funcPrintColumnSelect", "p:f5");
            dCatchTable.Add("funcProcessFiles", "p:f6");
            dCatchTable.Add("funcCheckFileRowsForDelimiter", "p:f7");

            if (dCatchTable.ContainsKey(strFunctionName))
            {
                strCatchCode = "err" + dCatchTable[strFunctionName] + ": ";
            }

            //[DebugLine] Console.WriteLine(strCatchCode + currentex.GetType().ToString());
            //[DebugLine] Console.WriteLine(strCatchCode + currentex.Message);

            Construct.WriteToOutputLogFile(strCatchCode + currentex.GetType().ToString());
            Construct.WriteToOutputLogFile(strCatchCode + currentex.Message);
        }

        static void Main(string[] args)
        {
            try
            {
                Construct.strProgramName = "ColumnPick";

                if (args.Length == 0)
                {
                    Construct.PrintParameterWarning(Construct.strProgramName);
                }
                else
                {
                    if (args[0] == "-?")
                    {
                        funcPrintParameterSyntax();
                    }
                    else
                    {
                        string[] arrArgs = args;
                        CMDArguments objArgumentsProcessed = funcParseCmdArguments(arrArgs);

                        if (objArgumentsProcessed.bParseCmdArguments)
                        {
                            funcProgramExecution(objArgumentsProcessed);
                        }
                        else
                        {
                            Construct.PrintParameterWarning(Construct.strProgramName);
                        } // check objArgumentsProcessed.bParseCmdArguments
                    } // check args[0] = "-?"
                } // check args.Length == 0
            }
            catch (Exception ex)
            {
                Console.WriteLine("tacf9: {0}", ex.Message);
            }

        } // Main()
    }
}
