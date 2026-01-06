using System;
using System.Collections.Generic;

// actual value of the question
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

// logged time for task completion
[Serializable]
public class TaskDurationLog
{
    public string taskIdentifier;
    public string resultData; 

    public TaskDurationLog(string id, string data)
    {
        taskIdentifier = id;
        resultData = data;
    }
}

// questionnaire data
[Serializable]
public class QuestionnaireResult
{
    
    public string condition;
    public int taskNumber;

    public string questionnaireName;
    public string timestamp;
    public List<ResponseItem> responses = new List<ResponseItem>();
}

// participant data
[Serializable]
public class ParticipantData
{
    public string participantID;
    public List<QuestionnaireResult> questionnaireResults = new List<QuestionnaireResult>();
    public List<TaskDurationLog> taskDurations = new List<TaskDurationLog>();
}

// highest level
[Serializable]
public class AllResultsData
{
    public List<ParticipantData> participants = new List<ParticipantData>();
}

// question data for dynamic changes
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

// questionnaire schema
[Serializable]
public class QuestionnaireData
{
    public string questionnaireName;
    public List<QuestionData> questions;
}