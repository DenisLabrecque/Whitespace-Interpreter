﻿using System;

namespace WhitespaceInterpreter
{
   class Program
   {
      static void Main(string[] args)
      {
         string helloWorld = "   \t   \t  \n[push-D]   \t  \t\t  \n[push-L]   \t \t  \t \n[push-R]   \t  \t\t\t\t\n[push-O]   \t \t \t\t\t\n[push-W]   \t     \n[push-space]   \t  \t\t\t\t\n[push-O]   \t  \t\t  \n[push-L] \n [duplicate]   \t   \t \t\n[push-E]   \t  \t   \n[push-H]\t\n  [output-letter] \n\n[discard]\t\n  [output-letter] \n\n[discard]\t\n  [output-letter] \n\n[discard]\t\n  [output-letter] \n\n[discard]\t\n  [output-letter] \n\n[discard]\t\n  [output-letter] \n\n[discard]\t\n  [output-letter] \n\n[discard]\t\n  [output-letter] \n\n[discard]\t\n  [output-letter] \n\n[discard]\t\n  [output-letter] \n\n[discard]\t\n  [output-letter]\n\n\n[halt]";
         string heapAccess = "   \t\n[push+1]   \t\t  \n[push+12]\t\t [store]   \t\n[push+1]\t\t\t[retrieve]\n\n\n[halt]";
         Whitespace program = new Whitespace(heapAccess);
         Console.WriteLine(program);
         try
         {
            Console.WriteLine(program.Run(true));
         }
         catch(Exception e)
         {
            Console.WriteLine(e.Message);
         }
      }
   }
}
