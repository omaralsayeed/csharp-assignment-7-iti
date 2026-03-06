using D7Task.Core;

namespace D7Task.Abstractions;

/// <summary>Notifies observers when exam starts</summary>
public delegate void ExamStartedHandler(object sender, ExamEventArgs e);
