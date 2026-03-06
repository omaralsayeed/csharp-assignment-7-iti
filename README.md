# Examination Management System - Design Document

This is a comprehensive Console-Based Examination Management System built with advanced Object-Oriented Programming (OOP) principles and modern C# features. The system supports creating, managing, and conducting different types of exams (Practice and Final) with automatic grading and event-driven student notifications.

---

## Design Decisions

### 1. **Why Abstract Base Class for Exam?**

**Decision:** Use abstract `Exam` class instead of concrete implementations.

**Rationale:**
- **Extensibility:** New exam types (Midterm, Quiz, etc.) can be easily added by inheriting from `Exam`
- **Polymorphism:** `ShowExam()` can behave differently for each exam type without client code knowing the details
- **Code Reuse:** Common exam logic (Start, Finish, CorrectExam, event handling) is centralized
- **Type Safety:** Abstract methods force derived classes to implement required behaviors

**Example:**
```csharp
// Client code works with any Exam type
Exam selectedExam = choice == "2" ? finalExam : practiceExam;
selectedExam.Start();  // Polymorphic behavior
selectedExam.ShowExam();  // Different output per type
```

---

### 2. **Why Use Arrays Instead of List<T>?**

**Decision:** All dynamic collections use `T[]` (arrays) with manual resizing.

**Rationale:**
- **Explicit Control:** Arrays require explicit capacity management, making memory operations transparent
- **Performance:** Direct memory access via index without abstraction overhead
- **Learning Purpose:** Demonstrates understanding of dynamic memory management
- **SOLID Compliance:** `QuestionList` and `Repository<T>` own their array implementation

**Implementation:**
```csharp
private T[] _items;
private int _count;

private void EnsureCapacity(int required)
{
    if (_items.Length >= required) return;
    int newCapacity = _items.Length * 2;  // Exponential growth
    Array.Resize(ref _items, newCapacity);
}
```

**Trade-offs:**
- ✅ More control and understanding
- ❌ More code than using `List<T>`
- ✅ Educational value

---

### 3. **Why Events & Delegates for Notifications?**

**Decision:** Use .NET Event pattern with custom `ExamStartedHandler` delegate.

**Rationale:**
- **Decoupling:** Exam doesn't know about Student; they communicate via events
- **Scalability:** Any number of listeners can subscribe without code changes
- **Standardization:** .NET event pattern is familiar to C# developers
- **Single Responsibility:** Notification logic is separate from exam logic

**Flow:**
```
Exam.Start() → Mode=Starting → OnExamStarted() 
    → ExamStarted event raised → All students notified via OnExamStarted()
```

**Why Not Direct Method Calls?**
- Direct calls would create tight coupling between Exam and Student
- Adding new listeners would require modifying Exam class
- Event pattern provides loose coupling and better maintainability

---

### 4. **Why Generic Repository<T> with Constraints?**

**Decision:** Create `Repository<T> where T : ICloneable, IComparable<T>`

**Rationale:**
- **Type Safety:** Constraint ensures only sortable, cloneable items are added
- **Reusability:** Same repository works for Exam, Student, Question, etc.
- **Sorting:** `IComparable<T>` enables generic QuickSort implementation
- **Cloning:** `ICloneable` enables deep copying of repository items

**Constraint Benefits:**
```csharp
public sealed class Repository<T> where T : ICloneable, IComparable<T>
```
- ✅ Compile-time type safety
- ✅ Generic Sort() can call CompareTo()
- ✅ Clone() method available for all items
- ✅ No casting needed at runtime

---

### 5. **Why QuestionAnswerDictionary Instead of Array?**

**Decision:** Use `Dictionary<Question, Answer>` for student responses.

**Rationale:**
- **Flexible Ordering:** Questions might not be answered in sequence
- **Direct Lookup:** Find student answer for a question in O(1) time
- **Sparse Data:** Not every question needs an answer
- **LINQ-Friendly:** Easy iteration with foreach for correction

**Alternative Rejected:**
- Using array would require mapping question index to answers
- Would lose the semantic connection between question and answer
- More complex indexing logic

---

### 6. **Why IComparable<Exam> Comparison Logic?**

**Decision:** Compare exams by Time first, then by NumberOfQuestions.

**Rationale:**
- **Primary Sort:** Time (duration) is more significant than question count
- **Secondary Sort:** NumberOfQuestions breaks ties
- **Business Logic:** Longer exams generally more important

**Implementation:**
```csharp
public int CompareTo(Exam? other)
{
    int timeComparison = Time.CompareTo(other.Time);
    if (timeComparison != 0) return timeComparison;  // Primary
    return NumberOfQuestions.CompareTo(other.NumberOfQuestions);  // Secondary
}
```

---

### 7. **Why Separate PracticeExam and FinalExam?**

**Decision:** Two distinct classes inheriting from abstract Exam.

**Rationale:**
- **Different Behavior:** Practice shows answers; Final hides them
- **Open/Closed Principle:** Can extend without modifying Exam
- **Single Responsibility:** Each exam type handles its own display logic
- **Polymorphism Demo:** ShowExam() is a perfect example of runtime polymorphism

**Behavior Difference:**
```csharp
// PracticeExam.ShowExam()
Console.WriteLine($"Correct Answer: {correctAnswer}");  // Always shown

// FinalExam.ShowExam()
Console.WriteLine("Correct answers will be revealed after exam closure.");  // Hidden
```

---

### 8. **Why AnswerList as Separate Class?**

**Decision:** Wrap `Answer[]` in `AnswerList` class instead of using array directly.

**Rationale:**
- **Encapsulation:** Hide array resizing logic
- **Indexer Support:** Enable bracket notation `answers[0]`
- **Validation:** Add(Answer) can validate before adding
- **Interface:** Provides clear public API (Add, GetById, Count)

**Benefits:**
- `Question` doesn't manage array details
- Consistent API across the codebase
- Can add features (e.g., Remove, Clear) without changing Question

---

### 9. **Why Question as Abstract Base Class?**

**Decision:** Use abstract `Question` with three concrete implementations.

**Rationale:**
- **Contract Enforcement:** Abstract methods (Display, CheckAnswer) must be implemented
- **Polymorphism:** Different question types behave differently
- **Code Reuse:** Common properties (Header, Body, Marks, AnswerList) in one place
- **Extensibility:** New question types (Essay, Matching, etc.) can be added

**Question Types:**
1. **TrueFalseQuestion:** Binary choice, always 2 answers
2. **ChooseOneQuestion:** Multiple answers, select 1
3. **ChooseAllQuestion:** Multiple answers, select multiple (set comparison)

---

### 10. **Why Subject Class Manages Students?**

**Decision:** `Subject` owns `Student[]` and provides enrollment methods.

**Rationale:**
- **Domain Model:** Subjects logically contain students
- **Notification Hub:** Subject facilitates exam notifications to its students
- **Aggregation:** Subject aggregates Student references
- **Maintainability:** Subject is responsible for its student list

**Responsibilities:**
```csharp
public class Subject
{
    private Student[] _enrolledStudents;  // Composition
    public void Enroll(Student student);  // Add student
    public void NotifyStudents();          // Notify all
    public Student[] GetEnrolledStudents();  // Return copy
}
```

---

### 11. **Why Virtual Methods in Exam?**

**Decision:** `Start()` and `Finish()` are virtual, not abstract.

**Rationale:**
- **Default Implementation:** All exams start and finish the same way (mode change + events)
- **Override Optional:** Derived classes can extend (not replace) behavior
- **Consistency:** All exam types follow same lifecycle

**Example:**
```csharp
public virtual void Start()
{
    Mode = ExamMode.Starting;
    OnExamStarted();  // Trigger event
}

// Derived class can override if needed:
public override void Start()
{
    base.Start();  // Call base behavior
    // Custom behavior here
}
```

---

### 12. **Why ICloneable on Exam?**

**Decision:** Implement `ICloneable` to enable exam duplication.

**Rationale:**
- **Duplication:** Create backup/copy of exam without reference sharing
- **Repository Requirement:** Generic `Repository<T>` requires ICloneable
- **Safety:** Prevent accidental modifications to original exam
- **Testing:** Easy to create test copies

**Implementation:**
```csharp
public virtual object Clone()
{
    var questionsClone = new Question[Questions.Length];
    // ... copy questions ...
    return CreateClone(questionsClone);  // Let subclass handle specifics
}
```

---

## Architecture Patterns

### **1. Template Method Pattern**
`Exam.Clone()` uses template method with abstract `CreateClone()`:
```csharp
public virtual object Clone()  // Template
{
    // Common cloning logic
    return CreateClone(questionsClone);  // Let subclass decide
}

protected abstract Exam CreateClone(Question[] clonedQuestions);  // Template method
```

### **2. Strategy Pattern**
Question types implement different answer-checking strategies:
```csharp
public override bool CheckAnswer(Answer studentAnswer)
{
    // Each question type has different strategy
}
```

### **3. Observer Pattern**
Event system for exam notifications:
```csharp
public event ExamStartedHandler? ExamStarted;
// Students observe exam events
```

### **4. Repository Pattern**
`Repository<T>` provides unified data access:
```csharp
public sealed class Repository<T> where T : ICloneable, IComparable<T>
```

---

## SOLID Principles Applied

### **S - Single Responsibility**
- `Exam`: Manages exam lifecycle
- `Subject`: Manages student enrollment
- `Question`: Represents a question
- `Student`: Represents a student

### **O - Open/Closed**
- `Exam` is open for extension (PracticeExam, FinalExam)
- But closed for modification (new types don't change base)
- `Question` hierarchy allows new question types

### **L - Liskov Substitution**
- Any `Exam` can replace another in Repository
- Any `Question` can replace another in arrays
- Substitution doesn't break functionality

### **I - Interface Segregation**
- `ICloneable`: Only for cloneable objects
- `IComparable<T>`: Only for comparable objects
- `EventArgs`: Specific event data

### **D - Dependency Inversion**
- `Repository<T>` depends on abstractions (`ICloneable`, `IComparable<T>`)
- Not on concrete types
- `Exam` depends on `Subject` abstraction, not implementation

---

## Key Features

### **1. Automatic Exam Correction**
```csharp
public int CorrectExam()
{
    int score = 0;
    foreach (var kvp in QuestionAnswerDictionary)
    {
        if (kvp.Key.CheckAnswer(kvp.Value))
            score += kvp.Key.Marks;
    }
    return score;
}
```

### **2. Event-Driven Notifications**
When exam starts, all enrolled students are notified:
```csharp
private void NotifyAllStudents()
{
    Student[] students = Subject.GetEnrolledStudents();
    for (int i = 0; i < students.Length; i++)
    {
        students[i].OnExamStarted(this, new ExamEventArgs(Subject, this));
    }
}
```

### **3. Generic Sorting**
Exams are sorted using QuickSort with custom comparison:
```csharp
examRepository.Sort();  // Sorts by Time, then Questions
```

### **4. Polymorphic Display**
Different exam types show results differently:
```csharp
selectedExam.ShowExam();  // Behavior depends on actual type
```

---

## File I/O Integration

**QuestionList** logs questions to file:
```csharp
public void Add(Question question)
{
    _items[_count++] = question;
    
    using var writer = new StreamWriter(_logFilePath, append: true);
    writer.WriteLine($"{DateTime.Now:O} | {question}");  // Appends to log
}
```

Questions can be read back:
```csharp
public string[] ReadLogLines()
{
    using var reader = new StreamReader(_logFilePath);
    // ... read and return lines ...
}
```

---

## Validation Strategy

### **Input Validation in Constructors**
All constructors validate inputs:
```csharp
public Exam(int time, int numberOfQuestions, Subject subject)
{
    if (time <= 0) throw new ArgumentOutOfRangeException(nameof(time));
    if (numberOfQuestions <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfQuestions));
    
    Subject = subject ?? throw new ArgumentNullException(nameof(subject));
}
```

### **Null Safety**
Using `ArgumentNullException.ThrowIfNull()`:
```csharp
ArgumentNullException.ThrowIfNull(student);
```

### **Business Logic Validation**
`ChooseOneQuestion` validates correct answer exists:
```csharp
if (answers.GetById(correctAnswer.Id) is null)
    throw new ArgumentException("Correct answer must exist in answer list.");
```

---

## Performance Considerations

### **Array Resizing Strategy**
Exponential growth (double capacity) prevents O(n) resizing on every add:
```csharp
int newCapacity = _items.Length * 2;  // Exponential: 4→8→16→32...
```

### **QuickSort for Sorting**
O(n log n) average case for exam sorting:
```csharp
private void QuickSort(int low, int high)
{
    if (low < high)
    {
        int pi = Partition(low, high);
        QuickSort(low, pi - 1);
        QuickSort(pi + 1, high);
    }
}
```

### **Dictionary for Answer Lookup**
O(1) average lookup for student answers:
```csharp
public Dictionary<Question, Answer> QuestionAnswerDictionary { get; protected set; }
```

---

## Extensibility Points

### **1. Add New Question Types**
```csharp
public sealed class EssayQuestion : Question
{
    public override void Display() { /* ... */ }
    public override bool CheckAnswer(Answer studentAnswer) { /* ... */ }
}
```

### **2. Add New Exam Types**
```csharp
public sealed class MidtermExam : Exam
{
    public override void ShowExam() { /* ... */ }
}
```

### **3. Add New Events**
```csharp
public class Exam
{
    public event EventHandler<EventArgs>? ExamEnded;
    
    public virtual void Finish()
    {
        Mode = ExamMode.Finished;
        ExamEnded?.Invoke(this, EventArgs.Empty);
    }
}
```

### **4. Repository for Any Type**
```csharp
var studentRepo = new Repository<Student>();  // Works if Student implements ICloneable, IComparable<Student>
var questionRepo = new Repository<Question>();  // Works if Question implements those interfaces
```

---

## Testing Considerations

The design enables easy testing:

```csharp
// Easy to test individual components
var subject = new Subject("Math");
var exam = new PracticeExam(30, 3, subject);

// Events can be verified
bool eventRaised = false;
exam.ExamStarted += (s, e) => eventRaised = true;
exam.Start();
Assert.True(eventRaised);

// Sorting can be tested
var repo = new Repository<Exam>();
repo.Add(exam1);
repo.Add(exam2);
repo.Sort();
// Verify sort order
```

---

## Conclusion

This examination system demonstrates enterprise-grade OOP design through:
1. **Inheritance Hierarchies** (Exam, Question, Answer types)
2. **Polymorphism** (ShowExam(), CheckAnswer() behavior varies by type)
3. **Generics with Constraints** (Repository<T> where T : ICloneable, IComparable<T>)
4. **Events & Delegates** (Exam notifications to students)
5. **Interface Implementation** (ICloneable, IComparable<T>, EventArgs)
6. **SOLID Principles** throughout
7. **Design Patterns** (Template Method, Strategy, Observer, Repository)
8. **Array-Based Collections** with explicit memory management
9. **Encapsulation** with validation and protected access

The system is production-ready, maintainable, extensible, and follows modern C# best practices.
