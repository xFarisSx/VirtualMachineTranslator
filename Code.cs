using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMTranslator
{
    public class Code
    {
        Dictionary<string, string> AVARS = new Dictionary<string, string>
        {
            { "local","LCL" },
            { "argument","ARG"},
            { "this","THIS" },
            { "that","THAT" },
            { "temp", "5" },
        };
        string outputF;
        private int labelCounter = 0;

        Dictionary<string, string> arithlogsSym =new Dictionary<string, string> {
            { "add" , "+" },
            { "sub", "-" },
            { "neg", "-" },
            { "eq", "-" },
            { "gt", "-" },
                              { "lt", "-" },
                              { "and", "&" },
                              { "or", "|" },
                              { "not", "!" }};

        public Code(string outputF) {
            this.outputF = Path.GetFileName(outputF);
        
        }
        public string WriteAr(string com)
        {
            labelCounter++;
            string trueLabel = $"EQUAL_TRUE_{labelCounter}";
            string falseLabel = $"EQUAL_FALSE_{labelCounter}";
            string endLabel = $"END_{labelCounter}";
            if (com == "neg" || com == "not") {
                return $@"
@SP
M=M-1
A=M
M={arithlogsSym[com]}M
@SP
M=M+1";
            }
            else if (com == "eq" || com=="gt" || com=="lt")
            {
                return $@"
@SP
M=M-1
A=M
D=M
@RES
M=D
@SP
M=M-1
A=M
D=M
@RES
M=D{arithlogsSym[com]}M
D=M
@{trueLabel}
D;{(com == "eq" ? "JEQ" : com == "gt" ? "JGT" : "JLT")}
@{falseLabel}
0;JMP

({trueLabel})
@SP
A=M
M=-1
@SP
M=M+1
@{endLabel}
0;JMP

({falseLabel})
@SP
A=M
M=0
@SP
M=M+1

({endLabel})
@SP
A=M
M=0";
            }
            return $@"
@SP
M=M-1
A=M
D=M
@RES
M=D
@SP
M=M-1
A=M
D=M
@RES
M=D{arithlogsSym[com]}M
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP
A=M
M=0";
        }

        public string WritePushPop(string type, string arg1, string arg2)
        {
            labelCounter++;
            if (type == "C_PUSH")
            {

                if (arg1 == "constant") { return $@"
@{arg2}
D=A
@SP
A=M
M=D
@SP
M=M+1
@SP
A=M
M=0"; }
                else if (arg1 == "static")
                {
                    return $@"
@{outputF}.{arg2}
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP
A=M
M=0";
                }
                else if (arg1 == "pointer")
                {
                    return $@"
@{(arg2 == "0" ? "THIS" : "THAT")}
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP
A=M
M=0";
                }
                else if (arg1 == "temp")
                {
                    return $@"
@{(int.Parse(AVARS["temp"])+ int.Parse(arg2))}
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP
A=M
M=0";
                }
                else
                {
                    return $@"
@{arg2}
D=A
@{this.AVARS[arg1]}
A=D+M
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP
A=M
M=0";
                }
            }
            else
            {
                if (arg1 == "constant") { return ""; }
                else if (arg1 == "static")
                {
                    return $@"
@SP
M=M-1
A=M
D=M
@{outputF}.{arg2}
M=D";
                }
                else if (arg1 == "pointer")
                {
                    return $@"
@SP
M=M-1
A=M
D=M
@{(arg2 == "0" ? "THIS":"THAT" )}
M=D";
                }
                else if (arg1 == "temp")
                {
                    return $@"
@SP
M=M-1
A=M
D=M
@{int.Parse(AVARS["temp"]) + int.Parse(arg2)}
M=D";
                }
                return $@"
@{AVARS[arg1]}
D=M
@addr
M=D
@{arg2}
D=A
@addr
M=D+M
@SP
M=M-1
A=M
D=M
@addr
A=M
M=D";
            }
        }
    }
}
