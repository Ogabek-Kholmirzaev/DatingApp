namespace API.Extentions;

public static class DateTimeExtensions
{
    public static int CalculateAge(this DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        if (dateOfBirth > today)
        {
            throw new ArgumentException("Date of birth cannot be in the future.");
        }
  
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}