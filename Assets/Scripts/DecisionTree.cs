using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EVENT_OUTCOME
{
    NEXT_DECISION = 0,
    GOLD = 1,
    WEAPON = 2,
    BATTLE = 3,
    LOSECREW = 4,
    NONE= 5
}

public class Decision
{
    public string _optionText;
    public EVENT_OUTCOME _outcome;

    public Decision( string optionText,EVENT_OUTCOME outcome){
        _outcome = outcome;
        _optionText = optionText;
    }
}

public class DecisionTree : MonoBehaviour
{

    public GameObject _decisionPanel;
    public Ship _ship;
    public DecisionTree _nextDecision;
    public string _eventDesciption;

    public Decision _option1;
    public Decision _option2;

    public void DisplayDecision(){
        
    }

    public DecisionTree(string eventDesciption, Decision decision1, Decision decision2, DecisionTree next)
    {
        _eventDesciption = eventDesciption;
        _option1 = decision1;
        _option2 = decision2;
        _nextDecision = next;
    }

    public void ResolveDecision(int option)
    {
        if (option == 1)
        {
            ResolvOutcome(_option1._outcome);
        }
        if (option == 2)
        {
            ResolvOutcome(_option2._outcome);
        }
    }

    public void ResolvOutcome(EVENT_OUTCOME outcome)
    {
        if (outcome == EVENT_OUTCOME.BATTLE){

        }
        // switch (outcome)
        // {
        //     case outcome == EVENT_OUTCOME.BATTLE:
        //         break;
        //     case outcome == EVENT_OUTCOME.GOLD:
        //         break;
        //     case outcome == EVENT_OUTCOME.NEXT_DECISION:
        //         break;
        //     case outcome == EVENT_OUTCOME.WEAPON:
        //         break;
        //     default:
        //         break;
        // }
    }
}
