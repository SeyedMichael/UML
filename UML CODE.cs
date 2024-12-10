// -------------------- Entities Layer --------------------
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Resume { get; set; } // User's resume information
}

public class Job
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Company { get; set; }
    public bool IsReviewed { get; set; } // Indicates if the job is reviewed
}

// -------------------- Use Cases Layer --------------------
public interface IUserService
{
    void Register(User user); // Register a new user
    User Login(string username, string password); // User login
    void UpdateUserProfile(int userId, User user); // Update user profile
}

public interface IJobService
{
    void PostJob(Job job); // Post a new job
    IEnumerable<Job> ListJobs(); // Retrieve all jobs
    IEnumerable<Job> SearchJobs(string keyword); // Search for jobs
    void ApplyForJob(int jobId, int userId); // Apply for a job
    void ReviewJob(int jobId); // Mark a job as reviewed
}

// -------------------- Repositories Layer --------------------
public class UserRepository : IUserService
{
    private readonly List<User> _users = new();

    public void Register(User user) => _users.Add(user);

    public User Login(string username, string password) =>
        _users.FirstOrDefault(u => u.Username == username && u.Password == password);

    public void UpdateUserProfile(int userId, User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == userId);
        if (existingUser != null)
        {
            existingUser.Username = user.Username;
            existingUser.Password = user.Password;
            existingUser.Resume = user.Resume;
        }
    }
}

public class JobRepository : IJobService
{
    private readonly List<Job> _jobs = new();
    private readonly Dictionary<int, List<int>> _applications = new(); // jobId -> userId

    public void PostJob(Job job) => _jobs.Add(job);

    public IEnumerable<Job> ListJobs() => _jobs;

    public IEnumerable<Job> SearchJobs(string keyword) =>
        _jobs.Where(j => j.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase));

    public void ApplyForJob(int jobId, int userId)
    {
        if (!_applications.ContainsKey(jobId))
            _applications[jobId] = new List<int>();
        _applications[jobId].Add(userId);
    }

    public void ReviewJob(int jobId)
    {
        var job = _jobs.FirstOrDefault(j => j.Id == jobId);
        if (job != null)
            job.IsReviewed = true;
    }
}

// -------------------- Controllers --------------------
public class JobPortalController
{
    private readonly IUserService _userService;
    private readonly IJobService _jobService;

    public JobPortalController(IUserService userService, IJobService jobService)
    {
        _userService = userService;
        _jobService = jobService;
    }

    // User Operations
    public void RegisterUser(User user) => _userService.Register(user);
    public User LoginUser(string username, string password) => _userService.Login(username, password);
    public void UpdateUserProfile(int userId, User user) => _userService.UpdateUserProfile(userId, user);

    // Job Operations
    public void PostNewJob(Job job) => _jobService.PostJob(job);
    public IEnumerable<Job> GetJobList() => _jobService.ListJobs();
    public IEnumerable<Job> SearchJobs(string keyword) => _jobService.SearchJobs(keyword);
    public void ApplyToJob(int jobId, int userId) => _jobService.ApplyForJob(jobId, userId);
    public void MarkJobAsReviewed(int jobId) => _jobService.ReviewJob(jobId);
}

// -------------------- Example Usage --------------------
var userService = new UserRepository();
var jobService = new JobRepository();
var controller = new JobPortalController(userService, jobService);

// Example: Register a User
var newUser = new User { Id = 1, Username = "Seyed-Sediq", Password = "Seyed4444", Resume = "Experienced in C# Development" };
controller.RegisterUser(newUser);

// Example: Post a Job
var newJob = new Job { Id = 1, Title = "Software Engineer", Description = "Develop high-quality software solutions", Company = "Tech Co." };
controller.PostNewJob(newJob);

// Example: User Login
var loggedInUser = controller.LoginUser("Seyed_Sediq", "Seyed4444");

// Example: Apply for a Job
if (loggedInUser != null)
    controller.ApplyToJob(1, loggedInUser.Id);

// Example: Search Jobs
var searchResults = controller.SearchJobs("Engineer");

// Example: Review a Job
controller.MarkJobAsReviewed(1);
