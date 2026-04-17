using System;
using System.ComponentModel.DataAnnotations;

namespace EmsDispatch.Backend.Utilities
{
    public static class ValidationUtilities
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidLatitude(double latitude)
        {
            return latitude >= -90 && latitude <= 90;
        }

        public static bool IsValidLongitude(double longitude)
        {
            return longitude >= -180 && longitude <= 180;
        }

        public static bool IsValidCoordinates(double latitude, double longitude)
        {
            return IsValidLatitude(latitude) && IsValidLongitude(longitude);
        }

        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }

        public static string? ValidateDispatchData(dynamic dispatchData)
        {
            if (dispatchData?.patientInfo?.name == null)
                return "Patient name is required";

            if (dispatchData?.patientInfo?.age < 0 || dispatchData?.patientInfo?.age > 150)
                return "Invalid patient age";

            if (dispatchData?.location?.latitude == null || dispatchData?.location?.longitude == null)
                return "Patient location is required";

            double lat = (double)dispatchData!.location!.latitude;
            double lon = (double)dispatchData!.location!.longitude;

            if (!IsValidCoordinates(lat, lon))
                return "Invalid coordinates";

            if (dispatchData?.priority == null)
                return "Priority is required";

            return null; // No validation errors
        }

        public static string? ValidateUserData(string email, string password, string name)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "Email is required";

            if (!IsValidEmail(email))
                return "Invalid email format";

            if (string.IsNullOrWhiteSpace(password))
                return "Password is required";

            if (!IsStrongPassword(password))
                return "Password must be at least 8 characters with uppercase, lowercase, digits, and special characters";

            if (string.IsNullOrWhiteSpace(name))
                return "Name is required";

            return null; // No validation errors
        }
    }
}
