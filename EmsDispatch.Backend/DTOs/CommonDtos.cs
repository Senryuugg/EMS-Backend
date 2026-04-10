namespace EmsDispatch.Backend.DTOs;

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
}

public class RegisterRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class DispatchDto
{
    public string Id { get; set; } = string.Empty;
    public string CallId { get; set; } = string.Empty;
    public PatientInfoDto PatientInfo { get; set; } = new();
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public LocationDto PickupLocation { get; set; } = new();
    public string? AssignedDriverId { get; set; }
    public string? AssignedAmbulanceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PatientInfoDto
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string MedicalCondition { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public List<string> Allergies { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
}

public class LocationDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
}

public class DriverDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string? AmbulanceId { get; set; }
    public LocationDto? CurrentLocation { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string? ActiveDispatchId { get; set; }
}

public class AmbulanceDto
{
    public string Id { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string? DriverId { get; set; }
    public LocationDto? CurrentLocation { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public List<string> Equipment { get; set; } = new();
}

public class HospitalDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public LocationDto Location { get; set; } = new();
    public string Phone { get; set; } = string.Empty;
    public List<string> Specialties { get; set; } = new();
    public int Capacity { get; set; }
    public int CurrentLoad { get; set; }
    public double Rating { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string>? Errors { get; set; }
}
