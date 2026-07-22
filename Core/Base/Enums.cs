namespace Core.Base
{
    public enum Role
    {
        Admin = 1,
        Center = 2,
        Teacher = 3
    }

    public enum AccountStatus
    {
        Pending = 0,
        Active = 1,
        Deactivated = 2
    }

    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }

    public enum ClassStatus
    {
        Pending = 0,
        OpenForEnrollment = 1,
        ClassStartedButStilOpenForEnrollment = 2,
        EnrollmentClosed = 3,
        Hidden = 4,
        Rejected = 5
    }

    public enum TeachingMethod
    {
        Online = 1,
        InPerson = 2
    }

    public enum Degree
    {
        Bachelor = 1,
        Master = 2,
        PhD = 3,
        AssociateProf = 4,
        Prof = 5,
        Other = 6
    }
}
