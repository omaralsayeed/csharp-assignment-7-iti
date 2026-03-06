using D7Task.Core;
using D7Task.Models;
using D7Task.Questions;
using D7Task.Collections;
using D7Task.Abstractions;

namespace D7Task;

internal class Program
{
    static void Main(string[] args)
    {
        // Create subject and students
        var mathSubject = new Subject("Mathematics");
        Console.WriteLine($"[SUBJECT] {mathSubject.Name} created.\n");

        var student1 = new Student(1, "Ahmed");
        var student2 = new Student(2, "Fatima");
        var student3 = new Student(3, "Hassan");

        mathSubject.Enroll(student1);
        mathSubject.Enroll(student2);
        mathSubject.Enroll(student3);
        Console.WriteLine("[ENROLLMENT] Students enrolled.\n");

        // Create exams
        var practiceExam = new PracticeExam(30, 3, mathSubject);
        var finalExam = new FinalExam(60, 3, mathSubject);

        // Create answers and questions
        var q1 = new TrueFalseQuestion("Q1", "2 + 2 = 4", 5, isTrueCorrect: true);
        var q2 = new ChooseOneQuestion("Q2", "What is 5 * 6?", 5, CreateMultiplyAnswers(), new Answer(3, "30"));
        var q3 = new ChooseAllQuestion("Q3", "Select even numbers.", 10, CreateNumberAnswers(), [1, 3]);

        // Add to exams
        practiceExam.Questions[0] = q1;
        practiceExam.Questions[1] = q2;
        practiceExam.Questions[2] = q3;

        finalExam.Questions[0] = q1;
        finalExam.Questions[1] = q2;
        finalExam.Questions[2] = q3;

        // Repository demo
        Console.WriteLine("========== REPOSITORY DEMO ==========");
        var examRepository = new Repository<Exam>(4);
        examRepository.Add(practiceExam);
        examRepository.Add(finalExam);
        Console.WriteLine($"[REPO] {examRepository.Count} exams added.\n");

        // User selects exam type
        Console.WriteLine("Select Exam Type:");
        Console.WriteLine("1 - Practice");
        Console.WriteLine("2 - Final");
        Console.Write("Enter choice (1 or 2): ");

        string choice = Console.ReadLine() ?? "1";
        Exam selectedExam = choice == "2" ? finalExam : practiceExam;

        Console.WriteLine($"\n[SELECTED] {(selectedExam is PracticeExam ? "Practice" : "Final")} Exam\n");

        // Start exam (triggers events)
        Console.WriteLine("========== STARTING EXAM ==========\n");
        selectedExam.Start();

        // Submit answers
        Console.WriteLine("\n========== STUDENT ANSWERS ==========\n");
        selectedExam.QuestionAnswerDictionary[q1] = new Answer(1, "True");
        selectedExam.QuestionAnswerDictionary[q2] = new Answer(3, "30");
        selectedExam.QuestionAnswerDictionary[q3] = new MultiSelectAnswer(100, "Selected", [1, 3]);
        Console.WriteLine("Answers submitted.\n");

        // Finish and show results
        selectedExam.Finish();
        selectedExam.ShowExam();

        // Sort and display repository
        Console.WriteLine("========== SORTING EXAMS ==========");
        examRepository.Sort();
        Exam[] sortedExams = examRepository.GetAll();
        Console.WriteLine("Sorted by Time, then Questions:\n");
        for (int i = 0; i < sortedExams.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {sortedExams[i]}");
        }

        Console.WriteLine("\n[DONE]");
    }

    static AnswerList CreateMultiplyAnswers()
    {
        var list = new AnswerList(3);
        list.Add(new Answer(1, "25"));
        list.Add(new Answer(2, "20"));
        list.Add(new Answer(3, "30"));
        return list;
    }

    static AnswerList CreateNumberAnswers()
    {
        var list = new AnswerList(4);
        list.Add(new Answer(1, "2"));
        list.Add(new Answer(2, "3"));
        list.Add(new Answer(3, "4"));
        list.Add(new Answer(4, "5"));
        return list;
    }
}
