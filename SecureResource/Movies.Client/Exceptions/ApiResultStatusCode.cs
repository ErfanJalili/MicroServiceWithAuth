using System.ComponentModel.DataAnnotations;

namespace Movies.Client.Exceptions
{
    public enum ApiResultStatusCode
    {
        [Display(Name = "Operation Success")]
        Success = 0,

        [Display(Name = "Internal Server Error")]
        ServerError = 1,

        [Display(Name = "Parameters Is Not Valid")]
        BadRequest = 2,

        [Display(Name = "Item Not Found")]
        NotFound = 3,

        [Display(Name = "List is Empty")]
        ListEmpty = 4,

        [Display(Name = "Logic Error")]
        LogicError = 5,

        [Display(Name = "UnAuthorized Access")]
        UnAuthorized = 6
    }
}
