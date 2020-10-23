using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CODINGSTANDARDS
{
    //Public Variables should use camel Case
    public int variable;
    public int multiWordVariable;

    //Private Variables are camel case but start with and Underscore;
    private float _multiWordedVariable;
    private bool _facts;

    //Function Naming should follow Pascal Case (Title Case) 
    //Parameter naming should not be confusing with the variable it is meant to reference and should be camel case
    private void FunctionName(int parameterName)
    {

    }

    //One or two line comments can be done with "//"

    
     /* Comments that need multiple lines
     * or more thorough explanation 
     * should use this comment format */


}
