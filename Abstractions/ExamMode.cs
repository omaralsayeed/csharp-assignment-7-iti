namespace D7Task.Abstractions;

/// <summary>Exam states during lifecycle</summary>
public enum ExamMode
{
    Queued = 1,    // Waiting to start
    Starting = 2,  // Currently running
    Finished = 3   // Completed
}
