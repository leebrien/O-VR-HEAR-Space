using System;
using System.Collections.Generic;

[Serializable]
public class ResponseItem
{
    public string id;
    public float score;

    public ResponseItem(string id, float score)
    {
        this.id = id;
        this.score = score;
    }
}

[Serializable]
public class QuestionnaireResult
{
    public string questionnaireName;
    public string timestamp;
    public List<ResponseItem> responses = new List<ResponseItem>();
}

[Serializable]
public class ConditionResult
{
    public string condition; // e.g., "Personalized" or "Standard"
    public int taskNumber;
    public List<QuestionnaireResult> questionnaires = new List<QuestionnaireResult>();
}

[Serializable]
public class ParticipantData
{
    public string participantID;
    public List<ConditionResult> conditionResults = new List<ConditionResult>();
}

[Serializable]
public class AllResultsData
{
    public List<ParticipantData> participants = new List<ParticipantData>();
}


[Serializable]
public class QuestionData
{
    // Common fields
    public string id;
    public string type;
    public string text;
    public string lowLabel;
    public string highLabel;
    // Slider-specific fields
    public float minValue;
    public float maxValue;
    // Radio-specific field
    public int steps;
}

[Serializable]
public class QuestionnaireData
{
    public string questionnaireName;
    public List<QuestionData> questions;
}